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

    [Header("Debug Helpers")]
    [SyncVar]
    public int handCardCount = 0;
    [SyncVar]
    public List<byte[]> handCardBytesList;
    public List<string> handCardNames = new List<string> ();

    [ClientRpc]
    public void RpcAddCard(byte[] cardBytes)
    {
        if (handCardBytesList == null)
            handCardBytesList = new List<byte[]>();

        handCardBytesList.Add(cardBytes);
        player.hand.ReceiveCard(Card.Deserialize(cardBytes));
        // helper section
        handCardCount = player.hand.CardsCount;
        PrintCardNames();
    }

    [ClientRpc]
    public void RpcShowCards()
    {
        foreach(var player in player.Master.players)
        {
            if (player.hand.CardsCount == 0 && player.networkAgent.handCardBytesList.Count != 0)
                player.hand.UpdateHand(player.networkAgent.handCardBytesList);

            player.hand.CreateCardTemplates();
            player.hand.OrganizeCardTemplates();
        }
    }

    public void PrintCardNames()
    {
        if(player.hand.cards != null)
            handCardNames = player.hand.cards.Select(c => c.GetMaterialName()).ToList();
    }
}
