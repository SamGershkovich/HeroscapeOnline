using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public GameManager manager;
    public ArmyCard card;
    public ArmyCardUI armyCardUI;

    public int amountTaken;

    public Text nameText;
    public Button takeButton;

    public void SetCard(ArmyCard card, GameManager manager)
    {
        this.card = card;
        this.manager = manager;
        nameText.text = card.Name;
    }

    public void ViewCard()
    {

    }

    public void TakeCard(bool doAdd)
    {
        amountTaken++;
        takeButton.interactable = manager.TakeCard(this, doAdd);       
    }

    public void PutBackCard()
    {
        amountTaken--;
        takeButton.interactable = !(card._type == ArmyCard.Type.UniqueHero || card._type == ArmyCard.Type.UniqueSquad) || (card._type == ArmyCard.Type.UniqueHero || card._type == ArmyCard.Type.UniqueSquad) && amountTaken == 0;
    }

}
