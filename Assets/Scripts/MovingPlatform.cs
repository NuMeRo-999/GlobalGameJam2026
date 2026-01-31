using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTimeAtPoint = 0.5f;
    [SerializeField] private bool usePingPongMode = true;
    
    [Header("Path Visualization")]
    [SerializeField] private bool showPath = true;
    [SerializeField] private Color pathColor = Color.yellow;
    [SerializeField] private float pathWidth = 0.1f;
    
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;
    private int currentTargetIndex = 1;
    private int direction = 1;
    private bool isWaiting = false;
    
    private Transform playerOnPlatform;
    
    private void Awake()
    {
        //add current position as first waypoint of platform as waypoint 1 to the current waypoint array
        Transform[] updatedWaypoints = new Transform[waypoints.Length + 1];
        //Instantiate empty transform at current position
        GameObject emptyTransformObject = new GameObject("InitialWaypoint");
        emptyTransformObject.transform.position = transform.position;
        updatedWaypoints[0] = emptyTransformObject.transform;
        for (int i = 0; i < waypoints.Length; i++)
        {
            updatedWaypoints[i + 1] = waypoints[i];
        }

        waypoints = updatedWaypoints;

        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
        
        // LineRenderer setup
        if (GetComponent<LineRenderer>() == null)
            gameObject.AddComponent<LineRenderer>();
        
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = pathColor;
        lineRenderer.endColor = pathColor;
        lineRenderer.startWidth = pathWidth;
        lineRenderer.endWidth = pathWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.sortingOrder = -1;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = showPath;
        
        if (waypoints.Length >= 2 && showPath)
        {
            if (usePingPongMode)
            {
                lineRenderer.positionCount = waypoints.Length;
                for (int i = 0; i < waypoints.Length; i++)
                {
                    lineRenderer.SetPosition(i, waypoints[i].position);
                }
            }
            else
            {
                lineRenderer.positionCount = waypoints.Length + 1;
                for (int i = 0; i < waypoints.Length; i++)
                {
                    lineRenderer.SetPosition(i, waypoints[i].position);
                }
                lineRenderer.SetPosition(waypoints.Length, waypoints[0].position);
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (waypoints.Length < 2 || isWaiting)
            return;
        
        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = waypoints[currentTargetIndex].position;
        Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.fixedDeltaTime);
        Vector2 platformMovement = newPosition - currentPosition;
        
        rb.MovePosition(newPosition);
        
        // VOLVER A transform.position - NO usar rb.MovePosition en el jugador
        if (playerOnPlatform != null)
        {
            playerOnPlatform.position += (Vector3)platformMovement;
        }
        
        if (Vector2.Distance(newPosition, targetPosition) < 0.05f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    playerOnPlatform = collision.transform;
                    break;
                }
            }
        }
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerOnPlatform == null)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    playerOnPlatform = collision.transform;
                    break;
                }
            }
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = null;
        }
    }
    
    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTimeAtPoint);
        
        if (usePingPongMode)
        {
            currentTargetIndex += direction;
            
            if (currentTargetIndex >= waypoints.Length)
            {
                currentTargetIndex = waypoints.Length - 2;
                direction = -1;
            }
            else if (currentTargetIndex < 0)
            {
                currentTargetIndex = 1;
                direction = 1;
            }
        }
        else
        {
            currentTargetIndex++;
            
            if (currentTargetIndex >= waypoints.Length)
            {
                currentTargetIndex = 0;
            }
        }
        
        isWaiting = false;
    }
}
