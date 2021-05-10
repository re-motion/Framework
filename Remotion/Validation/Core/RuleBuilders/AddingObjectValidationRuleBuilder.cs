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
  /// Default implementation of the <see cref="IAddingObjectValidationRuleBuilder{TValidatedType}"/>.
  /// </summary>
  public class AddingObjectValidationRuleBuilder<TValidatedType> : IConditionalAddingObjectValidationRuleBuilder<TValidatedType>
  {
    private readonly IAddingObjectValidationRuleCollector _addingObjectValidationRuleCollector;
    private readonly IObjectMetaValidationRuleCollector _objectMetaValidationRuleCollector;

    public AddingObjectValidationRuleBuilder (
        IAddingObjectValidationRuleCollector addingObjectValidationRuleCollector,
        IObjectMetaValidationRuleCollector objectMetaValidationRuleCollector)
    {
      ArgumentUtility.CheckNotNull ("addingObjectValidationRuleCollector", addingObjectValidationRuleCollector);
      ArgumentUtility.CheckNotNull ("objectMetaValidationRuleCollector", objectMetaValidationRuleCollector);

      _addingObjectValidationRuleCollector = addingObjectValidationRuleCollector;
      _objectMetaValidationRuleCollector = objectMetaValidationRuleCollector;
    }

    public IAddingObjectValidationRuleCollector AddingObjectValidationRuleCollector
    {
      get { return _addingObjectValidationRuleCollector; }
    }

    public IObjectMetaValidationRuleCollector ObjectMetaValidationRuleCollector
    {
      get { return _objectMetaValidationRuleCollector; }
    }

    public IAddingObjectValidationRuleBuilder<TValidatedType> SetCondition (Func<TValidatedType, bool> predicate)
    {
      ArgumentUtility.CheckNotNull ("predicate", predicate);

      _addingObjectValidationRuleCollector.SetCondition (predicate);
      return this;
    }

    public IAddingObjectValidationRuleBuilder<TValidatedType> CanBeRemoved ()
    {
      _addingObjectValidationRuleCollector.SetRemovable();
      return this;
    }

    public IAddingObjectValidationRuleBuilder<TValidatedType> AddMetaValidationRule (IObjectMetaValidationRule metaValidationRule)
    {
      ArgumentUtility.CheckNotNull ("metaValidationRule", metaValidationRule);

      _objectMetaValidationRuleCollector.RegisterMetaValidationRule (metaValidationRule);
      return this;
    }

    public IAddingObjectValidationRuleBuilder<TValidatedType> AddMetaValidationRule (
        Func<IEnumerable<IObjectValidator>, MetaValidationRuleValidationResult> rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);

      var metaValidationRule = new DelegateObjectMetaValidationRule<IObjectValidator> (rule);
      _objectMetaValidationRuleCollector.RegisterMetaValidationRule (metaValidationRule);
      return this;
    }

    public IAddingObjectValidationRuleBuilder<TValidatedType> AddMetaValidationRule<TValidator> (
        Expression<Func<IEnumerable<TValidator>, bool>> metaValidationRuleExpression)
        where TValidator: IObjectValidator
    {
      ArgumentUtility.CheckNotNull ("metaValidationRuleExpression", metaValidationRuleExpression);

      var metaValidationRuleExecutor = metaValidationRuleExpression.Compile ();

      var metaValidationRule = new DelegateObjectMetaValidationRule<TValidator> (
          validationRules =>
          {
            var isValid = metaValidationRuleExecutor (validationRules);
            if (isValid)
              return MetaValidationRuleValidationResult.CreateValidResult ();

            return MetaValidationRuleValidationResult.CreateInvalidResult (
                "Meta validation rule '{0}' failed for validator '{1}' on type '{2}'.",
                metaValidationRuleExpression,
                typeof (TValidator).GetFullNameSafe(),
                _addingObjectValidationRuleCollector.ValidatedType.GetFullNameSafe());
          });

      _objectMetaValidationRuleCollector.RegisterMetaValidationRule (metaValidationRule);
      return this;
    }

    public IAddingObjectValidationRuleBuilder<TValidatedType> SetValidator (
        Func<ObjectValidationRuleInitializationParameters, IObjectValidator> validatorFactory)
    {
      ArgumentUtility.CheckNotNull ("validatorFactory", validatorFactory);

      _addingObjectValidationRuleCollector.RegisterValidator (validatorFactory);
      return this;
    }
  }
}