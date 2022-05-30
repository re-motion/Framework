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
using Remotion.ObjectBinding.UnitTests.BusinessObjectReferenceDataSourceBaseTests.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectReferenceDataSourceBaseTests
{
#pragma warning disable 612,618
  [TestFixture]
  public class SaveValue
  {
    private Mock<IBusinessObjectReferenceProperty> _referencePropertyStub;
    private Mock<IBusinessObjectReferenceProperty> _readOnlyReferencePropertyStub;
    private Mock<IBusinessObjectDataSource> _referencedDataSourceStub;

    [SetUp]
    public void SetUp ()
    {
      _referencedDataSourceStub = new Mock<IBusinessObjectDataSource>();
      _referencedDataSourceStub.SetupProperty(_ => _.BusinessObject);
      _referencedDataSourceStub.Object.BusinessObject = new Mock<IBusinessObject>().Object;
      _referencedDataSourceStub.Setup(_ => _.BusinessObjectClass).Returns(new Mock<IBusinessObjectClass>().Object);
      _referencedDataSourceStub.Object.Mode = DataSourceMode.Edit;
      _referencePropertyStub = new Mock<IBusinessObjectReferenceProperty>();
      _referencePropertyStub.Setup(_ => _.ReflectedClass).Returns(new Mock<IBusinessObjectClass>().Object);
      _referencePropertyStub.Setup(stub => stub.ReferenceClass).Returns(new Mock<IBusinessObjectClass>().Object);
      _readOnlyReferencePropertyStub = new Mock<IBusinessObjectReferenceProperty>();
      _readOnlyReferencePropertyStub.Setup(_ => _.ReflectedClass).Returns(new Mock<IBusinessObjectClass>().Object);
      _readOnlyReferencePropertyStub.Setup(stub => stub.ReferenceClass).Returns(new Mock<IBusinessObjectClass>().Object);
      _readOnlyReferencePropertyStub
          .Setup(stub => stub.IsReadOnly(It.IsAny<IBusinessObject>()))
          .Returns(true);
    }

    [Test]
    public void SavesValueIntoBoundObject_AndReturnsTrue ()
    {
      var expectedValue = new Mock<IBusinessObject>();

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = expectedValue.Object;

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(_referencePropertyStub.Object, expectedValue.Object), Times.AtLeastOnce());
    }

    [Test]
    public void ClearsHasBusinessObjectChanged ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = new Mock<IBusinessObject>().Object;
      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.True);

      referenceDataSource.SaveValue(false);

      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void ClearsHasBusinessObjectCreated ()
    {
      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _referencePropertyStub.Setup(stub => stub.CreateDefaultValue(_referencedDataSourceStub.Object.BusinessObject))
          .Returns(new Mock<IBusinessObject>().Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;
      referenceDataSource.LoadValue(false);
      Assert.That(referenceDataSource.HasBusinessObjectCreated, Is.True);

      referenceDataSource.SaveValue(false);

      Assert.That(referenceDataSource.HasBusinessObjectCreated, Is.False);
    }

    [Test]
    public void ParentIsNull_DoesNotSaveValueIntoBoundObject_AndReturnsFalse ()
    {
      var parentObjectStub = _referencedDataSourceStub.Object.BusinessObject;
      _referencedDataSourceStub.Object.BusinessObject = null;
      var expectedValue = new Mock<IBusinessObject>();

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = expectedValue.Object;

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.False);
      Mock.Get(parentObjectStub).Verify(stub => stub.SetProperty(It.IsAny<IBusinessObjectProperty>(), It.IsAny<object>()), Times.Never());
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_ClearsHasBusinessObjectChanged ()
    {
      _referencedDataSourceStub.Object.BusinessObject = null;

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = new Mock<IBusinessObject>().Object;
      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.True);

      referenceDataSource.SaveValue(false);

      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_ClearsHasBusinessObjectCreated ()
    {
      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _referencePropertyStub.Setup(stub => stub.CreateDefaultValue(_referencedDataSourceStub.Object.BusinessObject))
          .Returns(new Mock<IBusinessObject>().Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;
      referenceDataSource.LoadValue(false);
      Assert.That(referenceDataSource.HasBusinessObjectCreated, Is.True);

      _referencedDataSourceStub.Object.BusinessObject = null;
      referenceDataSource.SaveValue(false);

      Assert.That(referenceDataSource.HasBusinessObjectCreated, Is.False);
    }

    [Test]
    public void HasBusinessObjectChangedFalse_DoesNotSaveValueIntoBoundObject_AndReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Setup(stub => stub.GetProperty(_referencePropertyStub.Object))
          .Returns(new Mock<IBusinessObject>().Object);
      referenceDataSource.LoadValue(false);
      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.False);

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(It.IsAny<IBusinessObjectProperty>(), It.IsAny<object>()), Times.Never());
    }

    [Test]
    public void HasBusinessObjectChangedFalse_BusinessObjectNull_DoesNotSaveValueIntoBoundObject_AndReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Setup(stub => stub.GetProperty(_referencePropertyStub.Object)).Returns((object)null);
      referenceDataSource.LoadValue(false);
      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.False);

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(It.IsAny<IBusinessObjectProperty>(), It.IsAny<object>()), Times.Never());
    }

    [Test]
    public void ReferencedDataSourceNull_DoesNotReadBusinessObject_DoesNotClearHasBusinessObjectChanged ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(null, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = new Mock<IBusinessObject>().Object;

      referenceDataSource.SaveValue(false);

      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(It.IsAny<IBusinessObjectProperty>(), It.IsAny<object>()), Times.Never());
      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.True);
    }

    [Test]
    public void ReferencedPropertyNull_DoesNotReadBusinessObject ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, null);
      referenceDataSource.BusinessObject = new Mock<IBusinessObject>().Object;

      referenceDataSource.SaveValue(false);

      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(It.IsAny<IBusinessObjectProperty>(), It.IsAny<object>()), Times.Never());
    }

    [Test]
    public void ReferenceClassRequiresWriteBackTrue_ReadsBusinessObject_SavesValueIntoBoundObject_AndReturnsTrue ()
    {
      var expectedValue = new Mock<IBusinessObject>();
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Setup(stub => stub.GetProperty(_referencePropertyStub.Object)).Returns(expectedValue.Object);
      Mock.Get(_referencePropertyStub.Object.ReferenceClass).Setup(stub => stub.RequiresWriteBack).Returns(true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.LoadValue(false);
      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.False);

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(_referencePropertyStub.Object, expectedValue.Object), Times.AtLeastOnce());
    }

    [Test]
    public void HasBusinessObjectCreatedTrue_ReadsBusinessObject_SavesValueIntoBoundObject_AndReturnsTrue ()
    {
      var expectedValue = new Mock<IBusinessObject>();
      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _referencePropertyStub.Setup(stub => stub.CreateDefaultValue(_referencedDataSourceStub.Object.BusinessObject)).Returns(expectedValue.Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;
      referenceDataSource.LoadValue(false);
      Assert.That(referenceDataSource.HasBusinessObjectCreated, Is.True);

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(_referencePropertyStub.Object, expectedValue.Object), Times.AtLeastOnce());
    }

    [Test]
    public void SavesValuesForBoundControls_OwnBusinessObjectNotNull ()
    {
      var expectedValue = new Mock<IBusinessObject>();
      bool isControlSaved = false;

      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Setup(stub => stub.SetProperty(_referencePropertyStub.Object, expectedValue.Object))
// ReSharper disable AccessToModifiedClosure
          .Callback((IBusinessObjectProperty property, object value) => Assert.That(isControlSaved, Is.True));
// ReSharper restore AccessToModifiedClosure

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = expectedValue.Object;

      var firstControlMock = new Mock<IBusinessObjectBoundEditableControl>();
      firstControlMock.Setup(stub => stub.HasValidBinding).Returns(true);
      referenceDataSource.Register(firstControlMock.Object);

      var secondControlMock = new Mock<IBusinessObjectBoundEditableControl>();
      secondControlMock.Setup(stub => stub.HasValidBinding).Returns(true);
      referenceDataSource.Register(secondControlMock.Object);

      firstControlMock.Setup(mock => mock.SaveValue(false)).Returns(true).Callback((bool interim) => isControlSaved = true).Verifiable();
      secondControlMock.Setup(mock => mock.SaveValue(false)).Returns(true).Verifiable();

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      firstControlMock.Verify();
      secondControlMock.Verify();
    }

    [Test]
    public void SavesValuesForBoundControls_OwnBusinessObjectNull_AndReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = null;

      var controlMock = new Mock<IBusinessObjectBoundEditableControl>();
      controlMock.Setup(stub => stub.HasValidBinding).Returns(true);
      referenceDataSource.Register(controlMock.Object);

      controlMock.Setup(mock => mock.SaveValue(false)).Returns(true).Verifiable();

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      controlMock.Verify();
    }

    [Test]
    public void SavesValuesForBoundControls_NotAllControlsCanSave_ReturnsFalse ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = null;

      var firstControlMock = new Mock<IBusinessObjectBoundEditableControl>();
      firstControlMock.Setup(stub => stub.HasValidBinding).Returns(true);
      referenceDataSource.Register(firstControlMock.Object);

      var secondControlMock = new Mock<IBusinessObjectBoundEditableControl>();
      secondControlMock.Setup(stub => stub.HasValidBinding).Returns(true);
      referenceDataSource.Register(secondControlMock.Object);

      firstControlMock.Setup(mock => mock.SaveValue(false)).Returns(false).Verifiable();
      secondControlMock.Setup(mock => mock.SaveValue(false)).Returns(true).Verifiable();

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.False);
      firstControlMock.Verify();
      secondControlMock.Verify();
    }

    [Test]
    public void IsDefaultValueTrue_SupportsDeleteTrue_DeletesObject_AndReturnsTrue ()
    {
      var referencedObject = new Mock<IBusinessObject>();

      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _referencePropertyStub
          .Setup(stub => stub.IsDefaultValue(_referencedDataSourceStub.Object.BusinessObject, referencedObject.Object, new IBusinessObjectProperty[0]))
          .Returns(true);
      _referencePropertyStub.Setup(stub => stub.SupportsDelete).Returns(true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = referencedObject.Object;

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      _referencePropertyStub.Verify(stub => stub.Delete(_referencedDataSourceStub.Object.BusinessObject, referencedObject.Object), Times.AtLeastOnce());
    }

    [Test]
    public void IsDefaultValueTrue_SupportsDeleteFales_DoesNotDeleteObject_AndReturnsTrue ()
    {
      var referencedObject = new Mock<IBusinessObject>();

      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _referencePropertyStub.Setup(stub => stub.IsDefaultValue(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty[]>())).Returns(true);
      _referencePropertyStub.Setup(stub => stub.SupportsDelete).Returns(false);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = referencedObject.Object;

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      _referencePropertyStub.Verify(stub => stub.Delete(_referencedDataSourceStub.Object.BusinessObject, referencedObject.Object), Times.Never());
      Assert.That(referenceDataSource.BusinessObject, Is.Null);
    }

    [Test]
    public void IsDefaultValue_SavesNullIntoBoundObject_AndReturnsTrue ()
    {
      var referencedObject = new Mock<IBusinessObject>();

      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _referencePropertyStub.Setup(stub => stub.IsDefaultValue(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty[]>())).Returns(true);
      _referencePropertyStub.Setup(stub => stub.SupportsDelete).Returns(true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = referencedObject.Object;

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(_referencePropertyStub.Object, null), Times.AtLeastOnce());
    }

    [Test]
    public void IsDefaultValue_ClearsBusinessObject_AndReturnsTrue ()
    {
      var referencedObject = new Mock<IBusinessObject>();

      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _referencePropertyStub.Setup(stub => stub.IsDefaultValue(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty[]>())).Returns(true);
      _referencePropertyStub.Setup(stub => stub.SupportsDelete).Returns(true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = referencedObject.Object;

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      Assert.That(referenceDataSource.BusinessObject, Is.Null);
    }

    [Test]
    public void IsDefaultValue_ClearsHasBusinessObjectChanged ()
    {
      var referencedObject = new Mock<IBusinessObject>();

      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _referencePropertyStub.Setup(stub => stub.IsDefaultValue(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty[]>())).Returns(true);
      _referencePropertyStub.Setup(stub => stub.SupportsDelete).Returns(true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = referencedObject.Object;

      referenceDataSource.SaveValue(false);

      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void IsDefaultValue_SavesValuesForBoundControls_AndReturnsTrue ()
    {
      var referencedObject = new Mock<IBusinessObject>();

      var firstPropertyStub = new Mock<IBusinessObjectProperty>();
      var secondPropertyStub = new Mock<IBusinessObjectProperty>();

      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _referencePropertyStub.Setup(stub => stub.IsDefaultValue(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty[]>())).Returns(true);
      _referencePropertyStub.Setup(stub => stub.SupportsDelete).Returns(true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = referencedObject.Object;

      var firstControlMock = new Mock<IBusinessObjectBoundEditableControl>();
      firstControlMock.Setup(stub => stub.HasValidBinding).Returns(true);
      firstControlMock.Setup(stub => stub.HasValue).Returns(false);
      firstControlMock.Setup(mock => mock.SaveValue(false)).Returns(true).Verifiable();
      firstControlMock.Object.Property = firstPropertyStub.Object;
      referenceDataSource.Register(firstControlMock.Object);

      var secondControlMock = new Mock<IBusinessObjectBoundEditableControl>();
      secondControlMock.Setup(stub => stub.HasValidBinding).Returns(true);
      secondControlMock.Setup(stub => stub.HasValue).Returns(false);
      secondControlMock.Setup(mock => mock.SaveValue(false)).Returns(true).Verifiable();
      secondControlMock.Object.Property = secondPropertyStub.Object;
      referenceDataSource.Register(secondControlMock.Object);

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      firstControlMock.Verify();
      secondControlMock.Verify();
    }

    [Test]
    public void IsDefaultValue_RequiresAllBoundControlsEmpty_ContainsOnlyEmtpyControls_AndReturnsTrue ()
    {
      var referencedObject = new Mock<IBusinessObject>();

      var firstPropertyStub = new Mock<IBusinessObjectProperty>();
      var secondPropertyStub = new Mock<IBusinessObjectProperty>();

      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _referencePropertyStub
          .Setup(
              stub =>
              stub.IsDefaultValue(_referencedDataSourceStub.Object.BusinessObject, referencedObject.Object, new[] { firstPropertyStub.Object, secondPropertyStub.Object }))
          .Returns(true);
      _referencePropertyStub.Setup(stub => stub.SupportsDelete).Returns(true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = referencedObject.Object;

      var firstControlStub = new Mock<IBusinessObjectBoundControl>();
      firstControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      firstControlStub.Setup(stub => stub.HasValue).Returns(false);
      firstControlStub.SetupProperty(_ => _.Property);
      firstControlStub.Object.Property = firstPropertyStub.Object;
      referenceDataSource.Register(firstControlStub.Object);

      var secondControlStub = new Mock<IBusinessObjectBoundControl>();
      secondControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      secondControlStub.Setup(stub => stub.HasValue).Returns(false);
      secondControlStub.SetupProperty(_ => _.Property);
      secondControlStub.Object.Property = secondPropertyStub.Object;
      referenceDataSource.Register(secondControlStub.Object);

      var thirdControlStub = new Mock<IBusinessObjectBoundControl>();
      thirdControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      thirdControlStub.Setup(stub => stub.HasValue).Returns(false);
      thirdControlStub.SetupProperty(_ => _.Property);
      thirdControlStub.Object.Property = firstPropertyStub.Object;
      referenceDataSource.Register(thirdControlStub.Object);

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      _referencePropertyStub.Verify(stub => stub.Delete(_referencedDataSourceStub.Object.BusinessObject, referencedObject.Object), Times.AtLeastOnce());
      Assert.That(referenceDataSource.BusinessObject, Is.Null);
      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void IsDefaultValue_RequiresAllBoundControlsEmpty_ContainsNonEmtpyControls_AndReturnsTrue ()
    {
      var referencedObject = new Mock<IBusinessObject>();

      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _referencePropertyStub.Setup(stub => stub.SupportsDelete).Returns(true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = referencedObject.Object;

      var firstControlStub = new Mock<IBusinessObjectBoundControl>();
      firstControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      firstControlStub.Setup(stub => stub.HasValue).Returns(false);
      referenceDataSource.Register(firstControlStub.Object);

      var secondControlStub = new Mock<IBusinessObjectBoundControl>();
      secondControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      secondControlStub.Setup(stub => stub.HasValue).Returns(true);
      referenceDataSource.Register(secondControlStub.Object);

      var thirdControlStub = new Mock<IBusinessObjectBoundControl>();
      thirdControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      thirdControlStub.Setup(stub => stub.HasValue).Returns(false);
      referenceDataSource.Register(thirdControlStub.Object);

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      _referencePropertyStub.Verify(stub => stub.IsDefaultValue(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty[]>()), Times.Never());
      _referencePropertyStub.Verify(stub => stub.Delete(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>()), Times.Never());
      Assert.That(referenceDataSource.BusinessObject, Is.SameAs(referencedObject.Object));
      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void BusinessObjectNull_IgnoresDefaultValueSemantics_AndReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = null;

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      _referencePropertyStub.Verify(stub => stub.SupportsDefaultValue, Times.Never());
      _referencePropertyStub.Verify(stub => stub.SupportsDelete, Times.Never());
      _referencePropertyStub.Verify(stub => stub.IsDefaultValue(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty[]>()), Times.Never());
      _referencePropertyStub.Verify(stub => stub.Delete(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>()), Times.Never());
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(_referencePropertyStub.Object, null), Times.AtLeastOnce());
    }

    [Test]
    public void SupportsDefaultValueFalse_IsDefaultValueNotCalled ()
    {
      var referencedObject = new Mock<IBusinessObject>();

      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(false);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = referencedObject.Object;

      referenceDataSource.SaveValue(false);

      _referencePropertyStub.Verify(stub => stub.IsDefaultValue(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty[]>()), Times.Never());
    }

    [Test]
    public void InterimSave_DoesNotUseDefaultValueSemantics_AndReturnsTrue ()
    {
      var expectedValue = new Mock<IBusinessObject>();
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = expectedValue.Object;

      var firstControlMock = new Mock<IBusinessObjectBoundEditableControl>();
      firstControlMock.Setup(stub => stub.HasValidBinding).Returns(true);
      referenceDataSource.Register(firstControlMock.Object);

      firstControlMock.Setup(mock => mock.SaveValue(true)).Returns(true).Verifiable();

      var result = referenceDataSource.SaveValue(true);

      Assert.That(result, Is.True);
      firstControlMock.Verify();
      _referencePropertyStub.Verify(stub => stub.SupportsDefaultValue, Times.Never());
      _referencePropertyStub.Verify(stub => stub.SupportsDelete, Times.Never());
      _referencePropertyStub.Verify(stub => stub.IsDefaultValue(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty[]>()), Times.Never());
      _referencePropertyStub.Verify(stub => stub.Delete(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>()), Times.Never());
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(_referencePropertyStub.Object, expectedValue.Object), Times.AtLeastOnce());
    }

    [Test]
    public void PropertyIsReadWrite_SavesValueIntoBoundObject_AndReturnsTrue ()
    {
      var expectedValue = new Mock<IBusinessObject>();
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Setup(stub => stub.GetProperty(_referencePropertyStub.Object)).Returns(expectedValue.Object);
      Mock.Get(_referencePropertyStub.Object.ReferenceClass).Setup(stub => stub.RequiresWriteBack).Returns(true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.LoadValue(false);
      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.False);

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(_referencePropertyStub.Object, expectedValue.Object), Times.AtLeastOnce());
    }

    [Test]
    public void PropertyIsReadOnly_ThrowsInvalidOperationException ()
    {
      var expectedValue = new Mock<IBusinessObject>();
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Setup(stub => stub.GetProperty(_readOnlyReferencePropertyStub.Object)).Returns(expectedValue.Object);
      Mock.Get(_readOnlyReferencePropertyStub.Object.ReferenceClass).Setup(stub => stub.RequiresWriteBack).Returns(true);
      _readOnlyReferencePropertyStub.Setup(stub => stub.Identifier).Returns("TestProperty");

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _readOnlyReferencePropertyStub.Object);
      referenceDataSource.ID = "TestDataSource";
      referenceDataSource.LoadValue(false);
      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.False);

      Assert.That(
          () => referenceDataSource.SaveValue(false),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "The business object of the TestDataSource could not be saved into the domain model "
              + "because the property 'TestProperty' is read only."));

      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(It.IsAny<IBusinessObjectProperty>(), It.IsAny<object>()), Times.Never());
    }

    [Test]
    public void PropertyIsReadOnly_DoesNotSaveValueIntoBoundObject ()
    {
      var expectedValue = new Mock<IBusinessObject>();
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Setup(stub => stub.GetProperty(_readOnlyReferencePropertyStub.Object)).Returns(expectedValue.Object);
      Mock.Get(_readOnlyReferencePropertyStub.Object.ReferenceClass).Setup(stub => stub.RequiresWriteBack).Returns(true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _readOnlyReferencePropertyStub.Object);
      referenceDataSource.LoadValue(false);
      Assert.That(referenceDataSource.HasBusinessObjectChanged, Is.False);

      Assert.That(() => referenceDataSource.SaveValue(false), Throws.Exception);

      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(It.IsAny<IBusinessObjectProperty>(), It.IsAny<object>()), Times.Never());
    }

    [Test]
    public void PropertyIsReadWrite_IsDefaultValue_ClearsBusinessObject_AndReturnsTrue ()
    {
      var referencedObject = new Mock<IBusinessObject>();

      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _referencePropertyStub.Setup(stub => stub.IsDefaultValue(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty[]>())).Returns(true);
      _referencePropertyStub.Setup(stub => stub.SupportsDelete).Returns(true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = referencedObject.Object;

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      Assert.That(referenceDataSource.BusinessObject, Is.Null);
    }

    [Test]
    public void PropertyIsReadOnly_IsDefaultValue_ThrowsInvalidOperationException ()
    {
      var referencedObject = new Mock<IBusinessObject>();

      _readOnlyReferencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _readOnlyReferencePropertyStub.Setup(stub => stub.IsDefaultValue(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty[]>())).Returns(true);
      _readOnlyReferencePropertyStub.Setup(stub => stub.SupportsDelete).Returns(true);
      _readOnlyReferencePropertyStub.Setup(stub => stub.Identifier).Returns("TestProperty");
      Mock.Get(_referencedDataSourceStub.Object.BusinessObject).Setup(stub => stub.GetProperty(_readOnlyReferencePropertyStub.Object)).Returns(referencedObject.Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _readOnlyReferencePropertyStub.Object);
      referenceDataSource.ID = "TestDataSource";
      referenceDataSource.LoadValue(false);

      Assert.That(
          () => referenceDataSource.SaveValue(false),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "The TestableBusinessObjectReferenceDataSource 'TestDataSource' could not be marked as changed "
              + "because the bound property 'TestProperty' is read only."));

      Assert.That(referenceDataSource.BusinessObject, Is.Null);
    }

    [Test]
    public void PropertyIsReadWrite_SavesValuesForBoundControls_AndReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);

      var controlMock = new Mock<IBusinessObjectBoundEditableControl>();
      controlMock.Setup(stub => stub.HasValidBinding).Returns(true);
      referenceDataSource.Register(controlMock.Object);

      controlMock.Setup(mock => mock.SaveValue(false)).Returns(true).Verifiable();

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      controlMock.Verify();
    }

    [Test]
    public void PropertyIsReadOnly_SavesValuesForBoundControls_AndReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _readOnlyReferencePropertyStub.Object);

      var controlMock = new Mock<IBusinessObjectBoundEditableControl>();
      controlMock.Setup(stub => stub.HasValidBinding).Returns(true);
      referenceDataSource.Register(controlMock.Object);

      controlMock.Setup(mock => mock.SaveValue(false)).Returns(true).Verifiable();

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      controlMock.Verify();
    }

    [Test]
    public void DoesNotRequireWriteBack_AllControlsCanSave_ReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      Mock.Get(_referencedDataSourceStub.Object.BusinessObjectClass).Setup(_ => _.RequiresWriteBack).Returns(false);
      _referencePropertyStub.Setup(_ => _.IsReadOnly(It.IsAny<IBusinessObject>())).Returns(true); // IsReadOnlyInDomain to ensure exit branch

      var controlMock = new Mock<IBusinessObjectBoundEditableControl>();
      controlMock.Setup(stub => stub.HasValidBinding).Returns(true);
      referenceDataSource.Register(controlMock.Object);

      controlMock.Setup(mock => mock.SaveValue(false)).Returns(true).Verifiable();

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
      controlMock.Verify();
    }

    [Test]
    public void DoesNotRequireWriteBack_WithoutControls_ReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      Mock.Get(_referencedDataSourceStub.Object.BusinessObjectClass).Setup(_ => _.RequiresWriteBack).Returns(false);
      _referencePropertyStub.Setup(_ => _.IsReadOnly(It.IsAny<IBusinessObject>())).Returns(true); // IsReadOnlyInDomain to ensure exit branch

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.True);
    }

    [Test]
    public void DoesNotRequireWriteBack_NotAllControlsCanSave_ReturnsFalse ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource(_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      Mock.Get(_referencedDataSourceStub.Object.BusinessObjectClass).Setup(_ => _.RequiresWriteBack).Returns(false);
      _referencePropertyStub.Setup(_ => _.IsReadOnly(It.IsAny<IBusinessObject>())).Returns(true); // IsReadOnlyInDomain to ensure exit branch

      var controlMock = new Mock<IBusinessObjectBoundEditableControl>();
      controlMock.Setup(stub => stub.HasValidBinding).Returns(true);
      referenceDataSource.Register(controlMock.Object);

      controlMock.Setup(mock => mock.SaveValue(false)).Returns(false).Verifiable();

      var result = referenceDataSource.SaveValue(false);

      Assert.That(result, Is.False);
      controlMock.Verify();
    }
  }
#pragma warning restore 612,618
}
