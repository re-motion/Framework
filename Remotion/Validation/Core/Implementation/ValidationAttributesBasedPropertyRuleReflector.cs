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
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Attributes.MetaValidation;
using Remotion.Validation.Attributes.Validation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Creates <see cref="IPropertyValidator"/>s based on attributes derived from <see cref="AddingValidationAttributeBase"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class ValidationAttributesBasedPropertyRuleReflector : IAttributesBasedValidationPropertyRuleReflector
  {
    private readonly PropertyInfo _propertyInfo;
    private readonly PropertyInfoAdapter _property;
    private readonly IValidationMessageFactory _validationMessageFactory;

    public ValidationAttributesBasedPropertyRuleReflector (PropertyInfo property, IValidationMessageFactory validationMessageFactory)
    {
      ArgumentUtility.CheckNotNull("property", property);
      ArgumentUtility.CheckNotNull("validationMessageFactory", validationMessageFactory);

      // TODO RM-5906: Replace with IPropertyInformation and propagate to call and callee-site
      _propertyInfo = property;
      _property = PropertyInfoAdapter.Create(property);
      _validationMessageFactory = validationMessageFactory;
    }

    public IPropertyInformation ValidatedProperty
    {
      get { return _property; }
    }

    public Func<object, object> GetValidatedPropertyFunc (Type validatedType)
    {
      ArgumentUtility.CheckNotNull("validatedType", validatedType);

      // TODO RM-5906: Replace with IPropertyInformation.GetGetMethod().GetFastInvoker.
      // TODO RM-5906: Add cache, try to unify with AddingComponentPropertyRule and DomainObjectAttributesBasedValidationPropertyRuleReflector
      var parameterExpression = Expression.Parameter(typeof(object), "t");

      // object o => (object) (TheType o).TheProperty
      var accessorExpression = Expression.Lambda<Func<object, object>>(
          Expression.Convert(
              Expression.MakeMemberAccess(
                  Expression.Convert(parameterExpression, validatedType),
                  _propertyInfo),
              typeof(object)),
          parameterExpression);

      return accessorExpression.Compile();
    }

    public IEnumerable<IPropertyValidator> GetRemovablePropertyValidators ()
    {
      var addingValidationAttributes = _property.GetCustomAttributes<AddingValidationAttributeBase>(false)
          .Where(a => a.IsRemovable);
      return addingValidationAttributes.SelectMany(a => a.GetPropertyValidators(_property, _validationMessageFactory));
    }

    public IEnumerable<IPropertyValidator> GetNonRemovablePropertyValidators ()
    {
      var addingValidationAttributes = _property.GetCustomAttributes<AddingValidationAttributeBase>(false)
          .Where(a => !a.IsRemovable);
      return addingValidationAttributes.SelectMany(a => a.GetPropertyValidators(_property, _validationMessageFactory));
    }

    public IEnumerable<RemovingValidatorRegistration> GetRemovingValidatorRegistrations ()
    {
      var removingValidationAttributes = _property.GetCustomAttributes<RemoveValidatorAttribute>(false);
      return removingValidationAttributes.Select(a => new RemovingValidatorRegistration(a.ValidatorType, a.CollectorTypeToRemoveFrom));
    }

    public IEnumerable<IPropertyMetaValidationRule> GetMetaValidationRules ()
    {
      var metaValidationAttributes = _property.GetCustomAttributes<AddingMetaValidationRuleAttributeBase>(false);
      return metaValidationAttributes.Select(mvr => mvr.GetMetaValidationRule(_propertyInfo));
    }
  }
}
