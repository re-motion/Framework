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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.StorageProviderCommands;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.StorageProviderCommands
{
  [TestFixture]
  public class SingleDataContainerAssociateWithIDCommandTest : StandardMappingTest
  {
    private ObjectID _expectedID;
    private IStorageProviderCommand<DataContainer, object> _innerCommandMock;

    private SingleDataContainerAssociateWithIDCommand<object> _associateCommand;

    private object _fakeContext;

    public override void SetUp ()
    {
      base.SetUp ();

      _expectedID = DomainObjectIDs.Order1;
      _innerCommandMock = MockRepository.GenerateStrictMock<IStorageProviderCommand<DataContainer, object>>();

      _associateCommand = new SingleDataContainerAssociateWithIDCommand<object> (_expectedID, _innerCommandMock);

      _fakeContext = "context";
    }

    [Test]
    public void Execute_MatchingDataContainer ()
    {
      var dataContainer = DataContainerObjectMother.Create (_expectedID);
      _innerCommandMock.Expect (mock => mock.Execute (_fakeContext)).Return (dataContainer);

      var result = _associateCommand.Execute (_fakeContext);

      _innerCommandMock.VerifyAllExpectations();
      Assert.That (result.ObjectID, Is.EqualTo (_expectedID));
      Assert.That (result.LocatedObject, Is.SameAs (dataContainer));
    }

    [Test]
    public void Execute_NonMatchingDataContainer ()
    {
      var dataContainer = DataContainerObjectMother.Create (DomainObjectIDs.Order3);
      _innerCommandMock.Expect (mock => mock.Execute (_fakeContext)).Return (dataContainer);

      Assert.That (
          () => _associateCommand.Execute (_fakeContext),
          Throws.TypeOf<PersistenceException> ().With.Message.EqualTo (
            "The ObjectID of the loaded DataContainer 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' and the expected ObjectID "
            + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' differ."));
    }
    
    [Test]
    public void Execute_Null ()
    {
      _innerCommandMock.Expect (mock => mock.Execute (_fakeContext)).Return (null);

      var result = _associateCommand.Execute (_fakeContext);

      _innerCommandMock.VerifyAllExpectations ();
      Assert.That (result.ObjectID, Is.EqualTo (_expectedID));
      Assert.That (result.LocatedObject, Is.Null);
    }
  }
}