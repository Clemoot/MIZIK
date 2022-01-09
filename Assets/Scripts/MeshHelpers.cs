using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHelpers
{
    /// <summary>
    /// R�alise la triangulation du face
    /// </summary>
    /// <param name="hg">Index du point sup�rieur gauche</param>
    /// <param name="hd">Index du point sup�rieur droite</param>
    /// <param name="bg">Index du point inf�rieur gauche</param>
    /// <param name="bd">Index du point inf�rieur droite</param>
    /// <returns>Indices r�sultant de la triangulation</returns>
    public static int[] TriangulateQuadFace(int hg, int hd, int bg, int bd)
    {
        return new int[]
        {
            bg, hd, hg,
            bg, bd, hd
        };
    }

}
