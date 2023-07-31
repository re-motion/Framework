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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Utilities.ReSharperAnnotations;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.MetaValidation.Rules.Custom;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Validators;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Create <see cref="IPropertyValidator"/>s based on the <see cref="IDomainModelConstraintProvider.GetMaxLength"/> and the <see cref="IDomainModelConstraintProvider.IsNullable"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class DomainObjectAttributesBasedValidationPropertyRuleReflector : IAttributesBasedValidationPropertyRuleReflector
  {
    private class FakeDomainObject
    {
      public static readonly FakeDomainObject SingleValue = new FakeDomainObject();
      public static readonly IEnumerable<FakeDomainObject> CollectionValue = new List<FakeDomainObject> { SingleValue }.AsReadOnly();
    }

    private readonly PropertyInfo _interfaceProperty;
    private readonly PropertyInfo _implementationProperty;
    private readonly IDomainModelConstraintProvider _domainModelConstraintProvider;
    private readonly IPropertyInformation _implementationPropertyInformation;
    private readonly IValidationMessageFactory _validationMessageFactory;
    private readonly PropertyInfoAdapter _interfacePropertyInformation;

    public DomainObjectAttributesBasedValidationPropertyRuleReflector (
        PropertyInfo interfaceProperty,
        PropertyInfo implementationProperty,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IValidationMessageFactory validationMessageFactory)
    {
      ArgumentUtility.CheckNotNull("interfaceProperty", interfaceProperty);
      ArgumentUtility.CheckNotNull("implementationProperty", implementationProperty);
      ArgumentUtility.CheckNotNull("domainModelConstraintProvider", domainModelConstraintProvider);
      ArgumentUtility.CheckNotNull("validationMessageFactory", validationMessageFactory);

      var interfacePropertyInformation = PropertyInfoAdapter.Create(interfaceProperty);
      var implementationPropertyInformation = PropertyInfoAdapter.Create(implementationProperty);
      var isMixinProperty = Mixins.Utilities.ReflectionUtility.IsMixinType(implementationProperty.DeclaringType!);

      if (isMixinProperty && !interfacePropertyInformation.DeclaringType!.IsInterface)
      {
        throw new ArgumentException(
            string.Format(
                "The property '{0}' was declared on type '{1}' but only interface declarations are supported when using mixin properties.",
                interfacePropertyInformation.Name,
                interfacePropertyInformation.DeclaringType.GetFullNameSafe()),
            "interfaceProperty");
      }

      if (!implementationPropertyInformation.IsOriginalDeclaration())
      {
        throw new ArgumentException(
            string.Format(
                "The property '{0}' was used from the overridden declaration on type '{1}' but only original declarations are supported.",
                implementationPropertyInformation.Name,
                implementationPropertyInformation.DeclaringType!.GetFullNameSafe()),
            "implementationProperty");
      }

      // TODO RM-5906: Replace with IPropertyInformation and propagate to call and callee-site
      _interfaceProperty = interfaceProperty;
      // TODO RM-5906: Replace with IPropertyInformation and propagate to call and callee-site
      _implementationProperty = implementationProperty;
      _domainModelConstraintProvider = domainModelConstraintProvider;
      _validationMessageFactory = validationMessageFactory;
      _interfacePropertyInformation = interfacePropertyInformation;
      _implementationPropertyInformation = implementationPropertyInformation;
    }

    public IPropertyInformation ImplementationPropertyInformation => _implementationPropertyInformation;

    public IPropertyInformation InterfacePropertyInformation => _interfacePropertyInformation;

    IPropertyInformation IAttributesBasedValidationPropertyRuleReflector.ValidatedProperty => _interfacePropertyInformation;

    public Func<object, object> GetValidatedPropertyFunc (Type validatedType)
    {
      ArgumentUtility.CheckNotNull("validatedType", validatedType);

      // TODO RM-5906: Add cache, try to unify with ValidationAttributesBasedPropertyRuleReflector and AddingComponentPropertyRule

      var parameterExpression = Expression.Parameter(typeof(object), "t");

      // object o => UsePersistentProperty ((DomainObject)o, _interfaceProperty) ? (object) (TheType o).TheProperty : nonEmptyObject;

      var usePersistentPropertyMethod = MemberInfoFromExpressionUtility.GetMethod(() => UsePersistentProperty(null!, null!));
      var conditionExpression = Expression.Call(
          usePersistentPropertyMethod,
          Expression.Convert(parameterExpression, typeof(DomainObject)),
          Expression.Constant(_implementationProperty, typeof(PropertyInfo)));

      // object o => (object) (TheType o).TheProperty
      var domainObjectPropertyAccessExpression = Expression.Convert(
          Expression.MakeMemberAccess(
              Expression.Convert(parameterExpression, validatedType),
              _interfaceProperty),
          typeof(object));


      object nonEmptyDummyValue;
      if (ReflectionUtility.IsObjectList(_implementationProperty.PropertyType))
        nonEmptyDummyValue = FakeDomainObject.CollectionValue;
      else if (ReflectionUtility.IsIObjectList(_implementationProperty.PropertyType))
        nonEmptyDummyValue = FakeDomainObject.CollectionValue;
      else
        nonEmptyDummyValue = FakeDomainObject.SingleValue;
      var nonEmptyDummyValueExpression = Expression.Constant(nonEmptyDummyValue, typeof(object));

      var accessorExpression = Expression.Lambda<Func<object, object>>(
          Expression.Condition(conditionExpression, domainObjectPropertyAccessExpression, nonEmptyDummyValueExpression),
          parameterExpression);

      return accessorExpression.Compile();
    }

    [ReflectionAPI]
    private static bool UsePersistentProperty (DomainObject domainObject, PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("property", property);

      if (!ReflectionUtility.IsRelationType(property.PropertyType))
        return true;

      var dataManager = DataManagementService.GetDataManager(domainObject.DefaultTransactionContext.ClientTransaction);
      var endPointID = RelationEndPointID.Create(domainObject.ID, property.DeclaringType!, property.Name);
      var endPoint = dataManager.GetRelationEndPointWithLazyLoad(endPointID);
      return endPoint.IsDataComplete;
    }

    public IEnumerable<IPropertyValidator> GetRemovablePropertyValidators ()
    {
      if (!_domainModelConstraintProvider.IsNullable(_implementationPropertyInformation))
      {
        if (_implementationProperty.PropertyType == typeof(string))
        {
          yield return PropertyValidatorFactory.Create(
              _implementationPropertyInformation,
              parameters => new NotEmptyOrWhitespaceValidator(parameters.ValidationMessage),
              _validationMessageFactory);
        }
        else if (_implementationProperty.PropertyType == typeof(byte[]))
        {
          yield return PropertyValidatorFactory.Create(
              _implementationPropertyInformation,
              parameters => new NotEmptyBinaryValidator(parameters.ValidationMessage),
              _validationMessageFactory);
        }
      }
    }

    public IEnumerable<IPropertyValidator> GetNonRemovablePropertyValidators ()
    {
      var maxLength = _domainModelConstraintProvider.GetMaxLength(_implementationPropertyInformation);
      if (maxLength.HasValue)
      {
        yield return PropertyValidatorFactory.Create(
            _implementationPropertyInformation,
            parameters => new MaximumLengthValidator(maxLength.Value, parameters.ValidationMessage),
            _validationMessageFactory);
      }

      if (!_domainModelConstraintProvider.IsNullable(_implementationPropertyInformation))
      {
        yield return PropertyValidatorFactory.Create(
            _implementationPropertyInformation,
            parameters => new NotNullValidator(parameters.ValidationMessage),
            _validationMessageFactory);

        if (ReflectionUtility.IsObjectList(_implementationProperty.PropertyType)
            || ReflectionUtility.IsIObjectList(_implementationProperty.PropertyType))
        {
          yield return PropertyValidatorFactory.Create(
              _implementationPropertyInformation,
              parameters => new NotEmptyCollectionValidator(parameters.ValidationMessage),
              _validationMessageFactory);
        }
      }
    }

    public IEnumerable<RemovingValidatorRegistration> GetRemovingValidatorRegistrations ()
    {
      return Enumerable.Empty<RemovingValidatorRegistration>();
    }

    public IEnumerable<IPropertyMetaValidationRule> GetMetaValidationRules ()
    {
      return Enumerable.Empty<IPropertyMetaValidationRule>();
    }
  }
}
