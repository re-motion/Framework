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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.Enlistment
{
  [TestFixture]
  public class DictionaryBasedEnlistedDomainObjectManagerTest : StandardMappingTest
  {
    private DictionaryBasedEnlistedDomainObjectManager _manager;
    private Order _order;

    public override void SetUp ()
    {
      base.SetUp();

      _manager = new DictionaryBasedEnlistedDomainObjectManager();
      _order = DomainObjectMother.GetObjectInOtherTransaction<Order>(DomainObjectIDs.Order1);
    }

    [Test]
    public void EnlistDomainObject ()
    {
      Assert.That(_manager.IsEnlisted(_order), Is.False);

      _manager.EnlistDomainObject(_order);

      Assert.That(_manager.IsEnlisted(_order), Is.True);
    }

    [Test]
    public void EnlistedDomainObjectCount ()
    {
      Assert.That(_manager.EnlistedDomainObjectCount, Is.EqualTo(0));

      _manager.EnlistDomainObject(_order);

      Assert.That(_manager.EnlistedDomainObjectCount, Is.EqualTo(1));
    }

    [Test]
    public void EnlistDomainObject_Multiple ()
    {
      _manager.EnlistDomainObject(_order);
      _manager.EnlistDomainObject(_order);

      Assert.That(_manager.IsEnlisted(_order), Is.True);
    }

    [Test]
    public void EnlistDomainObject_IDAlreadyEnlisted ()
    {
      var orderA = DomainObjectMother.GetObjectInOtherTransaction<Order>(DomainObjectIDs.Order1);
      var orderB = DomainObjectMother.GetObjectInOtherTransaction<Order>(DomainObjectIDs.Order1);

      _manager.EnlistDomainObject(orderA);
      Assert.That(
          () => _manager.EnlistDomainObject(orderB),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "A domain object instance for object "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' already exists in this transaction."));
    }

    [Test]
    public void Disenlist ()
    {
      _manager.EnlistDomainObject(_order);
      Assert.That(_manager.IsEnlisted(_order), Is.True);

      _manager.DisenlistDomainObject(_order);

      Assert.That(_manager.IsEnlisted(_order), Is.False);
    }

    [Test]
    public void Disenlist_NotEnlisted ()
    {
      Assert.That(
          () => _manager.DisenlistDomainObject(_order),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is not enlisted."));
    }

    [Test]
    public void Disenlist_OtherReferenceEnlisted ()
    {
      var orderA = DomainObjectMother.GetObjectInOtherTransaction<Order>(_order.ID);
      _manager.EnlistDomainObject(orderA);

      Assert.That(
          () => _manager.DisenlistDomainObject(_order),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is not enlisted."));
    }

    [Test]
    public void GetEnlistedDomainObjects ()
    {
      _manager.EnlistDomainObject(_order);

      Assert.That(_manager.GetEnlistedDomainObjects().ToArray(), Is.EqualTo(new[] { _order }));
    }

    [Test]
    public void GetEnlistedDomainObjects_EnlistWhileIteratingResultSequence ()
    {
      var orderA = DomainObjectMother.GetObjectInOtherTransaction<Order>(DomainObjectIDs.Order1);
      var orderB = DomainObjectMother.GetObjectInOtherTransaction<Order>(DomainObjectIDs.Order2);

      _manager.EnlistDomainObject(orderA);
      var enlistedObjects = new List<IDomainObject>();
      foreach (var enlistedDomainObject in _manager.GetEnlistedDomainObjects())
      {
        enlistedObjects.Add(enlistedDomainObject);
        if (enlistedDomainObject == orderA)
          _manager.EnlistDomainObject(orderB);
      }

      Assert.That(enlistedObjects, Is.EqualTo(new[] { orderA, orderB }));
    }

    [Test]
    public void GetEnlistedDomainObjects_DisenlistWhileIteratingResultSequence ()
    {
      var orderA = DomainObjectMother.GetObjectInOtherTransaction<Order>(DomainObjectIDs.Order1);
      var orderB = DomainObjectMother.GetObjectInOtherTransaction<Order>(DomainObjectIDs.Order2);

      _manager.EnlistDomainObject(orderA);
      _manager.EnlistDomainObject(orderB);
      var enlistedObjects = new List<IDomainObject>();
      foreach (var enlistedDomainObject in _manager.GetEnlistedDomainObjects())
      {
        enlistedObjects.Add(enlistedDomainObject);
        if (enlistedDomainObject == orderA)
          _manager.DisenlistDomainObject(orderB);
      }

      Assert.That(enlistedObjects, Is.EqualTo(new[] { orderA }));
    }

    [Test]
    public void GetEnlistedDomainObject ()
    {
      _manager.EnlistDomainObject(_order);

      Assert.That(_manager.GetEnlistedDomainObject(_order.ID), Is.SameAs(_order));
    }

    [Test]
    public void GetEnlistedDomainObject_NotEnlisted ()
    {
      Assert.That(_manager.GetEnlistedDomainObject(_order.ID), Is.Null);
    }
  }
}
