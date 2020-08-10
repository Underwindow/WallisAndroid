using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector]
    public Vector3 movement = Vector2.zero;
    [HideInInspector]
    public UnityEvent MovementApplied = new UnityEvent();
    [HideInInspector]
    public Vector3 discreteMovement = Vector2.zero;
    [HideInInspector]
    public int touchId = -1;

    float sensitivity;
    float offset = .5f;

    bool moving = false;
    WallBuilder wall;
    TouchManager touchManager;
    List<HeldTouch> uiTouches =>
        touchManager
            .touches
            .Where(touch => touch.target == TouchTarget.Ui)
            .OrderBy(touch => touch.id)
            .ToList();


    Quaternion startAttitude;
    // Use this for initialization
    void Start()
    {
        startAttitude = GyroToUnity(Input.gyro.attitude);

        wall = FindObjectOfType<WallBuilder>();
        touchManager = FindObjectOfType<TouchManager>();
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 11) / Screen.width;
    }
    void Update()
    {
        MovePlayer();
    }

    private void LateUpdate()
    {
        if (!moving && discreteMovement != Vector3.zero)
            discreteMovement = movement = Vector2.zero;
    }

    void MovePlayer()
    {
        if (!moving)
            return;

        HeldTouch touch; 
        if ((touch = GetFinger()) != null)
        {
            var touchMovement = (touch.position - touch.startPosition) * sensitivity;
            movement = new Vector3(touchMovement.x, touchMovement.y) * offset;
            discreteMovement = new Vector3(Mathf.Round(touchMovement.x), Mathf.Round(touchMovement.y)) * offset;
        }
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    public HeldTouch GetFinger()
    {
        foreach (var touch in uiTouches)
            if (touch.id == touchId)
                return touch;
        return null;
    }

    public Vector3 MoveFigure(Transform figure, Vector3 startPos)
    {
        var figureDesiredPos = ClampMovement(startPos + discreteMovement);

        if (moving)
            figure.position = Vector3.Lerp(figure.position, figureDesiredPos, .125f);
        else
            startPos = figure.position = figureDesiredPos;

        return startPos;
    }

    public Vector3 MoveShadow(Transform shadow, Vector3 startPos, Vector3 localOffset, bool visible)
    {
        shadow.gameObject.SetActive(visible);

        shadow.position = localOffset + ClampMovement(startPos - localOffset + (moving? movement : discreteMovement)) + Vector3.forward * shadow.position.z;
        if (!moving)
            startPos = shadow.position;

        MoveTowardFigure(shadow);

        return startPos;
    }

    void MoveTowardFigure(Transform tr) => 
        tr.position -= Vector3.forward * GameManager.SPEED * Time.deltaTime;

    public Vector3 ClampMovement(Vector3 movement)
    {
        float border = (wall.wallSize - 1) * .25f;
        return 
            new Vector3(
                Mathf.Clamp(movement.x, -border, border), 
                Mathf.Clamp(movement.y, -border, border), 
                movement.z
                );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var newTouches = touchManager
            .GetTouches()
            .Where(touch => touch.target == TouchTarget.Ui)
            .OrderBy(touch => touch.id);

        foreach (var touch in newTouches)
        {
            if (touch.Phase == TouchPhase.Began)
            {
                touchId = touch.id;
                moving = true;
                break;
            }
        }
    }

    public void Active(bool value)
    {
        if (!value)
        {
            touchId = -1;
            moving = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MovementApplied.Invoke();
        Active(false);
    }
}
