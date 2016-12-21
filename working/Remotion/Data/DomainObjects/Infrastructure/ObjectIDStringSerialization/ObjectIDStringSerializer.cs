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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectIDStringSerialization
{
  /// <summary>
  /// Provides a mechanism to serialize <see cref="ObjectID"/> instances to and from strings.
  /// </summary>
  public class ObjectIDStringSerializer
  {
    public static readonly ObjectIDStringSerializer Instance = new ObjectIDStringSerializer ();

    private const char c_delimiter = '|';
    private const string c_escapedDelimiter = "&pipe;";
    private const string c_escapedDelimiterPlaceholder = "&amp;pipe;";

    private ObjectIDStringSerializer ()
    {
    }

    /// <summary>
    /// Checks whether the given <paramref name="value"/> can be used as an <see cref="ObjectID"/> value if the <see cref="ObjectID"/> should
    /// later be serialized to a <see cref="string"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    public void CheckSerializableStringValue (string value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      if (value.IndexOf (c_escapedDelimiterPlaceholder) >= 0)
      {
        var message = string.Format ("Value cannot contain '{0}'.", c_escapedDelimiterPlaceholder);
        throw new ArgumentException (message, "value");
      }
    }

    /// <summary>
    /// Serializes the specified object ID into a <see cref="String"/> value that can later be re-parsed via <see cref="Parse"/> or <see cref="TryParse"/>.
    /// </summary>
    public string Serialize (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      Type valueType = objectID.Value.GetType ();

      return Escape (objectID.ClassID) + c_delimiter +
             Escape (objectID.Value.ToString ()) + c_delimiter +
             Escape (valueType.FullName);
    }

    /// <summary>
    /// Parses the specified object ID string, throwing an <see cref="FormatException"/> if an error occurs.
    /// </summary>
    public ObjectID Parse (string objectIDString)
    {
      ArgumentUtility.CheckNotNull ("objectIDString", objectIDString);
      return ParseWithCustomErrorHandler (objectIDString, msg => { throw new FormatException (msg); });
    }

    /// <summary>
    /// Parses the specified object ID string, indicating by a boolean return value whether the operation was completed successfully.
    /// </summary>
    public bool TryParse (string objectIDString, out ObjectID result)
    {
      ArgumentUtility.CheckNotNull ("objectIDString", objectIDString);

      result = ParseWithCustomErrorHandler (objectIDString, msg => null);
      return result != null;
    }

    private ObjectID ParseWithCustomErrorHandler (string objectIDString, Func<string, ObjectID> errorHandler)
    {
      ArgumentUtility.CheckNotNull ("objectIDString", objectIDString);

      if (objectIDString == string.Empty)
      {
        var message = string.Format ("Serialized ObjectID '{0}' is not correctly formatted: it must not be empty.", objectIDString);
        return errorHandler (message);
      }

      string[] parts = objectIDString.Split (c_delimiter);
      if (parts.Length != 3)
      {
        var message = string.Format ("Serialized ObjectID '{0}' is not correctly formatted: it should have three parts.", objectIDString);
        return errorHandler (message);
      }

      for (int i = 0; i < parts.Length; i++)
        parts[i] = Unescape (parts[i]);

      Type type = TypeUtility.GetType (parts[2], false);
      if (type == null)
      {
        var message = string.Format ("Serialized ObjectID '{0}' is invalid: '{1}' is not the name of a loadable type.", objectIDString, parts[2]);
        return errorHandler (message);
      }

      var valueParser = GetValueParser (type);
      if (valueParser == null)
      {
        var message = string.Format ("Serialized ObjectID '{0}' is invalid: type '{1}' is not supported.", objectIDString, parts[2]);
        return errorHandler (message);
      }

      object value;
      if (!valueParser.TryParse (parts[1], out value))
      {
        var message = string.Format (
            "Serialized ObjectID '{0}' is not correctly formatted: value '{1}' is not in the correct format for type '{2}'.",
            objectIDString,
            parts[1], 
            parts[2]);
        return errorHandler (message);
      }
      
      if (!MappingConfiguration.Current.ContainsClassDefinition (parts[0]))
      {
        var message = string.Format ("Serialized ObjectID '{0}' is invalid: '{1}' is not a valid class ID.", objectIDString, parts[0]);
        return errorHandler (message);
      }

      var classDefinition = MappingConfiguration.Current.GetClassDefinition (parts[0]);
      return new ObjectID(classDefinition, value);
    }

    private IObjectIDValueParser GetValueParser (Type type)
    {
      if (type == typeof (Guid))
        return GuidObjectIDValueParser.Instance;
      else if (type == typeof (int))
        return Int32ObjectIDValueParser.Instance;
      else if (type == typeof (string))
        return StringObjectIDValueParser.Instance;
      else
        return null;
    }

    private string Escape (string value)
    {
      if (value.IndexOf (c_escapedDelimiter) >= 0)
        value = value.Replace (c_escapedDelimiter, c_escapedDelimiterPlaceholder);

      if (value.IndexOf (c_delimiter) >= 0)
        value = value.Replace (c_delimiter.ToString(), c_escapedDelimiter);

      return value;
    }

    private static string Unescape (string value)
    {
      if (value.IndexOf (c_escapedDelimiter) >= 0)
        value = value.Replace (c_escapedDelimiter, c_delimiter.ToString());

      if (value.IndexOf (c_escapedDelimiterPlaceholder) >= 0)
        value = value.Replace (c_escapedDelimiterPlaceholder, c_escapedDelimiter);

      return value;
    }
  }
}
