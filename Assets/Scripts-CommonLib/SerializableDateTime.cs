using System;
using UnityEngine;

/// <summary>
/// Serializable wrapper for System.DateTime.
/// Can be implicitly converted to/from System.DateTime.
/// </summary>
[Serializable]
public struct SerializableDateTime : ISerializationCallbackReceiver
{
    public DateTime Dt { get; private set; }
    [SerializeField] private string SerializedDateTime;

    public SerializableDateTime(DateTime dt)
    {
        Dt = dt;
        SerializedDateTime = null;
    }

    public override bool Equals(object obj)
    {
        return obj is SerializableDateTime dt &&
                this.Dt.Equals(dt.Dt);
    }

    public override int GetHashCode()
    {
        return Dt.GetHashCode();
    }

    public void OnAfterDeserialize()
    {
        try
        {
            Dt = DateTime.Parse(SerializedDateTime);
        }
        catch
        {
            Dt = DateTime.MinValue;
            //Debug.Log("SerializedDateTime", LogType.Warning, "Attempted to parse invalid DateTime string '{0}'. DateTime will set to DateTime.min", SerializedDateTime);
        }
    }

    public void OnBeforeSerialize()
    {
        SerializedDateTime = Dt.ToString();
    }

    public override string ToString() => Dt.ToString();
    public string ToString(string format) => Dt.ToString(format);

    public TimeSpan TimeOfDay => Dt.TimeOfDay;
    public DateTime Date => Dt.Date;
    public int Hour => Dt.Hour;
    public int Minute => Dt.Minute;
    public int Second => Dt.Second;

    public static bool operator ==(SerializableDateTime a, DateTime b) => a.Dt == b;
    public static bool operator !=(SerializableDateTime a, DateTime b) => a.Dt != b;
    public static TimeSpan operator -(SerializableDateTime a, DateTime b) => a.Dt - b;
    public static TimeSpan operator -(SerializableDateTime a, SerializableDateTime b) => a.Dt - b.Dt;
    public static DateTime operator +(SerializableDateTime a, TimeSpan b) => a.Dt + b;
    public static DateTime operator -(SerializableDateTime a, TimeSpan b) => a.Dt - b;

    public static implicit operator SerializableDateTime(DateTime dt) => new SerializableDateTime(dt);
    public static implicit operator DateTime(SerializableDateTime dt) => dt.Dt;
    public static implicit operator string(SerializableDateTime dt) => dt.ToString();
}

public static class SerializableDateTimeExtensions
{
    public static string ToShortTimeString(this SerializableDateTime dt) => ((DateTime)dt).ToShortTimeString();
}