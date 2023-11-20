using System;

namespace Remotion.Development.UnitTesting.Reflection
{
  public static class TypeExtensions
  {
    /// <summary>
    ///   Returns the value that a field of the given <paramref name="type"/> is initialized with by .NET.
    /// </summary>
    public static object? GetDefaultValue (this Type type)
    {
      return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
  }
}
