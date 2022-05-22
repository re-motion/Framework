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
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation.Transport;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Transport
{
  [TestFixture]
  public class DomainObjectTransporterTest : StandardMappingTest
  {
    private DomainObjectTransporter _transporter;
    private MemoryStream _stream;

    public override void SetUp ()
    {
      base.SetUp();
      _transporter = new DomainObjectTransporter();
      _stream = new MemoryStream();
    }

    public override void TearDown ()
    {
      _stream.Dispose();
      base.TearDown();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_transporter.ObjectIDs.Count, Is.EqualTo(0));
      Assert.That(_transporter.ObjectIDs, Is.Empty);
    }

    [Test]
    public void Load ()
    {
      IDomainObject domainObject = _transporter.Load(DomainObjectIDs.Order1);
      Assert.That(_transporter.ObjectIDs.Count, Is.EqualTo(1));
      Assert.That(_transporter.ObjectIDs, Is.EqualTo(new[] { DomainObjectIDs.Order1 }));

      Assert.That(_transporter.GetTransportedObject(domainObject.ID), Is.SameAs(domainObject));
    }

    [Test]
    public void Load_Twice ()
    {
      IDomainObject domainObject1 = _transporter.Load(DomainObjectIDs.Order1);
      IDomainObject domainObject2 = _transporter.Load(DomainObjectIDs.Order1);
      Assert.That(_transporter.ObjectIDs.Count, Is.EqualTo(1));
      Assert.That(_transporter.ObjectIDs, Is.EqualTo(new[] { DomainObjectIDs.Order1 }));
      Assert.That(domainObject2, Is.SameAs(domainObject1));
    }

    [Test]
    public void Load_Inexistent ()
    {
      Assert.That(
          () => _transporter.Load(new ObjectID(typeof(Order), Guid.NewGuid())),
          Throws.InstanceOf<ObjectsNotFoundException>()
              .With.Message.Matches(
                  "Object 'Order|.*|System.Guid' could not be found."));
    }

    [Test]
    public void Load_Multiple ()
    {
      _transporter.Load(DomainObjectIDs.Order1);
      Assert.That(_transporter.ObjectIDs.Count, Is.EqualTo(1));
      _transporter.Load(DomainObjectIDs.Order3);
      Assert.That(_transporter.ObjectIDs.Count, Is.EqualTo(2));
      _transporter.Load(DomainObjectIDs.OrderItem1);
      Assert.That(_transporter.ObjectIDs.Count, Is.EqualTo(3));
      Assert.That(_transporter.ObjectIDs, Is.EqualTo(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.OrderItem1 }));
    }

    [Test]
    public void LoadWithRelatedObjects ()
    {
      IEnumerable<IDomainObject> loadedObjects = _transporter.LoadWithRelatedObjects(DomainObjectIDs.Order1);
      Assert.That(_transporter.ObjectIDs.Count, Is.EqualTo(6));
      Assert.That(
          _transporter.ObjectIDs,
          Is.EquivalentTo(
              new[]
              {
                  DomainObjectIDs.Order1, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2,
                  DomainObjectIDs.OrderTicket1, DomainObjectIDs.Customer1, DomainObjectIDs.Official1
              }));
      Assert.That(
          loadedObjects.ToArray(),
          Is.EquivalentTo(
              new object[]
              {
                  _transporter.GetTransportedObject(DomainObjectIDs.Order1), _transporter.GetTransportedObject(DomainObjectIDs.OrderItem1),
                  _transporter.GetTransportedObject(DomainObjectIDs.OrderItem2),
                  _transporter.GetTransportedObject(DomainObjectIDs.OrderTicket1), _transporter.GetTransportedObject(DomainObjectIDs.Customer1),
                  _transporter.GetTransportedObject(DomainObjectIDs.Official1)
              }));
    }

    [Test]
    public void LoadRecursive ()
    {
      IEnumerable<IDomainObject> loadedObjects = _transporter.LoadRecursive(DomainObjectIDs.Employee1);
      Assert.That(_transporter.ObjectIDs.Count, Is.EqualTo(5));
      Assert.That(
          _transporter.ObjectIDs,
          Is.EquivalentTo(
              new[]
              {
                  DomainObjectIDs.Employee1, DomainObjectIDs.Employee4, DomainObjectIDs.Computer2,
                  DomainObjectIDs.Employee5, DomainObjectIDs.Computer3
              }));
      Assert.That(
          loadedObjects.ToArray(),
          Is.EquivalentTo(
              new object[]
              {
                  _transporter.GetTransportedObject(DomainObjectIDs.Employee1), _transporter.GetTransportedObject(DomainObjectIDs.Employee4),
                  _transporter.GetTransportedObject(DomainObjectIDs.Computer2),
                  _transporter.GetTransportedObject(DomainObjectIDs.Employee5), _transporter.GetTransportedObject(DomainObjectIDs.Computer3)
              }));
    }

    [Test]
    public void LoadRecursive_WithStrategy_ShouldFollow ()
    {
      var strategy = new FollowOnlyOneLevelStrategy();
      IEnumerable<IDomainObject> loadedObjects = _transporter.LoadRecursive(DomainObjectIDs.Employee1, strategy);
      Assert.That(
          _transporter.ObjectIDs, Is.EquivalentTo(new[] { DomainObjectIDs.Employee1, DomainObjectIDs.Employee4, DomainObjectIDs.Employee5 }));
      Assert.That(
          loadedObjects.ToArray(),
          Is.EquivalentTo(
              new object[]
              {
                  _transporter.GetTransportedObject(DomainObjectIDs.Employee1), _transporter.GetTransportedObject(DomainObjectIDs.Employee4),
                  _transporter.GetTransportedObject(DomainObjectIDs.Employee5)
              }));
    }

    [Test]
    public void LoadRecursive_WithStrategy_ShouldProcess ()
    {
      var strategy = new OnlyProcessComputersStrategy();
      IEnumerable<IDomainObject> loadedObjects = _transporter.LoadRecursive(DomainObjectIDs.Employee1, strategy);
      Assert.That(_transporter.ObjectIDs, Is.EquivalentTo(new[] { DomainObjectIDs.Computer2, DomainObjectIDs.Computer3 }));
      Assert.That(
          loadedObjects.ToArray(),
          Is.EquivalentTo(
              new object[]
              {
                  _transporter.GetTransportedObject(DomainObjectIDs.Computer2), _transporter.GetTransportedObject(DomainObjectIDs.Computer3)
              }));
    }

    [Test]
    public void LoadNew ()
    {
      var order = (Order)_transporter.LoadNew(typeof(Order), ParamList.Empty);
      Assert.That(order, Is.Not.Null);
      Assert.That(_transporter.IsLoaded(order.ID), Is.True);
    }

    [Test]
    public void LoadTransportData ()
    {
      _transporter.Load(DomainObjectIDs.Employee1);
      _transporter.Load(DomainObjectIDs.Employee2);

      TransportedDomainObjects transportedObjects = ExportAndLoadTransportData();

      Assert.That(transportedObjects, Is.Not.Null);
      var domainObjects = new List<IDomainObject>(transportedObjects.TransportedObjects);
      Assert.That(domainObjects.Count, Is.EqualTo(2));
      Assert.That(domainObjects.ConvertAll(obj => obj.ID), Is.EquivalentTo(new[] { DomainObjectIDs.Employee1, DomainObjectIDs.Employee2 }));
    }

    [Test]
    public void LoadTransportData_XmlStrategy ()
    {
      _transporter.Load(DomainObjectIDs.Employee1);
      _transporter.Load(DomainObjectIDs.Employee2);

      TransportedDomainObjects transportedObjects = ExportAndLoadTransportData(XmlImportStrategy.Instance, XmlExportStrategy.Instance);

      Assert.That(transportedObjects, Is.Not.Null);
      var domainObjects = new List<IDomainObject>(transportedObjects.TransportedObjects);
      Assert.That(domainObjects.Count, Is.EqualTo(2));
      Assert.That(domainObjects.ConvertAll(obj => obj.ID), Is.EquivalentTo(new[] { DomainObjectIDs.Employee1, DomainObjectIDs.Employee2 }));
    }

    [Test]
    public void LoadTransportData_InvalidData ()
    {
      using (var stream = new MemoryStream(new byte[] { 1, 2, 3 }))
      {
        Assert.That(
            () => DomainObjectTransporter.LoadTransportData(stream),
            Throws.InstanceOf<TransportationException>()
                .With.Message.EqualTo("Invalid data specified: End of Stream encountered before parsing was completed."));
      }
    }

    [Test]
    public void IsLoaded_True ()
    {
      _transporter.Load(DomainObjectIDs.Employee1);
      Assert.That(_transporter.IsLoaded(DomainObjectIDs.Employee1), Is.True);
    }

    [Test]
    public void IsLoaded_False ()
    {
      Assert.That(_transporter.IsLoaded(DomainObjectIDs.Employee1), Is.False);
    }

    [Test]
    public void TransactionContainsMoreObjects_ThanAreTransported ()
    {
      _transporter.LoadRecursive(DomainObjectIDs.Employee1, new FollowAllProcessNoneStrategy());
      Assert.That(_transporter.ObjectIDs.Count, Is.EqualTo(0));
      Assert.That(new List<ObjectID>(_transporter.ObjectIDs), Is.Empty);

      TransportedDomainObjects transportedObjects = ExportAndLoadTransportData();
      Assert.That(transportedObjects.TransportedObjects, Is.Empty);
    }

    [Test]
    public void Export ()
    {
      _transporter.Load(DomainObjectIDs.Order1);
      _transporter.Export(_stream);
      Assert.That(_stream.Position > 0);
    }

    [Test]
    public void Export_SpecialStrategy ()
    {
      IDomainObject loadedObject1 = _transporter.Load(DomainObjectIDs.Order1);
      IDomainObject loadedObject2 = _transporter.Load(DomainObjectIDs.Order3);

      var strategyMock = new Mock<IExportStrategy>(MockBehavior.Strict);

      strategyMock
          .Setup(mock => mock.Export(_stream, It.Is<TransportItem[]>(items => items.Length == 2 && items[0].ID == loadedObject1.ID && items[1].ID == loadedObject2.ID)))
          .Verifiable();

      _transporter.Export(_stream, strategyMock.Object);
      strategyMock.Verify();
    }

    [Test]
    public void GetTransportedObject_ReturnsCorrectObject ()
    {
      _transporter.Load(DomainObjectIDs.Order1);
      var order = (Order)_transporter.GetTransportedObject(DomainObjectIDs.Order1);
      Assert.That(order, Is.Not.Null);
      Assert.That(order.ID, Is.EqualTo(DomainObjectIDs.Order1));
    }

    [Test]
    public void GetTransportedObject_ThrowsOnUnloadedObject ()
    {
      Assert.That(
          () => _transporter.GetTransportedObject(DomainObjectIDs.Order1),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be "
                  +
                  "retrieved, it hasn't been loaded yet. Load it first, then retrieve it for editing.", "loadedObjectID"
));
    }

    [Test]
    public void GetTransportedObject_ReturnsObjectBoundToTransportTransaction ()
    {
      _transporter.Load(DomainObjectIDs.Order1);
      var order = (Order)_transporter.GetTransportedObject(DomainObjectIDs.Order1);
      Assert.That(order.RootTransaction, Is.SameAs(PrivateInvoke.GetNonPublicField(_transporter, "_transportTransaction")));
    }

    [Test]
    public void GetTransportedObject_GetSetPropertyValue ()
    {
      _transporter.Load(DomainObjectIDs.Order1);
      var order = (Order)_transporter.GetTransportedObject(DomainObjectIDs.Order1);
      ++order.OrderNumber;
      Assert.That(order.OrderNumber, Is.EqualTo(2));
    }

    [Test]
    public void GetTransportedObject_GetSetRelatedObject_RealSide ()
    {
      _transporter.Load(DomainObjectIDs.Computer1);
      var computer = (Computer)_transporter.GetTransportedObject(DomainObjectIDs.Computer1);
      computer.Employee = null;
      Assert.That(computer.Employee, Is.Null);
    }

    [Test]
    public void GetTransportedObject_GetSetRelatedObject_VirtualSide_Loaded ()
    {
      _transporter.Load(DomainObjectIDs.Computer1);
      _transporter.Load(DomainObjectIDs.Employee3);
      var employee = (Employee)_transporter.GetTransportedObject(DomainObjectIDs.Employee3);
      employee.Computer = null;
      Assert.That(employee.Computer, Is.Null);
    }

    [Test]
    public void GetTransportedObject_GetSetRelatedObject_VirtualSide_Unloaded ()
    {
      _transporter.Load(DomainObjectIDs.Employee3);
      var employee = (Employee)_transporter.GetTransportedObject(DomainObjectIDs.Employee3);
      Assert.That(
          () => employee.Computer = null,
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Object 'Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid' "
                  +
                  "cannot be modified for transportation because it hasn't been loaded yet. Load it before manipulating it."
));
    }

    private TransportedDomainObjects ExportAndLoadTransportData ()
    {
      _transporter.Export(_stream);
      _stream.Seek(0, SeekOrigin.Begin);
      return DomainObjectTransporter.LoadTransportData(_stream);
    }

    private TransportedDomainObjects ExportAndLoadTransportData (IImportStrategy importStrategy, IExportStrategy exportStrategy)
    {
      _transporter.Export(_stream, exportStrategy);
      _stream.Seek(0, SeekOrigin.Begin);
      return DomainObjectTransporter.LoadTransportData(_stream, importStrategy);
    }
  }
}
