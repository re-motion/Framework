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
  public abstract class ValidationRuleCollectorBase<TValidatedType> : IValidationRuleCollector<TValidatedType>
  {
    private readonly List<IAddingPropertyValidationRuleCollector> _addedPropertyRules;
    private readonly List<IPropertyMetaValidationRuleCollector> _propertyMetaValidationRules;
    private readonly List<IRemovingPropertyValidationRuleCollector> _removedPropertyRules;

    protected ValidationRuleCollectorBase ()
    {
      _addedPropertyRules = new List<IAddingPropertyValidationRuleCollector>();
      _propertyMetaValidationRules = new List<IPropertyMetaValidationRuleCollector>();
      _removedPropertyRules = new List<IRemovingPropertyValidationRuleCollector>();
    }

    public Type ValidatedType
    {
      get { return typeof (TValidatedType); }
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IAddingPropertyValidationRuleCollector> AddedPropertyRules
    {
      get { return _addedPropertyRules.AsReadOnly(); }
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IPropertyMetaValidationRuleCollector> PropertyMetaValidationRules
    {
      get { return _propertyMetaValidationRules.AsReadOnly(); }
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IRemovingPropertyValidationRuleCollector> RemovedPropertyRules
    {
      get { return _removedPropertyRules.AsReadOnly(); }
    }

    /// <inheritdoc />
    public IConditionalAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> AddRule<TProperty> (
        Expression<Func<TValidatedType, TProperty>> propertySelector)
    {
      ArgumentUtility.CheckNotNull ("propertySelector", propertySelector);
      
      var propertyRule = AddingPropertyValidationRuleCollector.Create (propertySelector, GetType());
      _addedPropertyRules.Add (propertyRule);

      var metaValidationPropertyRule = PropertyMetaValidationRuleCollector.Create (propertySelector, GetType());
      _propertyMetaValidationRules.Add (metaValidationPropertyRule);

      return new AddingPropertyValidationRuleBuilder<TValidatedType, TProperty> (propertyRule, metaValidationPropertyRule);
    }

    /// <inheritdoc />
    public IRemovingPropertyValidationRuleBuilder<TValidatedType, TProperty> RemoveRule<TProperty> (
        Expression<Func<TValidatedType, TProperty>> propertySelector)
    {
      ArgumentUtility.CheckNotNull ("propertySelector", propertySelector);
      
      var propertyRule = RemovingPropertyValidationRuleCollector.Create (propertySelector, GetType());
      _removedPropertyRules.Add (propertyRule);

      return new RemovingPropertyValidationRuleBuilder<TValidatedType, TProperty> (propertyRule);
    }
  }
}