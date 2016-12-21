// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class SecurableClassValidationResultTest : DomainTest
  {
    [Test]
    public void IsValid_Valid ()
    {
      SecurableClassValidationResult result = new SecurableClassValidationResult();

      Assert.That (result.IsValid, Is.True);
    }

    [Test]
    public void IsValid_DuplicateStateCombination ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StateCombination stateCombination = testHelper.CreateStateCombination (orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddDuplicateStateCombination (stateCombination);

        Assert.That (result.IsValid, Is.False);
      }
    }

    [Test]
    public void DuplicateStateCombinations_AllValid ()
    {
      SecurableClassValidationResult result = new SecurableClassValidationResult();

      Assert.That (result.DuplicateStateCombinations.Count, Is.EqualTo (0));
    }

    [Test]
    public void DuplicateStateCombinations_OneInvalidStateCombination ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StateCombination stateCombination = testHelper.CreateStateCombination (orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddDuplicateStateCombination (stateCombination);

        Assert.That (result.DuplicateStateCombinations.Count, Is.EqualTo (1));
        Assert.That (result.DuplicateStateCombinations, Has.Member (stateCombination));
      }
    }

    [Test]
    public void DuplicateStateCombinations_TwoInvalidStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty (orderClass);
        StateCombination statelessCombination = testHelper.CreateStateCombination (orderClass);
        StateCombination paidStateCombination = testHelper.CreateStateCombination (
            orderClass, paymentProperty[EnumWrapper.Get (PaymentState.Paid).Name]);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddDuplicateStateCombination (statelessCombination);
        result.AddDuplicateStateCombination (paidStateCombination);

        Assert.That (result.DuplicateStateCombinations.Count, Is.EqualTo (2));
        Assert.That (result.DuplicateStateCombinations, Has.Member (statelessCombination));
        Assert.That (result.DuplicateStateCombinations, Has.Member (paidStateCombination));
      }
    }


    [Test]
    public void IsValid_InvalidStateCombination ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StateCombination stateCombination = testHelper.CreateStateCombination (orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddInvalidStateCombination (stateCombination);

        Assert.That (result.IsValid, Is.False);
      }
    }

    [Test]
    public void InvalidStateCombinations_AllValid ()
    {
      SecurableClassValidationResult result = new SecurableClassValidationResult();

      Assert.That (result.InvalidStateCombinations.Count, Is.EqualTo (0));
    }

    [Test]
    public void InvalidStateCombinations_OneInvalidStateCombination ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StateCombination stateCombination = testHelper.CreateStateCombination (orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddInvalidStateCombination (stateCombination);

        Assert.That (result.InvalidStateCombinations.Count, Is.EqualTo (1));
        Assert.That (result.InvalidStateCombinations, Has.Member (stateCombination));
      }
    }

    [Test]
    public void InvalidStateCombinations_TwoInvalidStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty (orderClass);
        StateCombination statelessCombination = testHelper.CreateStateCombination (orderClass);
        StateCombination paidStateCombination = testHelper.CreateStateCombination (
            orderClass, paymentProperty[EnumWrapper.Get (PaymentState.Paid).Name]);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddInvalidStateCombination (statelessCombination);
        result.AddInvalidStateCombination (paidStateCombination);

        Assert.That (result.InvalidStateCombinations.Count, Is.EqualTo (2));
        Assert.That (result.InvalidStateCombinations, Has.Member (statelessCombination));
        Assert.That (result.InvalidStateCombinations, Has.Member (paidStateCombination));
      }
    }
  }
}
