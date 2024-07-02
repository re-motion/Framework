// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Concurrent;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>Represents an access type enum value.</summary>
  /// <remarks>
  /// Use the static <see cref="AccessType.Get(Enum)"/> methods to convert an enum to an access type.
  /// <note>For the set of basic access types see <see cref="T:Remotion.Security.GeneralAccessTypes"/>.</note>
  /// </remarks>
  /// <summary>Wraps an enum and exposes the enum information as string.</summary>
  /// <remarks>Used for example to cross web service boundaries, when the server is unaware of a given enum type.</remarks>
  public struct EnumWrapper : IEquatable<EnumWrapper>
  {
    private static readonly ConcurrentDictionary<Enum, EnumWrapper> s_enumWrapperCache = new ConcurrentDictionary<Enum, EnumWrapper>();

    ///<remarks>Optimized for memory allocations</remarks>
    private static readonly Func<Enum, EnumWrapper> s_createEnumWrapperFromEnumValueFunc = CreateEnumWrapperFromEnumValue;

    /// <summary>
    /// Gets an <see cref="EnumWrapper"/>, setting the wrapper's <see cref="Name"/> to a string of the format "valueName|typeName".
    /// </summary>
    /// <param name="valueName">The enum value name to be set.</param>
    /// <param name="typeName">The type name to be integrated into the name.</param>
    public static EnumWrapper Get (string valueName, string typeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("valueName", valueName);
      ArgumentUtility.CheckNotNullOrEmpty("typeName", typeName);

      return new EnumWrapper(BuildEnumName(valueName, typeName));
    }

    /// <summary>
    /// Gets an <see cref="EnumWrapper"/>, setting the wrapper's name to the specified string.
    /// </summary>
    /// <param name="name">The name to be set.</param>
    public static EnumWrapper Get (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      return new EnumWrapper(name);
    }

    /// <summary>
    /// Gets an <see cref="EnumWrapper"/>, setting the wrapper's name to a string of the format "enumValue|enumType".
    /// </summary>
    /// <param name="enumValue">The enum value.</param>
    public static EnumWrapper Get (Enum enumValue)
    {
      ArgumentUtility.CheckNotNull("enumValue", enumValue);

      return s_enumWrapperCache.GetOrAdd(enumValue, s_createEnumWrapperFromEnumValueFunc);
    }

    private static EnumWrapper CreateEnumWrapperFromEnumValue (Enum enumValue)
    {
      Type type = enumValue.GetType();
      if (Attribute.IsDefined(type, typeof(FlagsAttribute), false))
      {
        throw new ArgumentException(
            string.Format(
                "Enumerated type '{0}' cannot be wrapped. Only enumerated types without the {1} can be wrapped.",
                type.GetFullNameSafe(),
                typeof(FlagsAttribute).GetFullNameSafe()),
            "enumValue");
      }

      return Get(BuildEnumName(enumValue.ToString(), TypeUtility.GetPartialAssemblyQualifiedName(enumValue.GetType())));
    }

    private static string BuildEnumName (string valueName, string typeName)
    {
      return valueName + "|" + typeName;
    }

    private readonly string _name;

    private EnumWrapper (string name)
    {
      _name = string.Intern(name);
    }

    public string Name
    {
      get { return _name; }
    }

    public override string ToString ()
    {
      return _name;
    }

    public bool Equals (EnumWrapper other)
    {
      return string.Equals(this._name, other._name);
    }

    public override bool Equals (object? obj)
    {
      if (obj == null)
        return false;
      if (obj.GetType() != typeof(EnumWrapper))
        return false;
      return Equals((EnumWrapper)obj);
    }

    public override int GetHashCode ()
    {
      return _name.GetHashCode();
    }
  }
}
