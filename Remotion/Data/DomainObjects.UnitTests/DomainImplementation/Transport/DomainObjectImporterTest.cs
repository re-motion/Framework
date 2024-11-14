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
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.DomainImplementation.Transport;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Transport
{
  [TestFixture]
  public class DomainObjectImporterTest : StandardMappingTest
  {
    [Test]
    public void EmptyTransport ()
    {
      var imported = DomainObjectTransporterTestHelper.ImportObjects();
      Assert.That(imported, Is.Empty);
    }

    [Test]
    public void NonEmptyTransport ()
    {
      var loadedIDs = new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Company1 };
      var imported = DomainObjectTransporterTestHelper.ImportObjects(loadedIDs);

      Assert.That(imported, Is.Not.Empty);
      List<ObjectID> ids = imported.ConvertAll(obj => obj.ID);
      Assert.That(ids, Is.EquivalentTo(loadedIDs));
    }

    [Test]
    public void NonEmptyTransport_ObjectsBoundToTransaction ()
    {
      var loadedIDs = new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Company1 };
      var data = DomainObjectTransporterTestHelper.GetBinaryDataFor(loadedIDs);

      TransportedDomainObjects transportedObjects = DomainObjectTransporterTestHelper.Import(data);
      foreach (DomainObject domainObject in transportedObjects.TransportedObjects)
      {
        Assert.That(domainObject.RootTransaction, Is.SameAs(transportedObjects.DataTransaction));
      }
    }

    [Test]
    public void InvalidData ()
    {
      Assert.That(
          () => DomainObjectTransporterTestHelper.Import(new byte[] { 1, 2, 3 }),
          Throws.InstanceOf<TransportationException>()
              .With.Message.EqualTo("Invalid data specified: There is an error in XML document (1, 1)."));
    }

    [Test]
    public void NonExistingObjects_PreparedForUse ()
    {
      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(DomainObjectIDs.ClassWithAllDataTypes1);
      ModifyDatabase(() => DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>().Delete());

      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);

      var importedInstance = (ClassWithAllDataTypes)imported[0];
      var pipeline = ((DomainObjectCreator)importedInstance.ID.ClassDefinition.InstanceCreator).PipelineRegistry.DefaultPipeline;
      Assert.That(pipeline.ReflectionService.IsAssembledType(((object)importedInstance).GetType()), Is.True, "creator should be used");
      Assert.That(importedInstance.ID, Is.EqualTo(DomainObjectIDs.ClassWithAllDataTypes1), "ID should be copied");

      var importTransaction = importedInstance.RootTransaction;
      Assert.That(importedInstance.InternalDataContainer.ClientTransaction, Is.SameAs(importTransaction), "DataContainer should be registered");
      Assert.That(importedInstance.InternalDataContainer.DomainObject, Is.SameAs(importedInstance), "DataContainer should have DomainObject set");

      Assert.That(importTransaction.IsEnlisted(importedInstance), Is.True, "DomainObject should be enlisted");
    }

    [Test]
    public void NonExistingObjects_New ()
    {
      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(DomainObjectIDs.ClassWithAllDataTypes1);
      ModifyDatabase(() => DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>().Delete());

      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);
      Assert.That(imported[0].State.IsNew, Is.True);
    }

    [Test]
    public void NonExistingObjects_ChangedBySource ()
    {
      byte[] binaryData = GetBinaryDataForChangedObject(
          DomainObjectIDs.ClassWithAllDataTypes1,
          ReflectionMappingHelper.GetPropertyName(typeof(ClassWithAllDataTypes), "Int32Property"),
          12);
      ModifyDatabase(() => DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>().Delete());

      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);
      Assert.That(imported[0].State.IsNew, Is.True);
      Assert.That(((ClassWithAllDataTypes)imported[0]).Int32Property, Is.EqualTo(12));
    }

    [Test]
    public void NonExistingObjects_NewInSource ()
    {
      var transporter = new DomainObjectTransporter();
      var outerComputer = (Computer)transporter.LoadNew(typeof(Computer), ParamList.Empty);
      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(transporter);

      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);

      Assert.That(imported[0].State.IsNew, Is.True);
      Assert.That(imported[0].ID, Is.EqualTo(outerComputer.ID));
    }

    [Test]
    public void ExistingObjects_Loaded ()
    {
      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(DomainObjectIDs.Order1);
      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);

      Assert.That(imported[0].State.IsUnchanged, Is.True);
    }

    [Test]
    public void ExistingObjects_ChangedIfNecessary ()
    {
      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(DomainObjectIDs.Order1, DomainObjectIDs.Order3);
      ModifyDatabase(delegate { DomainObjectIDs.Order1.GetObject<Order>().OrderNumber++; });

      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);

      Assert.That(imported[0].State.IsChanged, Is.True);
      Assert.That(imported[1].State.IsUnchanged, Is.True);
    }

    [Test]
    public void ExistingObjects_ChangedBySource ()
    {
      byte[] binaryData = GetBinaryDataForChangedObject(
          DomainObjectIDs.ClassWithAllDataTypes1,
          ReflectionMappingHelper.GetPropertyName(typeof(ClassWithAllDataTypes), "Int32Property"),
          12);

      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);

      Assert.That(imported[0].State.IsChanged, Is.True);
      Assert.That(((ClassWithAllDataTypes)imported[0]).Int32Property, Is.EqualTo(12));
    }

    [Test]
    public void SimplePropertyChanges ()
    {
      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(DomainObjectIDs.Order1);
      ModifyDatabase(delegate { DomainObjectIDs.Order1.GetObject<Order>().OrderNumber = 13; });

      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);
      Assert.That(((Order)imported[0]).Properties[typeof(Order), "OrderNumber"].HasChanged, Is.True);
      Assert.That(((Order)imported[0]).OrderNumber, Is.EqualTo(1));
      Assert.That(((Order)imported[0]).Properties[typeof(Order), "DeliveryDate"].HasChanged, Is.False);
    }

    [Test]
    public void RelatedObjectChanges_RealSide ()
    {
      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Computer3);
      ModifyDatabase(
          delegate
          {
            DomainObjectIDs.Computer1.GetObject<Computer>().Employee = null;
            DomainObjectIDs.Computer2.GetObject<Computer>().Employee = DomainObjectIDs.Employee1.GetObject<Employee>();
          });

      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);
      var loadedObject1 = (Computer)imported[0];
      var loadedObject2 = (Computer)imported[1];
      var loadedObject3 = (Computer)imported[2];

      Assert.That(loadedObject1.Properties[typeof(Computer), "Employee"].HasChanged, Is.True);
      Assert.That(loadedObject2.Properties[typeof(Computer), "Employee"].HasChanged, Is.True);
      Assert.That(loadedObject3.Properties[typeof(Computer), "Employee"].HasChanged, Is.False);

      Assert.That(loadedObject1.Employee, Is.EqualTo(DomainObjectIDs.Employee3.GetObject<Employee>(loadedObject1.RootTransaction)));
      Assert.That(loadedObject2.Employee, Is.EqualTo(DomainObjectIDs.Employee4.GetObject<Employee>(loadedObject1.RootTransaction)));
      Assert.That(loadedObject3.Employee, Is.EqualTo(DomainObjectIDs.Employee5.GetObject<Employee>(loadedObject1.RootTransaction)));
    }

    [Test]
    public void RelatedObjectChanges_ToNull_RealSide ()
    {
      ModifyDatabase(delegate { DomainObjectIDs.Computer1.GetObject<Computer>().Employee = null; });

      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(DomainObjectIDs.Computer1);
      ModifyDatabase(delegate
      {
        DomainObjectIDs.Computer1.GetObject<Computer>().Employee = DomainObjectIDs.Employee3.GetObject<Employee>();
      });

      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);
      var loadedObject1 = (Computer)imported[0];

      Assert.That(loadedObject1.Properties[typeof(Computer), "Employee"].HasChanged, Is.True);
      Assert.That(loadedObject1.Employee, Is.Null);
    }

    [Test]
    public void RelatedObjectChanges_NonExistentObject_RealSide ()
    {
      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(DomainObjectIDs.Computer1);
      ModifyDatabase(() => DomainObjectIDs.Computer1.GetObject<Computer>().Employee.Delete());

      Assert.That(
          () => DomainObjectTransporterTestHelper.Import(binaryData),
          Throws.InstanceOf<ObjectsNotFoundException>()
              .With.Message.EqualTo("Object(s) could not be found: 'Employee|3c4f3fc8-0db2-4c1f-aa00-ade72e9edb32|System.Guid'."));
    }

    [Test]
    public void RelatedObjectChanges_VirtualSide ()
    {
      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(DomainObjectIDs.Employee3);
      ModifyDatabase(delegate { DomainObjectIDs.Employee3.GetObject<Employee>().Computer = null; });

      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);

      Assert.That(imported[0].State.IsUnchanged, Is.True);
    }

    [Test]
    public void RelatedObjectCollection_OneSide ()
    {
      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      ModifyDatabase(delegate
      {
        DomainObjectIDs.OrderItem1.GetObject<OrderItem>().Order = DomainObjectIDs.Order3.GetObject<Order>();
      });

      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);
      var loadedObject1 = (OrderItem)imported[0];
      var loadedObject2 = (OrderItem)imported[1];

      Assert.That(loadedObject1.Properties[typeof(OrderItem), "Order"].HasChanged, Is.True);
      Assert.That(loadedObject2.Properties[typeof(OrderItem), "Order"].HasChanged, Is.False);

      Assert.That(loadedObject1.Order, Is.EqualTo(DomainObjectIDs.Order1.GetObject<Order>(loadedObject1.RootTransaction)));
      Assert.That(loadedObject2.Order, Is.EqualTo(DomainObjectIDs.Order1.GetObject<Order>(loadedObject1.RootTransaction)));
    }

    [Test]
    public void RelatedObjectCollection_ManySide ()
    {
      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(DomainObjectIDs.Order1);
      ModifyDatabase(delegate
      {
        DomainObjectIDs.Order1.GetObject<Order>().OrderItems[0].Order = DomainObjectIDs.Order3.GetObject<Order>();
      });

      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);

      Assert.That(((Order)imported[0]).Properties[typeof(Order), "OrderItems"].HasChanged, Is.False);
    }

    [Test]
    public void ChangedBySource_PropertyValue ()
    {
      byte[] binaryData = GetBinaryDataForChangedObject(
          DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName(typeof(Order), "OrderNumber"), 2);

      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);

      Assert.That(((Order)imported[0]).OrderNumber, Is.EqualTo(2));
    }

    [Test]
    public void ChangedBySource_RelatedObjectToExistingObject_RealSide ()
    {
      var transporter = new DomainObjectTransporter();
      transporter.Load(DomainObjectIDs.Computer1);
      transporter.Load(DomainObjectIDs.Computer2);
      transporter.Load(DomainObjectIDs.Employee3);
      transporter.Load(DomainObjectIDs.Employee4);
      var computer = (Computer)transporter.GetTransportedObject(DomainObjectIDs.Computer1);
      computer.Employee = (Employee)transporter.GetTransportedObject(DomainObjectIDs.Employee4);

      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(transporter);
      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);

      var loadedObject1 = (Computer)imported.Find(obj => obj.ID == DomainObjectIDs.Computer1);
      var loadedObject2 = (Employee)imported.Find(obj => obj.ID == DomainObjectIDs.Employee4);
      Assert.That(loadedObject1.Employee, Is.SameAs(loadedObject2));
    }

    [Test]
    public void ChangedBySource_RelatedObjectToExistingObject_VirtualSide ()
    {
      var transporter = new DomainObjectTransporter();
      transporter.Load(DomainObjectIDs.Computer1);
      transporter.Load(DomainObjectIDs.Computer2);
      transporter.Load(DomainObjectIDs.Employee3);
      transporter.Load(DomainObjectIDs.Employee4);
      var employee = (Employee)transporter.GetTransportedObject(DomainObjectIDs.Employee3);
      employee.Computer = (Computer)transporter.GetTransportedObject(DomainObjectIDs.Computer2);

      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(transporter);
      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);
      var loadedObject1 = (Computer)imported.Find(obj => obj.ID == DomainObjectIDs.Computer2);
      var loadedObject2 = (Employee)imported.Find(obj => obj.ID == DomainObjectIDs.Employee3);
      Assert.That(loadedObject2.Computer, Is.SameAs(loadedObject1));
    }

    [Test]
    public void ChangedBySource_RelatedObjectToNew ()
    {
      var transporter = new DomainObjectTransporter();
      var computer = (Computer)transporter.LoadNew(typeof(Computer), ParamList.Empty);
      var employee = (Employee)transporter.LoadNew(typeof(Employee), ParamList.Empty);

      computer.Employee = employee;

      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(transporter);
      var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);

      var loadedObject1 = (Computer)imported.Find(obj => obj is Computer);
      var loadedObject2 = (Employee)imported.Find(obj => obj is Employee);
      Assert.That(loadedObject1.Employee, Is.SameAs(loadedObject2));
    }

    [Test]
    public void SpecialStrategy ()
    {
      TransportItem[] items;
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        items = new[] { TransportItem.PackageDataContainer(DomainObjectIDs.Order1.GetObject<Order>().InternalDataContainer) };
      }

      var strategyMock = new Mock<IImportStrategy>(MockBehavior.Strict);
      var streamFake = new Mock<Stream>();

      strategyMock.Setup(mock => mock.Import(streamFake.Object)).Returns(items).Verifiable();

      var importer = DomainObjectImporter.CreateImporterFromStream(streamFake.Object, strategyMock.Object);
      TransportedDomainObjects result = importer.GetImportedObjects();
      Assert.That(
          result.TransportedObjects,
          Is.EquivalentTo(LifetimeService.GetObjects<Order>(result.DataTransaction, DomainObjectIDs.Order1)));

      strategyMock.Verify();
    }

    [Test]
    public void OnObjectImportedCallback ()
    {
      var transporter = new DomainObjectTransporter();
      var instance = (DomainObjectWithImportCallback)transporter.LoadNew(typeof(DomainObjectWithImportCallback), ParamList.Empty);
      instance.Property = 17;

      byte[] binaryData = DomainObjectTransporterTestHelper.GetBinaryDataFor(transporter);

      var persistenceStrategyMock = new Mock<IFetchEnabledPersistenceStrategy>(MockBehavior.Strict);
      persistenceStrategyMock
          .Setup(mock => mock.LoadObjectData(new[] { instance.ID }))
          .Returns(new[] { (ILoadedObjectData)new FreshlyLoadedObjectData(DataContainerObjectMother.CreateExisting(instance.ID)) });

      using (RootClientTransactionComponentFactoryMixin.CreatePersistenceStrategyScope(persistenceStrategyMock.Object))
      {
        var imported = DomainObjectTransporterTestHelper.ImportObjects(binaryData);

        var importedInstance = (DomainObjectWithImportCallback)imported.Single();
        Assert.That(importedInstance.CallbackCalled, Is.True);
        Assert.That(importedInstance.PropertyValueInCallback, Is.EqualTo(17));
        Assert.That(importedInstance.CallbackTransaction, Is.SameAs(importedInstance.RootTransaction));
      }
    }

    private byte[] GetBinaryDataForChangedObject (ObjectID id, string propertyToTouch, object newValue)
    {
      var transporter = new DomainObjectTransporter();
      transporter.Load(id);
      var domainObject = transporter.GetTransportedObject(id);
      var properties = new PropertyIndexer(domainObject);
      properties[propertyToTouch].SetValueWithoutTypeCheck(newValue);
      return DomainObjectTransporterTestHelper.GetBinaryDataFor(transporter);
    }

    private void ModifyDatabase (Action changer)
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        changer();
        ClientTransaction.Current.Commit();
      }
    }

    [DBTable]
    [IncludeInMappingTestDomain]
    public class DomainObjectWithImportCallback : DomainObject, IDomainObjectImporterCallback
    {
      [StorageClassNone]
      public bool CallbackCalled { get; private set; }
      [StorageClassNone]
      public int PropertyValueInCallback { get; private set; }
      [StorageClassNone]
      public ClientTransaction CallbackTransaction { get; private set; }

      public virtual int Property { get; set; }

      public void OnDomainObjectImported (ClientTransaction importTransaction)
      {
        CallbackCalled = true;
        PropertyValueInCallback = Property;
        CallbackTransaction = importTransaction;
      }
    }

    public class RootClientTransactionComponentFactoryMixin : Mixin<RootClientTransactionComponentFactory>
    {
      public static IDisposable CreatePersistenceStrategyScope (IFetchEnabledPersistenceStrategy persistenceStrategy)
      {
        var mixinConfigurationScope = MixinConfiguration.BuildFromActive()
            .ForClass<RootClientTransactionComponentFactory>()
            .AddMixin<RootClientTransactionComponentFactoryMixin>()
            .EnterScope();
        s_persistenceStrategy = persistenceStrategy;
        return new PostActionDisposableDecorator(mixinConfigurationScope, () => { s_persistenceStrategy = null; });
      }

      [ThreadStatic]
      private static IFetchEnabledPersistenceStrategy s_persistenceStrategy;

      public RootClientTransactionComponentFactoryMixin ()
      {
      }

      [OverrideTarget]
      public IPersistenceStrategy CreatePersistenceStrategy ([UsedImplicitly] ClientTransaction constructedTransaction)
      {
        if (s_persistenceStrategy == null)
          throw new InvalidOperationException("No persistence strategy has been given.");
        return s_persistenceStrategy;
      }
    }
  }
}
