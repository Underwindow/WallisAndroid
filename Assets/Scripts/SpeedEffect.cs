using System;
using System.Collections;
using UnityEngine;

public class SpeedEffect : MonoBehaviour
{
    [HideInInspector]
    public FigureMotion figure;
    ParticleSystem particles;
    float simulationSpeed;
    float desiredSpeed;
    ParticleSystem.MainModule main;
    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        main = particles.main;
        simulationSpeed = main.simulationSpeed;
        main.simulationSpeed = 1;
        desiredSpeed = 1;
        FindObjectOfType<Booster>().BoostEnabled += EnableEffect;
        FindObjectOfType<Booster>().BoostDisabled += DisableEffect;
    }

    // Update is called once per frame
    void Update()
    {
        if (figure is null)
            figure = FindObjectOfType<FigureMotion>();

        var figurePos = figure.transform.position;
        transform.position = new Vector3(figurePos.x, figurePos.y);
    }

    void EnableEffect(object sender, EventArgs e)
    {
        main.simulationSpeed = 1;

        var emission = particles.emission;
        emission.enabled = true;
        desiredSpeed = simulationSpeed;

        StartCoroutine(EffectCoroutine());
    }

    void DisableEffect(object sender, EventArgs e)
    {
        var emission = particles.emission;
        emission.enabled = false;

        desiredSpeed = 1;
    }

    IEnumerator EffectCoroutine()
    {
        while (particles.emission.enabled)
        {
            main.simulationSpeed = Mathf.Lerp(main.simulationSpeed, desiredSpeed, 0.1f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
