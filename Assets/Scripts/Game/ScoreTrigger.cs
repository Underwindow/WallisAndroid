using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ScoreTrigger : MonoBehaviour
{
    public Color scoreTextColor;
    int scoreValue = 50;
    bool collectable = true;
    AudioManager audioManager;
    [HideInInspector]
    public UnityEvent ScoreTriggered;

    private void Start()
    {
        FindObjectOfType<GameManager>().GameOver.AddListener(StopCollect);
        audioManager = FindObjectOfType<AudioManager>();
        GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    private void Update()
    {
        RaycastHit hit;
        if (
            transform.position.z < 2 &&
            Physics.Raycast(new Ray(transform.position, Vector3.back), out hit, 4) &&
            hit.transform.gameObject.layer == LayerMask.NameToLayer("Figure") && 
            collectable
            )
        {
            StartCoroutine(TriggerPrediction());
        }
    }

    IEnumerator TriggerPrediction()
    {
        while (transform.position.z > 0)
            yield return new WaitForSeconds(Time.deltaTime);

        print("PREDICTED");
        CollectScore();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Figure") && collectable)
            CollectScore();
    }

    void CollectScore()
    {
        audioManager.Play("Score");

        //GameManager.SCORE += scoreValue * GameManager.LEVEL * GameManager.SCORE_BONUS;
        GameManager.SCORE += scoreValue * GameManager.SCORE_BONUS;

        ScoreTriggered?.Invoke(); ScoreTriggered = null;
        gameObject.SetActive(false);
        enabled = false;
    }

    void StopCollect() 
        => collectable = false;
}