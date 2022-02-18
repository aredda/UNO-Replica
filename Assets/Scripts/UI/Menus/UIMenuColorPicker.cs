using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuColorPicker 
    : MonoBehaviour
{
    [Header("Card Reference")]
    public CardTemplate cardTemplate;

    [Header("Button References")]
    public Button buttonRed;
    public Button buttonBlue;
    public Button buttonGreen;
    public Button buttonYellow;

    public void Show(CardTemplate template, System.Action onFinish = null)
    {
        this.cardTemplate = template;

        if(this.cardTemplate == null)
            throw new System.Exception("UIMenuColorPicker.Show#Exception: Card template is missing");
        
        if(!(this.cardTemplate.card is WildCard))
            throw new System.Exception("UIMenuColorPicker.Show#Exception: Can't choose a color to a non-Wild card");

        Dictionary<ECardColor, Button> buttons = new Dictionary<ECardColor, Button>() 
        {
            { ECardColor.Blue, buttonBlue },
            { ECardColor.Red, buttonRed },
            { ECardColor.Green, buttonGreen },
            { ECardColor.Yellow, buttonYellow }
        };
        foreach(ECardColor color in buttons.Keys)
        {
            buttons[color].onClick.RemoveAllListeners();
            buttons[color].onClick.AddListener(delegate() 
            {
                WildCard wildCard = (WildCard) this.cardTemplate.card;
                wildCard.chosenColor = color;
                this.cardTemplate.card = wildCard;
                // onFinish callback
                if(onFinish != null)
                    onFinish.Invoke();
                // hide menu
                this.gameObject.SetActive(false);
            });
        }

        this.gameObject.SetActive(true);
    }
}
