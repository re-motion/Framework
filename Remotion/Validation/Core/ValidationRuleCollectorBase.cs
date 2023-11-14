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
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.RuleCollectors;

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
    private readonly List<IAddingObjectValidationRuleCollector> _addedObjectRules;
    private readonly List<IObjectMetaValidationRuleCollector> _objectMetaValidationRules;
    private readonly List<IRemovingObjectValidationRuleCollector> _removedObjectRules;

    protected ValidationRuleCollectorBase ()
    {
      _addedPropertyRules = new List<IAddingPropertyValidationRuleCollector>();
      _propertyMetaValidationRules = new List<IPropertyMetaValidationRuleCollector>();
      _removedPropertyRules = new List<IRemovingPropertyValidationRuleCollector>();
      _addedObjectRules = new List<IAddingObjectValidationRuleCollector>();
      _objectMetaValidationRules = new List<IObjectMetaValidationRuleCollector>();
      _removedObjectRules = new List<IRemovingObjectValidationRuleCollector>();
    }

    public Type ValidatedType => typeof(TValidatedType);

    /// <inheritdoc />
    public IReadOnlyCollection<IAddingPropertyValidationRuleCollector> AddedPropertyRules => _addedPropertyRules.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyCollection<IPropertyMetaValidationRuleCollector> PropertyMetaValidationRules => _propertyMetaValidationRules.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyCollection<IRemovingPropertyValidationRuleCollector> RemovedPropertyRules => _removedPropertyRules.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyCollection<IAddingObjectValidationRuleCollector> AddedObjectRules => _addedObjectRules.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyCollection<IObjectMetaValidationRuleCollector> ObjectMetaValidationRules => _objectMetaValidationRules.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyCollection<IRemovingObjectValidationRuleCollector> RemovedObjectRules => _removedObjectRules.AsReadOnly();

    /// <summary>
    /// Registers a new validation rule for a property. 
    /// </summary>
    /// <typeparam name="TProperty">The <see cref="Type"/> of the validated property (used only for syntactical sugar).</typeparam>
    /// <param name="propertySelector">Specifies the property for which the validation rule is added.</param>
    /// <returns>A builder object used for specifying the validation rules of the property.</returns>
    /// <remarks>TODO RM-5906: usage sample</remarks>
    public IConditionalAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> AddRule<TProperty> (
        Expression<Func<TValidatedType, TProperty>> propertySelector)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);

      var propertyRule = AddingPropertyValidationRuleCollector.Create(propertySelector, GetType());
      _addedPropertyRules.Add(propertyRule);

      var metaValidationPropertyRule = PropertyMetaValidationRuleCollector.Create(propertySelector, GetType());
      _propertyMetaValidationRules.Add(metaValidationPropertyRule);

      return new AddingPropertyValidationRuleBuilder<TValidatedType, TProperty>(propertyRule, metaValidationPropertyRule);
    }

    /// <summary>
    /// Registers a new validation rule for a property with a custom property value lookup.
    /// </summary>
    /// <typeparam name="TProperty">The <see cref="Type"/> of the validated property (used only for syntactical sugar).</typeparam>
    /// <param name="propertyInfo">Specifies the property for which the validation rule is added.</param>
    /// <param name="propertyGetter">Specifies a custom getter for the <paramref name="propertyInfo"/>.</param>
    /// <returns>A builder object used for specifying the validation rules of the property.</returns>
    /// <remarks>TODO RM-5906: usage sample</remarks>
    public IConditionalAddingPropertyValidationRuleBuilder<TValidatedType, TProperty> AddRule<TProperty> (
        IPropertyInformation propertyInfo,
        Func<object, object> propertyGetter)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull("propertyGetter", propertyGetter);

      var collectorType = GetType();

      var propertyRule = new AddingPropertyValidationRuleCollector<TValidatedType, TProperty>(propertyInfo, propertyGetter, collectorType);
      _addedPropertyRules.Add(propertyRule);

      var metaValidationPropertyRule = new PropertyMetaValidationRuleCollector(propertyInfo, collectorType);
      _propertyMetaValidationRules.Add(metaValidationPropertyRule);

      return new AddingPropertyValidationRuleBuilder<TValidatedType, TProperty>(propertyRule, metaValidationPropertyRule);
    }

    /// <summary>
    /// Registers which validation rules should be removed from the property. This is used to remove validation rules introduced by other validation 
    /// collectors of <typeparamref name="TValidatedType"/>.
    /// </summary>
    /// <typeparam name="TProperty">The <see cref="Type"/> of the validated property (used only for syntactical sugar).</typeparam>
    /// <param name="propertySelector">Specifies the property for which a specific validation rule should be removed.</param>
    /// <returns>A builder object used for specifying the validation rules to be removed from the property.</returns>
    /// <remarks>TODO RM-5906: usage sample</remarks>
    public IRemovingPropertyValidationRuleBuilder<TValidatedType, TProperty> RemoveRule<TProperty> (
        Expression<Func<TValidatedType, TProperty>> propertySelector)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);

      var propertyRule = RemovingPropertyValidationRuleCollector.Create(propertySelector, GetType());
      _removedPropertyRules.Add(propertyRule);

      return new RemovingPropertyValidationRuleBuilder<TValidatedType, TProperty>(propertyRule);
    }

    /// <summary>
    /// Registers which validation rules should be removed from the property. This is used to remove validation rules introduced by other validation
    /// collectors of <typeparamref name="TValidatedType"/>.
    /// </summary>
    /// <typeparam name="TProperty">The <see cref="Type"/> of the validated property (used only for syntactical sugar).</typeparam>
    /// <param name="propertyInformation">Specifies the property for which a specific validation rule should be removed.</param>
    /// <returns>A builder object used for specifying the validation rules to be removed from the property.</returns>
    /// <remarks>TODO RM-5906: usage sample</remarks>
    public IRemovingPropertyValidationRuleBuilder<TValidatedType, TProperty> RemoveRule<TProperty> (
        IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull("propertyInformation", propertyInformation);

      var propertyRule = new RemovingPropertyValidationRuleCollector(propertyInformation, GetType());
      _removedPropertyRules.Add(propertyRule);

      return new RemovingPropertyValidationRuleBuilder<TValidatedType, TProperty>(propertyRule);
    }

    /// <summary>
    /// Registers a new validation rule for an object of type <typeparamref name="TValidatedType"/>.
    /// </summary>
    /// <returns>A builder object used for specifying the validation rules of the type <typeparamref name="TValidatedType"/>.</returns>
    /// <remarks>TODO RM-5906: usage sample</remarks>
    public IConditionalAddingObjectValidationRuleBuilder<TValidatedType> AddRule ()
    {
      var objectRule = AddingObjectValidationRuleCollector.Create<TValidatedType>(GetType());
      _addedObjectRules.Add(objectRule);

      var metaValidationRule = ObjectMetaValidationRuleCollector.Create<TValidatedType>(GetType());
      _objectMetaValidationRules.Add(metaValidationRule);

      return new AddingObjectValidationRuleBuilder<TValidatedType>(objectRule, metaValidationRule);
    }

    /// <summary>
    /// Registers which validation rules should be removed from the type <typeparamref name="TValidatedType"/>.
    /// This is used to remove validation rules introduced by other validation collectors of <typeparamref name="TValidatedType"/>.
    /// </summary>
    /// <returns>A builder object used for specifying the validation rules to be removed from the type <typeparamref name="TValidatedType"/>.</returns>
    /// <remarks>TODO RM-5906: usage sample</remarks>
    public IRemovingObjectValidationRuleBuilder<TValidatedType> RemoveRule ()
    {
      var objectRule = RemovingObjectValidationRuleCollector.Create<TValidatedType>(GetType());
      _removedObjectRules.Add(objectRule);

      return new RemovingObjectValidationRuleBuilder<TValidatedType>(objectRule);
    }
  }
}
