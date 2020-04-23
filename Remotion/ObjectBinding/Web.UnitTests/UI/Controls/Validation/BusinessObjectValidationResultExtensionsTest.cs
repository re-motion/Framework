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
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class BusinessObjectValidationResultExtensionsTest
  {
    [Test]
    public void GetValidationFailures_HasValidBindingFalse_ReturnsEmpty ()
    {
      var controlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableWebControl>();
      controlMock.Expect (_ => _.HasValidBinding).Return (false);

      var validationResultStub = MockRepository.GenerateStub<IBusinessObjectValidationResult>();
      validationResultStub
          .Stub (_ => _.GetValidationFailures (Arg<IBusinessObject>.Is.Anything, Arg<IBusinessObjectProperty>.Is.Anything, Arg<bool>.Is.Anything))
          .Throw (new InvalidOperationException());

      var validationFailures = BusinessObjectValidationResultExtensions.GetValidationFailures (validationResultStub, controlMock);

      Assert.That (validationFailures, Is.Empty);
      controlMock.VerifyAllExpectations();
    }

    [Test]
    public void GetValidationFailures_WithDataSourceNull_ReturnsEmpty()
    {
      var controlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableWebControl>();
      controlMock.Expect (_ => _.HasValidBinding).Return (true);
      controlMock.Expect (_ => _.DataSource).Return (null);

      var validationResultStub = MockRepository.GenerateStub<IBusinessObjectValidationResult>();
      validationResultStub
          .Stub (_ => _.GetValidationFailures (Arg<IBusinessObject>.Is.Anything, Arg<IBusinessObjectProperty>.Is.Anything, Arg<bool>.Is.Anything))
          .Throw (new InvalidOperationException());

      var validationFailures = BusinessObjectValidationResultExtensions.GetValidationFailures (validationResultStub, controlMock);

      Assert.That (validationFailures, Is.Empty);
      controlMock.VerifyAllExpectations();
    }

    [Test]
    public void GetValidationFailures_WithPropertyNull_ReturnsEmpty()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var dataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      dataSourceStub.BusinessObject = businessObjectStub;

      var controlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableWebControl>();
      controlMock.Expect (_ => _.HasValidBinding).Return (true);
      controlMock.Expect (_ => _.DataSource).Return (dataSourceStub);
      controlMock.Expect (_ => _.Property).Return (null);

      var validationResultStub = MockRepository.GenerateStub<IBusinessObjectValidationResult>();
      validationResultStub
          .Stub (_ => _.GetValidationFailures (Arg<IBusinessObject>.Is.Anything, Arg<IBusinessObjectProperty>.Is.Anything, Arg<bool>.Is.Anything))
          .Throw (new InvalidOperationException());

      var validationFailures = BusinessObjectValidationResultExtensions.GetValidationFailures (validationResultStub, controlMock);

      Assert.That (validationFailures, Is.Empty);
      controlMock.VerifyAllExpectations();
    }

    [Test]
    public void GetValidationFailures_WithValidObjectAndProperty_ReturnsValidationFailures ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();
      var businessObjectPropertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();

      var dataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      dataSourceStub.BusinessObject = businessObjectStub;

      var controlStub = MockRepository.GenerateStub<IBusinessObjectBoundEditableWebControl>();
      controlStub.Stub (_ => _.HasValidBinding).Return (true);
      controlStub.DataSource = dataSourceStub;
      controlStub.Property = businessObjectPropertyStub;

      var businessObjectValidationFailure1 = BusinessObjectValidationFailure.Create ("Error 1");
      var businessObjectValidationFailure2 = BusinessObjectValidationFailure.Create ("Error 2");
      var validationResultStub = MockRepository.GenerateStub<IBusinessObjectValidationResult>();
      validationResultStub
          .Stub (_ => _.GetValidationFailures (businessObjectStub, businessObjectPropertyStub, true))
          .Return (new[] {businessObjectValidationFailure1, businessObjectValidationFailure2});

      var validationFailures = BusinessObjectValidationResultExtensions.GetValidationFailures (validationResultStub, controlStub);

      Assert.That (validationFailures, Is.EquivalentTo (new[] {businessObjectValidationFailure1, businessObjectValidationFailure2}));
    }
  }
}