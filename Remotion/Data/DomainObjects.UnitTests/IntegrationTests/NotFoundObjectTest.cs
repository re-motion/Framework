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
using NUnit.Framework.Constraints;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class NotFoundObjectTest : ClientTransactionBaseTest
  {
    private ObjectID _nonExistingObjectID;
    private ObjectID _nonExistingObjectIDForSubtransaction;

    public override void SetUp ()
    {
      base.SetUp();

      _nonExistingObjectID = new ObjectID(typeof(Order), Guid.NewGuid());

      var classWithAllDataTypes = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      classWithAllDataTypes.Delete();
      _nonExistingObjectIDForSubtransaction = classWithAllDataTypes.ID;
    }

    [Test]
    public void GetObject_True_ShouldThrow_AndMarkObjectNotFound ()
    {
      Assert.That(() => _nonExistingObjectID.GetObject<Order>(includeDeleted: true), ThrowsObjectNotFoundException(_nonExistingObjectID));

      CheckObjectIsMarkedInvalid(_nonExistingObjectID);

      // After the object has been marked invalid
      Assert.That(() => _nonExistingObjectID.GetObject<Order>(includeDeleted: true), ThrowsObjectInvalidException(_nonExistingObjectID));
    }

    [Test]
    public void GetObject_False_ShouldThrow_AndMarkObjectNotFound ()
    {
      Assert.That(() => _nonExistingObjectID.GetObject<Order>(includeDeleted: false), ThrowsObjectNotFoundException(_nonExistingObjectID));

      CheckObjectIsMarkedInvalid(_nonExistingObjectID);

      // After the object has been marked invalid
      Assert.That(() => _nonExistingObjectID.GetObject<Order>(includeDeleted: true), ThrowsObjectInvalidException(_nonExistingObjectID));
    }

    [Test]
    public void GetObject_Subtransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(
            () => _nonExistingObjectIDForSubtransaction.GetObject<TestDomainBase>(includeDeleted: true),
            ThrowsObjectInvalidException(_nonExistingObjectIDForSubtransaction));

        CheckObjectIsMarkedInvalid(_nonExistingObjectIDForSubtransaction);

        Assert.That(
            () => _nonExistingObjectID.GetObject<TestDomainBase>(includeDeleted: true),
            ThrowsObjectNotFoundException(_nonExistingObjectID));

        CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      }

      CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      CheckObjectIsNotMarkedInvalid(_nonExistingObjectIDForSubtransaction);
    }

    [Test]
    public void TryGetObject_ShouldReturnNull_AndMarkObjectNotFound ()
    {
      DomainObject instance = null;
      Assert.That(() => instance = _nonExistingObjectID.TryGetObject<TestDomainBase>(), Throws.Nothing);

      Assert.That(instance, Is.Null);
      CheckObjectIsMarkedInvalid(_nonExistingObjectID);

      // After the object has been marked invalid
      Assert.That(() => instance = _nonExistingObjectID.TryGetObject<TestDomainBase>(), Throws.Nothing);
      Assert.That(instance, Is.Not.Null);
      Assert.That(instance.State.IsInvalid, Is.True);
    }

    [Test]
    public void TryGetObject_Subtransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        DomainObject instance = null;
        CheckObjectIsMarkedInvalid(_nonExistingObjectIDForSubtransaction);

        Assert.That(() => instance = _nonExistingObjectIDForSubtransaction.TryGetObject<TestDomainBase>(), Throws.Nothing);

        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.State.IsInvalid, Is.True);

        Assert.That(() => instance = _nonExistingObjectID.TryGetObject<TestDomainBase>(), Throws.Nothing);

        Assert.That(instance, Is.Null);
        CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      }

      CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      CheckObjectIsNotMarkedInvalid(_nonExistingObjectIDForSubtransaction);
    }

    [Test]
    public void GetObjectReference_ShouldGiveNotLoadedYetObject ()
    {
      var instance = LifetimeService.GetObjectReference(TestableClientTransaction, _nonExistingObjectID);
      Assert.That(((DomainObject)instance).State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void EnsureDataAvailable_ShouldThrow_AndMarkObjectNotFound ()
    {
      var instance = LifetimeService.GetObjectReference(TestableClientTransaction, _nonExistingObjectID);
      Assert.That(((DomainObject)instance).State.IsNotLoadedYet, Is.True);

      Assert.That(() => instance.EnsureDataAvailable(), ThrowsObjectNotFoundException(_nonExistingObjectID));

      CheckObjectIsMarkedInvalid(instance.ID);

      // After the object has been marked invalid
      Assert.That(() => instance.EnsureDataAvailable(), ThrowsObjectInvalidException(_nonExistingObjectID));
    }

    [Test]
    public void EnsureDataAvailable_Subtransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var instance = LifetimeService.GetObjectReference(ClientTransaction.Current, _nonExistingObjectIDForSubtransaction);
        CheckObjectIsMarkedInvalid(instance.ID);

        Assert.That(() => instance.EnsureDataAvailable(), ThrowsObjectInvalidException(_nonExistingObjectIDForSubtransaction));

        var instance2 = LifetimeService.GetObjectReference(ClientTransaction.Current, _nonExistingObjectID);
        Assert.That(() => instance2.EnsureDataAvailable(), ThrowsObjectNotFoundException(_nonExistingObjectID));
        CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      }

      CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      CheckObjectIsNotMarkedInvalid(_nonExistingObjectIDForSubtransaction);
    }

    [Test]
    public void EnsureDataAvailable_MultipleObjects_ShouldThrow_AndMarkObjectNotFound ()
    {
      Assert.That(
          () => TestableClientTransaction.EnsureDataAvailable(new[] { _nonExistingObjectID, DomainObjectIDs.Order1 }),
          ThrowsObjectNotFoundException(_nonExistingObjectID));

      CheckObjectIsMarkedInvalid(_nonExistingObjectID);

      // After the object has been marked invalid
      Assert.That(
          () => TestableClientTransaction.EnsureDataAvailable(new[] { _nonExistingObjectID, DomainObjectIDs.Order1 }),
          ThrowsObjectInvalidException(_nonExistingObjectID));
    }

    [Test]
    public void EnsureDataAvailable_MultipleObjects_Subtransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        CheckObjectIsMarkedInvalid(_nonExistingObjectIDForSubtransaction);
        Assert.That(
            () => ClientTransaction.Current.EnsureDataAvailable(new[] { _nonExistingObjectIDForSubtransaction, DomainObjectIDs.Order1 }),
            ThrowsObjectInvalidException(_nonExistingObjectIDForSubtransaction));

        Assert.That(
            () => ClientTransaction.Current.EnsureDataAvailable(new[] { _nonExistingObjectID, DomainObjectIDs.Order3 }),
            ThrowsObjectNotFoundException(_nonExistingObjectID));
        CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      }

      CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      CheckObjectIsNotMarkedInvalid(_nonExistingObjectIDForSubtransaction);
    }

    [Test]
    public void TryEnsureDataAvailable_ShouldReturnFalse_AndMarkObjectNotFound ()
    {
      var instance = LifetimeService.GetObjectReference(TestableClientTransaction, _nonExistingObjectID);
      Assert.That(((DomainObject)instance).State.IsNotLoadedYet, Is.True);

      var result = instance.TryEnsureDataAvailable();

      Assert.That(result, Is.False);
      CheckObjectIsMarkedInvalid(instance.ID);

      // After the object has been marked invalid
      Assert.That(() => instance.TryEnsureDataAvailable(), ThrowsObjectInvalidException(_nonExistingObjectID));
    }

    [Test]
    public void TryEnsureDataAvailable_Subtransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var instance = LifetimeService.GetObjectReference(ClientTransaction.Current, _nonExistingObjectIDForSubtransaction);
        CheckObjectIsMarkedInvalid(instance.ID);

        Assert.That(() => instance.TryEnsureDataAvailable(), ThrowsObjectInvalidException(_nonExistingObjectIDForSubtransaction));

        var instance2 = LifetimeService.GetObjectReference(ClientTransaction.Current, _nonExistingObjectID);

        var result = instance2.TryEnsureDataAvailable();

        Assert.That(result, Is.False);
        CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      }

      CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      CheckObjectIsNotMarkedInvalid(_nonExistingObjectIDForSubtransaction);
    }

    [Test]
    public void TryEnsureDataAvailable_MultipleObjects_ShouldReturnFalse_AndMarkObjectNotFound ()
    {
      var result = TestableClientTransaction.TryEnsureDataAvailable(new[] { _nonExistingObjectID, DomainObjectIDs.Order1 });
      Assert.That(result, Is.False);
      CheckObjectIsMarkedInvalid(_nonExistingObjectID);

      // After the object has been marked invalid
      Assert.That(
          () => TestableClientTransaction.TryEnsureDataAvailable(new[] { _nonExistingObjectID, DomainObjectIDs.Order1 }),
          ThrowsObjectInvalidException(_nonExistingObjectID));
    }

    [Test]
    public void TryEnsureDataAvailable_MultipleObjects_Subtransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        CheckObjectIsMarkedInvalid(_nonExistingObjectIDForSubtransaction);
        Assert.That(
            () => ClientTransaction.Current.TryEnsureDataAvailable(new[] { _nonExistingObjectIDForSubtransaction, DomainObjectIDs.Order1 }),
            ThrowsObjectInvalidException(_nonExistingObjectIDForSubtransaction));

        var result = ClientTransaction.Current.TryEnsureDataAvailable(new[] { _nonExistingObjectID, DomainObjectIDs.Order1 });
        Assert.That(result, Is.False);
        CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      }

      CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      CheckObjectIsNotMarkedInvalid(_nonExistingObjectIDForSubtransaction);
    }

    [Test]
    public void PropertyAccess_ShouldThrow_ValueProperty ()
    {
      var instance = (Order)LifetimeService.GetObjectReference(TestableClientTransaction, _nonExistingObjectID);
      Assert.That(instance.State.IsNotLoadedYet, Is.True);
      Assert.That(() => instance.OrderNumber, ThrowsObjectNotFoundException(_nonExistingObjectID));
      CheckObjectIsMarkedInvalid(instance.ID);

      // After the object has been marked invalid
      Assert.That(() => instance.OrderNumber, ThrowsObjectInvalidException(_nonExistingObjectID));
    }

    [Test]
    public void PropertyAccess_ShouldThrow_VirtualRelationProperty ()
    {
      var instance = (Order)LifetimeService.GetObjectReference(TestableClientTransaction, _nonExistingObjectID);
      Assert.That(instance.State.IsNotLoadedYet, Is.True);
      Assert.That(() => instance.OrderTicket, ThrowsObjectNotFoundException(_nonExistingObjectID));
      CheckObjectIsMarkedInvalid(instance.ID);

      // After the object has been marked invalid
      Assert.That(() => instance.OrderTicket, ThrowsObjectInvalidException(_nonExistingObjectID));
    }

    [Test]
    public void PropertyAccess_ShouldThrow_ForeignKeyRelationProperty ()
    {
      var instance = (Order)LifetimeService.GetObjectReference(TestableClientTransaction, _nonExistingObjectID);
      Assert.That(instance.State.IsNotLoadedYet, Is.True);
      Assert.That(() => instance.Customer, ThrowsObjectNotFoundException(_nonExistingObjectID));
      CheckObjectIsMarkedInvalid(instance.ID);

      // After the object has been marked invalid
      Assert.That(() => instance.Customer, ThrowsObjectInvalidException(_nonExistingObjectID));
    }

    [Test]
    public void PropertyAccess_ShouldThrow_CollectionRelationProperty ()
    {
      var instance = (Order)LifetimeService.GetObjectReference(TestableClientTransaction, _nonExistingObjectID);
      Assert.That(instance.State.IsNotLoadedYet, Is.True);
      Assert.That(() => instance.OrderItems, ThrowsObjectNotFoundException(_nonExistingObjectID));
      CheckObjectIsMarkedInvalid(instance.ID);

      // After the object has been marked invalid
      Assert.That(() => instance.OrderItems, ThrowsObjectInvalidException(_nonExistingObjectID));
    }

    [Test]
    public void PropertyAccess_Subtransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var instance = (ClassWithAllDataTypes)LifetimeService.GetObjectReference(ClientTransaction.Current, _nonExistingObjectIDForSubtransaction);
        CheckObjectIsMarkedInvalid(_nonExistingObjectIDForSubtransaction);

        Assert.That(() => instance.StringProperty, ThrowsObjectInvalidException(_nonExistingObjectIDForSubtransaction));

        var instance2 = (Order)LifetimeService.GetObjectReference(ClientTransaction.Current, _nonExistingObjectID);
        Assert.That(() => instance2.OrderNumber, ThrowsObjectNotFoundException(_nonExistingObjectID));
        CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      }
      CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      CheckObjectIsNotMarkedInvalid(_nonExistingObjectIDForSubtransaction);
    }

    [Test]
    public void UnidirectionalRelationProperty_ShouldReturnInvalidObject ()
    {
      // Need to disable the foreign key constraints so that the property is allowed to point to an invalid ID in the database
      var clientTable = (TableDefinition)GetTypeDefinition(typeof(Client)).StorageEntityDefinition;
      DisableConstraints(clientTable);

      ObjectID clientID = null;
      try
      {
        clientID = CreateClientWithNonExistingParentClient();

        var client = clientID.GetObject<Client>();
        Client instance = null;
        Assert.That(() => instance = client.ParentClient, Throws.Nothing);
        Assert.That(instance.State.IsNotLoadedYet, Is.True);

        Assert.That(() => instance.EnsureDataAvailable(), Throws.TypeOf<ObjectsNotFoundException>());
        CheckObjectIsMarkedInvalid(instance.ID);
      }
      finally
      {
        if (clientID != null)
          CleanupClientWithNonExistingParentClient(clientID);

        EnableConstraints(clientTable);
      }
    }

    [Test]
    public void UnidirectionalRelationProperty_Subtransaction ()
    {
      var newClient = Client.NewObject();
      newClient.ParentClient = DomainObjectIDs.Client3.GetObject<Client>();

      var nonExistingParentClient = newClient.ParentClient;
      nonExistingParentClient.Delete();

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        CheckObjectIsMarkedInvalid(nonExistingParentClient.ID);

        Client instance = null;
        Assert.That(() => instance = nonExistingParentClient, Throws.Nothing);
        Assert.That(instance, Is.SameAs(nonExistingParentClient));
      }
    }

    [Test]
    public void BidirectionalForeignKeyRelationProperty_ShouldReturnNotLoadedObject ()
    {
      var id = new ObjectID(typeof(ClassWithInvalidRelation), new Guid("{AFA9CF46-8E77-4da8-9793-53CAA86A277C}"));
      var objectWithInvalidRelation = (ClassWithInvalidRelation)id.GetObject<TestDomainBase>();

      DomainObject instance = null;

      Assert.That(() => instance = objectWithInvalidRelation.ClassWithGuidKey, Throws.Nothing);

      Assert.That(instance.State.IsNotLoadedYet, Is.True);

      Assert.That(() => instance.EnsureDataAvailable(), Throws.TypeOf<ObjectsNotFoundException>());
      CheckObjectIsMarkedInvalid(instance.ID);

      // Note: See also ObjectWithInvalidForeignKeyTest
    }

    [Test]
    public void BidirectionalForeignKeyRelationProperty_Subtransaction ()
    {
      DomainObject instance = null;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var id = new ObjectID(typeof(ClassWithInvalidRelation), new Guid("{AFA9CF46-8E77-4da8-9793-53CAA86A277C}"));

        var objectWithInvalidRelation = (ClassWithInvalidRelation)id.GetObject<TestDomainBase>();

        Assert.That(() => instance = objectWithInvalidRelation.ClassWithGuidKey, Throws.Nothing);
        Assert.That(instance.State.IsNotLoadedYet, Is.True);

        Assert.That(() => instance.EnsureDataAvailable(), Throws.TypeOf<ObjectsNotFoundException>());
        CheckObjectIsMarkedInvalid(instance.ID);
      }
      CheckObjectIsMarkedInvalid(instance.ID);
    }

    [Test]
    public void Invalidity_IsPropagated_ToSubtransaction_EvenIfLoadedOnlyIntoParentTransaction ()
    {
      // When a subtransaction is active and a non-existing object is tried to be loaded into the parent transaction, the non-existing object should
      // be marked invalid in both transactions.

      // The only way to test this ATM is to trigger the loading of the non-existing object from a load event handler (which is triggered while the
      // parent transaction is temporarily writeable).
      var triggeringObjectReference = (TestDomainBase)LifetimeService.GetObjectReference(TestableClientTransaction, DomainObjectIDs.Order1);
      triggeringObjectReference.ProtectedLoaded += (sender, args) =>
      {
        if (ClientTransaction.Current == TestableClientTransaction)
          _nonExistingObjectID.TryGetObject<TestDomainBase>();
      };

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        // Trigger the load event
        triggeringObjectReference.EnsureDataAvailable();

        CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      }

      CheckObjectIsMarkedInvalid(_nonExistingObjectID);
    }

    [Test]
    public void Invalidity_WithMultipleSubTransactions ()
    {
      var middleTransaction = TestableClientTransaction.CreateSubTransaction();
      using (middleTransaction.EnterDiscardingScope())
      {
        var subTransaction = middleTransaction.CreateSubTransaction();
        using (subTransaction.EnterDiscardingScope())
        {
          Assert.That(() => _nonExistingObjectID.GetObject<Order>(includeDeleted: true), ThrowsObjectNotFoundException(_nonExistingObjectID));

          CheckObjectIsMarkedInvalid(_nonExistingObjectID);
        }
        CheckObjectIsMarkedInvalid(_nonExistingObjectID);
      }
      CheckObjectIsMarkedInvalid(_nonExistingObjectID);
    }

    private void EnableConstraints (TableDefinition tableDefinition)
    {
      var commandText = string.Format("ALTER TABLE [{0}] WITH CHECK CHECK CONSTRAINT all", tableDefinition.TableName.EntityName);
      DatabaseAgent.ExecuteCommand(commandText);
    }

    private void DisableConstraints (TableDefinition tableDefinition)
    {
      var commandText = string.Format("ALTER TABLE [{0}] NOCHECK CONSTRAINT all", tableDefinition.TableName.EntityName);
      DatabaseAgent.ExecuteCommand(commandText);
    }

    private void CheckObjectIsMarkedInvalid (ObjectID objectID)
    {
      var instance = LifetimeService.GetObjectReference(ClientTransaction.Current, objectID);
      Assert.That(((DomainObject)instance).State.IsInvalid, Is.True);
    }

    private void CheckObjectIsNotMarkedInvalid (ObjectID objectID)
    {
      var instance = LifetimeService.GetObjectReference(ClientTransaction.Current, objectID);
      Assert.That(((DomainObject)instance).State.IsInvalid, Is.False);
    }

    private IResolveConstraint ThrowsObjectNotFoundException (ObjectID objectID)
    {
      var expected = string.Format("Object(s) could not be found: '{0}'.", objectID);
      return Throws.TypeOf<ObjectsNotFoundException>().With.Message.EqualTo(expected);
    }

    private IResolveConstraint ThrowsObjectInvalidException (ObjectID objectID)
    {
      var expected = string.Format("Object '{0}' is invalid in this transaction.", objectID);
      return Throws.TypeOf<ObjectInvalidException>().With.Message.EqualTo(expected);
    }

    private ObjectID CreateClientWithNonExistingParentClient ()
    {
      ObjectID newClientID;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var newClient = Client.NewObject();
        newClientID = newClient.ID;
        newClient.ParentClient = DomainObjectIDs.Client3.GetObject<Client>();
        newClient.ParentClient.Delete();
        ClientTransaction.Current.Commit();
        Assert.That(newClient.ParentClient.State.IsInvalid, Is.True);
      }
      return newClientID;
    }

    private void CleanupClientWithNonExistingParentClient (ObjectID clientID)
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var client = clientID.GetObject<Client>();
        client.Delete();
        ClientTransaction.Current.Commit();
      }
    }
  }
}
