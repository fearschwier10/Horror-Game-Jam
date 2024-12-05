using System.Collections;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public float flickerDuration = 5f; // Duration for the flickering effect (default 5 seconds)
    private bool isFlickering = false; // To check if the light is currently flickering
    private Light lightSource; // Reference to the Light component

    [Tooltip("This is how big the light is. Experiment with it.")] public float scale;
    [Tooltip("The moves (new targets for properties; intensity, range, position) your light will do per second.")] public float speed;

    [HideInInspector] public bool MakeSourceStationary;
    [HideInInspector] public float positionOffset;

    private float intensityOrigin;
    private float intensityOffset;
    private float intensityDelta;
    private float rangeOrigin;
    private float rangeOffset;
    private float rangeTarget;
    private float rangeDelta;

    private Vector3 positionOrigin;
    private Vector3 positionDelta;

    private bool setNewTargets;

    private float deltaSum = 0;

    void Start()
    {
        lightSource = GetComponent<Light>(); // Get the Light component on the GameObject
        intensityOrigin = lightSource.intensity;
        rangeOrigin = lightSource.range;
        positionOrigin = transform.position;

        setNewTargets = true;

        scale *= 0.1f;
        speed *= 0.02f;

        intensityOffset = lightSource.intensity * scale;
        rangeOffset = lightSource.range * scale;
        positionOffset *= scale * 0.1f;
    }

    public void StartFlickering()
    {
        if (!isFlickering)
        {
            isFlickering = true;
            StartCoroutine(FlickerForDuration());
        }
    }

    private IEnumerator FlickerForDuration()
    {
        float elapsedTime = 0f;

        while (elapsedTime < flickerDuration)
        {
            IntensityAndRange(); // Call flickering effect
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        TurnOffLight(); // Turn off the light after flickering
        isFlickering = false;
    }

    private void TurnOffLight()
    {
        lightSource.intensity = 0;
        lightSource.enabled = false; // Completely turn off the light
    }

    private void IntensityAndRange()
    {
        if (setNewTargets)
        {
            intensityDelta = (intensityOrigin + Random.Range(-intensityOffset, intensityOffset) - lightSource.intensity) * speed;

            rangeTarget = rangeOrigin + Random.Range(-rangeOffset, rangeOffset);
            rangeDelta = (rangeTarget - lightSource.range) * speed;

            setNewTargets = false;
        }

        lightSource.intensity += intensityDelta;
        lightSource.range += rangeDelta;

        if (Mathf.Abs(lightSource.range - rangeTarget) < 5f * scale)
            setNewTargets = true;
    }

    void Update()
    {
        if (deltaSum >= 0.02f && isFlickering)
        {
            IntensityAndRange();
            deltaSum -= 0.02f;
        }

        deltaSum += Time.deltaTime;
    }
}

