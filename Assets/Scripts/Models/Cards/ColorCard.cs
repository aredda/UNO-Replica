[System.Serializable]
public abstract class ColorCard 
    : Card
{
    public ECardColor color;

    public override bool IsPlayable(Card boardCard, bool isDrawImposed = false)
    {
        if(boardCard is ColorCard)
            return ((ColorCard) boardCard).color == color;

        if(boardCard is WildCard)
            return ((WildCard) boardCard).chosenColor == color;
            
        return false;
    }
}
