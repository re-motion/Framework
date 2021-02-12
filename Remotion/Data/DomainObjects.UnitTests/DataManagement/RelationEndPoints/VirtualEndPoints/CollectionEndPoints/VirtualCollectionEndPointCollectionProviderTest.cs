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
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class VirtualCollectionEndPointCollectionProviderTest : StandardMappingTest
  {
    private IVirtualEndPointProvider _virtualEndPointProviderMock;
    private RelationEndPointID _endPointID;

    private VirtualCollectionEndPointCollectionProvider _provider;

    public override void SetUp ()
    {
      base.SetUp();

      _virtualEndPointProviderMock = MockRepository.GenerateStrictMock<IVirtualEndPointProvider>();

      _provider = new VirtualCollectionEndPointCollectionProvider (_virtualEndPointProviderMock);

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Product1, typeof (Product), "ProductReviews");
    }

    [Test]
    public void GetCollection ()
    {
      var result = _provider.GetCollection (_endPointID);

      Assert.That (result, Is.TypeOf<VirtualObjectList<ProductReview>>());

      var virtualCollectionData = VirtualCollectionDataTestHelper.GetDataStrategy (result);
      Assert.That (virtualCollectionData, Is.TypeOf<EndPointDelegatingVirtualCollectionData>());
      Assert.That (
          ((EndPointDelegatingVirtualCollectionData) virtualCollectionData).VirtualEndPointProvider,
          Is.SameAs (_virtualEndPointProviderMock));
      Assert.That (virtualCollectionData.AssociatedEndPointID, Is.SameAs (_endPointID));

      Assert.That (result, Is.SameAs (_provider.GetCollection (_endPointID)));
    }

    [Test]
    public void Serialization ()
    {
      var instance = new VirtualCollectionEndPointCollectionProvider (new SerializableVirtualEndPointProviderFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.VirtualEndPointProvider, Is.Not.Null);
    }
  }
}