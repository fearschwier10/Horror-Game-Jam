using UnityEngine;
using UnityEngine.UI;

public class HorrorSprintSlider : MonoBehaviour
{
    public Slider sprintSlider;
    private Image fillImage;
    private Color originalColor;
    private Color darkGreen = new Color(0.1f, 0.3f, 0.1f); // Add dark green for variation

    void Start()
    {
        fillImage = sprintSlider.fillRect.GetComponent<Image>();
        originalColor = fillImage.color;
    }

    void Update()
    {
        // Glitch effect with color variation
        if (Random.value > 0.8f)
        {
            fillImage.color = new Color(
                originalColor.r * Random.Range(0.5f, 1.1f),
                originalColor.g * Random.Range(0.2f, 0.4f),
                originalColor.b * Random.Range(0.4f, 0.6f)
            );
        }
        else
        {
            fillImage.color = originalColor;
        }

        // Pulsing effect blending between dark red and green
        float flickerAmount = Mathf.PingPong(Time.time * 0.7f, 0.4f);
        fillImage.color = Color.Lerp(originalColor, darkGreen, flickerAmount);
    }
}

