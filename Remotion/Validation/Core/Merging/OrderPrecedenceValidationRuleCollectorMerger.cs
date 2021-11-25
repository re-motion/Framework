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
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Rules;

namespace Remotion.Validation.Merging
{
  /// <summary>
  /// Implements the <see cref="IValidationRuleCollectorMerger"/> interface to merge <see cref="IValidationRule"/>s 
  /// based on the order of precedence established during retrieval of the <see cref="IValidationRuleCollector"/>s.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof(IValidationRuleCollectorMerger), Lifetime = LifetimeKind.Singleton)]
  public class OrderPrecedenceValidationRuleCollectorMerger : ValidationRuleCollectorMergerBase
  {
    private readonly IPropertyValidatorExtractorFactory _propertyValidatorExtractorFactory;
    private readonly IObjectValidatorExtractorFactory _objectValidatorExtractorFactory;

    public OrderPrecedenceValidationRuleCollectorMerger (
        IPropertyValidatorExtractorFactory propertyValidatorExtractorFactory,
        IObjectValidatorExtractorFactory objectValidatorExtractorFactory)
    {
      ArgumentUtility.CheckNotNull("propertyValidatorExtractorFactory", propertyValidatorExtractorFactory);
      ArgumentUtility.CheckNotNull("objectValidatorExtractorFactory", objectValidatorExtractorFactory);

      _propertyValidatorExtractorFactory = propertyValidatorExtractorFactory;
      _objectValidatorExtractorFactory = objectValidatorExtractorFactory;
    }

    protected override ILogContext CreateNewLogContext ()
    {
      return new DefaultLogContext();
    }

    protected override void MergeRules (
        IEnumerable<ValidationRuleCollectorInfo> collectorGroup,
        List<IAddingPropertyValidationRuleCollector> collectedPropertyValidationRules,
        List<IAddingObjectValidationRuleCollector> collectedObjectValidationRules,
        ILogContext logContext)
    {
      ArgumentUtility.CheckNotNull("collectorGroup", collectorGroup);
      ArgumentUtility.CheckNotNull("collectedPropertyValidationRules", collectedPropertyValidationRules);
      ArgumentUtility.CheckNotNull("collectedObjectValidationRules", collectedObjectValidationRules);
      ArgumentUtility.CheckNotNull("logContext", logContext);

      var collectorInfos = collectorGroup.ToArray();

      //first: remove registered validator types for all collectors in same group (provider is responsible for order of collectors!)
      ApplyRemovingPropertyValidationRules(collectedPropertyValidationRules, logContext, collectorInfos);
      ApplyRemovingObjectValidationRules(collectedObjectValidationRules, logContext, collectorInfos);

      //second: add new rules
      ApplyAddingPropertyValidationRules(collectedPropertyValidationRules, collectorInfos);
      ApplyObjectValidationRules(collectedObjectValidationRules, collectorInfos);
    }

    private void ApplyRemovingPropertyValidationRules (
        IReadOnlyCollection<IAddingPropertyValidationRuleCollector> collectedPropertyValidationRules,
        ILogContext logContext,
        IReadOnlyCollection<ValidationRuleCollectorInfo> collectorInfos)
    {
      if (collectedPropertyValidationRules.Any())
      {
        var removingPropertyValidatorRegistrations = collectorInfos
            .Select(ci => ci.Collector)
            .SelectMany(c => c.RemovedPropertyRules)
            .SelectMany(r => r.Validators);

        var validatorExtractor = _propertyValidatorExtractorFactory.Create(removingPropertyValidatorRegistrations, logContext);
        foreach (var validationRule in collectedPropertyValidationRules)
          validationRule.ApplyRemoveValidatorRegistrations(validatorExtractor);
      }
    }

    private void ApplyRemovingObjectValidationRules (
        IReadOnlyCollection<IAddingObjectValidationRuleCollector> collectedObjectValidationRules,
        ILogContext logContext,
        IReadOnlyCollection<ValidationRuleCollectorInfo> collectorInfos)
    {
      if (collectedObjectValidationRules.Any())
      {
        var removingObjectValidatorRegistrations = collectorInfos
            .Select(ci => ci.Collector)
            .SelectMany(c => c.RemovedObjectRules)
            .SelectMany(r => r.Validators);

        var validatorExtractor = _objectValidatorExtractorFactory.Create(removingObjectValidatorRegistrations, logContext);
        foreach (var validationRule in collectedObjectValidationRules)
          validationRule.ApplyRemoveValidatorRegistrations(validatorExtractor);
      }
    }

    private void ApplyAddingPropertyValidationRules (
        List<IAddingPropertyValidationRuleCollector> collectedPropertyValidationRules,
        IReadOnlyCollection<ValidationRuleCollectorInfo> collectorInfos)
    {
      collectedPropertyValidationRules.AddRange(collectorInfos.SelectMany(g => g.Collector.AddedPropertyRules));
    }

    private void ApplyObjectValidationRules (
        List<IAddingObjectValidationRuleCollector> collectedObjectValidationRules,
        IReadOnlyCollection<ValidationRuleCollectorInfo> collectorInfos)
    {
      collectedObjectValidationRules.AddRange(collectorInfos.SelectMany(g => g.Collector.AddedObjectRules));
    }
  }
}
