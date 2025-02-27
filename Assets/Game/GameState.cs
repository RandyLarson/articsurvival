using System;
using UnityEngine;

public enum GameMode
{
    Playing = 1,
    Paused = 2,
}

[Serializable]
public class GameState
{
    public static GameState Current => GameDataHolder.Current.GameState;

    public DateTime CurrentDate { get => _currentDate; set => _currentDate = value; }
    public DateTime StartDate { get => _startDate; set => _startDate = value; }
    public GameMode GamePhase { get => _gamePhase; set => _gamePhase = value; }
    public ClockConfig DayAndTimeTraits { get => _dayAndTimeTraits; set => _dayAndTimeTraits = value; }

    [SerializeField] private DateTime _currentDate;
    [SerializeField] private DateTime _startDate;
    [SerializeField] private GameMode _gamePhase;
    [SerializeField] private ClockConfig _dayAndTimeTraits;
}
