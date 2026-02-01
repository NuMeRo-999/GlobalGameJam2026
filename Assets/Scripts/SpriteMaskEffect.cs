using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpriteMaskEffect : MonoBehaviour
{
    [SerializeField] private GameObject spriteMask;
    [SerializeField] private float scaleSpeed = 1f;
    [SerializeField] private float descaleSpeedMultiplier = 2f;
    [SerializeField] private float maxScale = 230f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float disableDelay = 5f;
    private Vector3 initialScale;
    private VisionController visionController;

    private bool isHoldingKey = false;
    private bool isIntroMode = false;
    private Coroutine currentScaleCoroutine;

    void Awake()
    {
        visionController = GetComponent<VisionController>();
    }

    void Start()
    {
        initialScale = spriteMask.transform.localScale;
        spriteMask.SetActive(false);
    }

    public void StartMaskEffect()
    {
        spriteMask.SetActive(true);
        visionController.hasMask = true;
        spriteMask.transform.localScale = initialScale;
        isHoldingKey = true;
        StopCurrentScaleCoroutine();
        currentScaleCoroutine = StartCoroutine(ScaleMaskUp(maxScale));
    }

    /// <summary>
    /// Inicia el efecto de máscara para la intro (escala inmediata al máximo)
    /// </summary>
    public void StartIntroMaskEffect()
    {
        isIntroMode = true;
        spriteMask.SetActive(true);
        visionController.hasMask = true;
        spriteMask.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
    }

    /// <summary>
    /// Detiene el efecto de máscara de la intro
    /// </summary>
    public void StopIntroMaskEffect()
    {
        isIntroMode = false;
        spriteMask.transform.localScale = initialScale;
        spriteMask.SetActive(false);
        visionController.hasMask = false;
    }

    public void StopMaskEffect()
    {
        StopCurrentScaleCoroutine();
        currentScaleCoroutine = StartCoroutine(ScaleMaskDown(0f));
    }

    private void StopCurrentScaleCoroutine()
    {
        if (currentScaleCoroutine != null)
        {
            StopCoroutine(currentScaleCoroutine);
            currentScaleCoroutine = null;
        }
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
        float scaleRange = targetScaleValue - startScale.x;
        float duration = scaleRange / scaleSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration && isHoldingKey)
        {
            elapsedTime += Time.deltaTime;
            float time = elapsedTime / duration;
            spriteMask.transform.localScale = Vector3.Lerp(startScale, targetScale, time);
            //Añadir rotacion en Z
            spriteMask.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        if (isHoldingKey)
        {
            spriteMask.transform.localScale = targetScale;
        }

        currentScaleCoroutine = null;
    }

    // Disminuye su escala progresivamente
    public IEnumerator ScaleMaskDown(float targetScaleValue)
    {
        Vector3 startScale = spriteMask.transform.localScale;
        Vector3 targetScale = new Vector3(targetScaleValue, targetScaleValue, targetScaleValue);
        float scaleRange = startScale.x - targetScaleValue;
        float descaleSpeed = scaleSpeed * descaleSpeedMultiplier;
        float duration = scaleRange / descaleSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration && !isHoldingKey)
        {
            elapsedTime += Time.deltaTime;
            float time = elapsedTime / duration;
            spriteMask.transform.localScale = Vector3.Lerp(startScale, targetScale, time);
            //Añadir rotacion en Z
            spriteMask.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        if (!isHoldingKey)
        {
            spriteMask.transform.localScale = targetScale;
            spriteMask.SetActive(false);
            visionController.hasMask = false;
        }

        currentScaleCoroutine = null;
    }

    private IEnumerator DisableLayerMaskAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isHoldingKey = false;
        StopCurrentScaleCoroutine();
        currentScaleCoroutine = StartCoroutine(ScaleMaskDown(0f));
    }

    public void OnPutMask(InputValue value)
    {
        if (value.isPressed)
        {
            isHoldingKey = true;
            spriteMask.SetActive(true);
            this.GetComponent<Animator>().SetTrigger("PutMask");
            this.GetComponent<Animator>().SetBool("mask", true);
            visionController.hasMask = true;
            StopCurrentScaleCoroutine();
            currentScaleCoroutine = StartCoroutine(ScaleMaskUp(maxScale));
        }
        else
        {
            isHoldingKey = false;
            if (spriteMask.activeSelf)
            {
                this.GetComponent<Animator>().SetTrigger("RemoveMask");
                this.GetComponent<Animator>().SetBool("mask", false);
                StopCurrentScaleCoroutine();
                currentScaleCoroutine = StartCoroutine(ScaleMaskDown(0f));
            }
        }
    }
}
