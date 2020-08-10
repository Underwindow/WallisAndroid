using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RotationEventArgs : EventArgs
{
    public RotatingDirection Direction { get; set; }
    public RotationEventArgs(RotatingDirection direction) { Direction = direction; }
}

public enum TouchType
{
    None,
    Swiping,
    DoubleClick
}

public class FigureMotion : MonoBehaviour
{
    [HideInInspector]
    public Transform rotator;

    [HideInInspector]
    public event EventHandler<RotationEventArgs> FigureRotated;

    private Transform figure;
    [HideInInspector]
    public Vector3 startPos;
    private Joystick joystick;

    private Vector2 beganPos = Vector2.left;
    private Vector2 wheelStart;

    private Quaternion startRotation;

    private bool allowRotation = true;
    private TouchType touchType = TouchType.None;
    private TouchManager touchManager;
    List<HeldTouch> touches =>
        touchManager
            .touches
            .Where(touch => touch.target == TouchTarget.Screen)
            .OrderBy(touch => touch.id)
            .ToList();

    private void Awake()
    {
        rotator = GameObject.FindGameObjectWithTag("Rotator").transform;
    }

    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
        touchManager = FindObjectOfType<TouchManager>();
        FindObjectOfType<SpeedEffect>().figure = this;
        startPos = FindObjectOfType<GameManager>().prevFigureStartPos;
        figure = gameObject.transform;
        figure.position = joystick.ClampMovement(joystick.discreteMovement + startPos);

        startRotation = figure.localRotation;
        CheckExistingTouches();

