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
using Remotion.ObjectBinding.UnitTests.BusinessObjectReferenceDataSourceBaseTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectReferenceDataSourceBaseTests
{
#pragma warning disable 612,618
  [TestFixture]
  public class SaveValue
  {
    private IBusinessObjectReferenceProperty _referencePropertyStub;
    private IBusinessObjectReferenceProperty _readOnlyReferencePropertyStub;
    private IBusinessObjectDataSource _referencedDataSourceStub;

    [SetUp]
    public void SetUp ()
    {
      _referencedDataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      _referencedDataSourceStub.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();
      _referencedDataSourceStub.Stub (_ => _.BusinessObjectClass).Return (MockRepository.GenerateStub<IBusinessObjectClass>());
      _referencedDataSourceStub.Mode = DataSourceMode.Edit;
      _referencePropertyStub = MockRepository.GenerateStub<IBusinessObjectReferenceProperty>();
      _referencePropertyStub.Stub (_ => _.ReflectedClass).Return (MockRepository.GenerateStub<IBusinessObjectClass>());
      _referencePropertyStub.Stub (stub => stub.ReferenceClass).Return (MockRepository.GenerateStub<IBusinessObjectClass>());
      _readOnlyReferencePropertyStub = MockRepository.GenerateStub<IBusinessObjectReferenceProperty>();
      _readOnlyReferencePropertyStub.Stub (_ => _.ReflectedClass).Return (MockRepository.GenerateStub<IBusinessObjectClass>());
      _readOnlyReferencePropertyStub.Stub (stub => stub.ReferenceClass).Return (MockRepository.GenerateStub<IBusinessObjectClass>());
      _readOnlyReferencePropertyStub
          .Stub (stub => stub.IsReadOnly (Arg<IBusinessObject>.Is.Anything))
          .Return (true);
    }

    [Test]
    public void SavesValueIntoBoundObject_AndReturnsTrue ()
    {
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = expectedValue;

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      _referencedDataSourceStub.BusinessObject.AssertWasCalled (stub => stub.SetProperty (_referencePropertyStub, expectedValue));
    }

    [Test]
    public void ClearsHasBusinessObjectChanged ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);

      referenceDataSource.SaveValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void ClearsHasBusinessObjectCreated ()
    {
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject))
          .Return (MockRepository.GenerateStub<IBusinessObject> ());

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;
      referenceDataSource.LoadValue (false);
      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);

      referenceDataSource.SaveValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.False);
    }

    [Test]
    public void ParentIsNull_DoesNotSaveValueIntoBoundObject_AndReturnsFalse ()
    {
      var parentObjectStub = _referencedDataSourceStub.BusinessObject;
      _referencedDataSourceStub.BusinessObject = null;
      var expectedValue = MockRepository.GenerateStub<IBusinessObject> ();

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = expectedValue;

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.False);
      parentObjectStub.AssertWasNotCalled (stub => stub.SetProperty (null, null), options=>options.IgnoreArguments());
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_ClearsHasBusinessObjectChanged ()
    {
      _referencedDataSourceStub.BusinessObject = null;

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = MockRepository.GenerateStub<IBusinessObject> ();
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);

      referenceDataSource.SaveValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_ClearsHasBusinessObjectCreated ()
    {
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject))
          .Return (MockRepository.GenerateStub<IBusinessObject> ());

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;
      referenceDataSource.LoadValue (false);
      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);

      _referencedDataSourceStub.BusinessObject = null;
      referenceDataSource.SaveValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.False);
    }

    [Test]
    public void HasBusinessObjectChangedFalse_DoesNotSaveValueIntoBoundObject_AndReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub))
          .Return (MockRepository.GenerateStub<IBusinessObject>());
      referenceDataSource.LoadValue (false);
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      _referencedDataSourceStub.BusinessObject.AssertWasNotCalled (stub => stub.SetProperty (null, null), options => options.IgnoreArguments());
    }

    [Test]
    public void HasBusinessObjectChangedFalse_BusinessObjectNull_DoesNotSaveValueIntoBoundObject_AndReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null);
      referenceDataSource.LoadValue (false);
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      _referencedDataSourceStub.BusinessObject.AssertWasNotCalled (stub => stub.SetProperty (null, null), options => options.IgnoreArguments ());
    }

    [Test]
    public void ReferencedDataSourceNull_DoesNotReadBusinessObject_DoesNotClearHasBusinessObjectChanged ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (null, _referencePropertyStub);
      referenceDataSource.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();

      referenceDataSource.SaveValue (false);

      _referencedDataSourceStub.BusinessObject.AssertWasNotCalled (stub => stub.SetProperty (null, null), options => options.IgnoreArguments ());
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);
    }

    [Test]
    public void ReferencedPropertyNull_DoesNotReadBusinessObject ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, null);
      referenceDataSource.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();

      referenceDataSource.SaveValue (false);

      _referencedDataSourceStub.BusinessObject.AssertWasNotCalled (stub => stub.SetProperty (null, null), options => options.IgnoreArguments ());
    }

    [Test]
    public void ReferenceClassRequiresWriteBackTrue_ReadsBusinessObject_SavesValueIntoBoundObject_AndReturnsTrue ()
    {
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (expectedValue);
      _referencePropertyStub.ReferenceClass.Stub (stub => stub.RequiresWriteBack).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.LoadValue (false);
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      _referencedDataSourceStub.BusinessObject.AssertWasCalled (stub => stub.SetProperty (_referencePropertyStub, expectedValue));
    }

    [Test]
    public void HasBusinessObjectCreatedTrue_ReadsBusinessObject_SavesValueIntoBoundObject_AndReturnsTrue ()
    {
      var expectedValue = MockRepository.GenerateStub<IBusinessObject> ();
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject)).Return (expectedValue);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;
      referenceDataSource.LoadValue (false);
      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      _referencedDataSourceStub.BusinessObject.AssertWasCalled (stub => stub.SetProperty (_referencePropertyStub, expectedValue));
    }

    [Test]
    public void SavesValuesForBoundControls_OwnBusinessObjectNotNull ()
    {
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      bool isControlSaved = false;

      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.SetProperty (_referencePropertyStub, expectedValue))
