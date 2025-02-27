using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum ClockEventKind
{
    Sunset = 0,
    Sunrise = 1,
    Intermediate = 2,
    Dusk = 3,
    // Useful at some point -- Daybreak = 4,
    Reset = 5,
    Midnight = 6
}

public class ClockEvent
{
    public ClockEventKind ClockEventKind;
    public TimeSpan CurrentTime;
}

public class TimeBasedCallback
{
    public Guid Id { get; }
    public int? MaxTimesToInvoke { get; }
    public int? DayToBeginOn { get; }
    public TimeSpan TriggerAtTimeOfDay { get; }
    public Action OnClockEvent { get; }
    public int TimesInvoked { get; set; }
    public int LastDayInvoked { get; set; } = -1;

    public TimeBasedCallback(int? dayNumToStart, TimeSpan triggerAtTimeOfDay, int? timesToInvoke, Action onClockEvent)
    {
        Id = Guid.NewGuid();
        DayToBeginOn = dayNumToStart;
        TriggerAtTimeOfDay = triggerAtTimeOfDay;
        MaxTimesToInvoke = timesToInvoke;
        OnClockEvent = onClockEvent;
    }
}

public class GameClock : MonoBehaviour
{
    public static GameClock Current => GameDataHolder.Current.GameClock;

    public bool OnlyTickInPlayingMode = true;
    public TimeSpan GameElapsedTime => GameState.Current.CurrentDate - GameState.Current.StartDate;

    public event EventHandler<ClockEvent> OnClockEvent;
    public event EventHandler<ClockEvent> OnSunriseSunset;
    public event EventHandler<ClockEvent> OnClockReset;

    List<TimeBasedCallback> RegisteredCallbacks { get; set; } = new List<TimeBasedCallback>();

    public SerializableDateTime StartDate { get; set; }
    public SerializableDateTime CurrentDate => GameState.Current.CurrentDate;
    public int DaysPlayed => (CurrentDate - (DateTime)StartDate).Days;

    private float LastTimeCounted { get; set; }
    private ClockEventKind? LastSunriseSetEvent { get; set; }
    public int LastMidnightEventSentDay { get; set; } = 0;
    public TimeSpan CurrentPhaseElapsed { get; set; } = TimeSpan.Zero;
    public float ElapsedGameSecondsForPhase => (float)CurrentPhaseElapsed.TotalSeconds;

    public float GamePhaseMaxLengthInMinutes = 5;