        if (SystemInfo.supportsGyroscope)
            Input.gyro.enabled = true;
    }

    void Update()
    {
        UpdateRotation();
        startPos = joystick.MoveFigure(figure, startPos);
    }

    //Check if touches already exist
    void CheckExistingTouches()
    {
        switch (touches.Count())
        {
            case 1:
                beganPos = touches[0].position;
                break;
            case 2:
                beganPos = touches[0].position;
                wheelStart = touches[1].position - beganPos;
                break;
        }
    }

    void UpdateRotation()
    {
        if (touches.Any())
            Rotate();
        else
            allowRotation = true;
    }

    //void Rotate()
    //{
    //    if (allowRotation)
    //        switch (touches.Count())
    //        {
    //            case 1:
    //                RotateByFinger(touches);
    //                break;
    //            case 2:
    //                RotateByWheel(touches);
    //                break;
    //        }
    //}
    void Rotate()
    {
        if (allowRotation)
            switch (touches.Count())
            {
                case 1:
                    if (touchType == TouchType.Swiping || (touches[0].tapCount < 2 && touchType != TouchType.DoubleClick))
                        RotateByFinger(touches);
                    else if (touchType == TouchType.DoubleClick || touches[0].tapCount >= 2)
                        RotateByDoubleClick(touches);
                    break;
                case 2:
                    RotateByWheel(touches);
                    break;
            }
    }
    void RotateByDoubleClick(List<HeldTouch> trackedTouches)
    {
        var touch = trackedTouches[0];
        var sign = touch.position.x > Screen.width * .5f ? -1 : 1;
        if (touch.Phase == TouchPhase.Moved)
        {
            rotator.localRotation = startRotation;
            rotator.Rotate(Vector3.forward, sign * 90, Space.World);

            figure.localRotation = Quaternion.Lerp(figure.localRotation, rotator.localRotation, 180f / Screen.width);

            touchType = TouchType.DoubleClick;
        }
        if (touch.Phase == TouchPhase.Ended)
        {
            if (startRotation != figure.localRotation)
            {
                var direction = GetRotatingDirection(2, Vector2.zero, sign);
                ApplyRotation(direction);

            }

            touchType = TouchType.None;
            allowRotation = false;
        }
    }

    void RotateByWheel(List<HeldTouch> trackedTouches)
    {
        var t1 = trackedTouches[0];
        var t2 = trackedTouches[1];
        var wheelUpdate = t2.position - t1.position;

        if (t2.Phase == TouchPhase.Began)
            wheelStart = wheelUpdate;

        float signedAngle = -Vector2.SignedAngle(wheelUpdate, wheelStart);
        if ((t1.Phase == TouchPhase.Moved) || (t2.Phase == TouchPhase.Moved))
        {

            float currAngle = Vector2.Angle(wheelUpdate, wheelStart);
            rotator.localRotation = startRotation;
            if (currAngle > 10 || (t2.deltaPosition.magnitude + t1.deltaPosition.magnitude) * Time.deltaTime > 1)
                rotator.Rotate(Vector3.forward, Math.Sign(signedAngle) * 90, Space.World);

            figure.localRotation = Quaternion.Lerp(figure.localRotation, rotator.localRotation, 180f / Screen.width);
        }
        if (t1.Phase == TouchPhase.Ended || t2.Phase == TouchPhase.Ended)
        {
            if (startRotation != figure.localRotation)
            {
                var direction = GetRotatingDirection(2, Vector2.zero, signedAngle);
                ApplyRotation(direction);
            }

            allowRotation = false;
        }
    }

    void RotateByFinger(List<HeldTouch> trackedTouches)
    {
        var trackedTouch = trackedTouches[0];

        if (trackedTouch.Phase == TouchPhase.Began || beganPos == Vector2.left)
            beganPos = trackedTouch.position;

        var movement = trackedTouch.position - beganPos;

        if (trackedTouch.Phase == TouchPhase.Moved)
        {
            rotator.localRotation = startRotation;
            if (movement.magnitude > 50 || trackedTouch.deltaPosition.magnitude * Time.deltaTime > .5f)
            {
                var rotVector = Mathf.Abs(movement.x) >= Mathf.Abs(movement.y)
                   ? new Vector3(0, -Mathf.Sign(movement.x) * 90, 0)
                   : new Vector3(Mathf.Sign(movement.y) * 90, 0, 0);
                rotator.Rotate(rotVector, Space.World);
            }
            figure.localRotation = Quaternion.Lerp(figure.localRotation, rotator.localRotation, 180f / Screen.width);
        }

        if (trackedTouch.Phase == TouchPhase.Ended)
        {
            if (startRotation != figure.localRotation)
            {
                var direction = GetRotatingDirection(1, movement);
                ApplyRotation(direction);

                touchType = TouchType.Swiping;
            }
            else
                touchType = TouchType.None;

            allowRotation = false;
        }
    }

    public void ApplyRotation(RotatingDirection direction)
    {
        FigureRotated?.Invoke(this, new RotationEventArgs(direction));

        var currAngle = Quaternion.Angle(startRotation, figure.localRotation);

        figure.localRotation = startRotation;

        if (currAngle > 5)
        {
            figure.Rotate(Rotations.eulers[direction], Space.World);
            startRotation = figure.localRotation;
        }

        rotator.rotation = Quaternion.identity;
        beganPos = Vector2.left;
    }

    RotatingDirection GetRotatingDirection(int touchesCount, Vector2 touchMovement, float signedAngle = 0)
    {
        RotatingDirection direction = RotatingDirection.None;
        switch (touchesCount)
        {
            case 1:
                var xAbs = Mathf.Abs(touchMovement.x);
                var yAbs = Mathf.Abs(touchMovement.y);
                if (xAbs > yAbs)
                {
                    var s = Mathf.Sign(touchMovement.x);
                    if (s != 0)
                        direction = s > 0 ? RotatingDirection.Right : RotatingDirection.Left;
                }
                else if (xAbs < yAbs)
                {
                    var s = Mathf.Sign(touchMovement.y);
                    if (s != 0)
                        direction = s > 0 ? RotatingDirection.Up : RotatingDirection.Down;
                }
                break;

            case 2:
                if (signedAngle != 0)
                    direction = signedAngle > 0 ? RotatingDirection.LeftAround : RotatingDirection.RightAround;
                break;
        }

        Debug.Log(direction.ToString());
        return direction;
    }
}
