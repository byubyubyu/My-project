using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private float hpBarHeight = 5f;
    [SerializeField] private Vector2 hpBarSize = new Vector2(100f, 10f);

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private float currentHP;
    private Image hpBarImage;
    private TeamMember teamMember;

    public event System.Action<Team> OnDeath;
    public event System.Action<float> OnDamaged;
    public event System.Action<float> OnHealed;

    void Start()
    {
        teamMember = GetComponent<TeamMember>();
        if (teamMember != null)
            teamMember.OnTeamChanged += OnTeamChanged;

        StatCalculator calculator = GetComponent<StatCalculator>();
        if (calculator != null)
            maxHP = calculator.GetStat(StatType.HP);

        currentHP = maxHP;
        CreateHPBar();
        UpdateHPBar();
    }

    void CreateHPBar()
    {
        if (hpBarPrefab == null) return;

        GameObject hpBarObj = Instantiate(hpBarPrefab, transform.position + Vector3.up * hpBarHeight, Quaternion.identity);
        hpBarObj.transform.SetParent(transform);

        Image[] images = hpBarObj.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            RectTransform rt = img.GetComponent<RectTransform>();
            if (rt != null)
                rt.sizeDelta = hpBarSize;

            if (img.name == "HPbar")
            {
                hpBarImage = img;
                hpBarImage.color = GetTeamColor();
            }
        }
    }

    Color GetTeamColor()
    {
        if (teamMember == null) return Color.white;

        switch (teamMember.Team)
        {
            case Team.Blue: return Color.blue;
            case Team.Red: return Color.red;
            case Team.Green: return Color.green;
            default: return Color.white;
        }
    }

    private void OnTeamChanged(Team newTeam)
    {
        if (hpBarImage != null)
            hpBarImage.color = GetTeamColor();
    }

    public void TakeDamage(float damage, Team attackerTeam)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0f);
        UpdateHPBar();
        OnDamaged?.Invoke(currentHP);

        if (currentHP <= 0)
        {
            OnDeath?.Invoke(attackerTeam);
            Destroy(gameObject);
        }
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        UpdateHPBar();
        OnHealed?.Invoke(currentHP);
    }

    void UpdateHPBar()
    {
        if (hpBarImage != null)
            hpBarImage.fillAmount = currentHP / maxHP;
    }

    void OnDestroy()
    {
        if (teamMember != null)
            teamMember.OnTeamChanged -= OnTeamChanged;
    }
}