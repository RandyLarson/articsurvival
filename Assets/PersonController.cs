using System;
using UnityEngine;

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

public class GamePiece : MonoBehaviour
{
    [SerializeField] protected GameObject SelectedHighlight;
    public bool IsSelected { get; set; }

    public void SetSelected(bool value)
    {
        IsSelected = value;
        SelectedHighlight.gameObject.SetActive(IsSelected);
    }

    public void ToggleSelection() => SetSelected(!IsSelected);

}

public class PersonController : GamePiece
{
    [SerializeField] private PersonData _coreStats;
    public PersonDataRuntime Stats { get; set; } = new PersonDataRuntime();
    public PersonData CoreStats { get => _coreStats; set => _coreStats = value; }

    void Start()
    {
        Stats = new PersonDataRuntime();
        LastTimeAccountedFor = GameClock.Current.Now;
    }

    private void Update()
    {
        HandleMovement();
    }

    DateTime LastTimeAccountedFor { get; set; }
    private void HandleMovement()
    {
        Vector3 bearing = Vector3.zero;

        Debug.DrawLine(transform.position, Stats.CurrentDestination, Color.magenta);
        if (Stats.CurrentSpeed != 0)
        {
            bearing = (Stats.CurrentDestination - transform.position).normalized;
        }

        float elapsedTimeSeconds = (float)(GameClock.Current.Now - LastTimeAccountedFor).TotalSeconds;
        LastTimeAccountedFor = GameClock.Current.Now;

        var deltaFeet = elapsedTimeSeconds * Stats.CurrentSpeed * bearing;
        var deltaWp = GameConfig.Current.FeetPerUnit * deltaFeet;
        transform.position = transform.position + deltaWp;

        if (Vector2.Distance(transform.position, Stats.CurrentDestination) < 1)
        {
            transform.position = Stats.CurrentDestination;
            Stats.CurrentSpeed = 0;
        }

        //UpdateAnimationVariables();
    }

    public void SetCurrentMovementDestination(Vector3 dst)
    {
        Stats.CurrentDestination = new Vector3(dst.x, dst.y, transform.position.z);
        Stats.CurrentSpeed = CoreStats.WalkingSpeed;
    }
}
