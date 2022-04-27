using System;

[Serializable]
public class SkipCard
    : EffectCard
{
    public override string GetMaterialName()
    {
        return $"{color.ToString()}_Skip";
    }

    public override bool IsPlayable(Card boardCard, bool isDrawImposed = false)
    {
        if(isDrawImposed)
            return false;

        if(base.IsPlayable(boardCard))
            return base.IsPlayable(boardCard);
        
        return boardCard is SkipCard;
    }

    public override void Activate(CardTemplate template, Action onFinish = null)
    {
        template.hand.Master.turnStep ++;

        base.Activate(template, onFinish);
    }
}
