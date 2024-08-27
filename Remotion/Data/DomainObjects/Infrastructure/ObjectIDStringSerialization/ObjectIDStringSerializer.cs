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
using System.Diagnostics.CodeAnalysis;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectIDStringSerialization
{
  /// <summary>
  /// Provides a mechanism to serialize <see cref="ObjectID"/> instances to and from strings.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public class ObjectIDStringSerializer
  {
    public const char Delimiter = '|';

    public static readonly ObjectIDStringSerializer Instance = new ObjectIDStringSerializer();

    private static readonly string s_delimiterAsString = Delimiter.ToString();
    private static readonly string s_delimitedGuidTypeName = Delimiter + typeof(Guid).GetFullNameChecked();
    private static readonly string s_delimitedInt32TypeName = Delimiter + typeof(Int32).GetFullNameChecked();
    private static readonly string s_delimitedStringTypeName = Delimiter + typeof(String).GetFullNameChecked();

    private ObjectIDStringSerializer ()
    {
    }

    /// <summary>
    /// Serializes the specified object ID into a <see cref="String"/> value that can later be re-parsed via <see cref="Parse"/> or <see cref="TryParse"/>.
    /// </summary>
    public string Serialize (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

#if DEBUG
      if (objectID.ClassID.IndexOf(Delimiter) != -1)
      {
        throw new ArgumentException(
            string.Format(
                "The class id '{0}' contains the delimiter character ('{1}'). This is not allowed when serializing the ObjectID.",
                objectID.ClassID,
                Delimiter),
            "objectID");
      }
#endif

      return string.Concat(
          objectID.ClassID,
          s_delimiterAsString,
          objectID.Value.ToString(),
          GetDelimitedValueTypeName(objectID.Value));
    }

    /// <summary>
    /// Parses the specified object ID string, throwing an <see cref="FormatException"/> if an error occurs.
    /// </summary>
    public ObjectID Parse (string objectIDString)
    {
      ArgumentUtility.CheckNotNull("objectIDString", objectIDString);
      return ParseWithCustomErrorHandler(objectIDString, msg => throw new FormatException(msg));
    }

    /// <summary>
    /// Parses the specified object ID string, indicating by a boolean return value whether the operation was completed successfully.
    /// </summary>

    public bool TryParse (string objectIDString, [MaybeNullWhen(false)] out ObjectID result)
    {
      ArgumentUtility.CheckNotNull("objectIDString", objectIDString);

      result = ParseWithCustomErrorHandler(objectIDString, msg => null!);
      // ReSharper disable once ConditionIsAlwaysTrueOrFalse
      return result != null;
    }

    private ObjectID ParseWithCustomErrorHandler (string objectIDString, Func<string, ObjectID> errorHandler)
    {
      ArgumentUtility.CheckNotNull("objectIDString", objectIDString);

      var indexOfClassIDDelimiter = objectIDString.IndexOf(Delimiter);
      var indexOfValuePart = indexOfClassIDDelimiter + 1;
      var indexOfTypeDelimiter = objectIDString.LastIndexOf(Delimiter);
      var indexOfTypePart = indexOfTypeDelimiter + 1;

      if (objectIDString == string.Empty)
        return errorHandler(string.Format("Serialized ObjectID '{0}' is not correctly formatted: it must not be empty.", objectIDString));

      if (indexOfClassIDDelimiter < 0)
        return errorHandler(string.Format("Serialized ObjectID '{0}' is not correctly formatted: it must have three parts.", objectIDString));

      if (indexOfValuePart == objectIDString.Length)
        return errorHandler(string.Format("Serialized ObjectID '{0}' is not correctly formatted: it must have three parts.", objectIDString));

      if (indexOfTypeDelimiter < indexOfValuePart)
        return errorHandler(string.Format("Serialized ObjectID '{0}' is not correctly formatted: it must have three parts.", objectIDString));

      if (indexOfClassIDDelimiter < 2)
        return errorHandler(string.Format("Serialized ObjectID '{0}' is not correctly formatted: the class id must not be empty.", objectIDString));

      if (indexOfTypeDelimiter < indexOfValuePart + 1)
        return errorHandler(string.Format("Serialized ObjectID '{0}' is not correctly formatted: the value must not be empty.", objectIDString));

      if (indexOfTypeDelimiter > objectIDString.Length - 2)
        return errorHandler(string.Format("Serialized ObjectID '{0}' is not correctly formatted: the type must not be empty.", objectIDString));

      var classIDPart = objectIDString.Substring(0, indexOfClassIDDelimiter);
      var valuePart = objectIDString.Substring(indexOfValuePart, indexOfTypeDelimiter - indexOfValuePart);
      var typePart = objectIDString.Substring(indexOfTypePart);

      var typeName = typePart;

      IObjectIDValueParser? valueParser = typeName switch
      {
          "System.Guid" => GuidObjectIDValueParser.Instance,
          "System.Int32" => Int32ObjectIDValueParser.Instance,
          "System.String" => StringObjectIDValueParser.Instance,
          _ => null
      };

      if (valueParser == null)
        return errorHandler(string.Format("Serialized ObjectID '{0}' is invalid: type '{1}' is not supported.", objectIDString, typePart));

      if (!valueParser.TryParse(valuePart, out var value))
      {
        return errorHandler(string.Format(
            "Serialized ObjectID '{0}' is not correctly formatted: value '{1}' is not in the correct format for type '{2}'.",
            objectIDString,
            valuePart,
            typePart));
      }

      if (!MappingConfiguration.Current.ContainsClassDefinition(classIDPart))
        return errorHandler(string.Format("Serialized ObjectID '{0}' is invalid: '{1}' is not a valid class ID.", objectIDString, classIDPart));

      var classDefinition = MappingConfiguration.Current.GetClassDefinition(classIDPart);
      return new ObjectID(classDefinition, value);
    }

    private string GetDelimitedValueTypeName (object value)
    {
      if (value is Guid)
        return s_delimitedGuidTypeName;
      else if (value is Int32)
        return s_delimitedInt32TypeName;
      else if (value is String)
        return s_delimitedStringTypeName;
      else
        throw new FormatException(string.Format("ObjectIDs with a value of type '{0}' are not supported.", value.GetType().GetFullNameSafe()));
    }
  }
}
