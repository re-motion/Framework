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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.RhinoMocks.UnitTesting;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class StateUpdateRaisingRelationEndPointFactoryDecoratorTest : StandardMappingTest
  {
    private IRelationEndPointFactory _innerFactoryMock;
    private IVirtualEndPointStateUpdateListener _listenerStub;

    private StateUpdateRaisingRelationEndPointFactoryDecorator _decorator;
    private DecoratorTestHelper<IRelationEndPointFactory> _decoratorTestHelper;

    public override void SetUp ()
    {
      base.SetUp ();

      _innerFactoryMock = MockRepository.GenerateStrictMock<IRelationEndPointFactory>();
      _listenerStub = MockRepository.GenerateStub<IVirtualEndPointStateUpdateListener>();

      _decorator = new StateUpdateRaisingRelationEndPointFactoryDecorator (_innerFactoryMock, _listenerStub);
      _decoratorTestHelper = new DecoratorTestHelper<IRelationEndPointFactory> (_decorator, _innerFactoryMock);
    }

    [Test]
    public void CreateRealObjectEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer");
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);

      _decoratorTestHelper.CheckDelegation (
          f => f.CreateRealObjectEndPoint (endPointID, dataContainer), MockRepository.GenerateStub<IRealObjectEndPoint>());
    }

    [Test]
    public void CreateVirtualObjectEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");
      var fakeResult = MockRepository.GenerateStub<IVirtualObjectEndPoint> ();
      _decoratorTestHelper.CheckDelegation (
          f => f.CreateVirtualObjectEndPoint (endPointID),
          fakeResult,
          result => Assert.That (
              result,
              Is.TypeOf<StateUpdateRaisingVirtualObjectEndPointDecorator> ()
                .With.Property<StateUpdateRaisingVirtualObjectEndPointDecorator> (d => d.Listener).SameAs (_listenerStub)
                .And.Property<StateUpdateRaisingVirtualObjectEndPointDecorator> (d => d.InnerEndPoint).SameAs (fakeResult)));
    }

    [Test]
    public void CreateCollectionEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      var fakeResult = MockRepository.GenerateStub<ICollectionEndPoint> ();
      _decoratorTestHelper.CheckDelegation (
          f => f.CreateCollectionEndPoint (endPointID), 
          fakeResult,
          result => Assert.That (
              result, 
              Is.TypeOf<StateUpdateRaisingCollectionEndPointDecorator>()
                .With.Property<StateUpdateRaisingCollectionEndPointDecorator> (d => d.Listener).SameAs (_listenerStub)
                .And.Property<StateUpdateRaisingCollectionEndPointDecorator> (d => d.InnerEndPoint).SameAs (fakeResult)));
    }

    [Test]
    public void Serialization ()
    {
      var innerFactory = new SerializableRelationEndPointFactoryFake();
      var listener = new SerializableVirtualEndPointStateUpdateListenerFake();
      var decorator = new StateUpdateRaisingRelationEndPointFactoryDecorator (innerFactory, listener);

      var deserializedInstance = Serializer.SerializeAndDeserialize (decorator);

      Assert.That (deserializedInstance.InnerFactory, Is.Not.Null);
      Assert.That (deserializedInstance.Listener, Is.Not.Null);
    }
  }
}