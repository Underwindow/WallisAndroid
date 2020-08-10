using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Stars : MonoBehaviour
{
    ParticleSystem particle;
    UnityEvent StarsUpdated = new UnityEvent();
    private void Start()
    {
        StarsUpdated.AddListener(MoveStars);
        FindObjectOfType<GameManager>().GameOver.AddListener(StopMoving);
        particle = GetComponent<ParticleSystem>();
    }
    // Update is called once per frame
    void Update()
    {
        StarsUpdated.Invoke();
    }

    void MoveStars()
    {
        var ratio = GameManager.SPEED / GameManager.START_SPEED;
        var main = particle.main;
        main.simulationSpeed = ratio;

        //transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z); ;
    }

    void StopMoving()
    {
        StarsUpdated.RemoveListener(MoveStars);

        StartCoroutine(Stop(.23f));
    }



    IEnumerator Stop(float stoppingTime)
    {
        var startTime = Time.time;
        var ratio = GameManager.SPEED / GameManager.START_SPEED;
        var main = particle.main;

        while (Time.time - startTime < stoppingTime)
        {
            var temp = Mathf.Lerp(ratio, 0, (Time.time - startTime) / stoppingTime);

            main.simulationSpeed = temp;

            yield return null;
        }
    }
}
