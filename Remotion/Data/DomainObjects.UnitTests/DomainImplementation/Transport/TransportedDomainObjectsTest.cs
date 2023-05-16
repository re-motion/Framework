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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.DomainImplementation.Transport;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Transport
{
  [TestFixture]
  public class TransportedDomainObjectsTest : StandardMappingTest
  {
    [Test]
    public void EmptyTransport ()
    {
      ClientTransaction dataTransaction = ClientTransaction.CreateRootTransaction();
      var transportedObjects = new TransportedDomainObjects(dataTransaction, new List<DomainObject>());

      Assert.That(transportedObjects.DataTransaction, Is.Not.Null);
      Assert.That(transportedObjects.DataTransaction, Is.SameAs(dataTransaction));
      Assert.That(GetTransportedObjects(transportedObjects), Is.Empty);
    }

    [Test]
    public void TransportedObjectsStayConstant_WhenTransactionIsManipulated ()
    {
      var transportedObjects = new TransportedDomainObjects(ClientTransaction.CreateRootTransaction(), new List<DomainObject>());

      Assert.That(GetTransportedObjects(transportedObjects), Is.Empty);

      using (transportedObjects.DataTransaction.EnterNonDiscardingScope())
      {
        DomainObjectIDs.Order1.GetObject<Order>();
      }

      Assert.That(GetTransportedObjects(transportedObjects), Is.Empty);
    }

    [Test]
    public void NonEmptyTransport ()
    {
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction();
      var transportedObjectList = new List<DomainObject>();
      using (newTransaction.EnterNonDiscardingScope())
      {
        transportedObjectList.Add(DomainObjectIDs.Order1.GetObject<Order>());
        transportedObjectList.Add(DomainObjectIDs.Order3.GetObject<Order>());
        transportedObjectList.Add(DomainObjectIDs.Company1.GetObject<Company>());
      }

      var transportedObjects = new TransportedDomainObjects(newTransaction, transportedObjectList);

      Assert.That(transportedObjects.DataTransaction, Is.Not.Null);
      Assert.IsNotEmpty(GetTransportedObjects(transportedObjects));
      List<ObjectID> ids = GetTransportedObjects(transportedObjects).ConvertAll(obj => obj.ID);
      Assert.That(ids, Is.EquivalentTo(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Company1 }));
    }

    [Test]
    public void FinishTransport_CallsCommit ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects(
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);
      var extensionMock = new Mock<IClientTransactionExtension>(MockBehavior.Strict);

      // initialize mandatory transaction-only properties, which are null after loading the object
      foreach (var transportedObject in transportedObjects.TransportedObjects)
        InitializeTransactionOnlyProperties(transportedObject);

      extensionMock
          .Setup(
              mock => mock.Committing(
                  transportedObjects.DataTransaction,
                  It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(GetTransportedObjects(transportedObjects))),
                  It.IsAny<ICommittingEventRegistrar>()))
          .Verifiable();
      extensionMock
          .Setup(
              mock => mock.CommitValidate(
                  transportedObjects.DataTransaction,
                  It.Is<ReadOnlyCollection<PersistableData>>(c => c.Select(d => d.DomainObject).SetEquals(GetTransportedObjects(transportedObjects)))))
          .Verifiable();
      extensionMock
          .Setup(
              mock => mock.Committed(
                  transportedObjects.DataTransaction,
                  It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(GetTransportedObjects(transportedObjects)))))
          .Verifiable();
      extensionMock.Setup(mock => mock.TransactionDiscard(transportedObjects.DataTransaction)).Verifiable();

      extensionMock.Setup(stub => stub.Key).Returns("mock");

      transportedObjects.DataTransaction.Extensions.Add(extensionMock.Object);
      transportedObjects.FinishTransport();

      extensionMock.Verify();
    }

    [Test]
    public void FinishTransport_FilterCalledForEachChangedObject ()
    {
      var transporter = new DomainObjectTransporter();
      transporter.Load(DomainObjectIDs.ClassWithAllDataTypes1);
      transporter.Load(DomainObjectIDs.ClassWithAllDataTypes2);
      transporter.Load(DomainObjectIDs.Order1);

      ModifyDatabase(
          delegate
          {
            ClassWithAllDataTypes c1 = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
            ClassWithAllDataTypes c2 = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes>();
            c1.Delete();
            c2.Delete();
          });

      TransportedDomainObjects transportedObjects = Transport(transporter);

      // initialize mandatory transaction-only properties, which are null after loading the object
      foreach (var transportedObject in transportedObjects.TransportedObjects)
        InitializeTransactionOnlyProperties(transportedObject);

      var expectedObjects = new List<DomainObject>();
      using (transportedObjects.DataTransaction.EnterNonDiscardingScope())
      {
        expectedObjects.Add(DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>());
        expectedObjects.Add(DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes>());
      }

      // initialize mandatory transaction-only properties, which are null after loading the object
      foreach (var transportedObject in transportedObjects.TransportedObjects)
        InitializeTransactionOnlyProperties(transportedObject);

      var filteredObjects = new List<DomainObject>();
      transportedObjects.FinishTransport(
          delegate (DomainObject domainObject)
          {
            filteredObjects.Add(domainObject);
            return true;
          });
      Assert.That(filteredObjects, Is.EquivalentTo(expectedObjects));
    }

    [Test]
    public void FinishTransport_WithoutFilter ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects(
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      // initialize mandatory transaction-only properties, which are null after loading the object
      foreach (var transportedObject in transportedObjects.TransportedObjects)
        InitializeTransactionOnlyProperties(transportedObject);

      transportedObjects.FinishTransport();

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        ClassWithAllDataTypes c3 = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
        ClassWithAllDataTypes c4 = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes>();

        Assert.That(c3.Int32Property, Is.EqualTo(2147483647));
        Assert.That(c4.Int32Property, Is.EqualTo(-2147483647));
      }
    }

    [Test]
    public void FinishTransport_FilterNew ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects(
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      // initialize mandatory transaction-only properties, which are null after loading the object
      foreach (var transportedObject in transportedObjects.TransportedObjects)
        InitializeTransactionOnlyProperties(transportedObject);

      transportedObjects.FinishTransport(transportedObject =>
          {
            Assert.That(transportedObject.State.IsNew);
            return ((ClassWithAllDataTypes)transportedObject).Int32Property < 0;
          });

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        try
        {
          DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
          Assert.Fail("Expected ObjectsNotFoundException");
        }
        catch (ObjectsNotFoundException)
        {
          // ok
        }

        ClassWithAllDataTypes c2 = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes>();
        Assert.That(c2.Int32Property, Is.EqualTo(-2147483647));
      }
    }

    [Test]
    public void FinishTransport_FilterExisting ()
    {
      TransportedDomainObjects transportedObjects = TransportAndChangeObjects(
          typeof(ClassWithAllDataTypes).FullName + ".Int32Property",
          42,
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      // initialize mandatory transaction-only properties, which are null after loading the object
      foreach (var transportedObject in transportedObjects.TransportedObjects)
        InitializeTransactionOnlyProperties(transportedObject);

      transportedObjects.FinishTransport(transportedObject =>
      {
        Assert.That(transportedObject.State.IsChanged);
        return ((ClassWithAllDataTypes)transportedObject).BooleanProperty;
      });

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        var c1 = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
        Assert.That(c1.Int32Property, Is.EqualTo(42));

        var c2 = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes>();
        Assert.That(c2.Int32Property, Is.Not.EqualTo(42));
      }
    }

    [Test]
    public void FinishTransport_ClearsTransportedObjects ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects(
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport(delegate { return false; });
      Assert.That(transportedObjects.DataTransaction, Is.Null);
      Assert.That(transportedObjects.TransportedObjects, Is.Null);
    }

    [Test]
    public void FinishTransport_DiscardsTransaction ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects(
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      var transaction = transportedObjects.DataTransaction;

      // initialize mandatory transaction-only properties, which are null after loading the object
      foreach (var transportedObject in transportedObjects.TransportedObjects)
        InitializeTransactionOnlyProperties(transportedObject);

      transportedObjects.FinishTransport();

      Assert.That(transaction.IsDiscarded, Is.True);
    }

    [Test]
    public void FinishTransport_WithInactiveTTransaction ()
    {
      var dataTransaction = ClientTransaction.CreateRootTransaction();
      var transportedObjects = new TransportedDomainObjects(dataTransaction, new List<DomainObject>());

      using (ClientTransactionTestHelper.MakeInactive(dataTransaction))
      {
        Assert.That(
            () => transportedObjects.FinishTransport(),
            Throws.TypeOf<ClientTransactionReadOnlyException>());
      }
    }

    [Test]
    public void FinishTransport_Twice ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects(
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport(delegate { return false; });
      Assert.That(
          () => transportedObjects.FinishTransport(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "FinishTransport can only be called once."));
    }

    private TransportedDomainObjects TransportAndDeleteObjects (params ObjectID[] objectsToLoadAndDelete)
    {
      var transporter = new DomainObjectTransporter();
      foreach (ObjectID id in objectsToLoadAndDelete)
        transporter.Load(id);

      ModifyDatabase(
          delegate
          {
            foreach (var id in objectsToLoadAndDelete)
            {
              var domainObject = LifetimeService.GetObject(ClientTransaction.Current, id, false);
              LifetimeService.DeleteObject(ClientTransaction.Current, domainObject);
            }
          });

      return Transport(transporter);
    }

    private TransportedDomainObjects TransportAndChangeObjects (string propertyName, object newValue, params ObjectID[] objectsToLoadAndDelete)
    {
      var transporter = new DomainObjectTransporter();
      foreach (ObjectID id in objectsToLoadAndDelete)
        transporter.Load(id);

      ModifyDatabase(
          delegate
          {
            foreach (var id in objectsToLoadAndDelete)
            {
              var domainObject = LifetimeService.GetObject(ClientTransaction.Current, id, false);

              InitializeTransactionOnlyProperties(domainObject);

              var properties = new PropertyIndexer(domainObject);
              properties[propertyName].SetValueWithoutTypeCheck(newValue);
            }
          });

      return Transport(transporter);
    }

    private List<DomainObject> GetTransportedObjects (TransportedDomainObjects transportedObjects)
    {
      return new List<DomainObject>(transportedObjects.TransportedObjects);
    }

    private void ModifyDatabase (Action changer)
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        changer();
        ClientTransaction.Current.Commit();
      }
    }

    private TransportedDomainObjects Transport (DomainObjectTransporter transporter)
    {
      using (var stream = new MemoryStream())
      {
        transporter.Export(stream);
        stream.Seek(0, SeekOrigin.Begin);

        return DomainObjectTransporter.LoadTransportData(stream);
      }
    }

    private static void InitializeTransactionOnlyProperties (DomainObject domainObject)
    {
      if (domainObject is ClassWithAllDataTypes objectWithAllDataTypes)
      {
        objectWithAllDataTypes.TransactionOnlyStringProperty = "TransactionOnly";
        objectWithAllDataTypes.TransactionOnlyBinaryProperty = new byte[] { 08, 15 };
      }
    }
  }
}