    [Tooltip("The length of the day phase currently in effect.")]
    [DoNotSerialize] public float _activeDayLengthInGameMinutes = 1;


    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        LastSunriseSetEvent = IsDay ? ClockEventKind.Sunrise : ClockEventKind.Sunset;
        DetermineCurrentPhaseLength();
        LastMidnightEventSentDay = 0;
        LastTimeCounted = Time.time;
    }

    public void GameClockReset()
    {
        Initialize();

        OnClockEvent?.Invoke(this, new ClockEvent { ClockEventKind = ClockEventKind.Reset, CurrentTime = TimeOfDay });
        OnClockReset?.Invoke(this, new ClockEvent { ClockEventKind = ClockEventKind.Reset, CurrentTime = TimeOfDay });
    }

    private void Update()
    {
        //DiagnosticController.Current.Add("GameTime", GameStateData.Current.Now.ToShortTimeString());
        //DiagnosticController.Current.Add("Game Days", GameElapsedTime.Days);
        //DiagnosticController.Current.Add("Days Played", PlayerStateData.Current.CurrentInventory.DaysPlayed);

        if (!OnlyTickInPlayingMode || GameState.Current.GamePhase == GameMode.Playing)
        {
            var clockTimeElapsed = Time.time - LastTimeCounted;
            var relGameTimeElapsed = CalculateRelativeGameTime(clockTimeElapsed);

            CurrentPhaseElapsed += TimeSpan.FromSeconds(clockTimeElapsed);
            GameState.Current.CurrentDate += relGameTimeElapsed;
            SendEvents();
        }
        LastTimeCounted = Time.time;
    }

    public TimeSpan CalculateFutureGameTime(float secondsFromNow)
    {
        var clockTimeElapsed = (Time.time + secondsFromNow) - LastTimeCounted;
        var relGameTimeElapsed = CalculateRelativeGameTime(clockTimeElapsed);
        return (GameState.Current.CurrentDate + relGameTimeElapsed).TimeOfDay;
    }

    public bool IsGameDayRealTimeSetToMax => _activeDayLengthInGameMinutes == GamePhaseMaxLengthInMinutes;
    public void SetGameDayRealTimeToMax()
    {
        _activeDayLengthInGameMinutes = GamePhaseMaxLengthInMinutes;
    }

    public void RestoreGameDayRealTimeToNormal()
    {
        DetermineCurrentPhaseLength();
    }

    public void DetermineCurrentPhaseLength()
    {
        DetermineDayPhaseLength();
    }

    private void SendEvents()
    {
        if (LastSunriseSetEvent == ClockEventKind.Sunset && IsDay)
        {
            CurrentPhaseElapsed = TimeSpan.Zero;
            DetermineCurrentPhaseLength();
            OnClockEvent?.Invoke(this, new ClockEvent { ClockEventKind = ClockEventKind.Sunrise, CurrentTime = TimeOfDay });
            OnSunriseSunset?.Invoke(this, new ClockEvent { ClockEventKind = ClockEventKind.Sunrise, CurrentTime = TimeOfDay });
            LastSunriseSetEvent = ClockEventKind.Sunrise;
        }

        ClockConfig dayTraits = GameState.Current.DayAndTimeTraits;
        TimeSpan set = TimeSpan.FromHours(dayTraits.Sunset);
        TimeSpan transitionSpan = TimeSpan.FromMinutes(dayTraits.TransitionTimeMinutes);

        if (LastSunriseSetEvent == ClockEventKind.Sunrise && TimeOfDay > (set - transitionSpan))
        {
            OnClockEvent?.Invoke(this, new ClockEvent { ClockEventKind = ClockEventKind.Dusk, CurrentTime = TimeOfDay });
            OnSunriseSunset?.Invoke(this, new ClockEvent { ClockEventKind = ClockEventKind.Dusk, CurrentTime = TimeOfDay });
            LastSunriseSetEvent = ClockEventKind.Dusk;
        }

        if (LastSunriseSetEvent == ClockEventKind.Dusk && IsNight)
        {
            CurrentPhaseElapsed = TimeSpan.Zero;
            ResetDayLength();
            DetermineCurrentPhaseLength();
            OnClockEvent?.Invoke(this, new ClockEvent { ClockEventKind = ClockEventKind.Sunset, CurrentTime = TimeOfDay });
            OnSunriseSunset?.Invoke(this, new ClockEvent { ClockEventKind = ClockEventKind.Sunset, CurrentTime = TimeOfDay });
            LastSunriseSetEvent = ClockEventKind.Sunset;
        }

        if (TimeOfDay.TotalMinutes < 60 && LastMidnightEventSentDay < GameElapsedTime.Days)
        {
            OnClockEvent?.Invoke(this, new ClockEvent { ClockEventKind = ClockEventKind.Midnight, CurrentTime = TimeOfDay });
            LastMidnightEventSentDay = GameElapsedTime.Days;
        }

        if (RegisteredCallbacks?.Any() == true)
        {
            List<TimeBasedCallback> toRemove = new List<TimeBasedCallback>();

            foreach (var toInvoke in RegisteredCallbacks.Where(rc => (!rc.DayToBeginOn.HasValue || rc.DayToBeginOn <= GameElapsedTime.Days)
                                                                     && rc.LastDayInvoked < GameElapsedTime.Days
                                                                     && rc.TriggerAtTimeOfDay < TimeOfDay))
            {
                try
                {
                    toInvoke.LastDayInvoked = GameElapsedTime.Days;

                    if (!toInvoke.MaxTimesToInvoke.HasValue || toInvoke.TimesInvoked < toInvoke.MaxTimesToInvoke)
                    {
                        toInvoke?.OnClockEvent();
                        toInvoke.TimesInvoked++;
                    }

                    if (toInvoke.MaxTimesToInvoke.HasValue && toInvoke.TimesInvoked >= toInvoke.MaxTimesToInvoke)
                    {
                        toRemove.Add(toInvoke);
                    }
                }
                catch (Exception)
                {
                    toRemove.Add(toInvoke);
                }
            }

            RegisteredCallbacks = RegisteredCallbacks.Except(toRemove).ToList();
        }

    }

    public bool IsWithinHourTimeFrame(float beginHours, float endHours)
    {
        return beginHours <= TimeOfDay.TotalHours && TimeOfDay.TotalHours <= endHours;
    }

    public DateTime Now => CurrentDate;
    public TimeSpan TimeOfDay => Now.TimeOfDay;
    public int DayNum => DaysPlayed;

    /// <summary>
    /// Given an elapsed time of second and the logical minutes per day, calculate the logical 
    /// elapsed time of time.
    /// </summary>
    /// <param name="deltaTimeGameSeconds">An length of game seconds that have passed.</param>
    /// <param name="spanGameMinutes">The number of game minutes per day.</param>
    /// <returns>The logical span of time that has elapsed.</returns>
    TimeSpan CalculateRelativeGameTime(float deltaTimeGameSeconds, float spanGameMinutes, TimeSpan logicalSpanLength)
    {
        var percentElapsed = TimeSpan.FromSeconds(deltaTimeGameSeconds).TotalMinutes / spanGameMinutes;
        if (double.IsInfinity(percentElapsed) || double.IsNaN(percentElapsed))
            percentElapsed = 0;

        return TimeSpan.FromMinutes(logicalSpanLength.TotalMinutes * percentElapsed);
    }

    public bool IsDusk
    {
        get
        {
            ClockConfig dayTraits = GameState.Current.DayAndTimeTraits;
            TimeSpan sunsetAt = TimeSpan.FromHours(dayTraits.Sunset);
            TimeSpan transitionSpan = TimeSpan.FromMinutes(dayTraits.TransitionTimeMinutes);

            return TimeOfDay > (sunsetAt - transitionSpan) && TimeOfDay < sunsetAt;
        }
    }

    public bool IsDay => GameState.Current.CurrentDate.TimeOfDay > TimeSpan.FromHours(GameState.Current.DayAndTimeTraits.Sunrise) && GameState.Current.CurrentDate.TimeOfDay < TimeSpan.FromHours(GameState.Current.DayAndTimeTraits.Sunset);
    public bool IsNight => !IsDay;

    TimeSpan CalculateRelativeGameTime(float deltaTimeGameSeconds)
    {
        ClockConfig dayTraits = GameState.Current.DayAndTimeTraits;
        return CalculateRelativeGameTime(deltaTimeGameSeconds, _activeDayLengthInGameMinutes, TimeSpan.FromHours(24));
    }


    private void DetermineDayPhaseLength()
    {
        float dayLengthMinutes = 1;
        try
        {
            dayLengthMinutes = GameState.Current.DayAndTimeTraits.DayLengthBaseInGameMinutes;
            dayLengthMinutes = Mathf.Min(dayLengthMinutes, GameState.Current.DayAndTimeTraits.DayLengthMaxInGameMinutes);
        }
        catch (Exception)
        {
        }
        finally
        {
            //DiagnosticController.Current.Add("Day length", dayLength);
            _activeDayLengthInGameMinutes = dayLengthMinutes;
        }
    }

    private void ResetDayLength()
    {
        _activeDayLengthInGameMinutes = GameState.Current.DayAndTimeTraits.DayLengthBaseInGameMinutes;
    }

    public static float SecondsAsMinutes(float seconds) => (float)TimeSpan.FromSeconds(seconds).TotalMinutes;
    public static float MinutesAsSeconds(float min) => (float)TimeSpan.FromMinutes(min).TotalSeconds;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startOnDayNum"></param>
    /// <param name="triggerAtTimeOfDay"></param>
    /// <param name="timesToInvoke">`null` for infinite</param>
    /// <param name="onClockEvent"></param>
    internal Guid RegisterCallbackAtTime(int? startOnDayNum, TimeSpan triggerAtTimeOfDay, int? timesToInvoke, Action onClockEvent)
    {
        var callback = new TimeBasedCallback(startOnDayNum, triggerAtTimeOfDay, timesToInvoke, onClockEvent);
        RegisteredCallbacks.Add(callback);
        return callback.Id;
    }

    internal void RemoveRegisteredCallback(Guid id)
    {
        if (id.IsEmpty()) return;

        RegisteredCallbacks = RegisteredCallbacks.Where(m => m.Id != id).ToList();
    }
}
