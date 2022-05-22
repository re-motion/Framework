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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  public abstract class ObjectEndPointSetCommandTestBase : ClientTransactionBaseTest
  {
    private Mock<IRelationEndPointProvider> _endPointProviderStub;
    private bool _oppositeObjectSetterCalled;
    private IDomainObject _oppositeObjectSetterObject;
    private Action<IDomainObject> _oppositeObjectSetter;
    private Mock<IClientTransactionEventSink> _transactionEventSinkWithMock;

    protected Mock<IRelationEndPointProvider> EndPointProviderStub
    {
      get { return _endPointProviderStub; }
    }

    public bool OppositeObjectSetterCalled
    {
      get { return _oppositeObjectSetterCalled; }
    }

    public IDomainObject OppositeObjectSetterObject
    {
      get { return _oppositeObjectSetterObject; }
    }

    public Action<IDomainObject> OppositeObjectSetter
    {
      get { return _oppositeObjectSetter; }
    }

    public Mock<IClientTransactionEventSink> TransactionEventSinkWithMock
    {
      get { return _transactionEventSinkWithMock; }
    }

    public override void SetUp ()
    {
      base.SetUp();

      _endPointProviderStub = new Mock<IRelationEndPointProvider>();
      _oppositeObjectSetterCalled = false;
      _oppositeObjectSetter = id =>
      {
        _oppositeObjectSetterCalled = true;
        _oppositeObjectSetterObject = id;
      };
      _transactionEventSinkWithMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);
    }
  }
}
