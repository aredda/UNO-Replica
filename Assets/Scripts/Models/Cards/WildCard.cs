[System.Serializable]
public abstract class WildCard 
    : Card
{
    /**
    Types of Wild Cards:
    - Make next player draw 4 Cards
    - Switch color
    */
    public ECardColor chosenColor;

    public override string GetMaterialName()
    {
        return "Wild";
    }

    public override bool IsPlayable(Card boardCard, bool isDrawImposed = false)
    {
        return true;
    }

    public override void Activate(CardTemplate template, System.Action onFinish = null)
    {
        base.Activate(template, onFinish);

        // TODO: fix this patch
        // When the player is actually a bot, choose the dominant color in his hand
        //if(template.hand.player.isBot)
        //{
        //    // Set the color
        //    WildCard wildCard = template.card as WildCard;
        //    wildCard.chosenColor = template.hand.GetDominantColor();
        //    // Invoke the callback
        //    if(onFinish != null)
        //        onFinish.Invoke();
        //}
        // TODO: fix this because it will break on offline mode
        //else
        //{
        //    if (ManagerDirector.director.gameMaster.isOnline && template.hand.player.agent.isLocalPlayer)
        //    {
        //        // Change the player's state
        //        template.hand.player.state = PlayerState.DecidingColor;
        //        // Show menu
        //        ManagerDirector.director.uiManager.menuColorPicker.Show(template, onFinish);
        //    }
        //}
    }
}
