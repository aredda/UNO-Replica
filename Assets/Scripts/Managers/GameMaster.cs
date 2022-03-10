using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMaster 
    : Manager
{
    [Header("Game Settings")]
    public bool isOnline = false;
    public GameRule rules = new GameRule();

    [Header("Hand Positions")]
    public List<HandDisposition> handCases;

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

    [Header("Declare One Card")]
    public PlayerController playerToDeclare;
    public bool hasDeclared = false;

    public void AddPlayer(PlayerController player)
    {
        players.Add(player);
    }

    public void PreparePlayers()
    {
        // retrieve proper disposition
        HandDisposition disposition = handCases.Single(d => d.handPositions.Count == totalPlayers);
        // set up players
        for(int i=0; i < totalPlayers; i++)
        {
            PlayerController player_controller = Instantiate(director.prefabManager.playerController);
            player_controller.SetHandPosition(disposition.handPositions[i]);
            player_controller.InitializeHand();

            players.Add(player_controller);
        }
        players[0].isLocalPlayer = true;
        // If bots are allowed
        if(enableBots)
            for(int i=1; i < totalPlayers; i++)
                players[i].BecomeBot();
    }

    public void SortPlayers()
    {
        // clone the cut part before removing
        int index = players.IndexOf(players.Single(p => p.IsLocalPlayer()));
        List<PlayerController> previous = players.GetRange(0, index);
        players.RemoveRange(0, index);
        // re-sort list
        foreach (var item in previous)
            players.Add(item);
    }

    public void MobilizePlayers()
    {
        // retrieve correct disposition
        HandDisposition disposition = handCases.Single(d => d.handPositions.Count == players.Count);
        // set up players
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetHandPosition(disposition.handPositions[i]);
            players[i].InitializeHand();
        }
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
        foreach (var p in players)
            if(p != player)
                p.SetCanPlay(false);
        // Grant the ability to play to the current turn
        player.SetCanPlay();
        // Update hand count
        director.uiManager.UpdatePlayerCards();
        // Highlight the turn
        director.uiManager.HighlightPlayerCard(turn);
        // Show declare button
        if (rules.enableOneCardCall)
        {
            if(turn.IsLocalPlayer())
            {
                if(turn.hand.CardsCount == 2 && turn.hand.FetchPlayableCards().Count != 0)
                    director.uiManager.buttonDeclare.Show(turn);
            }
        }
    }

    public void EndTurn()
    {
        StartCoroutine(RoutineEndTurn());
    }

    IEnumerator RoutineEndTurn()
    {
        // Hide one card declare button
        director.uiManager.buttonDeclare.Hide();
        // catch someone when not declaring one card
        // bot section
        PlayerController currentTurn = GetNextPlayer(turnStep);
        if (rules.enableOneCardCall)
            if (currentTurn.isBot)
                if (playerToDeclare && currentTurn != playerToDeclare && !hasDeclared)
                {
                    // show label
                    director.uiManager.labelPlayerState.Show($"Did not declare UNO (+2)", false, director.uiManager.GetPlayerCardID(playerToDeclare).RectTransform.anchoredPosition);
                    // if the player is caught, make him draw 2 cards
                    director.deckDealer.DealCards(playerToDeclare, 2, delegate ()
                    {
                        director.uiManager.labelPlayerState.Hide();
                    });
                    // reset declaration
                    ResetPlayerToDeclare();
                    // wait a little
                    yield return new WaitForSeconds(1f);
                }
        // Pass turn to the next player
        PassTurn(GetNextPlayer(turnStep));
        // Wait
        yield return new WaitForSeconds(0.5f);
        // Reset step
        turnStep = 1;
        // If challenging is enabled
        // and drawing is imposed
        // and if the last played card is a draw 4 card
        // and if there are no playable cards
        // and if the player is the local one
        if(rules.enableWildDrawChallenge && isDrawImposed && boardCardTemplate.card is Draw4Card && turn.hand.FetchPlayableCards().Count == 0 && turn.IsLocalPlayer())
        {
            // show him the challenge menu
            director.uiManager.menuChallenger.Show();
        }
        // If draw-stacking is disabled and drawing is imposed, force him to draw
        else if(!rules.enableDrawStacking && isDrawImposed)
            DrawImposedCards();
        // If the game is online, and if someone else is challenging do nothing
        else if(isOnline && rules.enableWildDrawChallenge && rules.enableDrawStacking && isDrawImposed && boardCardTemplate.card is Draw4Card && !turn.IsLocalPlayer())
            yield break;
        // If draw stacking is allowed & this player has no playable cards
        else if(rules.enableDrawStacking && isDrawImposed && turn.hand.FetchPlayableCards().Count == 0)
            DrawImposedCards();
        else
        {
            // If there are no playable cards, the player should draw
            if (turn.hand.FetchPlayableCards().Count == 0)
            {
                // Resort draw
                LastResortDraw(turn);
            }
            // If the next player is a bot
            else if(turn.isBot)
                turn.bot.Decide();
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
        isDrawImposed = true;
        drawTotal += cardsToDraw;
        // Update UI label
        director.uiManager.labelDrawTotal.Show(drawTotal);
        // Update action menu draw button text
        director.uiManager.menuCardActionPicker.SetDrawButtonText($"Draw +{drawTotal} Cards");
        // Move draw total label next to the threathened player
        director.cardAnimator.MoveDrawTotalText(director.uiManager.labelDrawTotal.RectTransform, director.uiManager.GetPlayerCardID(GetNextPlayer()).GetDrawCounterPosition());
    }

    public void DrawImposedCards(System.Action onFinish = null)
    {
        // turn of the possibility of playing
        turn.SetCanPlay(false);
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
        isDrawImposed = false;
        drawTotal = 0;
        // Hide label
        director.uiManager.labelDrawTotal.Hide();
        // Reset action menu draw button test
        director.uiManager.menuCardActionPicker.SetDrawButtonText("Draw Card");
    }

    public void WinChallenge(PlayerController challenged)
    {
        // change this player's state
        turn.SetCanPlay(false);
        // move the total draw text back to the previous player
        var previousPlayerCardID = director.uiManager.GetPlayerCardID(previousTurn);
        var currentPlayerCardID = director.uiManager.GetPlayerCardID(turn);

        director.cardAnimator.MoveDrawTotalText(
            director.uiManager.labelDrawTotal.RectTransform,
            previousPlayerCardID.GetDrawCounterPosition()
        );
        // show the player state
        director.uiManager.labelPlayerState.Show("Challenge Won", true, currentPlayerCardID.GetStateTextPosition());
        // make the challenged player draw
        director.deckDealer.DealCards(challenged, drawTotal, delegate ()
        {
            // hide state text
            director.uiManager.labelPlayerState.Hide();
            // reset draw mode
            ResetDrawing();
            // when cards are dealt, return the ability to play
            turn.SetCanPlay();
            // if there is nothing to play end turn
            if (turn.hand.FetchPlayableCards().Count == 0)
                EndTurn();
        });
    }

    public void LoseChallenge()
    {
        // change player state
        turn.SetCanPlay(false);
        // due to losing the challenge, a +2 penalty is added
        drawTotal += 2;
        // update text of draw total
        director.uiManager.labelDrawTotal.Show(drawTotal);
        // update draw total text
        director.cardAnimator.MoveDrawTotalText(
            director.uiManager.labelDrawTotal.RectTransform,
            director.uiManager.GetPlayerCardID(turn).GetDrawCounterPosition()
        );
        // show the player state
        director.uiManager.labelPlayerState.Show("Challenge Lost (+2 penalty)", false, director.uiManager.GetPlayerCardID(turn).GetStateTextPosition());
        // imposed drawing
        DrawImposedCards(delegate ()
        {
            // hide state text
            director.uiManager.labelPlayerState.Hide();
        });
    }

    public void SetPlayerToDeclare(PlayerController player, bool declared = false)
    {
        playerToDeclare = player;
        hasDeclared = declared;
    }

    public void ResetPlayerToDeclare()
    {
        SetPlayerToDeclare(null);
    }
}
