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
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public static class DomainObjectCollectionDataTestHelper
  {
    public static IDomainObjectCollectionData GetDataStrategy (DomainObjectCollection collection)
    {
      return (IDomainObjectCollectionData) PrivateInvoke.GetNonPublicField (collection, "_dataStrategy");
    }

    public static T GetDataStrategyAndCheckType<T> (DomainObjectCollection collection) where T : IDomainObjectCollectionData
    {
      var data = GetDataStrategy (collection);
      Assert.That (data, Is.InstanceOf (typeof (T)));
      return (T) data;
    }

    public static void SetDataStrategy (DomainObjectCollection collection, IDomainObjectCollectionData dataStrategy)
    {
      PrivateInvoke.SetNonPublicField (collection, "_dataStrategy", dataStrategy);
    }

    public static T GetWrappedDataAndCheckType<T> (DomainObjectCollectionDataDecoratorBase decorator) where T : IDomainObjectCollectionData
    {
      object data = GetWrappedData(decorator);
      Assert.That (data, Is.InstanceOf (typeof (T)));
      return (T) data;
    }

    public static IDomainObjectCollectionData GetWrappedData (DomainObjectCollectionDataDecoratorBase decorator)
    {
      return (IDomainObjectCollectionData) PrivateInvoke.GetNonPublicField (decorator, "_wrappedData");
    }

    public static void CheckAssociatedCollectionStrategy (DomainObjectCollection collection, Type expectedRequiredItemType, RelationEndPointID expectedEndPointID)
    {
      // collection => checking checking decorator => end point data => actual data store
      var checkingDecorator = GetDataStrategy (collection);
      CheckAssociatedCollectionStrategy (checkingDecorator, expectedRequiredItemType, expectedEndPointID);
    }

    public static void CheckAssociatedCollectionStrategy (
        IDomainObjectCollectionData domainObjectCollectionData, 
        Type expectedRequiredItemType, 
        RelationEndPointID expectedEndPointID)
    {
      Assert.That (domainObjectCollectionData, Is.TypeOf<ModificationCheckingCollectionDataDecorator> ());
      var checkingDecorator = (ModificationCheckingCollectionDataDecorator) domainObjectCollectionData;
      Assert.That (checkingDecorator.RequiredItemType, Is.SameAs (expectedRequiredItemType));

      var delegator = GetWrappedDataAndCheckType<EndPointDelegatingCollectionData> (checkingDecorator);
      Assert.That (delegator.AssociatedEndPointID, Is.EqualTo (expectedEndPointID));
    }

    public static void CheckStandAloneCollectionStrategy (DomainObjectCollection collection, Type expectedRequiredItemType, IDomainObjectCollectionData expectedDataStore)
    {
      // collection => checking decorator => event decorator => actual data store

      var checkingDecorator = GetDataStrategyAndCheckType<ModificationCheckingCollectionDataDecorator> (collection);
      Assert.That (checkingDecorator.RequiredItemType, Is.SameAs (expectedRequiredItemType));

      var eventRaisingDecorator = GetWrappedDataAndCheckType<EventRaisingCollectionDataDecorator> (checkingDecorator);
      var eventRaiserAsIndirectRaiser = eventRaisingDecorator.EventRaiser as IndirectDomainObjectCollectionEventRaiser;

      if (eventRaiserAsIndirectRaiser == null)
        Assert.That (eventRaisingDecorator.EventRaiser, Is.SameAs (collection));
      else
        Assert.That (eventRaiserAsIndirectRaiser.EventRaiser, Is.SameAs (collection));

      var dataStore = GetWrappedDataAndCheckType<DomainObjectCollectionData> (eventRaisingDecorator);
      Assert.That (dataStore, Is.SameAs (expectedDataStore));
    }

    public static void CheckStandAloneCollectionStrategy (DomainObjectCollection collection, Type expectedRequiredItemType)
    {
      // collection => checking decorator => event decorator => actual data store

      var checkingDecorator = GetDataStrategyAndCheckType<ModificationCheckingCollectionDataDecorator> (collection);
      Assert.That (checkingDecorator.RequiredItemType, Is.SameAs (expectedRequiredItemType));

      var eventRaisingDecorator = GetWrappedDataAndCheckType<EventRaisingCollectionDataDecorator> (checkingDecorator);
      var eventRaiserAsIndirectRaiser = eventRaisingDecorator.EventRaiser as IndirectDomainObjectCollectionEventRaiser;
      
      if (eventRaiserAsIndirectRaiser == null)
        Assert.That (eventRaisingDecorator.EventRaiser, Is.SameAs (collection));
      else
        Assert.That (eventRaiserAsIndirectRaiser.EventRaiser, Is.SameAs (collection));

      GetWrappedDataAndCheckType<DomainObjectCollectionData> (eventRaisingDecorator);
    }

    public static void CheckReadOnlyCollectionStrategy (DomainObjectCollection collection)
    {
      // collection => read-only decorator => actual data store

      var readOnlyDecorator = GetDataStrategyAndCheckType<ReadOnlyCollectionDataDecorator> (collection);
      GetWrappedDataAndCheckType<DomainObjectCollectionData> (readOnlyDecorator);
    }

    public static void MakeCollectionReadOnly (DomainObjectCollection collection)
    {
      // strip off all decorators
      var checkingDecorator = GetDataStrategyAndCheckType<ModificationCheckingCollectionDataDecorator> (collection);
      var originalStrategy = GetWrappedData (checkingDecorator);
      if (originalStrategy is EventRaisingCollectionDataDecorator)
        originalStrategy = GetWrappedData ((EventRaisingCollectionDataDecorator) originalStrategy);

      var newStrategy = new ReadOnlyCollectionDataDecorator (originalStrategy);
      SetDataStrategy (collection, newStrategy);
    }

    public static ICollectionEndPoint GetAssociatedEndPoint (DomainObjectCollection collection)
    {
      if (collection.AssociatedEndPointID == null)
        return null;

      var checkingDecorator = GetDataStrategyAndCheckType<ModificationCheckingCollectionDataDecorator> (collection);
      var delegatingStrategy = GetWrappedDataAndCheckType<EndPointDelegatingCollectionData> (checkingDecorator);
      return delegatingStrategy.GetAssociatedEndPoint();
    }
  }
}