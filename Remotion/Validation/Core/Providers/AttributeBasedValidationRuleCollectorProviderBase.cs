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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.RuleCollectors;

namespace Remotion.Validation.Providers
{
  using ValidationRulesResult = Tuple<IAddingPropertyValidationRuleCollector, IAddingPropertyValidationRuleCollector, PropertyMetaValidationRuleCollector, RemovingPropertyValidationRuleCollector>;

  /// <summary>
  /// Base class for <see cref="IValidationRuleCollectorProvider"/> implementations which use property annotations to define the constraints. 
  /// </summary>
  public abstract class AttributeBasedValidationRuleCollectorProviderBase : IValidationRuleCollectorProvider
  {
    private static readonly ConcurrentDictionary<(Type ValidatedType, Type PropertyType), Func<IAttributesBasedValidationPropertyRuleReflector, ValidationRulesResult>> s_cachedDelegatesForSetValidationRulesForProperty = 
        new ConcurrentDictionary<(Type, Type), Func<IAttributesBasedValidationPropertyRuleReflector, ValidationRulesResult>>();

    protected AttributeBasedValidationRuleCollectorProviderBase ()
    {
    }

    protected abstract ILookup<Type, IAttributesBasedValidationPropertyRuleReflector> CreatePropertyRuleReflectors (IEnumerable<Type> types);

    public IEnumerable<IEnumerable<ValidationRuleCollectorInfo>> GetValidationRuleCollectors (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      var reflectorLookUp = CreatePropertyRuleReflectors (types);
      return reflectorLookUp.Select (g => GetValidationCollector (g.Key, g))
          .Select (collector => EnumerableUtility.Singleton (new ValidationRuleCollectorInfo (collector, GetType())));
    }

    private IValidationRuleCollector GetValidationCollector (
        Type validatedType,
        IEnumerable<IAttributesBasedValidationPropertyRuleReflector> propertyRuleReflectors)
    {
      var validationRules = propertyRuleReflectors.Select (r => SetValidationRulesForProperty (r, validatedType)).ToList();

      return new AttributeBasedValidationRuleCollector (
          validatedType,
          validationRules.Select (vr => vr.Item1).Concat (validationRules.Select (vr => vr.Item2)),
          validationRules.Select (vr => vr.Item3),
          validationRules.Select (vr => vr.Item4));
    }

    private ValidationRulesResult SetValidationRulesForProperty (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        Type validatedType)
    {
      var key = (ValidatedType: validatedType, PropertyType: propertyRuleReflector.ValidatedProperty.PropertyType);
      var cachedDelegate = s_cachedDelegatesForSetValidationRulesForProperty.GetOrAdd (key, ValueFactory);
      return cachedDelegate (propertyRuleReflector);

      Func<IAttributesBasedValidationPropertyRuleReflector, ValidationRulesResult> ValueFactory ((Type ValidatedType, Type PropertyType) @params)
      {
        var methodInfo = MemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => SetValidationRulesForProperty<object, object> (null));
        var closedGenericMethodInfo = methodInfo.MakeGenericMethod (@params.ValidatedType, @params.PropertyType);
        return (Func<IAttributesBasedValidationPropertyRuleReflector, ValidationRulesResult>) closedGenericMethodInfo.CreateDelegate (
            typeof (Func<IAttributesBasedValidationPropertyRuleReflector, ValidationRulesResult>));
      }
    }

    private static ValidationRulesResult SetValidationRulesForProperty<TValidatedType, TProperty> (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector)
    {
      var validatedType = typeof (TValidatedType);
      var property = propertyRuleReflector.ValidatedProperty;
      var propertyFunc = propertyRuleReflector.GetValidatedPropertyFunc (validatedType); // TODO RM-5906: change to return Func<TValidatedType, TProperty>
      var collectorType = typeof (AttributeBasedValidationRuleCollector);

      var addingPropertyRule = GetAddingPropertyRuleForRemovableValidators<TValidatedType, TProperty> (propertyRuleReflector, property, propertyFunc, collectorType);
      var addingHardConstraintPropertyRule = GetAddingPropertyRuleForNonRemovableValidators<TValidatedType, TProperty> (
          propertyRuleReflector,
          property,
          propertyFunc,
          collectorType);
      var addingMetaValidationPropertyRule = GetAddingComponentPropertyMetaValidationRule (propertyRuleReflector, property, collectorType);
      var removingPropertyRule = GetRemovingPropertyValidationRule (propertyRuleReflector, property, collectorType);

      return Tuple.Create (
          addingPropertyRule,
          addingHardConstraintPropertyRule,
          addingMetaValidationPropertyRule,
          removingPropertyRule);
    }

    private static IAddingPropertyValidationRuleCollector GetAddingPropertyRuleForRemovableValidators<TValidatedType, TProperty> (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        IPropertyInformation property,
        Func<object, object> propertyFunc,
        Type collectorType)
    {
      IAddingPropertyValidationRuleCollector addingPropertyValidationRuleCollector = new AddingPropertyValidationRuleCollector<TValidatedType, TProperty> (property, propertyFunc, collectorType);

      addingPropertyValidationRuleCollector.SetRemovable();
      foreach (var validator in propertyRuleReflector.GetRemovablePropertyValidators())
        addingPropertyValidationRuleCollector.RegisterValidator (_ => validator);

      return addingPropertyValidationRuleCollector;
    }

    private static IAddingPropertyValidationRuleCollector GetAddingPropertyRuleForNonRemovableValidators<TValidatedType, TProperty> (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        IPropertyInformation property,
        Func<object, object> propertyFunc,
        Type collectorType)
    {
      IAddingPropertyValidationRuleCollector addingPropertyValidationRuleCollector = new AddingPropertyValidationRuleCollector<TValidatedType, TProperty> (property, propertyFunc, collectorType);

      foreach (var validator in propertyRuleReflector.GetNonRemovablePropertyValidators ())
        addingPropertyValidationRuleCollector.RegisterValidator (_ => validator);

      return addingPropertyValidationRuleCollector;
    }

    private static RemovingPropertyValidationRuleCollector GetRemovingPropertyValidationRule (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        IPropertyInformation property,
        Type collectorType)
    {
      var propertyRule = new RemovingPropertyValidationRuleCollector (property, collectorType);
      
      foreach (var removingValidatorRegistration in propertyRuleReflector.GetRemovingValidatorRegistrations())
        propertyRule.RegisterValidator (removingValidatorRegistration.ValidatorType, removingValidatorRegistration.CollectorTypeToRemoveFrom, null);
      
      return propertyRule;
    }

    private static PropertyMetaValidationRuleCollector GetAddingComponentPropertyMetaValidationRule (
        IAttributesBasedValidationPropertyRuleReflector propertyRuleReflector,
        IPropertyInformation propertyInfo,
        Type collectorType)
    {
      var propertyRule = new PropertyMetaValidationRuleCollector (propertyInfo, collectorType);
      
      foreach (var metaValidationRule in propertyRuleReflector.GetMetaValidationRules())
        propertyRule.RegisterMetaValidationRule (metaValidationRule);
      
      return propertyRule;
    }
  }
}