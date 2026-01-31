using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpriteMaskEffect : MonoBehaviour
{
    [SerializeField] private GameObject spriteMask;
    [SerializeField] private float scaleSpeed = 1f;
    [SerializeField] private float maxScale = 230f;
    [SerializeField] private float disableDelay = 5f;
    private Vector3 initialScale;
    private VisionController visionController;

    void Start()
    {
        initialScale = spriteMask.transform.localScale;
        spriteMask.SetActive(false);
        visionController = GetComponent<VisionController>();
    }

    public void StartMaskEffect()
    {
        spriteMask.SetActive(true);
        visionController.hasMask = true;
        spriteMask.transform.localScale = initialScale;
        StartCoroutine(ScaleMaskUp(maxScale));
    }

    public void DisableLayerMask(float seconds)
    {
        StartCoroutine(DisableLayerMaskAfterDelay(seconds));
    }

    // Aumenta su escala progresivamente
    public IEnumerator ScaleMaskUp(float targetScaleValue)
    {
        Vector3 startScale = spriteMask.transform.localScale;
        Vector3 targetScale = new Vector3(targetScaleValue, targetScaleValue, targetScaleValue);
        float duration = targetScaleValue / scaleSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float time = elapsedTime / duration;
            spriteMask.transform.localScale = Vector3.Lerp(startScale, targetScale, time);
            yield return null;
        }

        spriteMask.transform.localScale = targetScale;
    }

    // Disminuye su escala progresivamente
    public IEnumerator ScaleMaskDown(float targetScaleValue)
    {
        Vector3 startScale = spriteMask.transform.localScale;
        Vector3 targetScale = new Vector3(targetScaleValue, targetScaleValue, targetScaleValue);
        float scaleRange = startScale.x - targetScaleValue;
        float duration = scaleRange / scaleSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float time = elapsedTime / duration;
            spriteMask.transform.localScale = Vector3.Lerp(startScale, targetScale, time);
            yield return null;
        }

        spriteMask.transform.localScale = targetScale;
        spriteMask.SetActive(false);
    }

    private IEnumerator DisableLayerMaskAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartCoroutine(ScaleMaskDown(0f));
        visionController.hasMask = false;
    }

    public void OnPutMask(InputValue value)
    {
        if (value.isPressed)
        {
            StartMaskEffect();
            DisableLayerMask(disableDelay);
        }
    }
}
