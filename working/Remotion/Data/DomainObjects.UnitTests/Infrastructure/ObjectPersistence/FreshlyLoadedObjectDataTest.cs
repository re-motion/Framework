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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class FreshlyLoadedObjectDataTest : StandardMappingTest
  {
    private DataContainer _dataContainer;
    private FreshlyLoadedObjectData _loadedObjectData;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _loadedObjectData = new FreshlyLoadedObjectData (_dataContainer);
    }
    
    [Test]
    public void Initialization ()
    {
      Assert.That (_loadedObjectData.FreshlyLoadedDataContainer, Is.SameAs (_dataContainer));
      Assert.That (_loadedObjectData.ObjectID, Is.EqualTo (_dataContainer.ID));
    }

    [Test]
    public void Initialization_WithClientTransaction_Throws ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer (ClientTransaction.CreateRootTransaction (), dataContainer);

      Assert.That (
          () => new FreshlyLoadedObjectData (dataContainer),
          Throws.ArgumentException.With.Message.EqualTo (
              "The DataContainer must not have been registered with a ClientTransaction.\r\nParameter name: freshlyLoadedDataContainer"));
    }

    [Test]
    public void Initialization_WithDomainObject_Throws ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dataContainer.SetDomainObject (DomainObjectMother.CreateFakeObject<Order> (dataContainer.ID));

      Assert.That (
          () => new FreshlyLoadedObjectData (dataContainer),
          Throws.ArgumentException.With.Message.EqualTo (
              "The DataContainer must not have been registered with a DomainObject.\r\nParameter name: freshlyLoadedDataContainer"));
    }

    [Test]
    public void GetDomainObjectReference_BeforeRegistration ()
    {
      Assert.That (
          () => _loadedObjectData.GetDomainObjectReference (), 
          Throws.InvalidOperationException.With.Message.EqualTo (
            "Cannot obtain a DomainObject reference for a freshly loaded object that has not yet been registered."));
    }

    [Test]
    public void GetDomainObjectReference_AfterRegistration ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> (_dataContainer.ID);
      _dataContainer.SetDomainObject (domainObject);

      Assert.That (_loadedObjectData.GetDomainObjectReference (), Is.SameAs (domainObject));
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ILoadedObjectVisitor>();
      visitorMock.Expect (mock => mock.VisitFreshlyLoadedObject (_loadedObjectData));
      visitorMock.Replay();

      _loadedObjectData.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _loadedObjectData).IsNull, Is.False);
    }
  }
}