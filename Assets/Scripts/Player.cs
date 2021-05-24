using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInfo info;
    public int playerId;
    public int maxArmyPoints;
    public int armyPoints;
    public string playerName;
    public List<ArmyCard> army = new List<ArmyCard>();

    public void UpdateArmy()
    {
        info.UpdatePoints();
    }

    public void AddCard(ArmyCard card)
    {
        army.Add(card);
        armyPoints += card.Points;
        UpdateArmy();
    }

    public void RemoveCard(ArmyCard card)
    {
        army.Remove(card);
        armyPoints -= card.Points;
        UpdateArmy();

    }
}
