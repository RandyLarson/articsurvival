using System.Linq;

public static class DataExtensions
{

    public static bool IsEmpty(this SerializableGuid src) => src == (SerializableGuid)System.Guid.Empty;

}