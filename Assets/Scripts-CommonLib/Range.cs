using System;

[System.Serializable]
public class Range
{
    public float min = .1f;
    public float max = 3f;
    public Range(float iMin, float iMax)
    {
        min = iMin;
        max = iMax;
    }

    internal float Length => max - min;

    internal bool IsInRange(float v)
    {
        return (min <= v) && (v <= max);
    }

    internal float PickOne()
    {
        return UnityEngine.Random.Range(min, max);
    }

    internal int PickOneInt()
    {
        var res = (int)MathF.Round(UnityEngine.Random.Range(min, max));
        return res;
    }
}
