using UnityEngine;

public class Gesture
{
    public readonly string name;
    public Sprite sprite;
    
    public Gesture(Sprite source)
    {
        sprite = source;
        name = source.name;
    }

}
