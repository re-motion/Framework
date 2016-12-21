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
  public class AlreadyExistingLoadedObjectDataTest : StandardMappingTest
  {
    private DataContainer _dataContainer;
    private AlreadyExistingLoadedObjectData _loadedObjectData;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataContainer.SetDomainObject (DomainObjectMother.CreateFakeObject<Order> (_dataContainer.ID));
      ClientTransactionTestHelper.RegisterDataContainer (ClientTransaction.CreateRootTransaction (), _dataContainer);

      _loadedObjectData = new AlreadyExistingLoadedObjectData (_dataContainer);
    }
    
    [Test]
    public void Initialization ()
    {
      Assert.That (_loadedObjectData.ExistingDataContainer, Is.SameAs (_dataContainer));
      Assert.That (_loadedObjectData.ObjectID, Is.EqualTo (_dataContainer.ID));
    }

    [Test]
    public void Initialization_WithoutClientTransaction_Throws ()
    {
      var existingDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      existingDataContainer.SetDomainObject (DomainObjectMother.CreateFakeObject<Order> (existingDataContainer.ID));
      
      Assert.That (
          () => new AlreadyExistingLoadedObjectData (existingDataContainer),
          Throws.ArgumentException.With.Message.EqualTo (
              "The DataContainer must have been registered with a ClientTransaction.\r\nParameter name: existingDataContainer"));
    }

    [Test]
    public void GetDomainObjectReference ()
    {
      var reference = _loadedObjectData.GetDomainObjectReference();

      Assert.That (reference, Is.SameAs (_dataContainer.DomainObject));
    }
    
    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ILoadedObjectVisitor>();
      visitorMock.Expect (mock => mock.VisitAlreadyExistingLoadedObject (_loadedObjectData));
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