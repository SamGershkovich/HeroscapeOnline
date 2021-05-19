using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementManager : MonoBehaviour
{
    Vector3 mousePos;

    int currentTouchLevel = 0;

    GameObject guide = null;

    GameObject prevTouchedPeice;
    GameObject touchedPeice;
    RaycastHit mouseHit;

    bool didMouseHit = false;
    int guideRotation = 0;
    int tileValue = 0;
    int toolValue = 0;
    bool hasGuide = false;
    bool isHovering = false;

    public Dropdown toolDropdown;
    public Dropdown tileDropdown;
    public BoardManager boardManager;

    public GameObject singleTile;
    public GameObject doubleTile;
    public GameObject tripleTile;
    public GameObject septupleTile;

    public Material selectMaterial;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            guideRotation += 60;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            guideRotation -= 60;
        }

        GetBoardTouchPosition();

        if (Input.GetMouseButtonDown(0) && didMouseHit)
        {
            switch (toolValue)
            {
                case 0://Add
                    PlacePeice();
                    break;
                case 1://Remove
                    RemovePeice();
                    break;
                case 2://Select
                    break;
                case 3://Move
                    break;
            }
        }
    }

    public void GetBoardTouchPosition()
    {
        int layerMask = 1 << 6;
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(inputRay, out mouseHit, Mathf.Infinity, layerMask))
        {
            didMouseHit = true;
            prevTouchedPeice = touchedPeice;
            touchedPeice = mouseHit.collider.transform.parent.parent.gameObject;
            currentTouchLevel = mouseHit.collider.transform.parent.parent.gameObject.GetComponent<HexPeice>().level;

            GetToolModeHover(true);
        }
        else
        {
            didMouseHit = false;
            GetToolModeHover(false);
        }
    }
    public void ClearGuide()
    {
        Destroy(guide);
        hasGuide = false;
    }
    public void MakeGuide()
    {
        if (!hasGuide)
        {
            hasGuide = true;
            switch (tileValue)
            {
                case 0:
                    guide = Instantiate(singleTile, mousePos, Quaternion.Euler(new Vector3(0, guideRotation, 0)));
                    break;
                case 1:
                    guide = Instantiate(doubleTile, mousePos, Quaternion.Euler(new Vector3(0, guideRotation, 0)));
                    break;
                case 2:
                    guide = Instantiate(tripleTile, mousePos, Quaternion.Euler(new Vector3(0, guideRotation, 0)));
                    break;
                case 3:
                    guide = Instantiate(septupleTile, mousePos, Quaternion.Euler(new Vector3(0, guideRotation, 0)));
                    break;
            }
            foreach (Transform child in guide.transform)
            {
                child.GetChild(0).gameObject.layer = 3;
            }
        }
    }
    public void PlacePeice()
    {
        GameObject newPeice = Instantiate(guide, guide.transform.position, guide.transform.rotation);

        foreach (Transform child in newPeice.transform)
        {
            child.GetChild(0).gameObject.layer = 6;
        }

        newPeice.GetComponent<HexPeice>().level = currentTouchLevel + 1;

        boardManager.AddPeice(currentTouchLevel + 1, newPeice);

        ClearGuide();
        GetBoardTouchPosition();
    }
    public void RemovePeice()
    {
        if (currentTouchLevel != 0)
        {
            boardManager.RemovePeice(currentTouchLevel, touchedPeice);
            Destroy(touchedPeice);
        }
    }
    public void GetToolModeHover(bool hover)
    {
        if (prevTouchedPeice != touchedPeice)
        {
            switch (toolValue)
            {
                case 0://Add

                    AddToolHover(hover);
                    break;

                case 1://Remove

                    RemoveToolHover(hover);
                    break;

                case 2://Select

                    break;

                case 3://Move


                    break;
            }
        }
    }
    public void AddToolHover(bool hover)
    {
        if (hover)
        {
            Vector3 prevPos = mousePos;

            mousePos = mouseHit.collider.transform.position;
            mousePos.y += 1.75f;
            MakeGuide();

            GameObject temp = Instantiate(guide);
            temp.transform.position = mousePos;

            ClearGuide();

            if (boardManager.LevelCellEmpty(currentTouchLevel + 1, temp) && boardManager.WithinGameBoard(temp))
            {
                MakeGuide();
                guide.transform.position = mousePos;
            }

            Destroy(temp);
        }
        else
        {
            ClearGuide();

        }
    }
    public void RemoveToolHover(bool hover)
    {
        if (hover)
        {
            if (prevTouchedPeice)
            {
                prevTouchedPeice.GetComponent<HexPeice>().DeSelect();
            }
            touchedPeice.GetComponent<HexPeice>().Select(selectMaterial);
        }
        else
        {
            if (prevTouchedPeice)
            {
                prevTouchedPeice.GetComponent<HexPeice>().DeSelect();
            }
            touchedPeice.GetComponent<HexPeice>().DeSelect();
        }
    }
    public void SelectTileType()
    {
        ClearGuide();
        tileValue = tileDropdown.value;
    }
    public void SelectToolType()
    {
        toolValue = toolDropdown.value;
    }
}

