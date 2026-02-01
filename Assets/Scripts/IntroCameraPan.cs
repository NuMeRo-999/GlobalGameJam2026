using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class IntroCameraPan : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Puntos por los que pasará la cámara durante la intro")]
    [SerializeField] private Transform[] waypoints;

    [Header("Timing Settings")]
    [Tooltip("Velocidad de movimiento de la cámara entre puntos")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("Tiempo de espera en cada punto")]
    [SerializeField] private float waitTimeAtPoint = 1f;
    [Tooltip("Tiempo de espera antes de empezar el recorrido")]
    [SerializeField] private float initialDelay = 0.5f;

    [Header("Camera References")]
    [Tooltip("La cámara virtual de Cinemachine que sigue al jugador")]
    [SerializeField] private CinemachineCamera playerFollowCamera;
    [Tooltip("La cámara virtual para el recorrido de intro")]
    [SerializeField] private CinemachineCamera introCamera;

    [Header("Player References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private SpriteMaskEffect spriteMaskEffect;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private VisionController visionController;

    [Header("Mask During Intro")]
    [Tooltip("Activar máscara durante la intro")]
    [SerializeField] private bool enableMaskDuringIntro = true;

    public GameObject squareMask;

    public static event Action OnIntroStarted;
    public static event Action OnIntroFinished;

    public bool IsIntroPlaying { get; private set; } = false;

    private void Awake()
    {
        // Posicionar la cámara de intro en el primer waypoint ANTES de que Cinemachine haga blend
        if (introCamera != null && waypoints != null && waypoints.Length > 0 && waypoints[0] != null)
        {
            introCamera.transform.position = new Vector3(
                waypoints[0].position.x,
                waypoints[0].position.y,
                introCamera.transform.position.z
            );

            // Dar prioridad a la cámara de intro desde el inicio
            introCamera.Priority = 20;
            if (playerFollowCamera != null)
            {
                playerFollowCamera.Priority = 10;
            }
        }
    }

    private void Start()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("IntroCameraPan: No waypoints assigned. Skipping intro.");
            return;
        }

        // Activar hasMask desde el inicio
        if (enableMaskDuringIntro && visionController != null)
        {
            visionController.hasMask = true;
        }

        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        IsIntroPlaying = true;
        OnIntroStarted?.Invoke();

        // Pausar el timer de speedrun durante la intro
        if (SpeedrunTimer.Instance != null)
        {
            SpeedrunTimer.Instance.PauseTimer();
        }

        // Desactivar movimiento del jugador durante la intro
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Indicar a PlayerHealth que no aumente el highMeter
        if (playerHealth != null)
        {
            playerHealth.SetIntroMode(true);
        }

        // Activar máscara si está configurado (modo intro - sin animación)
        if (enableMaskDuringIntro && spriteMaskEffect != null)
        {
            spriteMaskEffect.StartIntroMaskEffect();
        }

        // Asegurar que hasMask esté activo durante la intro
        if (enableMaskDuringIntro && visionController != null)
        {
            visionController.hasMask = true;
            squareMask.SetActive(true);
        }

        yield return new WaitForSeconds(initialDelay);

        // Recorrer todos los waypoints
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            Vector3 targetPosition = new Vector3(
                waypoints[i].position.x,
                waypoints[i].position.y,
                introCamera.transform.position.z
            );

            // Mover hacia el waypoint
            yield return StartCoroutine(MoveToPosition(targetPosition));

            // Esperar en el punto
            yield return new WaitForSeconds(waitTimeAtPoint);
        }

        // Terminar intro - devolver control a la cámara del jugador
        EndIntro();
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(introCamera.transform.position, targetPosition) > 0.01f)
        {
            introCamera.transform.position = Vector3.MoveTowards(
                introCamera.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        introCamera.transform.position = targetPosition;
    }

    private void EndIntro()
    {
        IsIntroPlaying = false;

        // Reanudar el timer de speedrun
        if (SpeedrunTimer.Instance != null)
        {
            SpeedrunTimer.Instance.ResumeTimer();
        }

        // Cambiar prioridades para que la cámara del jugador tome el control
        if (introCamera != null && playerFollowCamera != null)
        {
            introCamera.Priority = 0;
            playerFollowCamera.Priority = 10;
        }

        // Desactivar máscara si estaba activa (modo intro)
        if (enableMaskDuringIntro && spriteMaskEffect != null)
        {
            spriteMaskEffect.StopIntroMaskEffect();
        }

        // Desactivar squareMask al terminar la intro
        if (squareMask != null)
        {
            Destroy(squareMask);
        }

        // Reactivar el highMeter
        if (playerHealth != null)
        {
            playerHealth.SetIntroMode(false);
        }

        // Reactivar movimiento del jugador
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        OnIntroFinished?.Invoke();

        Debug.Log("Intro finished - Player has control");
    }


    // Método para saltar la intro (por ejemplo con un botón)
    public void SkipIntro()
    {
        StopAllCoroutines();
        EndIntro();
    }

    private void OnDestroy()
    {
        // Limpiar eventos
        OnIntroStarted = null;
        OnIntroFinished = null;
    }
}
