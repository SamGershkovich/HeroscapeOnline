using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public GameManager manager;
    public Player player;
    public Text _name;
    public Text nameLabel;
    public Toggle readyToggle;
    public Text points;

    public int playerNum;

    public void SetPlayerName()
    {
        player.playerName = _name.text;
    }
    public void BuildArmy()
    {
        manager.BuildPlayerArmy(player);
    }
    public void ReadyUp()
    {
        manager.PlayerReady(player.playerId, readyToggle.isOn);
    }
    public void UpdatePoints()
    {
        points.text = player.armyPoints.ToString() + "/" + manager.armyPoints + " pts";
    }
}
