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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateCombinationTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void GetClass ()
    {
      StateCombination combination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder();

      Assert.That(combination.AccessControlList.Class, Is.Not.Null);
      Assert.That(combination.Class, Is.SameAs(combination.AccessControlList.Class));
    }

    [Test]
    public void MatchesStates_StatefulAndWithoutDemandedStates ()
    {
      StateCombination combination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder();
      List<StateDefinition> states = CreateEmptyStateList();

      Assert.That(combination.MatchesStates(states), Is.False);
    }

    [Test]
    public void MatchesStates_DeliveredAndUnpaid ()
    {
      StateCombination combination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder();
      StateDefinition[] states = combination.GetStates();

      Assert.That(combination.MatchesStates(states), Is.True);
    }

    [Test]
    [Ignore("TODO: Implement")]
    public void MatchesStates_StatefulWithWildcard ()
    {
      StateCombination combination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder();
      StateDefinition[] states = combination.GetStates();

      Assert.Fail("TODO: Implement");
      Assert.That(combination.MatchesStates(states), Is.True);
    }

    [Test]
    public void AttachState_NewState ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StateCombination combination = _testHelper.CreateStateCombination(classDefinition);
      StatePropertyDefinition property = _testHelper.CreateTestProperty();
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        combination.AttachState(property["Test1"]);

        var states = combination.GetStates();
        Assert.That(states.Length, Is.EqualTo(1));
        Assert.That(states[0], Is.SameAs(property["Test1"]));
      }
    }

    [Test]
    public void AttachState_WithoutClassDefinition ()
    {
      StateCombination combination = StateCombination.NewObject();
      StatePropertyDefinition property = _testHelper.CreateTestProperty();

      combination.AttachState(property["Test1"]);

      Assert.That(combination.GetStates().Length, Is.EqualTo(1));
    }

    [Test]
    public void ClearStates ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StateCombination combination = _testHelper.CreateStateCombination(classDefinition);
      StatePropertyDefinition property = _testHelper.CreateTestProperty();
      combination.AttachState(property["Test1"]);
      Assert.That(combination.GetStates(), Is.Not.Empty);

      combination.ClearStates();

      Assert.That(combination.GetStates(), Is.Empty);
    }

    [Test]
    public void GetStates_Empty ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StateCombination combination = _testHelper.CreateStateCombination(classDefinition);

      StateDefinition[] states = combination.GetStates();

      Assert.That(states.Length, Is.EqualTo(0));
    }

    [Test]
    public void GetStates_OneState ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition property = _testHelper.CreatePaymentStateProperty(classDefinition);
      StateDefinition state = property.DefinedStates[1];
      StateCombination combination = _testHelper.CreateStateCombination(classDefinition, state);

      StateDefinition[] states = combination.GetStates();

      Assert.That(states.Length, Is.EqualTo(1));
      Assert.That(states[0], Is.SameAs(state));
    }

    [Test]
    public void GetStates_MultipleStates ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty(classDefinition);
      StateDefinition paidState = paymentProperty.DefinedStates[1];
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty(classDefinition);
      StateDefinition deliveredState = orderStateProperty.DefinedStates[1];
      StateCombination combination = _testHelper.CreateStateCombination(classDefinition, paidState, deliveredState);

      StateDefinition[] states = combination.GetStates();

      Assert.That(states.Length, Is.EqualTo(2));
      Assert.That(states, Has.Member(paidState));
      Assert.That(states, Has.Member(deliveredState));
    }

    [Test]
    public void ValidateDuringCommit_ByTouchOnClassForChangedStateUsagesCollection ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty(orderClass);
      StateDefinition paidState = paymentProperty[EnumWrapper.Get(PaymentState.Paid).Name];
      StateDefinition notPaidState = paymentProperty[EnumWrapper.Get(PaymentState.None).Name];
      StateCombination combination1 = _testHelper.CreateStateCombination(orderClass, paidState);
      StateCombination combination2 = _testHelper.CreateStateCombination(orderClass, notPaidState);
      StateCombination combination3 = _testHelper.CreateStateCombination(orderClass);
      combination1.AccessControlList.AccessControlEntries.Add(AccessControlEntry.NewObject());
      combination2.AccessControlList.AccessControlEntries.Add(AccessControlEntry.NewObject());
      combination3.AccessControlList.AccessControlEntries.Add(AccessControlEntry.NewObject());
      var dupicateStateCombination = orderClass.CreateStatefulAccessControlList().StateCombinations[0];

      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
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
    public void Commit_DeletedStateCombination ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        orderClass.EnsureDataAvailable();
        Assert.That(orderClass.State.IsUnchanged, Is.True);

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          StateCombination combination = _testHelper.CreateStateCombination(orderClass, ClientTransaction.Current);
          combination.AccessControlList.AccessControlEntries.Add(AccessControlEntry.NewObject());

          using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
          {
            orderClass.EnsureDataAvailable();
            Assert.That(orderClass.State.IsUnchanged, Is.True);

            combination.AccessControlList.Delete();
            Assert.That(combination.Class, Is.Null);

            Assert.That(orderClass.State.IsChanged, Is.True);
            ClientTransaction.Current.Commit();
          }

          ClientTransaction.Current.Commit();
        }

        Assert.That(orderClass.State.IsChanged, Is.True);
      }
    }

    [Test]
    public void SetAndGet_Index ()
    {
      StateCombination stateCombination = StateCombination.NewObject();

      stateCombination.Index = 1;
      Assert.That(stateCombination.Index, Is.EqualTo(1));
    }

    [Test]
    public void OnCommitting_WithChangedStateCombination_RegistersClassForCommit ()
    {
      var classDefinition = _testHelper.CreateOrderClassDefinition();
      var combination = _testHelper.CreateStateCombination(classDefinition);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That(GetDataContainer((DomainObject)sender).HasBeenMarkedChanged, Is.True);
        };
        combination.RegisterForCommit();

        ClientTransaction.Current.Commit();

        Assert.That(commitOnClassWasCalled, Is.True);
      }
    }

    [Test]
    public void OnCommitting_WithDeletedStateCombination_RegistersClassForCommit ()
    {
      var classDefinition = _testHelper.CreateOrderClassDefinition();
      var combination = _testHelper.CreateStateCombination(classDefinition);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That(GetDataContainer((DomainObject)sender).HasBeenMarkedChanged, Is.True);
        };
        combination.Delete();

        ClientTransaction.Current.Commit();

        Assert.That(commitOnClassWasCalled, Is.True);
      }
    }

    private static List<StateDefinition> CreateEmptyStateList ()
    {
      return new List<StateDefinition>();
    }
  }
}
