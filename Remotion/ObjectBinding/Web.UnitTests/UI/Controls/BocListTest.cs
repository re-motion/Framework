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
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocListTest : BocTest
  {
    private BocListMock _bocList;
    private TypeWithReference _businessObject;
    private IBusinessObjectDataSource _dataSource;
    private IBusinessObjectReferenceProperty _propertyReferenceList;
    private IBusinessObjectReferenceProperty _propertyReferenceListAsList;
    private IBusinessObjectReferenceProperty _propertyReferenceValue;

    public BocListTest ()
    {
    }

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      Invoker.InitRecursive();

      _bocList = new BocListMock();
      _bocList.ID = "BocList";
      NamingContainer.Controls.Add(_bocList);

      _businessObject = TypeWithReference.Create();

      _propertyReferenceList =
          (IBusinessObjectReferenceProperty)((IBusinessObject)_businessObject).BusinessObjectClass.GetPropertyDefinition("ReferenceList");

      _propertyReferenceListAsList =
          (IBusinessObjectReferenceProperty)((IBusinessObject)_businessObject).BusinessObjectClass.GetPropertyDefinition("ReferenceListAsList");

      _dataSource = new StubDataSource(((IBusinessObject)_businessObject).BusinessObjectClass) { Mode = DataSourceMode.Edit };
      _dataSource.BusinessObject = (IBusinessObject)_businessObject;
    }


    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _bocList.ReadOnly = true;
      string[] actual = _bocList.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(0));
    }

    [Test]
    public void GetTrackedClientIDsInEditModeWithoutRowEditModeActive ()
    {
      _bocList.ReadOnly = false;
      Assert.That(_bocList.IsRowEditModeActive, Is.False);
      string[] actual = _bocList.GetTrackedClientIDs();
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(0));
    }

    [Test]
    public void SetValueToIReadOnlyList ()
    {
      var listStub = new Mock<IReadOnlyList<IBusinessObject>>();
      listStub
          .As<IEnumerable<IBusinessObject>>()
          .Setup(_ => _.GetEnumerator())
          .Returns(((IEnumerable<IBusinessObject>)Array.Empty<IBusinessObject>()).GetEnumerator());
      _bocList.Value = listStub.Object;
      Assert.That(_bocList.Value, Is.SameAs(listStub.Object));
      Assert.That(
          () => _bocList.ValueAsList,
          Throws.InvalidOperationException
              .With.Message.EqualTo("The value only implements the IReadOnlyList<IBusinessObject> interface. Use the Value property to access the value."));
      Assert.That(((IBusinessObjectBoundControl)_bocList).Value, Is.SameAs(listStub.Object));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToIReadOnlyListAndIList ()
    {
      var list = new[] { TypeWithReference.Create() };
      _bocList.Value = list;
      Assert.That(_bocList.Value, Is.SameAs(list));
      Assert.That(_bocList.ValueAsList, Is.SameAs(list));
      Assert.That(((IBusinessObjectBoundControl)_bocList).Value, Is.SameAs(list));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void SetValueToNull ()
    {
      _bocList.Value = null;
      Assert.That(_bocList.Value, Is.Null);
      Assert.That(_bocList.ValueAsList, Is.Null);
      Assert.That(((IBusinessObjectBoundControl)_bocList).Value, Is.Null);
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void SetValueAsListToIList ()
    {
      var list = new ArrayList();
      list.Add(TypeWithReference.Create());
      _bocList.ValueAsList = list;
      Assert.That(_bocList.ValueAsList, Is.SameAs(list));
      Assert.That(_bocList.Value, Is.InstanceOf<BusinessObjectListAdapter<IBusinessObject>>());
      Assert.That(((BusinessObjectListAdapter<IBusinessObject>)_bocList.Value).WrappedList, Is.SameAs(list));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void SetValueAsListToIListAndIReadOnlyList ()
    {
      var list = new List<TypeWithReference>();
      list.Add(TypeWithReference.Create());
      _bocList.ValueAsList = list;
      Assert.That(_bocList.ValueAsList, Is.SameAs(list));
      Assert.That(_bocList.Value, Is.SameAs(list));
      Assert.That(((IBusinessObjectBoundControl)_bocList).Value, Is.SameAs(list));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void SetValueAsListToNull ()
    {
      _bocList.ValueAsList = null;
      Assert.That(_bocList.ValueAsList, Is.Null);
      Assert.That(_bocList.Value, Is.Null);
      Assert.That(((IBusinessObjectBoundControl)_bocList).Value, Is.Null);
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void SetValueFromIBusinessObjectBoundControlToIReadOnlyList ()
    {
      var listStub = new Mock<IReadOnlyList<IBusinessObject>>();
      listStub
          .As<IEnumerable<IBusinessObject>>()
          .Setup(_ => _.GetEnumerator())
          .Returns(((IEnumerable<IBusinessObject>)Array.Empty<IBusinessObject>()).GetEnumerator());
      ((IBusinessObjectBoundControl)_bocList).Value = listStub.Object;
      Assert.That(((IBusinessObjectBoundControl)_bocList).Value, Is.SameAs(listStub.Object));
      Assert.That(_bocList.Value, Is.SameAs(listStub.Object));
      Assert.That(
          () => _bocList.ValueAsList,
          Throws.InvalidOperationException
              .With.Message.EqualTo("The value only implements the IReadOnlyList<IBusinessObject> interface. Use the Value property to access the value."));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void SetValueFromIBusinessObjectBoundControlToIReadOnlyListAndIList ()
    {
      var list = new[] { TypeWithReference.Create() };
      ((IBusinessObjectBoundControl)_bocList).Value = list;
      Assert.That(((IBusinessObjectBoundControl)_bocList).Value, Is.SameAs(list));
      Assert.That(_bocList.Value, Is.SameAs(list));
      Assert.That(_bocList.ValueAsList, Is.SameAs(list));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void SetFromIBusinessObjectBoundControlToIList ()
    {
      var list = new ArrayList();
      list.Add(TypeWithReference.Create());
      ((IBusinessObjectBoundControl)_bocList).Value = list;
      Assert.That(((IBusinessObjectBoundControl)_bocList).Value, Is.SameAs(list));
      Assert.That(_bocList.ValueAsList, Is.SameAs(list));
      Assert.That(_bocList.Value, Is.InstanceOf<BusinessObjectListAdapter<IBusinessObject>>());
      Assert.That(((BusinessObjectListAdapter<IBusinessObject>)_bocList.Value).WrappedList, Is.SameAs(list));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void SetValueFromIBusinessObjectBoundControlToNull ()
    {
      ((IBusinessObjectBoundControl)_bocList).Value = null;
      Assert.That(((IBusinessObjectBoundControl)_bocList).Value, Is.Null);
      Assert.That(_bocList.Value, Is.Null);
      Assert.That(_bocList.ValueAsList, Is.Null);
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void SetValueFromIBusinessObjectBoundControlToInvalidType ()
    {
      Assert.That(
          () => ((IBusinessObjectBoundControl)_bocList).Value = "fake",
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Parameter type 'System.String' is not supported. Parameters must implement interface IReadOnlyList<IBusinessObject> or IList.",
              "value"));
    }

    [Test]
    public void HasValue_ValueIsSet_ReturnsTrue ()
    {
      _bocList.Value = new IBusinessObject[1];
      Assert.That(_bocList.HasValue, Is.True);
    }

    [Test]
    public void HasValue_ValueIsEmpty_ReturnsFalse ()
    {
      _bocList.Value = new IBusinessObject[0];
      Assert.That(_bocList.HasValue, Is.False);
    }

    [Test]
    public void HasValue_ValueIsNull_ReturnsFalse ()
    {
      _bocList.Value = null;
      Assert.That(_bocList.HasValue, Is.False);
    }


    [Test]
    public void LoadValueAndInterimTrueWithListAndDirty ()
    {
      _businessObject.ReferenceList = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = null;
      _bocList.IsDirty = true;

      _bocList.LoadValue(true);
      Assert.That(_bocList.Value, Is.SameAs(_businessObject.ReferenceList));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimTrueWithNullAndDirty ()
    {
      _businessObject.ReferenceList = null;
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = true;

      _bocList.LoadValue(true);
      Assert.That(_bocList.Value, Is.SameAs(_businessObject.ReferenceList));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimTrueWithIReadOnlyListAndNotDirty ()
    {
      _businessObject.ReferenceList = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = null;
      _bocList.IsDirty = false;

      _bocList.LoadValue(true);
      Assert.That(_bocList.Value, Is.SameAs(_businessObject.ReferenceList));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimTrueWithIListAndNotDirty ()
    {
      _businessObject.ReferenceListAsList = new ArrayList(new[] { TypeWithReference.Create(), TypeWithReference.Create() });
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceListAsList;
      _bocList.Value = null;
      _bocList.IsDirty = false;

      _bocList.LoadValue(true);
      Assert.That(_bocList.Value, Is.EqualTo(_businessObject.ReferenceListAsList));
      Assert.That(_bocList.ValueAsList, Is.SameAs(_businessObject.ReferenceListAsList));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimTrueWithNullAndNotDirty ()
    {
      _businessObject.ReferenceList = null;
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = false;

      _bocList.LoadValue(true);
      Assert.That(_bocList.Value, Is.SameAs(_businessObject.ReferenceList));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithIReadOnlyListAndDirty ()
    {
      _businessObject.ReferenceList = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = null;
      _bocList.IsDirty = true;

      _bocList.LoadValue(false);
      Assert.That(_bocList.Value, Is.SameAs(_businessObject.ReferenceList));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithIListAndDirty ()
    {
      _businessObject.ReferenceListAsList = new ArrayList(new[] { TypeWithReference.Create(), TypeWithReference.Create() });
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceListAsList;
      _bocList.Value = null;
      _bocList.IsDirty = true;

      _bocList.LoadValue(false);
      Assert.That(_bocList.Value, Is.EqualTo(_businessObject.ReferenceListAsList));
      Assert.That(_bocList.ValueAsList, Is.SameAs(_businessObject.ReferenceListAsList));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithNullAndDirty ()
    {
      _businessObject.ReferenceList = null;
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = true;

      _bocList.LoadValue(false);
      Assert.That(_bocList.Value, Is.SameAs(_businessObject.ReferenceList));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceNull ()
    {
      TypeWithReference[] value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = null;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = value;
      _bocList.IsDirty = true;

      _bocList.LoadValue(false);
      Assert.That(_bocList.Value, Is.EqualTo(value));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithPropertyNull ()
    {
      TypeWithReference[] value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Property = null;
      _bocList.Value = value;
      _bocList.IsDirty = true;

      _bocList.LoadValue(false);
      Assert.That(_bocList.Value, Is.EqualTo(value));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void LoadValueAndInterimFalseWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.IsDirty = true;

      _bocList.LoadValue(false);
      Assert.That(_bocList.Value, Is.EqualTo(null));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadValueAndInterimTrueWithDataSourceBusinessObjectNull ()
    {
      _dataSource.BusinessObject = null;
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.IsDirty = true;

      _bocList.LoadValue(true);
      Assert.That(_bocList.Value, Is.EqualTo(null));
      Assert.That(_bocList.IsDirty, Is.True);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrueWithListAndDirty ()
    {
      TypeWithReference[] value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Value = null;
      _bocList.IsDirty = true;

      _bocList.LoadUnboundValue(value, true);
      Assert.That(_bocList.Value, Is.SameAs(value));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAndInterimTrueWithNullAndDirty ()
    {
      TypeWithReference[] value = null;
      _bocList.DataSource = _dataSource;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = true;

      _bocList.LoadUnboundValue(value, true);
      Assert.That(_bocList.Value, Is.SameAs(value));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAndInterimTrueWithListAndNotDirty ()
    {
      TypeWithReference[] value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Value = null;
      _bocList.IsDirty = false;

      _bocList.LoadUnboundValue(value, true);
      Assert.That(_bocList.Value, Is.SameAs(value));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimTrueWithNullAndNotDirty ()
    {
      TypeWithReference[] value = null;
      _bocList.DataSource = _dataSource;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = false;

      _bocList.LoadUnboundValue(value, true);
      Assert.That(_bocList.Value, Is.SameAs(value));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithListAndDirty ()
    {
      TypeWithReference[] value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Value = null;
      _bocList.IsDirty = true;

      _bocList.LoadUnboundValue(value, false);
      Assert.That(_bocList.Value, Is.SameAs(value));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNullAndDirty ()
    {
      TypeWithReference[] value = null;
      _bocList.DataSource = _dataSource;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = true;

      _bocList.LoadUnboundValue(value, false);
      Assert.That(_bocList.Value, Is.SameAs(value));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAsListAndInterimTrueWithListAndDirty ()
    {
      IList value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Value = null;
      _bocList.IsDirty = true;

      _bocList.LoadUnboundValueAsList(value, true);
      Assert.That(_bocList.Value, Is.SameAs(value));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAsListAndInterimTrueWithNullAndDirty ()
    {
      IList value = null;
      _bocList.DataSource = _dataSource;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = true;

      _bocList.LoadUnboundValueAsList(value, true);
      Assert.That(_bocList.Value, Is.SameAs(value));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void LoadUnboundValueAsListAndInterimTrueWithListAndNotDirty ()
    {
      IList value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Value = null;
      _bocList.IsDirty = false;

      _bocList.LoadUnboundValueAsList(value, true);
      Assert.That(_bocList.Value, Is.SameAs(value));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAsListAndInterimTrueWithNullAndNotDirty ()
    {
      IList value = null;
      _bocList.DataSource = _dataSource;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = false;

      _bocList.LoadUnboundValueAsList(value, true);
      Assert.That(_bocList.Value, Is.SameAs(value));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAsListAndInterimFalseWithListAndDirty ()
    {
      IList value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Value = null;
      _bocList.IsDirty = true;

      _bocList.LoadUnboundValueAsList(value, false);
      Assert.That(_bocList.Value, Is.SameAs(value));
      Assert.That(_bocList.ValueAsList, Is.SameAs(value));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAsListAndInterimFalseWithNullAndDirty ()
    {
      IList value = null;
      _bocList.DataSource = _dataSource;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = true;

      _bocList.LoadUnboundValueAsList(value, false);
      Assert.That(_bocList.Value, Is.SameAs(value));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void LoadUnboundValueAsListAndValueIsIList ()
    {
      IList value = new ArrayList();
      value.Add(TypeWithReference.Create());
      _bocList.DataSource = _dataSource;
      _bocList.Value = new TypeWithReference[0];
      _bocList.IsDirty = true;

      _bocList.LoadUnboundValueAsList(value, false);
      Assert.That(_bocList.Value, Is.EqualTo(value));
      Assert.That(((BusinessObjectListAdapter<IBusinessObject>)_bocList.Value).WrappedList, Is.SameAs(value));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndInterimTrue ()
    {
      _businessObject.ReferenceList = new[] { TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      TypeWithReference[] newValue = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.Value = newValue;
      _bocList.IsDirty = true;

      var result = _bocList.SaveValue(true);
      Assert.That(result, Is.True);
      Assert.That(_businessObject.ReferenceList, Is.EqualTo(newValue));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndInterimFalse ()
    {
      _businessObject.ReferenceList = new[] { TypeWithReference.Create() };
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      TypeWithReference[] newValue = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.Value = newValue;
      _bocList.IsDirty = true;

      var result = _bocList.SaveValue(false);
      Assert.That(result, Is.True);
      Assert.That(_businessObject.ReferenceList, Is.EqualTo(newValue));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndNotValid ()
    {
      var oldValue = new[] { TypeWithReference.Create() };
      _businessObject.ReferenceList = oldValue;
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      TypeWithReference[] newValue = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.Value = newValue;
      _bocList.IsDirty = true;
      _bocList.RegisterValidator(new AlwaysInvalidValidator());

      var result = _bocList.SaveValue(false);
      Assert.That(result, Is.False);
      Assert.That(_businessObject.ReferenceList, Is.EqualTo(oldValue));
      Assert.That(_bocList.IsDirty, Is.True);
    }

    [Test]
    public void SaveValueAndIsDirtyFalse ()
    {
      var oldValue = new[] { TypeWithReference.Create() };
      _businessObject.ReferenceList = oldValue;
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create(), TypeWithReference.Create() };
      _bocList.IsDirty = false;

      var result = _bocList.SaveValue(false);
      Assert.That(result, Is.True);
      Assert.That(_businessObject.ReferenceList, Is.EqualTo(oldValue));
      Assert.That(_bocList.IsDirty, Is.False);
    }

    [Test]
    public void SaveValueAndIsRowEditModeActiveTrueAndInterimTrue ()
    {
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create() };
      _bocList.SwitchRowIntoEditMode(0);
      Assert.That(_bocList.IsRowEditModeActive, Is.True);

      var result = _bocList.SaveValue(true);
      Assert.That(result, Is.True);
      Assert.That(_bocList.IsRowEditModeActive, Is.True);
    }

    [Test]
    public void SaveValueAndIsRowEditModeActiveTrueAndInterimFalse ()
    {
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create() };
      _bocList.SwitchRowIntoEditMode(0);
      Assert.That(_bocList.IsRowEditModeActive, Is.True);

      var result = _bocList.SaveValue(false);
      Assert.That(result, Is.True);
      Assert.That(_bocList.IsRowEditModeActive, Is.False);
    }

    [Test]
    public void SaveValueAndIsListEditModeActiveTrueAndInterimTrue ()
    {
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create() };
      _bocList.SwitchListIntoEditMode();
      Assert.That(_bocList.IsListEditModeActive, Is.True);

      var result = _bocList.SaveValue(true);
      Assert.That(result, Is.True);
      Assert.That(_bocList.IsListEditModeActive, Is.True);
    }

    [Test]
    public void SaveValueAndIsListEditModeActiveTrueAndInterimFalse ()
    {
      _bocList.DataSource = _dataSource;
      _bocList.Property = _propertyReferenceList;
      _bocList.Value = new[] { TypeWithReference.Create() };
      _bocList.SwitchListIntoEditMode();
      Assert.That(_bocList.IsListEditModeActive, Is.True);

      var result = _bocList.SaveValue(false);
      Assert.That(result, Is.True);
      Assert.That(_bocList.IsListEditModeActive, Is.False);
    }

    [Test]
    public void CreateValidators_UsesValidatorFactory ()
    {
      var control = new BocList();
      var serviceLocatorMock = new Mock<IServiceLocator>();
      var factoryMock = new Mock<IBocListValidatorFactory>();
      serviceLocatorMock.Setup(m => m.GetInstance<IBocListValidatorFactory>()).Returns(factoryMock.Object).Verifiable();
      factoryMock.Setup(f => f.CreateValidators(control, false)).Returns(new List<BaseValidator>()).Verifiable();

      using (new ServiceLocatorScope(serviceLocatorMock.Object))
      {
        control.CreateValidators();
      }

      factoryMock.Verify();
      serviceLocatorMock.Verify();
    }
  }
}
