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
using Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BusinessObjectBoundEditableWebControlTests.TestDomain;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BusinessObjectBoundEditableWebControlTests
{
  [TestFixture]
  public class SaveValueToDomainModel
  {
    private TestableBusinessObjectBoundEditableWebControl _control;
    private Mock<IBusinessObjectClass> _businessObjectClassStub;
    private Mock<IBusinessObjectProperty> _propertyStub;
    private Mock<IBusinessObjectProperty> _readOnlyPropertyStub;
    private Mock<IBusinessObjectDataSource> _dataSourceStub;
    private Mock<IBusinessObject> _businessObjectStub;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectStub = new Mock<IBusinessObject>();
      _businessObjectClassStub = new Mock<IBusinessObjectClass>();
      _dataSourceStub = new Mock<IBusinessObjectDataSource>();
      _dataSourceStub.SetupProperty(_ => _.BusinessObject);
      _dataSourceStub.SetupProperty(_ => _.Mode);
      _dataSourceStub.Object.BusinessObject = _businessObjectStub.Object;
      _dataSourceStub.Object.Mode = DataSourceMode.Edit;
      _dataSourceStub.Setup(_ => _.BusinessObjectClass).Returns(_businessObjectClassStub.Object);
      _propertyStub = new Mock<IBusinessObjectProperty>();
      _propertyStub.Setup(_ => _.ReflectedClass).Returns(_businessObjectClassStub.Object);
      _readOnlyPropertyStub = new Mock<IBusinessObjectProperty>();
      _readOnlyPropertyStub.Setup(_ => _.ReflectedClass).Returns(_businessObjectClassStub.Object);
      _readOnlyPropertyStub.Setup(stub => stub.IsReadOnly(It.IsAny<IBusinessObject>())).Returns(true);
      _control = new TestableBusinessObjectBoundEditableWebControl();
    }

    [Test]
    public void SaveValue ()
    {
      _control.DataSource = _dataSourceStub.Object;
      _control.Property = _propertyStub.Object;
      _control.Value = "test";

      Assert.That(_control.SaveValueToDomainModel(), Is.True);
      Mock.Get(_dataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(_propertyStub.Object, "test"), Times.AtLeastOnce());
    }

    [Test]
    public void SaveValueAndDataSourceNull ()
    {
      _control.DataSource = null;
      _control.Property = _propertyStub.Object;
      _control.Value = null;

      Assert.That(_control.SaveValueToDomainModel(), Is.False);
      Mock.Get(_dataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(It.IsAny<IBusinessObjectProperty>(), It.IsAny<object>()), Times.Never());
    }

    [Test]
    public void SaveValueAndPropertyNull ()
    {
      _control.DataSource = _dataSourceStub.Object;
      _control.Property = null;
      _control.Value = null;

      Assert.That(_control.SaveValueToDomainModel(), Is.False);
      Mock.Get(_dataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(It.IsAny<IBusinessObjectProperty>(), It.IsAny<object>()), Times.Never());
    }

    [Test]
    public void SaveValueAndBusinessObjectNullAndValueNotNull ()
    {
      _dataSourceStub.Object.BusinessObject = null;
      _control.DataSource = _dataSourceStub.Object;
      _control.Property = _propertyStub.Object;
      _control.Value = "value";

      Assert.That(_control.SaveValueToDomainModel(), Is.False);
      _businessObjectStub.Verify(stub => stub.SetProperty(It.IsAny<IBusinessObjectProperty>(), It.IsAny<object>()), Times.Never());
    }

    [Test]
    public void SaveValueAndBusinessObjectNullAndValueNull ()
    {
      _dataSourceStub.Object.BusinessObject = null;
      _control.DataSource = _dataSourceStub.Object;
      _control.Property = _propertyStub.Object;
      _control.Value = null;

      Assert.That(_control.SaveValueToDomainModel(), Is.True);
      _businessObjectStub.Verify(stub => stub.SetProperty(It.IsAny<IBusinessObjectProperty>(), It.IsAny<object>()), Times.Never());
    }

    [Test]
    public void SaveValueAndControlIsReadOnly ()
    {
      _control.DataSource = _dataSourceStub.Object;
      _control.Property = _propertyStub.Object;
      _control.Value = "test";
      _control.ReadOnly = true;

      Assert.That(_control.SaveValueToDomainModel(), Is.True);
      Mock.Get(_dataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(_propertyStub.Object, "test"), Times.AtLeastOnce());
    }

    [Test]
    public void SaveValueAndPropertyIsReadOnlyInDomainLayer ()
    {
      _control.ID = "TestID";
      _readOnlyPropertyStub.Setup(stub => stub.Identifier).Returns("TestProperty");
      _control.DataSource = _dataSourceStub.Object;
      _control.Property = _readOnlyPropertyStub.Object;
      _control.Value = null;

      Assert.That(
          () => _control.SaveValueToDomainModel(),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "The value of the TestableBusinessObjectBoundEditableWebControl 'TestID' could not be saved into the domain model "
              + "because the property 'TestProperty' is read only."));
      Mock.Get(_dataSourceStub.Object.BusinessObject).Verify(stub => stub.SetProperty(It.IsAny<IBusinessObjectProperty>(), It.IsAny<object>()), Times.Never());
    }
  }
}
