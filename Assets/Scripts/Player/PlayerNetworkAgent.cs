using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class PlayerNetworkAgent 
    : NetworkBehaviour
{
    [Header("Player Settings")]
    public PlayerController player;

    [ClientRpc]
    public void RpcShowCards()
    {
        foreach(var player in player.Master.players)
        {
            player.hand.CreateCardTemplates();
            player.hand.OrganizeCardTemplates();
        }
    }

    [Command]
    public void CmdPlayCard(byte[] cardBytes)
    {
        player.Director.networkPoint.RpcPlayCard(this, cardBytes);
    }

    [Command]
    public void CmdDrawCard()
    {
        player.Director.networkPoint.RpcDrawCard(this);
    }
}
