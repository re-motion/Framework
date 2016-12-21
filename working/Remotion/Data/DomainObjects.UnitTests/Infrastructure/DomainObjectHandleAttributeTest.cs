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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class DomainObjectHandleAttributeTest : StandardMappingTest
  {
    private DomainObjectHandleAttribute _attribute;

    public override void SetUp ()
    {
      base.SetUp ();
      _attribute = new DomainObjectHandleAttribute ();
    }

    [Test]
    public void GetReferencedType_ReturnsDomainObjectHandleTypeParameter ()
    {
      var result = _attribute.GetReferencedType (typeof (IDomainObjectHandle<Order>));

      Assert.That (result, Is.SameAs (typeof (Order)));
    }

    [Test]
    public void GetReferencedType_NonGenericType_Throws ()
    {
      Assert.That (
          () => _attribute.GetReferencedType (typeof (Order)),
          Throws.ArgumentException.With.Message.EqualTo (
              "The handleType parameter must be an instantiation of 'IDomainObjectHandle<T>'.\r\nParameter name: handleType"));
    }

    [Test]
    public void GetReferencedType_WrongGenericType_Throws ()
    {
      Assert.That (
          () => _attribute.GetReferencedType (typeof (List<Order>)),
          Throws.ArgumentException.With.Message.EqualTo (
              "The handleType parameter must be an instantiation of 'IDomainObjectHandle<T>'.\r\nParameter name: handleType"));
    }

    [Test]
    public void GetReferencedType_DomainObjectHandleClass_Throws ()
    {
      Assert.That (
          () => _attribute.GetReferencedType (typeof (DomainObjectHandle<Order>)),
          Throws.ArgumentException,
          "It's not recommended to use the DomainObjectHandle<T> class directly, the interface should be used instead.");
    }

    [Test]
    public void GetReferencedInstance_ReturnsObjectFromCurrentTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var order = Order.NewObject();
        var handle = order.GetHandle();

        var result = _attribute.GetReferencedInstance (handle);

        Assert.That (result, Is.SameAs (order));
      }
    }

    [Test]
    public void GetReferencedInstance_DeletedInstance_Works ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var order = DomainObjectMother.GetDeletedObject (ClientTransaction.Current, DomainObjectIDs.Order1);
        var handle = order.GetHandle();

        var result = _attribute.GetReferencedInstance (handle);

        Assert.That (result, Is.SameAs (order));
      }
    }

    [Test]
    public void GetReferencedInstance_NoHandleInstance ()
    {
      Assert.That (
          () => _attribute.GetReferencedInstance (new object()),
          Throws.ArgumentException.With.Message.EqualTo (
              "Parameter 'handleInstance' has type 'System.Object' "
              + "when type 'Remotion.Data.DomainObjects.IDomainObjectHandle`1[Remotion.Data.DomainObjects.DomainObject]' was expected."
              + "\r\nParameter name: handleInstance"));
    }

    [Test]
    public void GetReferencedInstance_NoTransaction ()
    {
      var handle = DomainObjectIDs.Order1.GetHandle<Order>();

      Assert.That (
          () => _attribute.GetReferencedInstance (handle),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }
  }
}