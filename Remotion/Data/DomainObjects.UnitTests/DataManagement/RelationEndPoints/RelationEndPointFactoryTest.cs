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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.UnitTesting;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RelationEndPointFactoryTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;
    private Mock<IRelationEndPointProvider> _endPointProviderStub;
    private Mock<ILazyLoader> _lazyLoaderStub;
    private Mock<IClientTransactionEventSink> _transactionEventSinkStub;
    private Mock<IVirtualObjectEndPointDataManagerFactory> _virtualObjectEndPointDataManagerFactoryStub;
    private Mock<IDomainObjectCollectionEndPointDataManagerFactory> _domainObjectCollectionEndPointDataManagerFactoryStub;
    private Mock<IDomainObjectCollectionEndPointCollectionProvider> _domainObjectCollectionEndPointCollectionProviderStub;
    private Mock<IAssociatedDomainObjectCollectionDataStrategyFactory> _associatedDomainObjectCollectionStrategyFactoryStub;
    private Mock<IVirtualCollectionEndPointCollectionProvider> _virtualCollectionEndPointCollectionProviderStub;
    private Mock<IVirtualCollectionEndPointDataManagerFactory> _virtualCollectionEndPointDataManagerFactoryStub;

    private RelationEndPointFactory _factory;

    public override void SetUp ()
    {
      base.SetUp();

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _endPointProviderStub = new Mock<IRelationEndPointProvider>();
      _lazyLoaderStub = new Mock<ILazyLoader>();
      _transactionEventSinkStub = new Mock<IClientTransactionEventSink>();

      var virtualObjectEndPointDataManagerStub = new Mock<IVirtualObjectEndPointDataManager>();
      virtualObjectEndPointDataManagerStub.Setup(stub => stub.OriginalOppositeEndPoint).Returns((IRealObjectEndPoint)null);
      _virtualObjectEndPointDataManagerFactoryStub = new Mock<IVirtualObjectEndPointDataManagerFactory>();
      _virtualObjectEndPointDataManagerFactoryStub
          .Setup(stub => stub.CreateEndPointDataManager(It.IsAny<RelationEndPointID>()))
          .Returns(virtualObjectEndPointDataManagerStub.Object);

      var domainObjectCollectionEndPointDataManagerStub = new Mock<IDomainObjectCollectionEndPointDataManager>();
      domainObjectCollectionEndPointDataManagerStub.Setup(stub => stub.OriginalOppositeEndPoints).Returns(new IRealObjectEndPoint[0]);
      _domainObjectCollectionEndPointDataManagerFactoryStub = new Mock<IDomainObjectCollectionEndPointDataManagerFactory>();
      _domainObjectCollectionEndPointDataManagerFactoryStub
          .Setup(stub => stub.CreateEndPointDataManager(It.IsAny<RelationEndPointID>()))
          .Returns(domainObjectCollectionEndPointDataManagerStub.Object);

      _domainObjectCollectionEndPointCollectionProviderStub = new Mock<IDomainObjectCollectionEndPointCollectionProvider>();
      _associatedDomainObjectCollectionStrategyFactoryStub = new Mock<IAssociatedDomainObjectCollectionDataStrategyFactory>();

      var virtualCollectionEndPointDataManagerStub= new Mock<IVirtualCollectionEndPointDataManager>();
      _virtualCollectionEndPointDataManagerFactoryStub = new Mock<IVirtualCollectionEndPointDataManagerFactory>();
      _virtualCollectionEndPointDataManagerFactoryStub
          .Setup(stub => stub.CreateEndPointDataManager(It.IsAny<RelationEndPointID>()))
          .Returns(virtualCollectionEndPointDataManagerStub.Object);

      _virtualCollectionEndPointCollectionProviderStub = new Mock<IVirtualCollectionEndPointCollectionProvider>();

      _factory = new RelationEndPointFactory(
          _clientTransaction,
          _endPointProviderStub.Object,
          _lazyLoaderStub.Object,
          _transactionEventSinkStub.Object,
          _virtualObjectEndPointDataManagerFactoryStub.Object,
          _domainObjectCollectionEndPointDataManagerFactoryStub.Object,
          _domainObjectCollectionEndPointCollectionProviderStub.Object,
          _associatedDomainObjectCollectionStrategyFactoryStub.Object,
          _virtualCollectionEndPointCollectionProviderStub.Object,
          _virtualCollectionEndPointDataManagerFactoryStub.Object);
    }

    [Test]
    public void CreateRealObjectEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer");
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);

      var endPoint = _factory.CreateRealObjectEndPoint(endPointID, dataContainer);

      Assert.That(endPoint, Is.TypeOf<RealObjectEndPoint>());
      Assert.That(endPoint.ClientTransaction, Is.SameAs(_clientTransaction));
      Assert.That(endPoint.ID, Is.EqualTo(endPointID));
      Assert.That(((RealObjectEndPoint)endPoint).ForeignKeyDataContainer, Is.SameAs(dataContainer));
      Assert.That(((RealObjectEndPoint)endPoint).EndPointProvider, Is.SameAs(_endPointProviderStub.Object));
      Assert.That(((RealObjectEndPoint)endPoint).TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
    }

    [Test]
    public void CreateRealObjectEndPoint_NonRealEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket");
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);

      Assert.That(
          () => _factory.CreateRealObjectEndPoint(endPointID, dataContainer),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("End point ID must refer to a non-virtual end point.", "id"));
    }

    [Test]
    public void CreateRealObjectEndPoint_NullEndPoint ()
    {
      var existingEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket");
      var endPointID = RelationEndPointID.Create(null, existingEndPointID.Definition);
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);

      Assert.That(
          () => _factory.CreateRealObjectEndPoint(endPointID, dataContainer),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("End point ID must have a non-null ObjectID.", "endPointID"));
    }

    [Test]
    public void CreateVirtualObjectEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket");

      var endPoint = _factory.CreateVirtualObjectEndPoint(endPointID);

      Assert.That(endPoint, Is.TypeOf<VirtualObjectEndPoint>());
      Assert.That(endPoint.ClientTransaction, Is.SameAs(_clientTransaction));
      Assert.That(endPoint.ID, Is.EqualTo(endPointID));
      Assert.That(((VirtualObjectEndPoint)endPoint).LazyLoader, Is.SameAs(_lazyLoaderStub.Object));
      Assert.That(((VirtualObjectEndPoint)endPoint).EndPointProvider, Is.SameAs(_endPointProviderStub.Object));
      Assert.That(((VirtualObjectEndPoint)endPoint).TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
      Assert.That(((VirtualObjectEndPoint)endPoint).DataManagerFactory, Is.SameAs(_virtualObjectEndPointDataManagerFactoryStub.Object));
      Assert.That(endPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void CreateVirtualObjectEndPoint_NonVirtualEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer");

      Assert.That(
          () => _factory.CreateVirtualObjectEndPoint(endPointID),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("End point ID must refer to a virtual end point.", "id"));
    }

    [Test]
    public void CreateVirtualObjectEndPoint_NonObjectEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");

      Assert.That(
          () => _factory.CreateVirtualObjectEndPoint(endPointID),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("End point ID must refer to an end point with cardinality 'One'.", "id"));
    }

    [Test]
    public void CreateVirtualCollectionEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Product1, typeof(Product), "Reviews");

      var endPoint = _factory.CreateVirtualCollectionEndPoint(endPointID);

      Assert.That(endPoint, Is.TypeOf<VirtualCollectionEndPoint>());
      Assert.That(endPoint.ClientTransaction, Is.SameAs(_clientTransaction));
      Assert.That(endPoint.ID, Is.EqualTo(endPointID));
      Assert.That(
          ((VirtualCollectionEndPoint)endPoint).CollectionManager,
          Is.TypeOf<VirtualCollectionEndPointCollectionManager>()
              .With.Property<VirtualCollectionEndPointCollectionManager>(p => p.CollectionProvider).SameAs(_virtualCollectionEndPointCollectionProviderStub.Object));
      Assert.That(((VirtualCollectionEndPoint)endPoint).LazyLoader, Is.SameAs(_lazyLoaderStub.Object));
      Assert.That(((VirtualCollectionEndPoint)endPoint).EndPointProvider, Is.SameAs(_endPointProviderStub.Object));
      Assert.That(((VirtualCollectionEndPoint)endPoint).TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
      Assert.That(((VirtualCollectionEndPoint)endPoint).DataManagerFactory, Is.SameAs(_virtualCollectionEndPointDataManagerFactoryStub.Object));
      Assert.That(endPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void CreateVirtualCollectionEndPoint_NonCollectionEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer");

      Assert.That(
          () => _factory.CreateVirtualCollectionEndPoint(endPointID),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("End point ID must refer to an end point with cardinality 'Many'.", "id"));
    }

    [Test]
    public void CreateVirtualCollectionEndPoint_AnonymousEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateAnonymousEndPointID();

      Assert.That(
          () => _factory.CreateVirtualCollectionEndPoint(endPointID),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("End point ID must not refer to an anonymous end point.", "id"));
    }

    [Test]
    public void CreateDomainObjectCollectionEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");

      var endPoint = _factory.CreateDomainObjectCollectionEndPoint(endPointID);

      Assert.That(endPoint, Is.TypeOf<DomainObjectCollectionEndPoint>());
      Assert.That(endPoint.ClientTransaction, Is.SameAs(_clientTransaction));
      Assert.That(endPoint.ID, Is.EqualTo(endPointID));
      Assert.That(
          ((DomainObjectCollectionEndPoint)endPoint).CollectionManager,
          Is.TypeOf<DomainObjectCollectionEndPointCollectionManager>()
              .With.Property<DomainObjectCollectionEndPointCollectionManager>(p => p.DomainObjectCollectionProvider)
              .SameAs(_domainObjectCollectionEndPointCollectionProviderStub.Object)
              .And.Property<DomainObjectCollectionEndPointCollectionManager>(p => p.DataStrategyFactory)
              .SameAs(_associatedDomainObjectCollectionStrategyFactoryStub.Object));
      Assert.That(((DomainObjectCollectionEndPoint)endPoint).LazyLoader, Is.SameAs(_lazyLoaderStub.Object));
      Assert.That(((DomainObjectCollectionEndPoint)endPoint).EndPointProvider, Is.SameAs(_endPointProviderStub.Object));
      Assert.That(((DomainObjectCollectionEndPoint)endPoint).TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
      Assert.That(((DomainObjectCollectionEndPoint)endPoint).DataManagerFactory, Is.SameAs(_domainObjectCollectionEndPointDataManagerFactoryStub.Object));
      Assert.That(endPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void CreateDomainObjectCollectionEndPoint_NonCollectionEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer");

      Assert.That(
          () => _factory.CreateDomainObjectCollectionEndPoint(endPointID),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("End point ID must refer to an end point with cardinality 'Many'.", "id"));
    }

    [Test]
    public void CreateDomainObjectCollectionEndPoint_AnonymousEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateAnonymousEndPointID();

      Assert.That(
          () => _factory.CreateDomainObjectCollectionEndPoint(endPointID),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("End point ID must not refer to an anonymous end point.", "id"));
    }
  }
}
