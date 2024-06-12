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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.UnitTesting;
using Remotion.Development.Moq.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class StateUpdateRaisingRelationEndPointFactoryDecoratorTest : StandardMappingTest
  {
    private Mock<IRelationEndPointFactory> _innerFactoryMock;
    private Mock<IVirtualEndPointStateUpdateListener> _listenerStub;

    private StateUpdateRaisingRelationEndPointFactoryDecorator _decorator;
    private DecoratorTestHelper<IRelationEndPointFactory> _decoratorTestHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _innerFactoryMock = new Mock<IRelationEndPointFactory>(MockBehavior.Strict);
      _listenerStub = new Mock<IVirtualEndPointStateUpdateListener>();

      _decorator = new StateUpdateRaisingRelationEndPointFactoryDecorator(_innerFactoryMock.Object, _listenerStub.Object);
      _decoratorTestHelper = new DecoratorTestHelper<IRelationEndPointFactory>(_decorator, _innerFactoryMock);
    }

    [Test]
    public void CreateRealObjectEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer");
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);

      _decoratorTestHelper.CheckDelegation(
          f => f.CreateRealObjectEndPoint(endPointID, dataContainer), new Mock<IRealObjectEndPoint>().Object);
    }

    [Test]
    public void CreateVirtualObjectEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket");
      var fakeResult = new Mock<IVirtualObjectEndPoint>();
      _decoratorTestHelper.CheckDelegation(
          f => f.CreateVirtualObjectEndPoint(endPointID),
          fakeResult.Object,
          result => Assert.That(
              result,
              Is.TypeOf<StateUpdateRaisingVirtualObjectEndPointDecorator>()
                .With.Property<StateUpdateRaisingVirtualObjectEndPointDecorator>(d => d.Listener).SameAs(_listenerStub.Object)
                .And.Property<StateUpdateRaisingVirtualObjectEndPointDecorator>(d => d.InnerEndPoint).SameAs(fakeResult.Object)));
    }

    [Test]
    public void CreateVirtualCollectionEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Product1, typeof(Product), "Reviews");

      var fakeResult = new Mock<IVirtualCollectionEndPoint>();
      _decoratorTestHelper.CheckDelegation(
          f => f.CreateVirtualCollectionEndPoint(endPointID),
          fakeResult.Object,
          result => Assert.That(
              result,
              Is.TypeOf<StateUpdateRaisingVirtualCollectionEndPointDecorator>()
                  .With.Property<StateUpdateRaisingVirtualCollectionEndPointDecorator>(d => d.Listener).SameAs(_listenerStub.Object)
                  .And.Property<StateUpdateRaisingVirtualCollectionEndPointDecorator>(d => d.InnerEndPoint).SameAs(fakeResult.Object)));
    }

    [Test]
    public void CreateDomainObjectCollectionEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");

      var fakeResult = new Mock<IDomainObjectCollectionEndPoint>();
      _decoratorTestHelper.CheckDelegation(
          f => f.CreateDomainObjectCollectionEndPoint(endPointID),
          fakeResult.Object,
          result => Assert.That(
              result,
              Is.TypeOf<StateUpdateRaisingDomainObjectCollectionEndPointDecorator>()
                .With.Property<StateUpdateRaisingDomainObjectCollectionEndPointDecorator>(d => d.Listener).SameAs(_listenerStub.Object)
                .And.Property<StateUpdateRaisingDomainObjectCollectionEndPointDecorator>(d => d.InnerEndPoint).SameAs(fakeResult.Object)));
    }
  }
}
