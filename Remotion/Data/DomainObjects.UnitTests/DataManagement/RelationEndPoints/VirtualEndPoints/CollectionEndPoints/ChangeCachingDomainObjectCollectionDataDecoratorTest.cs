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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class ChangeCachingDomainObjectCollectionDataDecoratorTest : StandardMappingTest
  {
    private Order _domainObject;

    private IDomainObjectCollectionData _wrappedData;
    private ChangeCachingDomainObjectCollectionDataDecorator _decoratorWithRealData;

    private Mock<IDomainObjectCollectionEndPointChangeDetectionStrategy> _strategyStrictMock;

    public override void SetUp ()
    {
      base.SetUp();

      _domainObject = DomainObjectMother.CreateFakeObject<Order>();

      _wrappedData = new DomainObjectCollectionData(new[] { _domainObject });
      _decoratorWithRealData = new ChangeCachingDomainObjectCollectionDataDecorator(_wrappedData);

      _strategyStrictMock = new Mock<IDomainObjectCollectionEndPointChangeDetectionStrategy>(MockBehavior.Strict);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);
      Assert.That(_decoratorWithRealData.HasChanged(_strategyStrictMock.Object), Is.False);
    }

    [Test]
    public void OriginalData_IsReadOnly ()
    {
      Assert.That(_decoratorWithRealData.OriginalData.IsReadOnly, Is.True);
      Assert.That(_decoratorWithRealData.OriginalData, Is.TypeOf(typeof(ReadOnlyDomainObjectCollectionDataDecorator)));
    }

    [Test]
    public void OriginalData_PointsToActualData_AfterInitialization ()
    {
      CheckOriginalDataNotCopied(_decoratorWithRealData);
    }

    [Test]
    public void HasChanged_FastAnswer_WhenContentsNotCopied ()
    {
      var result = _decoratorWithRealData.HasChanged(_strategyStrictMock.Object);

      _strategyStrictMock.Verify(mock => mock.HasDataChanged(_decoratorWithRealData, It.IsAny<IDomainObjectCollectionData>()), Times.Never());
      Assert.That(result, Is.False);
    }

    [Test]
    public void HasChanged_FastAnswer_WhenCountsDiffer ()
    {
      _decoratorWithRealData.Add(DomainObjectMother.CreateFakeObject<Order>()); // make counts differ
      Assert.That(_decoratorWithRealData.Count, Is.Not.EqualTo(_decoratorWithRealData.OriginalData.Count));

      var result = _decoratorWithRealData.HasChanged(_strategyStrictMock.Object);

      _strategyStrictMock.Verify(mock => mock.HasDataChanged(_decoratorWithRealData, It.IsAny<IDomainObjectCollectionData>()), Times.Never());
      Assert.That(result, Is.True);
    }

    [Test]
    public void HasChanged_UsesStrategy ()
    {
      _strategyStrictMock
          .Setup(mock => mock.HasDataChanged(_decoratorWithRealData, It.IsAny<IDomainObjectCollectionData>()))
          .Returns(true)
          .Callback((IDomainObjectCollectionData _, IDomainObjectCollectionData originalData) => CheckOriginalDataMatches(_decoratorWithRealData.OriginalData, originalData))
          .Verifiable();

      // Make strategy call necessary because both collections have the same count, but different items.
      _decoratorWithRealData.Replace(0, DomainObjectMother.CreateFakeObject<Order>());
      Assert.That(_decoratorWithRealData.Count, Is.EqualTo(_decoratorWithRealData.OriginalData.Count));

      var result = _decoratorWithRealData.HasChanged(_strategyStrictMock.Object);

      _strategyStrictMock.Verify(mock => mock.HasDataChanged(_decoratorWithRealData, It.IsAny<IDomainObjectCollectionData>()), Times.Once());
      Assert.That(result, Is.True);
    }

    [Test]
    public void HasChanged_CachesData ()
    {
      _strategyStrictMock
          .Setup(mock => mock.HasDataChanged(_decoratorWithRealData, It.IsAny<IDomainObjectCollectionData>()))
          .Returns(true)
          .Verifiable();

      // Make strategy call necessary because both collections have the same count, but different items.
      _decoratorWithRealData.Replace(0, DomainObjectMother.CreateFakeObject<Order>());

      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.False);

      var result1 = _decoratorWithRealData.HasChanged(_strategyStrictMock.Object);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      var result2 = _decoratorWithRealData.HasChanged(_strategyStrictMock.Object);
      var result3 = _decoratorWithRealData.HasChanged(_strategyStrictMock.Object);

      _strategyStrictMock.Verify(mock => mock.HasDataChanged(_decoratorWithRealData, It.IsAny<IDomainObjectCollectionData>()), Times.Once());
      Assert.That(result1, Is.True);
      Assert.That(result2, Is.True);
      Assert.That(result3, Is.True);
    }

    [Test]
    public void OnWrappedDataChanged ()
    {
      _decoratorWithRealData.HasChanged(_strategyStrictMock.Object);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      CallOnDataChangedOnWrappedData(_decoratorWithRealData);

      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void OnWrappedDataChanged_InvalidatedCacheLeadsToReEvaluation ()
    {
      _strategyStrictMock
          .SetupSequence(mock => mock.HasDataChanged(_decoratorWithRealData, It.IsAny<IDomainObjectCollectionData>()))
          .Returns(true)
          .Returns(false);

      _decoratorWithRealData.Replace(0, DomainObjectMother.CreateFakeObject<Order>()); // make strategy necessary

      var result1 = _decoratorWithRealData.HasChanged(_strategyStrictMock.Object);
      CallOnDataChangedOnWrappedData(_decoratorWithRealData);

      var result2 = _decoratorWithRealData.HasChanged(_strategyStrictMock.Object);

      Assert.That(result1, Is.True);
      Assert.That(result2, Is.False);
    }

    [Test]
    public void Clear_InvalidatesCache ()
    {
      WarmUpCache(_decoratorWithRealData, false);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      _decoratorWithRealData.Clear();

      Assert.That(_decoratorWithRealData.ToArray(), Is.Empty);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Clear_NoCacheInvalidation_WhenNothingToClear ()
    {
      _wrappedData.Clear();

      WarmUpCache(_decoratorWithRealData, false);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      _decoratorWithRealData.Clear();

      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);
    }

    [Test]
    public void Clear_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification((d, obj) => d.Clear());
    }

    [Test]
    public void Insert_InvalidatesCache ()
    {
      WarmUpCache(_decoratorWithRealData, false);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      _decoratorWithRealData.Insert(0, domainObject);

      Assert.That(_wrappedData.GetObject(0), Is.SameAs(domainObject));
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Insert_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification((d, obj) => d.Insert(0, _domainObject));
    }

    [Test]
    public void Remove_Object_InvalidatesCache_IfTrue ()
    {
      WarmUpCache(_decoratorWithRealData, false);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      _decoratorWithRealData.Remove(_domainObject);

      Assert.That(_wrappedData.ToArray(), Is.Empty);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Remove_Object_LeavesCache_IfFalse ()
    {
      _wrappedData.Clear();

      WarmUpCache(_decoratorWithRealData, false);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      _decoratorWithRealData.Remove(_domainObject);

      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);
    }

    [Test]
    public void Remove_Object_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification((d, obj) => d.Remove(obj));
    }

    [Test]
    public void Remove_ID_InvalidatesCache_IfTrue ()
    {
      WarmUpCache(_decoratorWithRealData, false);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      _decoratorWithRealData.Remove(_domainObject.ID);

      Assert.That(_wrappedData.ToArray(), Is.Empty);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Remove_ID_LeavesCache_IfFalse ()
    {
      _wrappedData.Clear();

      WarmUpCache(_decoratorWithRealData, false);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      _decoratorWithRealData.Remove(DomainObjectIDs.Order1);

      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);
    }

    [Test]
    public void Remove_ID_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification((d, obj) => d.Remove(obj.ID));
    }

    [Test]
    public void Replace_InvalidatesCache ()
    {
      WarmUpCache(_decoratorWithRealData, false);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      _decoratorWithRealData.Replace(0, domainObject);

      Assert.That(_wrappedData.GetObject(0), Is.SameAs(domainObject));
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Replace_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification((d, obj) => d.Replace(0, _domainObject));
    }

    [Test]
    public void Commit_RevertsOriginalObjects_ToCurrentObjects ()
    {
      var wrappedData = new DomainObjectCollectionData();
      var decorator = new ChangeCachingDomainObjectCollectionDataDecorator(wrappedData);
      decorator.Add(_domainObject);
      Assert.That(decorator.OriginalData.ToArray(), Is.Empty);

      decorator.Commit();

      Assert.That(decorator.OriginalData.ToArray(), Is.EqualTo(new[] { _domainObject }));
      CheckOriginalDataNotCopied(decorator);
    }

    [Test]
    public void Commit_SetsFlagUnchanged ()
    {
      var wrappedData = new DomainObjectCollectionData();
      var decorator = new ChangeCachingDomainObjectCollectionDataDecorator(wrappedData);
      decorator.Add(_domainObject);

      decorator.Commit();

      Assert.That(decorator.HasChanged(_strategyStrictMock.Object), Is.False);
      _strategyStrictMock.Verify(mock => mock.HasDataChanged(It.IsAny<IDomainObjectCollectionData>(), It.IsAny<IDomainObjectCollectionData>()), Times.Never());
    }

    [Test]
    public void Rollback_RevertsCurrentObjects_ToOriginalObjects ()
    {
      var wrappedData = new DomainObjectCollectionData();
      var decorator = new ChangeCachingDomainObjectCollectionDataDecorator(wrappedData);
      decorator.Add(_domainObject);

      Assert.That(decorator.ToArray(), Is.Not.Empty);
      Assert.That(decorator.OriginalData.ToArray(), Is.Empty);

      decorator.Rollback();

      Assert.That(decorator.ToArray(), Is.Empty);
      Assert.That(decorator.OriginalData.ToArray(), Is.Empty);
      CheckOriginalDataNotCopied(decorator);
    }

    [Test]
    public void Rollback_SetsFlagUnchanged ()
    {
      var wrappedData = new DomainObjectCollectionData();
      var decorator = new ChangeCachingDomainObjectCollectionDataDecorator(wrappedData);
      decorator.Add(_domainObject);

      decorator.Rollback();

      Assert.That(decorator.HasChanged(_strategyStrictMock.Object), Is.False);
      _strategyStrictMock.Verify(mock => mock.HasDataChanged(It.IsAny<IDomainObjectCollectionData>(), It.IsAny<IDomainObjectCollectionData>()), Times.Never());
    }

    [Test]
    public void RegisterOriginalItem_ItemAlreadyExists_InOriginal ()
    {
      var underlyingOriginalData = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<CopyOnWriteDomainObjectDomainObjectCollectionData>(
          _decoratorWithRealData.OriginalData);

      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      underlyingOriginalData.Add(domainObject);

      Assert.That(_decoratorWithRealData.GetObject(domainObject.ID), Is.Null);
      Assert.That(_decoratorWithRealData.OriginalData.GetObject(domainObject.ID), Is.Not.Null);
      Assert.That(
          () => _decoratorWithRealData.RegisterOriginalItem(domainObject),
          Throws.InvalidOperationException
              .With.Message.Matches(@"The original collection already contains a domain object with ID 'Order\|.*'\."));
    }

    [Test]
    public void RegisterOriginalItem_CollectionUnchanged_ItemAddedToBothLists ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      _decoratorWithRealData.RegisterOriginalItem(domainObject);

      Assert.That(_decoratorWithRealData.GetObject(1), Is.SameAs(domainObject));
      Assert.That(_decoratorWithRealData.OriginalData.GetObject(1), Is.SameAs(domainObject));
    }

    [Test]
    public void RegisterOriginalItem_CollectionUnchanged_ChangeFlagRetained ()
    {
      PrepareCheckChangeFlagRetained(_decoratorWithRealData, false);

      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      _decoratorWithRealData.RegisterOriginalItem(domainObject);

      CheckChangeFlagRetained(_decoratorWithRealData);
    }

    [Test]
    public void RegisterOriginalItem_CollectionUnchanged_OriginalCollectionNotCopied ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      _decoratorWithRealData.RegisterOriginalItem(domainObject);

      CheckOriginalDataNotCopied(_decoratorWithRealData);
    }

    [Test]
    public void RegisterOriginalItem_OriginalDataCopiedd_ItemAddedToBothCollections ()
    {
      Assert.That(_decoratorWithRealData.Count, Is.GreaterThan(0));
      _decoratorWithRealData.Clear();

      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      _decoratorWithRealData.RegisterOriginalItem(domainObject);

      Assert.That(_decoratorWithRealData.GetObject(0), Is.SameAs(domainObject));
      Assert.That(_decoratorWithRealData.OriginalData.GetObject(1), Is.SameAs(domainObject));
    }

    [Test]
    public void RegisterOriginalItem_OriginalDataCopied_ChangeFlagRetained ()
    {
      Assert.That(_decoratorWithRealData.Count, Is.GreaterThan(0));
      _decoratorWithRealData.Clear();

      PrepareCheckChangeFlagRetained(_decoratorWithRealData, true);

      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      _decoratorWithRealData.RegisterOriginalItem(domainObject);

      CheckChangeFlagRetained(_decoratorWithRealData);
    }

    [Test]
    public void RegisterOriginalItem_CurrentCollectionAlreadyContainsItem_ItemAddedToOriginalCollection ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      _decoratorWithRealData.Add(domainObject);

      Assert.That(_decoratorWithRealData.GetObject(1), Is.SameAs(domainObject));

      _decoratorWithRealData.RegisterOriginalItem(domainObject);

      Assert.That(_decoratorWithRealData.GetObject(1), Is.SameAs(domainObject));
      Assert.That(_decoratorWithRealData.OriginalData.GetObject(1), Is.SameAs(domainObject));
    }

    [Test]
    public void RegisterOriginalItem_CurrentCollectionAlreadyContainsItem_ChangeFlagInvalidated ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      _decoratorWithRealData.Add(domainObject);

      PrepareCheckChangeFlagInvalidated(_decoratorWithRealData, true);

      _decoratorWithRealData.RegisterOriginalItem(domainObject);

      CheckChangeFlagInvalidated(_decoratorWithRealData);
    }

    [Test]
    public void UnregisterOriginalItem_ItemNotExists_InOriginal ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      _decoratorWithRealData.Add(domainObject);

      Assert.That(_decoratorWithRealData.GetObject(domainObject.ID), Is.Not.Null);
      Assert.That(_decoratorWithRealData.OriginalData.GetObject(domainObject.ID), Is.Null);
      Assert.That(
          () => _decoratorWithRealData.UnregisterOriginalItem(domainObject.ID),
          Throws.InvalidOperationException
              .With.Message.Matches(@"The original collection does not contain a domain object with ID 'Order\|.*'\."));
    }

    [Test]
    public void UnregisterOriginalItem_CollectionUnchanged_ItemRemovedFromBothLists ()
    {
      Assert.That(_decoratorWithRealData.ContainsObjectID(_domainObject.ID), Is.True);
      Assert.That(_decoratorWithRealData.OriginalData.ContainsObjectID(_domainObject.ID), Is.True);

      _decoratorWithRealData.UnregisterOriginalItem(_domainObject.ID);

      Assert.That(_decoratorWithRealData.ContainsObjectID(_domainObject.ID), Is.False);
      Assert.That(_decoratorWithRealData.OriginalData.ContainsObjectID(_domainObject.ID), Is.False);
    }

    [Test]
    public void UnregisterOriginalItem_CollectionUnchanged_ChangeFlagRetained ()
    {
      PrepareCheckChangeFlagRetained(_decoratorWithRealData, false);

      _decoratorWithRealData.UnregisterOriginalItem(_domainObject.ID);

      CheckChangeFlagRetained(_decoratorWithRealData);
    }

    [Test]
    public void UnregisterOriginalItem_CollectionUnchanged_OriginalCollectionNotCopied ()
    {
      _decoratorWithRealData.UnregisterOriginalItem(_domainObject.ID);

      CheckOriginalDataNotCopied(_decoratorWithRealData);
    }

    [Test]
    public void UnregisterOriginalItem_OriginalDataCopied_ItemRemovedFromBothCollections ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      _decoratorWithRealData.Add(domainObject);

      Assert.That(_decoratorWithRealData.ContainsObjectID(_domainObject.ID), Is.True);
      Assert.That(_decoratorWithRealData.OriginalData.ContainsObjectID(_domainObject.ID), Is.True);

      _decoratorWithRealData.UnregisterOriginalItem(_domainObject.ID);

      Assert.That(_decoratorWithRealData.ContainsObjectID(_domainObject.ID), Is.False);
      Assert.That(_decoratorWithRealData.OriginalData.ContainsObjectID(_domainObject.ID), Is.False);
    }

    [Test]
    public void UnregisterOriginalItem_OriginalDataCopied_ChangeFlagInvalidated ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      _decoratorWithRealData.Add(domainObject);

      PrepareCheckChangeFlagInvalidated(_decoratorWithRealData, true);

      _decoratorWithRealData.UnregisterOriginalItem(_domainObject.ID);

      CheckChangeFlagInvalidated(_decoratorWithRealData);
    }

    [Test]
    public void UnregisterOriginalItem_CurrentCollectionDoesNotContainItem_ItemRemovedFromOriginalCollection ()
    {
      _decoratorWithRealData.Remove(_domainObject.ID);

      Assert.That(_decoratorWithRealData.ContainsObjectID(_domainObject.ID), Is.False);
      Assert.That(_decoratorWithRealData.OriginalData.ContainsObjectID(_domainObject.ID), Is.True);

      _decoratorWithRealData.UnregisterOriginalItem(_domainObject.ID);

      Assert.That(_decoratorWithRealData.ContainsObjectID(_domainObject.ID), Is.False);
      Assert.That(_decoratorWithRealData.OriginalData.ContainsObjectID(_domainObject.ID), Is.False);
    }

    [Test]
    public void UnregisterOriginalItem_CurrentCollectionAlreadyContainsItem_ChangeStateInvalidated ()
    {
      _decoratorWithRealData.Remove(_domainObject.ID);

      PrepareCheckChangeFlagInvalidated(_decoratorWithRealData, true);

      _decoratorWithRealData.UnregisterOriginalItem(_domainObject.ID);

      CheckChangeFlagInvalidated(_decoratorWithRealData);
    }

    [Test]
    public void Sort_InvalidatesCache ()
    {
      var secondDomainObject = DomainObjectMother.CreateFakeObject<Order>();
      _wrappedData.Add(secondDomainObject);

      WarmUpCache(_decoratorWithRealData, false);
      Assert.That(_decoratorWithRealData, Is.EqualTo(new[] { _domainObject, secondDomainObject }));
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      var weights = new Dictionary<IDomainObject, int> { { _domainObject, 2 }, { secondDomainObject, 1 } };
      Comparison<IDomainObject> comparison = (one, two) => weights[one].CompareTo(weights[two]);
      _decoratorWithRealData.Sort(comparison);

      Assert.That(_decoratorWithRealData, Is.EqualTo(new[] { secondDomainObject, _domainObject }));
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Sort_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification((d, obj) => d.Sort((one, two) => 0));
    }

    [Test]
    public void SortOriginalAndCurrent_Unchanged ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<OrderItem>();
      var domainObject3 = DomainObjectMother.CreateFakeObject<Customer>();

      var decorator = new ChangeCachingDomainObjectCollectionDataDecorator(
          new DomainObjectCollectionData(new DomainObject[] { domainObject1, domainObject2, domainObject3 }));

      decorator.SortOriginalAndCurrent(CompareTypeNames);

      Assert.That(decorator.ToArray(), Is.EqualTo(new DomainObject[] { domainObject3, domainObject1, domainObject2 }));
      Assert.That(decorator.OriginalData.ToArray(), Is.EqualTo(new DomainObject[] { domainObject3, domainObject1, domainObject2 }));
    }

    [Test]
    public void SortOriginalAndCurrent_Unchanged_OriginalDataIsNotCopied ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<OrderItem>();
      var domainObject3 = DomainObjectMother.CreateFakeObject<Customer>();

      var decorator = new ChangeCachingDomainObjectCollectionDataDecorator(
          new DomainObjectCollectionData(new DomainObject[] { domainObject1, domainObject2, domainObject3 }));

      decorator.SortOriginalAndCurrent(CompareTypeNames);

      CheckOriginalDataNotCopied(decorator);
    }

    [Test]
    public void SortOriginalAndCurrent_Unchanged_ChangeFlagRetained ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<OrderItem>();
      var domainObject3 = DomainObjectMother.CreateFakeObject<Customer>();

      var decorator = new ChangeCachingDomainObjectCollectionDataDecorator(
          new DomainObjectCollectionData(new DomainObject[] { domainObject1, domainObject2, domainObject3 }));

      PrepareCheckChangeFlagRetained(decorator, false);

      decorator.SortOriginalAndCurrent(CompareTypeNames);

      CheckChangeFlagRetained(decorator);
    }

    [Test]
    public void SortOriginalAndCurrent_OriginalDataCopied ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<OrderItem>();
      var domainObject3 = DomainObjectMother.CreateFakeObject<Customer>();

      var decorator = new ChangeCachingDomainObjectCollectionDataDecorator(
          new DomainObjectCollectionData(new DomainObject[] { domainObject1, domainObject2, domainObject3 }));

      decorator.Remove(domainObject2);

      decorator.SortOriginalAndCurrent(CompareTypeNames);

      Assert.That(decorator.ToArray(), Is.EqualTo(new DomainObject[] { domainObject3, domainObject1}));
      Assert.That(decorator.OriginalData.ToArray(), Is.EqualTo(new DomainObject[] { domainObject3, domainObject1, domainObject2 }));
    }

    [Test]
    public void SortOriginalAndCurrent_OriginalDataCopied_ChangeFlagInvalidated ()
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<OrderItem>();
      var domainObject3 = DomainObjectMother.CreateFakeObject<Customer>();

      var decorator = new ChangeCachingDomainObjectCollectionDataDecorator(
          new DomainObjectCollectionData(new DomainObject[] { domainObject1, domainObject2, domainObject3 }));

      decorator.Remove(domainObject2);

      PrepareCheckChangeFlagInvalidated(decorator, true);

      decorator.SortOriginalAndCurrent(CompareTypeNames);

      CheckChangeFlagInvalidated(decorator);
    }

    [Test]
    public void Serializable ()
    {
      Assert2.IgnoreIfFeatureSerializationIsDisabled();

      var wrappedData = new DomainObjectCollectionData(new[] { _domainObject });
      var decorator = new ChangeCachingDomainObjectCollectionDataDecorator(wrappedData);

      WarmUpCache(decorator, false);

      Assert.That(decorator.Count, Is.EqualTo(1));
      Assert.That(decorator.IsCacheUpToDate, Is.True);
      Assert.That(decorator.HasChanged(_strategyStrictMock.Object), Is.False);

      var deserializedDecorator = Serializer.SerializeAndDeserialize(decorator);

      Assert.That(deserializedDecorator.Count, Is.EqualTo(1));
      Assert.That(deserializedDecorator.IsCacheUpToDate, Is.True);
      Assert.That(deserializedDecorator.HasChanged(_strategyStrictMock.Object), Is.False);
    }

    private void WarmUpCache (ChangeCachingDomainObjectCollectionDataDecorator decorator, bool hasChanged)
    {
      _strategyStrictMock.Setup(mock => mock.HasDataChanged(decorator, It.IsAny<IDomainObjectCollectionData>())).Returns(hasChanged);

      decorator.HasChanged(_strategyStrictMock.Object);
      Assert.That(decorator.IsCacheUpToDate, Is.True);
    }

    private void CallOnDataChangedOnWrappedData (ChangeCachingDomainObjectCollectionDataDecorator decorator)
    {
      var wrappedData = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<ObservableDomainObjectCollectionDataDecorator>(decorator);
      PrivateInvoke.InvokeNonPublicMethod(wrappedData, "OnDataChanged", ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Remove, _domainObject, 12);
    }

    private void CheckOriginalValuesCopiedBeforeModification (Action<ChangeCachingDomainObjectCollectionDataDecorator, DomainObject> action)
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<Order>();

      var wrappedData = new DomainObjectCollectionData(new[] { domainObject1, domainObject2 });
      var decorator = new ChangeCachingDomainObjectCollectionDataDecorator(wrappedData);

      action(decorator, domainObject1);

      Assert.That(decorator.OriginalData.ToArray(), Is.EqualTo(new[] { domainObject1, domainObject2 }));
    }

    private void CheckOriginalDataMatches (IDomainObjectCollectionData expected, IDomainObjectCollectionData actual)
    {
      var expectedInner = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<CopyOnWriteDomainObjectDomainObjectCollectionData>(
          (ReadOnlyDomainObjectCollectionDataDecorator)expected);
      var actualInner = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<CopyOnWriteDomainObjectDomainObjectCollectionData>(
          (ReadOnlyDomainObjectCollectionDataDecorator)actual);
      Assert.That(actualInner, Is.SameAs(expectedInner));
    }

    private void PrepareCheckChangeFlagRetained (ChangeCachingDomainObjectCollectionDataDecorator decorator, bool hasChanged)
    {
      WarmUpCache(decorator, hasChanged);
      Assert.That(decorator.IsCacheUpToDate, Is.True);
      Assert.That(decorator.HasChanged(_strategyStrictMock.Object), Is.EqualTo(hasChanged));
    }

    private void CheckChangeFlagRetained (ChangeCachingDomainObjectCollectionDataDecorator decorator)
    {
      Assert.That(decorator.IsCacheUpToDate, Is.True);
    }

    private void CheckOriginalDataNotCopied (ChangeCachingDomainObjectCollectionDataDecorator decorator)
    {
      var originalData = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<CopyOnWriteDomainObjectDomainObjectCollectionData>(
          decorator.OriginalData);

      var originalDataStore = DomainObjectCollectionDataTestHelper.GetWrappedData(originalData);
      var observedWrappedData = PrivateInvoke.GetNonPublicField(decorator, "_observedWrappedData");
      Assert.That(originalDataStore, Is.SameAs(observedWrappedData));
    }

    private void PrepareCheckChangeFlagInvalidated (ChangeCachingDomainObjectCollectionDataDecorator decorator, bool hasChanged)
    {
      WarmUpCache(decorator, hasChanged);
      Assert.That(decorator.HasChanged(_strategyStrictMock.Object), Is.EqualTo(hasChanged));
    }

    private void CheckChangeFlagInvalidated (ChangeCachingDomainObjectCollectionDataDecorator decorator)
    {
      Assert.That(decorator.IsCacheUpToDate, Is.False);
    }

    private static int CompareTypeNames (IDomainObject x, IDomainObject y)
    {
// ReSharper disable PossibleNullReferenceException
      return x.GetPublicDomainObjectType().FullName.CompareTo(y.GetPublicDomainObjectType().FullName);
// ReSharper restore PossibleNullReferenceException
    }
  }
}
