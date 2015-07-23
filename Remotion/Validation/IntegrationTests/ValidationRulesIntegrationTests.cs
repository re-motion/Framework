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
using Remotion.Validation.IntegrationTests.TestDomain.ComponentA;
using Remotion.Validation.IntegrationTests.TestDomain.ComponentB;
using Remotion.Validation.MetaValidation;

namespace Remotion.Validation.IntegrationTests
{
  [TestFixture]
  public class ValidationRulesIntegrationTests : IntegrationTestBase
  {
    public override void SetUp ()
    {
      base.SetUp ();

      ShowLogOutput = false;
    }

    [Test]
    public void BuildSpecialCustomer_RemoveLastNameHardConstraint_ThrowsException ()
    {
      Assert.That (() => ValidationBuilder.BuildValidator<SpecialCustomer2> (), 
        Throws.TypeOf<ValidationConfigurationException> ().And.Message.EqualTo (
        "Hard constraint validator(s) 'LengthValidator' on property "
        + "'Remotion.Validation.IntegrationTests.TestDomain.ComponentA.Person.LastName' cannot be removed."));
    }

    [Test]
    public void BuildAdressValidator_WhenAndUnlessConditionApplied ()
    {
      var address1 = new Address { Country = "Deutschland", PostalCode = "DE - 432134" };
      var address2 = new Address { Country = "Deutschland", PostalCode = "AT - 1220" };
      var address3 = new Address { Street = "Maria Hilferstrasse 145", City = "Wien", PostalCode = "1090" };
      var address4 = new Address { Street = "Maria Hilferstrasse 145", City = "Salzburg", PostalCode = "1090" };
      var address5 = new Address { Country = "Brunei" };
      var address6 = new Address { Country = "Tschiputi" };

      var validator = ValidationBuilder.BuildValidator<Address>();

      var result1 = validator.Validate (address1);
      Assert.That (result1.IsValid, Is.True);

      var result2 = validator.Validate (address2);
      Assert.That (result2.IsValid, Is.False);
      Assert.That (result2.Errors.Count, Is.EqualTo (1));
      Assert.That (result2.Errors[0].ErrorMessage, Is.EqualTo ("'PostalCode' is not in the correct format."));

      var result3 = validator.Validate (address3);
      Assert.That (result3.IsValid, Is.True);

      var result4 = validator.Validate (address4);
      Assert.That (result4.IsValid, Is.False);
      Assert.That (result4.Errors.Count, Is.EqualTo (1));
      Assert.That (result4.Errors[0].ErrorMessage, Is.EqualTo ("'City' is not in the correct format."));

      var result5 = validator.Validate (address5);
      Assert.That (result5.IsValid, Is.True);

      var result6 = validator.Validate (address6);
      Assert.That (result6.IsValid, Is.False);
      Assert.That (result6.Errors.Count, Is.EqualTo (1));
      Assert.That (result6.Errors[0].ErrorMessage, Is.EqualTo ("'PostalCode' must not be empty."));
    }

    [Test]
    public void BuildEmployeeValidator_ConditionalMessage ()
    {
      var employee = new Employee { FirstName = "FirstName", LastName = "LastName" };

      var validator = ValidationBuilder.BuildValidator<Employee>();

      var result = validator.Validate (employee);

      Assert.That (result.IsValid, Is.False);
      Assert.That (result.Errors.Count, Is.EqualTo (1));
      Assert.That (result.Errors[0].ErrorMessage, Is.EqualTo ("Conditional Message Test: Kein Gehalt definiert"));
    }

    [Test]
    public void BuildOrderItemValidator_SetValueTypeToDefaulValue_ValidationFails ()
    {
      var orderItem = new OrderItem();
      orderItem.Quantity = 0;

      var validator = ValidationBuilder.BuildValidator<OrderItem>();

      var result = validator.Validate (orderItem);

      Assert.That (result.IsValid, Is.False);
      Assert.That (result.Errors.Count, Is.EqualTo (1));
      Assert.That (result.Errors[0].ErrorMessage, Is.EqualTo ("'Quantity' should not be empty."));
    }
  }
}