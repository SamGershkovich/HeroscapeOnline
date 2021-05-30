using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZoneTile : MonoBehaviour
{
    public int level;
    public int playerID;
    public Material material;
    public bool selected = false;
    public void Setup(int level, int id)
    {
        this.level = level;
        playerID = id;
    }

    public void SetMaterial(Material mat)
    {
        material = mat;

        foreach (Transform child in transform)
        {
            child.GetChild(0).gameObject.GetComponent<Renderer>().material = mat;
        }
    }

    public void Select(Material mat)
    {
        foreach (Transform child in transform)
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
