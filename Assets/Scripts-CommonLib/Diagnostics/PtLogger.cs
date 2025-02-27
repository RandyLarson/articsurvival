using UnityEngine;

public static class PtLogger
{
    static public void Log(string label, string format, params object[] args) => Log(label, LogType.Log, format, args);
    static public void Log(string label, LogType level, string format, params object[] args)
    {
#if UNITY_EDITOR
        if (Debug.isDebugBuild)
        {
            Debug.LogFormat(level, LogOption.None, (UnityEngine.Object)null, format, args);
        }
#endif
    }

    static public void Log(this UnityEngine.Object orgObject, string format, params object[] args) => Log(orgObject, LogType.Log, format, args);
    static public void Log(this UnityEngine.Object orgObject, LogType level, string format, params object[] args)
    {
#if UNITY_EDITOR
        if (Debug.isDebugBuild)
        {
            Debug.LogFormat(level, LogOption.None, orgObject, format, args);
        }
#endif
    }
}
