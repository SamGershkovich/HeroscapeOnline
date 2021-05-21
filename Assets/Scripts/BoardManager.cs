using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public List<GameObject> pieces = new List<GameObject>();
}

public class BoardManager : MonoBehaviour
{
    public List<Level> levels = new List<Level>();

    private void Start()
    {
        AddLevel();
    }

    public void AddPiece(int level, GameObject piece)
    {
        if (level == levels.Count)
        {
            AddLevel();
        }

        foreach (Transform child in piece.transform)
        {
            levels[level].pieces.Add(child.gameObject);
        }
    }

    public void RemovePiece(int level, GameObject piece)
    {
        
        foreach (Transform child in piece.transform)
        {
            levels[level].pieces.Remove(child.gameObject);
        }
    }

    public void AddLevel()
    {
        levels.Add(new Level());
    }

    public bool WithinGameBoard(GameObject piece)
    {
        foreach (Transform child in piece.transform)//every tile in piece
        {
            bool currTileWithin = false;

            for (int i = 0; i < levels[0].pieces.Count; i++)//every tile in board
            {
                Vector3 translatedPos = new Vector3(child.GetChild(0).position.x, levels[0].pieces[i].transform.position.y, child.GetChild(0).position.z);

                float distance = Vector3.Distance(levels[0].pieces[i].transform.position, translatedPos);
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

    public bool LevelCellEmpty(int level, GameObject piece)
    {
        if (level == levels.Count)
        {
            return true;
        }
        for (int i = 0; i < levels[level].pieces.Count; i++)
        {
            foreach (Transform child in piece.transform)
            {
                float distance = Vector3.Distance(levels[level].pieces[i].transform.position, child.GetChild(0).position);
                if (distance < HexMetrics.outerRadius)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
