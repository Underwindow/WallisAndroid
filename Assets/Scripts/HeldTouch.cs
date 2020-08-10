using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
public enum TouchTarget
{
    Ui,
    Screen
}

public class HeldTouch
{
    public readonly int id;
    public readonly TouchTarget target;
    public readonly int startFrame;
    public readonly Vector2 startPosition;
    public Vector2 position;
    public Vector2 prevPosition;
    public Vector2 deltaPosition;
    public int tapCount {
        get {
            var touches = Input.touches;
            return touches.Select(touch => touch.fingerId).Contains(id)
                ? touches.ToList().Find(touch => touch.fingerId == id).tapCount
                : 0;
        }
    }

    private float startTime;

    public float ExistenceTime 
        => Time.time - startTime;

    public int PassedFrames 
        => Time.frameCount - startFrame;

    public TouchPhase Phase
    {
        get 
            => Exists ? (PassedFrames > 0 ? TouchPhase.Moved : TouchPhase.Began)
            : TouchPhase.Ended;
        private set {; }
    }

    public bool Exists 
        => Input.touches
        .ToList()
        .Select(touch => touch.fingerId)
        .Contains(id);

    public HeldTouch(Touch touch)
    {
        id = touch.fingerId;
        startPosition = prevPosition = position = touch.position;
        target = EventSystem.current.IsPointerOverGameObject(touch.fingerId) ? TouchTarget.Ui : TouchTarget.Screen;
        Phase = TouchPhase.Began;
        startFrame = Time.frameCount;
        startTime = Time.time;
    }
}