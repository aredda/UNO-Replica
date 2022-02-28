[System.Serializable]
public abstract class EffectCard 
    : ColorCard
{
    /**
    Types of Effect Cards:
    - Draw 2 Cards
    - Skip Next Player Turn
    - Reverse Turn Direction
    */
    public override bool IsPlayable(Card boardCard, bool isDrawImposed = false)
    {
        return base.IsPlayable(boardCard);
    }
}
