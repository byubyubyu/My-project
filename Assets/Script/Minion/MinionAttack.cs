using System;
using UnityEngine;

public class MinionAttack : MonoBehaviour, IMinionAction
{
    // IMinionAction実装
    public event Action OnActionStart;
    public event Action OnActionEnd;
    public event Action<Vector3> OnChaseTarget;
    public bool StopsMovement => true;
    public int PriorityLevel => 1;

    private float attackRange;
    private float attackDamage;
    private float attackInterval;
    private float attackTimer = 0f;
    private MinionDetection detection;
    private MinionEffect effect;
    private bool isAttacking = false;

    void Start()
    {
        detection = GetComponent<MinionDetection>();
        effect = GetComponent<MinionEffect>();

        StatCalculator calculator = GetComponent<StatCalculator>();
        if (calculator != null)
        {
            attackRange = calculator.GetStat(StatType.AttackRange);
            attackDamage = calculator.GetStat(StatType.AttackDamage);
            attackInterval = calculator.GetStat(StatType.AttackInterval);
        }
    }

    void Update()
    {
        attackTimer += Time.deltaTime;
        GameObject target = detection.FindNearestEnemy();

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance <= attackRange)
            {
                // 攻撃範囲内
                if (!isAttacking)
                {
                    isAttacking = true;
                    OnActionStart?.Invoke();
                }

                if (attackTimer >= attackInterval)
                {
                    Attack(target);
                    attackTimer = 0f;
                }
            }
            else
            {
                // 攻撃範囲外 → 追いかける
                if (isAttacking)
                {
                    isAttacking = false;
                    OnActionEnd?.Invoke();
                }
                OnChaseTarget?.Invoke(target.transform.position);
            }
        }
        else
        {
            // ターゲットなし
            if (isAttacking)
            {
                isAttacking = false;
                OnActionEnd?.Invoke();
            }
        }
    }

    void Attack(GameObject target)
    {
        if (target == null) return;

        Health health = target.GetComponent<Health>();
        if (health != null)
        {
            TeamMember tm = GetComponent<TeamMember>();
            health.TakeDamage(attackDamage, tm != null ? tm.Team : Team.None);
        }

        if (effect != null)
        {
            effect.PlayAttackEffect(target);
        }
    }
}