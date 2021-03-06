using System;

[Serializable]
public class ReverseCard 
    : EffectCard
{
    public override string GetMaterialName()
    {
        return $"{color.ToString()}_Reverse";
    }

    public override bool IsPlayable(Card boardCard, bool isDrawImposed = false)
    {
        if(isDrawImposed)
            return false;

        if(base.IsPlayable(boardCard))
            return base.IsPlayable(boardCard);
        
        return boardCard is ReverseCard;
    }

    public override void Activate(CardTemplate template, Action onFinish = null)
    {
        // Reverse turn direction
        template.hand.Master.ReverseTurnDirection();
        // Call base activation
        base.Activate(template, onFinish);
    }
}
