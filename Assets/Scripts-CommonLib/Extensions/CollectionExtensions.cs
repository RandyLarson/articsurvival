using System.Collections.Generic;
using System.Linq;

public static class CollectionExtensions
{
    public static T PickOne<T>(this IEnumerable<T> from) where T : class
    {
        if (from?.Any() == true)
        {
            return from
                .Skip(UnityEngine.Random.Range(0, from.Count()))
                .First();
        }

        return null;
    }
}
