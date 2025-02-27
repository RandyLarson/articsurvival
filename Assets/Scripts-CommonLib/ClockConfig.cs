using UnityEngine;

[CreateAssetMenu(fileName = "ClockConfig", menuName = "Arctic/ClockConfig")]
public class ClockConfig : ScriptableObject
{
    public float DayLengthBaseInGameMinutes = 5;
    public float DayLengthMaxInGameMinutes = 5;

    [Tooltip("Time of sunrise.")]
    public float Sunrise = 6.5f;
    [Tooltip("Time of sunset.")]

    public float Sunset = 18.5f;
    [Tooltip("Logical game minutes spent transitioning between day and sunrise/sunset.")]
    public float TransitionTimeMinutes = 90;


    // Lighting -- factor out

    [Tooltip("Street light intensity minimum.")]
    public float MinStreetLightIntensity = .2f;
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
