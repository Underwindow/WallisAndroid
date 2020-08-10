using UnityEngine;

public class Projection
{
    private Transform figure;
    private Transform rotator;
    private Transform figureElement;
    public readonly Transform shadow;
    private Vector3 shadowStart;

    public GameObject GameObject
        => shadow?.gameObject;

    public Projection(Transform figure, Transform figureElement, Transform shadow)
    {
        this.figure = figure;
        rotator = figure.GetComponent<FigureMotion>().rotator;
        this.figureElement = figureElement;
        var pos =
            figure.GetComponent<FigureMotion>().startPos -
            figure.position +
            new Vector3(this.figureElement.position.x, this.figureElement.position.y);
        shadowStart = new Vector2(pos.x - figureElement.localPosition.x, pos.y - figureElement.localPosition.y);
        this.shadow = shadow;
    }

    public void Move(Joystick joystick, bool visible)
    {
        var localOffset = Vector3
            .Scale(figureElement.position - figure.position, new Vector3(1,1));
        shadowStart = Vector3
            .Scale(joystick.MoveShadow(shadow, shadowStart + localOffset, localOffset, visible) - localOffset, new Vector3(1, 1));
        
        shadow.rotation = Quaternion.Euler(rotator.rotation.eulerAngles.z * Vector3.forward);
    }
}

