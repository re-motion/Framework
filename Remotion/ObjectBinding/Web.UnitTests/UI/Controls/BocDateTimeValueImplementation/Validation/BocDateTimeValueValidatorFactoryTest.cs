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
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation;
using Rhino.Mocks;

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
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (BocDateTimeRequiredValidator), typeof (BocDateTimeFormatValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (BocDateTimeFormatValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabled (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsEnabled (isRequired);
      var validators = _validatorFactory.CreateValidators (control, isReadonly);

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, true, true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, true, true, true, new[] { typeof (BocDateTimeRequiredValidator), typeof (BocDateTimeFormatValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, true, true, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, true, true, true, new[] { typeof (BocDateTimeFormatValidator) }, Description = "Not Required/Not ReadOnly")]
    [TestCase (true, false, false, false, true, new[] { typeof (BocDateTimeFormatValidator) }, Description = "Required/Not ReadOnly/No DataSource")]
    [TestCase (true, false, true, false, true, new[] { typeof (BocDateTimeFormatValidator) }, Description = "Required/Not ReadOnly/No BusinessObject")]
    [TestCase (true, false, true, true, false, new[] { typeof (BocDateTimeFormatValidator) }, Description = "Required/Not ReadOnly/No Property")]
    public void CreateValidators_WithOptionalValidatorsDisabled (bool isRequired, bool isReadonly, bool hasDataSource, bool hasBusinessObject, bool hasProperty, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsDisabled (isRequired, hasDataSource, hasBusinessObject, hasProperty);
      var validators = _validatorFactory.CreateValidators (control, isReadonly);

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    private IBocDateTimeValue GetControlWithOptionalValidatorsEnabled (bool isRequired)
    {
      var dataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      var propertyStub = MockRepository.GenerateStub<IBusinessObjectDateTimeProperty>();
      propertyStub.Stub (p => p.IsRequired).Throw (new InvalidOperationException ("Property is not relevant with optional validators enabled."));

      var controlMock = MockRepository.GenerateMock<IBocDateTimeValue>();
      controlMock.Expect (c => c.IsRequired).Return (isRequired);
      controlMock.Expect (c => c.AreOptionalValidatorsEnabled).Return (true);
      controlMock.Expect (c => c.Property).Return (propertyStub);
      controlMock.Expect (c => c.DataSource).Return (dataSourceStub);

      var resourceManagerMock = MockRepository.GenerateMock<IResourceManager>();
      resourceManagerMock.Expect (r => r.TryGetString (Arg<string>.Is.Anything, out Arg<string>.Out ("MockValue").Dummy))
          .Return (true);

      controlMock.Expect (c => c.GetResourceManager()).Return (resourceManagerMock);
      controlMock.Expect (c => c.TargetControl).Return (new Control() { ID = "ID" });

      return controlMock;
    }

    private IBocDateTimeValue GetControlWithOptionalValidatorsDisabled (bool isRequired, bool hasDataSource = true, bool hasBusinessObject = true, bool hasProperty = true)
    {
      var dataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      dataSourceStub.BusinessObject = hasBusinessObject ? MockRepository.GenerateStub<IBusinessObject>() : null;
      var propertyStub = MockRepository.GenerateStub<IBusinessObjectDateTimeProperty>();
      propertyStub.Stub (p => p.IsRequired).Return (isRequired);

      var controlMock = MockRepository.GenerateMock<IBocDateTimeValue>();
      controlMock.Expect (c => c.IsRequired).Throw (new InvalidOperationException ("Control settings are not relevant with optional validators disabled."));
      controlMock.Expect (c => c.AreOptionalValidatorsEnabled).Return (false);
      controlMock.Expect (c => c.Property).Return (hasProperty ? propertyStub : null);
      controlMock.Expect (c => c.DataSource).Return (hasDataSource ? dataSourceStub : null);

      var resourceManagerMock = MockRepository.GenerateMock<IResourceManager>();
      resourceManagerMock.Expect (r => r.TryGetString (Arg<string>.Is.Anything, out Arg<string>.Out ("MockValue").Dummy))
          .Return (true);

      controlMock.Expect (c => c.GetResourceManager()).Return (resourceManagerMock);
      controlMock.Expect (c => c.TargetControl).Return (new Control() { ID = "ID" });

      return controlMock;
    }
  }
}