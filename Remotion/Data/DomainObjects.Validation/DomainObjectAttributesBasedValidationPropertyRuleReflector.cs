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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Utilities.ReSharperAnnotations;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.MetaValidation.Rules.Custom;
using Remotion.Validation.Rules;
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
      ArgumentUtility.CheckNotNull ("interfaceProperty", interfaceProperty);
      ArgumentUtility.CheckNotNull ("implementationProperty", implementationProperty);
      ArgumentUtility.CheckNotNull ("domainModelConstraintProvider", domainModelConstraintProvider);
      ArgumentUtility.CheckNotNull ("validationMessageFactory", validationMessageFactory);

      if (Mixins.Utilities.ReflectionUtility.IsMixinType (implementationProperty.DeclaringType) && !interfaceProperty.DeclaringType.IsInterface)
      {
        throw new ArgumentException (
            string.Format (
                "The property '{0}' was declared on type '{1}' but only interface declarations are supported when using mixin properties.",
                interfaceProperty.Name,
                interfaceProperty.DeclaringType.Name),
            "interfaceProperty");
      }

      // TODO RM-5906: Replace with IPropertyInformation and propagate to call and callee-site
      _interfaceProperty = interfaceProperty;
      // TODO RM-5906: Replace with IPropertyInformation and propagate to call and callee-site
      _implementationProperty = implementationProperty;
      _domainModelConstraintProvider = domainModelConstraintProvider;
      _validationMessageFactory = validationMessageFactory;
      _interfacePropertyInformation = PropertyInfoAdapter.Create (_interfaceProperty);
      _implementationPropertyInformation = PropertyInfoAdapter.Create (_implementationProperty);
    }

    public IPropertyInformation ValidatedProperty
    {
      get { return _interfacePropertyInformation; }
    }

    public Func<object, object> GetValidatedPropertyFunc (Type validatedType)
    {
      ArgumentUtility.CheckNotNull ("validatedType", validatedType);

      // TODO RM-5906: Add cache, try to unify with ValidationAttributesBasedPropertyRuleReflector and AddingComponentPropertyRule

      var parameterExpression = Expression.Parameter (typeof (object), "t");

      // object o => UsePersistentProperty ((DomainObject)o, _interfaceProperty) ? (object) (TheType o).TheProperty : nonEmptyObject;

      var usePersistentPropertyMethod = MemberInfoFromExpressionUtility.GetMethod (() => UsePersistentProperty (null, null));
      var conditionExpression = Expression.Call (
          usePersistentPropertyMethod,
          Expression.Convert (parameterExpression, typeof (DomainObject)),
          Expression.Constant (_implementationProperty, typeof (PropertyInfo)));

      // object o => (object) (TheType o).TheProperty
      var domainObjectPropertyAccessExpression = Expression.Convert (
          Expression.MakeMemberAccess (
              Expression.Convert (parameterExpression, validatedType),
              _interfaceProperty),
          typeof (object));


      var nonEmptyDummyValue = ReflectionUtility.IsObjectList (_implementationProperty.PropertyType)
          ? FakeDomainObject.CollectionValue
          : (object) FakeDomainObject.SingleValue;
      var nonEmptyDummyValueExpression = Expression.Constant (nonEmptyDummyValue, typeof (object));

      var accessorExpression = Expression.Lambda<Func<object, object>> (
          Expression.Condition (conditionExpression, domainObjectPropertyAccessExpression, nonEmptyDummyValueExpression),
          parameterExpression);

      return accessorExpression.Compile();
    }

    [ReflectionAPI]
    private static bool UsePersistentProperty (DomainObject domainObject, PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("property", property);

      if (!ReflectionUtility.IsRelationType (property.PropertyType))
        return true;

      var dataManager = DataManagementService.GetDataManager (domainObject.DefaultTransactionContext.ClientTransaction);
      var endPointID = RelationEndPointID.Create (domainObject.ID, property.DeclaringType, property.Name);
      var endPoint = dataManager.GetRelationEndPointWithLazyLoad (endPointID);
      return endPoint.IsDataComplete;
    }

    public IEnumerable<IPropertyValidator> GetRemovablePropertyValidators ()
    {
      var maxLength = _domainModelConstraintProvider.GetMaxLength (_implementationPropertyInformation);
      if (maxLength.HasValue)
      {
        var validationMessage = CreateValidationMessageForPropertyValidator (typeof (MaximumLengthValidator), _implementationPropertyInformation);
        yield return new MaximumLengthValidator (maxLength.Value, validationMessage);
      }

      if (!_domainModelConstraintProvider.IsNullable (_implementationPropertyInformation) 
          && typeof (IEnumerable).IsAssignableFrom (_implementationProperty.PropertyType)
          && !ReflectionUtility.IsObjectList (_implementationProperty.PropertyType))
      {
        var validationMessage = CreateValidationMessageForPropertyValidator (typeof (NotEmptyValidator),_implementationPropertyInformation);
        yield return new NotEmptyValidator (validationMessage);
      }
    }

    public IEnumerable<IPropertyValidator> GetNonRemovablePropertyValidators ()
    {
      if (!_domainModelConstraintProvider.IsNullable (_implementationPropertyInformation))
      {
        var notNullValidationMessage = CreateValidationMessageForPropertyValidator (typeof (NotNullValidator), _implementationPropertyInformation);
        yield return new NotNullValidator (notNullValidationMessage);

        if (ReflectionUtility.IsObjectList (_implementationProperty.PropertyType))
        {
          var notEmptyValidationMessage = CreateValidationMessageForPropertyValidator (typeof (NotEmptyValidator), _implementationPropertyInformation);
          yield return new NotEmptyValidator (notEmptyValidationMessage);
        }
      }
    }

    public IEnumerable<ValidatorRegistration> GetRemovingPropertyRegistrations ()
    {
      return Enumerable.Empty<ValidatorRegistration>();
    }

    public IEnumerable<IMetaValidationRule> GetMetaValidationRules ()
    {
      var maxLength = _domainModelConstraintProvider.GetMaxLength (_implementationPropertyInformation);
      if (maxLength.HasValue)
        yield return new RemotionMaxLengthMetaValidationRule (_implementationProperty, maxLength.Value);
    }

    [NotNull]
    private ValidationMessage CreateValidationMessageForPropertyValidator (Type validatorType, IPropertyInformation property)
    {
      var validationMessage = _validationMessageFactory.CreateValidationMessageForPropertyValidator (validatorType, property);
      if (validationMessage == null)
      {
        throw new InvalidOperationException (
            $"The {nameof (IValidationMessageFactory)} did not return a result for {validatorType.Name} applied to property '{property.Name}' on type '{property.GetOriginalDeclaringType().FullName}'.");
      }

      return validationMessage;
    }
  }
}