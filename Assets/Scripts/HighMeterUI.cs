using UnityEngine;
using UnityEngine.UI;

public class HighMeterUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fillImage; // Sprite del relleno (debe tener Image Type: Filled)
    [SerializeField] private Image baseImage; // Sprite de la base (opcional, solo visual)

    [Header("Fill Settings")]
    [SerializeField] private Image.FillMethod fillMethod = Image.FillMethod.Horizontal;
    [SerializeField] private bool fillFromRight = false;

    [Header("Color Settings")]
    [SerializeField] private bool useGradient = true;
    [SerializeField] private Color lowColor = Color.green;
    [SerializeField] private Color midColor = Color.yellow;
    [SerializeField] private Color highColor = Color.red;
    [SerializeField] private float midThreshold = 0.5f;

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool animateFill = true;

    private PlayerHealth playerHealth;
    private float displayedFillAmount;

    void Start()
    {
        // Buscar PlayerHealth en la escena
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogWarning("HighMeterUI: No se encontró PlayerHealth en la escena!");
        }

        // Configurar el fill image
        if (fillImage != null)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = fillMethod;
            fillImage.fillOrigin = fillFromRight ? 1 : 0;
            displayedFillAmount = 0f;
        }
    }

    void Update()
    {
        if (playerHealth == null || fillImage == null) return;

        float targetFill = playerHealth.GetHighMeterPercentage();

        // Animación suave o directa
        if (animateFill)
        {
            displayedFillAmount = Mathf.Lerp(displayedFillAmount, targetFill, smoothSpeed * Time.deltaTime);
        }
        else
        {
            displayedFillAmount = targetFill;
        }

        // Actualizar el fill amount
        fillImage.fillAmount = displayedFillAmount;

        // Actualizar color basado en el porcentaje
        if (useGradient)
        {
            fillImage.color = GetGradientColor(displayedFillAmount);
        }
    }

    private Color GetGradientColor(float percentage)
    {
        if (percentage < midThreshold)
        {
            // De low a mid
            float t = percentage / midThreshold;
            return Color.Lerp(lowColor, midColor, t);
        }
        else
        {
            // De mid a high
            float t = (percentage - midThreshold) / (1f - midThreshold);
            return Color.Lerp(midColor, highColor, t);
        }
    }

    // Método público para asignar PlayerHealth manualmente
    public void SetPlayerHealth(PlayerHealth health)
    {
        playerHealth = health;
    }
}
