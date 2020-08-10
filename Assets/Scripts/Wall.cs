using UnityEngine;
using UnityEngine.Events;

public class Wall : MonoBehaviour
{
    float destroyDistance = 4f;
    [HideInInspector]
    public UnityEvent WallDestroyed = new UnityEvent();
    // Update is called once per frame
    void Update()
    {
        transform.position -= Vector3.forward * GameManager.SPEED * Time.deltaTime;
        if (transform.position.z < -destroyDistance)
        {
            WallDestroyed.Invoke();
            Destroy(gameObject);
        }
    }
}
