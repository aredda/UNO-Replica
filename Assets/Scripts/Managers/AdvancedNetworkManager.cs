using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class AdvancedNetworkManager 
    : NetworkManager
{
    [Header("Network Point Reference")]
    public NetworkPoint networkPoint;

    [Header("Matchmaking Settings")]
    public int maxCapacity = 8;
    public int minCapacity = 2;

    public List<PlayerNetworkAgent> playerAgents;

    public override void OnStartServer()
    {
        base.OnStartServer();

        Debug.Log("Server is started");
    }

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
            DeckDealer dealer = networkPoint.director.deckDealer;
            dealer.Prepare();
            dealer.Shuffle();
            dealer.SetBoardCard();
            // configure network points
            foreach (var agent in playerAgents)
            {
                // sort players
                networkPoint.RpcSortPlayers(agent);
                // move each player to its respective position
                networkPoint.RpcMobilizePlayers(agent);
                // send synchronized board card to all clients
                networkPoint.RpcSetBoardCard(agent.connectionToClient, agent, dealer.boardCard.Serialize());
                // set player cards
                networkPoint.RpcSetPlayerUICards(agent.connectionToClient);
            }
            // deal cards to clients
            StartCoroutine(RoutineDealCardsToAgents(delegate ()
            {
                // show cards
                playerAgents[0].RpcShowCards();
                // set the initial turn
                networkPoint.RpcSetInitialTurn(playerAgents[0]);
            }));
        }
    }

    // For some reason, RPCs need delays in order to work properly
    IEnumerator RoutineDealCardsToAgents(System.Action onFinish)
    {
        DeckDealer dealer = networkPoint.director.deckDealer;

        foreach (var agent in playerAgents)
        {
            for (int i = 0; i < dealer.startingHand; i++)
                // add card to hand
                agent.RpcAddCardToHand(dealer.Dequeue().Serialize());
            yield return new WaitForSeconds(0.5f);
        }
        // send synchronized deck to all clients' deck dealers
        networkPoint.ClientRpcUpdateDeck(dealer.GetDeckBytesList());

        onFinish.Invoke();
    }

    public void ConnectToServer()
    {
        StartClient();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        // TODO: fix this temporary patchwork
        foreach (PlayerNetworkAgent agent in FindObjectsOfType<PlayerNetworkAgent>())
            Destroy(agent);

        playerAgents = new List<PlayerNetworkAgent>();
    }
}
