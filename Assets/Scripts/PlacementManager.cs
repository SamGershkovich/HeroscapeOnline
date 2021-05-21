using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementManager : MonoBehaviour
{
    Vector3 mousePos;
    Vector3 prevPos;

    int currentTouchLevel = 0;

    bool placeLiquid = false;

    HexPiece.TileType selectedTileType = HexPiece.TileType.Grass;

    GameObject guide = null;
    Material selectedMaterial;

    GameObject prevTouchedPiece;
    GameObject touchedPiece;
    RaycastHit mouseHit;

    public List<GameObject> selectedPieces = new List<GameObject>();


    bool didMouseHit = false;
    int guideRotation = 0;
    bool hasGuide = false;
    bool isHovering = false;
    int tileValue = 0;
    int toolValue = 0;
    int biomeValue = 0;

    public GameObject board;
    public GameObject selectGroup;

    public Dropdown toolDropdown;
    public Dropdown tileDropdown;
    public Dropdown biomeDropdown;
    public BoardManager boardManager;

    public GameObject liquidTile;
    public GameObject singleTile;
    public GameObject doubleTile;
    public GameObject tripleTile;
    public GameObject septupleTile;
    public GameObject twentyFourTile;

    public Material asphaltMat;
    public Material castleMat;
    public Material concreteMat;
    public Material dungeonMat;
    public Material grassMat;
    public Material iceMat;
    public Material lavaMat;
    public Material lavaFieldMat;
    public Material roadMat;
    public Material rockMat;
    public Material sandMat;
    public Material snowMat;
    public Material swampMat;
    public Material swampWaterMat;
    public Material waterMat;

    public Material hoverMaterial;

    private void Start()
    {
        selectedMaterial = grassMat;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            guideRotation += 60;

            switch (toolValue)
            {
                case 0://Add
                    break;
                case 1://Remove
                    break;
                case 2://Select
                    RotateGroup(60);
                    break;
                case 3://Move
                    break;
            }

            ClearGuide();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            guideRotation -= 60;
            switch (toolValue)
            {
                case 0://Add
                    break;
                case 1://Remove
                    break;
                case 2://Select
                    RotateGroup(-60);
                    break;
                case 3://Move
                    break;
            }
            ClearGuide();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if(selectedPieces.Count > 0)
            {
                DeleteSelectedPieces();
            }
        }

        GetBoardTouchPosition();

        if (Input.GetMouseButtonDown(0) && didMouseHit && !UIManager.isHoveringUIElement)
        {
            switch (toolValue)
            {
                case 0://Add
                    PlacePiece();
                    break;
                case 1://Remove
                    RemovePiece();
                    break;
                case 2://Select
                    SelectPiece();
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
            Debug.DrawRay(mouseHit.point, Vector3.up, Color.red);
            didMouseHit = true;
            prevTouchedPiece = touchedPiece;
            touchedPiece = mouseHit.collider.transform.parent.parent.gameObject;
            currentTouchLevel = mouseHit.collider.transform.parent.parent.gameObject.GetComponent<HexPiece>().level;

            GetToolModeHover(true);
        }
        else
        {
            didMouseHit = false;
            GetToolModeHover(false);
        }
    }
    
    public void GetToolModeHover(bool hover)
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
                SelectToolHover(hover);
                break;
            case 3://Move
                break;
        }
    }
   
    public void AddToolHover(bool hover)
    {
        if (hover)
        {
            prevPos = mousePos;

            /* if (currentTouchLevel != 0)
             {*/
            mousePos = mouseHit.collider.transform.position;
            mousePos.y += 1.75f;
            /*}
            else
            {
                mousePos = mouseHit.point;
                mousePos = new Vector3(mousePos.x / 15, mousePos.y, mousePos.z / 15);
                mousePos.x = ((int)mousePos.x + (int)mousePos.z * 0.5f - (int)mousePos.z / 2) * (HexMetrics.innerRadius * 2f);
                mousePos.z = (int)mousePos.z * (HexMetrics.outerRadius * 1.5f);
            }*/

            MakeGuide();

            GameObject temp = Instantiate(guide);
            temp.transform.position = mousePos;

            ClearGuide();

            if (boardManager.LevelCellEmpty(currentTouchLevel + 1, temp) /*&& boardManager.WithinGameBoard(temp)*/)
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
        if (currentTouchLevel != 0 && hover)
        {
            if (prevTouchedPiece)
            {
                prevTouchedPiece.GetComponent<HexPiece>().DeSelect();
            }
            touchedPiece.GetComponent<HexPiece>().Select(hoverMaterial);
        }
        else
        {
            if (prevTouchedPiece)
            {
                prevTouchedPiece.GetComponent<HexPiece>().DeSelect();
            }
            touchedPiece.GetComponent<HexPiece>().DeSelect();
        }

    }
    public void SelectToolHover(bool hover)
    {
        if (currentTouchLevel != 0 && hover)
        {
            if (prevTouchedPiece && !prevTouchedPiece.GetComponent<HexPiece>().selected)
            {
                prevTouchedPiece.GetComponent<HexPiece>().DeSelect();
            }
            touchedPiece.GetComponent<HexPiece>().Select(hoverMaterial);
        }
        else
        {

            if (prevTouchedPiece && !prevTouchedPiece.GetComponent<HexPiece>().selected)
            {
                prevTouchedPiece.GetComponent<HexPiece>().DeSelect();
            }
            if (!touchedPiece.GetComponent<HexPiece>().selected)
            {
                touchedPiece.GetComponent<HexPiece>().DeSelect();
            }
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
            if (placeLiquid)
            {
                guide = Instantiate(liquidTile, prevPos, Quaternion.Euler(new Vector3(0, guideRotation, 0)));
            }
            else
            {
                switch (tileValue)
                {
                    case 0:
                        guide = Instantiate(singleTile, prevPos, Quaternion.Euler(new Vector3(0, guideRotation, 0)));
                        break;
                    case 1:
                        guide = Instantiate(doubleTile, prevPos, Quaternion.Euler(new Vector3(0, guideRotation, 0)));
                        break;
                    case 2:
                        guide = Instantiate(tripleTile, prevPos, Quaternion.Euler(new Vector3(0, guideRotation, 0)));
                        break;
                    case 3:
                        guide = Instantiate(septupleTile, prevPos, Quaternion.Euler(new Vector3(0, guideRotation, 0)));
                        break;
                    case 4:
                        guide = Instantiate(twentyFourTile, prevPos, Quaternion.Euler(new Vector3(0, guideRotation, 0)));
                        break;
                }
            }

            guide.GetComponent<HexPiece>().SetMaterial(selectedMaterial);

            foreach (Transform child in guide.transform)
            {
                child.GetChild(0).gameObject.layer = 3;
            }
        }
    }
    public void PlacePiece()
    {
        if (hasGuide)
        {
            GameObject newPiece = Instantiate(guide, guide.transform.position, guide.transform.rotation);

            foreach (Transform child in newPiece.transform)
            {
                child.GetChild(0).gameObject.layer = 6;
            }

            newPiece.GetComponent<HexPiece>().level = currentTouchLevel + 1;
            newPiece.GetComponent<HexPiece>().SetMaterial(selectedMaterial);
            newPiece.GetComponent<HexPiece>().liquid = placeLiquid;
            newPiece.GetComponent<HexPiece>().tileType = selectedTileType;

            boardManager.AddPiece(currentTouchLevel + 1, newPiece);

            ClearGuide();
            GetBoardTouchPosition();
        }
    }

    public void RemovePiece()
    {
        if (currentTouchLevel != 0)
        {
            boardManager.RemovePiece(currentTouchLevel, touchedPiece);
            Destroy(touchedPiece);
        }
    }

    public void SelectPiece()
    {
        if (currentTouchLevel != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (touchedPiece.GetComponent<HexPiece>().selected)
                {
                    touchedPiece.GetComponent<HexPiece>().selected = false;
                    touchedPiece.GetComponent<HexPiece>().DeSelect();
                    selectedPieces.Remove(touchedPiece);
                    touchedPiece.transform.SetParent(null, true);
                }
                else
                {
                    touchedPiece.GetComponent<HexPiece>().selected = true;
                    touchedPiece.GetComponent<HexPiece>().Select(hoverMaterial);
                    selectedPieces.Add(touchedPiece);
                    touchedPiece.transform.SetParent(selectGroup.transform, true);
                }
            }
            else
            {
                DeselectGroup();
                if (touchedPiece.GetComponent<HexPiece>().selected)
                {
                    touchedPiece.GetComponent<HexPiece>().selected = false;
                    touchedPiece.GetComponent<HexPiece>().DeSelect();
                    selectedPieces.Remove(touchedPiece);
                    touchedPiece.transform.SetParent(null, true);
                }
                else
                {
                    touchedPiece.GetComponent<HexPiece>().selected = true;
                    touchedPiece.GetComponent<HexPiece>().Select(hoverMaterial);
                    selectedPieces.Add(touchedPiece);
                    touchedPiece.transform.SetParent(selectGroup.transform, true);
                }
            }
            
        }
    }
    public void RotateGroup(int dir)
    {
        if (selectedPieces.Count > 0)
        {
            selectGroup.transform.RotateAround(selectedPieces[0].transform.position, transform.up, dir);
        }
    }
    public void DeselectGroup()
    {
        foreach (GameObject piece in selectedPieces)
        {
            piece.GetComponent<HexPiece>().selected = false;
            piece.GetComponent<HexPiece>().DeSelect();
            piece.transform.SetParent(null, true);
        }
        selectedPieces.Clear();
    }
    public void DeleteSelectedPieces()
    {
        while(selectedPieces.Count > 0)
        {
            touchedPiece = selectedPieces[0];
            currentTouchLevel = touchedPiece.GetComponent<HexPiece>().level;
            selectedPieces.Remove(touchedPiece);
            RemovePiece();
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
        DeselectGroup();
    }
    public void SelectBiomeType()
    {
        ClearGuide();
        biomeValue = biomeDropdown.value;
        switch (biomeValue)
        {
            case 0:
                selectedMaterial = asphaltMat;
                placeLiquid = false;
                selectedTileType = HexPiece.TileType.Asphalt;
                break;
            case 1:
                selectedMaterial = castleMat;
                placeLiquid = false;
                selectedTileType = HexPiece.TileType.Castle;
                break;
            case 2:
                selectedMaterial = concreteMat;
                placeLiquid = false;
                selectedTileType = HexPiece.TileType.Concrete;
                break;
            case 3:
                selectedMaterial = dungeonMat;
                placeLiquid = false;
                selectedTileType = HexPiece.TileType.Dungeon;
                break;
            case 4:
                selectedMaterial = grassMat;
                placeLiquid = false;
                selectedTileType = HexPiece.TileType.Grass;
                break;
            case 5:
                selectedMaterial = iceMat;
                placeLiquid = true;
                selectedTileType = HexPiece.TileType.Ice;
                break;
            case 6:
                selectedMaterial = lavaMat;
                placeLiquid = true;
                selectedTileType = HexPiece.TileType.Lava;
                break;
            case 7:
                selectedMaterial = lavaFieldMat;
                placeLiquid = false;
                selectedTileType = HexPiece.TileType.LavaField;
                break;
            case 8:
                selectedMaterial = roadMat;
                placeLiquid = false;
                selectedTileType = HexPiece.TileType.Road;
                break;
            case 9:
                selectedMaterial = rockMat;
                placeLiquid = false;
                selectedTileType = HexPiece.TileType.Rock;
                break;
            case 10:
                selectedMaterial = sandMat;
                placeLiquid = false;
                selectedTileType = HexPiece.TileType.Sand;
                break;
            case 11:
                selectedMaterial = snowMat;
                placeLiquid = false;
                selectedTileType = HexPiece.TileType.Snow;
                break;
            case 12:
                selectedMaterial = swampMat;
                placeLiquid = false;
                selectedTileType = HexPiece.TileType.Swamp;
                break;
            case 13:
                selectedMaterial = swampWaterMat;
                placeLiquid = true;
                selectedTileType = HexPiece.TileType.SwampWater;
                break;
            case 14:
                selectedMaterial = waterMat;
                placeLiquid = true;
                selectedTileType = HexPiece.TileType.Water;
                break;
        }
    }
}

