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
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateCombinationComparerTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void Equals_TwoNull ()
    {

      StateCombinationComparer comparer = new StateCombinationComparer();

      Assert.That(comparer.Equals(null,null), Is.True);
    }

    [Test]
    public void Equals_OneNull ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StateCombination combination = _testHelper.CreateStateCombination(orderClass);

      StateCombinationComparer comparer = new StateCombinationComparer();

      Assert.That(comparer.Equals(combination,null), Is.False);
      Assert.That(comparer.Equals(null, combination), Is.False);
    }

    [Test]
    public void Equals_TwoStatelessCombinations ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StateCombination combination1 = _testHelper.CreateStateCombination(orderClass);
      StateCombination combination2 = _testHelper.CreateStateCombination(orderClass);

      StateCombinationComparer comparer = new StateCombinationComparer();

      Assert.That(comparer.Equals(combination1, combination2), Is.True);
    }

    [Test]
    public void Equals_OneStatelessAndOneWithAState ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty(orderClass);
      StateCombination combination1 = _testHelper.CreateStateCombination(orderClass);
      StateCombination combination2 = _testHelper.CreateStateCombination(orderClass, paymentProperty[EnumWrapper.Get(PaymentState.Paid).Name]);

      StateCombinationComparer comparer = new StateCombinationComparer();

      Assert.That(comparer.Equals(combination1, combination2), Is.False);
    }

    [Test]
    public void Equals_TwoDifferent ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty(orderClass);
      StateCombination combination1 = _testHelper.CreateStateCombination(orderClass, paymentProperty[EnumWrapper.Get(PaymentState.None).Name]);
      StateCombination combination2 = _testHelper.CreateStateCombination(orderClass, paymentProperty[EnumWrapper.Get(PaymentState.Paid).Name]);

      StateCombinationComparer comparer = new StateCombinationComparer();

      Assert.That(comparer.Equals(combination1, combination2), Is.False);
    }

    [Test]
    public void GetHashCode_TwoStatelessCombinations ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StateCombination combination1 = _testHelper.CreateStateCombination(orderClass);
      StateCombination combination2 = _testHelper.CreateStateCombination(orderClass);

      StateCombinationComparer comparer = new StateCombinationComparer();

      Assert.That(comparer.GetHashCode(combination2), Is.EqualTo(comparer.GetHashCode(combination1)));
    }

    [Test]
    public void GetHashCode_OneStatelessAndOneWithAState ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty(orderClass);
      StateCombination combination1 = _testHelper.CreateStateCombination(orderClass);
      StateCombination combination2 = _testHelper.CreateStateCombination(orderClass, paymentProperty[EnumWrapper.Get(PaymentState.Paid).Name]);

      StateCombinationComparer comparer = new StateCombinationComparer();

      Assert.That(comparer.GetHashCode(combination2), Is.Not.EqualTo(comparer.GetHashCode(combination1)));
    }

    [Test]
    public void GetHashCode_TwoDifferent ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty(orderClass);
      StateCombination combination1 = _testHelper.CreateStateCombination(orderClass, paymentProperty[EnumWrapper.Get(PaymentState.None).Name]);
      StateCombination combination2 = _testHelper.CreateStateCombination(orderClass, paymentProperty[EnumWrapper.Get(PaymentState.Paid).Name]);

      StateCombinationComparer comparer = new StateCombinationComparer();

      Assert.That(comparer.GetHashCode(combination2), Is.Not.EqualTo(comparer.GetHashCode(combination1)));
    }
  }
}
