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
using FluentValidation.Validators;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Providers;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Providers
{
  public class TestableAttributeBasedValidationCollectorProviderBase : AttributeBasedValidationCollectorProviderBase
  {
    private readonly IDictionary<Type, IAttributesBasedValidationPropertyRuleReflector> _validationPropertyRuleReflectorMocks;
    private readonly IPropertyValidator _propertyValidatorStub1;
    private readonly IPropertyValidator _propertyValidatorStub2;
    private readonly IPropertyValidator _propertyValidatorStub3;
    private readonly IPropertyValidator _propertyValidatorStub4;
    private readonly IPropertyValidator _propertyValidatorStub5;
    private readonly IPropertyValidator _propertyValidatorStub6;
    private readonly ValidatorRegistration _validatorRegistration1;
    private readonly ValidatorRegistration _validatorRegistration2;
    private readonly ValidatorRegistration _validatorRegistration3;
    private readonly ValidatorRegistration _validatorRegistration4;
    private readonly IMetaValidationRule _metaValidationRule1;
    private readonly IMetaValidationRule _metaValidationRule2;
    private readonly IMetaValidationRule _metaValidationRule3;

    public TestableAttributeBasedValidationCollectorProviderBase (
        IDictionary<Type, IAttributesBasedValidationPropertyRuleReflector> validationPropertyRuleReflectorMocks,
        IPropertyValidator propertyValidatorStub1 = null,
        IPropertyValidator propertyValidatorStub2 = null,
        IPropertyValidator propertyValidatorStub3 = null,
        IPropertyValidator propertyValidatorStub4 = null,
        IPropertyValidator propertyValidatorStub5 = null,
        IPropertyValidator propertyValidatorStub6 = null,
        ValidatorRegistration validatorRegistration1 = null,
        ValidatorRegistration validatorRegistration2 = null,
        ValidatorRegistration validatorRegistration3 = null,
        ValidatorRegistration validatorRegistration4 = null,
        IMetaValidationRule metaValidationRule1 = null,
        IMetaValidationRule metaValidationRule2 = null,
        IMetaValidationRule metaValidationRule3 = null)
    {
      _validationPropertyRuleReflectorMocks = validationPropertyRuleReflectorMocks;
      _propertyValidatorStub1 = propertyValidatorStub1;
      _propertyValidatorStub2 = propertyValidatorStub2;
      _propertyValidatorStub3 = propertyValidatorStub3;
      _propertyValidatorStub4 = propertyValidatorStub4;
      _propertyValidatorStub5 = propertyValidatorStub5;
      _propertyValidatorStub6 = propertyValidatorStub6;
      _validatorRegistration1 = validatorRegistration1;
      _validatorRegistration2 = validatorRegistration2;
      _validatorRegistration3 = validatorRegistration3;
      _validatorRegistration4 = validatorRegistration4;
      _metaValidationRule1 = metaValidationRule1;
      _metaValidationRule2 = metaValidationRule2;
      _metaValidationRule3 = metaValidationRule3;
    }

    protected override ILookup<Type, IAttributesBasedValidationPropertyRuleReflector> CreatePropertyRuleReflectors (IEnumerable<Type> types)
    {
      var involvedTypes = types.ToArray();
      foreach (var type in involvedTypes)
      {
        foreach (var property in type.GetProperties (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
          _validationPropertyRuleReflectorMocks[type].Stub (stub => stub.ValidatedProperty).Return (property);

          if (property.Name == "Position")
          {
            _validationPropertyRuleReflectorMocks[type]
                .Expect (mock => mock.GetPropertyAccessExpression (typeof (Employee)))
                .Return (e => ((Employee) e).Position);
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetAddingPropertyValidators ()).Return (new[] { _propertyValidatorStub1 });
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetHardConstraintPropertyValidators ())
                .Return (new[] { _propertyValidatorStub2 });
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetRemovingPropertyRegistrations ()).Return (new ValidatorRegistration[0]);
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetMetaValidationRules ()).Return (new IMetaValidationRule[0]);
          }
          else if (property.Name == "Notes")
          {
            _validationPropertyRuleReflectorMocks[type]
                .Expect (mock => mock.GetPropertyAccessExpression (typeof (Employee)))
                .Return (e => ((Employee) e).Notes);
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetAddingPropertyValidators ()).Return (new[] { _propertyValidatorStub3 });
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetHardConstraintPropertyValidators ()).Return (new IPropertyValidator[0]);
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetRemovingPropertyRegistrations ())
                .Return (new[] { _validatorRegistration1, _validatorRegistration2 });
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetMetaValidationRules ()).Return (new IMetaValidationRule[0]);
          }
          else if (property.Name == "LastName")
          {
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetPropertyAccessExpression(typeof(SpecialCustomer1))).Return (c =>
                ((SpecialCustomer1) c).LastName);
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetAddingPropertyValidators ())
                .Return (new[] { _propertyValidatorStub4, _propertyValidatorStub5 });
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetHardConstraintPropertyValidators ()).Return (new IPropertyValidator[0]);
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetRemovingPropertyRegistrations ()).Return (new ValidatorRegistration[0]);
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetMetaValidationRules ())
                .Return (new[] { _metaValidationRule1, _metaValidationRule3 });
          }
          else if (property.Name == "UserName")
          {
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetPropertyAccessExpression (typeof(SpecialCustomer1))).Return (c =>
                ((SpecialCustomer1) c).UserName);
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetAddingPropertyValidators ()).Return (new[] { _propertyValidatorStub6 });
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetHardConstraintPropertyValidators ()).Return (new IPropertyValidator[0]);
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetRemovingPropertyRegistrations ())
                .Return (new[] { _validatorRegistration3, _validatorRegistration4 });
            _validationPropertyRuleReflectorMocks[type].Expect (mock => mock.GetMetaValidationRules ()).Return (new[] { _metaValidationRule2 });
          }
          else
          {
            if (property.DeclaringType != typeof (Person))
              throw new Exception (string.Format ("Property '{0}' not expected.", property.Name));
          }
        }
      }

      return
        involvedTypes.SelectMany (t => t.GetProperties (BindingFlags.Public | BindingFlags.Instance  | BindingFlags.DeclaredOnly))
            .Select (p => new { Type = p.DeclaringType, Property = p })
            .Select (t => new Tuple<Type, IAttributesBasedValidationPropertyRuleReflector> (t.Type, _validationPropertyRuleReflectorMocks[t.Type]))
            .ToLookup (c => c.Item1, c => c.Item2);
    }
  }
}