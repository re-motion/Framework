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
    public void ValidateDuringCommit ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty(orderClass);
      StateDefinition paidState = paymentProperty[EnumWrapper.Get(PaymentState.Paid).Name];
      StateDefinition notPaidState = paymentProperty[EnumWrapper.Get(PaymentState.None).Name];
      _testHelper.CreateStateCombination(orderClass, paidState);
      _testHelper.CreateStateCombination(orderClass, notPaidState);
      _testHelper.CreateStateCombination(orderClass);
      var dupicateStateCombination = orderClass.CreateStatefulAccessControlList().StateCombinations[0];

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        dupicateStateCombination.AttachState(paidState);
        Assert.That(
            () => ClientTransaction.Current.Commit(),
            Throws.InstanceOf<ConstraintViolationException>()
                .With.Message.EqualTo(
                    "The securable class definition 'Remotion.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination "
                    + "that has been defined twice."));
      }
    }

    [Test]
    public void OnCommitting_WithChangedStateUsage_RegistersClassForCommit ()
    {
      var classDefinition = _testHelper.CreateOrderClassDefinition();
      var property = _testHelper.CreatePaymentStateProperty(classDefinition);
      var state = property[EnumWrapper.Get(PaymentState.Paid).Name];
      var combination = _testHelper.CreateStateCombination(classDefinition);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That(GetDataContainer((DomainObject)sender).HasBeenMarkedChanged, Is.True);
        };
        combination.AttachState(state);

        ClientTransaction.Current.Commit();

        Assert.That(commitOnClassWasCalled, Is.True);
      }
    }

    [Test]
    public void OnCommitting_WithDeletedStateUsage_RegistersClassForCommit ()
    {
      var classDefinition = _testHelper.CreateOrderClassDefinition();
      var property = _testHelper.CreatePaymentStateProperty(classDefinition);
      var combination = _testHelper.CreateStateCombination(classDefinition);
      combination.AttachState(property[EnumWrapper.Get(PaymentState.Paid).Name]);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That(GetDataContainer((DomainObject)sender).HasBeenMarkedChanged, Is.True);
        };
        combination.ClearStates();
        combination.AttachState(property[EnumWrapper.Get(PaymentState.None).Name]);

        ClientTransaction.Current.Commit();

        Assert.That(commitOnClassWasCalled, Is.True);
      }
    }
  }
}
