using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTemplateAnimator 
    : Manager
{
    [Header("Play Card Animation")]
    public float liftSpeed = 8f;
    public float liftOffset = 1.75f;
    public float setSpeed = 10f;
    public Vector3 targetPosition = new Vector3(0, 0.1f, 0);
    public Vector3 targetRotation = new Vector3(90f, 0, 0);
    public Vector3 targetScale = new Vector3(1.5f, 2f, 1f);

    [Header("Draw Card Animation")]
    public float addSpeed = 10f;
    public Vector3 handRotation = new Vector3(45f, 0, 0);
    public Vector3 handScale = new Vector3(0.75f, 1f, 1f);

    [Header("Change Card Color Animation")]
    public float changeColorSpeed = 5f;

    public void PlayCard(CardTemplate template, System.Action onFinish)
    {
        StartCoroutine(RoutinePlayCard(template, onFinish));
    }

    IEnumerator RoutinePlayCard(CardTemplate template, System.Action onFinish)
    {
        Transform card_transform = template.transform;
        // Lift Animation
        Vector3 start_position = card_transform.position;
        Vector3 lift_destination = start_position + Vector3.up * liftOffset;
        float distance = Vector3.Distance(start_position, lift_destination);
        do
        {
            card_transform.position = Vector3.Lerp(card_transform.position, lift_destination, liftSpeed * Time.deltaTime);
            distance = Vector3.Distance(card_transform.position, lift_destination);

            yield return new WaitForEndOfFrame();
        }
        while(distance > 0.1f);
        // Put On Table Animation
        start_position = card_transform.position;
        distance = Vector3.Distance(start_position, targetPosition);
        float elapsedTime = 0f;
        do
        {
            card_transform.position = Vector3.Lerp(card_transform.position, targetPosition, elapsedTime / setSpeed);
            card_transform.rotation = Quaternion.Euler(Vector3.Lerp(card_transform.rotation.eulerAngles, targetRotation, elapsedTime / setSpeed));
            card_transform.localScale = Vector3.Lerp(card_transform.localScale, targetScale, elapsedTime / setSpeed);

            distance = Vector3.Distance(card_transform.position, targetPosition);
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        while(distance > 0.1f);
        // Animation OnFinish Callback
        if(onFinish != null)
            onFinish.Invoke();
    }

    public void DrawCard(CardTemplate template, Vector3 cardPosition, Vector3 parentRotation, System.Action onFinish)
    {
        StartCoroutine(RoutineDrawCard(template, cardPosition, parentRotation, onFinish));
    }

    IEnumerator RoutineDrawCard(CardTemplate template, Vector3 handPosition, Vector3 parentRotation, System.Action onFinish)
    {
        // Place card on initial position
        Transform card_transform = template.transform;
        card_transform.position = director.gameMaster.deck.position;
        card_transform.rotation = Quaternion.Euler(new Vector3(270f, 0 , 0));
        card_transform.localScale = targetScale;
        // Enable template
        template.Enable();
        // Lift Animation
        Vector3 start_position = card_transform.position;
        Vector3 lift_destination = start_position + Vector3.up * liftOffset;
        float distance = Vector3.Distance(start_position, lift_destination);
        do
        {
            card_transform.position = Vector3.Lerp(card_transform.position, lift_destination, liftSpeed * Time.deltaTime);
            distance = Vector3.Distance(card_transform.position, lift_destination);

            yield return new WaitForEndOfFrame();
        }
        while(distance > 0.1f);
        // Add to hand Animation
        start_position = card_transform.position;
        distance = Vector3.Distance(start_position, handPosition);
        float elapsedTime = 0f;
        do
        {
            // Position Setting
            card_transform.position = Vector3.Lerp(card_transform.position, handPosition, elapsedTime / addSpeed);
            // Rotation Setting
            float xRot = Mathf.LerpAngle(card_transform.rotation.eulerAngles.x, handRotation.x, elapsedTime / setSpeed);
            float yRot = parentRotation.y;
            float zRot = parentRotation.z;
            card_transform.rotation = Quaternion.Euler(xRot, yRot, zRot);
            // Scale Setting
            card_transform.localScale = Vector3.Lerp(card_transform.localScale, handScale, elapsedTime / setSpeed);

            distance = Vector3.Distance(card_transform.position, handPosition);
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        while(distance > 0.001f);
        // OnFinishhCallback
        if(onFinish != null)
            onFinish.Invoke();
    }

    public void ChangeCardColor(CardTemplate template, Color targetColor, System.Action onFinish = null)
    {
        StartCoroutine(RoutineChangeColor(template.faceMeshRenderer.material, targetColor, changeColorSpeed, onFinish));
    }

    public void ChangeBoardColor(Material boardMaterial, Color targetColor, float speed, System.Action onFinish = null)
    {
        StartCoroutine(RoutineChangeColor(boardMaterial, targetColor, speed, onFinish));
    }

    IEnumerator RoutineChangeColor(Material material, Color targetColor, float speed, System.Action onFinish = null)
    {
        float timeElapsed = 0f;
        do
        {
            material.color = Color.Lerp(material.color, targetColor, timeElapsed / speed);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while(!(Mathf.Approximately(material.color.r, targetColor.r) 
            && Mathf.Approximately(material.color.g, targetColor.g) 
            && Mathf.Approximately(material.color.b, targetColor.b) 
            && Mathf.Approximately(material.color.a, targetColor.a)));
        // Call back
        if(onFinish != null)
            onFinish.Invoke();
    }
}
