using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public List<GameObject> peices = new List<GameObject>();
}

public class BoardManager : MonoBehaviour
{
    public List<Level> levels = new List<Level>();

    private void Start()
    {
        AddLevel();
    }

    public void AddPeice(int level, GameObject peice)
    {
        if (level == levels.Count)
        {
            AddLevel();
        }

        foreach (Transform child in peice.transform)
        {
            levels[level].peices.Add(child.gameObject);
        }
    }

    public void RemovePeice(int level, GameObject peice)
    {
        
        foreach (Transform child in peice.transform)
        {
            levels[level].peices.Remove(child.gameObject);
        }
    }

    public void AddLevel()
    {
        levels.Add(new Level());
    }

    public bool WithinGameBoard(GameObject peice)
    {
        foreach (Transform child in peice.transform)//every tile in peice
        {
            bool currTileWithin = false;

            for (int i = 0; i < levels[0].peices.Count; i++)//every tile in board
            {
                Vector3 translatedPos = new Vector3(child.GetChild(0).position.x, levels[0].peices[i].transform.position.y, child.GetChild(0).position.z);

                float distance = Vector3.Distance(levels[0].peices[i].transform.position, translatedPos);
                if (distance < HexMetrics.outerRadius)
                {
                    currTileWithin = true;
                    break;
                }
            }
            if (!currTileWithin)
            {
                return false;
            }
        }

        return true;
    }

    public bool LevelCellEmpty(int level, GameObject peice)
    {
        if (level == levels.Count)
        {
            return true;
        }
        for (int i = 0; i < levels[level].peices.Count; i++)
        {
            foreach (Transform child in peice.transform)
            {
                float distance = Vector3.Distance(levels[level].peices[i].transform.position, child.GetChild(0).position);
                if (distance < HexMetrics.outerRadius)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
