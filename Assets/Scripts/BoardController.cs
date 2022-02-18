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

    [Header("Arrow Settings")]
    public GameObject arrow;
    public float arrowRotationSpeed = 5f;

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

        StartCoroutine(RoutineRotateArrows());
    }

    public void Change(ECardColor color)
    {
        if(this.colors != null && this.selectedColor == color)
            return;

        this.Setup();
        this.selectedColor = color;
        ManagerDirector.director.cardAnimator.ChangeBoardColor(this.meshRenderer.material, colors[color], changeSpeed);
    }

    public bool CanTurn()
    {
        return true;
    }

    public void ReverseArrows()
    {
        Vector3 newScale = arrow.transform.localScale;
        newScale.x *= -1;
        // Flip the image horizontally
        arrow.transform.localScale = newScale;
    }

    IEnumerator RoutineRotateArrows()
    {
        do
        {
            int turnDirection = ManagerDirector.director.gameMaster.turnDirection;
            // Rotate the arrows
            arrow.transform.rotation = Quaternion.Euler(arrow.transform.rotation.eulerAngles + Vector3.up * -arrowRotationSpeed * turnDirection * Time.deltaTime); 

            yield return new WaitForEndOfFrame();
        }
        while(CanTurn());
    }
}
