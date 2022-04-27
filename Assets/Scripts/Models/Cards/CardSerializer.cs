using Mirror;

public static class CardSerializer
{
    public static void Write(this NetworkWriter writer, Card card)
    {
        // we need to preserve exact type
        System.Type type = typeof(Card);
        // if the card is colored
        if (card is ColorCard)
        {
            // write color
            writer.WriteInt((int) ((ColorCard) card).color);
            // if card is numbered
            if(card is NumberCard)
            {
                writer.WriteInt(((NumberCard) card).number);
                type = typeof(NumberCard);
            }
            // effect types
            else if(card is EffectCard)
                type = card.GetType();
        }
        // if card is a wild one
        else if(card is WildCard)
        {
            WildCard wildCard = (WildCard) card;
            // write chosen color
            writer.Write((int)wildCard.chosenColor);
            // if it's for color switch
            type = card is SwitchCard ? typeof(SwitchCard) : typeof(Draw4Card);
        }
        // write the type's name
        writer.WriteString(nameof(type));
    }

    public static Card Read(this NetworkReader reader)
    {
        switch(reader.ReadString())
        {
            case nameof(NumberCard):
                return new NumberCard {
                    color = (ECardColor)reader.ReadInt(),
                    number = reader.ReadInt()
                };
            case nameof(Draw2Card):
                return new Draw2Card {
                    color = (ECardColor)reader.ReadInt()
                };
            case nameof(SkipCard):
                return new SkipCard {
                    color = (ECardColor)reader.ReadInt()
                };
            case nameof(ReverseCard):
                return new ReverseCard {
                    color = (ECardColor)reader.ReadInt()
                };
            case nameof(SwitchCard):
                return new SwitchCard {
                    chosenColor = (ECardColor)reader.ReadInt()
                };
            case nameof(Draw4Card):
                return new Draw4Card {
                    chosenColor = (ECardColor)reader.ReadInt()
                };
        }
        return null;
    }
}
