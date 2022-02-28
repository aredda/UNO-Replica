using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public void RpcAddCardsToDeck(NetworkConnection _, PlayerNetworkAgent agent, List<byte[]> cardBytesList)
    {
        if (agent.player.Director.deckDealer.deck != null)
        {
            // TODO: to delete later
            agent.player.Director.deckDealer.PrintCardNames();

            return;
        }
        // add card to dealer's deck
        agent.player.Director.deckDealer.AddCards(cardBytesList);
        // TODO: to delete later
        agent.player.Director.deckDealer.PrintCardNames();
    }

    [TargetRpc]
    public void RpcSetBoardCard(NetworkConnection _, PlayerNetworkAgent agent, byte[] boardCardBytes)
    {
        agent.player.Director.deckDealer.SetBoardCard(Card.Deserialize(boardCardBytes));
        agent.player.Master.DisplayBoardCard();
    }
}
