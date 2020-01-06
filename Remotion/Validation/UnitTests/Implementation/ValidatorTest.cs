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
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class ValidatorTest
  {
    private IValidationRule _validationRuleStub1;
    private IValidationRule _validationRuleStub2;
    private Validator _validator;
    private ValidationFailure _validationFailure;
    private Customer _validatedObject;

    [SetUp]
    public void SetUp ()
    {
      _validatedObject = new Customer();

      _validationRuleStub1 = MockRepository.GenerateStub<IValidationRule>();
      _validationRuleStub2 = MockRepository.GenerateStub<IValidationRule>();

      _validationFailure = new ObjectValidationFailure (_validatedObject, "Error", "ValidationMessage");

      _validator = new Validator (new[] { _validationRuleStub1, _validationRuleStub2 }, typeof (Customer));
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_validator.ValidationRules, Is.EquivalentTo (new[] { _validationRuleStub1, _validationRuleStub2 }));
    }

    [Test]
    public void Create ()
    {
      var result = _validator.Create<Customer>();

      Assert.That (result, Is.TypeOf (typeof (TypedValidatorDecorator<Customer>)));
    }

    [Test]
    public void Validate ()
    {
      var customer = _validatedObject;

      _validationRuleStub1
          .Stub (stub => stub.Validate (Arg<ValidationContext>.Is.NotNull))
          .Return (new[] { _validationFailure });
      _validationRuleStub2
          .Stub (stub => stub.Validate (Arg<ValidationContext>.Is.NotNull))
          .Return (new ValidationFailure[0]);

      var result = _validator.Validate (customer);

      Assert.That (result.Errors, Is.EquivalentTo (new[] { _validationFailure }));
    }

    [Test]
    public void Validate_InvalidInstance ()
    {
      Assert.That (
          () => ((IValidator) _validator).Validate ("Invalid"),
          Throws.InvalidOperationException.And.Message.EqualTo (
              "Cannot validate instances of type 'String'. This validator can only validate instances of type 'Customer'."));
    }

    [Test]
    public void CreateDescriptor ()
    {
      var result = _validator.CreateDescriptor();

      Assert.That (result.ValidationRules, Is.EquivalentTo (new[] { _validationRuleStub1, _validationRuleStub2 }));
    }

    [Test]
    public void CanValidateInstancesOfType_Customer_True ()
    {
      Assert.That (_validator.CanValidateInstancesOfType (typeof (Customer)), Is.True);
    }

    [Test]
    public void CanValidateInstancesOfType_NoCustomer_False ()
    {
      Assert.That (_validator.CanValidateInstancesOfType (typeof (Address)), Is.False);
    }
  }
}