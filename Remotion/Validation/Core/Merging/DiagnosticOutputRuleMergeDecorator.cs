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
using System.Linq;
using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Remotion.Logging;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Rules;

namespace Remotion.Validation.Merging
{
  /// <summary>
  /// Implements the <see cref="IValidationCollectorMerger"/> interface as a decorator which logs the merge operation via the application default logging infrastructure.
  /// </summary>
  [ImplementationFor (typeof (IValidationCollectorMerger), Position = 0, RegistrationType = RegistrationType.Decorator)]
  public class DiagnosticOutputRuleMergeDecorator : IValidationCollectorMerger
  {
    private readonly ILog _logger;
    private readonly IValidationCollectorMerger _validationCollectorMerger;
    private readonly IValidatorFormatter _validatorFormatter;

    public DiagnosticOutputRuleMergeDecorator (
        IValidationCollectorMerger validationCollectorMerger,
        IValidatorFormatter validatorFormatter,
        ILogManager logManager)
    {
      ArgumentUtility.CheckNotNull ("validationCollectorMerger", validationCollectorMerger);
      ArgumentUtility.CheckNotNull ("validatorFormatter", validatorFormatter);
      ArgumentUtility.CheckNotNull ("logManager", logManager);

      _validationCollectorMerger = validationCollectorMerger;
      _validatorFormatter = validatorFormatter;
      _logger = logManager.GetLogger (typeof (DiagnosticOutputRuleMergeDecorator));
    }

    public IValidationCollectorMerger ValidationCollectorMerger
    {
      get { return _validationCollectorMerger; }
    }

    public IValidatorFormatter ValidatorFormatter
    {
      get { return _validatorFormatter; }
    }

    public ValidationCollectorMergeResult Merge (IEnumerable<IEnumerable<ValidationCollectorInfo>> validationCollectorInfos)
    {
      ArgumentUtility.CheckNotNull ("validationCollectorInfos", validationCollectorInfos);

      var collectorInfos = validationCollectorInfos.ToArray();

      var beforeMergeLog = string.Empty;
      if (_logger.IsInfoEnabled())
        beforeMergeLog = GetLogBefore (collectorInfos);

      var validationCollectorMergeResult = _validationCollectorMerger.Merge (collectorInfos);

      var afterMergeLog = string.Empty;
      if (_logger.IsInfoEnabled())
        afterMergeLog = GetLogAfter (validationCollectorMergeResult.CollectedRules, validationCollectorMergeResult.LogContext);

      if (_logger.IsInfoEnabled())
      {
        //"after"-output provides better initial diagnostics. 
        //"before"-output is usually only analyzed when the problem is not obvious from the "after"-output.
        _logger.Info (afterMergeLog);
        _logger.Info (beforeMergeLog);
      }

      return validationCollectorMergeResult;
    }

    protected virtual string GetTypeName (Type type)
    {
      return type.Name;
    }

    protected virtual string GetDomainTypeName (Type domainType)
    {
      return domainType.FullName;
    }

    private string GetLogBefore (IEnumerable<IEnumerable<ValidationCollectorInfo>> validationCollectorInfos)
    {
      var sb = new StringBuilder();
      sb.AppendLine();
      sb.Append ("BEFORE MERGE:");
      foreach (var validationCollectorInfo in validationCollectorInfos.SelectMany (v => v))
      {
        var allProperties = GetAllPropertiesForCollector (validationCollectorInfo).ToArray();
        if (!allProperties.Any())
          continue;

        sb.AppendLine().AppendLine();
        sb.Append ("-> " + GetTypeName (validationCollectorInfo.ProviderType) + "#" + DisplayCollectorType (validationCollectorInfo));

        AppendPropertyCollectionOutput (allProperties, validationCollectorInfo, sb);
      }
      return sb.ToString();
    }

