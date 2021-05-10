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
using System.Linq.Expressions;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation.RuleBuilders
{
  /// <summary>
  /// Default implementation of the <see cref="IAddingPropertyValidationRuleBuilder{TValidatedType,TProperty}"/>.
  /// </summary>
  public class AddingPropertyValidationRuleBuilder<TValidatedType, TProperty> : IConditionalAddingPropertyValidationRuleBuilder<TValidatedType, TProperty>
  {
    private readonly IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollector;
    private readonly IPropertyMetaValidationRuleCollector _propertyMetaValidationRuleCollector;

    public AddingPropertyValidationRuleBuilder (
        IAddingPropertyValidationRuleCollector addingPropertyValidationRuleCollector,
        IPropertyMetaValidationRuleCollector propertyMetaValidationRuleCollector)
    {
      ArgumentUtility.CheckNotNull ("addingPropertyValidationRuleCollector", addingPropertyValidationRuleCollector);
      ArgumentUtility.CheckNotNull ("propertyMetaValidationRuleCollector", propertyMetaValidationRuleCollector);

      _addingPropertyValidationRuleCollector = addingPropertyValidationRuleCollector;
      _propertyMetaValidationRuleCollector = propertyMetaValidationRuleCollector;
    }

    public IAddingPropertyValidationRuleCollector AddingPropertyValidationRuleCollector
    {
      get { return _addingPropertyValidationRuleCollector; }
    }

    public IPropertyMetaValidationRuleCollector PropertyMetaValidationRuleCollector
    {
      get { return _propertyMetaValidationRuleCollector; }
    }

    public IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> SetCondition (Func<TValidatedType, bool> predicate)
    {
      ArgumentUtility.CheckNotNull ("predicate", predicate);

      _addingPropertyValidationRuleCollector.SetCondition (predicate);
      return this;
    }

    public IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> CanBeRemoved ()
    {
      _addingPropertyValidationRuleCollector.SetRemovable();
      return this;
    }

    public IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> AddMetaValidationRule (IPropertyMetaValidationRule propertyMetaValidationRule)
    {
      ArgumentUtility.CheckNotNull ("propertyMetaValidationRule", propertyMetaValidationRule);

      _propertyMetaValidationRuleCollector.RegisterMetaValidationRule (propertyMetaValidationRule);
      return this;
    }

    public IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> AddMetaValidationRule (
        Func<IEnumerable<IPropertyValidator>, MetaValidationRuleValidationResult> rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);

      var metaValidationRule = new DelegatePropertyMetaValidationRule<IPropertyValidator> (rule);
      _propertyMetaValidationRuleCollector.RegisterMetaValidationRule (metaValidationRule);
      return this;
    }

    public IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> AddMetaValidationRule<TValidator> (
        Expression<Func<IEnumerable<TValidator>, bool>> metaValidationRuleExpression)
        where TValidator: IPropertyValidator
    {
      ArgumentUtility.CheckNotNull ("metaValidationRuleExpression", metaValidationRuleExpression);

      var metaValidationRuleExecutor = metaValidationRuleExpression.Compile();

      var metaValidationRule = new DelegatePropertyMetaValidationRule<TValidator> (
          validationRules =>
          {
            var isValid = metaValidationRuleExecutor (validationRules);
            if (isValid)
              return MetaValidationRuleValidationResult.CreateValidResult();

            return MetaValidationRuleValidationResult.CreateInvalidResult (
                "Meta validation rule '{0}' failed for validator '{1}' on property '{2}.{3}'.",
                metaValidationRuleExpression,
                typeof (TValidator).GetFullNameSafe(),
                _addingPropertyValidationRuleCollector.Property.DeclaringType.GetFullNameSafe(),
                _addingPropertyValidationRuleCollector.Property.Name);
          });

      _propertyMetaValidationRuleCollector.RegisterMetaValidationRule (metaValidationRule);
      return this;
    }

    public IAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> SetValidator (
        Func<PropertyValidationRuleInitializationParameters, IPropertyValidator> validatorFactory)
    {
      ArgumentUtility.CheckNotNull ("validatorFactory", validatorFactory);

      _addingPropertyValidationRuleCollector.RegisterValidator (validatorFactory);
      return this;
    }
  }
}