using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MinionDetection : MonoBehaviour
{
    [SerializeField] private float searchInterval = 0.2f;
    [SerializeField] private TargetTag[] targetTags = { TargetTag.Minion };

    private float detectionRange;
    private TeamMember teamMember;
    private LineRenderer detectionCircle;
    private GameObject targetEnemy;
    private float searchTimer = 0f;

    // イベント
    public event System.Action<GameObject> OnEnemyFound;
    public event System.Action OnEnemyLost;

    void Start()
    {
        teamMember = GetComponent<TeamMember>();

        StatCalculator calculator = GetComponent<StatCalculator>();
        if (calculator != null)
        {
            detectionRange = calculator.GetStat(StatType.DetectionRange);
        }

        CreateCircle();
    }

    void Update()
    {
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchInterval)
        {
            GameObject previousTarget = targetEnemy;
            targetEnemy = SearchNearestEnemy();

            // 新しく敵を発見
            if (previousTarget == null && targetEnemy != null)
            {
                OnEnemyFound?.Invoke(targetEnemy);
            }
            // 敵を見失った
            else if (previousTarget != null && targetEnemy == null)
            {
                OnEnemyLost?.Invoke();
            }

            searchTimer = 0f;
        }
    }

    GameObject SearchNearestEnemy()
    {
        GameObject nearest = null;
        float minDistance = detectionRange;
        int lowestPriority = int.MaxValue;

        foreach (TargetTag tag in targetTags)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag.ToString());

            foreach (GameObject target in targets)
            {
                TeamMember tm = target.GetComponent<TeamMember>();
                if (tm == null || tm.Team == teamMember.Team) continue;

                Health health = target.GetComponent<Health>();
                if (health == null) continue;

                Priority p = target.GetComponent<Priority>();
                int priority = p != null ? p.PriorityLevel : int.MaxValue;

                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance > detectionRange) continue;

                if (priority < lowestPriority || (priority == lowestPriority && distance < minDistance))
                {
                    lowestPriority = priority;
                    minDistance = distance;
                    nearest = target;
                }
            }
        }
        return nearest;
    }

    public GameObject FindNearestEnemy()
    {
        return targetEnemy;
    }

    void CreateCircle()
    {
        GameObject circleObj = new GameObject("DetectionRangeCircle");
        circleObj.transform.SetParent(transform);
        circleObj.transform.localPosition = Vector3.zero;

        detectionCircle = circleObj.AddComponent<LineRenderer>();
        detectionCircle.loop = true;
        detectionCircle.widthMultiplier = 0.1f;
        detectionCircle.positionCount = 36;
        detectionCircle.startColor = Color.yellow;
        detectionCircle.endColor = Color.yellow;
        detectionCircle.useWorldSpace = false;

        for (int i = 0; i < 36; i++)
        {
            float angle = i * 10f * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * detectionRange;
            float z = Mathf.Sin(angle) * detectionRange;
            detectionCircle.SetPosition(i, new Vector3(x, 0.1f, z));
        }
    }
}