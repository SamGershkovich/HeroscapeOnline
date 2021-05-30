using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    public List<GameObject> tiles = new List<GameObject>();

    public Material zoneMat;

    private void Start()
    {
        zoneMat = new Material(zoneMat);
        zoneMat.SetColor("_EmissionColor", Color.HSVToRGB(Random.Range(0, 360), 100, 40, true));

    }

    public void SetMaterial()
    {

    }
}
