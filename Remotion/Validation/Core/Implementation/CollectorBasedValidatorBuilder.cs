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
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Merging;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Providers;
using Remotion.Validation.Rules;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Collector-based validator builder.  
  /// </summary>
  [ImplementationFor (typeof (IValidatorBuilder), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Single)]
  public class CollectorBasedValidatorBuilder : IValidatorBuilder
  {
    public IValidationCollectorProvider ComponentValidationCollectorProvider { get; }
    public IValidationCollectorMerger ValidationCollectorMerger { get; }
    public IMetaRulesValidatorFactory MetaRulesValidatorFactory { get; }
    public IValidationMessageFactory ValidationMessageFactory { get; }
    public IMemberInformationNameResolver MemberInformationNameResolver { get; }
    public ICollectorValidator CollectorValidator { get; }

    public CollectorBasedValidatorBuilder (
        IValidationCollectorProvider validationCollectorProvider,
        IValidationCollectorMerger validationCollectorMerger,
        IMetaRulesValidatorFactory metaRuleValidatorFactory,
        IValidationMessageFactory validationMessageFactory,
        IMemberInformationNameResolver memberInformationNameResolver,
        ICollectorValidator collectorValidator)
    {
      ArgumentUtility.CheckNotNull ("validationCollectorProvider", validationCollectorProvider);
      ArgumentUtility.CheckNotNull ("validationCollectorMerger", validationCollectorMerger);
      ArgumentUtility.CheckNotNull ("metaRuleValidatorFactory", metaRuleValidatorFactory);
      ArgumentUtility.CheckNotNull ("validationMessageFactory", validationMessageFactory);
      ArgumentUtility.CheckNotNull ("memberInformationNameResolver", memberInformationNameResolver);
      ArgumentUtility.CheckNotNull ("collectorValidator", collectorValidator);

      ComponentValidationCollectorProvider = validationCollectorProvider;
      ValidationCollectorMerger = validationCollectorMerger;
      MetaRulesValidatorFactory = metaRuleValidatorFactory;
      ValidationMessageFactory = validationMessageFactory;
      MemberInformationNameResolver = memberInformationNameResolver;
      CollectorValidator = collectorValidator;
    }


    public IValidator BuildValidator (Type validatedType)
    {
      ArgumentUtility.CheckNotNull ("validatedType", validatedType);

      var allCollectors = ComponentValidationCollectorProvider.GetValidationCollectors (new[] { validatedType }).Select (c => c.ToArray()).ToArray();
      ValidateCollectors (allCollectors.SelectMany (c => c));
      var validationCollectorMergeResult = ValidationCollectorMerger.Merge (allCollectors);
      ValidateMetaRules (allCollectors, validationCollectorMergeResult.CollectedRules);

      var validationRules = validationCollectorMergeResult.CollectedRules.Select (r => r.CreateValidationRule(ValidationMessageFactory));

      return new Validator (validationRules, validatedType);
    }

    private void ValidateCollectors (IEnumerable<ValidationCollectorInfo> allCollectors)
    {
      foreach (var validationCollectorInfo in allCollectors)
        CollectorValidator.CheckValid (validationCollectorInfo.Collector);
    }

    private void ValidateMetaRules (
        IEnumerable<IEnumerable<ValidationCollectorInfo>> allCollectors,
        IEnumerable<IAddingComponentPropertyRule> allRules)
    {
      var addingComponentPropertyMetaValidationRules =
          allCollectors.SelectMany (cg => cg).Select (ci => ci.Collector).SelectMany (c => c.AddedPropertyMetaValidationRules);
      var metaRulesValidator = MetaRulesValidatorFactory.CreateMetaRuleValidator (addingComponentPropertyMetaValidationRules);

      var metaValidationResults = metaRulesValidator.Validate (allRules.ToArray()).Where (r => !r.IsValid).ToArray();
      if (metaValidationResults.Any())
        throw CreateMetaValidationException (metaValidationResults);
    }

    private ValidationConfigurationException CreateMetaValidationException (IEnumerable<MetaValidationRuleValidationResult> metaValidationResults)
    {
      var messages = new StringBuilder();
      foreach (var validationResult in metaValidationResults)
      {
        if (messages.Length > 0)
          messages.AppendLine (new string ('-', 10));
        messages.AppendLine (validationResult.Message);
      }

      return new ValidationConfigurationException (messages.ToString().Trim());
    }
  }
}