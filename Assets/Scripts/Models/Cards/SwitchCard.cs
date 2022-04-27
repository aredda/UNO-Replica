[System.Serializable]
public class SwitchCard 
    : WildCard
{
    public override bool IsPlayable(Card boardCard, bool isDrawImposed = false)
    {
        if(isDrawImposed)
            return false;

        return true;
    }
}
