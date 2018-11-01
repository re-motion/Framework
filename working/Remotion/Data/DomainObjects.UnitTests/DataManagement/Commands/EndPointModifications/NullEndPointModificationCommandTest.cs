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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class NullEndPointModificationCommandTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IRelationEndPoint _endPointMock;
    private NullEndPointModificationCommand _command;
    private RelationEndPointID _id;

    public override void SetUp ()
    {
      base.SetUp ();
      _mockRepository = new MockRepository ();
      _id = RelationEndPointID.Create(DomainObjectIDs.Computer1,
                   ReflectionMappingHelper.GetPropertyName (typeof (Computer), "Employee"));

      _endPointMock = _mockRepository.StrictMock<IRelationEndPoint> ();

      _command = new NullEndPointModificationCommand (_endPointMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.AffectedEndPoint, Is.SameAs (_endPointMock));
    }

    [Test]
    public void Initialization_FromNullObjectEndPoint ()
    {
      var endPoint = new NullObjectEndPoint (TestableClientTransaction, _id.Definition);
      var command = (NullEndPointModificationCommand) endPoint.CreateSetCommand (Employee.NewObject());
      Assert.That (command.AffectedEndPoint, Is.SameAs (endPoint));
    }

    [Test]
    public void GetAllExceptions ()
    {
      Assert.That (_command.GetAllExceptions (), Is.Empty);
    }

    [Test]
    public void Begin_DoesNothing ()
    {
      _mockRepository.ReplayAll ();

      _command.Begin ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PerformDoesNothing ()
    {
      _mockRepository.ReplayAll ();

      _command.Perform ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void End_DoesNothing ()
    {
      _mockRepository.ReplayAll ();

      _command.End ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var result = ((IDataManagementCommand) _command).ExpandToAllRelatedObjects ();

      Assert.That (result.GetNestedCommands(), Is.EqualTo (new[] { _command }));
    }
  }
}
