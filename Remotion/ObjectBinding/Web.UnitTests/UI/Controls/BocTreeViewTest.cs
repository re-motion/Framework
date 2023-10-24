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
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UnitTests.Domain;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocTreeViewTest : BocTest
  {
    private BocTreeView _bocTreeView;
    private TypeWithReference _businessObject;
    private BusinessObjectReferenceDataSource _dataSource;

    public BocTreeViewTest ()
    {
    }


    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocTreeView = new BocTreeView();
      _bocTreeView.ID = "BocTreeView";
      NamingContainer.Controls.Add(_bocTreeView);

      _businessObject = TypeWithReference.Create();

      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject)_businessObject;
    }

    [Test]
    public void SetValueToIReadOnlyList ()
    {
      var listStub = new Mock<IReadOnlyList<IBusinessObjectWithIdentity>>();
      listStub
          .As<IEnumerable>()
          .Setup(_ => _.GetEnumerator())
          .Returns(((IEnumerable)Array.Empty<IBusinessObjectWithIdentity>()).GetEnumerator());
      _bocTreeView.Value = listStub.Object;
      Assert.That(_bocTreeView.Value, Is.SameAs(listStub.Object));
      Assert.That(
          () => _bocTreeView.ValueAsList,
          Throws.InvalidOperationException
              .With.Message.EqualTo("The value only implements the IReadOnlyList<IBusinessObjectWithIdentity> interface. Use the Value property to access the value."));
      Assert.That(((IBusinessObjectBoundControl)_bocTreeView).Value, Is.SameAs(listStub.Object));
    }

    [Test]
    public void SetValueToIReadOnlyListAndIList ()
    {
      var list = new[] { TypeWithReference.Create() };
      _bocTreeView.Value = list;
      Assert.That(_bocTreeView.Value, Is.SameAs(list));
      Assert.That(_bocTreeView.ValueAsList, Is.SameAs(list));
      Assert.That(((IBusinessObjectBoundControl)_bocTreeView).Value, Is.SameAs(list));
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocTreeView.Value = null;
      Assert.That(_bocTreeView.Value, Is.Null);
      Assert.That(_bocTreeView.ValueAsList, Is.Null);
      Assert.That(((IBusinessObjectBoundControl)_bocTreeView).Value, Is.Null);
    }

    [Test]
    public void SetValueAsListToIList ()
    {
      var list = new ArrayList();
      list.Add(TypeWithReference.Create());
      _bocTreeView.ValueAsList = list;
      Assert.That(_bocTreeView.ValueAsList, Is.SameAs(list));
      Assert.That(_bocTreeView.Value, Is.InstanceOf<BusinessObjectListAdapter<IBusinessObjectWithIdentity>>());
      Assert.That(((BusinessObjectListAdapter<IBusinessObjectWithIdentity>)_bocTreeView.Value).WrappedList, Is.SameAs(list));
    }

    [Test]
    public void SetValueAsListToIListAndIReadOnlyList ()
    {
      var list = new List<TypeWithReference>();
      list.Add(TypeWithReference.Create());
      _bocTreeView.ValueAsList = list;
      Assert.That(_bocTreeView.ValueAsList, Is.SameAs(list));
      Assert.That(_bocTreeView.Value, Is.SameAs(list));
      Assert.That(((IBusinessObjectBoundControl)_bocTreeView).Value, Is.SameAs(list));
    }

    [Test]
    public void SetValueAsListToNull ()
    {
      _bocTreeView.ValueAsList = null;
      Assert.That(_bocTreeView.ValueAsList, Is.Null);
      Assert.That(_bocTreeView.Value, Is.Null);
      Assert.That(((IBusinessObjectBoundControl)_bocTreeView).Value, Is.Null);
    }

    [Test]
    public void SetValueFromIBusinessObjectBoundControlToIReadOnlyList ()
    {
      var listStub = new Mock<IReadOnlyList<IBusinessObjectWithIdentity>>();
      listStub
          .As<IEnumerable>()
          .Setup(_ => _.GetEnumerator())
          .Returns(((IEnumerable)Array.Empty<IBusinessObjectWithIdentity>()).GetEnumerator());
      ((IBusinessObjectBoundControl)_bocTreeView).Value = listStub.Object;
      Assert.That(((IBusinessObjectBoundControl)_bocTreeView).Value, Is.SameAs(listStub.Object));
      Assert.That(_bocTreeView.Value, Is.SameAs(listStub.Object));
      Assert.That(
          () => _bocTreeView.ValueAsList,
          Throws.InvalidOperationException
              .With.Message.EqualTo("The value only implements the IReadOnlyList<IBusinessObjectWithIdentity> interface. Use the Value property to access the value."));
    }

    [Test]
    public void SetValueFromIBusinessObjectBoundControlToIReadOnlyListAndIList ()
    {
      var list = new[] { TypeWithReference.Create() };
      ((IBusinessObjectBoundControl)_bocTreeView).Value = list;
      Assert.That(((IBusinessObjectBoundControl)_bocTreeView).Value, Is.SameAs(list));
      Assert.That(_bocTreeView.Value, Is.SameAs(list));
      Assert.That(_bocTreeView.ValueAsList, Is.SameAs(list));
    }

    [Test]
    public void SetFromIBusinessObjectBoundControlToIList ()
    {
      var list = new ArrayList();
      list.Add(TypeWithReference.Create());
      ((IBusinessObjectBoundControl)_bocTreeView).Value = list;
      Assert.That(((IBusinessObjectBoundControl)_bocTreeView).Value, Is.SameAs(list));
      Assert.That(_bocTreeView.ValueAsList, Is.SameAs(list));
      Assert.That(_bocTreeView.Value, Is.InstanceOf<BusinessObjectListAdapter<IBusinessObjectWithIdentity>>());
      Assert.That(((BusinessObjectListAdapter<IBusinessObjectWithIdentity>)_bocTreeView.Value).WrappedList, Is.SameAs(list));
    }

    [Test]
    public void SetFromIBusinessObjectBoundControlToIBusinessObject ()
    {
      var value = TypeWithReference.Create();
      ((IBusinessObjectBoundControl)_bocTreeView).Value = value;
      Assert.That(((IBusinessObjectBoundControl)_bocTreeView).Value, Is.EqualTo(new[] { value }));
      Assert.That(_bocTreeView.ValueAsList, Is.EqualTo(new[] { value }));
      Assert.That(_bocTreeView.Value, Is.EqualTo(new[] { value }));
    }

    [Test]
    public void SetValueFromIBusinessObjectBoundControlToNull ()
    {
      ((IBusinessObjectBoundControl)_bocTreeView).Value = null;
      Assert.That(((IBusinessObjectBoundControl)_bocTreeView).Value, Is.Null);
      Assert.That(_bocTreeView.Value, Is.Null);
      Assert.That(_bocTreeView.ValueAsList, Is.Null);
    }

    [Test]
    public void SetValueFromIBusinessObjectBoundControlToInvalidType ()
    {
      Assert.That(
          () => ((IBusinessObjectBoundControl)_bocTreeView).Value = "fake",
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Parameter type 'System.String' is not supported. Parameters must implement interface IBusinessObjectWithIdentity, IReadOnlyList<IBusinessObjectWithIdentity>, or IList.",
              "value"));
    }

    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocTreeView.Value = new IBusinessObjectWithIdentity[0];
      Assert.That(_bocTreeView.HasValue, Is.True);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocTreeView.Value = null;
      Assert.That(_bocTreeView.HasValue, Is.False);
    }


    [Test]
    public void LoadValueAndInterimTrueWithObject ()
    {
      _bocTreeView.DataSource = _dataSource;
      _bocTreeView.Value = null;

      _bocTreeView.LoadValue(true);
      var actual = _bocTreeView.Value;
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Count, Is.EqualTo(1));
      Assert.That(actual[0], Is.EqualTo(_businessObject));
    }

    [Test]
    public void LoadValueAndInterimFalseWithObject ()
    {
      _bocTreeView.DataSource = _dataSource;
      _bocTreeView.Value = null;

      _bocTreeView.LoadValue(false);
      var actual = _bocTreeView.Value;
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Count, Is.EqualTo(1));
      Assert.That(actual[0], Is.EqualTo(_businessObject));
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      var value = new[] { TypeWithReference.Create() };
      _bocTreeView.DataSource = null;
      _bocTreeView.Value = value;

      _bocTreeView.LoadValue(false);
      Assert.That(_bocTreeView.Value, Is.EqualTo(value));
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocTreeView.DataSource = _dataSource;
      _bocTreeView.Value = new[] { TypeWithReference.Create() };

      _bocTreeView.LoadValue(false);
      Assert.That(_bocTreeView.Value, Is.EqualTo(null));
    }


    [Test]
    public void LoadUnboundValueAndInterimTrueWithList ()
    {
      TypeWithReference[] value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocTreeView.Value = null;

      _bocTreeView.LoadUnboundValue(value, true);
      Assert.That(_bocTreeView.Value, Is.EqualTo(value));
    }

    [Test]
    public void LoadUnboundValueAndInterimTrueWithNull ()
    {
      TypeWithReference[] value = null;
      _bocTreeView.Value = new TypeWithReference[0];

      _bocTreeView.LoadUnboundValue(value, true);
      Assert.That(_bocTreeView.Value, Is.EqualTo(value));
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithList ()
    {
      TypeWithReference[] value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocTreeView.Value = null;

      _bocTreeView.LoadUnboundValue(value, false);
      Assert.That(_bocTreeView.Value, Is.EqualTo(value));
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNull ()
    {
      TypeWithReference[] value = null;
      _bocTreeView.Value = new TypeWithReference[0];

      _bocTreeView.LoadUnboundValue(value, false);
      Assert.That(_bocTreeView.Value, Is.EqualTo(value));
    }

    [Test]
    public void LoadUnboundValueAsListAndInterimTrueWithList ()
    {
      IList value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocTreeView.Value = null;

      _bocTreeView.LoadUnboundValueAsList(value, true);
      Assert.That(_bocTreeView.Value, Is.EqualTo(value));
    }

    [Test]
    public void LoadUnboundValueAsListAndInterimTrueWithNull ()
    {
      IList value = null;
      _bocTreeView.Value = new TypeWithReference[0];

      _bocTreeView.LoadUnboundValueAsList(value, true);
      Assert.That(_bocTreeView.Value, Is.EqualTo(value));
    }

    [Test]
    public void LoadUnboundValueAsListAndInterimFalseWithList ()
    {
      IList value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocTreeView.Value = null;

      _bocTreeView.LoadUnboundValueAsList(value, false);
      Assert.That(_bocTreeView.Value, Is.EqualTo(value));
    }

    [Test]
    public void LoadUnboundValueAsListAndInterimFalseWithNull ()
    {
      IList value = null;
      _bocTreeView.Value = new TypeWithReference[0];

      _bocTreeView.LoadUnboundValueAsList(value, false);
      Assert.That(_bocTreeView.Value, Is.EqualTo(value));
    }
  }
}
