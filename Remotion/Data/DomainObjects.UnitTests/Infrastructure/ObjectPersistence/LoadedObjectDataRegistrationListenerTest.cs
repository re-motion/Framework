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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class LoadedObjectDataRegistrationListenerTest : StandardMappingTest
  {
    private Mock<IClientTransactionEventSink> _eventSinkWithMock;
    private Mock<ITransactionHierarchyManager> _hierarchyManagerMock;

    private LoadedObjectDataRegistrationListener _decorator;

    public override void SetUp ()
    {
      base.SetUp();

      _eventSinkWithMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);
      _hierarchyManagerMock = new Mock<ITransactionHierarchyManager>(MockBehavior.Strict);

      _decorator = new LoadedObjectDataRegistrationListener(_eventSinkWithMock.Object, _hierarchyManagerMock.Object);
    }

    [Test]
    public void OnBeforeObjectRegistration ()
    {
      var loadedObjectIDs = Array.AsReadOnly(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });
      var sequence = new VerifiableSequence();
      _hierarchyManagerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnBeforeObjectRegistration(loadedObjectIDs)).Verifiable();
      _eventSinkWithMock.InVerifiableSequence(sequence).Setup(mock => mock.RaiseObjectsLoadingEvent(loadedObjectIDs)).Verifiable();

      _decorator.OnBeforeObjectRegistration(loadedObjectIDs);

      _eventSinkWithMock.Verify();
      _hierarchyManagerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void OnBeforeObjectRegistration_ExceptionInInnerListener ()
    {
      var loadedObjectIDs = Array.AsReadOnly(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      var exception = new Exception("Test");
      var sequence = new VerifiableSequence();
      _hierarchyManagerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnBeforeObjectRegistration(loadedObjectIDs)).Throws(exception).Verifiable();

      Assert.That(() => _decorator.OnBeforeObjectRegistration(loadedObjectIDs), Throws.Exception.SameAs(exception));

      _eventSinkWithMock.Verify(mock => mock.RaiseObjectsLoadingEvent(It.IsAny<ReadOnlyCollection<ObjectID>>()), Times.Never());
      _hierarchyManagerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void OnBeforeObjectRegistration_ExceptionInEventSink_CausesHierarchyManagerToBeNotifiedOfEnd ()
    {
      var loadedObjectIDs = Array.AsReadOnly(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      var exception = new Exception("Test");
      var sequence = new VerifiableSequence();
      _hierarchyManagerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnBeforeObjectRegistration(loadedObjectIDs)).Verifiable();
      _eventSinkWithMock.InVerifiableSequence(sequence).Setup(mock => mock.RaiseObjectsLoadingEvent(loadedObjectIDs)).Throws(exception).Verifiable();
      _hierarchyManagerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnAfterObjectRegistration(loadedObjectIDs)).Verifiable();

      Assert.That(() => _decorator.OnBeforeObjectRegistration(loadedObjectIDs), Throws.Exception.SameAs(exception));

      _eventSinkWithMock.Verify();
      _hierarchyManagerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void OnAfterObjectRegistration ()
    {
      var loadedObjectIDs = Array.AsReadOnly(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });
      var actuallyLoadedDomainObjects = Array.AsReadOnly(new[] { DomainObjectMother.CreateFakeObject(), DomainObjectMother.CreateFakeObject() });

      var sequence = new VerifiableSequence();
      _eventSinkWithMock.InVerifiableSequence(sequence).Setup(mock => mock.RaiseObjectsLoadedEvent(actuallyLoadedDomainObjects)).Verifiable();
      _hierarchyManagerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnAfterObjectRegistration(loadedObjectIDs)).Verifiable();

      _decorator.OnAfterObjectRegistration(loadedObjectIDs, actuallyLoadedDomainObjects);

      _eventSinkWithMock.Verify();
      _hierarchyManagerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void OnAfterObjectRegistration_ExceptionInEventSink_HierarchyManager_IsStillNotified ()
    {
      var loadedObjectIDs = Array.AsReadOnly(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });
      var actuallyLoadedDomainObjects = Array.AsReadOnly(new[] { DomainObjectMother.CreateFakeObject(), DomainObjectMother.CreateFakeObject() });

      var exception = new Exception("Test");
      var sequence = new VerifiableSequence();
      _eventSinkWithMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.RaiseObjectsLoadedEvent(actuallyLoadedDomainObjects))
            .Throws(exception)
            .Verifiable();
      _hierarchyManagerMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnAfterObjectRegistration(loadedObjectIDs))
            .Verifiable();

      Assert.That(() => _decorator.OnAfterObjectRegistration(loadedObjectIDs, actuallyLoadedDomainObjects), Throws.Exception.SameAs(exception));

      _eventSinkWithMock.Verify();
      _hierarchyManagerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void OnAfterObjectRegistration_NoLoadedObjects ()
    {
      var loadedObjectIDs = Array.AsReadOnly(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });
      var actuallyLoadedDomainObjects = Array.AsReadOnly(new DomainObject[0]);

      _hierarchyManagerMock.Setup(mock => mock.OnAfterObjectRegistration(loadedObjectIDs)).Verifiable();

      _decorator.OnAfterObjectRegistration(loadedObjectIDs, actuallyLoadedDomainObjects);

      _eventSinkWithMock.Verify(mock => mock.RaiseObjectsLoadedEvent(It.IsAny<ReadOnlyCollection<DomainObject>>()), Times.Never());
      _hierarchyManagerMock.Verify();
    }

    [Test]
    public void OnObjectsNotFound ()
    {
      var notFoundObjectIDs = Array.AsReadOnly(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      _eventSinkWithMock.Setup(mock => mock.RaiseObjectsNotFoundEvent(notFoundObjectIDs)).Verifiable();

      _decorator.OnObjectsNotFound(notFoundObjectIDs);

      _eventSinkWithMock.Verify();
      _hierarchyManagerMock.Verify();
    }
  }
}