// ReSharper disable AccessToModifiedClosure
          .WhenCalled (mi => Assert.That (isControlSaved, Is.True));
// ReSharper restore AccessToModifiedClosure

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = expectedValue;

      var firstControlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl>();
      firstControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (firstControlMock);

      var secondControlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl>();
      secondControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (secondControlMock);

      firstControlMock.Expect (mock => mock.SaveValue (false)).Return (true).WhenCalled (mi => isControlSaved = true);
      secondControlMock.Expect (mock => mock.SaveValue (false)).Return (true);

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      firstControlMock.VerifyAllExpectations();
      secondControlMock.VerifyAllExpectations();
    }

    [Test]
    public void SavesValuesForBoundControls_OwnBusinessObjectNull_AndReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = null;

      var controlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl> ();
      controlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (controlMock);

      controlMock.Expect (mock => mock.SaveValue (false)).Return (true);
      
      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      controlMock.VerifyAllExpectations ();
    }

    [Test]
    public void SavesValuesForBoundControls_NotAllControlsCanSave_ReturnsFalse ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = null;

      var firstControlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl> ();
      firstControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (firstControlMock);
      
      var secondControlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl> ();
      secondControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (secondControlMock);

      firstControlMock.Expect (mock => mock.SaveValue (false)).Return (false);
      secondControlMock.Expect (mock => mock.SaveValue (false)).Return (true);
      
      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.False);
      firstControlMock.VerifyAllExpectations ();
      secondControlMock.VerifyAllExpectations ();
    }

    [Test]
    public void IsDefaultValueTrue_SupportsDeleteTrue_DeletesObject_AndReturnsTrue ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub
          .Stub (stub => stub.IsDefaultValue (_referencedDataSourceStub.BusinessObject, referencedObject, new IBusinessObjectProperty[0]))
          .Return (true);
      _referencePropertyStub.Stub (stub => stub.SupportsDelete).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      _referencePropertyStub.AssertWasCalled (stub => stub.Delete (_referencedDataSourceStub.BusinessObject, referencedObject));
    }

    [Test]
    public void IsDefaultValueTrue_SupportsDeleteFales_DoesNotDeleteObject_AndReturnsTrue ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.IsDefaultValue (null, null, null)).IgnoreArguments ().Return (true);
      _referencePropertyStub.Stub (stub => stub.SupportsDelete).Return (false);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.Delete (_referencedDataSourceStub.BusinessObject, referencedObject));
      Assert.That (referenceDataSource.BusinessObject, Is.Null);
    }

    [Test]
    public void IsDefaultValue_SavesNullIntoBoundObject_AndReturnsTrue ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.IsDefaultValue (null, null, null)).IgnoreArguments ().Return (true);
      _referencePropertyStub.Stub (stub => stub.SupportsDelete).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      _referencedDataSourceStub.BusinessObject.AssertWasCalled (stub => stub.SetProperty (_referencePropertyStub, null));
    }

    [Test]
    public void IsDefaultValue_ClearsBusinessObject_AndReturnsTrue ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.IsDefaultValue (null, null, null)).IgnoreArguments ().Return (true);
      _referencePropertyStub.Stub (stub => stub.SupportsDelete).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.Null);
    }

    [Test]
    public void IsDefaultValue_ClearsHasBusinessObjectChanged ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.IsDefaultValue (null, null, null)).IgnoreArguments ().Return (true);
      _referencePropertyStub.Stub (stub => stub.SupportsDelete).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      referenceDataSource.SaveValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void IsDefaultValue_SavesValuesForBoundControls_AndReturnsTrue ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      var firstPropertyStub = MockRepository.GenerateStub<IBusinessObjectProperty> ();
      var secondPropertyStub = MockRepository.GenerateStub<IBusinessObjectProperty> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.IsDefaultValue (null, null, null)).IgnoreArguments().Return (true);
      _referencePropertyStub.Stub (stub => stub.SupportsDelete).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      var firstControlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl> ();
      firstControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      firstControlMock.Stub (stub => stub.HasValue).Return (false);
      firstControlMock.Expect (mock => mock.SaveValue (false)).Return (true);
      firstControlMock.Property = firstPropertyStub;
      referenceDataSource.Register (firstControlMock);

      var secondControlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl> ();
      secondControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      secondControlMock.Stub (stub => stub.HasValue).Return (false);
      secondControlMock.Expect (mock => mock.SaveValue (false)).Return (true);
      secondControlMock.Property = secondPropertyStub;
      referenceDataSource.Register (secondControlMock);

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      firstControlMock.VerifyAllExpectations();
      secondControlMock.VerifyAllExpectations();
    }

    [Test]
    public void IsDefaultValue_RequiresAllBoundControlsEmpty_ContainsOnlyEmtpyControls_AndReturnsTrue ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      var firstPropertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();
      var secondPropertyStub = MockRepository.GenerateStub<IBusinessObjectProperty> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub
          .Stub (
              stub =>
              stub.IsDefaultValue (_referencedDataSourceStub.BusinessObject, referencedObject, new[] { firstPropertyStub, secondPropertyStub }))
          .Return (true);
      _referencePropertyStub.Stub (stub => stub.SupportsDelete).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      var firstControlStub = MockRepository.GenerateStub<IBusinessObjectBoundControl> ();
      firstControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      firstControlStub.Stub (stub => stub.HasValue).Return (false);
      firstControlStub.Property = firstPropertyStub;
      referenceDataSource.Register (firstControlStub);

      var secondControlStub = MockRepository.GenerateStub<IBusinessObjectBoundControl> ();
      secondControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      secondControlStub.Stub (stub => stub.HasValue).Return (false);
      secondControlStub.Property = secondPropertyStub;
      referenceDataSource.Register (secondControlStub);

      var thirdControlStub = MockRepository.GenerateStub<IBusinessObjectBoundControl> ();
      thirdControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      thirdControlStub.Stub (stub => stub.HasValue).Return (false);
      thirdControlStub.Property = firstPropertyStub;
      referenceDataSource.Register (thirdControlStub);

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      _referencePropertyStub.AssertWasCalled (stub => stub.Delete (_referencedDataSourceStub.BusinessObject, referencedObject));
      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void IsDefaultValue_RequiresAllBoundControlsEmpty_ContainsNonEmtpyControls_AndReturnsTrue ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.SupportsDelete).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      var firstControlStub = MockRepository.GenerateStub<IBusinessObjectBoundControl> ();
      firstControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      firstControlStub.Stub (stub => stub.HasValue).Return (false);
      referenceDataSource.Register (firstControlStub);

      var secondControlStub = MockRepository.GenerateStub<IBusinessObjectBoundControl> ();
      secondControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      secondControlStub.Stub (stub => stub.HasValue).Return (true);
      referenceDataSource.Register (secondControlStub);

      var thirdControlStub = MockRepository.GenerateStub<IBusinessObjectBoundControl> ();
      thirdControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      thirdControlStub.Stub (stub => stub.HasValue).Return (false);
      referenceDataSource.Register (thirdControlStub);

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.IsDefaultValue (null, null, null), options => options.IgnoreArguments());
      _referencePropertyStub.AssertWasNotCalled (stub => stub.Delete (null, null), options => options.IgnoreArguments ());
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (referencedObject));
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void BusinessObjectNull_IgnoresDefaultValueSemantics_AndReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = null;

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.SupportsDefaultValue);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.SupportsDelete);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.IsDefaultValue (null, null, null), options => options.IgnoreArguments ());
      _referencePropertyStub.AssertWasNotCalled (stub => stub.Delete (null, null), options => options.IgnoreArguments ());
      _referencedDataSourceStub.BusinessObject.AssertWasCalled (stub => stub.SetProperty (_referencePropertyStub, null));
    }

    [Test]
    public void SupportsDefaultValueFalse_IsDefaultValueNotCalled ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (false);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      referenceDataSource.SaveValue (false);

      _referencePropertyStub.AssertWasNotCalled (stub => stub.IsDefaultValue (null, null, null), options => options.IgnoreArguments ());
    }

    [Test]
    public void InterimSave_DoesNotUseDefaultValueSemantics_AndReturnsTrue ()
    {
      var expectedValue = MockRepository.GenerateStub<IBusinessObject> ();
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = expectedValue;

      var firstControlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl> ();
      firstControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (firstControlMock);

      firstControlMock.Expect (mock => mock.SaveValue (true)).Return (true);

      var result = referenceDataSource.SaveValue (true);

      Assert.That (result, Is.True);
      firstControlMock.VerifyAllExpectations ();
      _referencePropertyStub.AssertWasNotCalled (stub => stub.SupportsDefaultValue);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.SupportsDelete);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.IsDefaultValue (null, null, null), options => options.IgnoreArguments ());
      _referencePropertyStub.AssertWasNotCalled (stub => stub.Delete (null, null), options => options.IgnoreArguments ());
      _referencedDataSourceStub.BusinessObject.AssertWasCalled (stub => stub.SetProperty (_referencePropertyStub, expectedValue));
    }

    [Test]
    public void PropertyIsReadWrite_SavesValueIntoBoundObject_AndReturnsTrue ()
    {
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (expectedValue);
      _referencePropertyStub.ReferenceClass.Stub (stub => stub.RequiresWriteBack).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.LoadValue (false);
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      _referencedDataSourceStub.BusinessObject.AssertWasCalled (stub => stub.SetProperty (_referencePropertyStub, expectedValue));
    }

    [Test]
    public void PropertyIsReadOnly_ThrowsInvalidOperationException ()
    {
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_readOnlyReferencePropertyStub)).Return (expectedValue);
      _readOnlyReferencePropertyStub.ReferenceClass.Stub (stub => stub.RequiresWriteBack).Return (true);
      _readOnlyReferencePropertyStub.Stub (stub => stub.Identifier).Return ("TestProperty");

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _readOnlyReferencePropertyStub);
      referenceDataSource.ID = "TestDataSource";
      referenceDataSource.LoadValue (false);
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);

      Assert.That (
          () => referenceDataSource.SaveValue (false),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "The business object of the TestDataSource could not be saved into the domain model "
              + "because the property 'TestProperty' is read only."));

      _referencedDataSourceStub.BusinessObject.AssertWasNotCalled (stub => stub.SetProperty (null, null), options=>options.IgnoreArguments());
    }

    [Test]
    public void PropertyIsReadOnly_DoesNotSaveValueIntoBoundObject ()
    {
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_readOnlyReferencePropertyStub)).Return (expectedValue);
      _readOnlyReferencePropertyStub.ReferenceClass.Stub (stub => stub.RequiresWriteBack).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _readOnlyReferencePropertyStub);
      referenceDataSource.LoadValue (false);
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);

      Assert.That (() => referenceDataSource.SaveValue (false), Throws.Exception);

      _referencedDataSourceStub.BusinessObject.AssertWasNotCalled (stub => stub.SetProperty (null, null), options=>options.IgnoreArguments());
    }

    [Test]
    public void PropertyIsReadWrite_IsDefaultValue_ClearsBusinessObject_AndReturnsTrue ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.IsDefaultValue (null, null, null)).IgnoreArguments ().Return (true);
      _referencePropertyStub.Stub (stub => stub.SupportsDelete).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.Null);
    }

    [Test]
    public void PropertyIsReadOnly_IsDefaultValue_ThrowsInvalidOperationException ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _readOnlyReferencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _readOnlyReferencePropertyStub.Stub (stub => stub.IsDefaultValue (null, null, null)).IgnoreArguments ().Return (true);
      _readOnlyReferencePropertyStub.Stub (stub => stub.SupportsDelete).Return (true);
      _readOnlyReferencePropertyStub.Stub (stub => stub.Identifier).Return ("TestProperty");
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_readOnlyReferencePropertyStub)).Return (referencedObject);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _readOnlyReferencePropertyStub);
      referenceDataSource.ID = "TestDataSource";
      referenceDataSource.LoadValue (false);

      Assert.That (
          () => referenceDataSource.SaveValue (false),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "The TestableBusinessObjectReferenceDataSource 'TestDataSource' could not be marked as changed "
              + "because the bound property 'TestProperty' is read only."));

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
    }

    [Test]
    public void PropertyIsReadWrite_SavesValuesForBoundControls_AndReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);

      var controlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl>();
      controlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (controlMock);

      controlMock.Expect (mock => mock.SaveValue (false)).Return (true);

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      controlMock.VerifyAllExpectations();
    }

    [Test]
    public void PropertyIsReadOnly_SavesValuesForBoundControls_AndReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _readOnlyReferencePropertyStub);

      var controlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl>();
      controlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (controlMock);

      controlMock.Expect (mock => mock.SaveValue (false)).Return (true);

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      controlMock.VerifyAllExpectations();
    }

    [Test]
    public void DoesNotRequireWriteBack_AllControlsCanSave_ReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      _referencedDataSourceStub.BusinessObjectClass.Stub (_ => _.RequiresWriteBack).Return (false);
      _referencePropertyStub.Stub (_ => _.IsReadOnly (Arg<IBusinessObject>.Is.Anything)).Return (true); // IsReadOnlyInDomain to ensure exit branch

      var controlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl> ();
      controlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (controlMock);

      controlMock.Expect (mock => mock.SaveValue (false)).Return (true);

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
      controlMock.VerifyAllExpectations ();
    }

    [Test]
    public void DoesNotRequireWriteBack_WithoutControls_ReturnsTrue ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      _referencedDataSourceStub.BusinessObjectClass.Stub (_ => _.RequiresWriteBack).Return (false);
      _referencePropertyStub.Stub (_ => _.IsReadOnly (Arg<IBusinessObject>.Is.Anything)).Return (true); // IsReadOnlyInDomain to ensure exit branch

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.True);
    }

    [Test]
    public void DoesNotRequireWriteBack_NotAllControlsCanSave_ReturnsFalse ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      _referencedDataSourceStub.BusinessObjectClass.Stub (_ => _.RequiresWriteBack).Return (false);
      _referencePropertyStub.Stub (_ => _.IsReadOnly (Arg<IBusinessObject>.Is.Anything)).Return (true); // IsReadOnlyInDomain to ensure exit branch

      var controlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl> ();
      controlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (controlMock);

      controlMock.Expect (mock => mock.SaveValue (false)).Return (false);

      var result = referenceDataSource.SaveValue (false);

      Assert.That (result, Is.False);
      controlMock.VerifyAllExpectations ();
    }
  }
#pragma warning restore 612,618
}