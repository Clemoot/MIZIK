using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(BoxCollider))]
public class PlaneHoleMesh : MonoBehaviour
{
    private static readonly Vector2 MESH_MIN_BOUNDS = Vector2.one * -2.0f;
    private static readonly Vector2 MESH_MAX_BOUNDS = Vector2.one *  2.0f;

    public Vector2 HoleCenter { get; private set; }
    public float MeshSize { get => _meshSize; }
    public float HoleSize { get => _holeSize; }

    [SerializeField]
    [Range(1.0f, 4.0f)]
    private float _meshSize = 2.0f;

    [SerializeField]
    [Range(0.05f, 0.5f)]
    private float _holeSize = 0.05f;

    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private BoxCollider _holeCollider;

    bool _editorUpdateMesh = false;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _holeCollider = GetComponent<BoxCollider>();

        Vector3 holeCenter, holeSize;
        Mesh planeHole = CreatePlaneHoleMesh(out holeCenter, out holeSize);
        _meshFilter.sharedMesh = planeHole;
        _meshCollider.sharedMesh = planeHole;

        HoleCenter = holeCenter;
        ConfigureBoxCollider(holeCenter, holeSize);
    }

    private void Update()
    {
        if (Application.isEditor && _editorUpdateMesh)
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
            _holeCollider = GetComponent<BoxCollider>();

            Vector3 holeCenter, holeSize;
            Mesh planeHole = CreatePlaneHoleMesh(out holeCenter, out holeSize);
            _meshFilter.sharedMesh = planeHole;
            _meshCollider.sharedMesh = planeHole;

            HoleCenter = holeCenter;
            ConfigureBoxCollider(holeCenter, holeSize);

            _editorUpdateMesh = false;
        }
    }

    private void OnValidate()
    {
        if (Application.isEditor)
            _editorUpdateMesh = true;
    }

    #region Génération du mesh

    /// <summary>
    /// Génère le mesh avec le trou
    /// </summary>
    Mesh CreatePlaneHoleMesh(out Vector3 holeCenter, out Vector3 holeSize)
    {
        Mesh mesh = new Mesh();

        // génère les points d'un plan 2D
        Vector3[] planeVertices = GetPlaneVertices();

        // détermine la taille du trou et les bordures à ne pas dépasser
        Vector2 holeSize2D = Vector2.one * _holeSize;
        Vector2 holeMinBounds, holeMaxBounds;
        GetHoleBounds(holeSize2D, out holeMinBounds, out holeMaxBounds);

        Vector2 center = new Vector2(
            Random.Range(holeMinBounds.x, holeMaxBounds.x),
            Random.Range(holeMinBounds.y, holeMaxBounds.y)
            );

        // génère les points du trou
        Vector3[] holeTopVertices, holeBottomVertices;
        GetHoleVertices(center, holeSize2D, out holeTopVertices, out holeBottomVertices);

        holeCenter = new Vector3(center.x, -0.5f, center.y);
        holeSize = new Vector3(holeSize2D.x, 1.0f, holeSize2D.y);

        // triangulation du mesh
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        /**
          * Il est nécessaire d'ajouter plusieurs fois les mêmes vertices dans le vertex buffer.
          * Lors du rendu, les normales du mesh sont interpolées et en utilisant des vertices partagés, cela créerait des problèmes d'éclairage et d'ombres.
          * Le meilleur solution aurait été de réaliser un shader, mais éviter à cause du temps imparti
          */

        vertices.AddRange(planeVertices);
        vertices.AddRange(holeTopVertices);
        vertices.AddRange(holeTopVertices);
        vertices.AddRange(holeBottomVertices);
        vertices.AddRange(holeBottomVertices);

        indices.AddRange(MeshHelpers.TriangulateQuadFace(0, 2, 4, 6));
        indices.AddRange(MeshHelpers.TriangulateQuadFace(6, 2, 7, 3));
        indices.AddRange(MeshHelpers.TriangulateQuadFace(5, 7, 1, 3));
        indices.AddRange(MeshHelpers.TriangulateQuadFace(0, 4, 1, 5));
        indices.AddRange(MeshHelpers.TriangulateQuadFace(8, 10, 12, 14));
        indices.AddRange(MeshHelpers.TriangulateQuadFace(10, 11, 14, 15));
        indices.AddRange(MeshHelpers.TriangulateQuadFace(11, 9, 15, 13));
        indices.AddRange(MeshHelpers.TriangulateQuadFace(9, 8, 13, 12));
        indices.AddRange(MeshHelpers.TriangulateQuadFace(16, 18, 17, 19));

        mesh.SetVertices(vertices.ToArray());
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.ClearBlendShapes();

        mesh.name = "Plane Hole";

        return mesh;
    }

    /// <summary>
    /// Génère les quatre points d'un plan 2D
    /// </summary>
    /// <returns>
    /// Quatre vertices ordonné comme-ci :
    ///     0: Supérieur Gauche
    ///     1: Inférieur Gauche
    ///     2: Supérieur Droite
    ///     3: Inférieur Droite
    /// </returns>
    Vector3[] GetPlaneVertices()
    {
        return new Vector3[]
        {
            new Vector3(-_meshSize, 0.0f, -_meshSize),
            new Vector3(-_meshSize, 0.0f,  _meshSize),
            new Vector3( _meshSize, 0.0f, -_meshSize),
            new Vector3( _meshSize, 0.0f,  _meshSize)
        };
    }

    /// <summary>
    /// Génère les points du trou
    /// </summary>
    /// <param name="center">Centre du trou</param>
    /// <param name="size">Taile du trou</param>
    /// <param name="topVertices">Vertices constituant la surface du trou</param>
    /// <param name="bottomVertices">Vertices constituant le fond du trou</param>
    void GetHoleVertices(Vector2 center, Vector2 size, out Vector3[] topVertices, out Vector3[] bottomVertices)
    {
        Vector3 center3D = new Vector3(center.x, 0.0f, center.y);

        size /= 2;

        topVertices = new Vector3[]
        {
            center3D + new Vector3(-size.x,  0.0f, -size.y),
            center3D + new Vector3(-size.x,  0.0f,  size.y),
            center3D + new Vector3( size.x,  0.0f, -size.y),
            center3D + new Vector3( size.x,  0.0f,  size.y)
        };
        bottomVertices = new Vector3[] { 
            center3D + new Vector3(-size.x, -1.0f, -size.y),
            center3D + new Vector3(-size.x, -1.0f,  size.y),
            center3D + new Vector3( size.x, -1.0f, -size.y),
            center3D + new Vector3( size.x, -1.0f,  size.y)
        };
    }

    /// <summary>
    /// Détermine les bordures pour contenir le trou dans les bordures min et max
    /// </summary>
    /// <param name="holeSize">Taille du trou</param>
    /// <param name="min">Bordure inférieure du plan 2D</param>
    /// <param name="max">Bordure supérieure du plan 2D</param>
    void GetHoleBounds(Vector2 holeSize, out Vector2 min, out Vector2 max)
    {
        min = -Vector2.one * _meshSize + holeSize / 2;
        max = Vector2.one * _meshSize - holeSize / 2;
    }

    #endregion

    void ConfigureBoxCollider(Vector3 holeCenter, Vector3 holeSize)
    {
        _holeCollider.center = holeCenter;
        _holeCollider.size = holeSize;
        _holeCollider.isTrigger = true;
    }
}