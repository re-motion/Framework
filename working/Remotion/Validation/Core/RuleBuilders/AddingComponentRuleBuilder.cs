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
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Remotion.Utilities;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Rules;

namespace Remotion.Validation.RuleBuilders
{
  /// <summary>
  /// Default implementation of the <see cref="IAddingComponentRuleBuilder{TValidatedType,TProperty}"/>.
  /// </summary>
  public class AddingComponentRuleBuilder<TValidatedType, TProperty> : IAddingComponentRuleBuilderOptions<TValidatedType, TProperty>
  {
    private readonly IAddingComponentPropertyRule _addingComponentPropertyRule;
    private readonly IAddingComponentPropertyMetaValidationRule _addingMetaValidationPropertyRule;

    public AddingComponentRuleBuilder (
        IAddingComponentPropertyRule addingComponentPropertyPropertyRule,
        IAddingComponentPropertyMetaValidationRule addingMetaValidationPropertyRule)
    {
      ArgumentUtility.CheckNotNull ("addingComponentPropertyPropertyRule", addingComponentPropertyPropertyRule);
      ArgumentUtility.CheckNotNull ("addingMetaValidationPropertyRule", addingMetaValidationPropertyRule);

      _addingComponentPropertyRule = addingComponentPropertyPropertyRule;
      _addingMetaValidationPropertyRule = addingMetaValidationPropertyRule;
    }

    public IAddingComponentPropertyRule AddingComponentPropertyRule
    {
      get { return _addingComponentPropertyRule; }
    }

    public IAddingComponentPropertyMetaValidationRule AddingMetaValidationPropertyRule
    {
      get { return _addingMetaValidationPropertyRule; }
    }

    public IRuleBuilderOptions<TValidatedType, TProperty> NotRemovable ()
    {
      _addingComponentPropertyRule.SetHardConstraint();
      return this;
    }

    public IRuleBuilderOptions<TValidatedType, TProperty> AddMetaValidationRule (IMetaValidationRule metaValidationRule)
    {
      ArgumentUtility.CheckNotNull ("metaValidationRule", metaValidationRule);

      _addingMetaValidationPropertyRule.RegisterMetaValidationRule (metaValidationRule);
      return this;
    }

    public IRuleBuilderOptions<TValidatedType, TProperty> AddMetaValidationRule (
        Func<IEnumerable<IPropertyValidator>, MetaValidationRuleValidationResult> rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);

      var metaValidationRule = new DelegateMetaValidationRule<IPropertyValidator> (rule);
      _addingMetaValidationPropertyRule.RegisterMetaValidationRule (metaValidationRule);
      return this;
    }

    public IRuleBuilderOptions<TValidatedType, TProperty> AddMetaValidationRule<TValidator> (
        Expression<Func<IEnumerable<TValidator>, bool>> metaValidationRuleExpression)
        where TValidator: IPropertyValidator
    {
      ArgumentUtility.CheckNotNull ("metaValidationRuleExpression", metaValidationRuleExpression);

      var metaValidationRuleExecutor = metaValidationRuleExpression.Compile();

      var metaValidationRule = new DelegateMetaValidationRule<TValidator> (
          validationRules =>
          {
            var isValid = metaValidationRuleExecutor (validationRules);
            if (isValid)
              return MetaValidationRuleValidationResult.CreateValidResult();

            return MetaValidationRuleValidationResult.CreateInvalidResult (
                "Meta validation rule '{0}' failed for validator '{1}' on property '{2}.{3}'.",
                metaValidationRuleExpression,
                typeof (TValidator).FullName,
                _addingComponentPropertyRule.Property.DeclaringType.FullName,
                _addingComponentPropertyRule.Property.Name);
          });

      _addingMetaValidationPropertyRule.RegisterMetaValidationRule (metaValidationRule);
      return this;
    }

    public IRuleBuilderOptions<TValidatedType, TProperty> SetValidator (IPropertyValidator validator)
    {
      ArgumentUtility.CheckNotNull ("validator", validator);

      AddValidator (validator);
      return this;
    }

    IRuleBuilderOptions<TValidatedType, TProperty> IRuleBuilder<TValidatedType, TProperty>.SetValidator (IValidator<TProperty> validator)
        //called from FluentValidation ExtensionMethod
    {
      AddValidator (new ChildValidatorAdaptor (validator));
      return this;
    }

    IRuleBuilderOptions<TValidatedType, TProperty> IConfigurable<PropertyRule, IRuleBuilderOptions<TValidatedType, TProperty>>.Configure (Action<PropertyRule> configurator)
    {
      ArgumentUtility.CheckNotNull ("configurator", configurator);

      configurator ((PropertyRule) _addingComponentPropertyRule);
      return this;
    }

    private void AddValidator (IPropertyValidator validator)
    {
      _addingComponentPropertyRule.RegisterValidator (validator);
    }

    IRuleBuilderOptions<TValidatedType, TProperty> IRuleBuilder<TValidatedType, TProperty>.SetValidator (IValidator validator)
    {
      throw new NotSupportedException (
          "This overload of SetValidator is no longer used. If you are trying to set a child validator for a collection, use SetCollectionValidator instead.");
    }
  }
}