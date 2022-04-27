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

    void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    [ClientRpc]
    public void RpcAddCardToHand(byte[] cardBytes)
    {
        player.hand.ReceiveCard(Card.Deserialize(cardBytes));
    }

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

    [Command]
    public void CmdWinChallenge()
    {
        player.Director.networkPoint.RpcWinChallenge(this);
    }

    [Command]
    public void CmdLoseChallenge()
    {
        player.Director.networkPoint.RpcLoseChallenge(this);
    }

    [Command]
    public void CmdSurrenderChallenge()
    {
        player.Director.networkPoint.RpcImposeDraw(this);
    }
}
