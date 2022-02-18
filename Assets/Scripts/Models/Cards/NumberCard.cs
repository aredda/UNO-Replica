public class NumberCard 
    : ColorCard
{
    public int number;

    public override string GetMaterialName()
    {
        return $"{this.color.ToString()}_{this.number}";
    }

    public override bool IsPlayable(Card boardCard)
    {
        if(base.IsPlayable(boardCard))
            return base.IsPlayable(boardCard);
        
        if(boardCard is NumberCard)
            return ((NumberCard) boardCard).number == number;

        return false;
    }
}
