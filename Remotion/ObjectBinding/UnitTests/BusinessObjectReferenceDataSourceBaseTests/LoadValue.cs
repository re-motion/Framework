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
  public class LoadValue
  {
    private IBusinessObjectReferenceProperty _referencePropertyStub;
    private IBusinessObjectDataSource _referencedDataSourceStub;

    [SetUp]
    public void SetUp ()
    {
      _referencedDataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      _referencedDataSourceStub.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();
      _referencedDataSourceStub.Stub (_ => _.BusinessObjectClass).Return (MockRepository.GenerateStub<IBusinessObjectClass>());
      _referencePropertyStub = MockRepository.GenerateStub<IBusinessObjectReferenceProperty> ();
      _referencePropertyStub.Stub (_ => _.ReflectedClass).Return (MockRepository.GenerateStub<IBusinessObjectClass>());
    }

    [Test]
    public void LoadsValueFromBoundObject ()
    {
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (expectedValue);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = null;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue));
    }

    [Test]
    public void ClearsHasBusinessObjectChanged ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub))
          .Return (MockRepository.GenerateStub<IBusinessObject>());

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = null;

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_SetsBusinessObjectNull ()
    {
      _referencedDataSourceStub.BusinessObject = null;

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_DoesNotLoadValueFromProperty ()
    {
      var parentObjectStub = _referencedDataSourceStub.BusinessObject;
      _referencedDataSourceStub.BusinessObject = null;

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = MockRepository.GenerateStub<IBusinessObject> ();

      referenceDataSource.LoadValue (false);

      parentObjectStub.AssertWasNotCalled (stub => stub.GetProperty (null), options => options.IgnoreArguments ());
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_ClearsHasBusinessObjectChanged ()
    {
      _referencedDataSourceStub.BusinessObject = null;

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = MockRepository.GenerateStub<IBusinessObject> ();

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_ClearsHasBusinessObjectCreated ()
    {
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (null)).IgnoreArguments().Return (MockRepository.GenerateStub<IBusinessObject>());

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Not.Null);
      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);

      _referencedDataSourceStub.BusinessObject = null;
      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.False);
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_DoesNotCreateDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject = null;

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = null;

      referenceDataSource.LoadValue (false);

      _referencePropertyStub.AssertWasNotCalled (stub => stub.SupportsDefaultValue);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.CreateDefaultValue (null), options=>options.IgnoreArguments ());
    }

    [Test]
    public void ReferencedDataSourceNull_DoesNotSetBusinessObject_DoesNotClearHasBusinessObjectChanged ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (null, _referencePropertyStub);
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      referenceDataSource.BusinessObject = expectedValue;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue));
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);
    }

    [Test]
    public void ReferencedPropertyNull_DoesNotSetBusinessObject_DoesNotClearHasBusinessObjectChanged ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, null);
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      referenceDataSource.BusinessObject = expectedValue;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue));
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);
    }

    [Test]
    public void LoadsValuesForBoundControls ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject>();
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (referencedObject);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);

      var firstControlMock = MockRepository.GenerateMock<IBusinessObjectBoundControl>();
      firstControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (firstControlMock);

      var secondControlMock = MockRepository.GenerateMock<IBusinessObjectBoundControl>();
      secondControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (secondControlMock);

      firstControlMock.Expect(mock => mock.LoadValue (false))
          .WhenCalled (mi=>Assert.That (referenceDataSource.BusinessObject, Is.SameAs (referencedObject)));
      secondControlMock.Expect(mock => mock.LoadValue (false))
          .WhenCalled (mi=>Assert.That (referenceDataSource.BusinessObject, Is.SameAs (referencedObject)));

      referenceDataSource.LoadValue (false);

      firstControlMock.VerifyAllExpectations();
      secondControlMock.VerifyAllExpectations();
    }

    [Test]
    public void SetsDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null);
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject)).Return (expectedValue);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue));
    }

    [Test]
    public void SetsDefaultValue_ClearsHasBusinessObjectChanged ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null);
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      var expectedValue = MockRepository.GenerateStub<IBusinessObject> ();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject)).Return (expectedValue);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void SetsDefaultValue_SetHasBusinessObjectCreated ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null);
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      var expectedValue = MockRepository.GenerateStub<IBusinessObject> ();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject)).Return (expectedValue);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
    }

    [Test]
    public void DataSourceModeRead_DoesNotSetDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null);
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Read;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.CreateDefaultValue (Arg<IBusinessObject>.Is.Anything));
    }

    [Test]
    public void DataSourceModeSearch_DoesNotSetDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null);
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Search;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.CreateDefaultValue (Arg<IBusinessObject>.Is.Anything));
    }

    [Test]
    public void SupportsDefaultValueFalse_DoesNotSetDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null);
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (false);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.CreateDefaultValue (Arg<IBusinessObject>.Is.Anything));
    }

    [Test]
    public void HasValueFromProperty_DoesNotSetDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub))
          .Return (MockRepository.GenerateStub<IBusinessObject>());
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject))
          .Return (MockRepository.GenerateStub<IBusinessObject> ());

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Not.Null);
    }

    [Test]
    public void LoadValueCalledAgainWithInterimTrue_HasBusinessObjectCreatedTrue_LeavesBusinessObjectUntouched ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null).Repeat.Once();
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true).Repeat.Once();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject))
          .Return (MockRepository.GenerateStub<IBusinessObject> ())
          .Repeat.Once();

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      var oldValue = referenceDataSource.BusinessObject;

      referenceDataSource.LoadValue (true);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (oldValue));
    }

    [Test]
    public void LoadValueCalledAgainWithInterimFalse_HasBusinessObjectCreatedTrue_CreatesNewDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null).Repeat.Once();
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      var oldValue = MockRepository.GenerateStub<IBusinessObject>();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject))
          .Return (oldValue)
          .Repeat.Once();
      var newValue = MockRepository.GenerateStub<IBusinessObject>();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject))
          .Return (newValue)
          .Repeat.Once();


      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (oldValue));

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.Not.Null);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (newValue));
    }

    [Test]
    public void LoadValueCalledAgainWithInterimFalse_HasBusinessObjectCreatedTrue_SupportsDelete_DeletesOldValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null).Repeat.Once ();
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.SupportsDelete).Return (true);
      var oldValue = MockRepository.GenerateStub<IBusinessObject> ();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject))
          .Return (oldValue)
          .Repeat.Once ();
      var newValue = MockRepository.GenerateStub<IBusinessObject> ();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject))
          .Return (newValue)
          .Repeat.Once ();

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (oldValue));
      _referencePropertyStub.AssertWasNotCalled (stub => stub.Delete (_referencedDataSourceStub.BusinessObject, oldValue));

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (newValue));
      _referencePropertyStub.AssertWasCalled (stub => stub.Delete (_referencedDataSourceStub.BusinessObject, oldValue));
    }

    [Test]
    public void LoadValueCalledAgainWithInterimFalse_HasBusinessObjectCreatedTrue_DoesNotSupportDelete_DoesNotDeleteOldValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null).Repeat.Once ();
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.SupportsDelete).Return (false);
      var oldValue = MockRepository.GenerateStub<IBusinessObject> ();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject))
          .Return (oldValue)
          .Repeat.Once ();
      var newValue = MockRepository.GenerateStub<IBusinessObject> ();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject))
          .Return (newValue)
          .Repeat.Once ();

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (oldValue));
      _referencePropertyStub.AssertWasNotCalled (stub => stub.Delete (_referencedDataSourceStub.BusinessObject, oldValue));

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (newValue));
      _referencePropertyStub.AssertWasNotCalled (stub => stub.Delete (_referencedDataSourceStub.BusinessObject, oldValue));
    }

    [Test]
    public void LoadValueCalledAgainWithInterimFalse_HasBusinessObjectCreatedTrue_BoundPropertyContainsValue_DoesNotCreateDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null).Repeat.Once ();
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      var oldValue = MockRepository.GenerateStub<IBusinessObject> ();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject))
          .Return (oldValue)
          .Repeat.Once ();

      var expectedValue = MockRepository.GenerateStub<IBusinessObject> ();
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (expectedValue).Repeat.Once ();

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (oldValue));

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.False);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue));
    }
  }
#pragma warning restore 612,618
}