using System;

[Serializable]
public class Draw2Card 
    : EffectCard
{
    public override string GetMaterialName()
    {
        return $"{color.ToString()}_Draw";
    }

    public override bool IsPlayable(Card boardCard, bool isDrawImposed = false)
    {
        if(!isDrawImposed && base.IsPlayable(boardCard))
            return base.IsPlayable(boardCard);
        
        return boardCard is Draw2Card;
    }

    public override void Activate(CardTemplate template, Action onFinish = null)
    {
        // Make the game master impose the draw on players
        template.hand.Master.ImposeDrawing();

        base.Activate(template, onFinish);
    }
}
