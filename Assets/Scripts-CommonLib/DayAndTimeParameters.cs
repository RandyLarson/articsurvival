using System;
using UnityEngine;

[Serializable]
public class DayAndTimeParameters
{
    [Tooltip("The game will set its clock to this upon starting.")]
    public SerializableDateTime GameStartDate = new SerializableDateTime(new DateTime(2022, 01, 01, 0, 0, 0));

    //public float GameStartTime = 10.5f;

    [Tooltip("The length of the day phase currently in effect.")]
    public float ActiveDayLengthInGameMinutes = 1;
    [Tooltip("Logical game minutes of days's length.")]
    public float DayLengthBaseInGameMinutes = 1;
    [Tooltip("Logical game minutes of days's length.")]
    public float DayLengthMaxInGameMinutes = 2;
    [Tooltip("Logical game minutes added to night length per activity available to player. This is meant to help tune the day length. (in seconds).")]
    public float DayLengthPerActivityInGameSeconds = 6;

    [Tooltip("Logical game minutes of night's length (e.g., .5 = 30 seconds).")]
    public float NightLengthBaseLengthInGameSeconds = 13;
    [Tooltip("Logical game minutes of night's length when there are no healthy plants (in seconds).")]
    public float NightLengthMinInGameSeconds = 10;
    [Tooltip("Logical game minutes of night's length (e.g., .5 = 30 seconds).")]
    public float NightLengthMaxInGameSeconds = 30;
    [Tooltip("Logical game minutes added to night length per activity available to player. This is meant to help tune the night length. (in seconds).")]
    public float NightLengthPerActivityInGameSeconds = 4;
    [Tooltip("The length of the night phase currently in effect")]
    public float ActiveNightLengthInGameMinutes = 1;

    [Tooltip("The intensity of sunlight when it is completely dark.")]
    public float NightMinLightIntensity = .1f;
    public Color SkySunsetColorStart = Color.white;
    public Color SkySunsetColorMid = Color.white;
    public Color SkySunsetColorEnd = Color.white;
    public float NightSkyMinLightIntensity = .1f;
    public Color SkySunriseColorStart = Color.white;
    public Color SkySunriseColorMid = Color.white;
    public Color SkySunriseColorEnd = Color.white;
    public float NightOutsideMinLightIntensity = .1f;

    [Tooltip("Time of sunrise.")]
    public float Sunrise = 6.5f;
    [Tooltip("Time of sunset.")]
    public float Sunset = 18.5f;
    [Tooltip("Logical game minutes spent transitioning between day and sunrise/sunset.")]
    public float TransitionTimeMinutes = 90;
    [Tooltip("Street light intensity minimum.")]
    public float MinStreetLightIntensity = .2f;

    [Tooltip("Range of time the night phase starts for each item participating.")]
    public Range NightPhaseStart = new Range(19, 21.5f);
    [Tooltip("Time of night phase ending.")]
    public float NightPhaseEnd = 4f;

    [Tooltip("Time of day when nighttime rewards are activated.")]
    public float NightPhaseRewardTime = 22.5f;

    [Tooltip("The time alloted for the nighttime shader to transition to and from full intensity.")]
    public float NightPhaseShaderIntensityChangeDurationSeconds = 4f;

    [Tooltip("The range of intensity of the nighttime shader.")]
    public Range NightShaderIntensityRange = new Range(.1f, 1.75f);

    [Tooltip("Controlling switch to perform nighttime reward activity.")]
    public bool NighttimeRewardActivityEnabled = true;
}
