using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController 
    : MonoBehaviour
{
    public ECardColor selectedColor;
    public float changeSpeed = 5f;

    [Header("Mesh Reference")]
    public MeshRenderer meshRenderer;

    [Header("Board Color States")]
    public Color blue;
    public Color red;
    public Color green;
    public Color yellow;

    public Dictionary<ECardColor, Color> colors;

    public void Setup()
    {
        if(colors != null)
            return;
        
        this.colors = new Dictionary<ECardColor, Color>() {
            { ECardColor.Blue, blue },
            { ECardColor.Red, red },
            { ECardColor.Green, green },
            { ECardColor.Yellow, yellow }
        };
    }

    public void Change(ECardColor color)
    {
        if(this.colors != null && this.selectedColor == color)
            return;

        this.Setup();
        this.selectedColor = color;
        ManagerDirector.director.cardAnimator.ChangeBoardColor(this.meshRenderer.material, colors[color], changeSpeed);
    }
}
