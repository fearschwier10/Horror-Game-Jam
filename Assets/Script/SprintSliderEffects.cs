using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HorrorSprintSlider : MonoBehaviour
{
    public Slider sprintSlider;
    private Image fillImage;
    private Color originalColor;
    private Color darkGreen = new Color(0.1f, 0.3f, 0.1f); // Add dark green for variation

    public CanvasGroup sliderCanvasGroup; // CanvasGroup to control the visibility
    public float fadeDuration = 1f; // Duration of the fade in/out

    private bool isFadingOut = false;
    private bool isVisible = false; // Tracks slider visibility state

    void Start()
    {
        fillImage = sprintSlider.fillRect.GetComponent<Image>();
        originalColor = fillImage.color;

        if (sliderCanvasGroup == null)
        {
            sliderCanvasGroup = sprintSlider.gameObject.AddComponent<CanvasGroup>();
        }

        HideSliderInstantly();
    }

    void Update()
    {
        // Toggle slider visibility based on Shift key press
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            ShowSlider();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            HideSlider();
        }

        // Glitch effect with color variation if the slider is visible
        if (isVisible)
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

    private void ShowSlider()
    {
        if (!isVisible)
        {
            isVisible = true;
            StopAllCoroutines(); // Stop any active fade coroutine
            StartCoroutine(FadeSlider(1f));
        }
    }

    private void HideSlider()
    {
        if (isVisible)
        {
            isVisible = false;
            StopAllCoroutines(); // Stop any active fade coroutine
            StartCoroutine(FadeSlider(0f));
        }
    }

    private void HideSliderInstantly()
    {
        sliderCanvasGroup.alpha = 0f;
        sliderCanvasGroup.interactable = false;
        sliderCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeSlider(float targetAlpha)
    {
        float startAlpha = sliderCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            sliderCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        sliderCanvasGroup.alpha = targetAlpha;
        sliderCanvasGroup.interactable = targetAlpha > 0f;
        sliderCanvasGroup.blocksRaycasts = targetAlpha > 0f;
    }
}
