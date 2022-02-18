using System;

public class ReverseCard 
    : EffectCard
{
    public override string GetMaterialName()
    {
        return $"{color.ToString()}_Reverse";
    }

    public override bool IsPlayable(Card boardCard)
    {
        if(base.IsPlayable(boardCard))
            return base.IsPlayable(boardCard);
        
        return boardCard is ReverseCard;
    }

    public override void Activate(CardTemplate template, Action onFinish = null)
    {
        // Reverse turn direction
        template.hand.master.turnDirection *= -1;
        // Call base activation
        base.Activate(template, onFinish);
    }
}
