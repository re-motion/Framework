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
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Rules;

namespace Remotion.Validation.Providers
{
  /// <summary>
  /// Base class for <see cref="IValidationCollectorProvider"/> implementations which use property annotations to define the constraints. 
  /// </summary>
  public abstract class AttributeBasedValidationCollectorProviderBase : IValidationCollectorProvider
  {
    protected AttributeBasedValidationCollectorProviderBase ()
    {
    }

    protected abstract ILookup<Type, IAttributesBasedValidationPropertyRuleReflector> CreatePropertyRuleReflectors (IEnumerable<Type> types);

    public IEnumerable<IEnumerable<ValidationCollectorInfo>> GetValidationCollectors (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      var reflectorLookUp = CreatePropertyRuleReflectors (types);
      return reflectorLookUp.Select (g => GetValidationCollector (g.Key, g))
          .Select (collector => EnumerableUtility.Singleton (new ValidationCollectorInfo (collector, GetType())));
    }

    private IComponentValidationCollector GetValidationCollector (
        Type validatedType,
        IEnumerable<IAttributesBasedValidationPropertyRuleReflector> propertyRuleReflectors)
    {
      var validationRules = propertyRuleReflectors.Select (r => SetValidationRulesForProperty (r, validatedType)).ToList();

      return new AttributeBasedComponentValidationCollector (
          validatedType,
          validationRules.Select (vr => vr.Item1).Concat (validationRules.Select (vr => vr.Item2)),
          validationRules.Select (vr => vr.Item3),
          validationRules.Select (vr => vr.Item4));
    }

    private Tuple<AddingComponentPropertyRule, AddingComponentPropertyRule, AddingComponentPropertyMetaValidationRule, RemovingComponentPropertyRule>
        SetValidationRulesForProperty (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector, Type validatedType)
    {
      var propertyAccessExpression = propertyRuleReflector.GetPropertyAccessExpression (validatedType);
      var propertyInfo = propertyRuleReflector.ValidatedProperty;
      var propertyFunc = propertyAccessExpression.Compile();
      var collectorType = typeof (AttributeBasedComponentValidationCollector);

      var addingPropertyRule = GetAddingPropertyRule (propertyRuleReflector, validatedType, propertyInfo, propertyFunc, collectorType);
      var addingHardConstraintPropertyRule = GetAddingHardConstraintPropertyRule (
          propertyRuleReflector, validatedType, propertyInfo, propertyFunc, collectorType);
      var addingMetaValidationPropertyRule = GetAddingComponentPropertyMetaValidationRule (propertyRuleReflector, propertyInfo, collectorType);
      var removingPropertyRule = GetRemovingComponentPropertyRule (propertyRuleReflector, propertyInfo, collectorType);

      return Tuple.Create (
          addingPropertyRule,
          addingHardConstraintPropertyRule,
          addingMetaValidationPropertyRule,
          removingPropertyRule);
    }

    private AddingComponentPropertyRule GetAddingPropertyRule (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        Type validatedType,
        PropertyInfo propertyInfo,
        Func<object, object> propertyFunc,
        Type collectorType)
    {
      var propertyRule = new AddingComponentPropertyRule (validatedType, propertyInfo, propertyFunc, collectorType);

      foreach (var validator in propertyRuleReflector.GetAddingPropertyValidators ())
        propertyRule.RegisterValidator (validator);

      return propertyRule;
    }

    private AddingComponentPropertyRule GetAddingHardConstraintPropertyRule (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        Type validatedType,
        PropertyInfo propertyInfo,
        Func<object, object> propertyFunc,
        Type collectorType)
    {
      var propertyRule = new AddingComponentPropertyRule (validatedType, propertyInfo, propertyFunc, collectorType);

      propertyRule.SetHardConstraint ();
      foreach (var validator in propertyRuleReflector.GetHardConstraintPropertyValidators ())
        propertyRule.RegisterValidator (validator);

      return propertyRule;
    }

    private RemovingComponentPropertyRule GetRemovingComponentPropertyRule (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        PropertyInfo propertyInfo,
        Type collectorType)
    {
      var propertyRule = new RemovingComponentPropertyRule (propertyInfo, collectorType);
      
      foreach (var validatorRegistration in propertyRuleReflector.GetRemovingPropertyRegistrations())
        propertyRule.RegisterValidator (validatorRegistration.ValidatorType, validatorRegistration.CollectorTypeToRemoveFrom);
      
      return propertyRule;
    }

    private AddingComponentPropertyMetaValidationRule GetAddingComponentPropertyMetaValidationRule (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        PropertyInfo propertyInfo,
        Type collectorType)
    {
      var propertyRule = new AddingComponentPropertyMetaValidationRule (propertyInfo, collectorType);
      
      foreach (var metaValidationRule in propertyRuleReflector.GetMetaValidationRules())
        propertyRule.RegisterMetaValidationRule (metaValidationRule);
      
      return propertyRule;
    }
  }
}