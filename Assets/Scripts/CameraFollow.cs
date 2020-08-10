using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    private Vector3 smoothVector;
    private void Start()
    {
        smoothVector = target.position;
    }
    void Update()
    {
        transform.LookAt(smoothVector = Vector3.Lerp(smoothVector, target.position.z < 1? Vector3.forward * 40 : target.position, .05f));

        //// Same as above, but setting the worldUp parameter to Vector3.left in this example turns the camera on its side
        //transform.LookAt(target, Vector3.left);
    }
}
