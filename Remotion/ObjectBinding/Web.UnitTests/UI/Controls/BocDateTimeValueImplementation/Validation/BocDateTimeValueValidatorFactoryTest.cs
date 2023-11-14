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
using System.Linq;
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocDateTimeValueImplementation.Validation
{
  [TestFixture]
  public class BocDateTimeValueValidatorFactoryTest
  {
    private IBocDateTimeValueValidatorFactory _validatorFactory;

    [SetUp]
    public void SetUp ()
    {
      _validatorFactory = new BocDateTimeValueValidatorFactory();
    }

    [Test]
    [TestCase(true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase(true, false, new[] { typeof(BocDateTimeRequiredValidator), typeof(BocDateTimeFormatValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase(false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase(false, false, new[] { typeof(BocDateTimeFormatValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabled (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsEnabled(isRequired);
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    [Test]
    [TestCase(true, true, true, true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase(true, false, true, true, true, new[] { typeof(BocDateTimeRequiredValidator), typeof(BocDateTimeFormatValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase(false, true, true, true, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase(false, false, true, true, true, new[] { typeof(BocDateTimeFormatValidator) }, Description = "Not Required/Not ReadOnly")]
    [TestCase(true, false, false, false, true, new[] { typeof(BocDateTimeFormatValidator) }, Description = "Required/Not ReadOnly/No DataSource")]
    [TestCase(true, false, true, false, true, new[] { typeof(BocDateTimeFormatValidator) }, Description = "Required/Not ReadOnly/No BusinessObject")]
    [TestCase(true, false, true, true, false, new[] { typeof(BocDateTimeFormatValidator) }, Description = "Required/Not ReadOnly/No Property")]
    public void CreateValidators_WithOptionalValidatorsDisabled (
        bool isRequired,
        bool isReadonly,
        bool hasDataSource,
        bool hasBusinessObject,
        bool hasProperty,
        Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsDisabled(isRequired, hasDataSource, hasBusinessObject, hasProperty);
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    private IBocDateTimeValue GetControlWithOptionalValidatorsEnabled (bool isRequired)
    {
      var outValue = "MockValue";
      var dataSourceStub = new Mock<IBusinessObjectDataSource>();
      var propertyStub = new Mock<IBusinessObjectDateTimeProperty>();
      propertyStub.Setup(p => p.IsNullable).Throws(new InvalidOperationException("Property.IsNullable is not relevant with optional validators enabled."));
      propertyStub.Setup(p => p.IsRequired).Throws(new InvalidOperationException("Property.IsRequired is not relevant."));

      var controlMock = new Mock<IBocDateTimeValue>();
      controlMock.Setup(c => c.IsRequired).Returns(isRequired).Verifiable();
      controlMock.Setup(c => c.AreOptionalValidatorsEnabled).Returns(true).Verifiable();
      controlMock.Setup(c => c.Property).Returns(propertyStub.Object).Verifiable();
      controlMock.Setup(c => c.DataSource).Returns(dataSourceStub.Object).Verifiable();

      var resourceManagerMock = new Mock<IResourceManager>();
      resourceManagerMock
          .Setup(r => r.TryGetString(It.IsAny<string>(), out outValue))
          .Returns(true)
          .Verifiable();

      controlMock.Setup(c => c.GetResourceManager()).Returns(resourceManagerMock.Object).Verifiable();
      controlMock.Setup(c => c.TargetControl).Returns(new Control() { ID = "ID" }).Verifiable();

      return controlMock.Object;
    }

    private IBocDateTimeValue GetControlWithOptionalValidatorsDisabled (bool isRequired, bool hasDataSource = true, bool hasBusinessObject = true, bool hasProperty = true)
    {
      var outValue = "MockValue";
      var dataSourceStub = new Mock<IBusinessObjectDataSource>();
      dataSourceStub.SetupProperty(_ => _.BusinessObject);
      dataSourceStub.Object.BusinessObject = hasBusinessObject ? new Mock<IBusinessObject>().Object : null;
      var propertyStub = new Mock<IBusinessObjectDateTimeProperty>();
      propertyStub.Setup(p => p.IsNullable).Returns(!isRequired);
      propertyStub.Setup(p => p.IsRequired).Throws(new InvalidOperationException("Property.IsRequired is not relevant."));

      var controlMock = new Mock<IBocDateTimeValue>();
      controlMock.Setup(c => c.IsRequired).Throws(new InvalidOperationException("Control settings are not relevant with optional validators disabled.")).Verifiable();
      controlMock.Setup(c => c.AreOptionalValidatorsEnabled).Returns(false).Verifiable();
      controlMock.Setup(c => c.Property).Returns(hasProperty ? propertyStub.Object : null).Verifiable();
      controlMock.Setup(c => c.DataSource).Returns(hasDataSource ? dataSourceStub.Object : null).Verifiable();

      var resourceManagerMock = new Mock<IResourceManager>();
      resourceManagerMock
          .Setup(r => r.TryGetString(It.IsAny<string>(), out outValue))
          .Returns(true)
          .Verifiable();

      controlMock.Setup(c => c.GetResourceManager()).Returns(resourceManagerMock.Object).Verifiable();
      controlMock.Setup(c => c.TargetControl).Returns(new Control() { ID = "ID" }).Verifiable();

      return controlMock.Object;
    }
  }
}
