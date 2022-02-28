using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

    #region Serialization
    public virtual byte[] Serialize()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream()) {
            formatter.Serialize(stream, this);
            return stream.ToArray();
        }
    }

    public static Card Deserialize(byte[] bytes)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using(MemoryStream stream = new MemoryStream()) {
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return (Card)formatter.Deserialize(stream);
        }
    }

    #endregion
}
