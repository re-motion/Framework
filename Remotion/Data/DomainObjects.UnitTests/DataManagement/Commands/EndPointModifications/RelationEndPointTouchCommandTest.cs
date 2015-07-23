
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
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class RelationEndPointTouchCommandTest : StandardMappingTest
  {
    private TestableClientTransaction _transaction;
    private IRelationEndPoint _endPoint;
    private RelationEndPointTouchCommand _command;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new TestableClientTransaction ();

      var id = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      _endPoint = _transaction.ExecuteInScope (() => RelationEndPointObjectMother.CreateObjectEndPoint (id, null));

      _command = new RelationEndPointTouchCommand (_endPoint);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.EndPoint, Is.SameAs (_endPoint));
    }

    [Test]
    public void GetAllExceptions ()
    {
      Assert.That (_command.GetAllExceptions (), Is.Empty);
    }

    [Test]
    public void Begin_DoesNothing ()
    {
      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents (_transaction);

      _command.Begin ();
    }

    [Test]
    public void End_DoesNothing ()
    {
      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents (_transaction);

      _command.End ();
    }

    [Test]
    public void Perform ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _command.Perform ();

      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var result = ((IDataManagementCommand) _command).ExpandToAllRelatedObjects ();
      Assert.That (result.GetNestedCommands(), Is.EqualTo (new[] { _command }));
    }
  }
}
