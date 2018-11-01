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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  public abstract class EnsureDataAvailableTestBase : ClientTransactionBaseTest
  {
    private DomainObject _loadedObject1;
    private DomainObject _loadedObject2;
    private DomainObject _notLoadedObject1;
    private DomainObject _notLoadedObject2;
    private DomainObject _invalidObject;
    private DomainObject _notLoadedNonExistingObject;
    private IClientTransactionListener _listenerMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _loadedObject1 = DomainObjectMother.GetUnchangedObject (TestableClientTransaction, DomainObjectIDs.Order1);
      _loadedObject2 = DomainObjectMother.GetUnchangedObject (TestableClientTransaction, DomainObjectIDs.Order3);
      _notLoadedObject1 = DomainObjectMother.GetNotLoadedObject (TestableClientTransaction, DomainObjectIDs.Order4);
      _notLoadedObject2 = DomainObjectMother.GetNotLoadedObject (TestableClientTransaction, DomainObjectIDs.Order5);
      _invalidObject = DomainObjectMother.GetInvalidObject (TestableClientTransaction);
      _notLoadedNonExistingObject = DomainObjectMother.GetNotLoadedObject (TestableClientTransaction, new ObjectID(typeof (ClassWithAllDataTypes), Guid.NewGuid()));

      _listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock (TestableClientTransaction);
    }

    protected DomainObject LoadedObject1
    {
      get { return _loadedObject1; }
    }

    protected DomainObject LoadedObject2
    {
      get { return _loadedObject2; }
    }

    protected DomainObject NotLoadedObject1
    {
      get { return _notLoadedObject1; }
    }

    protected DomainObject NotLoadedObject2
    {
      get { return _notLoadedObject2; }
    }

    protected DomainObject InvalidObject
    {
      get { return _invalidObject; }
    }

    protected DomainObject NotLoadedNonExistingObject
    {
      get { return _notLoadedNonExistingObject; }
    }

    protected IClientTransactionListener ListenerMock
    {
      get { return _listenerMock; }
    }

    protected void CheckLoaded (DomainObject domainObject)
    {
      Assert.That (domainObject.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Not.Null);
    }

    protected void CheckNotLoaded (DomainObject domainObject)
    {
      Assert.That (domainObject.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Null);
    }

    protected void CheckInvalid (DomainObject domainObject)
    {
      Assert.That (domainObject.State, Is.EqualTo (StateType.Invalid));
      Assert.That (TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Null);
    }
  }
}