using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ManagerDirector
    : MonoBehaviour
{
    public static ManagerDirector director;

    [Header("Managers")]
    public MaterialManager materialManager;
    public DeckDealer deckDealer;
    public GameMaster gameMaster;
    public PrefabManager prefabManager;
    public CardTemplateAnimator cardAnimator;
    public UIManager uiManager;
    public PoolingManager poolingManager;
    public AudioManager audioManager;
    public NetworkPoint networkPoint;

    void Awake()
    {
        if(director == null)
            director = this;
        else
            throw new System.Exception("ManagerDirector.Awake#Exception: Cant't have more than one ManagerDirector instance in the game");

        // Let them know who is the director
        foreach (Manager manager in GetComponentsInChildren(typeof(Manager)))
            manager.director = this;
    }

    void Start()
    {
        // exit if it's online
        if (gameMaster.isOnline)
            return;

        // Prepare deck
        deckDealer.Prepare();
        deckDealer.Shuffle();

        // Prepare Players
        gameMaster.PreparePlayers();

        // Prepare their UI cards
        uiManager.PreparePlayerCards(gameMaster.players);

        // Decide the board card
        deckDealer.SetBoardCard();

        // Display the board card
        gameMaster.DisplayBoardCard();

        // Change game state
        gameMaster.ChangeGameState(GameState.Started);
        
        // Deal Cards, the action will be called everytime a player finished being dealed with
        // Call Counter purpose is to ensure that this action is called when all the players have cards in their hands
        int callCounter = 0;
        deckDealer.DealCards(gameMaster.players, delegate() 
        {
            callCounter++;

            if(callCounter != gameMaster.players.Count)
                return;

            // Pass turn to the first player
            gameMaster.turn = gameMaster.players[gameMaster.players.Count - 1];
            gameMaster.EndTurn();
        });
    }
}
