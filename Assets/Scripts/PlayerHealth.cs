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

    public bool hasMask = false;

    void Start()
    {
        currentHighMeter = MinHighMeter;
    }

    void Update()
    {
        if (hasMask)
        {
            currentHighMeter += highMeterIncreaseRate * Time.deltaTime;

            if (currentHighMeter >= maxHighMeter)
            {
                currentHighMeter = maxHighMeter;
                hasMask = false;
                Die();
            }

            Debug.Log("High Meter: " + Mathf.RoundToInt(currentHighMeter));
        }
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

    private void Die()
    {
        Debug.Log("Player has died.");
        Respawn();
    }

    private void Respawn()
    {
        transform.position = respawnPoint.position;
        currentHighMeter = 0;
        hasMask = false;
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
