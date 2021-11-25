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
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Providers
{
  public class TestableAttributeBasedValidationRuleCollectorProviderBase : AttributeBasedValidationRuleCollectorProviderBase
  {
    private readonly IDictionary<Type, Mock<IAttributesBasedValidationPropertyRuleReflector>> _validationPropertyRuleReflectorMocks;
    private readonly IPropertyValidator _propertyValidatorStub1;
    private readonly IPropertyValidator _propertyValidatorStub2;
    private readonly IPropertyValidator _propertyValidatorStub3;
    private readonly IPropertyValidator _propertyValidatorStub4;
    private readonly IPropertyValidator _propertyValidatorStub5;
    private readonly IPropertyValidator _propertyValidatorStub6;
    private readonly RemovingValidatorRegistration _removingValidatorRegistration1;
    private readonly RemovingValidatorRegistration _removingValidatorRegistration2;
    private readonly RemovingValidatorRegistration _removingValidatorRegistration3;
    private readonly RemovingValidatorRegistration _removingValidatorRegistration4;
    private readonly IPropertyMetaValidationRule _propertyMetaValidationRule1;
    private readonly IPropertyMetaValidationRule _propertyMetaValidationRule2;
    private readonly IPropertyMetaValidationRule _propertyMetaValidationRule3;

    public TestableAttributeBasedValidationRuleCollectorProviderBase (
        IDictionary<Type, Mock<IAttributesBasedValidationPropertyRuleReflector>> validationPropertyRuleReflectorMocks,
        IPropertyValidator propertyValidatorStub1 = null,
        IPropertyValidator propertyValidatorStub2 = null,
        IPropertyValidator propertyValidatorStub3 = null,
        IPropertyValidator propertyValidatorStub4 = null,
        IPropertyValidator propertyValidatorStub5 = null,
        IPropertyValidator propertyValidatorStub6 = null,
        RemovingValidatorRegistration removingValidatorRegistration1 = null,
        RemovingValidatorRegistration removingValidatorRegistration2 = null,
        RemovingValidatorRegistration removingValidatorRegistration3 = null,
        RemovingValidatorRegistration removingValidatorRegistration4 = null,
        IPropertyMetaValidationRule propertyMetaValidationRule1 = null,
        IPropertyMetaValidationRule propertyMetaValidationRule2 = null,
        IPropertyMetaValidationRule propertyMetaValidationRule3 = null)
    {
      _validationPropertyRuleReflectorMocks = validationPropertyRuleReflectorMocks;
      _propertyValidatorStub1 = propertyValidatorStub1;
      _propertyValidatorStub2 = propertyValidatorStub2;
      _propertyValidatorStub3 = propertyValidatorStub3;
      _propertyValidatorStub4 = propertyValidatorStub4;
      _propertyValidatorStub5 = propertyValidatorStub5;
      _propertyValidatorStub6 = propertyValidatorStub6;
      _removingValidatorRegistration1 = removingValidatorRegistration1;
      _removingValidatorRegistration2 = removingValidatorRegistration2;
      _removingValidatorRegistration3 = removingValidatorRegistration3;
      _removingValidatorRegistration4 = removingValidatorRegistration4;
      _propertyMetaValidationRule1 = propertyMetaValidationRule1;
      _propertyMetaValidationRule2 = propertyMetaValidationRule2;
      _propertyMetaValidationRule3 = propertyMetaValidationRule3;
    }

    protected override ILookup<Type, IAttributesBasedValidationPropertyRuleReflector> CreatePropertyRuleReflectors (IEnumerable<Type> types)
    {
      var involvedTypes = types.ToArray();
      foreach (var type in involvedTypes)
      {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        _validationPropertyRuleReflectorMocks[type].Setup(stub => stub.ValidatedProperty).Returns(PropertyInfoAdapter.Create(properties.First()));

        var sequence1 = new MockSequence();
        var sequence2 = new MockSequence();
        var sequence3 = new MockSequence();
        var sequence4 = new MockSequence();
        var sequence5 = new MockSequence();
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
          if (property.Name == "Position")
          {
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence1)
                .Setup(mock => mock.GetValidatedPropertyFunc(typeof (Employee)))
                .Returns(e => ((Employee) e).Position)
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence2)
                .Setup(mock => mock.GetRemovablePropertyValidators())
                .Returns(new[] { _propertyValidatorStub1 })
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence3)
                .Setup(mock => mock.GetNonRemovablePropertyValidators())
                .Returns(new[] { _propertyValidatorStub2 })
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence4)
                .Setup(mock => mock.GetRemovingValidatorRegistrations())
                .Returns(new RemovingValidatorRegistration[0])
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence5)
                .Setup(mock => mock.GetMetaValidationRules())
                .Returns(new IPropertyMetaValidationRule[0])
                .Verifiable();
          }
          else if (property.Name == "Notes")
          {
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence1)
                .Setup(mock => mock.GetValidatedPropertyFunc(typeof (Employee)))
                .Returns(e => ((Employee) e).Notes)
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence2)
                .Setup(mock => mock.GetRemovablePropertyValidators())
                .Returns(new[] { _propertyValidatorStub3 })
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence3)
                .Setup(mock => mock.GetNonRemovablePropertyValidators())
                .Returns(new IPropertyValidator[0])
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence4)
                .Setup(mock => mock.GetRemovingValidatorRegistrations())
                .Returns(new[] { _removingValidatorRegistration1, _removingValidatorRegistration2 })
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence5)
                .Setup(mock => mock.GetMetaValidationRules())
                .Returns(new IPropertyMetaValidationRule[0])
                .Verifiable();
          }
          else if (property.Name == "LastName")
          {
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence1)
                .Setup(mock => mock.GetValidatedPropertyFunc(typeof (SpecialCustomer1)))
                .Returns(c => ((SpecialCustomer1) c).LastName)
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence2)
                .Setup(mock => mock.GetRemovablePropertyValidators())
                .Returns(new[] { _propertyValidatorStub4, _propertyValidatorStub5 })
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence3)
                .Setup(mock => mock.GetNonRemovablePropertyValidators())
                .Returns(new IPropertyValidator[0])
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence4)
                .Setup(mock => mock.GetRemovingValidatorRegistrations())
                .Returns(new RemovingValidatorRegistration[0])
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence5)
                .Setup(mock => mock.GetMetaValidationRules())
                .Returns(new[] { _propertyMetaValidationRule1, _propertyMetaValidationRule3 })
                .Verifiable();
          }
          else if (property.Name == "UserName")
          {
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence1)
                .Setup(mock => mock.GetValidatedPropertyFunc(typeof (SpecialCustomer1)))
                .Returns(c => ((SpecialCustomer1) c).UserName)
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence2)
                .Setup(mock => mock.GetRemovablePropertyValidators())
                .Returns(new[] { _propertyValidatorStub6 })
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence3)
                .Setup(mock => mock.GetNonRemovablePropertyValidators())
                .Returns(new IPropertyValidator[0])
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence4)
                .Setup(mock => mock.GetRemovingValidatorRegistrations())
                .Returns(new[] { _removingValidatorRegistration3, _removingValidatorRegistration4 })
                .Verifiable();
            _validationPropertyRuleReflectorMocks[type]
                .InSequence(sequence5)
                .Setup(mock => mock.GetMetaValidationRules())
                .Returns(new[] { _propertyMetaValidationRule2 })
                .Verifiable();
          }
          else
          {
            if (property.DeclaringType != typeof (Person))
              throw new Exception(string.Format("Property '{0}' not expected.", property.Name));
          }
        }
      }

      return
        involvedTypes.SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance  | BindingFlags.DeclaredOnly))
            .Select(p => new { Type = p.DeclaringType, Property = p })
            .Select(t => new Tuple<Type, IAttributesBasedValidationPropertyRuleReflector>(t.Type, _validationPropertyRuleReflectorMocks[t.Type].Object))
            .ToLookup(c => c.Item1, c => c.Item2);
    }
  }
}