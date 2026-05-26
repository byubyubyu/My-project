using System;
using UnityEngine;

public class MinionRepair : MonoBehaviour, IMinionAction
{
    public event Action OnActionStart;
    public event Action OnActionEnd;
    public event Action<Vector3> OnChaseTarget;
    public bool StopsMovement => true;
    public int PriorityLevel => 2;

    [SerializeField] private TargetTag repairTag = TargetTag.Building;
    [SerializeField] private float repairRange = 5f;
    [SerializeField] private float repairInterval = 1f;

    private float repairAmount;
    private float repairTimer = 0f;
    private TeamMember teamMember;
    private MinionDetection detection;
    private bool isRepairing = false;

    void Start()
    {
        teamMember = GetComponent<TeamMember>();
        detection = GetComponent<MinionDetection>();

        StatCalculator calculator = GetComponent<StatCalculator>();
        if (calculator != null)
            repairAmount = calculator.GetStat(StatType.RepairAmount);

        if (repairAmount <= 0)
            enabled = false;
    }

    void Update()
    {
        if (detection != null && detection.FindNearestEnemy() != null)
        {
            if (isRepairing)
            {
                isRepairing = false;
                OnActionEnd?.Invoke();
            }
            return;
        }

        repairTimer += Time.deltaTime;
        if (repairTimer >= repairInterval)
        {
            RepairNearbyBase();
            repairTimer = 0f;
        }
    }

    void RepairNearbyBase()
    {
        bool repairedAny = false;

        GameObject[] bases = GameObject.FindGameObjectsWithTag(repairTag.ToString());
        foreach (GameObject baseObj in bases)
        {
            TeamMember tm = baseObj.GetComponent<TeamMember>();
            if (tm == null || tm.Team != teamMember.Team) continue;

            float distance = Vector3.Distance(transform.position, baseObj.transform.position);
            if (distance > repairRange) continue;

            Health health = baseObj.GetComponent<Health>();
            if (health == null || health.CurrentHP >= health.MaxHP) continue;

            if (!isRepairing)
            {
                isRepairing = true;
                OnActionStart?.Invoke();
            }

            health.Heal(repairAmount);
            repairedAny = true;

            if (health.CurrentHP >= health.MaxHP)
            {
                isRepairing = false;
                OnActionEnd?.Invoke();
            }
        }

        if (!repairedAny && isRepairing)
        {
            isRepairing = false;
            OnActionEnd?.Invoke();
        }
    }
}