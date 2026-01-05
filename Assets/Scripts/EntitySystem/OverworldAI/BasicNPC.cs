using System.Collections;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class FloorClerk : MonoBehaviour
{
    public enum State
    {
        PATROL,
        CHASE
    }

    public State state;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float patrolRadius = 10f;

    [Header("Vision")]
    [SerializeField] private float seeingDistance = 15f;
    [SerializeField] private float height = 1.5f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Pathfinding")]
    [SerializeField] private float nextWaypointDistance = 3f;
    [SerializeField] private float pathfindingInterval = 1f;
    [SerializeField] private float continuousGenerationInterval = 0.5f;

    private Transform target;
    private Seeker seeker;
    private Path path;
    private int currentWaypoint = 0;

    private Vector3 initPos;
    private Vector3 lastKnownPosition;
    private float pathfindingCooldown;

    private bool seeingTarget;
    private Vector3 eyePos;
    private Vector3 targetAtEyeHeight;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        initPos = transform.position;
        StartCoroutine(StartPath());
    }

    private void Update()
    {
        pathfindingCooldown += Time.deltaTime;

        // Vision check
        float dist;
        eyePos = transform.position + Vector3.up * height;
        targetAtEyeHeight = target.position + Vector3.up * height;

        seeingTarget = seeingDistance > (dist = Vector3.Distance(eyePos, targetAtEyeHeight))
            ? !Physics.Raycast(eyePos, (targetAtEyeHeight - eyePos).normalized, dist, obstacleLayer)
            : false;

        if (seeingTarget)
        {
            state = State.CHASE;
            lastKnownPosition = target.position;
        }
        else
        {
            state = State.PATROL;
        }

        // Update chase path
        if (state == State.CHASE && pathfindingCooldown > continuousGenerationInterval)
        {
            pathfindingCooldown = 0;
            StartCoroutine(StartPath());
        }

        // Follow path
        if (path != null)
        {
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);

            float distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);

            if (distanceToWaypoint < nextWaypointDistance)
            {
                if (currentWaypoint + 1 < path.vectorPath.Count)
                    currentWaypoint++;
                else
                    StartCoroutine(StartPath());
            }
        }
    }

    private IEnumerator StartPath(bool reset = true)
    {
        if (reset)
            path = null;

        Vector3 targetPos = SelectTarget();

        if (state == State.PATROL)
            yield return new WaitForSeconds(pathfindingInterval);

        seeker.StartPath(transform.position, targetPos);
        yield break;
    }

    private Vector3 SelectTarget()
    {
        switch (state)
        {
            case State.CHASE:
                return target.position;

            case State.PATROL:
            default:
                Vector2 rand = patrolRadius * Random.insideUnitCircle;
                return initPos + new Vector3(rand.x, 0, rand.y);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (p.error) return;

        path = p;
        currentWaypoint = 0;
    }
}