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
using System.Reflection;
using Moq;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Providers;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Providers
{
  public class TestableAttributeBasedValidationRuleCollectorProviderBase : AttributeBasedValidationRuleCollectorProviderBase
  {
    public static void SetupPropertyRuleReflector (
        Mock<IAttributesBasedValidationPropertyRuleReflector> validationPropertyRuleReflectorMock,
        Type validatedType,
        string propertyName,
        Func<object, object> validatedPropertyFunc,
        IPropertyValidator[] removablePropertyValidators = null,
        IPropertyValidator[] nonRemovablePropertyValidators = null,
        RemovingValidatorRegistration[] removingValidatorRegistrations = null,
        IPropertyMetaValidationRule[] propertyMetaValidationRules = null)
    {
      validationPropertyRuleReflectorMock
          .Setup(stub => stub.ValidatedProperty)
          .Returns(PropertyInfoAdapter.Create(validatedType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)));
      validationPropertyRuleReflectorMock
          .Setup(mock => mock.GetValidatedPropertyFunc(validatedType))
          .Returns(validatedPropertyFunc)
          .Verifiable();
      validationPropertyRuleReflectorMock
          .Setup(mock => mock.GetRemovablePropertyValidators())
          .Returns(removablePropertyValidators ?? Array.Empty<IPropertyValidator>())
          .Verifiable();
      validationPropertyRuleReflectorMock
          .Setup(mock => mock.GetNonRemovablePropertyValidators())
          .Returns(nonRemovablePropertyValidators ?? Array.Empty<IPropertyValidator>())
          .Verifiable();
      validationPropertyRuleReflectorMock
          .Setup(mock => mock.GetRemovingValidatorRegistrations())
          .Returns(removingValidatorRegistrations ?? Array.Empty<RemovingValidatorRegistration>())
          .Verifiable();
      validationPropertyRuleReflectorMock
          .Setup(mock => mock.GetMetaValidationRules())
          .Returns(propertyMetaValidationRules ?? Array.Empty<IPropertyMetaValidationRule>())
          .Verifiable();
    }

    private readonly IDictionary<(Type DeclaringType, string PropertyName), Mock<IAttributesBasedValidationPropertyRuleReflector>> _validationPropertyRuleReflectorMocks;

    public TestableAttributeBasedValidationRuleCollectorProviderBase (
        IDictionary<(Type DeclaringType, string PropertyName), Mock<IAttributesBasedValidationPropertyRuleReflector>> validationPropertyRuleReflectorMocks)
    {
      _validationPropertyRuleReflectorMocks = validationPropertyRuleReflectorMocks;
    }

    protected override ILookup<Type, IAttributesBasedValidationPropertyRuleReflector> CreatePropertyRuleReflectors (IEnumerable<Type> types)
    {
      return types.SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
          .Select(p => new { Type = p.DeclaringType, ValidationPropertyRuleReflector = GetValidationPropertyRuleReflector(p) })
          .Select(t => new Tuple<Type, IAttributesBasedValidationPropertyRuleReflector>(t.Type, t.ValidationPropertyRuleReflector))
          .ToLookup(c => c.Item1, c => c.Item2);

      IAttributesBasedValidationPropertyRuleReflector GetValidationPropertyRuleReflector (PropertyInfo property)
      {
        if (_validationPropertyRuleReflectorMocks.TryGetValue((DeclaringType: property.DeclaringType, PropertyName: property.Name), out var mock))
          return mock.Object;

        throw new InvalidOperationException(string.Format("Property '{0}' declared on type '{1}' has not been set up.", property.Name, property.DeclaringType));
      }
    }
  }
}
