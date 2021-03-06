using System;

[Serializable]
public class Draw4Card 
    : WildCard
{
    public override string GetMaterialName()
    {
        return "Wild_Draw";
    }

    public override void Activate(CardTemplate template, Action onFinish = null)
    {
        // Impose drawing 4 cards
        template.hand.Master.ImposeDrawing(4);

        base.Activate(template, onFinish);
    }
}
