using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public List<HeldTouch> touches = new List<HeldTouch>();

    private void Start()
    {
        GetTouches();
    }

    private void Update()
    {
        GetTouches();
    }

    private void LateUpdate()
    {
        ClearEndedTouches();
    }

    public List<HeldTouch> GetTouches()
    {
        var input = Input.touches.ToList();
        if (touches.Any())
        {
            var newTouchesIds = input
                .Select(i => i.fingerId)
                .Except(touches.Select(t => t.id))
                .ToList();

            // Adding new touches
            newTouchesIds.ForEach(
                id => touches.Add(new HeldTouch(input.Find(i => i.fingerId == id)))
            );

            //Set actual position for touches
            foreach (var touch in touches)
            {
                touch.position = touch.Exists ? input.Find(i => i.fingerId == touch.id).position : touch.position;
                touch.deltaPosition = touch.position - touch.prevPosition;
                touch.prevPosition = touch.position;
            }
        }
        else
            input.ForEach(
                touch => touches.Add(new HeldTouch(touch))
            );

        return 
            touches
            .OrderBy(touch => touch.startFrame)
            .ToList();
    }

    public void ClearEndedTouches() 
        => touches = touches.Where(touch => touch.Exists).ToList();
}
