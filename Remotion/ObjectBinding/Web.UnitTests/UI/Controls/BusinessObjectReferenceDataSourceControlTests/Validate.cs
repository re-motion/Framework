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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BusinessObjectReferenceDataSourceControlTests
{
  [TestFixture]
  public class Validate : BocTest
  {
    private Mock<IBusinessObjectReferenceProperty> _referencePropertyStub;
    private Mock<IBusinessObjectDataSource> _referencedDataSourceStub;
    private BusinessObjectReferenceDataSourceControl _dataSourceControl;

    public override void SetUp ()
    {
      base.SetUp();

      _referencedDataSourceStub = new Mock<IBusinessObjectDataSource>();
      _referencedDataSourceStub.SetupProperty(_ => _.BusinessObject);
      _referencedDataSourceStub.SetupProperty(_ => _.Mode);
      _referencedDataSourceStub.Object.BusinessObject = new Mock<IBusinessObject>().Object;
      _referencedDataSourceStub.Object.Mode = DataSourceMode.Edit;
      _referencePropertyStub = new Mock<IBusinessObjectReferenceProperty>();
      _referencePropertyStub.Setup(stub => stub.ReferenceClass).Returns(new Mock<IBusinessObjectClass>().Object);
      _referencePropertyStub.Setup(stub => stub.ReflectedClass).Returns(new Mock<IBusinessObjectClass>().Object);

      _dataSourceControl = new BusinessObjectReferenceDataSourceControl();
      _dataSourceControl.DataSource = _referencedDataSourceStub.Object;
      _dataSourceControl.Property = _referencePropertyStub.Object;
      _dataSourceControl.BusinessObject = new Mock<IBusinessObject>().Object;

      Assert.That(_dataSourceControl.IsReadOnly, Is.False);
    }

    [Test]
    public void NoBoundControls_ReturnsTrue ()
    {
      Assert.That(_dataSourceControl.Validate(), Is.True);
    }

    [Test]
    public void AllBoundControlsValid_ReturnsTrue ()
    {
      var firstControlStub = new Mock<IBusinessObjectBoundControl>();
      firstControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      firstControlStub.Setup(stub => stub.HasValue).Returns(true);
      firstControlStub.As<IValidatableControl>().Setup(stub => stub.Validate()).Returns(true);
      _dataSourceControl.Register(firstControlStub.Object);

      var secondControlStub = new Mock<IBusinessObjectBoundControl>();
      secondControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      secondControlStub.Setup(stub => stub.HasValue).Returns(true);
      secondControlStub.As<IValidatableControl>().Setup(stub => stub.Validate()).Returns(true);
      _dataSourceControl.Register(secondControlStub.Object);

      Assert.That(_dataSourceControl.Validate(), Is.True);
    }

    [Test]
    public void NotAllBoundControlsValid_ReturnsFalse ()
    {
      var firstControlStub = new Mock<IBusinessObjectBoundControl>();
      firstControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      firstControlStub.Setup(stub => stub.HasValue).Returns(true);
      firstControlStub.As<IValidatableControl>().Setup(stub => stub.Validate()).Returns(true);
      _dataSourceControl.Register(firstControlStub.Object);

      var secondControlStub = new Mock<IBusinessObjectBoundControl>();
      secondControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      secondControlStub.Setup(stub => stub.HasValue).Returns(true);
      secondControlStub.As<IValidatableControl>().Setup(stub => stub.Validate()).Returns(false);
      _dataSourceControl.Register(secondControlStub.Object);

      var thirdControlStub = new Mock<IBusinessObjectBoundControl>();
      thirdControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      thirdControlStub.Setup(stub => stub.HasValue).Returns(true);
      thirdControlStub.As<IValidatableControl>().Setup(stub => stub.Validate()).Returns(true);
      _dataSourceControl.Register(thirdControlStub.Object);

      Assert.That(_dataSourceControl.Validate(), Is.False);
    }

#pragma warning disable 612,618
    [Test]
    public void SupportsDefaultValue_True_AllBoundControlsEmpty_NotAllBoundControlsValid_ReturnsTrue ()
    {
      _referencePropertyStub.Setup(stub => stub.SupportsDefaultValue).Returns(true);
      _referencePropertyStub.Setup(stub => stub.IsDefaultValue(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty[]>())).Returns(true);

      var firstControlStub = new Mock<IBusinessObjectBoundControl>();
      firstControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      firstControlStub.Setup(stub => stub.HasValue).Returns(false);
      firstControlStub.As<IValidatableControl>().Setup(stub => stub.Validate()).Returns(true);
      _dataSourceControl.Register(firstControlStub.Object);

      var secondControlStub = new Mock<IBusinessObjectBoundControl>();
      secondControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      secondControlStub.Setup(stub => stub.HasValue).Returns(false);
      secondControlStub.As<IValidatableControl>().Setup(stub => stub.Validate()).Returns(false);
      _dataSourceControl.Register(secondControlStub.Object);

      Assert.That(_dataSourceControl.Validate(), Is.True);
    }
#pragma warning restore 612,618

#pragma warning disable 612,618
    [Test]
    public void IsRequired_True_AllBoundControlsEmpty_NotAllBoundControlsValid_ReturnsFalse ()
    {
      _dataSourceControl.Required = true;

      var firstControlStub = new Mock<IBusinessObjectBoundControl>();
      firstControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      firstControlStub.Setup(stub => stub.HasValue).Returns(false);
      firstControlStub.As<IValidatableControl>().Setup(stub => stub.Validate()).Returns(true);
      _dataSourceControl.Register(firstControlStub.Object);

      var secondControlStub = new Mock<IBusinessObjectBoundControl>();
      secondControlStub.Setup(stub => stub.HasValidBinding).Returns(true);
      secondControlStub.Setup(stub => stub.HasValue).Returns(false);
      secondControlStub.As<IValidatableControl>().Setup(stub => stub.Validate()).Returns(false);
      _dataSourceControl.Register(secondControlStub.Object);

      Assert.That(_dataSourceControl.Validate(), Is.False);

      _referencePropertyStub.Verify(stub => stub.SupportsDefaultValue, Times.Never());
      _referencePropertyStub.Verify(stub => stub.IsDefaultValue(It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty[]>()), Times.Never());
    }
#pragma warning restore 612,618
  }
}
