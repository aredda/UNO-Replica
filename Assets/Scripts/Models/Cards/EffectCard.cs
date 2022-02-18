public abstract class EffectCard 
    : ColorCard
{
    /**
    Types of Effect Cards:
    - Draw 2 Cards
    - Skip Next Player Turn
    - Reverse Turn Direction
    */
    public override bool IsPlayable(Card boardCard)
    {
        return base.IsPlayable(boardCard);
    }
}
