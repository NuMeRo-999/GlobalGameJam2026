using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("High Meter Settings")]
    [SerializeField] private int MinHighMeter = 0;
    [SerializeField] private int maxHighMeter = 100;
    [SerializeField] private float currentHighMeter;
    [SerializeField] private float highMeterIncreaseRate = 10f;

    [Space]
    [SerializeField] private LayerMask damageLayers;
    [SerializeField] private Transform respawnPoint;

    private VisionController visionController;
    private bool isInIntroMode = false;

    void Awake()
    {
        visionController = GetComponent<VisionController>();
    }

    void Start()
    {
        currentHighMeter = MinHighMeter;
    }

    void Update()
    {
        // No aumentar highMeter durante la intro
        if (visionController.hasMask && !isInIntroMode)
        {
            currentHighMeter += highMeterIncreaseRate * Time.deltaTime;

            if (currentHighMeter >= maxHighMeter)
            {
                currentHighMeter = maxHighMeter;
                visionController.hasMask = false;
                Die();
            }

            Debug.Log("High Meter: " + Mathf.RoundToInt(currentHighMeter));
        }
    }

    /// <summary>
    /// Activa o desactiva el modo intro. Durante la intro, el highMeter no aumenta.
    /// </summary>
    public void SetIntroMode(bool introMode)
    {
        isInIntroMode = introMode;
    }

    /// <summary>
    /// Indica si el jugador está en modo intro.
    /// </summary>
    public bool IsInIntroMode()
    {
        return isInIntroMode;
    }

    public void IncreaseHighMeter(int amount)
    {
        currentHighMeter += amount;
        if (currentHighMeter >= maxHighMeter)
        {
            currentHighMeter = maxHighMeter;
        }
        Debug.Log("High Meter increased to: " + Mathf.RoundToInt(currentHighMeter));
    }

    public void DecreaseHighMeter(int amount)
    {
        currentHighMeter -= amount;
        if (currentHighMeter <= 0)
        {
            currentHighMeter = 0;
        }
        Debug.Log("High Meter decreased to: " + Mathf.RoundToInt(currentHighMeter));
    }

    // Método para obtener el valor actual como int para UI
    public int GetCurrentHighMeter()
    {
        return Mathf.RoundToInt(currentHighMeter);
    }

    // Método para obtener el porcentaje para barra de progreso
    public float GetHighMeterPercentage()
    {
        return currentHighMeter / maxHighMeter;
    }

    public void Die()
    {
        Debug.Log("Player has died.");
        Respawn();
    }

    private void Respawn()
    {
        transform.position = respawnPoint.position;
        currentHighMeter = 0;
        visionController.hasMask = false;
        Debug.Log("Player has respawned.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & damageLayers) != 0)
        {
            Die();
        }
    }
}
