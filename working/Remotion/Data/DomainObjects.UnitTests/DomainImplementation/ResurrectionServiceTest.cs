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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation
{
  [TestFixture]
  public class ResurrectionServiceTest : ClientTransactionBaseTest
  {
    private DomainObject _invalidObject;

    public override void SetUp ()
    {
      base.SetUp();

      _invalidObject = Order.NewObject();
      LifetimeService.DeleteObject (TestableClientTransaction, _invalidObject);
    }

    [Test]
    public void ResurrectInvalidObject ()
    {
      Assert.That (TestableClientTransaction.IsInvalid (_invalidObject.ID), Is.True);

      ResurrectionService.ResurrectInvalidObject (TestableClientTransaction, _invalidObject.ID);

      Assert.That (TestableClientTransaction.IsInvalid (_invalidObject.ID), Is.False);
    }

    [Test]
    public void ResurrectInvalidObject_Hierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        Assert.That (TestableClientTransaction.IsInvalid (_invalidObject.ID), Is.True);
        Assert.That (subTransaction.IsInvalid (_invalidObject.ID), Is.True);

        ResurrectionService.ResurrectInvalidObject (TestableClientTransaction, _invalidObject.ID);

        Assert.That (TestableClientTransaction.IsInvalid (_invalidObject.ID), Is.False);
        Assert.That (subTransaction.IsInvalid (_invalidObject.ID), Is.False);
      }
    }

    [Test]
    public void ResurrectInvalidObject_Hierarchy_InvalidInSubOnly ()
    {
      var objectInvalidInSubOnly = DomainObjectIDs.Order1.GetObject<Order> ();
      objectInvalidInSubOnly.Delete();

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Assert.That (TestableClientTransaction.IsInvalid (objectInvalidInSubOnly.ID), Is.False);
        Assert.That (subTransaction.IsInvalid (objectInvalidInSubOnly.ID), Is.True);

        Assert.That (
            () => ResurrectionService.ResurrectInvalidObject (TestableClientTransaction, objectInvalidInSubOnly.ID), 
            Throws.InvalidOperationException.With.Message.EqualTo (
                "Cannot resurrect object '" + objectInvalidInSubOnly.ID + "' because it is not invalid within the whole transaction hierarchy. "
                + "In transaction '" + TestableClientTransaction + "', the object has state 'Deleted'."));

        Assert.That (TestableClientTransaction.IsInvalid (objectInvalidInSubOnly.ID), Is.False);
        Assert.That (subTransaction.IsInvalid (objectInvalidInSubOnly.ID), Is.True);
      }
    }

    [Test]
    public void ResurrectInvalidObject_Hierarchy_InvalidInRootOnly ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        var objectInvalidInRootOnly = Order.NewObject ();

        Assert.That (TestableClientTransaction.IsInvalid (objectInvalidInRootOnly.ID), Is.True);
        Assert.That (subTransaction.IsInvalid (objectInvalidInRootOnly.ID), Is.False);

        Assert.That (
            () => ResurrectionService.ResurrectInvalidObject (TestableClientTransaction, objectInvalidInRootOnly.ID),
            Throws.InvalidOperationException.With.Message.EqualTo (
                "Cannot resurrect object '" + objectInvalidInRootOnly.ID + "' because it is not invalid within the whole transaction hierarchy. "
                + "In transaction '" + subTransaction + "', the object has state 'New'."));

        Assert.That (TestableClientTransaction.IsInvalid (objectInvalidInRootOnly.ID), Is.True);
        Assert.That (subTransaction.IsInvalid (objectInvalidInRootOnly.ID), Is.False);
      }
    }

    [Test]
    public void TryResurrectInvalidObject ()
    {
      Assert.That (TestableClientTransaction.IsInvalid (_invalidObject.ID), Is.True);

      var result = ResurrectionService.TryResurrectInvalidObject (TestableClientTransaction, _invalidObject.ID);

      Assert.That (result, Is.True);
      Assert.That (TestableClientTransaction.IsInvalid (_invalidObject.ID), Is.False);
    }

    [Test]
    public void TryResurrectInvalidObject_Hierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Assert.That (TestableClientTransaction.IsInvalid (_invalidObject.ID), Is.True);
        Assert.That (subTransaction.IsInvalid (_invalidObject.ID), Is.True);

        var result = ResurrectionService.TryResurrectInvalidObject (TestableClientTransaction, _invalidObject.ID);

        Assert.That (result, Is.True);
        Assert.That (TestableClientTransaction.IsInvalid (_invalidObject.ID), Is.False);
        Assert.That (subTransaction.IsInvalid (_invalidObject.ID), Is.False);
      }
    }

    [Test]
    public void TryResurrectInvalidObject_Hierarchy_InvalidInSubOnly ()
    {
      var objectInvalidInSubOnly = DomainObjectIDs.Order1.GetObject<Order> ();
      objectInvalidInSubOnly.Delete ();

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Assert.That (TestableClientTransaction.IsInvalid (objectInvalidInSubOnly.ID), Is.False);
        Assert.That (subTransaction.IsInvalid (objectInvalidInSubOnly.ID), Is.True);

        var result = ResurrectionService.TryResurrectInvalidObject (TestableClientTransaction, objectInvalidInSubOnly.ID);

        Assert.That (result, Is.False);
        Assert.That (TestableClientTransaction.IsInvalid (objectInvalidInSubOnly.ID), Is.False);
        Assert.That (subTransaction.IsInvalid (objectInvalidInSubOnly.ID), Is.True);
      }
    }

    [Test]
    public void TryResurrectInvalidObject_Hierarchy_InvalidInRootOnly ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        var objectInvalidInRootOnly = Order.NewObject ();

        Assert.That (TestableClientTransaction.IsInvalid (objectInvalidInRootOnly.ID), Is.True);
        Assert.That (subTransaction.IsInvalid (objectInvalidInRootOnly.ID), Is.False);

        var result = ResurrectionService.TryResurrectInvalidObject (TestableClientTransaction, objectInvalidInRootOnly.ID);

        Assert.That (result, Is.False);
        Assert.That (TestableClientTransaction.IsInvalid (objectInvalidInRootOnly.ID), Is.True);
        Assert.That (subTransaction.IsInvalid (objectInvalidInRootOnly.ID), Is.False);
      }
    }
  }
}