using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> units = new List<GameObject>();

    List<string> generalFilterList = new List<string>();
    List<string> homeWorldFilterList = new List<string>();
    List<string> speciesFilterList = new List<string>();
    List<string> typeFilterList = new List<string>();
    List<string> classFilterList = new List<string>();
    List<string> personalityFilterList = new List<string>();
    List<string> sizeFilterList = new List<string>();
    List<string> baseFilterList = new List<string>();

    int lifeMinFilter = 0;
    int lifeMaxFilter = 0;

    int moveMinFilter = 0;
    int moveMaxFilter = 0;

    int rangeMinFilter = 0;
    int rangeMaxFilter = 0;

    int attackMinFilter = 0;
    int attackMaxFilter = 0;

    int defenseMinFilter = 0;
    int defenseMaxFilter = 0;

    int numberOfUnitsMinFilter = 0;
    int numberOfUnitsMaxFilter = 0;

    int heightMinFilter = 0;
    int heightMaxFilter = 0;

    int pointsMinFilter = 0;
    int pointsMaxFilter = 0;


    public Canvas mainCanvas;

    public Transform playersGroup;
    public GameObject player;
    public GameObject playerInfo;

    public GameObject mainCamera;
    public GameObject board;
    public GameObject boardBase;

    public GameObject mapBuilderUI;

    public GameObject mapPlayerSettings;


    public GameObject preGameSettingsUI;
    public Transform playerInfos;
    public Text numPlayerInput;
    public Text armyPointsInput;

    public GameObject armyBuilderUI;
    public Transform unitViewContainer;
    public GameObject cardUI;
    public Transform armyViewContainer;
    public GameObject armyCardUI;
    public Button startGameButton;
    public Canvas armyBuilderCanvas;
    public Text filteredCardsText;
    List<bool> readyStates = new List<bool>();
    Player currentBuildPlayer;
    public Text armyCardCountText;
    public Text armyPointText;
    public List<GameObject> armyViewList = new List<GameObject>();
    public List<GameObject> unitViewList = new List<GameObject>();
    public List<GameObject> tempArmyList = new List<GameObject>();
    public List<GameObject> tempUnitList = new List<GameObject>();
    public int numFilteredCards = 0;

    public int numPlayers;
    public int armyPoints;

    public List<Player> players = new List<Player>();

    private void Start()
    {
        var temp = Resources.LoadAll("Units", typeof(GameObject));
        foreach (GameObject unit in temp)
        {
            units.Add(unit);
        }

    }

    public void ShowTerrainBuilderUI()
    {
        boardBase.SetActive(true);
        mapBuilderUI.SetActive(true);
        mapPlayerSettings.SetActive(false);
    }

    public void MapPlayerSettings()
    {
        boardBase.SetActive(false);
        mapBuilderUI.SetActive(false);
        mapPlayerSettings.SetActive(true);
    }

    public void PreSettings()
    {
        mainCamera.GetComponent<CameraSpectator>().enabled = false;
        board.SetActive(false);
        mapBuilderUI.SetActive(false);
        preGameSettingsUI.SetActive(true);

    }

    public void BuildArmy()
    {

        numPlayers = int.Parse(numPlayerInput.text);
        armyPoints = int.Parse(armyPointsInput.text);

        for (int i = 0; i < numPlayers; i++)
        {
            GameObject newPlayer = Instantiate(player);
            newPlayer.name = "Player " + (i + 1);
            newPlayer.GetComponent<Player>().maxArmyPoints = armyPoints;
            newPlayer.GetComponent<Player>().playerId = i;
            newPlayer.transform.SetParent(playersGroup, true);
            players.Add(newPlayer.GetComponent<Player>());

            GameObject newPlayerInfo = Instantiate(playerInfo);
            newPlayerInfo.GetComponent<PlayerInfo>().player = players[i];
            newPlayerInfo.GetComponent<PlayerInfo>().playerNum = i + 1;
            newPlayerInfo.GetComponent<PlayerInfo>().nameLabel.text = "Player " + (i + 1) + " Name: ";
            newPlayerInfo.GetComponent<PlayerInfo>().manager = this;
            newPlayerInfo.transform.SetParent(playerInfos, false);
            newPlayerInfo.transform.position += Vector3.up * -75 * i;

            newPlayer.GetComponent<Player>().info = newPlayerInfo.GetComponent<PlayerInfo>();
            readyStates.Add(false);

        }

        preGameSettingsUI.SetActive(false);
        armyBuilderUI.SetActive(true);

    }

    public void BuildPlayerArmy(Player player)
    {
        armyBuilderCanvas.gameObject.SetActive(true);
        mainCanvas.gameObject.SetActive(false);
        currentBuildPlayer = player;
        int cardCount = currentBuildPlayer.army.Count;

        ClearUnitViewList();
        ClearArmyViewList();

        FilterUnits();

        int counter = 0;

        foreach (GameObject card in units)
        {
            SetArmyListUIs(card, counter);

            if (card.GetComponent<ArmyCard>().isFiltered)
            {
                //card.gameObject.transform.localPosition = new Vector3(225, -50 + counter * -75, 0);
                counter++;
            }
        }


        foreach (ArmyCard card in currentBuildPlayer.army)
        {
            for (int i = 0; i < tempArmyList.Count; i++)
            {
                bool found = false;
                GameObject ui = tempArmyList[i];
                if (ui.GetComponent<ArmyCardUI>().card.Name == card.Name)
                {
                    ui.transform.localPosition = new Vector3(225, -50 + armyViewList.Count * -75, 0);
                    armyViewList.Add(ui);
                    tempArmyList.Remove(ui);
                    found = true;
                }
                if (found)
                {
                    break;
                }
            }
        }

        armyCardCountText.text = "Army: " + cardCount + " Cards";
        armyPointText.text = currentBuildPlayer.armyPoints + " pts";
    }

    public void SetArmyListUIs(GameObject card, int counter)
    {
        GameObject newCardUI = Instantiate(cardUI, Vector3.zero, Quaternion.identity);
        newCardUI.GetComponent<CardUI>().SetCard(card.GetComponent<ArmyCard>(), this);
        newCardUI.transform.SetParent(unitViewContainer, false);
        newCardUI.transform.localPosition = new Vector3(225, -50 + counter * -75, 0);

        newCardUI.SetActive(card.GetComponent<ArmyCard>().isFiltered);

        unitViewContainer.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100 * counter);


        foreach (ArmyCard playerCard in currentBuildPlayer.army)
        {
            if (playerCard.Name == card.GetComponent<ArmyCard>().Name)
            {
                newCardUI.GetComponent<CardUI>().TakeCard(false);
            }
        }
        unitViewList.Add(newCardUI);
    }

    public void TakeCard(CardUI cardUI, bool doAdd)
    {
        bool inDeck = false;
        bool inArmyList = false;
        ArmyCard thisCard = cardUI.card;
        int amountTaken = cardUI.amountTaken;


        foreach (ArmyCard card in currentBuildPlayer.army)
        {
            if (card.Name == thisCard.Name)
            {
                inDeck = true;
                break;
            }
        }

        if (!doAdd)
        {
            foreach (GameObject card in tempArmyList)
            {
                if (card.GetComponent<ArmyCardUI>().card.Name == thisCard.Name)
                {
                    inArmyList = true;
                    break;
                }
            }
        }

        if (doAdd)
        {
            currentBuildPlayer.AddCard(thisCard);
        }
        int cardCount = armyViewList.Count;

        armyCardCountText.text = "Army: " + (cardCount + 1) + " Cards";
        armyPointText.text = currentBuildPlayer.armyPoints + " pts";

        if (!inDeck || (!doAdd && !inArmyList))//create card in army view ui
        {
            GameObject newArmyCardUI = Instantiate(armyCardUI, Vector3.zero, Quaternion.identity);
            newArmyCardUI.GetComponent<ArmyCardUI>().SetCard(thisCard, cardUI, this);
            newArmyCardUI.transform.SetParent(armyViewContainer, false);
            newArmyCardUI.transform.localPosition = new Vector3(225, -50 + cardCount * -75, 0);

            armyViewContainer.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100 * cardCount);

            cardUI.armyCardUI = newArmyCardUI.GetComponent<ArmyCardUI>();

            if (doAdd)
            {
                armyViewList.Add(newArmyCardUI);
            }
            else
            {
                tempArmyList.Add(newArmyCardUI);
            }
        }

        cardUI.armyCardUI.TakeCard();

        if (doAdd)
            ShowFilteredCards();
    }

    public void PutBackCard(ArmyCardUI armyCardUI, bool lastOne)
    {
        currentBuildPlayer.RemoveCard(armyCardUI.card);
        armyCardUI.linkedUI.PutBackCard();

        if (lastOne)
        {
            armyViewList.Remove(armyCardUI.gameObject);
        }

        int cardCount = currentBuildPlayer.army.Count;

        armyCardCountText.text = "Army: " + cardCount + " Cards";
        armyPointText.text = currentBuildPlayer.armyPoints + " pts";

        int counter = 0;
        foreach (GameObject ui in armyViewList)
        {
            ui.transform.localPosition = new Vector3(225, -50 + counter * -75, 0);
            counter++;
        }

        ShowFilteredCards();
    }

    public void ShowFilteredCards()
    {
        FilterUnits();
        int counter = 0;
        foreach (GameObject ui in unitViewList)
        {
            ui.SetActive(ui.GetComponent<CardUI>().card.isFiltered);

            if (ui.GetComponent<CardUI>().card.isFiltered)
            {
                ui.transform.localPosition = new Vector3(225, -50 + counter * -75, 0);
                counter++;
            }
        }
    }

    public void ClearUnitViewList()
    {
        while (unitViewList.Count > 0)
        {
            Destroy(unitViewList[0]);
            unitViewList.RemoveAt(0);
        }
    }

    public void ClearArmyViewList()
    {
        while (armyViewList.Count > 0)
        {
            Destroy(armyViewList[0]);
            armyViewList.RemoveAt(0);
        }
    }

    public void FilterUnits()
    {
        numFilteredCards = 0;
        foreach (GameObject unit in units)
        {
            ArmyCard card = unit.GetComponent<ArmyCard>();

            bool addToFilter = true;

            if (card.Points > (currentBuildPlayer.maxArmyPoints - currentBuildPlayer.armyPoints))
            {
                print(card.Name + " " + 1);
                addToFilter = false;
            }
            else if (generalFilterList.Count > 0 && !generalFilterList.Contains(card._general.ToString()))
            {
                print(card.Name + " " + 2);

                addToFilter = false;
            }
            else if (homeWorldFilterList.Count > 0 && !homeWorldFilterList.Contains(card._homeWorld.ToString()))
            {
                addToFilter = false;
            }
            else if (speciesFilterList.Count > 0 && !speciesFilterList.Contains(card._species.ToString()))
            {
                print(card.Name + " " + 3);

                addToFilter = false;
            }
            else if (typeFilterList.Count > 0 && !typeFilterList.Contains(card._type.ToString()))
            {
                print(card.Name + " " + 4);

                addToFilter = false;
            }
            else if (classFilterList.Count > 0 && !classFilterList.Contains(card._class.ToString()))
            {
                print(card.Name + " " + 5);

                addToFilter = false;
            }
            else if (personalityFilterList.Count > 0 && !personalityFilterList.Contains(card._personality.ToString()))
            {
                print(card.Name + " " + 6);

                addToFilter = false;
            }
            else if (sizeFilterList.Count > 0 && !sizeFilterList.Contains(card._size.ToString()))
            {
                print(card.Name + " " + 7);

                addToFilter = false;
            }
            else if (baseFilterList.Count > 0 && !baseFilterList.Contains(card._base.ToString()))
            {
                print(card.Name + " " + 8);

                addToFilter = false;
            }
            else if (lifeMaxFilter > 0 && (card.Life < lifeMinFilter || card.Life > lifeMaxFilter))
            {
                print(card.Name + " " + 9);

                addToFilter = false;
            }
            else if (moveMaxFilter > 0 && (card.Move < moveMinFilter || card.Move > moveMaxFilter))
            {
                print(card.Name + " " + 10);

                addToFilter = false;
            }
            else if (rangeMaxFilter > 0 && (card.Range < rangeMinFilter || card.Range > rangeMaxFilter))
            {
                print(card.Name + " " + 11);

                addToFilter = false;
            }
            else if (attackMaxFilter > 0 && (card.Attack < attackMinFilter || card.Attack > attackMaxFilter))
            {
                print(card.Name + " " + 12);

                addToFilter = false;
            }
            else if (defenseMaxFilter > 0 && (card.Defense < defenseMinFilter || card.Defense > defenseMaxFilter))
            {
                print(card.Name + " " + 13);
                addToFilter = false;
            }
            else if (numberOfUnitsMaxFilter > 0 && (card.numberOfUnits < numberOfUnitsMinFilter || card.numberOfUnits > numberOfUnitsMaxFilter))
            {
                print(card.Name + " " + 14);

                addToFilter = false;
            }
            else if (heightMaxFilter > 0 && (card.Height < heightMinFilter || card.Height > heightMaxFilter))
            {
                print(card.Name + " " + 15);

                addToFilter = false;
            }
            else if (pointsMaxFilter > 0 && (card.Points < pointsMinFilter || card.Points > pointsMaxFilter))
            {
                print(card.Name + " " + 16);

                addToFilter = false;
            }

            card.isFiltered = addToFilter;
            if (addToFilter)
            {
                numFilteredCards++;
            }
        }
        filteredCardsText.text = "Showing " + numFilteredCards + "/" + units.Count + " cards";
    }

    public void FinishBuildPlayerArmy()
    {
        currentBuildPlayer.UpdateArmy();
        armyBuilderCanvas.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
    }

    public void PlayerReady(int index, bool ready)
    {
        readyStates[index] = ready;
        bool allReady = true;
        foreach (bool state in readyStates)
        {
            if (!state)
            {
                allReady = false;
                break;
            }
        }
        startGameButton.interactable = allReady;

    }

    public void StartGame()
    {
        armyBuilderUI.SetActive(false);
        board.SetActive(true);
        mainCamera.GetComponent<CameraSpectator>().enabled = true;
    }
}
