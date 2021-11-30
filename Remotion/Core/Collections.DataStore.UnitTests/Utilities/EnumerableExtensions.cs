using System;
using System.Collections;

namespace Remotion.Collections.DataStore.UnitTests.Utilities
{
  internal static class EnumerableExtensions
  {
    public static IEnumerable ToNonGenericEnumerable (this IEnumerable enumerable)
    {
      return enumerable;
    }
  }
}
