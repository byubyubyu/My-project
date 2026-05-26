using System.Collections;
using UnityEngine;

public class MinionEffect : MonoBehaviour
{
    private float attackRange;
    private LineRenderer attackRangeCircle;
    private MinionAttack minionAttack;

    void Start()
    {
        minionAttack = GetComponent<MinionAttack>();

        StatCalculator calculator = GetComponent<StatCalculator>();
        if (calculator != null)
        {
            attackRange = calculator.GetStat(StatType.AttackRange);
        }

        // MinionAttackのイベントを購読
        if (minionAttack != null)
        {
            minionAttack.OnActionStart += OnAttackStart;
            minionAttack.OnActionEnd += OnAttackEnd;
        }

        CreateCircle();
    }

    void OnAttackStart()
    {
        // 攻撃開始時の演出（必要なら追加）
    }

    void OnAttackEnd()
    {
        // 攻撃終了時の演出（必要なら追加）
    }

    public void PlayAttackEffect(GameObject target)
    {
        StartCoroutine(AttackEffect(target));
    }

    IEnumerator AttackEffect(GameObject target)
    {
        GameObject effectObj = new GameObject("AttackEffect");
        effectObj.transform.SetParent(transform);
        LineRenderer effect = effectObj.AddComponent<LineRenderer>();
        effect.loop = true;
        effect.widthMultiplier = 0.5f;
        effect.positionCount = 36;
        effect.startColor = Color.yellow;
        effect.endColor = Color.yellow;
        effect.useWorldSpace = true;
        effect.material = attackRangeCircle.material;

        float radius = 1f;
        Vector3 startPos = transform.position + Vector3.up * 15f;
        Vector3 endPos = target != null ? target.transform.position + Vector3.up * 15f : startPos;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (target == null)
            {
                Destroy(effectObj);
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);

            for (int i = 0; i < 36; i++)
            {
                float angle = i * 10f * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                effect.SetPosition(i, currentPos + new Vector3(x, 0, z));
            }
            yield return null;
        }

        if (effectObj != null) Destroy(effectObj);
    }

    void CreateCircle()
    {
        GameObject circleObj = new GameObject("AttackRangeCircle");
        circleObj.transform.SetParent(transform);
        circleObj.transform.localPosition = Vector3.zero;

        attackRangeCircle = circleObj.AddComponent<LineRenderer>();
        attackRangeCircle.loop = true;
        attackRangeCircle.widthMultiplier = 0.1f;
        attackRangeCircle.positionCount = 36;
        attackRangeCircle.startColor = Color.red;
        attackRangeCircle.endColor = Color.red;
        attackRangeCircle.useWorldSpace = false;

        for (int i = 0; i < 36; i++)
        {
            float angle = i * 10f * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * attackRange;
            float z = Mathf.Sin(angle) * attackRange;
            attackRangeCircle.SetPosition(i, new Vector3(x, 0.1f, z));
        }
    }

    void OnDestroy()
    {
        if (minionAttack != null)
        {
            minionAttack.OnActionStart -= OnAttackStart;
            minionAttack.OnActionEnd -= OnAttackEnd;
        }
        StopAllCoroutines();
    }
}