using System;
using UnityEngine;

public class CityHall : MonoBehaviour
{
    public event Action<Team> OnDestroyed;

    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
        if (health != null)
            health.OnDeath += HandleDeath;
    }

    private void HandleDeath(Team attackerTeam)
    {
        OnDestroyed?.Invoke(attackerTeam);
    }

    void OnDestroy()
    {
        if (health != null)
            health.OnDeath -= HandleDeath;
    }
}