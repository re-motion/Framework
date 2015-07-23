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
using FluentValidation;
using Remotion.ServiceLocation;
using Remotion.Validation.Implementation;
using Remotion.Validation.Rules;

namespace Remotion.Validation.Merging
{
  /// <summary>
  /// Implements the <see cref="IValidationCollectorMerger"/> interface to merge <see cref="IValidationRule"/>s 
  /// based on the order of precendence established during retrieval of the <see cref="IComponentValidationCollector"/>s.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof (IValidationCollectorMerger), Lifetime = LifetimeKind.Singleton)]
  public class OrderPrecedenceValidationCollectorMerger : ValidationCollectorMergerBase
  {
    private readonly IPropertyValidatorExtractorFactory _propertyValidatorExtractorFactory;

    public OrderPrecedenceValidationCollectorMerger (IPropertyValidatorExtractorFactory propertyValidatorExtractorFactory)
    {
      _propertyValidatorExtractorFactory = propertyValidatorExtractorFactory;
    }

    protected override ILogContext CreateNewLogContext ()
    {
      return new DefaultLogContext ();
    }

    protected override void MergeRules (IEnumerable<ValidationCollectorInfo> collectorGroup, List<IAddingComponentPropertyRule> collectedRules, ILogContext logContext)
    {
      var collectorInfos = collectorGroup.ToArray();

      //first: remove registered validator types for all collectors in same group (provider is responsible for order of collectors!)
      if (collectedRules.Any())
      {
        var registrationsWithContext = GetValidatorRegistrationsWithContext (collectorInfos);
        var propertyValidatorExtractor = _propertyValidatorExtractorFactory.Create (registrationsWithContext, logContext);
        foreach (var validationRule in collectedRules)
          validationRule.ApplyRemoveValidatorRegistrations (propertyValidatorExtractor);
      }

      //second: add new rules
      collectedRules.AddRange (collectorInfos.SelectMany (g => g.Collector.AddedPropertyRules));
    }

    private IEnumerable<ValidatorRegistrationWithContext> GetValidatorRegistrationsWithContext (ValidationCollectorInfo[] collectorInfos)
    {
      return collectorInfos
          .Select (ci => ci.Collector)
          .SelectMany (c => c.RemovedPropertyRules)
          .SelectMany (
              r => r.Validators,
              (propertyRule, validatorRegistration) => new ValidatorRegistrationWithContext (validatorRegistration, propertyRule));
    }
  }
}