    private string GetLogAfter (IEnumerable<IValidationRule> mergedRules, ILogContext logContext)
    {
      var sb = new StringBuilder();
      sb.AppendLine();
      sb.Append ("AFTER MERGE:");

      var propertyRules = mergedRules.OfType<PropertyRule>().ToArray();
      var allProperties = propertyRules.Select (mr => (PropertyInfo) mr.Member).Distinct ();
      foreach (var property in allProperties)
      {
        IPropertyInformation actualProperty = PropertyInfoAdapter.Create(property);
        var propertyRulesForMember = propertyRules.Where (pr =>(PropertyInfoAdapter.Create( (PropertyInfo) pr.Member)).Equals(actualProperty)).ToArray();
        var validators = propertyRulesForMember.SelectMany (pr => pr.Validators).ToArray();
        var logContextInfos = propertyRulesForMember.SelectMany (logContext.GetLogContextInfos).ToArray();
        AppendPropertyRuleOutput (actualProperty, validators, logContextInfos, sb);
      }

      foreach (var validationRule in mergedRules.Except (propertyRules))
        AppendValidationRuleOutput (validationRule, sb, logContext);

      return sb.ToString();
    }

    private void AppendPropertyCollectionOutput (
        IPropertyInformation[] allProperties,
        ValidationCollectorInfo validationCollectorInfo,
        StringBuilder sb)
    {
      var loggedProperties = new Dictionary<IPropertyInformation, bool> ();
      foreach (var property in allProperties)
      {
        if (!loggedProperties.ContainsKey (property))
        {
          IPropertyInformation actualProperty = property;
          var removedRegistrations = validationCollectorInfo.Collector.RemovedPropertyRules
              .Where (pr => pr.Property.Equals(actualProperty))
              .SelectMany (pr => pr.Validators).ToArray();
          var addedHardValidators = validationCollectorInfo.Collector.AddedPropertyRules
              .Where (pr => pr.IsHardConstraint && pr.Property.Equals(actualProperty))
              .SelectMany (pr => pr.Validators).ToArray();
          var addedSoftValidators = validationCollectorInfo.Collector.AddedPropertyRules
              .Where (pr => !pr.IsHardConstraint && pr.Property.Equals(actualProperty))
              .SelectMany (pr => pr.Validators).ToArray();
          var addedMetaValidations = validationCollectorInfo.Collector.AddedPropertyMetaValidationRules
              .Where (pr => pr.Property.Equals(actualProperty))
              .SelectMany (pr => pr.MetaValidationRules).ToArray();

          if (removedRegistrations.Any() || addedHardValidators.Any() || addedSoftValidators.Any() || addedMetaValidations.Any())
            AppendPropertyOutput (actualProperty, removedRegistrations, addedHardValidators, addedSoftValidators, addedMetaValidations, sb);

          loggedProperties[property] = true;
        }
      }
    }

    private void AppendPropertyOutput (
        IPropertyInformation actualProperty,
        ValidatorRegistration[] removedRegistrations,
        IPropertyValidator[] addedHardValidators,
        IPropertyValidator[] addedSoftValidators,
        IMetaValidationRule[] addedMetaValidations,
        StringBuilder sb)
    {
      AppendPropertyName (actualProperty, sb);
      AppendGroupedValidatorRegistrationsOutput (removedRegistrations, "REMOVED VALIDATORS:", sb);
      AppendGroupedValidatorsOutput (addedHardValidators, "ADDED HARD CONSTRAINT VALIDATORS:", sb);
      AppendGroupedValidatorsOutput (addedSoftValidators, "ADDED SOFT CONSTRAINT VALIDATORS:", sb);
      AppendCollectionData (sb, addedMetaValidations, "ADDED META VALIDATION RULES:", i => GetTypeName (i.GetType()));
    }

    private void AppendPropertyRuleOutput (
        IPropertyInformation actualProperty,
        IPropertyValidator[] validators,
        LogContextInfo[] logContextInfos,
        StringBuilder sb)
    {
      AppendPropertyName (actualProperty, sb);
      AppendGroupedValidatorsOutput (validators, "VALIDATORS:", sb);
      AppendMergeOutput (logContextInfos, sb);
    }

    private void AppendValidationRuleOutput (IValidationRule validationRule, StringBuilder sb, ILogContext logContext)
    {
      sb.AppendLine().AppendLine();
      sb.Append (new string (' ', 4) + "-> ");
      sb.Append (GetTypeName (validationRule.GetType()));

      AppendGroupedValidatorsOutput (validationRule.Validators.ToArray(), "VALIDATORS:", sb);
      AppendMergeOutput (logContext.GetLogContextInfos (validationRule).ToArray(), sb);
    }

