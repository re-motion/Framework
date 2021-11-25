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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting.NUnit;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class OneToManyRelationChangeWithVirtualCollectionTest : RelationChangeBaseTest
  {
    private Person _oldReviewer;
    private Person _newReviewer;
    private ProductReview _productReview1;

    public override void SetUp ()
    {
      base.SetUp();

      _oldReviewer = DomainObjectIDs.Person1.GetObject<Person>();
      _newReviewer = DomainObjectIDs.Person4.GetObject<Person>();
      _productReview1 = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();

      _oldReviewer.EnsureDataAvailable();
      _newReviewer.EnsureDataAvailable();
      _productReview1.EnsureDataAvailable();
      _oldReviewer.Reviews.EnsureDataComplete();
      _newReviewer.Reviews.EnsureDataComplete();
    }

    [Test]
    public void ChangeEvents ()
    {
      var domainObjectEventSources = new DomainObject[]
                                     {
                                         _oldReviewer, _newReviewer, _productReview1
                                     };

      var eventReceiver = new SequenceEventReceiver(domainObjectEventSources, new DomainObjectCollection[0]);

      _productReview1.Reviewer = _newReviewer;

      var expectedChangeStates = new ChangeState[]
                                 {
                                     new RelationChangeState(
                                         _productReview1,
                                         typeof (ProductReview).FullName + ".Reviewer",
                                         _oldReviewer,
                                         _newReviewer,
                                         "1. Changing event of reviews from old to new reviewer"),
                                     new RelationChangeState(
                                         _newReviewer,
                                         typeof (Person).FullName + ".Reviews",
                                         null,
                                         _productReview1,
                                         "2. Changing event of new reviewer"),
                                     new RelationChangeState(
                                         _oldReviewer,
                                         typeof (Person).FullName + ".Reviews",
                                         _productReview1,
                                         null,
                                         "3. Changing event of old reviewer"),
                                     new RelationChangeState(
                                         _oldReviewer,
                                         typeof (Person).FullName + ".Reviews",
                                         null,
                                         null,
                                         "4. Changed event of old reviewer"),
                                     new RelationChangeState(
                                         _newReviewer,
                                         typeof (Person).FullName + ".Reviews",
                                         null,
                                         null,
                                         "5. Changed event of new reviewer"),
                                     new RelationChangeState(
                                         _productReview1,
                                         typeof (ProductReview).FullName + ".Reviewer",
                                         null,
                                         null,
                                         "6. Changed event of reviews from old to new reviewer"),
                                 };

      eventReceiver.Check(expectedChangeStates);

      Assert.That(_productReview1.State.IsChanged, Is.True);
      Assert.That(_oldReviewer.State.IsChanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(_newReviewer.State.IsChanged, Is.True); // TODO: RM-7294: IsRelationChanged

      Assert.That(_productReview1.Reviewer, Is.SameAs(_newReviewer));
      Assert.That(_oldReviewer.Reviews.GetObject(_productReview1.ID), Is.Null);
      Assert.That(_newReviewer.Reviews.GetObject(_productReview1.ID), Is.SameAs(_productReview1));

      Assert.That(_productReview1.InternalDataContainer.State.IsChanged, Is.True);
      Assert.That(_oldReviewer.InternalDataContainer.State.IsUnchanged, Is.True);
      Assert.That(_newReviewer.InternalDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void ChildCancelsChangeEvent ()
    {
      var domainObjectEventSources = new DomainObject[] { _oldReviewer, _newReviewer, _productReview1 };
      var eventReceiver = new SequenceEventReceiver(domainObjectEventSources, new DomainObjectCollection[0], 1);

      Assert.That(() => _productReview1.Reviewer = _newReviewer, Throws.TypeOf<EventReceiverCancelException>());

      var expectedChangeStates = new ChangeState[]
                                 {
                                     new RelationChangeState(
                                         _productReview1,
                                         typeof (ProductReview).FullName + ".Reviewer",
                                         _oldReviewer,
                                         _newReviewer,
                                         "1. Changing event of reviews from old to new reviewer")
                                 };

      eventReceiver.Check(expectedChangeStates);

      Assert.That(_productReview1.State.IsUnchanged, Is.True);
      Assert.That(_oldReviewer.State.IsUnchanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(_newReviewer.State.IsUnchanged, Is.True); // TODO: RM-7294: IsRelationChanged

      Assert.That(_productReview1.Reviewer, Is.SameAs(_oldReviewer));
      Assert.That(_oldReviewer.Reviews.GetObject(_productReview1.ID), Is.SameAs(_productReview1));
      Assert.That(_newReviewer.Reviews.GetObject(_productReview1.ID), Is.Null);
    }

    [Test]
    public void NewParentCancelsChangeEvent ()
    {
      var domainObjectEventSources = new DomainObject[]
                                     {
                                         _oldReviewer, _newReviewer, _productReview1
                                     };

      var eventReceiver = new SequenceEventReceiver(domainObjectEventSources, new DomainObjectCollection[0], 2);

      Assert.That(() => _productReview1.Reviewer = _newReviewer, Throws.TypeOf<EventReceiverCancelException>());

      var expectedChangeStates = new ChangeState[]
                                 {
                                     new RelationChangeState(
                                         _productReview1,
                                         typeof (ProductReview).FullName + ".Reviewer",
                                         _oldReviewer,
                                         _newReviewer,
                                         "1. Changing event of reviews from old to new reviewer"),
                                     new RelationChangeState(
                                         _newReviewer,
                                         typeof (Person).FullName + ".Reviews",
                                         null,
                                         _productReview1,
                                         "2. Changing event of new reviewer")
                                 };

      eventReceiver.Check(expectedChangeStates);

      Assert.That(_productReview1.State.IsUnchanged, Is.True);
      Assert.That(_oldReviewer.State.IsUnchanged, Is.True);
      Assert.That(_newReviewer.State.IsUnchanged, Is.True);

      Assert.That(_productReview1.Reviewer, Is.SameAs(_oldReviewer));
      Assert.That(_oldReviewer.Reviews.GetObject(_productReview1.ID), Is.SameAs(_productReview1));
      Assert.That(_newReviewer.Reviews.GetObject(_productReview1.ID), Is.Null);
    }

    [Test]
    public void OldParentCancelsChangeEvent ()
    {
      var domainObjectEventSources = new DomainObject[]
                                     {
                                         _oldReviewer, _newReviewer, _productReview1
                                     };

      var eventReceiver = new SequenceEventReceiver(domainObjectEventSources, new DomainObjectCollection[0], 3);

      Assert.That(() => _productReview1.Reviewer = _newReviewer, Throws.TypeOf<EventReceiverCancelException>());

      var expectedChangeStates = new ChangeState[]
                                 {
                                     new RelationChangeState(
                                         _productReview1,
                                         typeof (ProductReview).FullName + ".Reviewer",
                                         _oldReviewer,
                                         _newReviewer,
                                         "1. Changing event of reviews from old to new reviewer"),
                                     new RelationChangeState(
                                         _newReviewer,
                                         typeof (Person).FullName + ".Reviews",
                                         null,
                                         _productReview1,
                                         "2. Changing event of new reviewer"),
                                     new RelationChangeState(
                                         _oldReviewer,
                                         typeof (Person).FullName + ".Reviews",
                                         _productReview1,
                                         null,
                                         "3. Changing event of old reviewer")
                                 };

      eventReceiver.Check(expectedChangeStates);

      Assert.That(_productReview1.State.IsUnchanged, Is.True);
      Assert.That(_oldReviewer.State.IsUnchanged, Is.True);
      Assert.That(_newReviewer.State.IsUnchanged, Is.True);

      Assert.That(_productReview1.Reviewer, Is.SameAs(_oldReviewer));
      Assert.That(_oldReviewer.Reviews.GetObject(_productReview1.ID), Is.SameAs(_productReview1));
      Assert.That(_newReviewer.Reviews.GetObject(_productReview1.ID), Is.Null);
    }

    [Test]
    public void StateTracking ()
    {
      _productReview1.Reviewer = _newReviewer;

      Assert.That(_productReview1.State.IsChanged, Is.True);
      Assert.That(_oldReviewer.State.IsChanged, Is.True);
      Assert.That(_newReviewer.State.IsChanged, Is.True);
    }

    [Test]
    public void SetNewParentThroughChild ()
    {
      _productReview1.Reviewer = _newReviewer;

      Assert.That(_productReview1.State.IsChanged, Is.True);
      Assert.That(_oldReviewer.State.IsChanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(_newReviewer.State.IsChanged, Is.True); // TODO: RM-7294: IsRelationChanged

      Assert.That(_productReview1.Reviewer, Is.SameAs(_newReviewer));
      Assert.That(_oldReviewer.Reviews.GetObject(_productReview1.ID), Is.Null);
      Assert.That(_newReviewer.Reviews.GetObject(_productReview1.ID), Is.SameAs(_productReview1));

      Assert.That(_productReview1.InternalDataContainer.State.IsChanged, Is.True);
      Assert.That(_newReviewer.InternalDataContainer.State.IsUnchanged, Is.True);
      Assert.That(_oldReviewer.InternalDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void ChangeRelationBackToOriginalValue ()
    {
      _productReview1.Reviewer = _newReviewer;
      Assert.That(_productReview1.State.IsChanged, Is.True);
      Assert.That(_oldReviewer.State.IsChanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(_newReviewer.State.IsChanged, Is.True); // TODO: RM-7294: IsRelationChanged

      _productReview1.Reviewer = _oldReviewer;
      Assert.That(_productReview1.State.IsUnchanged, Is.True);
      Assert.That(_oldReviewer.State.IsUnchanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(_newReviewer.State.IsUnchanged, Is.True); // TODO: RM-7294: IsRelationChanged
    }

    [Test]
    public void SetOriginalValue ()
    {
      _productReview1.Reviewer = _productReview1.Reviewer;
      Assert.That(_productReview1.State.IsUnchanged, Is.True);
      Assert.That(_productReview1.Reviewer.State.IsUnchanged, Is.True);

      Assert.That(_productReview1.InternalDataContainer.State.IsUnchanged, Is.True);
      Assert.That(_productReview1.Reviewer.InternalDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void HasBeenTouched_FromOneProperty ()
    {
      CheckTouching(
          delegate { _productReview1.Reviewer = _newReviewer; },
          _productReview1,
          "Reviewer",
          RelationEndPointID.Create(_productReview1.ID, typeof (ProductReview).FullName + ".Reviewer"),
          RelationEndPointID.Create(_newReviewer.ID, typeof (Person).FullName + ".Reviews"),
          RelationEndPointID.Create(_oldReviewer.ID, typeof (Person).FullName + ".Reviews"));
    }

    [Test]
    public void HasBeenTouched_FromOneProperty_OriginalValue ()
    {
      CheckTouching(
          delegate { _productReview1.Reviewer = _productReview1.Reviewer; },
          _productReview1,
          "Reviewer",
          RelationEndPointID.Create(_productReview1.ID, typeof (ProductReview).FullName + ".Reviewer"),
          RelationEndPointID.Create(_oldReviewer.ID, typeof (Person).FullName + ".Reviews"));
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      _productReview1.Reviewer = _newReviewer;

      Assert.That(_productReview1.Reviewer, Is.SameAs(_newReviewer));
      Assert.That(_productReview1.GetOriginalRelatedObject(typeof (ProductReview).FullName + ".Reviewer"), Is.SameAs(_oldReviewer));
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      Assert.That(_newReviewer.Reviews.GetObject(_productReview1.ID), Is.Null);

      _productReview1.Reviewer = _newReviewer;

      var oldOrders = _newReviewer.GetOriginalRelatedObjectsAsVirtualCollection(typeof (Person).FullName + ".Reviews");
      Assert.That(_newReviewer.Reviews.GetObject(_productReview1.ID), Is.SameAs(_productReview1));
      Assert.That(oldOrders.Contains(_productReview1), Is.False);
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      var reviewer = DomainObjectIDs.Person1.GetObject<Person>();
      var productReviews = reviewer.GetOriginalRelatedObjectsAsVirtualCollection(typeof (Person).FullName + ".Reviews");

      Assert.That(productReviews.Count, Is.EqualTo(2));
    }

    [Test]
    public void SetRelatedObjectWithInvalidObjectClass ()
    {
      Assert.That(
          () => _productReview1.SetRelatedObject(typeof (ProductReview).FullName + ".Reviewer", DomainObjectIDs.Company1.GetObject<Company>()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "DomainObject 'Company|c4954da8-8870-45c1-b7a3-c7e5e6ad641a|System.Guid' cannot be assigned "
                  + "to property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Reviewer' "
                  + "of DomainObject 'ProductReview|877540a7-fbcf-4bf3-9007-355ea43e796f|System.Guid', because it is not compatible with the type of the property.",
                  "newRelatedObject"));
    }
  }
}