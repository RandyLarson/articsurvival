using System;
using UnityEngine;

public enum GameMode
{
    Playing = 1,
    Paused = 2,
}

public class GameStateData
{
    public static GameStateData Current => GameStateHolder.Current.GameState;

    public DateTime CurrentDate { get => _currentDate; set => _currentDate = value; }
    public DateTime StartDate { get => _startDate; set => _startDate = value; }
    public GameMode GamePhase { get => _gamePhase; set => _gamePhase = value; }
    public DayAndTimeParameters DayAndTimeTraits { get => _dayAndTimeTraits; set => _dayAndTimeTraits = value; }

    [SerializeField] private DateTime _currentDate;
    [SerializeField] private DateTime _startDate;
    [SerializeField] private GameMode _gamePhase;
    [SerializeField] private DayAndTimeParameters _dayAndTimeTraits;
}

[CreateAssetMenu(fileName = "GameConfig", menuName = "Arctic/GameConfig")]
public class GameConfig : ScriptableObject
{
    public static GameConfig Current => GameStateHolder.Current.GameConfig;

    public DateTime CurrentDate { get => _currentDate; set => _currentDate = value; }
    public DateTime StartDate { get => _startDate; set => _startDate = value; }
    public GameMode GamePhase { get => _gamePhase; set => _gamePhase = value; }
    public DayAndTimeParameters DayAndTimeTraits { get => _dayAndTimeTraits; set => _dayAndTimeTraits = value; }

    [SerializeField] private DateTime _currentDate;
    [SerializeField] private DateTime _startDate;
    [SerializeField] private GameMode _gamePhase;
    [SerializeField] private DayAndTimeParameters _dayAndTimeTraits;
}
