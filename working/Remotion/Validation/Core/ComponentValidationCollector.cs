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
using System.Linq.Expressions;
using FluentValidation.Internal;
using Remotion.Utilities;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.Rules;

namespace Remotion.Validation
{
  /// <summary>
  /// Provides a base class for declaring the validation rules within a component.
  /// </summary>
  /// <remarks>TODO RM-5906: sample</remarks>
  /// <threadsafety static="true" instance="false" />
  public abstract class ComponentValidationCollector<TValidatedType> : IComponentValidationCollector<TValidatedType>
  {
    private readonly TrackingCollection<IAddingComponentPropertyRule> _addedPropertyRules;
    private readonly TrackingCollection<IAddingComponentPropertyMetaValidationRule> _addedPropertyMetaValidationRules;
    private readonly TrackingCollection<IRemovingComponentPropertyRule> _removedPropertyRules;

    protected ComponentValidationCollector ()
    {
      _addedPropertyRules = new TrackingCollection<IAddingComponentPropertyRule>();
      _addedPropertyMetaValidationRules = new TrackingCollection<IAddingComponentPropertyMetaValidationRule>();
      _removedPropertyRules = new TrackingCollection<IRemovingComponentPropertyRule>();
    }

    public Type ValidatedType
    {
      get { return typeof (TValidatedType); }
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IAddingComponentPropertyRule> AddedPropertyRules
    {
      get { return _addedPropertyRules.ToList().AsReadOnly(); }
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IAddingComponentPropertyMetaValidationRule> AddedPropertyMetaValidationRules
    {
      get { return _addedPropertyMetaValidationRules.ToList().AsReadOnly(); }
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IRemovingComponentPropertyRule> RemovedPropertyRules
    {
      get { return _removedPropertyRules.ToList().AsReadOnly(); }
    }

    /// <inheritdoc />
    public IAddingComponentRuleBuilderOptions<TValidatedType, TProperty> AddRule<TProperty> (
        Expression<Func<TValidatedType, TProperty>> propertySelector)
    {
      ArgumentUtility.CheckNotNull ("propertySelector", propertySelector);
      
      var componentPropertyRule = AddingComponentPropertyRule.Create (propertySelector, GetType());
      _addedPropertyRules.Add (componentPropertyRule);

      var metaValidationPropertyRule = AddingComponentPropertyMetaValidationRule.Create (propertySelector, GetType());
      _addedPropertyMetaValidationRules.Add (metaValidationPropertyRule);

      return new AddingComponentRuleBuilder<TValidatedType, TProperty> (componentPropertyRule, metaValidationPropertyRule);
    }

    /// <inheritdoc />
    public IRemovingComponentRuleBuilderOptions<TValidatedType, TProperty> RemoveRule<TProperty> (
        Expression<Func<TValidatedType, TProperty>> propertySelector)
    {
      ArgumentUtility.CheckNotNull ("propertySelector", propertySelector);
      
      var componentPropertyRule = RemovingComponentPropertyRule.Create (propertySelector, GetType());
      _removedPropertyRules.Add (componentPropertyRule);

      return new RemovingComponentRuleBuilder<TValidatedType, TProperty> (componentPropertyRule);
    }

    /// <inheritdoc />
    public void When (Func<TValidatedType, bool> predicate, Action action)
    {
      var addedPropertyRules = new List<IAddingComponentPropertyRule>();
      Action<IAddingComponentPropertyRule> onRuleAdded = addedPropertyRules.Add;
      Action<IRemovingComponentPropertyRule> onRuleRemoved =
          a => { throw new InvalidOperationException ("Conditions are not allowed for removing validation rules registrations."); };

      using (_addedPropertyRules.OnItemAdded (onRuleAdded))
      {
        using (_removedPropertyRules.OnItemAdded (onRuleRemoved))
        {
          action();
        }
      }

      // Must apply the predictae after the rule has been fully created to ensure any rules-specific conditions have already been applied.
      addedPropertyRules.ForEach (x => x.ApplyCondition (predicate.CoerceToNonGeneric()));
    }

    /// <inheritdoc />
    public void Unless (Func<TValidatedType, bool> predicate, Action action)
    {
      When (x => !predicate (x), action);
    }
   
  }
}