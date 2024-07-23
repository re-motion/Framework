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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.CollectionData
{
  [Ignore("TODO RM-7294: Tests for VirtualCollectionData")]
  [TestFixture]
  public class VirtualCollectionDataTest : StandardMappingTest
  {
    private ProductReview _domainObject;
    private RelationEndPointID _endPointID;

    private VirtualCollectionData _wrappedData;
    private VirtualCollectionData _decoratorWithRealData;

    private Mock<IVirtualCollectionEndPointChangeDetectionStrategy> _strategyStrictMock;
    private Mock<IDataContainerMapReadOnlyView> _dataContainerMapStub;

    public override void SetUp ()
    {
      base.SetUp();

      var parent = DomainObjectMother.CreateFakeObject<Product>();
      _domainObject = DomainObjectMother.CreateFakeObject<ProductReview>();
      _endPointID = RelationEndPointID.Resolve(parent, o => o.Reviews);
      _dataContainerMapStub = new Mock<IDataContainerMapReadOnlyView>();
      _wrappedData = new VirtualCollectionData(_endPointID, _dataContainerMapStub.Object, ValueAccess.Current);
      //_wrappedData.Add (_domainObject);
      _decoratorWithRealData = _wrappedData;

      _strategyStrictMock = new Mock<IVirtualCollectionEndPointChangeDetectionStrategy>(MockBehavior.Strict);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);
    }

    [Test]
    public void OriginalData_IsReadOnly ()
    {
      Assert.That(_decoratorWithRealData.GetOriginalData().IsReadOnly, Is.True);
      Assert.That(_decoratorWithRealData.GetOriginalData(), Is.TypeOf(typeof(ReadOnlyVirtualCollectionDataDecorator)));
    }

    [Test]
    public void OriginalData_PointsToActualData_AfterInitialization ()
    {
      CheckOriginalDataNotCopied(_decoratorWithRealData);
    }

    [Test]
    public void OnWrappedDataChanged ()
    {
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      CallOnDataChangedOnWrappedData(_decoratorWithRealData);

      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Clear_InvalidatesCache ()
    {
      WarmUpCache(_decoratorWithRealData, false);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      ((IVirtualCollectionData)_decoratorWithRealData).Clear();

      Assert.That(_decoratorWithRealData.ToArray(), Is.Empty);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Clear_NoCacheInvalidation_WhenNothingToClear ()
    {
      //_wrappedData.Clear();

      WarmUpCache(_decoratorWithRealData, false);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      ((IVirtualCollectionData)_decoratorWithRealData).Clear();

      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);
    }

    [Test]
    public void Clear_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification((d, obj) => ((IVirtualCollectionData)d).Clear());
    }

    [Test]
    public void Remove_Object_InvalidatesCache_IfTrue ()
    {
      WarmUpCache(_decoratorWithRealData, false);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      ((IVirtualCollectionData)_decoratorWithRealData).Remove(_domainObject);

      Assert.That(_wrappedData.ToArray(), Is.Empty);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.False);
    }

    [Test]
    public void Remove_Object_LeavesCache_IfFalse ()
    {
      //_wrappedData.Clear();

      WarmUpCache(_decoratorWithRealData, false);
      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);

      ((IVirtualCollectionData)_decoratorWithRealData).Remove(_domainObject);

      Assert.That(_decoratorWithRealData.IsCacheUpToDate, Is.True);
    }

    [Test]
    public void Remove_Object_OriginalValuesCopied ()
    {
      CheckOriginalValuesCopiedBeforeModification((d, obj) => ((IVirtualCollectionData)d).Remove(obj));
    }

    [Test]
    public void ResetCachedDomainObjects_RevertsCurrentObjects_ToOriginalObjects ()
    {
      var decorator = new VirtualCollectionData(_endPointID, _dataContainerMapStub.Object, ValueAccess.Current);
      ((IVirtualCollectionData)decorator).Add(_domainObject);

      Assert.That(decorator.ToArray(), Is.Not.Empty);
      Assert.That(decorator.GetOriginalData().ToArray(), Is.Empty);

      decorator.ResetCachedDomainObjects();

      Assert.That(decorator.ToArray(), Is.Empty);
      Assert.That(decorator.GetOriginalData().ToArray(), Is.Empty);
      CheckOriginalDataNotCopied(decorator);
    }

    [Test]
    public void ResetCachedDomainObjects_SetsFlagUnchanged ()
    {
      var decorator = new VirtualCollectionData(_endPointID, _dataContainerMapStub.Object, ValueAccess.Current);
      ((IVirtualCollectionData)decorator).Add(_domainObject);

      decorator.ResetCachedDomainObjects();

      Assert.That(decorator.IsCacheUpToDate, Is.True);
    }

    [Test]
    public void RegisterOriginalItem_ItemAlreadyExists_InOriginal ()
    {
      IVirtualCollectionData underlyingOriginalData = null;
      //var underlyingOriginalData = VirtualCollectionDataTestHelper.GetWrappedDataAndCheckType<CopyOnWriteDomainObjectVirtualCollectionData> (
      //    _decoratorWithRealData.OriginalData);

      var domainObject = DomainObjectMother.CreateFakeObject<ProductReview>();
      underlyingOriginalData.Add(domainObject);

      Assert.That(((IVirtualCollectionData)_decoratorWithRealData).GetObject(domainObject.ID), Is.Null);
      Assert.That(_decoratorWithRealData.GetOriginalData().GetObject(domainObject.ID), Is.Not.Null);
      Assert.That(
          //() => _decoratorWithRealData.RegisterOriginalItem (domainObject),
          () => _decoratorWithRealData.ResetCachedDomainObjects(),
      Throws.InvalidOperationException
              .With.Message.Matches(@"The original collection already contains a domain object with ID 'ProductReview\|.*'\."));
    }

    [Test]
    public void RegisterOriginalItem_CollectionUnchanged_ItemAddedToBothLists ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ProductReview>();
      //_decoratorWithRealData.RegisterOriginalItem (domainObject);
      _decoratorWithRealData.ResetCachedDomainObjects();

      Assert.That(((IVirtualCollectionData)_decoratorWithRealData).GetObject(1), Is.SameAs(domainObject));
      Assert.That(_decoratorWithRealData.GetOriginalData().GetObject(1), Is.SameAs(domainObject));
    }

    [Test]
    public void RegisterOriginalItem_CollectionUnchanged_ChangeFlagRetained ()
    {
      PrepareCheckChangeFlagRetained(_decoratorWithRealData, false);

      var domainObject = DomainObjectMother.CreateFakeObject<ProductReview>();
      //_decoratorWithRealData.RegisterOriginalItem (domainObject);
      _decoratorWithRealData.ResetCachedDomainObjects();

      CheckChangeFlagRetained(_decoratorWithRealData);
    }

    [Test]
    public void RegisterOriginalItem_CollectionUnchanged_OriginalCollectionNotCopied ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ProductReview>();
      //_decoratorWithRealData.RegisterOriginalItem (domainObject);
      _decoratorWithRealData.ResetCachedDomainObjects();

      CheckOriginalDataNotCopied(_decoratorWithRealData);
    }

    [Test]
    public void RegisterOriginalItem_OriginalDataCopiedd_ItemAddedToBothCollections ()
    {
      Assert.That(_decoratorWithRealData.Count, Is.GreaterThan(0));
      ((IVirtualCollectionData)_decoratorWithRealData).Clear();

      var domainObject = DomainObjectMother.CreateFakeObject<ProductReview>();
      //_decoratorWithRealData.RegisterOriginalItem (domainObject);
      _decoratorWithRealData.ResetCachedDomainObjects();

      Assert.That(((IVirtualCollectionData)_decoratorWithRealData).GetObject(0), Is.SameAs(domainObject));
      Assert.That(_decoratorWithRealData.GetOriginalData().GetObject(1), Is.SameAs(domainObject));
    }

    [Test]
    public void RegisterOriginalItem_OriginalDataCopied_ChangeFlagRetained ()
    {
      Assert.That(_decoratorWithRealData.Count, Is.GreaterThan(0));
      ((IVirtualCollectionData)_decoratorWithRealData).Clear();

      PrepareCheckChangeFlagRetained(_decoratorWithRealData, true);

      var domainObject = DomainObjectMother.CreateFakeObject<ProductReview>();
      //_decoratorWithRealData.RegisterOriginalItem (domainObject);
      _decoratorWithRealData.ResetCachedDomainObjects();

      CheckChangeFlagRetained(_decoratorWithRealData);
    }

    [Test]
    public void RegisterOriginalItem_CurrentCollectionAlreadyContainsItem_ItemAddedToOriginalCollection ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ProductReview>();
      ((IVirtualCollectionData)_decoratorWithRealData).Add(domainObject);

      Assert.That(((IVirtualCollectionData)_decoratorWithRealData).GetObject(1), Is.SameAs(domainObject));

      //_decoratorWithRealData.RegisterOriginalItem (domainObject);
      _decoratorWithRealData.ResetCachedDomainObjects();

      Assert.That(((IVirtualCollectionData)_decoratorWithRealData).GetObject(1), Is.SameAs(domainObject));
      Assert.That(_decoratorWithRealData.GetOriginalData().GetObject(1), Is.SameAs(domainObject));
    }

    [Test]
    public void RegisterOriginalItem_CurrentCollectionAlreadyContainsItem_ChangeFlagInvalidated ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ProductReview>();
      ((IVirtualCollectionData)_decoratorWithRealData).Add(domainObject);

      PrepareCheckChangeFlagInvalidated(_decoratorWithRealData, true);

      //_decoratorWithRealData.RegisterOriginalItem (domainObject);
      _decoratorWithRealData.ResetCachedDomainObjects();

      CheckChangeFlagInvalidated(_decoratorWithRealData);
    }

    [Test]
    public void UnregisterOriginalItem_ItemNotExists_InOriginal ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ProductReview>();
      ((IVirtualCollectionData)_decoratorWithRealData).Add(domainObject);

      Assert.That(((IVirtualCollectionData)_decoratorWithRealData).GetObject(domainObject.ID), Is.Not.Null);
      Assert.That(_decoratorWithRealData.GetOriginalData().GetObject(domainObject.ID), Is.Null);
      Assert.That(
          //() => _decoratorWithRealData.UnregisterOriginalItem (domainObject.ID),
          () => _decoratorWithRealData.ResetCachedDomainObjects(),
          Throws.InvalidOperationException
              .With.Message.Matches(@"The original collection does not contain a domain object with ID 'Order\|.*'\."));
    }

    [Test]
    public void UnregisterOriginalItem_CollectionUnchanged_ItemRemovedFromBothLists ()
    {
      Assert.That(((IVirtualCollectionData)_decoratorWithRealData).ContainsObjectID(_domainObject.ID), Is.True);
      Assert.That(_decoratorWithRealData.GetOriginalData().ContainsObjectID(_domainObject.ID), Is.True);

      //_decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);
      _decoratorWithRealData.ResetCachedDomainObjects();

      Assert.That(((IVirtualCollectionData)_decoratorWithRealData).ContainsObjectID(_domainObject.ID), Is.False);
      Assert.That(_decoratorWithRealData.GetOriginalData().ContainsObjectID(_domainObject.ID), Is.False);
    }

    [Test]
    public void UnregisterOriginalItem_CollectionUnchanged_ChangeFlagRetained ()
    {
      PrepareCheckChangeFlagRetained(_decoratorWithRealData, false);

      //_decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);
      _decoratorWithRealData.ResetCachedDomainObjects();

      CheckChangeFlagRetained(_decoratorWithRealData);
    }

    [Test]
    public void UnregisterOriginalItem_CollectionUnchanged_OriginalCollectionNotCopied ()
    {
      //_decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);
      //_decoratorWithRealData.ResetCachedHasChangedState();

      CheckOriginalDataNotCopied(_decoratorWithRealData);
    }

    [Test]
    public void UnregisterOriginalItem_OriginalDataCopied_ItemRemovedFromBothCollections ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ProductReview>();
      ((IVirtualCollectionData)_decoratorWithRealData).Add(domainObject);

      Assert.That(((IVirtualCollectionData)_decoratorWithRealData).ContainsObjectID(_domainObject.ID), Is.True);
      Assert.That(_decoratorWithRealData.GetOriginalData().ContainsObjectID(_domainObject.ID), Is.True);

      //_decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);
      _decoratorWithRealData.ResetCachedDomainObjects();

      Assert.That(((IVirtualCollectionData)_decoratorWithRealData).ContainsObjectID(_domainObject.ID), Is.False);
      Assert.That(_decoratorWithRealData.GetOriginalData().ContainsObjectID(_domainObject.ID), Is.False);
    }

    [Test]
    public void UnregisterOriginalItem_OriginalDataCopied_ChangeFlagInvalidated ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ProductReview>();
      ((IVirtualCollectionData)_decoratorWithRealData).Add(domainObject);

      PrepareCheckChangeFlagInvalidated(_decoratorWithRealData, true);

     // _decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);
      _decoratorWithRealData.ResetCachedDomainObjects();

      CheckChangeFlagInvalidated(_decoratorWithRealData);
    }

    [Test]
    public void UnregisterOriginalItem_CurrentCollectionDoesNotContainItem_ItemRemovedFromOriginalCollection ()
    {
#pragma warning disable 618
      ((IVirtualCollectionData)_decoratorWithRealData).Remove(_domainObject);
#pragma warning restore 618

      Assert.That(((IVirtualCollectionData)_decoratorWithRealData).ContainsObjectID(_domainObject.ID), Is.False);
      Assert.That(_decoratorWithRealData.GetOriginalData().ContainsObjectID(_domainObject.ID), Is.True);

      //_decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);
      _decoratorWithRealData.ResetCachedDomainObjects();

      Assert.That(((IVirtualCollectionData)_decoratorWithRealData).ContainsObjectID(_domainObject.ID), Is.False);
      Assert.That(_decoratorWithRealData.GetOriginalData().ContainsObjectID(_domainObject.ID), Is.False);
    }

    [Test]
    public void UnregisterOriginalItem_CurrentCollectionAlreadyContainsItem_ChangeStateInvalidated ()
    {
#pragma warning disable 618
      ((IVirtualCollectionData)_decoratorWithRealData).Remove(_domainObject);
#pragma warning restore 618

      PrepareCheckChangeFlagInvalidated(_decoratorWithRealData, true);

      //_decoratorWithRealData.UnregisterOriginalItem (_domainObject.ID);
      _decoratorWithRealData.ResetCachedDomainObjects();

      CheckChangeFlagInvalidated(_decoratorWithRealData);
    }

    private void WarmUpCache (VirtualCollectionData decorator, bool hasChanged)
    {
      Assert.That(decorator.IsCacheUpToDate, Is.True);
    }

    private void CallOnDataChangedOnWrappedData (VirtualCollectionData decorator)
    {
      throw new NotImplementedException();
      //var wrappedData = VirtualCollectionDataTestHelper.GetWrappedDataAndCheckType<ObservableVirtualCollectionDataDecorator> (decorator);
      //PrivateInvoke.InvokeNonPublicMethod (
      //    wrappedData,
      //    "OnDataChanged",
      //    ObservableVirtualCollectionDataDecoratorBase.OperationKind.Remove,
      //    _domainObject,
      //    12);
    }

    private void CheckOriginalValuesCopiedBeforeModification (Action<VirtualCollectionData, DomainObject> action)
    {
      var domainObject1 = DomainObjectMother.CreateFakeObject<ProductReview>();
      var domainObject2 = DomainObjectMother.CreateFakeObject<ProductReview>();

      var decorator = new VirtualCollectionData(_endPointID, _dataContainerMapStub.Object, ValueAccess.Current);
      //decorator.Add (domainObject1);
      //decorator.Add (domainObject2);

      action(decorator, domainObject1);

      Assert.That(decorator.GetOriginalData().ToArray(), Is.EqualTo(new[] { domainObject1, domainObject2 }));
    }

    private void CheckOriginalDataMatches (IVirtualCollectionData expected, IVirtualCollectionData actual)
    {
      throw new NotImplementedException();
      //var expectedInner = VirtualCollectionDataTestHelper.GetWrappedDataAndCheckType<CopyOnWriteDomainObjectVirtualCollectionData> (
      //    (ReadOnlyVirtualCollectionData) expected);
      //var actualInner = VirtualCollectionDataTestHelper.GetWrappedDataAndCheckType<CopyOnWriteDomainObjectVirtualCollectionData> (
      //    (ReadOnlyVirtualCollectionData) actual);
      //Assert.That (actualInner, Is.SameAs (expectedInner));
    }

    private void PrepareCheckChangeFlagRetained (VirtualCollectionData decorator, bool hasChanged)
    {
      WarmUpCache(decorator, hasChanged);
      Assert.That(decorator.IsCacheUpToDate, Is.True);
    }

    private void CheckChangeFlagRetained (VirtualCollectionData decorator)
    {
      Assert.That(decorator.IsCacheUpToDate, Is.True);
    }

    private void CheckOriginalDataNotCopied (VirtualCollectionData decorator)
    {
      throw new NotImplementedException();
      //var originalData = VirtualCollectionDataTestHelper.GetWrappedDataAndCheckType<CopyOnWriteDomainObjectVirtualCollectionData> (
      //    decorator.OriginalData);

      //var originalDataStore = VirtualCollectionDataTestHelper.GetWrappedData (originalData);
      //var observedWrappedData = PrivateInvoke.GetNonPublicField (decorator, "_observedWrappedData");
      //Assert.That (originalDataStore, Is.SameAs (observedWrappedData));
    }

    private void PrepareCheckChangeFlagInvalidated (VirtualCollectionData decorator, bool hasChanged)
    {
      WarmUpCache(decorator, hasChanged);
    }

    private void CheckChangeFlagInvalidated (VirtualCollectionData decorator)
    {
      Assert.That(decorator.IsCacheUpToDate, Is.False);
    }

    private static int CompareTypeNames (DomainObject x, DomainObject y)
    {
      // ReSharper disable PossibleNullReferenceException
      return x.GetPublicDomainObjectType().FullName.CompareTo(y.GetPublicDomainObjectType().FullName);
      // ReSharper restore PossibleNullReferenceException
    }
  }
}
