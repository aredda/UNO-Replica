using System;

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

    public override void Activate(CardTemplate template, Action onFinish = null)
    {
        template.hand.master.turnStep ++;

        base.Activate(template, onFinish);
    }
}
