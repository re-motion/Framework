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
using System.Text;
using Microsoft.Extensions.Logging;
using Remotion.Logging;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Merging
{
  /// <summary>
  /// Implements the <see cref="IValidationRuleCollectorMerger"/> interface as a decorator which logs the merge operation via the application default logging infrastructure.
  /// </summary>
  [ImplementationFor(typeof(IValidationRuleCollectorMerger), Position = 0, RegistrationType = RegistrationType.Decorator)]
  public class DiagnosticOutputValidationRuleMergeDecorator : IValidationRuleCollectorMerger
  {
    private readonly ILogger _logger;
    private readonly IValidationRuleCollectorMerger _validationRuleCollectorMerger;
    private readonly IValidatorFormatter _validatorFormatter;

    public DiagnosticOutputValidationRuleMergeDecorator (
        IValidationRuleCollectorMerger validationRuleCollectorMerger,
        IValidatorFormatter validatorFormatter,
        ILoggerFactory loggerFactory)
    {
      ArgumentUtility.CheckNotNull("validationRuleCollectorMerger", validationRuleCollectorMerger);
      ArgumentUtility.CheckNotNull("validatorFormatter", validatorFormatter);
      ArgumentUtility.CheckNotNull("loggerFactory", loggerFactory);

      _validationRuleCollectorMerger = validationRuleCollectorMerger;
      _validatorFormatter = validatorFormatter;
      _logger = loggerFactory.CreateLogger<DiagnosticOutputValidationRuleMergeDecorator>();
    }

    public IValidationRuleCollectorMerger ValidationRuleCollectorMerger
    {
      get { return _validationRuleCollectorMerger; }
    }

    public IValidatorFormatter ValidatorFormatter
    {
      get { return _validatorFormatter; }
    }

    public ValidationCollectorMergeResult Merge (IEnumerable<IEnumerable<ValidationRuleCollectorInfo>> validationCollectorInfos)
    {
      ArgumentUtility.CheckNotNull("validationCollectorInfos", validationCollectorInfos);

      var collectorInfos = validationCollectorInfos.ToArray();

      var beforeMergeLog = string.Empty;
      if (_logger.IsEnabled(LogLevel.Information))
        beforeMergeLog = GetLogBefore(collectorInfos);

      var validationCollectorMergeResult = _validationRuleCollectorMerger.Merge(collectorInfos);

      var afterMergeLog = string.Empty;
      if (_logger.IsEnabled(LogLevel.Information))
        afterMergeLog = GetLogAfter(
            validationCollectorMergeResult.CollectedPropertyValidationRules,
            validationCollectorMergeResult.CollectedObjectValidationRules,
            validationCollectorMergeResult.LogContext);

      if (_logger.IsEnabled(LogLevel.Information))
      {
        //"after"-output provides better initial diagnostics. 
        //"before"-output is usually only analyzed when the problem is not obvious from the "after"-output.
        _logger.LogInformation(afterMergeLog);
        _logger.LogInformation(beforeMergeLog);
      }

      return validationCollectorMergeResult;
    }

    protected virtual string GetTypeName (Type type)
    {
      return type.Name;
    }

    protected virtual string GetDomainTypeName (Type domainType)
    {
      return domainType.GetFullNameChecked();
    }

    private string GetLogBefore (IEnumerable<IEnumerable<ValidationRuleCollectorInfo>> validationCollectorInfos)
    {
      var sb = new StringBuilder();
      sb.AppendLine();
      sb.Append("BEFORE MERGE:");
      foreach (var validationCollectorInfo in validationCollectorInfos.SelectMany(v => v))
      {
        var allProperties = GetAllPropertiesForCollector(validationCollectorInfo).ToArray();
        if (!allProperties.Any())
          continue;

        sb.AppendLine().AppendLine();
        sb.Append("-> " + GetTypeName(validationCollectorInfo.ProviderType) + "#" + DisplayCollectorType(validationCollectorInfo));

        AppendPropertyCollectionOutput(allProperties, validationCollectorInfo, sb);
        //AppendObjectCollectionOutput (allProperties, validationCollectorInfo, sb);
        //TODO RM-5906 implement diagnostic output for object rules
      }
      return sb.ToString();
    }

    private string GetLogAfter (
        IEnumerable<IAddingPropertyValidationRuleCollector> mergedPropertyRules,
        IEnumerable<IAddingObjectValidationRuleCollector> mergedObjectRules,
        ILogContext logContext)
    {
      var sb = new StringBuilder();
      sb.AppendLine();
      sb.Append("AFTER MERGE:");

      foreach (var propertyRulesForMember in mergedPropertyRules.ToLookup(pr => pr.Property))
      {
        var validators = propertyRulesForMember.SelectMany(pr => pr.Validators).ToArray();
        var logContextInfos = propertyRulesForMember.SelectMany(logContext.GetLogContextInfos).ToArray();
        AppendPropertyRuleOutput(propertyRulesForMember.Key, validators, logContextInfos, sb);
      }

      {
        var validators = mergedObjectRules.SelectMany(pr => pr.Validators).ToArray();
        var logContextInfos = mergedObjectRules.SelectMany(logContext.GetLogContextInfos).ToArray();
        //AppendObjectRuleOutput (mergedObjectRules.Key, validators, logContextInfos, sb);
        //TODO RM-5906 implement diagnostic output for object rules
      }

      return sb.ToString();
    }

    private void AppendPropertyCollectionOutput (
        IPropertyInformation[] allProperties,
        ValidationRuleCollectorInfo validationRuleCollectorInfo,
        StringBuilder sb)
    {
      var loggedProperties = new Dictionary<IPropertyInformation, bool>();
      foreach (var property in allProperties)
      {
        if (!loggedProperties.ContainsKey(property))
        {
          IPropertyInformation actualProperty = property;
          var removedRegistrations = validationRuleCollectorInfo.Collector.RemovedPropertyRules
              .Where(pr => pr.Property.Equals(actualProperty))
              .SelectMany(pr => pr.Validators).ToArray();
          var addedNonRemovableValidators = validationRuleCollectorInfo.Collector.AddedPropertyRules
              .Where(pr => !pr.IsRemovable && pr.Property.Equals(actualProperty))
              .SelectMany(pr => pr.Validators).ToArray();
          var addedRemovableValidators = validationRuleCollectorInfo.Collector.AddedPropertyRules
              .Where(pr => pr.IsRemovable && pr.Property.Equals(actualProperty))
              .SelectMany(pr => pr.Validators).ToArray();
          var addedMetaValidations = validationRuleCollectorInfo.Collector.PropertyMetaValidationRules
              .Where(pr => pr.Property.Equals(actualProperty))
              .SelectMany(pr => pr.MetaValidationRules).ToArray();

          if (removedRegistrations.Any() || addedNonRemovableValidators.Any() || addedRemovableValidators.Any() || addedMetaValidations.Any())
            AppendPropertyOutput(actualProperty, removedRegistrations, addedNonRemovableValidators, addedRemovableValidators, addedMetaValidations, sb);

          loggedProperties[property] = true;
        }
      }
    }

    private void AppendPropertyOutput (
        IPropertyInformation actualProperty,
        RemovingPropertyValidatorRegistration[] removingPropertyValidatorRegistrations,
        IPropertyValidator[] addedNonRemovableValidators,
        IPropertyValidator[] addedRemovableValidators,
        IPropertyMetaValidationRule[] addedPropertyMetaValidationRules,
        StringBuilder sb)
    {
      AppendPropertyName(actualProperty, sb);
      AppendGroupedRemovingPropertyValidatorRegistrationsOutput(removingPropertyValidatorRegistrations, "REMOVED VALIDATORS:", sb);
      AppendGroupedValidatorsOutput(addedNonRemovableValidators, "ADDED NON-REMOVABLE VALIDATORS:", sb);
      AppendGroupedValidatorsOutput(addedRemovableValidators, "ADDED REMOVABLE VALIDATORS:", sb);
      AppendCollectionData(sb, addedPropertyMetaValidationRules, "ADDED META VALIDATION RULES:", i => GetTypeName(i.GetType()));
    }

    private void AppendPropertyRuleOutput (
        IPropertyInformation actualProperty,
        IPropertyValidator[] validators,
        PropertyValidatorLogContextInfo[] logContextInfos,
        StringBuilder sb)
    {
      AppendPropertyName(actualProperty, sb);
      AppendGroupedValidatorsOutput(validators, "VALIDATORS:", sb);
      AppendMergeOutput(logContextInfos, sb);
    }

    private void AppendMergeOutput (PropertyValidatorLogContextInfo[] logContextInfos, StringBuilder sb)
    {
      if (!logContextInfos.Any())
        return;

      sb.AppendLine();
      sb.Append(new string(' ', 8) + "MERGE LOG:");
      foreach (var logContextInfo in logContextInfos)
      {
        var removingCollectors =
            logContextInfo.RemovingPropertyValidatorRegistrations.Select(ci => ci.RemovingPropertyValidationRuleCollector.CollectorType.Name)
                .Distinct()
                .ToArray();
        var logEntry = string.Format(
            "'{0}' was removed from {1} '{2}'",
            GetTypeName(logContextInfo.RemovedValidator.GetType()),
            removingCollectors.Count() > 1 ? "collectors" : "collector",
            string.Join(", ", removingCollectors));
        sb.AppendLine();
        sb.Append(new string(' ', 8) + "-> " + logEntry);
      }
    }

    private void AppendGroupedValidatorsOutput (IPropertyValidator[] validators, string title, StringBuilder sb)
    {
      var groupedValidators =
          validators.Select(v => _validatorFormatter.Format(v, GetTypeName))
              .GroupBy(f => f, (f, elements) => Tuple.Create(f, elements.Count()))
              .ToArray();
      AppendCollectionData(sb, groupedValidators, title, i => i.Item1 + " (x" + i.Item2 + ")");
    }

    private void AppendGroupedRemovingPropertyValidatorRegistrationsOutput (RemovingPropertyValidatorRegistration[] validatorRegistrations, string title, StringBuilder sb)
    {
      var groupedValidatorRegistrations =
          validatorRegistrations
              .Select(
                  vr =>
                      GetTypeName(vr.ValidatorType)
                      + (vr.CollectorTypeToRemoveFrom != null ? "#" + GetTypeName(vr.CollectorTypeToRemoveFrom) : string.Empty)
                      + (vr.ValidatorPredicate != null ? "#Conditional" : string.Empty)
                  )
              .GroupBy(f => f, (f, elements) => Tuple.Create(f, elements.Count())).ToArray();
      AppendCollectionData(sb, groupedValidatorRegistrations, title, i => i.Item1 + " (x" + i.Item2 + ")");
    }

    private void AppendCollectionData<TValidatedType> (StringBuilder sb, TValidatedType[] data, string title, Func<TValidatedType, string> formater)
    {
      if (!data.Any())
        return;

      sb.AppendLine();
      sb.Append(new string(' ', 8) + title);
      foreach (var item in data)
      {
        sb.AppendLine();
        sb.Append(new string(' ', 8) + "-> ");
        sb.Append(formater(item));
      }
    }

    private IEnumerable<IPropertyInformation> GetAllPropertiesForCollector (ValidationRuleCollectorInfo validationRuleCollectorInfo)
    {
      return validationRuleCollectorInfo.Collector.RemovedPropertyRules.Select(pr => pr.Property)
          .Concat(validationRuleCollectorInfo.Collector.AddedPropertyRules.Select(pr => pr.Property))
          .Concat(validationRuleCollectorInfo.Collector.PropertyMetaValidationRules.Select(pr => pr.Property))
          .Distinct();
    }

    private string DisplayCollectorType (ValidationRuleCollectorInfo validationRuleCollectorInfo)
    {
      var collectorType = validationRuleCollectorInfo.Collector.GetType();
      var typeName = GetTypeName(collectorType);

      if (collectorType.IsGenericType)
      {
        if (typeName.IndexOf('`') > 0)
          return string.Format("{0}<{1}>", typeName.Remove(typeName.IndexOf('`')), GetTypeName(collectorType.GetGenericArguments()[0]));
      }

      return typeName;
    }

    private void AppendPropertyName (IPropertyInformation actualProperty, StringBuilder sb)
    {
      sb.AppendLine().AppendLine();
      sb.Append(new string(' ', 4) + "-> ");
      sb.Append(actualProperty.DeclaringType != null ? GetDomainTypeName(actualProperty.DeclaringType.ConvertToRuntimeType()) + "#" : string.Empty);
      sb.Append(actualProperty.Name);
    }
  }
}
