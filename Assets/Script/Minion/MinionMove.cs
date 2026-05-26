using UnityEngine;
using UnityEngine.AI;

public class MinionMove : MonoBehaviour
{
    private Transform[] waypoints;
    private int currentWaypoint = 0;
    private NavMeshAgent agent;

    public event System.Action OnWaypointReached;
    public event System.Action OnDestinationReached;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        StatCalculator calculator = GetComponent<StatCalculator>();
        if (calculator != null)
            agent.speed = calculator.GetStat(StatType.Speed);
    }

    void Update()
    {
        MoveToWaypoint();
    }

    public void ChaseTo(Vector3 targetPos)
    {
        agent.isStopped = false;
        agent.SetDestination(targetPos);
    }

    public void StopMove()
    {
        agent.isStopped = true;
    }

    public void ResumeMove()
    {
        agent.isStopped = false;
        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        if (agent.isStopped) return;
        if (currentWaypoint >= waypoints.Length) return;

        Transform target = waypoints[currentWaypoint];
        agent.SetDestination(target.position);

        if (!agent.pathPending && agent.remainingDistance < 20f)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Length)
            {
                OnDestinationReached?.Invoke();
                StopMove();
            }
            else
            {
                OnWaypointReached?.Invoke();
            }
        }
    }

    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
        currentWaypoint = 0;
    }
}