public class SkipCard 
    : EffectCard
{
    public override string GetMaterialName()
    {
        return $"{color.ToString()}_Skip";
    }

    public override bool IsPlayable(Card boardCard)
    {
        if(base.IsPlayable(boardCard))
            return base.IsPlayable(boardCard);
        
        return boardCard is SkipCard;
    }
}
