using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardTemplateAnimator 
    : Manager
{
    [Header("Time Settings")]
    public float androidDeltaTime = 0.075f;

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

    [Header("Card Sort Animation")]
    public float sortCardSpeed = 4f;

    [Header("Draw Total Animation")]
    public float moveDrawTotalSpeed = 3f;

    #region Delta Time Settings

    public float DeltaTime 
    {
        get
        {
            if(Application.platform == RuntimePlatform.Android)
                return androidDeltaTime;
            
            return Time.deltaTime;
        }
    }

    #endregion

    # region Routine Pooling

    private Dictionary<string, List<IEnumerator>> routinePool = new Dictionary<string, List<IEnumerator>> ();
    private List<RoutineDetail> detailedRoutinePool = new List<RoutineDetail> ();

    public void AddRoutine(string name, IEnumerator routine)
    {
        if(!routinePool.ContainsKey(name))
            this.routinePool.Add(name, new List<IEnumerator>());
        
        this.routinePool[name].Add(routine);
    }

    public void AddRoutine(string name, GameObject gameObject, IEnumerator routine)
    {
        this.detailedRoutinePool.Add(new RoutineDetail() {
            routineName = name,
            gameObject = gameObject,
            enumerator = routine
        });
    }

    public void StopAllRoutines(string name)
    {
        if(!routinePool.ContainsKey(name))
            return;
        
        foreach(IEnumerator routine in this.routinePool[name])
            StopCoroutine(routine);
        
        routinePool[name].Clear();
    }

    public void StopAllRoutines(string name, GameObject gameObject)
    {
        // Stop all routines
        foreach(RoutineDetail detail in detailedRoutinePool.Where(r => r.Concerns(gameObject) && r.Matches(name)))
            StopCoroutine(detail.enumerator);
        // Remove from list
        detailedRoutinePool.RemoveAll(r => r.Concerns(gameObject) && r.Matches(name));
    }

    #endregion

    public void PlayCard(CardTemplate template, System.Action onFinish)
    {
        StartCoroutine(RoutinePlayCard(template, onFinish));
    }

    IEnumerator RoutinePlayCard(CardTemplate template, System.Action onFinish)
    {
        // Retrieve template's transform
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
            elapsedTime += DeltaTime;

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
            elapsedTime += DeltaTime;

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
        // Store routine
        IEnumerator routine = RoutineChangeColor(boardMaterial, targetColor, speed, onFinish);
        // Stop all routines
        StopAllRoutines("boardColor");
        // Add routine
        AddRoutine("boardColor", routine);
        // Start routine
        StartCoroutine(routine);
    }

    IEnumerator RoutineChangeColor(Material material, Color targetColor, float speed, System.Action onFinish = null)
    {
        float timeElapsed = 0f;
        do
        {
            material.color = Color.Lerp(material.color, targetColor, timeElapsed / speed);
            timeElapsed += DeltaTime;
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

    public void ChangePlayerCardScale(Transform transform, Vector3 targetScale, System.Action onFinish = null, float speed = 5f)
    {
        StartCoroutine(RoutineChangeScale(transform, targetScale, onFinish, speed));
    }

    IEnumerator RoutineChangeScale(Transform transform, Vector3 targetScale, System.Action onFinish = null, float speed = 5f)
    {
        float timeElapsed = 0f;
        float distance = Vector3.Distance(transform.localScale, targetScale);
        do
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, timeElapsed / speed);
            distance = Vector3.Distance(transform.localScale, targetScale);

            timeElapsed += DeltaTime;
            yield return new WaitForEndOfFrame();
        }
        while(distance > 0.001f);
        // call back
        if(onFinish != null)
            onFinish.Invoke();
    }

    public void SetDeltaTime(string deltaTime)
    {
        androidDeltaTime = float.Parse(deltaTime);
    }

    public void MoveCard(Transform transform, Vector3 target, System.Action onFinish = null)
    {
        IEnumerator routine = RoutineMoveCard(transform, target, onFinish, sortCardSpeed);
        // Stop move routines that concern this object
        StopAllRoutines("moveCard", transform.gameObject);
        // Add routine
        AddRoutine("moveCard", transform.gameObject, routine);
        // Start routine
        StartCoroutine(routine);
    }

    IEnumerator RoutineMoveCard(Transform transform, Vector3 target, System.Action onFinish = null, float speed = 3f)
    {
        // Move Animation
        float distance = Vector3.Distance(transform.localPosition, target);
        do
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, target, speed * DeltaTime);
            distance = Vector3.Distance(transform.localPosition, target);

            yield return new WaitForEndOfFrame();
        }
        while(distance > 0.005f);
        // call back
        if(onFinish != null)
            onFinish.Invoke();
    }

    public void MoveDrawTotalText(RectTransform transform, Vector2 target, System.Action onFinish = null)
    {
        IEnumerator routine = RoutineMoveUI(transform, target, onFinish, moveDrawTotalSpeed);
        // pooling
        StopAllRoutines("moveDrawTotal");
        AddRoutine("moveDrawTotal", routine);
        // start routine
        StartCoroutine(routine);
    }

    IEnumerator RoutineMoveUI(RectTransform transform, Vector2 target, System.Action onFinish = null, float speed = 3f)
    {
        // Move Animation
        float distance = Vector2.Distance(transform.anchoredPosition, target);
        do
        {
            transform.localPosition = Vector2.Lerp(transform.anchoredPosition, target, speed * DeltaTime);
            distance = Vector2.Distance(transform.anchoredPosition, target);

            yield return new WaitForEndOfFrame();
        }
        while(distance > 0.005f);
        // call back
        if(onFinish != null)
            onFinish.Invoke();
    }
}
