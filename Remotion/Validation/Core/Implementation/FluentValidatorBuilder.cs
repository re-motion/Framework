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
using FluentValidation;
using FluentValidation.Internal;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Merging;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Providers;
using Remotion.Validation.Utilities;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Collector-based validator builder.  
  /// </summary>
  [ImplementationFor (typeof (IValidatorBuilder), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Single)]
  public class FluentValidatorBuilder : IValidatorBuilder
  {
    private readonly IValidationCollectorProvider _validationCollectorProvider;
    private readonly IValidationCollectorMerger _validationCollectorMerger;
    private readonly IMetaRulesValidatorFactory _metaRulesValidatorFactory;
    private readonly IValidationRuleMetadataService _validationRuleGlobalizationService;
    private readonly IMemberInformationNameResolver _memberInformationNameResolver;
    private readonly ICollectorValidator _collectorValidator;

    public FluentValidatorBuilder (
        IValidationCollectorProvider validationCollectorProvider,
        IValidationCollectorMerger validationCollectorMerger,
        IMetaRulesValidatorFactory metaRuleValidatorFactory,
        IValidationRuleMetadataService validationRuleGlobalizationService,
        IMemberInformationNameResolver memberInformationNameResolver,
        ICollectorValidator collectorValidator)
    {
      ArgumentUtility.CheckNotNull ("validationCollectorProvider", validationCollectorProvider);
      ArgumentUtility.CheckNotNull ("validationCollectorMerger", validationCollectorMerger);
      ArgumentUtility.CheckNotNull ("metaRuleValidatorFactory", metaRuleValidatorFactory);
      ArgumentUtility.CheckNotNull ("validationRuleGlobalizationService", validationRuleGlobalizationService);
      ArgumentUtility.CheckNotNull ("memberInformationNameResolver", memberInformationNameResolver);
      ArgumentUtility.CheckNotNull ("collectorValidator", collectorValidator);
      
      _validationCollectorProvider = validationCollectorProvider;
      _validationCollectorMerger = validationCollectorMerger;
      _metaRulesValidatorFactory = metaRuleValidatorFactory;
      _validationRuleGlobalizationService = validationRuleGlobalizationService;
      _memberInformationNameResolver = memberInformationNameResolver;
      _collectorValidator = collectorValidator;
    }

    public IValidationCollectorProvider ComponentValidationCollectorProvider
    {
      get { return _validationCollectorProvider; }
    }

    public IValidationCollectorMerger ValidationCollectorMerger
    {
      get { return _validationCollectorMerger; }
    }

    public IMetaRulesValidatorFactory MetaRulesValidatorFactory
    {
      get { return _metaRulesValidatorFactory; }
    }

    public IValidationRuleMetadataService ValidationRulesGlobalizationService
    {
      get { return _validationRuleGlobalizationService; }
    }

    public IMemberInformationNameResolver MemberInformationNameResolver
    {
      get { return _memberInformationNameResolver; }
    }

    public ICollectorValidator CollectorValidator
    {
      get { return _collectorValidator; }
    }

    public IValidator BuildValidator (Type validatedType)
    {
      ArgumentUtility.CheckNotNull ("validatedType", validatedType);
      
      var allCollectors = _validationCollectorProvider.GetValidationCollectors (new[] { validatedType }).Select (c => c.ToArray ()).ToArray ();
      ValidateCollectors (allCollectors.SelectMany (c => c));
      var validationCollectorMergeResult = _validationCollectorMerger.Merge (allCollectors);
      ValidateMetaRules (allCollectors, validationCollectorMergeResult.CollectedRules);
      
      ApplyTechnicalPropertyNames (validationCollectorMergeResult.CollectedRules.OfType<PropertyRule>());
      ApplyLocalization (validationCollectorMergeResult.CollectedRules, validatedType);

      return new Validator (validationCollectorMergeResult.CollectedRules, validatedType);
    }

    private void ValidateCollectors (IEnumerable<ValidationCollectorInfo> allCollectors)
    {
      foreach (var validationCollectorInfo in allCollectors)
        _collectorValidator.CheckValid (validationCollectorInfo.Collector);
    }

    private void ValidateMetaRules (IEnumerable<IEnumerable<ValidationCollectorInfo>> allCollectors, IEnumerable<IValidationRule> allRules)
    {
      var addingComponentPropertyMetaValidationRules =
          allCollectors.SelectMany (cg => cg).Select (ci => ci.Collector).SelectMany (c => c.AddedPropertyMetaValidationRules);
      var metaRulesValidator = _metaRulesValidatorFactory.CreateMetaRuleValidator (addingComponentPropertyMetaValidationRules);
      
      var metaValidationResults = metaRulesValidator.Validate (allRules.ToArray()).Where (r => !r.IsValid).ToArray();
      if (metaValidationResults.Any())
        throw CreateMetaValidationException (metaValidationResults);
    }

    private void ApplyTechnicalPropertyNames (IEnumerable<PropertyRule> propertyRules)
    {
      foreach (var propertyRule in propertyRules)
        propertyRule.PropertyName = _memberInformationNameResolver.GetPropertyName (PropertyInfoAdapter.Create (propertyRule.GetPropertyInfo ()));
    }

    private void ApplyLocalization (IEnumerable<IValidationRule> validationRules, Type validatedType)
    {
      foreach (var validationRule in validationRules)
        _validationRuleGlobalizationService.ApplyMetadata (validationRule, validatedType);
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