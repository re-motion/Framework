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
  public class LoadValue
  {
    private Mock<IBusinessObjectReferenceProperty> _referencePropertyStub;
    private Mock<IBusinessObjectDataSource> _referencedDataSourceStub;

    [SetUp]
    public void SetUp ()
    {
      _referencedDataSourceStub = new Mock<IBusinessObjectDataSource>();
      _referencedDataSourceStub.SetupProperty (_ => _.BusinessObject);
      _referencedDataSourceStub.Object.BusinessObject = new Mock<IBusinessObject>().Object;
      _referencedDataSourceStub.Setup (_ => _.BusinessObjectClass).Returns (new Mock<IBusinessObjectClass>().Object);
      _referencePropertyStub = new Mock<IBusinessObjectReferenceProperty>();
      _referencePropertyStub.Setup (_ => _.ReflectedClass).Returns (new Mock<IBusinessObjectClass>().Object);
    }

    [Test]
    public void LoadsValueFromBoundObject ()
    {
      var expectedValue = new Mock<IBusinessObject>();
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).Setup (stub => stub.GetProperty (_referencePropertyStub.Object)).Returns (expectedValue.Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = null;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue.Object));
    }

    [Test]
    public void ClearsHasBusinessObjectChanged ()
    {
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).Setup (stub => stub.GetProperty (_referencePropertyStub.Object))
          .Returns (new Mock<IBusinessObject>().Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = null;

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_SetsBusinessObjectNull ()
    {
      _referencedDataSourceStub.Object.BusinessObject = null;

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = new Mock<IBusinessObject>().Object;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_DoesNotLoadValueFromProperty ()
    {
      var parentObjectStub = _referencedDataSourceStub.Object.BusinessObject;
      _referencedDataSourceStub.Object.BusinessObject = null;

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = new Mock<IBusinessObject>().Object;

      referenceDataSource.LoadValue (false);

      Mock.Get (parentObjectStub).Verify (stub => stub.GetProperty (It.IsAny<IBusinessObjectProperty>()), Times.Never());
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_ClearsHasBusinessObjectChanged ()
    {
      _referencedDataSourceStub.Object.BusinessObject = null;

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = new Mock<IBusinessObject>().Object;

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_ClearsHasBusinessObjectCreated ()
    {
      _referencePropertyStub.Setup (stub => stub.SupportsDefaultValue).Returns (true);
      _referencePropertyStub.Setup (stub => stub.CreateDefaultValue (It.IsAny<IBusinessObject>())).Returns (new Mock<IBusinessObject>().Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Not.Null);
      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);

      _referencedDataSourceStub.Object.BusinessObject = null;
      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.False);
    }

    [Test]
    public void ReferencedDataSourceBusinessObjectNull_DoesNotCreateDefaultValue ()
    {
      _referencedDataSourceStub.Object.BusinessObject = null;

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.BusinessObject = null;

      referenceDataSource.LoadValue (false);

      _referencePropertyStub.Verify (stub => stub.SupportsDefaultValue, Times.Never());
      _referencePropertyStub.Verify (stub => stub.CreateDefaultValue (It.IsAny<IBusinessObject>()), Times.Never());
    }

    [Test]
    public void ReferencedDataSourceNull_DoesNotSetBusinessObject_DoesNotClearHasBusinessObjectChanged ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (null, _referencePropertyStub.Object);
      var expectedValue = new Mock<IBusinessObject>();
      referenceDataSource.BusinessObject = expectedValue.Object;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue.Object));
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);
    }

    [Test]
    public void ReferencedPropertyNull_DoesNotSetBusinessObject_DoesNotClearHasBusinessObjectChanged ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, null);
      var expectedValue = new Mock<IBusinessObject>();
      referenceDataSource.BusinessObject = expectedValue.Object;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue.Object));
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);
    }

    [Test]
    public void LoadsValuesForBoundControls ()
    {
      var referencedObject = new Mock<IBusinessObject>();
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).Setup (stub => stub.GetProperty (_referencePropertyStub.Object)).Returns (referencedObject.Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);

      var firstControlMock = new Mock<IBusinessObjectBoundControl>();
      firstControlMock.Setup (stub => stub.HasValidBinding).Returns (true);
      referenceDataSource.Register (firstControlMock.Object);

      var secondControlMock = new Mock<IBusinessObjectBoundControl>();
      secondControlMock.Setup (stub => stub.HasValidBinding).Returns (true);
      referenceDataSource.Register (secondControlMock.Object);

      firstControlMock.Setup(mock => mock.LoadValue (false))
          .Callback ((bool interim) =>Assert.That (referenceDataSource.BusinessObject, Is.SameAs (referencedObject.Object)))
          .Verifiable();
      secondControlMock.Setup(mock => mock.LoadValue (false))
          .Callback ((bool interim) =>Assert.That (referenceDataSource.BusinessObject, Is.SameAs (referencedObject.Object)))
          .Verifiable();

      referenceDataSource.LoadValue (false);

      firstControlMock.Verify();
      secondControlMock.Verify();
    }

    [Test]
    public void SetsDefaultValue ()
    {
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).Setup (stub => stub.GetProperty (_referencePropertyStub.Object)).Returns ((object) null);
      _referencePropertyStub.Setup (stub => stub.SupportsDefaultValue).Returns (true);
      var expectedValue = new Mock<IBusinessObject>();
      _referencePropertyStub.Setup (stub => stub.CreateDefaultValue (_referencedDataSourceStub.Object.BusinessObject)).Returns (expectedValue.Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue.Object));
    }

    [Test]
    public void SetsDefaultValue_ClearsHasBusinessObjectChanged ()
    {
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).Setup (stub => stub.GetProperty (_referencePropertyStub.Object)).Returns ((object) null);
      _referencePropertyStub.Setup (stub => stub.SupportsDefaultValue).Returns (true);
      var expectedValue = new Mock<IBusinessObject>();
      _referencePropertyStub.Setup (stub => stub.CreateDefaultValue (_referencedDataSourceStub.Object.BusinessObject)).Returns (expectedValue.Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void SetsDefaultValue_SetHasBusinessObjectCreated ()
    {
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).Setup (stub => stub.GetProperty (_referencePropertyStub.Object)).Returns ((object) null);
      _referencePropertyStub.Setup (stub => stub.SupportsDefaultValue).Returns (true);
      var expectedValue = new Mock<IBusinessObject>();
      _referencePropertyStub.Setup (stub => stub.CreateDefaultValue (_referencedDataSourceStub.Object.BusinessObject)).Returns (expectedValue.Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
    }

    [Test]
    public void DataSourceModeRead_DoesNotSetDefaultValue ()
    {
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).Setup (stub => stub.GetProperty (_referencePropertyStub.Object)).Returns ((object) null);
      _referencePropertyStub.Setup (stub => stub.SupportsDefaultValue).Returns (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Read;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      _referencePropertyStub.Verify (stub => stub.CreateDefaultValue (It.IsAny<IBusinessObject>()), Times.Never());
    }

    [Test]
    public void DataSourceModeSearch_DoesNotSetDefaultValue ()
    {
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).Setup (stub => stub.GetProperty (_referencePropertyStub.Object)).Returns ((object) null);
      _referencePropertyStub.Setup (stub => stub.SupportsDefaultValue).Returns (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Search;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      _referencePropertyStub.Verify (stub => stub.CreateDefaultValue (It.IsAny<IBusinessObject>()), Times.Never());
    }

    [Test]
    public void SupportsDefaultValueFalse_DoesNotSetDefaultValue ()
    {
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).Setup (stub => stub.GetProperty (_referencePropertyStub.Object)).Returns ((object) null);
      _referencePropertyStub.Setup (stub => stub.SupportsDefaultValue).Returns (false);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      _referencePropertyStub.Verify (stub => stub.CreateDefaultValue (It.IsAny<IBusinessObject>()), Times.Never());
    }

    [Test]
    public void HasValueFromProperty_DoesNotSetDefaultValue ()
    {
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).Setup (stub => stub.GetProperty (_referencePropertyStub.Object))
          .Returns (new Mock<IBusinessObject>().Object);
      _referencePropertyStub.Setup (stub => stub.SupportsDefaultValue).Returns (true);
      _referencePropertyStub.Setup (stub => stub.CreateDefaultValue (_referencedDataSourceStub.Object.BusinessObject))
          .Returns (new Mock<IBusinessObject>().Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Not.Null);
    }

    [Test]
    public void LoadValueCalledAgainWithInterimTrue_HasBusinessObjectCreatedTrue_LeavesBusinessObjectUntouched ()
    {
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).SetupSequence (stub => stub.GetProperty (_referencePropertyStub.Object))
          .Returns ((object) null)
          .Throws (new InvalidOperationException ("Method is supposed to be called only once"));
      _referencePropertyStub.SetupSequence (stub => stub.SupportsDefaultValue)
          .Returns (true)
          .Throws (new InvalidOperationException ("Method is supposed to be called only once"));
      _referencePropertyStub.Setup (stub => stub.CreateDefaultValue (_referencedDataSourceStub.Object.BusinessObject))
          .Returns (new Mock<IBusinessObject>().Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
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
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).SetupSequence (stub => stub.GetProperty (_referencePropertyStub.Object)).Returns ((object) null);
      _referencePropertyStub.Setup (stub => stub.SupportsDefaultValue).Returns (true);
      var oldValue = new Mock<IBusinessObject>();
      _referencePropertyStub.Setup (stub => stub.CreateDefaultValue (_referencedDataSourceStub.Object.BusinessObject))
          .Returns (oldValue.Object);
      var newValue = new Mock<IBusinessObject>();
      _referencePropertyStub.SetupSequence (stub => stub.CreateDefaultValue (_referencedDataSourceStub.Object.BusinessObject))
          .Returns (oldValue.Object)
          .Returns (newValue.Object);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (oldValue.Object));

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.Not.Null);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (newValue.Object));
    }

    [Test]
    public void LoadValueCalledAgainWithInterimFalse_HasBusinessObjectCreatedTrue_SupportsDelete_DeletesOldValue ()
    {
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).SetupSequence (stub => stub.GetProperty (_referencePropertyStub.Object)).Returns ((object) null);
      _referencePropertyStub.Setup (stub => stub.SupportsDefaultValue).Returns (true);
      _referencePropertyStub.Setup (stub => stub.SupportsDelete).Returns (true);
      var oldValue = new Mock<IBusinessObject>();
      var newValue = new Mock<IBusinessObject>();
      _referencePropertyStub.SetupSequence (stub => stub.CreateDefaultValue (_referencedDataSourceStub.Object.BusinessObject))
          .Returns (oldValue.Object)
          .Returns (newValue.Object)
          .Throws (new InvalidOperationException ("Method is not supposed to be called more than two times."));

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (oldValue.Object));
      _referencePropertyStub.Verify (stub => stub.Delete (_referencedDataSourceStub.Object.BusinessObject, oldValue.Object), Times.Never);

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (newValue.Object));
      _referencePropertyStub.Verify (stub => stub.Delete (_referencedDataSourceStub.Object.BusinessObject, oldValue.Object), Times.Once);
    }

    [Test]
    public void LoadValueCalledAgainWithInterimFalse_HasBusinessObjectCreatedTrue_DoesNotSupportDelete_DoesNotDeleteOldValue ()
    {
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).SetupSequence (stub => stub.GetProperty (_referencePropertyStub.Object)).Returns ((object) null);
      _referencePropertyStub.Setup (stub => stub.SupportsDefaultValue).Returns (true);
      _referencePropertyStub.Setup (stub => stub.SupportsDelete).Returns (false);
      var oldValue = new Mock<IBusinessObject>();
      var newValue = new Mock<IBusinessObject>();
      _referencePropertyStub.SetupSequence (stub => stub.CreateDefaultValue (_referencedDataSourceStub.Object.BusinessObject))
          .Returns (oldValue.Object)
          .Returns (newValue.Object)
          .Throws (new InvalidOperationException ("Method is not supposed to be called more than two times."));

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (oldValue.Object));
      _referencePropertyStub.Verify (stub => stub.Delete (_referencedDataSourceStub.Object.BusinessObject, oldValue.Object), Times.Never);

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (newValue.Object));
      _referencePropertyStub.Verify (stub => stub.Delete (_referencedDataSourceStub.Object.BusinessObject, oldValue.Object), Times.Never);
    }

    [Test]
    public void LoadValueCalledAgainWithInterimFalse_HasBusinessObjectCreatedTrue_BoundPropertyContainsValue_DoesNotCreateDefaultValue ()
    {
      _referencePropertyStub.SetupSequence (stub => stub.SupportsDefaultValue)
          .Returns (true)
          .Throws (new InvalidOperationException ("Method is supposed to be called only once"));
      _referencePropertyStub.Setup (stub => stub.SupportsDefaultValue).Returns (true);
      var oldValue = new Mock<IBusinessObject>();
      _referencePropertyStub.SetupSequence (stub => stub.CreateDefaultValue (_referencedDataSourceStub.Object.BusinessObject))
          .Returns (oldValue.Object)
          .Throws (new InvalidOperationException ("Method is supposed to be called only once"));

      var expectedValue = new Mock<IBusinessObject>();
      Mock.Get (_referencedDataSourceStub.Object.BusinessObject).SetupSequence (stub => stub.GetProperty (_referencePropertyStub.Object))
          .Returns ((object) null)
          .Returns (expectedValue.Object)
          .Throws (new InvalidOperationException ("Method is not supposed to be called more than two times."));

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub.Object, _referencePropertyStub.Object);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.True);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (oldValue.Object));

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectCreated, Is.False);
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue.Object));
    }
  }
#pragma warning restore 612,618
}