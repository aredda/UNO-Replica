using UnityEngine;

[System.Serializable]
public class GameRule
{
    [Tooltip("Draw Cards can be chained")]
    public bool enableDrawStacking = true;
    
    [Tooltip("Skip Cards can be chained")]
    public bool enableSkipStacking = false;

    [Tooltip("Draw +4 Cards can be challenged")]
    public bool enableWildDrawChallenge = false;

    [Tooltip("A player should call it when he is got one card left")]
    public bool enableOneCardCall = true;

    [Tooltip("A player can play the drawn card")]
    public bool enableDrawQuickPlay = true;

    [Tooltip("A player is automatically is dealt a card when he has nothing to play")]
    public bool enableAutoDraw = true;
}
