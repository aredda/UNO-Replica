using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class NetworkPoint 
    : NetworkBehaviour
{
    public ManagerDirector director;

    [ClientRpc]
    public void RpcAddPlayer(PlayerNetworkAgent agent)
    {
        director.gameMaster.AddPlayer(agent.player);
    }

    [ClientRpc]
    public void RpcSortPlayers(PlayerNetworkAgent agent)
    {
        agent.player.Master.SortPlayers();
    }

    [ClientRpc]
    public void RpcMobilizePlayers(PlayerNetworkAgent agent)
    {
        agent.player.Master.MobilizePlayers();
    }

    [ClientRpc]
    public void RpcSetPlayerUICards(PlayerNetworkAgent agent)
    {
        agent.player.Director.uiManager.PreparePlayerCards(agent.player.Master.players);
    }

    [TargetRpc]
    public void RpcSetBoardCard(NetworkConnection _, PlayerNetworkAgent agent, byte[] boardCardBytes)
    {
        agent.player.Director.deckDealer.SetBoardCard(Card.Deserialize(boardCardBytes));
        agent.player.Master.DisplayBoardCard();
    }

    [TargetRpc]
    public void RpcUpdateDeck(NetworkConnection _, PlayerNetworkAgent agent, List<byte[]> cardBytesList)
    {
        agent.player.Director.deckDealer.UpdateDeck(cardBytesList);
        agent.player.Director.deckDealer.PrintCardNames();
    }

    [ClientRpc]
    public void RpcShowHand()
    {
        // TODO: change it to do a proper animation
        foreach (var player in director.gameMaster.players)
        {
            player.hand.CreateCardTemplates();
            player.hand.OrganizeCardTemplates();
        }
    }
}
