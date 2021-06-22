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
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class BusinessObjectValidationResultExtensionsTest
  {
    [Test]
    public void GetValidationFailures_HasValidBindingFalse_ReturnsEmpty ()
    {
      var controlMock = new Mock<IBusinessObjectBoundEditableWebControl>();
      controlMock.Setup (_ => _.HasValidBinding).Returns (false).Verifiable();

      var validationResultStub = new Mock<IBusinessObjectValidationResult>();
      validationResultStub
          .Setup (_ => _.GetValidationFailures (It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty>(), It.IsAny<bool>()))
          .Throws (new InvalidOperationException());

      var validationFailures = BusinessObjectValidationResultExtensions.GetValidationFailures (validationResultStub.Object, controlMock.Object);

      Assert.That (validationFailures, Is.Empty);
      controlMock.Verify();
    }

    [Test]
    public void GetValidationFailures_WithDataSourceNull_ReturnsEmpty()
    {
      var controlMock = new Mock<IBusinessObjectBoundEditableWebControl>();
      controlMock.Setup (_ => _.HasValidBinding).Returns (true).Verifiable();
      controlMock.Setup (_ => _.DataSource).Returns ((IBusinessObjectDataSource) null).Verifiable();

      var validationResultStub = new Mock<IBusinessObjectValidationResult>();
      validationResultStub
          .Setup (_ => _.GetValidationFailures (It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty>(), It.IsAny<bool>()))
          .Throws (new InvalidOperationException());

      var validationFailures = BusinessObjectValidationResultExtensions.GetValidationFailures (validationResultStub.Object, controlMock.Object);

      Assert.That (validationFailures, Is.Empty);
      controlMock.Verify();
    }

    [Test]
    public void GetValidationFailures_WithPropertyNull_ReturnsEmpty()
    {
      var businessObjectStub = new Mock<IBusinessObject>();

      var dataSourceStub = new Mock<IBusinessObjectDataSource>();
      dataSourceStub.SetupProperty (_ => _.BusinessObject);
      dataSourceStub.Object.BusinessObject = businessObjectStub.Object;

      var controlMock = new Mock<IBusinessObjectBoundEditableWebControl>();
      controlMock.Setup (_ => _.HasValidBinding).Returns (true).Verifiable();
      controlMock.Setup (_ => _.DataSource).Returns (dataSourceStub.Object).Verifiable();
      controlMock.Setup (_ => _.Property).Returns ((IBusinessObjectProperty) null).Verifiable();

      var validationResultStub = new Mock<IBusinessObjectValidationResult>();
      validationResultStub
          .Setup (_ => _.GetValidationFailures (It.IsAny<IBusinessObject>(), It.IsAny<IBusinessObjectProperty>(), It.IsAny<bool>()))
          .Throws (new InvalidOperationException());

      var validationFailures = BusinessObjectValidationResultExtensions.GetValidationFailures (validationResultStub.Object, controlMock.Object);

      Assert.That (validationFailures, Is.Empty);
      controlMock.Verify();
    }

    [Test]
    public void GetValidationFailures_WithValidObjectAndProperty_ReturnsValidationFailures ()
    {
      var businessObjectStub = new Mock<IBusinessObject>();
      var businessObjectPropertyStub = new Mock<IBusinessObjectProperty>();

      var dataSourceStub = new Mock<IBusinessObjectDataSource>();
      dataSourceStub.SetupProperty (_ => _.BusinessObject);
      dataSourceStub.Object.BusinessObject = businessObjectStub.Object;

      var controlStub = new Mock<IBusinessObjectBoundEditableWebControl>();
      controlStub.Setup (_ => _.HasValidBinding).Returns (true);
      controlStub.SetupProperty (_ => _.DataSource);
      controlStub.SetupProperty (_ => _.Property);
      controlStub.Object.DataSource = dataSourceStub.Object;
      controlStub.Object.Property = businessObjectPropertyStub.Object;

      var businessObjectValidationFailure1 = BusinessObjectValidationFailure.Create ("Error 1");
      var businessObjectValidationFailure2 = BusinessObjectValidationFailure.Create ("Error 2");
      var validationResultStub = new Mock<IBusinessObjectValidationResult>();
      validationResultStub
          .Setup (_ => _.GetValidationFailures (businessObjectStub.Object, businessObjectPropertyStub.Object, true))
          .Returns (new[] {businessObjectValidationFailure1, businessObjectValidationFailure2});

      var validationFailures = BusinessObjectValidationResultExtensions.GetValidationFailures (validationResultStub.Object, controlStub.Object);

      Assert.That (validationFailures, Is.EquivalentTo (new[] {businessObjectValidationFailure1, businessObjectValidationFailure2}));
    }
  }
}