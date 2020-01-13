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
using Remotion.Validation.RuleCollectors;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Collector-based validator builder.  
  /// </summary>
  [ImplementationFor (typeof (IValidatorBuilder), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Single)]
  public class ValidationRuleCollectorBasedValidatorBuilder : IValidatorBuilder
  {
    public IValidationRuleCollectorProvider ValidationRuleCollectorProvider { get; }
    public IValidationRuleCollectorMerger ValidationRuleCollectorMerger { get; }
    public IMetaRulesValidatorFactory MetaRulesValidatorFactory { get; }
    public IValidationMessageFactory ValidationMessageFactory { get; }
    public IMemberInformationNameResolver MemberInformationNameResolver { get; }
    public IValidationRuleCollectorValidator CollectorValidator { get; }

    public ValidationRuleCollectorBasedValidatorBuilder (
        IValidationRuleCollectorProvider validationRuleCollectorProvider,
        IValidationRuleCollectorMerger validationRuleCollectorMerger,
        IMetaRulesValidatorFactory metaRuleValidatorFactory,
        IValidationMessageFactory validationMessageFactory,
        IMemberInformationNameResolver memberInformationNameResolver,
        IValidationRuleCollectorValidator collectorValidator)
    {
      ArgumentUtility.CheckNotNull ("validationRuleCollectorProvider", validationRuleCollectorProvider);
      ArgumentUtility.CheckNotNull ("validationRuleCollectorMerger", validationRuleCollectorMerger);
      ArgumentUtility.CheckNotNull ("metaRuleValidatorFactory", metaRuleValidatorFactory);
      ArgumentUtility.CheckNotNull ("validationMessageFactory", validationMessageFactory);
      ArgumentUtility.CheckNotNull ("memberInformationNameResolver", memberInformationNameResolver);
      ArgumentUtility.CheckNotNull ("collectorValidator", collectorValidator);

      ValidationRuleCollectorProvider = validationRuleCollectorProvider;
      ValidationRuleCollectorMerger = validationRuleCollectorMerger;
      MetaRulesValidatorFactory = metaRuleValidatorFactory;
      ValidationMessageFactory = validationMessageFactory;
      MemberInformationNameResolver = memberInformationNameResolver;
      CollectorValidator = collectorValidator;
    }


    public IValidator BuildValidator (Type validatedType)
    {
      ArgumentUtility.CheckNotNull ("validatedType", validatedType);

      var allCollectors = ValidationRuleCollectorProvider.GetValidationRuleCollectors (new[] { validatedType }).Select (c => c.ToArray()).ToArray();
      ValidateCollectors (allCollectors.SelectMany (c => c));
      var validationCollectorMergeResult = ValidationRuleCollectorMerger.Merge (allCollectors);
      ValidateMetaRules (allCollectors, validationCollectorMergeResult.CollectedPropertyValidationRules);

      var propertyValidationRules = validationCollectorMergeResult.CollectedPropertyValidationRules
          .Select (r => r.CreateValidationRule (ValidationMessageFactory));
      var objectValidationRules = validationCollectorMergeResult.CollectedObjectValidationRules
          .Select (r => r.CreateValidationRule (ValidationMessageFactory));
      var validationRules = propertyValidationRules.Concat (objectValidationRules);

      return new Validator (validationRules, validatedType);
    }

    private void ValidateCollectors (IEnumerable<ValidationRuleCollectorInfo> allCollectors)
    {
      foreach (var validationCollectorInfo in allCollectors)
        CollectorValidator.CheckValid (validationCollectorInfo.Collector);
    }

    private void ValidateMetaRules (
        IEnumerable<IEnumerable<ValidationRuleCollectorInfo>> allCollectors,
        IEnumerable<IAddingPropertyValidationRuleCollector> allRules)
    {
      var addingComponentPropertyMetaValidationRules =
          allCollectors.SelectMany (cg => cg).Select (ci => ci.Collector).SelectMany (c => c.PropertyMetaValidationRules);
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