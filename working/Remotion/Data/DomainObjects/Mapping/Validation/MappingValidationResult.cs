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
using System.Text;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation
{
  /// <summary>
  /// <see cref="MappingValidationResult"/> is returned by the validate-methods of the mapping configuration validators and contains the information,
  /// if the rule is valid. If not, it also returns an error message.
  /// </summary>
  public class MappingValidationResult
  {
    public static MappingValidationResult CreateValidResult ()
    {
      return new MappingValidationResult (true, null);
    }

    [JetBrains.Annotations.StringFormatMethod ("messageFormat")]
    public static MappingValidationResult CreateInvalidResult (string messageFormat, params object[] args)
    {
      ArgumentUtility.CheckNotNullOrEmpty("messageFormat", messageFormat);
      ArgumentUtility.CheckNotNull ("args", args);

      return new MappingValidationResult (false, string.Format (messageFormat, args));
    }

    [JetBrains.Annotations.StringFormatMethod ("messageFormat")]
    public static MappingValidationResult CreateInvalidResultForType (Type type, string messageFormat, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("messageFormat", messageFormat);
      ArgumentUtility.CheckNotNull ("args", args);

      return CreateInvalidResultForType (TypeAdapter.Create (type), messageFormat, args);
    }

    [JetBrains.Annotations.StringFormatMethod ("messageFormat")]
    public static MappingValidationResult CreateInvalidResultForType (ITypeInformation type, string messageFormat, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("messageFormat", messageFormat);
      ArgumentUtility.CheckNotNull ("args", args);

      return new MappingValidationResult (false, BuildMessage (type, null, null, messageFormat, args));
    }

    [JetBrains.Annotations.StringFormatMethod ("messageFormat")]
    public static MappingValidationResult CreateInvalidResultForProperty (IPropertyInformation propertyInfo, string messageFormat, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNullOrEmpty ("messageFormat", messageFormat);
      ArgumentUtility.CheckNotNull ("args", args);

      return new MappingValidationResult (false, BuildMessage (propertyInfo.DeclaringType, propertyInfo, null, messageFormat, args));
    }

    [JetBrains.Annotations.StringFormatMethod ("messageFormat")]
    public static MappingValidationResult CreateInvalidResultForRelation (string relationID, IPropertyInformation propertyInfo, string messageFormat, params object[] args)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("relationID", relationID);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNullOrEmpty ("messageFormat", messageFormat);
      ArgumentUtility.CheckNotNull ("args", args);

      return new MappingValidationResult (false, BuildMessage (propertyInfo.DeclaringType, propertyInfo, relationID, messageFormat, args));
    }

    private static string BuildMessage (
        ITypeInformation type, 
        IPropertyInformation property,
        string relationID,
        string messageFormat,
        params object[] args)
    {
      var stringBuilder = new StringBuilder();

      stringBuilder.AppendFormat (messageFormat, args);
      if (type != null)
      {
        stringBuilder.AppendLine();
        stringBuilder.AppendLine();
        stringBuilder.AppendFormat ("Declaring type: {0}", type);
        if (property != null)
        {
          stringBuilder.AppendLine();
          stringBuilder.AppendFormat ("Property: {0}", property.Name);
        }
        if (relationID != null)
        {
          stringBuilder.AppendLine();
          stringBuilder.AppendFormat ("Relation ID: {0}", relationID);
        }
      }

      return stringBuilder.ToString();
    }

    private readonly bool _isValid;
    private readonly string _message;

    protected MappingValidationResult (bool isValid, string message)
    {
      _isValid = isValid;
      _message = message;
    }

    public bool IsValid
    {
      get { return _isValid; }
    }

    public string Message
    {
      get { return _message; }
    }
  }
}