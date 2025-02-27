using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Arctic/GameConfig")]
public class GameConfig : ScriptableObject
{
    public static GameConfig Current => GameDataHolder.Current.GameConfig;

    [Tooltip("The game will set its clock to this upon starting.")]
    public SerializableDateTime GameStartDate = new SerializableDateTime(new DateTime(1770, 10, 01, 0, 0, 0));
    public DateTime CurrentDate { get => _currentDate; set => _currentDate = value; }
    public DateTime StartDate { get => _startDate; set => _startDate = value; }
    public GameMode GamePhase { get => _gamePhase; set => _gamePhase = value; }
    public ClockConfig ClockConfig { get => _clockConfig; set => _clockConfig = value; }
    public float FeetPerUnit { get => _feetPerUnit; set => _feetPerUnit = value; }
    public float MapPanningSpeed { get => _mapPanningSpeed; set => _mapPanningSpeed = value; }
    public float MapPanningThresholdPercentage { get => _mapPanningThresholdPercentage; set => _mapPanningThresholdPercentage = value; }

    [SerializeField] private float _feetPerUnit = .3f;
    [SerializeField] private DateTime _currentDate;
    [SerializeField] private DateTime _startDate;
    [SerializeField] private GameMode _gamePhase;
    [SerializeField] private ClockConfig _clockConfig;
    [SerializeField] private float _mapPanningSpeed = 100;
    [SerializeField] private float _mapPanningThresholdPercentage;
}
