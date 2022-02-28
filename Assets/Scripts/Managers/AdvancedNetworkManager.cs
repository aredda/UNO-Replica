using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AdvancedNetworkManager 
    : NetworkManager
{
    [Header("Network Point Reference")]
    public NetworkPoint networkPoint;

    [Header("Matchmaking Settings")]
    public int maxCapacity = 8;
    public int minCapacity = 2;

    private List<PlayerNetworkAgent> playerAgents;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // initialize player agent list if not
        if (playerAgents == null)
            playerAgents = new List<PlayerNetworkAgent> ();

        // retrieve agent
        PlayerNetworkAgent networkAgent = conn.identity.GetComponent<PlayerNetworkAgent> ();
        // grant authority
        conn.identity.AssignClientAuthority(conn);
        // save agent
        playerAgents.Add(networkAgent);

        // if the joined players number meets the minimum, start game | TODO: must be changed later
        if (numPlayers == minCapacity)
        {
            // register players in every client's game master
            foreach(var agent in playerAgents)
                networkPoint.RpcAddPlayer(agent);
            // prepare a deck of cards in server
            networkPoint.director.deckDealer.Prepare();
            networkPoint.director.deckDealer.Shuffle();
            networkPoint.director.deckDealer.SetBoardCard();
            // configure network points
            foreach (var agent in playerAgents)
            {
                // sort players
                networkPoint.RpcSortPlayers(agent);
                // move each player to its respective position
                networkPoint.RpcMobilizePlayers(agent);
                // set player cards
                networkPoint.RpcSetPlayerUICards(agent);
                // send synchronized deck to all clients' deck dealers
                networkPoint.RpcAddCardsToDeck(agent.connectionToClient, agent, networkPoint.director.deckDealer.GetDeckBytesList());
                // send synchronized board card to all clients
                networkPoint.RpcSetBoardCard(agent.connectionToClient, agent, networkPoint.director.deckDealer.boardCard.Serialize());
            }
        }
    }
}
