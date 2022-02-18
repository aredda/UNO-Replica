[System.Serializable]
public abstract class Card
{
    public virtual void Activate(CardTemplate template, System.Action onFinish = null)
    {
        if(onFinish != null)
            onFinish.Invoke();
    }
    public abstract string GetMaterialName();
    public override string ToString()
    {
        return this.GetMaterialName();
    }
    public abstract bool IsPlayable(Card boardCard, bool isDrawImposed = false);
}
