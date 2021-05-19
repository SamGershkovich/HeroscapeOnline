using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPeice : MonoBehaviour
{
    public int level = 0;
    public enum TileType
    {
        Grass,
        Stone,
        Sand,
        Swamp,
        Road,
        Magma,
        Water,
        Shadow,
        SwampWater,
        Lava
    };

    public TileType tileType = TileType.Grass;

    public Material material;


    public void Select(Material mat)
    {
        foreach(Transform child in transform)
        {
            child.GetChild(0).gameObject.GetComponent<Renderer>().material = mat;
        }
    }

    public void DeSelect()
    {
        foreach (Transform child in transform)
        {
            child.GetChild(0).gameObject.GetComponent<Renderer>().material = material;
        }
    }
}
