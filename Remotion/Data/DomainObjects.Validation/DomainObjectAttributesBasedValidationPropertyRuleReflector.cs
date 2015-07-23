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
using FluentValidation.Validators;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Mixins;
using Remotion.Utilities;
using Remotion.Utilities.ReSharperAnnotations;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.MetaValidation.Rules.Custom;
using Remotion.Validation.Rules;

namespace Remotion.Data.DomainObjects.Validation
{
  // TODO RM-5906: Refactor to ask the mapping directly: ClassDefintion.ResolveProperty() ?? ClassDefintion.ResolveRelationEndPoint()
  /// <summary>
  /// Create <see cref="IPropertyValidator"/>s based on the <see cref="ILengthConstrainedPropertyAttribute"/> and the <see cref="INullablePropertyAttribute"/>.
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

    public DomainObjectAttributesBasedValidationPropertyRuleReflector (PropertyInfo interfaceProperty, PropertyInfo implementationProperty)
    {
      ArgumentUtility.CheckNotNull ("interfaceProperty", interfaceProperty);
      ArgumentUtility.CheckNotNull ("implementationProperty", implementationProperty);
      if (Mixins.Utilities.ReflectionUtility.IsMixinType (implementationProperty.DeclaringType) && !interfaceProperty.DeclaringType.IsInterface)
      {
        throw new ArgumentException (
            string.Format (
                "The property '{0}' was declared on type '{1}' but only interface declarations are supported when using mixin properties.",
                interfaceProperty.Name,
                interfaceProperty.DeclaringType.Name),
            "interfaceProperty");
      }

      _interfaceProperty = interfaceProperty;
      _implementationProperty = implementationProperty;
    }

    public PropertyInfo ValidatedProperty
    {
      get { return _interfaceProperty; }
    }

    public Expression<Func<object, object>> GetPropertyAccessExpression (Type validatedType)
    {
      ArgumentUtility.CheckNotNull ("validatedType", validatedType);

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

      return Expression.Lambda<Func<object, object>> (
          Expression.Condition (conditionExpression, domainObjectPropertyAccessExpression, nonEmptyDummyValueExpression),
          parameterExpression);
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

    public IEnumerable<IPropertyValidator> GetAddingPropertyValidators ()
    {
      var lengthConstraintAttribute = AttributeUtility.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (_implementationProperty, false);
      if (lengthConstraintAttribute != null)
      {
        if (lengthConstraintAttribute.MaximumLength.HasValue)
          yield return new LengthValidator (0, lengthConstraintAttribute.MaximumLength.Value);
      }

      var nullableAttribute = AttributeUtility.GetCustomAttribute<INullablePropertyAttribute> (_implementationProperty, false);
      if (nullableAttribute != null && !nullableAttribute.IsNullable && typeof (IEnumerable).IsAssignableFrom (_implementationProperty.PropertyType)
          && !ReflectionUtility.IsObjectList (_implementationProperty.PropertyType))
      {
        yield return new NotEmptyValidator (GetDefaultValue (_implementationProperty.PropertyType));
      }
    }

    public IEnumerable<IPropertyValidator> GetHardConstraintPropertyValidators ()
    {
      var nullableAttribute = AttributeUtility.GetCustomAttribute<INullablePropertyAttribute> (_implementationProperty, false);
      if (nullableAttribute != null && !nullableAttribute.IsNullable)
      {
        yield return new NotNullValidator();
        if(ReflectionUtility.IsObjectList (_implementationProperty.PropertyType))
          yield return new NotEmptyValidator (GetDefaultValue (_implementationProperty.PropertyType));
      }
    }

    public IEnumerable<ValidatorRegistration> GetRemovingPropertyRegistrations ()
    {
      return Enumerable.Empty<ValidatorRegistration>();
    }

    public IEnumerable<IMetaValidationRule> GetMetaValidationRules ()
    {
      var lengthConstraintAttribute = AttributeUtility.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (_implementationProperty, false);
      if (lengthConstraintAttribute != null && lengthConstraintAttribute.MaximumLength.HasValue)
        yield return new RemotionMaxLengthMetaValidationRule (_implementationProperty, lengthConstraintAttribute.MaximumLength.Value);
    }

    private object GetDefaultValue (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      object output = null;

      if (type.IsValueType)
        output = Activator.CreateInstance (type);

      return output;
    }
  }
}