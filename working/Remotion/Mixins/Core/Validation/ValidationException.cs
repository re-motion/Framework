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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  /// <summary>
  /// Thrown when there is an error in the mixin configuration which is detected during validation of the configuration. The problem prevents
  /// code being generated from the configuration. See also <see cref="ConfigurationException"/>.
  /// </summary>
  [Serializable]
  public class ValidationException : Exception
  {
    private readonly SerializableValidationLogData _serializableValidationLogData;

    public SerializableValidationLogData ValidationLogData
    {
      get { return _serializableValidationLogData; }
    }

    private static string BuildExceptionString (ValidationLogData data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      var sb = new StringBuilder ("Some parts of the mixin configuration could not be validated.");
      foreach (ValidationResult item in data.GetResults())
      {
        if (item.TotalRulesExecuted != item.Successes.Count)
        {
          sb.AppendLine();
          sb.AppendFormat (
              "{0} '{1}', {2} rules executed",
              item.ValidatedDefinition.GetType().Name,
              item.ValidatedDefinition.FullName,
              item.TotalRulesExecuted);
          sb.AppendLine();

          string contextString = item.GetDefinitionContextPath();
          if (contextString.Length > 0)
          {
            sb.AppendFormat ("Context: " + contextString);
            sb.AppendLine();
          }

          AppendResults (sb, "unexpected exceptions", item.Exceptions);
          AppendResults (sb, "warnings", item.Warnings);
          AppendResults (sb, "failures", item.Failures);
        }
      }
      return sb.ToString();
    }

    private static void AppendResults<T> (StringBuilder sb, string title, ICollection<T> resultList)
        where T : IDefaultValidationResultItem
    {
      if (resultList.Count > 0)
      {
        sb.AppendFormat ("  {0} - {1}", title, resultList.Count);
        sb.AppendLine();
        foreach (T resultItem in resultList)
          sb.AppendFormat ("    {0} ({1})", resultItem.Message, resultItem.RuleName);
        sb.AppendLine();
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class and creates a descriptive message from the validation log.
    /// </summary>
    /// <param name="validationLogData">The validation log data.</param>
    /// <exception cref="ArgumentNullException">The log is empty.</exception>
    public ValidationException (ValidationLogData validationLogData)
        : base (BuildExceptionString (validationLogData))
    {
      _serializableValidationLogData = validationLogData.MakeSerializable();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class during deserialization.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
    /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
    protected ValidationException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      _serializableValidationLogData =
          (SerializableValidationLogData) info.GetValue ("SerializableValidationLogData", typeof (SerializableValidationLogData));
    }

    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData (info, context);
      info.AddValue ("SerializableValidationLogData", _serializableValidationLogData);
    }
  }
}