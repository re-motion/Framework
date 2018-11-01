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
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class LoggingClientTransactionListenerTest : StandardMappingTest
  {
    private MemoryAppender _memoryAppender;
    private TestableClientTransaction _clientTransaction;
    private LoggingClientTransactionListener _listener;
    private Client _domainObject;
    private Client _domainObject2;
    private Client _domainObject3;
    private DataContainer _dataContainer;
    private PropertyDefinition _propertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _memoryAppender = new MemoryAppender();
      BasicConfigurator.Configure (_memoryAppender);

      _clientTransaction = new TestableClientTransaction();

      _listener = new LoggingClientTransactionListener();

      _domainObject = DomainObjectMother.CreateObjectInTransaction<Client> (_clientTransaction);
      _dataContainer = _domainObject.GetInternalDataContainerForTransaction (_clientTransaction);
      _propertyDefinition = GetPropertyDefinition (typeof (Client), "ParentClient");

      _domainObject2 = DomainObjectMother.CreateObjectInTransaction<Client> (_clientTransaction);
      _domainObject3 = DomainObjectMother.CreateObjectInTransaction<Client> (_clientTransaction);
    }

    public override void TearDown ()
    {
      _memoryAppender.Clear();
      LogManager.ResetConfiguration();

      Assert.That (LogManager.GetLogger (typeof (LoggingClientTransactionListener)).IsDebugEnabled, Is.False);

      base.TearDown();
    }

    [Test]
    public void TransactionInitialize ()
    {
      CheckLoggingMethod (
          () => _listener.TransactionInitialize (_clientTransaction),
          string.Format ("{0} TransactionInitialize", _clientTransaction.ID));
    }

    [Test]
    public void TransactionDiscard ()
    {
      CheckLoggingMethod (
          () => _listener.TransactionDiscard (_clientTransaction),
          string.Format ("{0} TransactionDiscard", _clientTransaction.ID));
    }

    [Test]
    public void SubTransactionCreating ()
    {
      CheckLoggingMethod (
          () => _listener.SubTransactionCreating (_clientTransaction),
          string.Format ("{0} SubTransactionCreating", _clientTransaction.ID));
    }

    [Test]
    public void SubTransactionInitialize ()
    {
      CheckLoggingMethod (
          () => _listener.SubTransactionInitialize (_clientTransaction, _clientTransaction),
          string.Format ("{0} SubTransactionInitialize: {1}", _clientTransaction.ID, _clientTransaction.ID));
    }

    [Test]
    public void SubTransactionCreated ()
    {
      CheckLoggingMethod (
          () => _listener.SubTransactionCreated (_clientTransaction, _clientTransaction),
          string.Format ("{0} SubTransactionCreated: {1}", _clientTransaction.ID, _clientTransaction.ID));
    }

    [Test]
    public void NewObjectCreating ()
    {
      CheckLoggingMethod (
          () => _listener.NewObjectCreating (_clientTransaction, typeof (string)),
          string.Format ("{0} NewObjectCreating: {1}", _clientTransaction.ID, typeof (string).FullName));
    }

    [Test]
    public void ObjectsLoading ()
    {
      CheckLoggingMethod (
          () => _listener.ObjectsLoading (_clientTransaction, new ReadOnlyCollection<ObjectID> (new List<ObjectID>())),
          string.Format ("{0} ObjectsLoading: {1}", _clientTransaction.ID, ""));
    }

    [Test]
    public void ObjectsLoaded ()
    {
      CheckLoggingMethod (
          () => _listener.ObjectsLoaded (_clientTransaction, new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject })),
          string.Format ("{0} ObjectsLoaded: {1}", _clientTransaction.ID, _domainObject.ID));
    }

    [Test]
    public void ObjectsNotFound ()
    {
      CheckLoggingMethod (
          () => _listener.ObjectsNotFound (_clientTransaction, new ReadOnlyCollection<ObjectID> (new List<ObjectID> ())),
          string.Format ("{0} ObjectsNotFound: {1}", _clientTransaction.ID, ""));
    }

    [Test]
    public void ObjectsUnloaded ()
    {
      CheckLoggingMethod (
          () => _listener.ObjectsUnloaded (_clientTransaction, new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject })),
          string.Format ("{0} ObjectsUnloaded: {1}", _clientTransaction.ID, _domainObject.ID));
    }

    [Test]
    public void ObjectsUnloading ()
    {
      CheckLoggingMethod (
          () => _listener.ObjectsUnloading (_clientTransaction, new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject })),
          string.Format ("{0} ObjectsUnloading: {1}", _clientTransaction.ID, _domainObject.ID));
    }

    [Test]
    public void ObjectDeleting ()
    {
      CheckLoggingMethod (
          () => _listener.ObjectDeleting (_clientTransaction, _domainObject),
          string.Format ("{0} ObjectDeleting: {1}", _clientTransaction.ID, _domainObject.ID));
    }

    [Test]
    public void ObjectDeleted ()
    {
      CheckLoggingMethod (
          () => _listener.ObjectDeleted (_clientTransaction, _domainObject),
          string.Format ("{0} ObjectDeleted: {1}", _clientTransaction.ID, _domainObject.ID));
    }

    [Test]
    public void PropertyValueReading ()
    {
      CheckLoggingMethod (
          () => _listener.PropertyValueReading (_clientTransaction, _domainObject, _propertyDefinition, ValueAccess.Current),
          string.Format (
              "{0} PropertyValueReading: {1} ({2}, {3})", _clientTransaction.ID, _propertyDefinition.PropertyName, ValueAccess.Current, _dataContainer.ID));
    }

    [Test]
    public void PropertyValueChanging ()
    {
      CheckLoggingMethod (
          () => _listener.PropertyValueChanging (_clientTransaction, _domainObject, _propertyDefinition, 1, 2),
          string.Format ("{0} PropertyValueChanging: {1} {2}->{3} ({4})", _clientTransaction.ID, _propertyDefinition.PropertyName, 1, 2, _dataContainer.ID));
    }

    [Test]
    public void PropertyValueChanged ()
    {
      CheckLoggingMethod (
          () => _listener.PropertyValueChanged (_clientTransaction, _domainObject, _propertyDefinition, 1, 2),
          string.Format ("{0} PropertyValueChanged: {1} {2}->{3} ({4})", _clientTransaction.ID, _propertyDefinition.PropertyName, 1, 2, _dataContainer.ID));
    }

    [Test]
    public void RelationReading ()
    {
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      relationEndPointDefinition.Stub (n => n.PropertyName).Return ("Name");
      CheckLoggingMethod (
          () => _listener.RelationReading (_clientTransaction, _domainObject, relationEndPointDefinition, ValueAccess.Current),
          string.Format (
              "{0} RelationReading: {1} ({2}, {3})",
              _clientTransaction.ID,
              relationEndPointDefinition.PropertyName,
              ValueAccess.Current,
              _domainObject.ID));
    }

    [Test]
    public void RelationRead ()
    {
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      relationEndPointDefinition.Stub (n => n.PropertyName).Return ("Name");

      CheckLoggingMethod (
          () => _listener.RelationRead (_clientTransaction, _domainObject, relationEndPointDefinition, _domainObject, ValueAccess.Current),
          string.Format (
              "{0} RelationRead: {1}=={2} ({3}, {4})",
              _clientTransaction.ID,
              relationEndPointDefinition.PropertyName,
              _domainObject.ID,
              ValueAccess.Current,
              _domainObject.ID));
    }

    [Test]
    public void RelationRead_Collection ()
    {
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      relationEndPointDefinition.Stub (n => n.PropertyName).Return ("Items");

      var values =
          new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (new DomainObjectCollection (new[] { _domainObject2, _domainObject3 }, null));
      CheckLoggingMethod (
          () => _listener.RelationRead (_clientTransaction, _domainObject, relationEndPointDefinition, values, ValueAccess.Current),
          string.Format (
              "{0} RelationRead: {1} ({2}, {3}): {4}, {5}",
              _clientTransaction.ID,
              relationEndPointDefinition.PropertyName,
              ValueAccess.Current,
              _domainObject.ID,
              _domainObject2.ID,
              _domainObject3.ID));
    }

    [Test]
    public void RelationRead_LongCollection ()
    {
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      relationEndPointDefinition.Stub (n => n.PropertyName).Return ("Items");

      var values = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (
          new DomainObjectCollection (
              Enumerable.Range (0, 100).Select (i => LifetimeService.NewObject (_clientTransaction, typeof (Client), ParamList.Empty)),
              null));
      CheckLoggingMethod (
          () => _listener.RelationRead (_clientTransaction, _domainObject, relationEndPointDefinition, values, ValueAccess.Current),
          string.Format (
              "{0} RelationRead: {1} ({2}, {3}): {4}, +90",
              _clientTransaction.ID,
              relationEndPointDefinition.PropertyName,
              ValueAccess.Current,
              _domainObject.ID,
              string.Join (", ", values.Take (10))));
    }

    [Test]
    public void RelationChanging ()
    {
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      relationEndPointDefinition.Stub (n => n.PropertyName).Return ("Name");

      CheckLoggingMethod (
          () => _listener.RelationChanging (_clientTransaction, _domainObject, relationEndPointDefinition, _domainObject2, _domainObject3),
          string.Format (
              "{0} RelationChanging: {1}: {2}->{3} /{4}",
              _clientTransaction.ID,
              relationEndPointDefinition.PropertyName,
              _domainObject2.ID,
              _domainObject3.ID,
              _domainObject.ID));
    }

    [Test]
    public void RelationChanged ()
    {
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      relationEndPointDefinition.Stub (n => n.PropertyName).Return ("Name");

      CheckLoggingMethod (
          () => _listener.RelationChanged (_clientTransaction, _domainObject, relationEndPointDefinition, _domainObject2, _domainObject3),
          string.Format (
              "{0} RelationChanged: {1}: {2}->{3} /{4}",
              _clientTransaction.ID,
              relationEndPointDefinition.PropertyName,
              _domainObject2.ID,
              _domainObject3.ID,
              _domainObject.ID));
    }

    [Test]
    public void FilterQueryResult ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery>();
      queryStub.Stub (stub => stub.ID).Return ("a_query");
      queryStub.Stub (stub => stub.Statement).Return ("SELECT SMTH");

      var queryResult = new QueryResult<Order> (queryStub, new[] { DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1) });
      CheckLoggingMethod (
          () => _listener.FilterQueryResult (_clientTransaction, queryResult),
          string.Format ("{0} FilterQueryResult: a_query (SELECT SMTH): Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid", _clientTransaction.ID));
    }

    [Test]
    public void FilterCustomQueryResult ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery> ();
      queryStub.Stub (stub => stub.ID).Return ("a_query");
      queryStub.Stub (stub => stub.Statement).Return ("SELECT SMTH");

      var results = new[] { "item" };
      CheckLoggingMethod (
          () => _listener.FilterCustomQueryResult (_clientTransaction, queryStub, results),
          string.Format ("{0} FilterCustomQueryResult: a_query (SELECT SMTH)", _clientTransaction.ID));
    }

    [Test]
    public void TransactionCommitting ()
    {
      CheckLoggingMethod (
          () =>
          _listener.TransactionCommitting (
              _clientTransaction,
              new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject }),
              MockRepository.GenerateStub<ICommittingEventRegistrar>()),
          string.Format (
              "{0} TransactionCommitting: {1}",
              _clientTransaction.ID,
              _domainObject.ID));
    }

    [Test]
    public void TransactionCommitted ()
    {
      CheckLoggingMethod (
          () => _listener.TransactionCommitted (_clientTransaction, new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject })),
          string.Format (
              "{0} TransactionCommitted: {1}",
              _clientTransaction.ID,
              _domainObject.ID));
    }

    [Test]
    public void TransactionRollingBack ()
    {
      CheckLoggingMethod (
          () => _listener.TransactionRollingBack (_clientTransaction, new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject })),
          string.Format (
              "{0} TransactionRollingBack: {1}",
              _clientTransaction.ID,
              _domainObject.ID));
    }

    [Test]
    public void TransactionRolledBack ()
    {
      CheckLoggingMethod (
          () => _listener.TransactionRolledBack (_clientTransaction, new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject })),
          string.Format (
              "{0} TransactionRolledBack: {1}",
              _clientTransaction.ID,
              _domainObject.ID));
    }

    [Test]
    public void RelationEndPointMapRegistering ()
    {
      RelationEndPointID relationEndPointID;
      IObjectEndPoint relationEndPoint;
      using (_clientTransaction.EnterNonDiscardingScope())
      {
        relationEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_domainObject.ID, "ParentClient");
        relationEndPoint = RelationEndPointObjectMother.CreateRealObjectEndPoint (relationEndPointID);
      }

      CheckLoggingMethod (
          () => _listener.RelationEndPointMapRegistering (_clientTransaction, relationEndPoint),
          string.Format (
              "{0} RelationEndPointMapRegistering: {1}",
              _clientTransaction.ID,
              relationEndPointID));
    }

    [Test]
    public void RelationEndPointMapUnregistering ()
    {
      RelationEndPointID relationEndPointID;

      using (_clientTransaction.EnterNonDiscardingScope())
      {
        relationEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_domainObject.ID, "ParentClient");
      }

      CheckLoggingMethod (
          () => _listener.RelationEndPointMapUnregistering (_clientTransaction, relationEndPointID),
          string.Format (
              "{0} RelationEndPointMapUnregistering: {1}",
              _clientTransaction.ID,
              relationEndPointID));
    }

    [Test]
    public void RelationEndPointBecomingIncomplete ()
    {
      var relationEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_domainObject.ID, "ParentClient");

      CheckLoggingMethod (
          () => _listener.RelationEndPointBecomingIncomplete (_clientTransaction, relationEndPointID),
          string.Format (
              "{0} RelationEndPointBecomingIncomplete: {1}",
              _clientTransaction.ID,
              relationEndPointID));
    }

    [Test]
    public void ObjectMarkedInvalid ()
    {
      CheckLoggingMethod (
          () => _listener.ObjectMarkedInvalid (_clientTransaction, _domainObject),
          string.Format (
              "{0} ObjectMarkedInvalid: {1}",
              _clientTransaction.ID,
              _domainObject.ID));
    }

    [Test]
    public void ObjectMarkedNotInvalid ()
    {
      CheckLoggingMethod (
          () => _listener.ObjectMarkedNotInvalid (_clientTransaction, _domainObject),
          string.Format (
              "{0} ObjectMarkedNotInvalid: {1}",
              _clientTransaction.ID,
              _domainObject.ID));
    }

    [Test]
    public void DataContainerMapRegistering ()
    {
      CheckLoggingMethod (
          () => _listener.DataContainerMapRegistering (_clientTransaction, _dataContainer),
          string.Format (
              "{0} DataContainerMapRegistering: {1}",
              _clientTransaction.ID,
              _dataContainer.ID));
    }

    [Test]
    public void DataContainerMapUnregistering ()
    {
      CheckLoggingMethod (
          () => _listener.DataContainerMapUnregistering (_clientTransaction, _dataContainer),
          string.Format (
              "{0} DataContainerMapUnregistering: {1}",
              _clientTransaction.ID,
              _dataContainer.ID));
    }

    [Test]
    public void DataContainerStateUpdated ()
    {
      var newDataContainerState = new StateType();
      CheckLoggingMethod (
          () => _listener.DataContainerStateUpdated (_clientTransaction, _dataContainer, newDataContainerState),
          string.Format (
              "{0} DataContainerStateUpdated: {1} {2}",
              _clientTransaction.ID,
              _dataContainer.ID,
              newDataContainerState));
    }

    [Test]
    public void VirtualRelationEndPointStateUpdated ()
    {
      RelationEndPointID relationEndPointID;

      using (_clientTransaction.EnterNonDiscardingScope())
      {
        relationEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_domainObject.ID, "ParentClient");
      }

      CheckLoggingMethod (
          () => _listener.VirtualRelationEndPointStateUpdated (_clientTransaction, relationEndPointID, false),
          string.Format (
              "{0} VirtualRelationEndPointStateUpdated: {1} {2}",
              _clientTransaction.ID,
              relationEndPointID,
              false));
    }


    private IEnumerable<LoggingEvent> GetLoggingEvents ()
    {
      return _memoryAppender.GetEvents();
    }

    private void CheckLoggingMethod (Action action, string expectedMessage)
    {
      action();
      var loggingEvents = GetLoggingEvents();

      Assert.That (loggingEvents.Last().RenderedMessage, Is.EqualTo (expectedMessage));
    }
  }
}