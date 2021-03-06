using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementManager : MonoBehaviour
{
    Vector3 mousePos;
    Vector3 prevPos;

    int currentTouchLevel = 0;

    int startZoneID = 0;

    bool placeLiquid = false;
    public bool placingStartZones = false;
    public bool touchingStartZoneTile = false;

    HexPiece.TileType selectedTileType = HexPiece.TileType.Grass;

    GameObject moveOrigin;
    GameObject guide = null;
    Material selectedMaterial;

    GameObject prevTouchedPiece;
    GameObject touchedPiece;
    RaycastHit mouseHit;

    public Camera cam;

    public List<GameObject> selectedPieces = new List<GameObject>();
    public List<GameObject> startZones = new List<GameObject>();

    float touchedPieceHeight = 0;
    bool didMouseHit = false;
    int guideRotation = 0;
    bool hasGuide = false;
    bool hasMover = false;
    int tileValue = 0;
    public int toolValue = 0;
    int biomeValue = 0;

    public Text numMapPlayers;
    public Dropdown mapPlayersStartZoneDropdown;
    public Transform terrainParent;
    public Transform startZoneParent;
    public GameObject board;
    public GameObject selectGroup;

    public Dropdown toolDropdown;
    public Dropdown tileDropdown;
    public Dropdown biomeDropdown;
    public BoardManager boardManager;

    public GameObject startZone;
    public GameObject startZoneTile;
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

    public Material startZoneMat;
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
            if (selectedPieces.Count > 0)
            {
                DeleteSelectedPieces();
            }
        }

        GetBoardTouchPosition();

        if (didMouseHit && !UIManager.isHoveringUIElement)
        {
            if (Input.GetMouseButtonDown(0))
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
                        //pinchedY = touchedPiece.transform.position.y + touchedPieceHeight;
                        break;
                }
            }
            if (Input.GetMouseButton(0))
            {
                switch (toolValue)
                {
                    case 0://Add
                        break;
                    case 1://Remove
                        break;
                    case 2://Select
                        break;
                    case 3://Move
                        MovePieces();
                        break;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                switch (toolValue)
                {
                    case 0://Add
                        break;
                    case 1://Remove
                        break;
                    case 2://Select
                        break;
                    case 3://Move
                        ClearMover();
                        foreach (GameObject go in selectedPieces)
                        {
                            foreach (Transform child in go.transform)
                            {
                                child.GetChild(0).gameObject.layer = 6;
                            }
                        }
                        break;
                }
            }
        }
    }

    public void GetBoardTouchPosition()
    {
        int layerMask = 1 << 6;
        Ray inputRay = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(inputRay, out mouseHit, Mathf.Infinity, layerMask))
        {
            didMouseHit = true;
            prevTouchedPiece = touchedPiece;
            touchedPiece = mouseHit.collider.transform.parent.parent.gameObject;

            touchingStartZoneTile = touchedPiece.GetComponent<HexPiece>() == null;

            if (!touchingStartZoneTile)
            {
                currentTouchLevel = mouseHit.collider.transform.parent.parent.gameObject.GetComponent<HexPiece>().level;
                touchedPieceHeight = touchedPiece.GetComponent<HexPiece>().height;
            }

            prevPos = mousePos;
            mousePos = mouseHit.collider.transform.position;
            mousePos.y += touchedPieceHeight;

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
        if (hover && !touchingStartZoneTile)
        {

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
        if (placingStartZones && hover)
        {
            if (prevTouchedPiece)
            {
                prevTouchedPiece.GetComponent<StartZoneTile>().DeSelect();
            }
            touchedPiece.GetComponent<StartZoneTile>().Select(hoverMaterial);
        }
        else if (currentTouchLevel != 0 && !placingStartZones && hover)
        {
            if (prevTouchedPiece)
            {
                prevTouchedPiece.GetComponent<HexPiece>().DeSelect();
            }
            touchedPiece.GetComponent<HexPiece>().Select(hoverMaterial);
        }
        else
        {
            if (placingStartZones && touchingStartZoneTile)
            {
                if (prevTouchedPiece)
                {
                    prevTouchedPiece.GetComponent<StartZoneTile>().DeSelect();
                }
                touchedPiece.GetComponent<StartZoneTile>().DeSelect();
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

    }
    public void SelectToolHover(bool hover)
    {
        if (placingStartZones && hover)
        {
            if (prevTouchedPiece && !prevTouchedPiece.GetComponent<StartZoneTile>().selected)
            {
                prevTouchedPiece.GetComponent<StartZoneTile>().DeSelect();
            }
            touchedPiece.GetComponent<StartZoneTile>().Select(hoverMaterial);
        }
        else if (currentTouchLevel != 0 && !placingStartZones && hover)
        {
            if (prevTouchedPiece && !prevTouchedPiece.GetComponent<HexPiece>().selected)
            {
                prevTouchedPiece.GetComponent<HexPiece>().DeSelect();
            }
            touchedPiece.GetComponent<HexPiece>().Select(hoverMaterial);
        }
        else
        {
            if (placingStartZones)
            {
                if (prevTouchedPiece && !prevTouchedPiece.GetComponent<StartZoneTile>().selected)
                {
                    prevTouchedPiece.GetComponent<StartZoneTile>().DeSelect();
                }
                if (!touchedPiece.GetComponent<StartZoneTile>().selected)
                {
                    touchedPiece.GetComponent<StartZoneTile>().DeSelect();
                }
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
            if (placingStartZones)
            {
                guide = Instantiate(startZoneTile, prevPos, Quaternion.Euler(new Vector3(0, guideRotation, 0)));
                guide.GetComponent<StartZoneTile>().SetMaterial(startZones[startZoneID].GetComponent<StartZone>().zoneMat);

            }
            else if (placeLiquid)
            {
                guide = Instantiate(liquidTile, prevPos, Quaternion.Euler(new Vector3(0, guideRotation, 0)));
                guide.GetComponent<StartZoneTile>().SetMaterial(startZoneMat);
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

            if (!placingStartZones)
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
                int layer = 6;
                child.GetChild(0).gameObject.layer = layer;
            }

            if (placingStartZones)
            {
                newPiece.GetComponent<StartZoneTile>().Setup(currentTouchLevel, startZoneID);

                newPiece.transform.SetParent(startZones[startZoneID].transform, true);
                startZones[startZoneID].GetComponent<StartZone>().tiles.Add(newPiece);
            }
            else
            {
                newPiece.GetComponent<HexPiece>().level = currentTouchLevel + 1;
                newPiece.GetComponent<HexPiece>().SetMaterial(selectedMaterial);
                newPiece.GetComponent<HexPiece>().liquid = placeLiquid;
                newPiece.GetComponent<HexPiece>().tileType = selectedTileType;
                newPiece.transform.SetParent(terrainParent, true);

                boardManager.AddPiece(currentTouchLevel + 1, newPiece);
            }
            ClearGuide();
            GetBoardTouchPosition();
        }
    }
    public void RemovePiece()
    {
        if (placingStartZones)
        {
            Destroy(touchedPiece);
            startZones[startZoneID].GetComponent<StartZone>().tiles.Remove(touchedPiece);
        }
        else if (currentTouchLevel != 0)
        {
            boardManager.RemovePiece(currentTouchLevel, touchedPiece);
            Destroy(touchedPiece);
        }
    }

    public void SelectPiece()
    {
        if (placingStartZones)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (touchedPiece.GetComponent<StartZoneTile>().selected)
                {
                    touchedPiece.GetComponent<StartZoneTile>().selected = false;
                    touchedPiece.GetComponent<StartZoneTile>().DeSelect();
                    selectedPieces.Remove(touchedPiece);
                    touchedPiece.transform.SetParent(startZones[touchedPiece.GetComponent<StartZoneTile>().playerID].transform, true);
                }
                else
                {
                    touchedPiece.GetComponent<StartZoneTile>().selected = true;
                    touchedPiece.GetComponent<StartZoneTile>().Select(hoverMaterial);
                    selectedPieces.Add(touchedPiece);
                    touchedPiece.transform.SetParent(selectGroup.transform, true);
                }
            }
            else
            {
                DeselectGroup();
                if (touchedPiece.GetComponent<StartZoneTile>().selected)
                {
                    touchedPiece.GetComponent<StartZoneTile>().selected = false;
                    touchedPiece.GetComponent<StartZoneTile>().DeSelect();
                    selectedPieces.Remove(touchedPiece);
                    touchedPiece.transform.SetParent(startZones[touchedPiece.GetComponent<StartZoneTile>().playerID].transform, true);
                }
                else
                {
                    touchedPiece.GetComponent<StartZoneTile>().selected = true;
                    touchedPiece.GetComponent<StartZoneTile>().Select(hoverMaterial);
                    selectedPieces.Add(touchedPiece);
                    touchedPiece.transform.SetParent(selectGroup.transform, true);
                }
            }
        }
        else if (currentTouchLevel != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (touchedPiece.GetComponent<HexPiece>().selected)
                {
                    touchedPiece.GetComponent<HexPiece>().selected = false;
                    touchedPiece.GetComponent<HexPiece>().DeSelect();
                    selectedPieces.Remove(touchedPiece);
                    touchedPiece.transform.SetParent(terrainParent, true);
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
                    touchedPiece.transform.SetParent(terrainParent, true);
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

            /* float totalX = 0;   
             float totalZ = 0;

             foreach(GameObject piece in selectedPieces)
             {
                 totalX += piece.transform.position.x;
                 totalZ += piece.transform.position.z;
             }
             GameObject pivot = new GameObject();
             pivot.transform.position = new Vector3(totalX / selectedPieces.Count, 0, totalZ / selectedPieces.Count) ;*/

            selectGroup.transform.RotateAround(selectedPieces[0].transform.position, transform.up, dir);

        }
    }
    public void DeselectGroup()
    {
        foreach (GameObject piece in selectedPieces)
        {
            if (placingStartZones)
            {
                piece.GetComponent<StartZoneTile>().selected = false;
                piece.GetComponent<StartZoneTile>().DeSelect();
                piece.transform.SetParent(startZones[piece.GetComponent<StartZoneTile>().playerID].transform, true);
            }
            else
            {
                piece.GetComponent<HexPiece>().selected = false;
                piece.GetComponent<HexPiece>().DeSelect();
                piece.transform.SetParent(terrainParent, true);
            }
        }
        selectedPieces.Clear();
    }
    public void DeleteSelectedPieces()
    {
        while (selectedPieces.Count > 0)
        {
            if (placingStartZones)
            {
                touchedPiece = selectedPieces[0];
                selectedPieces.Remove(touchedPiece);
                RemovePiece();
            }
            else
            {
                touchedPiece = selectedPieces[0];
                currentTouchLevel = touchedPiece.GetComponent<HexPiece>().level;
                selectedPieces.Remove(touchedPiece);
                RemovePiece();
            }
        }
    }

    public void MovePieces()
    {
        if (selectedPieces.Count > 0)
        {
            if (!hasMover)
            {
                MakeMover();
            }
            foreach (GameObject go in selectedPieces)
            {
                foreach (Transform child in go.transform)
                {
                    child.GetChild(0).gameObject.layer = 3;
                }
            }

            moveOrigin.transform.position = new Vector3(mousePos.x, touchedPiece.transform.position.y + touchedPieceHeight, mousePos.z);
        }
    }
    public void MakeMover()
    {
        float yPos = Mathf.Infinity;
        foreach (GameObject piece in selectedPieces)
        {
            if (piece.transform.position.y < yPos)
            {
                yPos = piece.transform.position.y;
            }
        }
        moveOrigin = Instantiate(new GameObject("_Mover"), new Vector3(mousePos.x, yPos, mousePos.z), Quaternion.identity);
        selectGroup.transform.SetParent(moveOrigin.transform, true);
        hasMover = true;
    }
    public void ClearMover()
    {
        selectGroup.transform.SetParent(null, true);
        Destroy(GameObject.Find("_Mover"));
        Destroy(GameObject.Find("_Mover(Clone)"));
        hasMover = false;
    }

    public void SelectTileType()
    {
        ClearGuide();
        tileValue = tileDropdown.value;
    }
    public void SelectToolType(Dropdown drop)
    {
        toolValue = drop.value;
        //DeselectGroup();
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

    public void PlacingStartZones(bool placing)
    {
        placingStartZones = placing;
    }

    public void MakePlayerStartZoneDropdown()
    {
        int numPlayers = int.Parse(numMapPlayers.text);

        if (numPlayers < startZones.Count)
        {
            while (startZones.Count > numPlayers)
            {
                Destroy(startZones[startZones.Count - 1]);
                startZones.RemoveAt(startZones.Count - 1);
            }
        }
        else if (numPlayers > startZones.Count)
        {
            while (startZones.Count < numPlayers)
            {
                GameObject newStartZone = Instantiate(startZone, new Vector3(0, 0, 0), Quaternion.identity);
                newStartZone.name = "Start Zone " + (startZones.Count + 1);
                newStartZone.transform.SetParent(startZoneParent, true);
                newStartZone.GetComponent<StartZone>().SetMaterial();
                startZones.Add(newStartZone);
            }
        }

        mapPlayersStartZoneDropdown.ClearOptions();
        List<string> dropOptions = new List<string>();
        for (int i = 0; i < numPlayers; i++)
        {
            dropOptions.Add("Player " + (i + 1) + " Start Zone");
        }
        mapPlayersStartZoneDropdown.AddOptions(dropOptions);
        startZoneID = 0;
    }

    public void SelectPlayerStartZone()
    {
        startZoneID = mapPlayersStartZoneDropdown.value;
    }

}

