using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPiece : MonoBehaviour
{
    public int level = 0;
    public enum TileType
    {
        Asphalt,
        Castle,
        Concrete,
        Dungeon,
        Grass,
        Ice,
        Lava,
        LavaField,
        Road,
        Rock,
        Sand,
        Snow,
        Swamp,
        SwampWater,
        Water
    };

    public TileType tileType = TileType.Grass;
    public float height = 1;
    public Material material;
    public bool liquid = false;
    public bool selected = false;

    public void SetMaterial(Material mat)
    {
        material = mat;
        foreach (Transform child in transform)
        {
            child.GetChild(0).gameObject.GetComponent<Renderer>().material = material;
        }
    }
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
