using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyCardUI : MonoBehaviour
{
    public GameManager manager;
    public ArmyCard card;
    public CardUI linkedUI;

    public int amountTaken;

    public Text nameText;
    public Text amountTakenText;
    public Button putBackButton;

    public void SetCard(ArmyCard card, CardUI linkedUI, GameManager manager)
    {
        this.card = card;
        this.manager = manager;
        this.linkedUI = linkedUI;
        nameText.text = card.Name;
    }


    public void TakeCard()
    {
        amountTaken++;
        amountTakenText.text = "x" + amountTaken;
    }

    public void PutBackCard()
    {
        amountTaken--;
        amountTakenText.text = "x" + amountTaken;
        manager.PutBackCard(this, amountTaken <= 0);
        if (amountTaken <= 0)
        {
            Destroy(gameObject);
        }
    }

}
