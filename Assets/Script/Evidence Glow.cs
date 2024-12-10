using UnityEngine;

public class EvidenceGlow : MonoBehaviour
{
    private Renderer objectRenderer;
    public Color glowColor = Color.green;  // Color of the glow
    public float glowIntensity = 1f;  // Intensity of the glow (how bright it gets)
    public float pulseSpeed = 1f;  // Speed of the pulsing effect
    private Material material;

    private bool isGlowing = true; // Tracks whether the object is glowing

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        material = objectRenderer.material;  // Get the material of the object

        // Ensure the material uses a shader that supports emission
        material.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (!isGlowing)
            return; // Skip the glow update if the glow is turned off

        // Use Mathf.Sin to smoothly transition the emission value based on pulse speed
        float emissionValue = Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed));  // Adjusted for custom speed

        // Apply the emission color to make the object glow gently
        material.SetColor("_EmissionColor", glowColor * emissionValue);  // Apply the emission color

        // Optionally, update the lighting system to reflect the emission changes
        DynamicGI.SetEmissive(objectRenderer, glowColor * emissionValue);
    }

    // Public method to stop the glow effect
    public void StopGlow()
    {
        isGlowing = false; // Disable the glow
        material.SetColor("_EmissionColor", Color.black); // Set emission color to black to stop glowing
        DynamicGI.SetEmissive(objectRenderer, Color.black); // Disable the GI emission
    }

    // Public method to resume the glow effect
    public void StartGlow()
    {
        isGlowing = true; // Enable the glow
    }
}
