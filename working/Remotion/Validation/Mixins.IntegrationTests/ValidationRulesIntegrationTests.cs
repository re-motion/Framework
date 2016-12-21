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
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.TypePipe;
using Remotion.Validation.Mixins.IntegrationTests.TestDomain.ComponentA;
using Remotion.Validation.Mixins.IntegrationTests.TestDomain.ComponentB;

namespace Remotion.Validation.Mixins.IntegrationTests
{
  [TestFixture]
  public class ValidationRulesIntegrationTests : IntegrationTestBase
  {
    public override void SetUp ()
    {
      base.SetUp();

      ShowLogOutput = false;
    }

    [Test]
    public void BuildCustomerValidator_PersonLastNameEqualsValidatorRemovedByCollector ()
    {
      var customer = ObjectFactory.Create<Customer> (ParamList.Empty);
      customer.UserName = "cust1";
      customer.LastName = "Test";
      customer.FirstName = "Firstname";

      var validator = ValidationBuilder.BuildValidator<Customer>();

      var result = validator.Validate (customer);

      Assert.That (result.IsValid, Is.True);
      Assert.That (result.Errors.Any(), Is.False);
    }

    [Test]
    public void BuildSpecialCustomerValidator_CustomerUsernameMaxLengthAndAllFirstNameNotNullValidatorsRemoved ()
        //2 NotNull Validators removed (IPerson + CustomerValidationCollector!)
    {
      var specialCustomer = ObjectFactory.Create<SpecialCustomer1> (ParamList.Empty);
      specialCustomer.UserName = "Test123456";
      specialCustomer.LastName = "Test1234";
      specialCustomer.FirstName = "Test456";
      var validator = ValidationBuilder.BuildValidator<SpecialCustomer1>();

      var result = validator.Validate (specialCustomer);

      Assert.That (result.IsValid, Is.True);
      Assert.That (result.Errors.Any(), Is.False);
    }

    [Test]
    public void BuildCustomerValidator_CustomerMixinIntroducedValidator_MixinInterfaceIntroducedValidatorIsRemovedByApplyWithMixinCollector ()
    {
      var customer = ObjectFactory.Create<Customer> (ParamList.Empty);
      customer.FirstName = "Ralf";
      customer.LastName = "Mayr";
      customer.UserName = "mm2";
      ((ICustomerIntroduced) customer).Title = "Chef3";

      var validator = ValidationBuilder.BuildValidator<Customer>();

      var result = validator.Validate (customer);

      Assert.That (result.IsValid, Is.True);
    }
  }
}