    private void AppendMergeOutput (LogContextInfo[] logContextInfos, StringBuilder sb)
    {
      if (!logContextInfos.Any())
        return;

      sb.AppendLine();
      sb.Append (new string (' ', 8) + "MERGE LOG:");
      foreach (var logContextInfo in logContextInfos)
      {
        var removingCollectors =
            logContextInfo.RemovingValidatorRegistrationsWithContext.Select (ci => ci.RemovingComponentPropertyRule.CollectorType.Name)
                .Distinct()
                .ToArray();
        var logEntry = string.Format (
            "'{0}' was removed from {1} '{2}'",
            GetTypeName (logContextInfo.RemvovedValidator.GetType()),
            removingCollectors.Count() > 1 ? "collectors" : "collector",
            string.Join (", ", removingCollectors));
        sb.AppendLine();
        sb.Append (new string (' ', 8) + "-> " + logEntry);
      }
    }

    private void AppendGroupedValidatorsOutput (IPropertyValidator[] validators, string title, StringBuilder sb)
    {
      var groupedValidators =
          validators.Select (v => _validatorFormatter.Format (v, GetTypeName))
              .GroupBy (f => f, (f, elements) => Tuple.Create (f, elements.Count()))
              .ToArray();
      AppendCollectionData (sb, groupedValidators, title, i => i.Item1 + " (x" + i.Item2 + ")");
    }

    private void AppendGroupedValidatorRegistrationsOutput (ValidatorRegistration[] validatorRegistrations, string title, StringBuilder sb)
    {
      var groupedValidatorRegistrations =
          validatorRegistrations
              .Select (
                  vr =>
                      GetTypeName (vr.ValidatorType)
                      + (vr.CollectorTypeToRemoveFrom != null ? "#" + GetTypeName (vr.CollectorTypeToRemoveFrom) : string.Empty))
              .GroupBy (f => f, (f, elements) => Tuple.Create (f, elements.Count())).ToArray();
      AppendCollectionData (sb, groupedValidatorRegistrations, title, i => i.Item1 + " (x" + i.Item2 + ")");
    }

    private void AppendCollectionData<TValidatedType> (StringBuilder sb, TValidatedType[] data, string title, Func<TValidatedType, string> formater)
    {
      if (!data.Any())
        return;

      sb.AppendLine();
      sb.Append (new string (' ', 8) + title);
      foreach (var item in data)
      {
        sb.AppendLine();
        sb.Append (new string (' ', 8) + "-> ");
        sb.Append (formater (item));
      }
    }

    private IEnumerable<IPropertyInformation> GetAllPropertiesForCollector (ValidationCollectorInfo validationCollectorInfo)
    {
      return validationCollectorInfo.Collector.RemovedPropertyRules.Select (pr => pr.Property)
          .Concat (validationCollectorInfo.Collector.AddedPropertyRules.Select (pr => pr.Property))
          .Concat (validationCollectorInfo.Collector.AddedPropertyMetaValidationRules.Select (pr => pr.Property))
          .Distinct ();
    }

    private string DisplayCollectorType (ValidationCollectorInfo validationCollectorInfo)
    {
      var collectorType = validationCollectorInfo.Collector.GetType();
      var typeName = GetTypeName (collectorType);

      if (collectorType.IsGenericType)
      {
        if (typeName.IndexOf ('`') > 0)
          return string.Format ("{0}<{1}>", typeName.Remove (typeName.IndexOf ('`')), GetTypeName (collectorType.GetGenericArguments()[0]));
      }

      return typeName;
    }

    private void AppendPropertyName (IPropertyInformation actualProperty, StringBuilder sb)
    {
      sb.AppendLine().AppendLine();
      sb.Append (new string (' ', 4) + "-> ");
      sb.Append (actualProperty.DeclaringType != null ? GetDomainTypeName (actualProperty.DeclaringType.ConvertToRuntimeType()) + "#" : string.Empty);
      sb.Append (actualProperty.Name);
    }
  }
}