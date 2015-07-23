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
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateUsageTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    [ExpectedException (typeof (ConstraintViolationException), ExpectedMessage =
        "The securable class definition 'Remotion.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination "
        + "that has been defined twice.")]
    public void ValidateDuringCommit ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateDefinition paidState = paymentProperty[EnumWrapper.Get (PaymentState.Paid).Name];
      StateDefinition notPaidState = paymentProperty[EnumWrapper.Get (PaymentState.None).Name];
      _testHelper.CreateStateCombination (orderClass, paidState);
      _testHelper.CreateStateCombination (orderClass, notPaidState);
      _testHelper.CreateStateCombination (orderClass);
      var dupicateStateCombination = orderClass.CreateStatefulAccessControlList().StateCombinations[0];

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        dupicateStateCombination.AttachState (paidState);

        ClientTransaction.Current.Commit();
      }
    }

    [Test]
    public void OnCommitting_WithChangedStateUsage_RegistersClassForCommit ()
    {
      var classDefinition = _testHelper.CreateOrderClassDefinition();
      var property = _testHelper.CreatePaymentStateProperty (classDefinition);
      var state = property[EnumWrapper.Get (PaymentState.Paid).Name];
      var combination = _testHelper.CreateStateCombination (classDefinition);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That (GetDataContainer ((DomainObject) sender).HasBeenMarkedChanged, Is.True);
        };
        combination.AttachState (state);

        ClientTransaction.Current.Commit();

        Assert.That (commitOnClassWasCalled, Is.True);
      }
    }

    [Test]
    public void OnCommitting_WithDeletedStateUsage_RegistersClassForCommit ()
    {
      var classDefinition = _testHelper.CreateOrderClassDefinition();
      var property = _testHelper.CreatePaymentStateProperty (classDefinition);
      var combination = _testHelper.CreateStateCombination (classDefinition);
      combination.AttachState (property[EnumWrapper.Get (PaymentState.Paid).Name]);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That (GetDataContainer ((DomainObject) sender).HasBeenMarkedChanged, Is.True);
        };
        combination.ClearStates();
        combination.AttachState (property[EnumWrapper.Get (PaymentState.None).Name]);

        ClientTransaction.Current.Commit();

        Assert.That (commitOnClassWasCalled, Is.True);
      }
    }
  }
}
