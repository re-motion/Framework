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
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class LoadedObjectDataRegistrationAgentTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;
    private Mock<IDataManager> _dataManagerMock;
    private Mock<ILoadedObjectDataRegistrationListener> _registrationListenerMock;

    private LoadedObjectDataRegistrationAgent _agent;

    public override void SetUp ()
    {
      base.SetUp();

      _clientTransaction = ClientTransactionObjectMother.Create();

      _dataManagerMock = new Mock<IDataManager>(MockBehavior.Strict);
      _registrationListenerMock = new Mock<ILoadedObjectDataRegistrationListener>(MockBehavior.Strict);

      _agent = new LoadedObjectDataRegistrationAgent(_clientTransaction, _dataManagerMock.Object, _registrationListenerMock.Object);
    }

    [Test]
    public void RegisterIfRequired_AlreadyExistingLoadedObject ()
    {
      var alreadyExistingLoadedObject = GetAlreadyExistingLoadedObject();

      var result = _agent.RegisterIfRequired(new[] { alreadyExistingLoadedObject }, true);

      _registrationListenerMock.Verify(mock => mock.OnBeforeObjectRegistration(It.IsAny<ReadOnlyCollection<ObjectID>>()), Times.Never());
      _dataManagerMock.Verify(mock => mock.RegisterDataContainer(It.IsAny<DataContainer>()), Times.Never());
      Assert.That(result, Is.EqualTo(new[] { alreadyExistingLoadedObject }));
    }

    [Test]
    public void RegisterIfRequired_FreshlyLoadedObject ()
    {
      var freshlyLoadedObject = GetFreshlyLoadedObject();
      var dataContainer = freshlyLoadedObject.FreshlyLoadedDataContainer;
      Assert.That(dataContainer.HasDomainObject, Is.False);

      var loadedObjectIDs = new[] { dataContainer.ID };
      var sequence = new VerifiableSequence();
      _registrationListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnBeforeObjectRegistration(loadedObjectIDs)).Verifiable();
      _dataManagerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RegisterDataContainer(dataContainer))
          .Callback((DataContainer dataContainer) => CheckHasEnlistedDomainObject(dataContainer))
          .Verifiable();
      _registrationListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.OnAfterObjectRegistration(
                  loadedObjectIDs,
                  // Lazy matching because DataContainers don't have DomainObjects from the start
                  It.Is<ReadOnlyCollection<DomainObject>>(list => list.SequenceEqual(new[] { dataContainer.DomainObject }))))
          .Verifiable();

      var result = _agent.RegisterIfRequired(new[] { freshlyLoadedObject }, true);

      _dataManagerMock.Verify();
      _registrationListenerMock.Verify();
      sequence.Verify();
      Assert.That(_clientTransaction.IsDiscarded, Is.False);
      Assert.That(result, Is.EqualTo(new[] { freshlyLoadedObject }));
    }

    [Test]
    public void RegisterIfRequired_NullLoadedObject ()
    {
      var nullLoadedObject = GetNullLoadedObject();

      var result = _agent.RegisterIfRequired(new[] { nullLoadedObject }, true);

      _registrationListenerMock.Verify(mock => mock.OnBeforeObjectRegistration(It.IsAny<ReadOnlyCollection<ObjectID>>()), Times.Never());
      _dataManagerMock.Verify(mock => mock.RegisterDataContainer(It.IsAny<DataContainer>()), Times.Never());
      Assert.That(result, Is.EqualTo(new[] { nullLoadedObject }));
    }

    [Test]
    public void RegisterIfRequired_InvalidLoadedObject ()
    {
      var invalidLoadedObject = GetInvalidLoadedObject();

      var result = _agent.RegisterIfRequired(new[] { invalidLoadedObject }, true);

      _registrationListenerMock.Verify(mock => mock.OnBeforeObjectRegistration(It.IsAny<ReadOnlyCollection<ObjectID>>()), Times.Never());
      _dataManagerMock.Verify(mock => mock.RegisterDataContainer(It.IsAny<DataContainer>()), Times.Never());
      Assert.That(result, Is.EqualTo(new[] { invalidLoadedObject }));
    }

    [Test]
    public void RegisterIfRequired_NotFoundLoadedObject_ThrowOnNotFoundFalse ()
    {
      var notFoundLoadedObject = GetNotFoundLoadedObject();

      _registrationListenerMock
            .Setup(mock => mock.OnObjectsNotFound(new[] { notFoundLoadedObject.ObjectID }))
            .Verifiable();

      var result = _agent.RegisterIfRequired(new[] { notFoundLoadedObject }, false);

      _registrationListenerMock.Verify(mock => mock.OnBeforeObjectRegistration(It.IsAny<ReadOnlyCollection<ObjectID>>()), Times.Never());
      _registrationListenerMock.Verify();
      _dataManagerMock.Verify(mock => mock.RegisterDataContainer(It.IsAny<DataContainer>()), Times.Never());

      Assert.That(result, Is.EqualTo(new[] { notFoundLoadedObject }));
    }

    [Test]
    public void RegisterIfRequired_NotFoundLoadedObject_ThrowOnNotFoundTrue ()
    {
      var notFoundLoadedObject = GetNotFoundLoadedObject();

      _registrationListenerMock
          .Setup(mock => mock.OnObjectsNotFound(new[] { notFoundLoadedObject.ObjectID }))
          .Verifiable();

      Assert.That(
          () => _agent.RegisterIfRequired(new[] { notFoundLoadedObject }, true),
          Throws.TypeOf<ObjectsNotFoundException>().With.Message.EqualTo(
              string.Format("Object(s) could not be found: '{0}'.", notFoundLoadedObject.ObjectID)));

      _registrationListenerMock.Verify(mock => mock.OnBeforeObjectRegistration(It.IsAny<ReadOnlyCollection<ObjectID>>()), Times.Never());
      _registrationListenerMock.Verify();

      _dataManagerMock.Verify(mock => mock.RegisterDataContainer(It.IsAny<DataContainer>()), Times.Never());
    }

    [Test]
    public void RegisterIfRequired_MultipleObjects_ThrowOnNotFoundFalse ()
    {
      var freshlyLoadedObject1 = GetFreshlyLoadedObject();
      var registerableDataContainer1 = freshlyLoadedObject1.FreshlyLoadedDataContainer;
      Assert.That(registerableDataContainer1.HasDomainObject, Is.False);

      var freshlyLoadedObject2 = GetFreshlyLoadedObject();
      var registerableDataContainer2 = freshlyLoadedObject2.FreshlyLoadedDataContainer;
      Assert.That(registerableDataContainer2.HasDomainObject, Is.False);

      var alreadyExistingLoadedObject = GetAlreadyExistingLoadedObject();
      var nullLoadedObject = GetNullLoadedObject();
      var invalidLoadedObject = GetInvalidLoadedObject();
      var notFoundLoadedObject1 = GetNotFoundLoadedObject();
      var notFoundLoadedObject2 = GetNotFoundLoadedObject();

      var loadedObjectIDs = new[] { registerableDataContainer1.ID, registerableDataContainer2.ID };

      var sequence = new VerifiableSequence();
      _registrationListenerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnObjectsNotFound(new[] { notFoundLoadedObject1.ObjectID, notFoundLoadedObject2.ObjectID }))
            .Verifiable();
      _registrationListenerMock
            .InVerifiableSequence(sequence).Setup(mock => mock.OnBeforeObjectRegistration(loadedObjectIDs)).Verifiable();
      _dataManagerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.RegisterDataContainer(registerableDataContainer1))
            .Callback((DataContainer dataContainer) => CheckHasEnlistedDomainObject(registerableDataContainer1))
            .Verifiable();
      _dataManagerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.RegisterDataContainer(registerableDataContainer2))
            .Callback((DataContainer dataContainer) => CheckHasEnlistedDomainObject(registerableDataContainer2))
            .Verifiable();
      _registrationListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.OnAfterObjectRegistration(
                  loadedObjectIDs,
                  // Lazy matching because DataContainers don't have DomainObjects from the start
                  It.Is<ReadOnlyCollection<DomainObject>>(list => list.SequenceEqual(new[] { registerableDataContainer1.DomainObject, registerableDataContainer2.DomainObject }))))
          .Verifiable();

      var allObjects =
          new ILoadedObjectData[]
          {
              freshlyLoadedObject1,
              alreadyExistingLoadedObject,
              freshlyLoadedObject2,
              nullLoadedObject,
              invalidLoadedObject,
              notFoundLoadedObject1,
              notFoundLoadedObject2
          };
      var result = _agent.RegisterIfRequired(allObjects, false);

      _dataManagerMock.Verify();
      _registrationListenerMock.Verify();
      sequence.Verify();
      Assert.That(_clientTransaction.IsDiscarded, Is.False);
      Assert.That(result, Is.EqualTo(allObjects));
    }

    [Test]
    public void RegisterIfRequired_MultipleObjects_ThrowOnNotFoundTrue ()
    {
      var freshlyLoadedObject1 = GetFreshlyLoadedObject();
      var registerableDataContainer1 = freshlyLoadedObject1.FreshlyLoadedDataContainer;
      Assert.That(registerableDataContainer1.HasDomainObject, Is.False);

      var freshlyLoadedObject2 = GetFreshlyLoadedObject();
      var registerableDataContainer2 = freshlyLoadedObject2.FreshlyLoadedDataContainer;
      Assert.That(registerableDataContainer2.HasDomainObject, Is.False);

      var alreadyExistingLoadedObject = GetAlreadyExistingLoadedObject();
      var nullLoadedObject = GetNullLoadedObject();
      var invalidLoadedObject = GetInvalidLoadedObject();
      var notFoundLoadedObject1 = GetNotFoundLoadedObject();
      var notFoundLoadedObject2 = GetNotFoundLoadedObject();

      _registrationListenerMock
          .Setup(mock => mock.OnObjectsNotFound(new[] { notFoundLoadedObject1.ObjectID, notFoundLoadedObject2.ObjectID }))
          .Verifiable();

      var allObjects =
          new ILoadedObjectData[]
          {
              freshlyLoadedObject1,
              alreadyExistingLoadedObject,
              freshlyLoadedObject2,
              nullLoadedObject,
              invalidLoadedObject,
              notFoundLoadedObject1,
              notFoundLoadedObject2
          };
      Assert.That(
          () => _agent.RegisterIfRequired(allObjects, true),
          Throws.TypeOf<ObjectsNotFoundException>().With.Message.EqualTo(
              string.Format("Object(s) could not be found: '{0}', '{1}'.", notFoundLoadedObject1.ObjectID, notFoundLoadedObject2.ObjectID)));

      _registrationListenerMock.Verify(mock => mock.OnBeforeObjectRegistration(It.IsAny<ReadOnlyCollection<ObjectID>>()), Times.Never());
      _registrationListenerMock.Verify();

      _dataManagerMock.Verify(mock => mock.RegisterDataContainer(It.IsAny<DataContainer>()), Times.Never());
    }

    [Test]
    public void RegisterIfRequired_MultipleObjects_DuplicateFreshlyLoadedObjects_AreConsolidated ()
    {
      var freshlyLoadedObject1a = GetFreshlyLoadedObject();
      var registerableDataContainer1a = freshlyLoadedObject1a.FreshlyLoadedDataContainer;
      Assert.That(registerableDataContainer1a.HasDomainObject, Is.False);

      var freshlyLoadedObject1b = GetFreshlyLoadedObject(freshlyLoadedObject1a.ObjectID);
      var registerableDataContainer1b = freshlyLoadedObject1b.FreshlyLoadedDataContainer;
      Assert.That(registerableDataContainer1b.HasDomainObject, Is.False);

      var loadedObjectIDs = new[] { registerableDataContainer1a.ID };

      var sequence = new VerifiableSequence();
      _registrationListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnBeforeObjectRegistration(loadedObjectIDs)).Verifiable();
      _dataManagerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RegisterDataContainer(registerableDataContainer1a))
          .Callback((DataContainer dataContainer) => CheckHasEnlistedDomainObject(registerableDataContainer1a))
          .Verifiable();
      _registrationListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.OnAfterObjectRegistration(
                  loadedObjectIDs,
                  // Lazy matching because DataContainers don't have DomainObjects from the start
                  It.Is<ReadOnlyCollection<DomainObject>>(list => list.SequenceEqual(new[] { registerableDataContainer1a.DomainObject }))))
          .Verifiable();

      var allObjects = new ILoadedObjectData[] { freshlyLoadedObject1a, freshlyLoadedObject1b };
      var result = _agent.RegisterIfRequired(allObjects, false).ToArray();

      _dataManagerMock.Verify();
      _registrationListenerMock.Verify();
      sequence.Verify();
      Assert.That(_clientTransaction.IsDiscarded, Is.False);
      Assert.That(result, Is.EqualTo(new[] { freshlyLoadedObject1a, freshlyLoadedObject1a }));

      Assert.That(registerableDataContainer1b.HasDomainObject, Is.False);
    }

    [Test]
    public void RegisterIfRequired_NoObjects ()
    {
      var result = _agent.RegisterIfRequired(new ILoadedObjectData[0], true);

      _registrationListenerMock.Verify(mock => mock.OnBeforeObjectRegistration(It.IsAny<ReadOnlyCollection<ObjectID>>()), Times.Never());
      _registrationListenerMock.Verify(
          mock => mock.OnAfterObjectRegistration(It.IsAny<ReadOnlyCollection<ObjectID>>(), It.IsAny<ReadOnlyCollection<DomainObject>>()),
          Times.Never());
      _dataManagerMock.Verify(mock => mock.RegisterDataContainer(It.IsAny<DataContainer>()), Times.Never());
      Assert.That(result, Is.Empty);
    }

    [Test]
    public void RegisterIfRequired_ExceptionWhenRegisteringObject_SomeObjectsSucceeded ()
    {
      var exception = new Exception("Test");

      var freshlyLoadedObject1 = GetFreshlyLoadedObject();
      var registerableDataContainer1 = freshlyLoadedObject1.FreshlyLoadedDataContainer;
      Assert.That(registerableDataContainer1.HasDomainObject, Is.False);

      var freshlyLoadedObject2 = GetFreshlyLoadedObject();
      var registerableDataContainer2 = freshlyLoadedObject2.FreshlyLoadedDataContainer;
      Assert.That(registerableDataContainer2.HasDomainObject, Is.False);

      var loadedObjectIDs = new[] { registerableDataContainer1.ID, registerableDataContainer2.ID };

      var sequence = new VerifiableSequence();
      _registrationListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnBeforeObjectRegistration(loadedObjectIDs)).Verifiable();
      _dataManagerMock.InVerifiableSequence(sequence).Setup(mock => mock.RegisterDataContainer(registerableDataContainer1)).Verifiable();
      _dataManagerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RegisterDataContainer(registerableDataContainer2))
          .Throws(exception)
          .Verifiable();
      _registrationListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.OnAfterObjectRegistration(
                  loadedObjectIDs,
                  // Lazy matching because DataContainers don't have DomainObjects from the start
                  It.Is<ReadOnlyCollection<DomainObject>>(list => list.SequenceEqual(new[] { registerableDataContainer1.DomainObject }))))
          .Verifiable();

      Assert.That(
          () => _agent.RegisterIfRequired(new ILoadedObjectData[] { freshlyLoadedObject1, freshlyLoadedObject2 }, true),
          Throws.Exception.SameAs(exception));

      _dataManagerMock.Verify();
      _registrationListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void RegisterIfRequired_ExceptionWhenRegisteringObject_NoObjectsSucceeded ()
    {
      var exception = new Exception("Test");

      var freshlyLoadedObject1 = GetFreshlyLoadedObject();
      var registerableDataContainer1 = freshlyLoadedObject1.FreshlyLoadedDataContainer;

      var loadedObjectIDs = new[] { registerableDataContainer1.ID };

      var sequence = new VerifiableSequence();
      _registrationListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnBeforeObjectRegistration(loadedObjectIDs)).Verifiable();
      _dataManagerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RegisterDataContainer(registerableDataContainer1))
          .Throws(exception)
          .Verifiable();
      _registrationListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.OnAfterObjectRegistration(loadedObjectIDs, new DomainObject[0]))
          .Verifiable();

      Assert.That(
          () => _agent.RegisterIfRequired(new ILoadedObjectData[] { freshlyLoadedObject1 }, true),
          Throws.Exception.SameAs(exception));

      _dataManagerMock.Verify();
      _registrationListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void BeginRegisterIfRequired_RaisesBeginEvent_AndSetsDomainObjects_ButDoesNotRegister ()
    {
      var freshlyLoadedObject = GetFreshlyLoadedObject();
      var registerableDataContainer = freshlyLoadedObject.FreshlyLoadedDataContainer;
      Assert.That(registerableDataContainer.HasDomainObject, Is.False);

      var alreadyExistingLoadedObject = GetAlreadyExistingLoadedObject();
      var nullLoadedObject = GetNullLoadedObject();
      var invalidLoadedObject = GetInvalidLoadedObject();

      var allObjects =
          new ILoadedObjectData[]
          {
              freshlyLoadedObject,
              alreadyExistingLoadedObject,
              nullLoadedObject,
              invalidLoadedObject
          };
      var collector = new LoadedObjectDataPendingRegistrationCollector();
      var result = _agent.BeginRegisterIfRequired(allObjects, false, collector);

      _dataManagerMock.Verify();
      _registrationListenerMock.Verify();
      CheckHasEnlistedDomainObject(registerableDataContainer);

      Assert.That(collector.DataPendingRegistration, Is.EquivalentTo(new[] { freshlyLoadedObject }));
      Assert.That(result, Is.EqualTo(allObjects));
    }

    [Test]
    public void BeginRegisterIfRequired_WithNotFoundObjects_AndThrowOnNotFoundFalse_RaisesOnNotFoundAndProceeds ()
    {
      var freshlyLoadedObject = GetFreshlyLoadedObject();
      var registerableDataContainer = freshlyLoadedObject.FreshlyLoadedDataContainer;
      Assert.That(registerableDataContainer.HasDomainObject, Is.False);

      var notFoundLoadedObject = GetNotFoundLoadedObject();

      _registrationListenerMock
            .Setup(mock => mock.OnObjectsNotFound(new[] { notFoundLoadedObject.ObjectID }))
            .Verifiable();

      var allObjects =
          new ILoadedObjectData[]
          {
              freshlyLoadedObject,
              notFoundLoadedObject
          };
      var collector = new LoadedObjectDataPendingRegistrationCollector();
      var result = _agent.BeginRegisterIfRequired(allObjects, false, collector);

      _dataManagerMock.Verify();
      _registrationListenerMock.Verify();
      CheckHasEnlistedDomainObject(registerableDataContainer);

      Assert.That(collector.DataPendingRegistration, Is.EqualTo(new[] { freshlyLoadedObject }));
      Assert.That(result, Is.EqualTo(allObjects));
    }

    [Test]
    public void BeginRegisterIfRequired_WithNotFoundObjects_AndThrowOnNotFoundTrue_RaisesdOnNotFoundAndThrows ()
    {
      var freshlyLoadedObject = GetFreshlyLoadedObject();
      var registerableDataContainer = freshlyLoadedObject.FreshlyLoadedDataContainer;
      Assert.That(registerableDataContainer.HasDomainObject, Is.False);

      var notFoundLoadedObject = GetNotFoundLoadedObject();

      _registrationListenerMock
          .Setup(mock => mock.OnObjectsNotFound(new[] { notFoundLoadedObject.ObjectID }))
          .Verifiable();

      var allObjects =
          new ILoadedObjectData[]
          {
              freshlyLoadedObject,
              notFoundLoadedObject
          };
      var collector = new LoadedObjectDataPendingRegistrationCollector();
      Assert.That(() => _agent.BeginRegisterIfRequired(allObjects, true, collector), Throws.TypeOf<ObjectsNotFoundException>());

      _dataManagerMock.Verify();
      _registrationListenerMock.Verify();

      // Note: In this case, we currently set the DomainObjects of the freshly loaded DataContainer, and we also add them to the collector. This
      // shouldn't make any difference, and it's easier to implement.
      Assert.That(registerableDataContainer.HasDomainObject, Is.True);
      Assert.That(collector.DataPendingRegistration, Is.EquivalentTo(new[] { freshlyLoadedObject }));
    }

    [Test]
    public void BeginRegisterIfRequired_ConsolidatesDuplicates ()
    {
      var freshlyLoadedObject1a = GetFreshlyLoadedObject();
      var registerableDataContainer1a = freshlyLoadedObject1a.FreshlyLoadedDataContainer;
      Assert.That(registerableDataContainer1a.HasDomainObject, Is.False);

      var freshlyLoadedObject1b = GetFreshlyLoadedObject(freshlyLoadedObject1a.ObjectID);
      var registerableDataContainer1b = freshlyLoadedObject1b.FreshlyLoadedDataContainer;
      Assert.That(registerableDataContainer1b.HasDomainObject, Is.False);

      var allObjects = new ILoadedObjectData[] { freshlyLoadedObject1a, freshlyLoadedObject1b };
      var collector = new LoadedObjectDataPendingRegistrationCollector();
      var result = _agent.BeginRegisterIfRequired(allObjects, false, collector);

      _dataManagerMock.Verify();
      _registrationListenerMock.Verify();
      CheckHasEnlistedDomainObject(registerableDataContainer1a);

      Assert.That(collector.DataPendingRegistration, Is.EquivalentTo(new[] { freshlyLoadedObject1a }));
      Assert.That(result, Is.EqualTo(new[] { freshlyLoadedObject1a, freshlyLoadedObject1a }));

      Assert.That(registerableDataContainer1b.HasDomainObject, Is.False);
    }

    [Test]
    public void EndRegisterIfRequired_RegistersDataContainers_AndRaisesEvents ()
    {
      var dataContainer1 = DataContainerObjectMother.Create(DomainObjectIDs.Order1);
      var dataContainer2 = DataContainerObjectMother.Create(DomainObjectIDs.Order3);
      var collector = CreateCollectorAndPrepare(dataContainer1, dataContainer2);

      var sequence = new VerifiableSequence();

      var loadedObjectIDs = new[] { dataContainer1.ID, dataContainer2.ID };
      _registrationListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnBeforeObjectRegistration(loadedObjectIDs)).Verifiable();
      _dataManagerMock.InVerifiableSequence(sequence).Setup(mock => mock.RegisterDataContainer(dataContainer1)).Verifiable();
      _dataManagerMock.InVerifiableSequence(sequence).Setup(mock => mock.RegisterDataContainer(dataContainer2)).Verifiable();
      _registrationListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.OnAfterObjectRegistration(new[] { dataContainer1.ID, dataContainer2.ID }, new[] { dataContainer1.DomainObject, dataContainer2.DomainObject }))
          .Verifiable();

      _agent.EndRegisterIfRequired(collector);

      _dataManagerMock.Verify();
      _registrationListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void EndRegisterIfRequired_RaisesEndEvent_EvenWhenRegistrationThrows ()
    {
      var dataContainer1 = DataContainerObjectMother.Create(DomainObjectIDs.Order1);
      var dataContainer2 = DataContainerObjectMother.Create(DomainObjectIDs.Order3);
      var collector = CreateCollectorAndPrepare(dataContainer1, dataContainer2);

      var exception = new Exception("Test");

      var sequence = new VerifiableSequence();

      var loadedObjectIDs = new[] { dataContainer1.ID, dataContainer2.ID };
      _registrationListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnBeforeObjectRegistration(loadedObjectIDs)).Verifiable();
      _dataManagerMock.InVerifiableSequence(sequence).Setup(mock => mock.RegisterDataContainer(dataContainer1)).Verifiable();
      _dataManagerMock.InVerifiableSequence(sequence).Setup(mock => mock.RegisterDataContainer(dataContainer2)).Throws(exception).Verifiable();
      _registrationListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.OnAfterObjectRegistration(new[] { dataContainer1.ID, dataContainer2.ID }, new[] { dataContainer1.DomainObject }))
          .Verifiable();

      Assert.That(() => _agent.EndRegisterIfRequired(collector), Throws.Exception.SameAs(exception));

      _dataManagerMock.Verify();
      _registrationListenerMock.Verify();
      sequence.Verify();
    }

    private FreshlyLoadedObjectData GetFreshlyLoadedObject (ObjectID id = null)
    {
      id = id ?? new ObjectID(typeof(Order), Guid.NewGuid());
      var dataContainer = DataContainer.CreateForExisting(id, null, pd => pd.DefaultValue);
      return new FreshlyLoadedObjectData(dataContainer);
    }

    private static LoadedObjectDataPendingRegistrationCollector CreateCollectorAndPrepare (params DataContainer[] dataContainers)
    {
      var collector = new LoadedObjectDataPendingRegistrationCollector();
      foreach (var dataContainer in dataContainers)
      {
        collector.Add(new FreshlyLoadedObjectData(dataContainer));
        dataContainer.SetDomainObject(DomainObjectMother.CreateFakeObject(dataContainer.ID));
      }
      return collector;
    }

    private AlreadyExistingLoadedObjectData GetAlreadyExistingLoadedObject ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      var dataContainer = DataContainer.CreateForExisting(domainObject.ID, null, pd => pd.DefaultValue);
      dataContainer.SetDomainObject(domainObject);
      DataContainerTestHelper.SetClientTransaction(dataContainer, _clientTransaction);
      return new AlreadyExistingLoadedObjectData(dataContainer);
    }

    private NullLoadedObjectData GetNullLoadedObject ()
    {
      return new NullLoadedObjectData();
    }

    private InvalidLoadedObjectData GetInvalidLoadedObject ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      return new InvalidLoadedObjectData(domainObject);
    }

    private NotFoundLoadedObjectData GetNotFoundLoadedObject ()
    {
      return new NotFoundLoadedObjectData(new ObjectID(typeof(Order), Guid.NewGuid()));
    }

    private void CheckHasEnlistedDomainObject (DataContainer dataContainer)
    {
      Assert.That(dataContainer.HasDomainObject, Is.True);
      Assert.That(dataContainer.DomainObject, Is.SameAs(_clientTransaction.GetEnlistedDomainObject(dataContainer.ID)));
    }
  }
}
