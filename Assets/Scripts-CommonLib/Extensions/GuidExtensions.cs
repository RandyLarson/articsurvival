using System;

public static class GuidExtensions
{
    public static bool IsEmpty(this Guid src) => src == Guid.Empty;
}
