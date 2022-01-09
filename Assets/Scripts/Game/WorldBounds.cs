using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class WorldBounds : MonoBehaviour
{
    [SerializeField]
    private PlaneHoleMesh _worldMesh;

    private MeshCollider _collider;

    private void Start()
    {
        _collider = GetComponent<MeshCollider>();
        _collider.sharedMesh = GenerateMesh();
    }

    private Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();

        float meshSize = _worldMesh.MeshSize;
        Vector3[] lowerCorners =
        {
            new Vector3(-meshSize, 0.0f, -meshSize),
            new Vector3(-meshSize, 0.0f,  meshSize),
            new Vector3( meshSize, 0.0f, -meshSize),
            new Vector3( meshSize, 0.0f,  meshSize)
        };
        Vector3[] upperCorners = Array.ConvertAll(lowerCorners, new Converter<Vector3, Vector3>((v) => v + Vector3.up));

        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        vertices.AddRange(lowerCorners);
        vertices.AddRange(upperCorners);

        indices.AddRange(MeshHelpers.TriangulateQuadFace(4, 6, 0, 2));
        indices.AddRange(MeshHelpers.TriangulateQuadFace(6, 7, 2, 3));
        indices.AddRange(MeshHelpers.TriangulateQuadFace(7, 5, 3, 1));
        indices.AddRange(MeshHelpers.TriangulateQuadFace(5, 4, 1, 0));

        mesh.SetVertices(vertices.ToArray());
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }
}
