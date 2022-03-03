using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class NetworkPoint 
    : NetworkBehaviour
{
    public ManagerDirector director;

    [Header("Persisted Master Fields")]
    [SyncVar]
    public PlayerNetworkAgent currentTurn;

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
    }

    [ClientRpc]
    public void RpcSetInitialTurn(PlayerNetworkAgent turnAgent)
    {
        director.networkPoint.currentTurn = turnAgent;
        // pass turn to targeted player
        director.gameMaster.PassTurn(turnAgent.player);
    }

    // context: player clicks "Play Card" button from actions menu
    // - remove card from hand card list
    // - return it to deck (enqueued of course)
    // - play card animation locally
    // - broadcasting the play movement to all players
    // - update the player's hand in every network point (client)
    // - pass turn, should be synced in all networks too

    [ClientRpc]
    public void RpcPlayCard(PlayerNetworkAgent agent, byte[] cardBytes)
    {
        // get played card
        Card card = Card.Deserialize(cardBytes);
        // get related template
        CardTemplate template = agent.player.hand.GetCardTemplate(card);
        // activate
        card.Activate(template, delegate () {
            // play card
            agent.player.hand.PlayCard(template);
        });
    }

    [ClientRpc]
    public void RpcDrawCard(PlayerNetworkAgent agent)
    {
        // first, check if draw mode is imposed
        if (agent.player.Master.isDrawImposed)
        {
            agent.player.Master.DrawImposedCards();
        }
        else
        {
            // Last resort card draw
            agent.player.Master.LastResortDraw(agent.player);
            // TODO: the player can only call for one last resort draw
            // now, there's a hole in the logic of this button, because the player can
            // draw infinitely
        }
    }
}
