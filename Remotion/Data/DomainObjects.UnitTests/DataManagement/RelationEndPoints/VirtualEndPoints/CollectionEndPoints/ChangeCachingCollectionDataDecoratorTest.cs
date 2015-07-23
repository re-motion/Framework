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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class ChangeCachingCollectionDataDecoratorTest : StandardMappingTest
  {
    private Order _domainObject;

    private IDomainObjectCollectionData _wrappedData;
    private ChangeCachingCollectionDataDecorator _decoratorWithRealData;
    
    private ICollectionEndPointChangeDetectionStrategy _strategyStrictMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainObject = DomainObjectMother.CreateFakeObject<Order> ();

      _wrappedData = new DomainObjectCollectionData (new[] { _domainObject });
      _decoratorWithRealData = new ChangeCachingCollectionDataDecorator (_wrappedData);

      _strategyStrictMock = new MockRepository().StrictMock<ICollectionEndPointChangeDetectionStrategy> ();
    }

    [Test]
    public void Initialization ()
    {
      _strategyStrictMock.Replay();

      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);
      Assert.That (_decoratorWithRealData.HasChanged (_strategyStrictMock), Is.False);
    }

    [Test]
    public void OriginalData_IsReadOnly ()
    {
      Assert.That (_decoratorWithRealData.OriginalData.IsReadOnly, Is.True);
      Assert.That (_decoratorWithRealData.OriginalData, Is.TypeOf (typeof (ReadOnlyCollectionDataDecorator)));
    }

    [Test]
    public void OriginalData_PointsToActualData_AfterInitialization ()
    {
      CheckOriginalDataNotCopied (_decoratorWithRealData);
    }

    [Test]
    public void HasChanged_FastAnswer_WhenContentsNotCopied ()
    {
      _strategyStrictMock.Replay ();

      var result = _decoratorWithRealData.HasChanged (_strategyStrictMock);

      _strategyStrictMock.AssertWasNotCalled (
          mock => mock.HasDataChanged (Arg.Is (_decoratorWithRealData), Arg<IDomainObjectCollectionData>.Is.Anything));
      Assert.That (result, Is.False);
    }

    [Test]
    public void HasChanged_FastAnswer_WhenCountsDiffer ()
    {
      _strategyStrictMock.Replay ();

      _decoratorWithRealData.Add (DomainObjectMother.CreateFakeObject<Order> ()); // make counts differ
      Assert.That (_decoratorWithRealData.Count, Is.Not.EqualTo (_decoratorWithRealData.OriginalData.Count));

      var result = _decoratorWithRealData.HasChanged (_strategyStrictMock);

      _strategyStrictMock.AssertWasNotCalled (
          mock => mock.HasDataChanged (Arg.Is (_decoratorWithRealData), Arg<IDomainObjectCollectionData>.Is.Anything));
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChanged_UsesStrategy ()
    {
      _strategyStrictMock.Expect (mock => mock.HasDataChanged (Arg.Is (_decoratorWithRealData), Arg<IDomainObjectCollectionData>.Is.Anything))
               .Return (true)
               .WhenCalled (mi => CheckOriginalDataMatches (_decoratorWithRealData.OriginalData, (IDomainObjectCollectionData) mi.Arguments[1]))
               .Repeat.Once ();
      _strategyStrictMock.Replay ();

      // Make strategy call necessary because both collections have the same count, but different items.
      _decoratorWithRealData.Replace (0, DomainObjectMother.CreateFakeObject<Order> ());
      Assert.That (_decoratorWithRealData.Count, Is.EqualTo (_decoratorWithRealData.OriginalData.Count));

      var result = _decoratorWithRealData.HasChanged (_strategyStrictMock);

      _strategyStrictMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChanged_CachesData ()
    {
      _strategyStrictMock.Expect (mock => mock.HasDataChanged (Arg.Is (_decoratorWithRealData), Arg<IDomainObjectCollectionData>.Is.Anything))
          .Return (true)
          .Repeat.Once();
      _strategyStrictMock.Replay ();

      // Make strategy call necessary because both collections have the same count, but different items.
      _decoratorWithRealData.Replace (0, DomainObjectMother.CreateFakeObject<Order> ());

      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.False);

      var result1 = _decoratorWithRealData.HasChanged (_strategyStrictMock);
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);

      var result2 = _decoratorWithRealData.HasChanged (_strategyStrictMock);
      var result3 = _decoratorWithRealData.HasChanged (_strategyStrictMock);

      _strategyStrictMock.VerifyAllExpectations ();
      Assert.That (result1, Is.True);
      Assert.That (result2, Is.True);
      Assert.That (result3, Is.True);
    }

    [Test]
    public void OnWrappedDataChanged ()
    {
      _decoratorWithRealData.HasChanged (_strategyStrictMock);
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);

      CallOnDataChangedOnWrappedData (_decoratorWithRealData);

      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void OnWrappedDataChanged_InvalidatedCacheLeadsToReEvaluation ()
    {
      using (_strategyStrictMock.GetMockRepository ().Ordered ())
      {
        _strategyStrictMock.Expect (mock => mock.HasDataChanged (Arg.Is (_decoratorWithRealData), Arg<IDomainObjectCollectionData>.Is.Anything)).Return (true);
        _strategyStrictMock.Expect (mock => mock.HasDataChanged (Arg.Is (_decoratorWithRealData), Arg<IDomainObjectCollectionData>.Is.Anything)).Return (false);
      }
      _strategyStrictMock.Replay ();

      _decoratorWithRealData.Replace (0, DomainObjectMother.CreateFakeObject<Order> ()); // make strategy necessary

      var result1 = _decoratorWithRealData.HasChanged (_strategyStrictMock);
      CallOnDataChangedOnWrappedData (_decoratorWithRealData);

      var result2 = _decoratorWithRealData.HasChanged (_strategyStrictMock);

      _strategyStrictMock.VerifyAllExpectations ();

      Assert.That (result1, Is.True);
      Assert.That (result2, Is.False);
    }

    [Test]
    public void Clear_InvalidatesCache ()
    {
      WarmUpCache (_decoratorWithRealData, false);
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);

      _decoratorWithRealData.Clear ();

      Assert.That (_decoratorWithRealData.ToArray (), Is.Empty);
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Clear_NoCacheInvalidation_WhenNothingToClear ()
    {
      _wrappedData.Clear ();

      WarmUpCache (_decoratorWithRealData, false);
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);

      _decoratorWithRealData.Clear ();

      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);
    }

    [Test]
    public void Clear_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification ((d, obj) => d.Clear());
    }

    [Test]
    public void Insert_InvalidatesCache ()
    {
      WarmUpCache (_decoratorWithRealData, false);
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);

      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      _decoratorWithRealData.Insert (0, domainObject);

      Assert.That (_wrappedData.GetObject (0), Is.SameAs (domainObject));
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Insert_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification ((d, obj) => d.Insert (0, _domainObject));
    }

    [Test]
    public void Remove_Object_InvalidatesCache_IfTrue ()
    {
      WarmUpCache (_decoratorWithRealData, false);
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);

      _decoratorWithRealData.Remove (_domainObject);

      Assert.That (_wrappedData.ToArray (), Is.Empty);
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Remove_Object_LeavesCache_IfFalse ()
    {
      _wrappedData.Clear ();

      WarmUpCache (_decoratorWithRealData, false);
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);

      _decoratorWithRealData.Remove (_domainObject);

      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);
    }

    [Test]
    public void Remove_Object_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification ((d, obj) => d.Remove (obj));
    }

    [Test]
    public void Remove_ID_InvalidatesCache_IfTrue ()
    {
      WarmUpCache (_decoratorWithRealData, false);
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);

      _decoratorWithRealData.Remove (_domainObject.ID);

      Assert.That (_wrappedData.ToArray (), Is.Empty);
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Remove_ID_LeavesCache_IfFalse ()
    {
      _wrappedData.Clear ();

      WarmUpCache (_decoratorWithRealData, false);
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);

      _decoratorWithRealData.Remove (DomainObjectIDs.Order1);

      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);
    }

    [Test]
    public void Remove_ID_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification ((d, obj) => d.Remove (obj.ID));
    }

    [Test]
    public void Replace_InvalidatesCache ()
    {
      WarmUpCache (_decoratorWithRealData, false);
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);

      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _decoratorWithRealData.Replace (0, domainObject);

      Assert.That (_wrappedData.GetObject (0), Is.SameAs (domainObject));
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Replace_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification ((d, obj) => d.Replace (0, _domainObject));
    }

    [Test]
    public void Commit_RevertsOriginalObjects_ToCurrentObjects ()
    {
      var wrappedData = new DomainObjectCollectionData ();
      var decorator = new ChangeCachingCollectionDataDecorator (wrappedData);
      decorator.Add (_domainObject);
      Assert.That (decorator.OriginalData.ToArray (), Is.Empty);

      decorator.Commit ();

      Assert.That (decorator.OriginalData.ToArray (), Is.EqualTo (new[] { _domainObject }));
      CheckOriginalDataNotCopied (decorator);
    }

    [Test]
    public void Commit_SetsFlagUnchanged ()
    {
      var wrappedData = new DomainObjectCollectionData ();
      var decorator = new ChangeCachingCollectionDataDecorator (wrappedData);
      decorator.Add (_domainObject);
      _strategyStrictMock.Replay ();

      decorator.Commit ();

      Assert.That (decorator.HasChanged (_strategyStrictMock), Is.False);
      _strategyStrictMock.AssertWasNotCalled (
          mock => mock.HasDataChanged (Arg<IDomainObjectCollectionData>.Is.Anything, Arg<IDomainObjectCollectionData>.Is.Anything));
    }

    [Test]
    public void Rollback_RevertsCurrentObjects_ToOriginalObjects ()
    {
      var wrappedData = new DomainObjectCollectionData ();
      var decorator = new ChangeCachingCollectionDataDecorator (wrappedData);
      decorator.Add (_domainObject);

      Assert.That (decorator.ToArray (), Is.Not.Empty);
      Assert.That (decorator.OriginalData.ToArray (), Is.Empty);

      decorator.Rollback ();

      Assert.That (decorator.ToArray (), Is.Empty);
      Assert.That (decorator.OriginalData.ToArray (), Is.Empty);
      CheckOriginalDataNotCopied (decorator);
    }

    [Test]
    public void Rollback_SetsFlagUnchanged ()
    {
      var wrappedData = new DomainObjectCollectionData ();
      var decorator = new ChangeCachingCollectionDataDecorator (wrappedData);
      decorator.Add (_domainObject);
      _strategyStrictMock.Replay ();

      decorator.Rollback ();

      Assert.That (decorator.HasChanged (_strategyStrictMock), Is.False);
      _strategyStrictMock.AssertWasNotCalled (
          mock => mock.HasDataChanged (Arg<IDomainObjectCollectionData>.Is.Anything, Arg<IDomainObjectCollectionData>.Is.Anything));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        @"The original collection already contains a domain object with ID 'Order\|.*'\.", MatchType = MessageMatch.Regex)]
    public void RegisterOriginalItem_ItemAlreadyExists_InOriginal ()
    {
      var underlyingOriginalData = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<CopyOnWriteDomainObjectCollectionData> (
          _decoratorWithRealData.OriginalData);

      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      underlyingOriginalData.Add (domainObject);

      Assert.That (_decoratorWithRealData.GetObject (domainObject.ID), Is.Null);
      Assert.That (_decoratorWithRealData.OriginalData.GetObject (domainObject.ID), Is.Not.Null);

      _decoratorWithRealData.RegisterOriginalItem (domainObject);
    }

    [Test]
    public void RegisterOriginalItem_CollectionUnchanged_ItemAddedToBothLists ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _decoratorWithRealData.RegisterOriginalItem (domainObject);

      Assert.That (_decoratorWithRealData.GetObject (1), Is.SameAs (domainObject));
      Assert.That (_decoratorWithRealData.OriginalData.GetObject (1), Is.SameAs (domainObject));
    }

    [Test]
    public void RegisterOriginalItem_CollectionUnchanged_ChangeFlagRetained ()
    {
      PrepareCheckChangeFlagRetained(_decoratorWithRealData, false);

      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _decoratorWithRealData.RegisterOriginalItem (domainObject);

      CheckChangeFlagRetained(_decoratorWithRealData);
    }

    [Test]
    public void RegisterOriginalItem_CollectionUnchanged_OriginalCollectionNotCopied ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _decoratorWithRealData.RegisterOriginalItem (domainObject);

      CheckOriginalDataNotCopied (_decoratorWithRealData);
    }

    [Test]
    public void RegisterOriginalItem_OriginalDataCopiedd_ItemAddedToBothCollections ()
    {
      Assert.That (_decoratorWithRealData.Count, Is.GreaterThan (0));
      _decoratorWithRealData.Clear ();
      
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _decoratorWithRealData.RegisterOriginalItem (domainObject);

      Assert.That (_decoratorWithRealData.GetObject (0), Is.SameAs (domainObject));
      Assert.That (_decoratorWithRealData.OriginalData.GetObject (1), Is.SameAs (domainObject));
    }

    [Test]
    public void RegisterOriginalItem_OriginalDataCopied_ChangeFlagRetained ()
    {
      Assert.That (_decoratorWithRealData.Count, Is.GreaterThan (0));
      _decoratorWithRealData.Clear ();

      PrepareCheckChangeFlagRetained (_decoratorWithRealData, true);

      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _decoratorWithRealData.RegisterOriginalItem (domainObject);

      CheckChangeFlagRetained(_decoratorWithRealData);
    }

    [Test]
    public void RegisterOriginalItem_CurrentCollectionAlreadyContainsItem_ItemAddedToOriginalCollection ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _decoratorWithRealData.Add (domainObject);

      Assert.That (_decoratorWithRealData.GetObject (1), Is.SameAs (domainObject));

      _decoratorWithRealData.RegisterOriginalItem (domainObject);

      Assert.That (_decoratorWithRealData.GetObject (1), Is.SameAs (domainObject));
      Assert.That (_decoratorWithRealData.OriginalData.GetObject (1), Is.SameAs (domainObject));
    }

    [Test]
    public void RegisterOriginalItem_CurrentCollectionAlreadyContainsItem_ChangeFlagInvalidated ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _decoratorWithRealData.Add (domainObject);

      PrepareCheckChangeFlagInvalidated (_decoratorWithRealData, true);

      _decoratorWithRealData.RegisterOriginalItem (domainObject);

      CheckChangeFlagInvalidated (_decoratorWithRealData);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        @"The original collection does not contain a domain object with ID 'Order\|.*'\.", MatchType = MessageMatch.Regex)]
    public void UnregisterOriginalItem_ItemNotExists_InOriginal ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _decoratorWithRealData.Add (domainObject);

      Assert.That (_decoratorWithRealData.GetObject (domainObject.ID), Is.Not.Null);
      Assert.That (_decoratorWithRealData.OriginalData.GetObject (domainObject.ID), Is.Null);

      _decoratorWithRealData.UnregisterOriginalItem (domainObject.ID);
    }

    [Test]
    public void UnregisterOriginalItem_CollectionUnchanged_ItemRemovedFromBothLists ()
    {
      Assert.That (_decoratorWithRealData.ContainsObjectID (_domainObject.ID), Is.True);
      Assert.That (_decoratorWithRealData.OriginalData.ContainsObjectID (_domainObject.ID), Is.True);

      _decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);

      Assert.That (_decoratorWithRealData.ContainsObjectID (_domainObject.ID), Is.False);
      Assert.That (_decoratorWithRealData.OriginalData.ContainsObjectID (_domainObject.ID), Is.False);
    }

    [Test]
    public void UnregisterOriginalItem_CollectionUnchanged_ChangeFlagRetained ()
    {
      PrepareCheckChangeFlagRetained(_decoratorWithRealData, false);

      _decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);

      CheckChangeFlagRetained(_decoratorWithRealData);
    }

    [Test]
    public void UnregisterOriginalItem_CollectionUnchanged_OriginalCollectionNotCopied ()
    {
      _decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);

      CheckOriginalDataNotCopied (_decoratorWithRealData);
    }

    [Test]
    public void UnregisterOriginalItem_OriginalDataCopied_ItemRemovedFromBothCollections ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _decoratorWithRealData.Add (domainObject);

      Assert.That (_decoratorWithRealData.ContainsObjectID (_domainObject.ID), Is.True);
      Assert.That (_decoratorWithRealData.OriginalData.ContainsObjectID (_domainObject.ID), Is.True);

      _decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);

      Assert.That (_decoratorWithRealData.ContainsObjectID (_domainObject.ID), Is.False);
      Assert.That (_decoratorWithRealData.OriginalData.ContainsObjectID (_domainObject.ID), Is.False);
    }

    [Test]
    public void UnregisterOriginalItem_OriginalDataCopied_ChangeFlagInvalidated ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _decoratorWithRealData.Add (domainObject);

      PrepareCheckChangeFlagInvalidated (_decoratorWithRealData, true);

      _decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);

      CheckChangeFlagInvalidated (_decoratorWithRealData);
    }

    [Test]
    public void UnregisterOriginalItem_CurrentCollectionDoesNotContainItem_ItemRemovedFromOriginalCollection ()
    {
      _decoratorWithRealData.Remove (_domainObject.ID);

      Assert.That (_decoratorWithRealData.ContainsObjectID (_domainObject.ID), Is.False);
      Assert.That (_decoratorWithRealData.OriginalData.ContainsObjectID (_domainObject.ID), Is.True);

      _decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);

      Assert.That (_decoratorWithRealData.ContainsObjectID (_domainObject.ID), Is.False);
      Assert.That (_decoratorWithRealData.OriginalData.ContainsObjectID (_domainObject.ID), Is.False);
    }

    [Test]
    public void UnregisterOriginalItem_CurrentCollectionAlreadyContainsItem_ChangeStateInvalidated ()
    {
      _decoratorWithRealData.Remove (_domainObject.ID);

      PrepareCheckChangeFlagInvalidated (_decoratorWithRealData, true);

      _decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);

      CheckChangeFlagInvalidated (_decoratorWithRealData);
    }

    [Test]
    public void Sort_InvalidatesCache ()
    {
      var secondDomainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _wrappedData.Add (secondDomainObject);

      WarmUpCache (_decoratorWithRealData, false);
      Assert.That (_decoratorWithRealData, Is.EqualTo (new[] { _domainObject, secondDomainObject }));
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.True);

      var weights = new Dictionary<DomainObject, int> { { _domainObject, 2 }, { secondDomainObject, 1 } };
      Comparison<DomainObject> comparison = (one, two) => weights[one].CompareTo (weights[two]);
      _decoratorWithRealData.Sort (comparison);

      Assert.That (_decoratorWithRealData, Is.EqualTo (new[] { secondDomainObject, _domainObject }));
      Assert.That (_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Sort_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification ((d, obj) => d.Sort ((one, two) => 0));
    }

    [Test]
    public void SortOriginalAndCurrent_Unchanged ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var domainObject2 = DomainObjectMother.CreateFakeObject<OrderItem> ();
      var domainObject3 = DomainObjectMother.CreateFakeObject<Customer> ();

      var decorator = new ChangeCachingCollectionDataDecorator (
          new DomainObjectCollectionData (new DomainObject[] { domainObject1, domainObject2, domainObject3 }));

      decorator.SortOriginalAndCurrent (CompareTypeNames);

      Assert.That (decorator.ToArray (), Is.EqualTo (new DomainObject[] { domainObject3, domainObject1, domainObject2 }));
      Assert.That (decorator.OriginalData.ToArray (), Is.EqualTo (new DomainObject[] { domainObject3, domainObject1, domainObject2 }));
    }

    [Test]
    public void SortOriginalAndCurrent_Unchanged_OriginalDataIsNotCopied ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var domainObject2 = DomainObjectMother.CreateFakeObject<OrderItem> ();
      var domainObject3 = DomainObjectMother.CreateFakeObject<Customer> ();

      var decorator = new ChangeCachingCollectionDataDecorator (
          new DomainObjectCollectionData (new DomainObject[] { domainObject1, domainObject2, domainObject3 }));

      decorator.SortOriginalAndCurrent (CompareTypeNames);

      CheckOriginalDataNotCopied (decorator);
    }

    [Test]
    public void SortOriginalAndCurrent_Unchanged_ChangeFlagRetained ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var domainObject2 = DomainObjectMother.CreateFakeObject<OrderItem> ();
      var domainObject3 = DomainObjectMother.CreateFakeObject<Customer> ();

      var decorator = new ChangeCachingCollectionDataDecorator (
          new DomainObjectCollectionData (new DomainObject[] { domainObject1, domainObject2, domainObject3 }));

      PrepareCheckChangeFlagRetained (decorator, false);

      decorator.SortOriginalAndCurrent (CompareTypeNames);

      CheckChangeFlagRetained (decorator);
    }

    [Test]
    public void SortOriginalAndCurrent_OriginalDataCopied ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var domainObject2 = DomainObjectMother.CreateFakeObject<OrderItem> ();
      var domainObject3 = DomainObjectMother.CreateFakeObject<Customer> ();

      var decorator = new ChangeCachingCollectionDataDecorator (
          new DomainObjectCollectionData (new DomainObject[] { domainObject1, domainObject2, domainObject3 }));

      decorator.Remove (domainObject2);

      decorator.SortOriginalAndCurrent (CompareTypeNames);

      Assert.That (decorator.ToArray (), Is.EqualTo (new DomainObject[] { domainObject3, domainObject1}));
      Assert.That (decorator.OriginalData.ToArray (), Is.EqualTo (new DomainObject[] { domainObject3, domainObject1, domainObject2 }));
    }

    [Test]
    public void SortOriginalAndCurrent_OriginalDataCopied_ChangeFlagInvalidated ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var domainObject2 = DomainObjectMother.CreateFakeObject<OrderItem> ();
      var domainObject3 = DomainObjectMother.CreateFakeObject<Customer> ();

      var decorator = new ChangeCachingCollectionDataDecorator (
          new DomainObjectCollectionData (new DomainObject[] { domainObject1, domainObject2, domainObject3 }));

      decorator.Remove (domainObject2);

      PrepareCheckChangeFlagInvalidated (decorator, true);

      decorator.SortOriginalAndCurrent (CompareTypeNames);

      CheckChangeFlagInvalidated (decorator);
    }

    [Test]
    public void Serializable ()
    {
      var wrappedData = new DomainObjectCollectionData (new[] { _domainObject });
      var decorator = new ChangeCachingCollectionDataDecorator (wrappedData);

      WarmUpCache (decorator, false);

      Assert.That (decorator.Count, Is.EqualTo (1));
      Assert.That (decorator.IsCacheUpToDate, Is.True);
      Assert.That (decorator.HasChanged (_strategyStrictMock), Is.False);

      var deserializedDecorator = Serializer.SerializeAndDeserialize (decorator);

      Assert.That (deserializedDecorator.Count, Is.EqualTo (1));
      Assert.That (deserializedDecorator.IsCacheUpToDate, Is.True);
      Assert.That (deserializedDecorator.HasChanged (_strategyStrictMock), Is.False);
    }

    private void WarmUpCache (ChangeCachingCollectionDataDecorator decorator, bool hasChanged)
    {
      _strategyStrictMock.Stub (mock => mock.HasDataChanged (Arg.Is (decorator), Arg<IDomainObjectCollectionData>.Is.Anything)).Return (hasChanged);
      _strategyStrictMock.Replay ();

      decorator.HasChanged (_strategyStrictMock);
      Assert.That (decorator.IsCacheUpToDate, Is.True);
    }

    private void CallOnDataChangedOnWrappedData (ChangeCachingCollectionDataDecorator decorator)
    {
      var wrappedData = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<ObservableCollectionDataDecorator> (decorator);
      PrivateInvoke.InvokeNonPublicMethod (wrappedData, "OnDataChanged", ObservableCollectionDataDecoratorBase.OperationKind.Remove, _domainObject, 12);
    }

    private void CheckOriginalValuesCopiedBeforeModification (Action<ChangeCachingCollectionDataDecorator, DomainObject> action)
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();

      var wrappedData = new DomainObjectCollectionData (new[] { domainObject1, domainObject2 });
      var decorator = new ChangeCachingCollectionDataDecorator (wrappedData);

      action (decorator, domainObject1);

      Assert.That (decorator.OriginalData.ToArray (), Is.EqualTo (new[] { domainObject1, domainObject2 }));
    }

    private void CheckOriginalDataMatches (IDomainObjectCollectionData expected, IDomainObjectCollectionData actual)
    {
      var expectedInner = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<CopyOnWriteDomainObjectCollectionData> (
          (ReadOnlyCollectionDataDecorator) expected);
      var actualInner = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<CopyOnWriteDomainObjectCollectionData> (
          (ReadOnlyCollectionDataDecorator) actual);
      Assert.That (actualInner, Is.SameAs (expectedInner));
    }

    private void PrepareCheckChangeFlagRetained (ChangeCachingCollectionDataDecorator decorator, bool hasChanged)
    {
      WarmUpCache (decorator, hasChanged);
      Assert.That (decorator.IsCacheUpToDate, Is.True);
      Assert.That (decorator.HasChanged (_strategyStrictMock), Is.EqualTo (hasChanged));
    }

    private void CheckChangeFlagRetained (ChangeCachingCollectionDataDecorator decorator)
    {
      Assert.That (decorator.IsCacheUpToDate, Is.True);
    }

    private void CheckOriginalDataNotCopied (ChangeCachingCollectionDataDecorator decorator)
    {
      var originalData = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<CopyOnWriteDomainObjectCollectionData> (
          decorator.OriginalData);

      var originalDataStore = DomainObjectCollectionDataTestHelper.GetWrappedData (originalData);
      var observedWrappedData = PrivateInvoke.GetNonPublicField (decorator, "_observedWrappedData");
      Assert.That (originalDataStore, Is.SameAs (observedWrappedData));
    }

    private void PrepareCheckChangeFlagInvalidated (ChangeCachingCollectionDataDecorator decorator, bool hasChanged)
    {
      WarmUpCache (decorator, hasChanged);
      Assert.That (decorator.HasChanged (_strategyStrictMock), Is.EqualTo (hasChanged));
    }

    private void CheckChangeFlagInvalidated (ChangeCachingCollectionDataDecorator decorator)
    {
      Assert.That (decorator.IsCacheUpToDate, Is.False);
    }

    private static int CompareTypeNames (DomainObject x, DomainObject y)
    {
// ReSharper disable PossibleNullReferenceException
      return x.GetPublicDomainObjectType ().FullName.CompareTo (y.GetPublicDomainObjectType ().FullName);
// ReSharper restore PossibleNullReferenceException
    }
  }
}