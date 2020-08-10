using UnityEngine;

public class CameraCollisionController : MonoBehaviour
{
    public float collisionOffset;
    BoxCollider camCollider;

    // Start is called before the first frame update
    void Start()
    {
        var cam = GetComponent<Camera>();
        var ratio = collisionOffset / 1000 * (1.5f);

        camCollider = GetComponent<BoxCollider>();
        camCollider.center = Vector3.forward * (cam.nearClipPlane + collisionOffset);
        camCollider.size = new Vector3(Screen.width * ratio, Screen.height * ratio, 0.1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (
            !camCollider.isTrigger && 
            (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || (collision.gameObject.layer == LayerMask.NameToLayer("Figure")))
            )
        {
            collision.collider.isTrigger = false;
            collision.collider.attachedRigidbody.isKinematic = false;
            FindObjectOfType<AudioManager>().Play("KnocksCamera");
        }
    }
}
