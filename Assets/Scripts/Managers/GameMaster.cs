using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMaster 
    : Manager
{
    [Header("Game Settings")]
    public GameRule rules = new GameRule();

    [Header("Hand Positions")]
    public Transform handPositions;

    [Header("Turn Settings")]
    public PlayerController turn;
    public PlayerController previousTurn;
    public int turnDirection = 1; // 1: Clockwise | -1: Counterclockwise
    public int turnStep = 1;

    [Header("Player Settings")]
    public int totalPlayers = 4;
    public List<PlayerController> players;
    public bool enableBots = true;

    [Header("Board Settings")]
    public CardTemplate boardCardTemplate;
    public Transform deck;
    public BoardController board;

    [Header("Draw State")]
    public bool isDrawImposed = false;
    public int drawTotal = 0;

    public void PreparePlayers()
    {
        for(int i=0; i < totalPlayers; i++)
        {
            PlayerController player_controller = Instantiate(director.prefabManager.playerController);
            player_controller.SetHandPosition(handPositions.GetChild(i));
            player_controller.InitializeHand();

            players.Add(player_controller);
        }
        players[0].isLocalPlayer = true;
        // If bots are allowed
        if(enableBots)
            for(int i=1; i < totalPlayers; i++)
                players[i].BecomeBot();
    }

    public void DisplayBoardCard()
    {
        boardCardTemplate.SetCard(director.deckDealer.boardCard);
        // Change board card
        ChangeBoardCard(boardCardTemplate);
    }

    public void ReverseTurnDirection()
    {
        this.turnDirection *= -1;
        // Visually reverse the direction of arrows
        this.board.ReverseArrows();
    }

    public void LastResortDraw(PlayerController player)
    {
        director.deckDealer.DealCard(player, delegate() {
            // Reset the player's ability to play
            player.SetCanPlay();
            // Check for the quick-play rule
            if(!rules.enableDrawQuickPlay)
                EndTurn();
            else
            {
                // If the player still has no playable cards end turn
                if(player.hand.FetchPlayableCards().Count == 0)
                    EndTurn();
                // if it's a bot, then let it decide
                else if(player.isBot)
                    player.bot.Decide();
            }
        });
    }

    public void PassTurn(PlayerController player)
    {
        // assign turn
        previousTurn = turn;
        turn = player;
        // Revoke the ability to play from all players
        foreach(var p in players)
            if(p != player)
                p.SetCanPlay(false);
        // Grant the ability to play to the current turn
        player.SetCanPlay();
        // Update hand count
        director.uiManager.UpdatePlayerCards();
        // Highlight the turn
        director.uiManager.HighlightPlayerCard(turn);
    }

    public void EndTurn()
    {
        StartCoroutine(RoutineEndTurn());
    }

    IEnumerator RoutineEndTurn()
    {
        // Pass turn to the next player
        PassTurn(GetNextPlayer(turnStep));
        // Wait
        yield return new WaitForSeconds(1f);
        // Reset step
        turnStep = 1;
        // If challenging is enabled
        // and drawing is imposed
        // and if the last played card is a draw 4 card
        // and if there are no playable cards
        // and if the player is the local one
        if(rules.enableWildDrawChallenge && isDrawImposed && boardCardTemplate.card is Draw4Card && turn.hand.FetchPlayableCards().Count == 0 && turn.isLocalPlayer)
        {
            // show him the challenge menu
            director.uiManager.menuChallenger.Show();
        }
        // If draw-stacking is disabled and drawing is imposed
        else if(!rules.enableDrawStacking && isDrawImposed)
        {
            // Disable his ability to play
            turn.SetCanPlay(false);
            // Make him draw cards
            DrawImposedCards();
        }
        // If draw stacking is allowed & this player has no playable cards
        else if(rules.enableDrawStacking && isDrawImposed && turn.hand.FetchPlayableCards().Count == 0)
            DrawImposedCards();
        else
        {
            // If there are no playable cards, the player should draw
            if(turn.hand.FetchPlayableCards().Count == 0)
            {
                // Resort draw
                this.LastResortDraw(turn);
            }
            // If the next player is a bot
            else if(this.turn.isBot)
                this.turn.bot.Decide();
        }
    }

    public PlayerController GetNextPlayer(int step = 1)
    {
        int turnIndex = players.IndexOf(turn);
        
        for(int i=0; i < step; i++)
        {
            if(turnDirection > 0)
                turnIndex = turnIndex == players.Count - 1 ? 0 : turnIndex + 1;
            else
                turnIndex = turnIndex == 0 ? players.Count - 1 : turnIndex - 1;
        }

        return players[turnIndex];
    }

    public void ChangeBoardCard(CardTemplate template)
    {
        this.boardCardTemplate.SetCard(template.card);

        // Retrieve color
        ECardColor color = template.card is ColorCard ? ((ColorCard) template.card).color : ((WildCard) template.card).chosenColor;
        // Change board color
        this.board.Change(color);
    }

    public void ImposeDrawing(int cardsToDraw = 2)
    {
        // Activate drawing mode
        this.isDrawImposed = true;
        this.drawTotal += cardsToDraw;
        // Update UI label
        this.director.uiManager.labelDrawTotal.Show(this.drawTotal);
        // Update action menu draw button text
        director.uiManager.menuCardActionPicker.SetDrawButtonText($"Draw +{drawTotal} Cards");
        // Move draw total label next to the threathened player
        director.cardAnimator.MoveDrawTotalText(director.uiManager.labelDrawTotal.RectTransform, director.uiManager.cardPlayerIDs.Single(cpi => cpi.Concerns(GetNextPlayer())).GetDrawCounterPosition());
    }

    public void DrawImposedCards(System.Action onFinish = null)
    {
        // Deal cards to the defeated player
        director.deckDealer.DealCards(turn, drawTotal, delegate() 
        {
            // Callback
            if(onFinish != null)
                onFinish.Invoke();
            // Exit drawing mode
            ResetDrawing();
            // End turn
            EndTurn();
        });
    }

    public void ResetDrawing()
    {
        // Disable drawing mode
        this.isDrawImposed = false;
        this.drawTotal = 0;
        // Hide label
        director.uiManager.labelDrawTotal.Hide();
        // Reset action menu draw button test
        director.uiManager.menuCardActionPicker.SetDrawButtonText("Draw Card");
    }
}
