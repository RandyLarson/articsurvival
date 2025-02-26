using System;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[Serializable]
public class PersonDataRuntime
{
    [SerializeField] private float _health = 100;
    [SerializeField] private float _calories = 4000;
    [SerializeField] private Vector3 _currentDestination = Vector3.zero;
    [SerializeField] private float _currentSpeed = 0;

    public float Health { get => _health; set => _health = value; }
    public float Calories { get => _calories; set => _calories = value; }
    public Vector3 CurrentDestination { get => _currentDestination; set => _currentDestination = value; }
    public float CurrentSpeed { get => _currentSpeed; set => _currentSpeed = value; }
}


public class PersonController : MonoBehaviour
{
    public bool IsSelected { get; set; }
    [SerializeField] private GameObject SelectedHighlight;
    [SerializeField] private PersonData _coreStats;
    public PersonDataRuntime Stats { get; set; } = new PersonDataRuntime();
    public PersonData CoreStats { get => _coreStats; set => _coreStats = value; }

    void Start()
    {
        Stats = new PersonDataRuntime();
    }

    private void Update()
    {
        HandleMovement();
    }


    private void HandleMovement()
    {
        Vector3 bearing = Vector3.zero;

        Debug.DrawLine(transform.position, Stats.CurrentDestination, Color.magenta);
        if (Stats.CurrentSpeed != 0 )
        {
            bearing = (Stats.CurrentDestination - transform.position).normalized;
        }

        var delta = bearing * Stats.CurrentSpeed * Time.deltaTime;
        transform.position = transform.position + delta;

        if ( Vector2.Distance(transform.position, Stats.CurrentDestination) < 1)
        {
            transform.position = Stats.CurrentDestination;
            Stats.CurrentSpeed = 0;
        }

        //UpdateAnimationVariables();
    }

    public void ToggleSelection()
    {
        IsSelected = !IsSelected;
        SelectedHighlight.gameObject.SetActive(IsSelected);
    }

    public void SetCurrentMovementDestination(Vector3 dst)
    {
        Stats.CurrentDestination = dst;
        Stats.CurrentSpeed = CoreStats.WalkingSpeed;
    }
}
