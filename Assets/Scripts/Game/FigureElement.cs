using UnityEngine;

public class FigureElement : MonoBehaviour
{
    [SerializeField]
    private Object shadowPrefab;

    private Projection projection;
    private Joystick joystick;
    bool physDisabled = true;
    float distance;


    private void Awake()
    {
        if (GameManager.LEVEL >= GameManager.MAX_LEVEL)
            GetComponent<Renderer>().material.mainTexture = Resources.Load("Textures/Squares/Figure/1") as Texture;
        else
            GetComponent<Renderer>().material.mainTexture = Resources.Load($"Textures/Squares/{GameManager.LEVEL}lvl/1") as Texture;

        //GetComponent<Renderer>().material = Resources.Load("Materials/malachite", typeof(Material)) as Material;
    }

    private void Start()
    {
        var rb = GetComponent<Rigidbody>();
            rb.angularDrag = 0;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        transform.rotation = Quaternion.Euler(0, 0, 90);

        distance = FindObjectOfType<WallBuilder>().wallDistance;

        joystick = FindObjectOfType<Joystick>();

        RaycastHit hit;
        LayerMask mask = ~LayerMask.GetMask("Projection", "Figure");
        Vector3 wallPos = new Vector3(0, 0, distance);
        var rayDir = wallPos - transform.position;
        
        if (Physics.Raycast(transform.position, rayDir, out hit, distance * 2, mask))
        {
            var offset = transform.position + Vector3.forward * hit.point.z;
            var shadowPos = offset + (shadowPrefab as GameObject).transform.position;

            var shadow = Instantiate(shadowPrefab, shadowPos, Quaternion.identity, null) as GameObject;
            shadow.SetActive(false);
            projection = new Projection(transform.parent, transform, shadow.transform);
        }
        else
            Debug.Log("How bleat");

        FindObjectOfType<GameManager>().GameOver.AddListener(EnablePhysics);
    }

    private void Update()
    {
        if (projection?.GameObject != null)
        {
            var visible = !ThroughFigure() && transform.position.z < projection.shadow.position.z;
            projection.Move(joystick, visible);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            EnableWallGravity(other as BoxCollider);
            
            if (physDisabled)
            {
                EnablePhysics();
                physDisabled = false;
            }

            GameOver();
        } 
        else if (other.gameObject.layer == LayerMask.NameToLayer("Projection"))
            StopProjection();
    }

    void GameOver()
    {
        var gameOverEvent = FindObjectOfType<GameManager>().GameOver;
        gameOverEvent?.Invoke();
        FindObjectOfType<GameManager>().GameOver = null;

        enabled = false;
    }

    private void OnDestroy()
    {
        StopProjection();
    }

    void StopProjection()
    {
        if (projection?.shadow != null)
            Destroy(projection.GameObject);
        projection = null;
    }

    bool ThroughFigure()
    {
        RaycastHit hit;
        LayerMask mask = ~LayerMask.GetMask("Projection");

        bool hitSelf = false;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, distance, mask))
        {
            hitSelf =
                LayerMask.LayerToName(hit.collider.gameObject.layer) == "Figure" &&
                new Vector2(transform.position.x - hit.transform.position.x, transform.position.y - hit.transform.position.y).magnitude < .01f;
        }

        return hitSelf;
    }

    public void EnablePhysics()
    {
        StopProjection();

        transform.parent = null;
        var collider = GetComponent<BoxCollider>();
        collider.size = Vector3.one * .99f;
        var rb = collider.attachedRigidbody;

        rb.AddForce(
            Vector3.forward * 2 + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)).normalized,
            ForceMode.Impulse
            );
    }

    void EnableWallGravity(BoxCollider collider)
    {
        collider.transform.parent = null;
        collider.isTrigger = false;
        collider.attachedRigidbody.isKinematic = false;
        collider.attachedRigidbody.AddForce(
            Vector3.back * 2 + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)).normalized,
            ForceMode.Impulse
            );
    }
}
