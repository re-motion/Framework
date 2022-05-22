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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointDeleteCommandTest : ClientTransactionBaseTest
  {
    private ObjectEndPoint _endPoint;
    private RelationEndPointID _endPointID;
    private DomainObject _domainObject;

    private bool _oppositeObjectNullSetterCalled;
    private Action _oppositeObjectNullSetter;
    private Mock<IClientTransactionEventSink> _transactionEventSinkWithMock;

    private ObjectEndPointDeleteCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderTicket");
      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint(_endPointID, DomainObjectIDs.OrderTicket1);
      _domainObject = (DomainObject)_endPoint.GetDomainObject();

      _oppositeObjectNullSetterCalled = false;
      _oppositeObjectNullSetter = () =>
      {
        _oppositeObjectNullSetterCalled = true;
      };

      _transactionEventSinkWithMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);
      _command = new ObjectEndPointDeleteCommand(_endPoint, _oppositeObjectNullSetter, _transactionEventSinkWithMock.Object);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_command.ModifiedEndPoint, Is.SameAs(_endPoint));
      Assert.That(_command.OldRelatedObject, Is.Null);
      Assert.That(_command.NewRelatedObject, Is.Null);
    }

    [Test]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullObjectEndPoint(TestableClientTransaction, _endPointID.Definition);
      Assert.That(
          () => new ObjectEndPointDeleteCommand(endPoint, () => { }, _transactionEventSinkWithMock.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Modified end point is null, a NullEndPointModificationCommand is needed.",
                  "modifiedEndPoint"));
    }

    [Test]
    public void Begin_NoEvents ()
    {
      _command.Begin();
    }

    [Test]
    public void End_NoEvents ()
    {
      _command.End();
    }

    [Test]
    public void Perform ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      _domainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      _domainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      Assert.That(_oppositeObjectNullSetterCalled, Is.False);
      Assert.That(_endPoint.HasBeenTouched, Is.False);

      _command.Perform();

      Assert.That(relationChangingCalled, Is.False); // operation was not started
      Assert.That(relationChangedCalled, Is.False); // operation was not finished

      Assert.That(_oppositeObjectNullSetterCalled, Is.True);
      Assert.That(_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var bidirectionalModification = _command.ExpandToAllRelatedObjects();

      var steps = bidirectionalModification.GetNestedCommands();
      Assert.That(steps.Count, Is.EqualTo(1));
      Assert.That(steps[0], Is.SameAs(_command));
    }
  }
}
