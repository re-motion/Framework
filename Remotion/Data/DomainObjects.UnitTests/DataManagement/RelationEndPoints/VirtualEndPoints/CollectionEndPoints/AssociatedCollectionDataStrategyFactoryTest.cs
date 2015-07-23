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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class AssociatedCollectionDataStrategyFactoryTest : StandardMappingTest
  {
    private IVirtualEndPointProvider _virtualEndPointProviderStub;
    private AssociatedCollectionDataStrategyFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();

      _virtualEndPointProviderStub = MockRepository.GenerateStub<IVirtualEndPointProvider>();
      _factory = new AssociatedCollectionDataStrategyFactory (_virtualEndPointProviderStub);
    }

    [Test]
    public void CreateDataStrategyForEndPoint ()
    {
      var ordersEndPointID = RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders");

      var result = _factory.CreateDataStrategyForEndPoint (ordersEndPointID);

      Assert.That (result, Is.TypeOf<ModificationCheckingCollectionDataDecorator> ());
      var checkingDecorator = (ModificationCheckingCollectionDataDecorator) result;
      Assert.That (checkingDecorator.RequiredItemType, Is.SameAs (typeof (Order)));

      var delegator = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<EndPointDelegatingCollectionData> (checkingDecorator);
      Assert.That (delegator.AssociatedEndPointID, Is.EqualTo (ordersEndPointID));
      Assert.That (delegator.VirtualEndPointProvider, Is.SameAs (_virtualEndPointProviderStub));
    } 
  }
}