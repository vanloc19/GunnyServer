using System.Collections.Generic;
using System.Linq;

namespace Bussiness.Helpers
{
    public static class Functions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size, int startIndex)
        {
            for (var i = 1; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}