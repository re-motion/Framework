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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class LoadedObjectDataRegistrationListenerTest : StandardMappingTest
  {
    private MockRepository _mockRepository;
    private IClientTransactionEventSink _eventSinkWithMock;
    private ITransactionHierarchyManager _hierarchyManagerMock;

    private LoadedObjectDataRegistrationListener _decorator;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository();
      _eventSinkWithMock = _mockRepository.StrictMock<IClientTransactionEventSink>();
      _hierarchyManagerMock = _mockRepository.StrictMock<ITransactionHierarchyManager> ();

      _decorator = new LoadedObjectDataRegistrationListener (_eventSinkWithMock, _hierarchyManagerMock);
    }

    [Test]
    public void OnBeforeObjectRegistration ()
    {
      var loadedObjectIDs = Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });
      using (_mockRepository.Ordered ())
      {
        _hierarchyManagerMock.Expect (mock => mock.OnBeforeObjectRegistration (loadedObjectIDs));
        _eventSinkWithMock.Expect (mock => mock.RaiseObjectsLoadingEvent ( loadedObjectIDs));
      }
      _mockRepository.ReplayAll();

      _decorator.OnBeforeObjectRegistration (loadedObjectIDs);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OnBeforeObjectRegistration_ExceptionInInnerListener ()
    {
      var loadedObjectIDs = Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      var exception = new Exception ("Test");
      using (_mockRepository.Ordered ())
      {
        _hierarchyManagerMock.Expect (mock => mock.OnBeforeObjectRegistration (loadedObjectIDs)).Throw (exception);
      }
      _mockRepository.ReplayAll ();

      Assert.That (() => _decorator.OnBeforeObjectRegistration (loadedObjectIDs), Throws.Exception.SameAs (exception));

      _eventSinkWithMock.AssertWasNotCalled (mock => mock.RaiseObjectsLoadingEvent ( Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _hierarchyManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void OnBeforeObjectRegistration_ExceptionInEventSink_CausesHierarchyManagerToBeNotifiedOfEnd ()
    {
      var loadedObjectIDs = Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      var exception = new Exception ("Test");
      using (_mockRepository.Ordered ())
      {
        _hierarchyManagerMock.Expect (mock => mock.OnBeforeObjectRegistration (loadedObjectIDs));
        _eventSinkWithMock.Expect (mock => mock.RaiseObjectsLoadingEvent (loadedObjectIDs)).Throw (exception);
        _hierarchyManagerMock.Expect (mock => mock.OnAfterObjectRegistration (loadedObjectIDs));
      }
      _mockRepository.ReplayAll ();

      Assert.That (() => _decorator.OnBeforeObjectRegistration (loadedObjectIDs), Throws.Exception.SameAs (exception));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OnAfterObjectRegistration ()
    {
      var loadedObjectIDs = Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });
      var actuallyLoadedDomainObjects = Array.AsReadOnly (new[] { DomainObjectMother.CreateFakeObject(), DomainObjectMother.CreateFakeObject() });
      
      using (_mockRepository.Ordered ())
      {
        _eventSinkWithMock.Expect (mock => mock.RaiseObjectsLoadedEvent ( actuallyLoadedDomainObjects));
        _hierarchyManagerMock.Expect (mock => mock.OnAfterObjectRegistration (loadedObjectIDs));
      }
      _mockRepository.ReplayAll ();

      _decorator.OnAfterObjectRegistration (loadedObjectIDs, actuallyLoadedDomainObjects);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void OnAfterObjectRegistration_ExceptionInEventSink_HierarchyManager_IsStillNotified ()
    {
      var loadedObjectIDs = Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });
      var actuallyLoadedDomainObjects = Array.AsReadOnly (new[] { DomainObjectMother.CreateFakeObject (), DomainObjectMother.CreateFakeObject () });

      var exception = new Exception ("Test");
      using (_mockRepository.Ordered ())
      {
        _eventSinkWithMock.Expect (mock => mock.RaiseObjectsLoadedEvent ( actuallyLoadedDomainObjects))
            .Throw (exception);
        _hierarchyManagerMock.Expect (mock => mock.OnAfterObjectRegistration (loadedObjectIDs));
      }
      _mockRepository.ReplayAll ();

      Assert.That (() => _decorator.OnAfterObjectRegistration (loadedObjectIDs, actuallyLoadedDomainObjects), Throws.Exception.SameAs (exception));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void OnAfterObjectRegistration_NoLoadedObjects ()
    {
      var loadedObjectIDs = Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });
      var actuallyLoadedDomainObjects = Array.AsReadOnly (new DomainObject[0]);

      _hierarchyManagerMock.Expect (mock => mock.OnAfterObjectRegistration (loadedObjectIDs));
      _mockRepository.ReplayAll ();

      _decorator.OnAfterObjectRegistration (loadedObjectIDs, actuallyLoadedDomainObjects);

      _eventSinkWithMock.AssertWasNotCalled (mock => mock.RaiseObjectsLoadedEvent ( Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
      _hierarchyManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void OnObjectsNotFound ()
    {
      var notFoundObjectIDs = Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });
      
      _eventSinkWithMock.Expect (mock => mock.RaiseObjectsNotFoundEvent ( notFoundObjectIDs));
      _mockRepository.ReplayAll();

      _decorator.OnObjectsNotFound (notFoundObjectIDs);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Serializable ()
    {
      var instance = new LoadedObjectDataRegistrationListener (
          new SerializableClientTransactionEventSinkFake(), new SerializableTransactionHierarchyManagerFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.EventSink, Is.Not.Null);
      Assert.That (deserializedInstance.HierarchyManager, Is.Not.Null);
    }
  }
}