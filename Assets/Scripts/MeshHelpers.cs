using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHelpers
{
    /// <summary>
    /// Réalise la triangulation du face
    /// </summary>
    /// <param name="hg">Index du point supérieur gauche</param>
    /// <param name="hd">Index du point supérieur droite</param>
    /// <param name="bg">Index du point inférieur gauche</param>
    /// <param name="bd">Index du point inférieur droite</param>
    /// <returns>Indices résultant de la triangulation</returns>
    public static int[] TriangulateQuadFace(int hg, int hd, int bg, int bd)
    {
        return new int[]
        {
            bg, hd, hg,
            bg, bd, hd
        };
    }

}
