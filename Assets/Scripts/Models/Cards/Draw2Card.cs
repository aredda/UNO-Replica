public class Draw2Card 
    : EffectCard
{
    public override string GetMaterialName()
    {
        return $"{color.ToString()}_Draw";
    }

    public override bool IsPlayable(Card boardCard)
    {
        if(base.IsPlayable(boardCard))
            return base.IsPlayable(boardCard);
        
        return boardCard is Draw2Card;
    }
}
