using System.Collections;
using TMPro;
using UnityEngine;

public enum PlayerStatKind
{
    NA,
}


public enum GameStatKind
{
    NA = 0,
    Date = 1,
    Fps = 2,
    DayNum = 3,
    BuildVersion = 4,
}

public class StatVisualController : MonoBehaviour
{
    public TMP_Text DisplayElement;
    public PlayerStatKind ResourceToDisplay;
    public GameStatKind GameStatToDisplay;

    void LocateDisplayElement()
    {
        if (DisplayElement == null)
            DisplayElement = GetComponentInChildren<TextMeshProUGUI>();
        if (DisplayElement == null)
            DisplayElement = GetComponentInChildren<TextMeshPro>();
    }

    private IEnumerator UpdateCoRoutine()
    {
        if (DisplayElement == null)
            yield break;

        while (true)
        {
            try
            {
                if (ResourceToDisplay != PlayerStatKind.NA)
                    DisplayPlayerStat();
                else if (GameStatToDisplay != GameStatKind.NA)
                    DisplayGameStat();
            }
            catch (System.Exception)
            {
            }

            yield return new WaitForSecondsRealtime(.1f);
        }
    }

    public void UpdateDisplayValue(string value)
    {
        if (DisplayElement != null)
            GeneralUtilities.ProtectedCall(() => DisplayElement.text = value);
    }

    private void OnEnable()
    {
        LocateDisplayElement();
        StartCoroutine(UpdateCoRoutine());
    }

    public void DisplayGameStat()
    {
        string value = string.Empty;

        switch (GameStatToDisplay)
        {
            case GameStatKind.NA:
                break;
            case GameStatKind.Date:
                value = GameClock.Current.CurrentDate.ToShortTimeString();
                break;
            case GameStatKind.Fps:
                value = $"{FPSCounter.FramesPerSec:n0}"; //  / {(Debug.isDebugBuild ? 'D' : 'R')}";
                break;
            case GameStatKind.DayNum:
                value = $"{(1 + GameClock.Current.DayNum)}";
                break;
            case GameStatKind.BuildVersion:
                value = $"{Application.version}";
                break;
        }
        UpdateDisplayValue(value);
    }

    public void DisplayPlayerStat()
    {
        string value = "0";
        switch (ResourceToDisplay)
        {
            //case PlayerStatKind.Coin:
            //    value = $"{Mathf.FloorToInt(PlayerStateData.Current.Coins):N0}";
            //    break;

        }

        UpdateDisplayValue(value);
    }
}
