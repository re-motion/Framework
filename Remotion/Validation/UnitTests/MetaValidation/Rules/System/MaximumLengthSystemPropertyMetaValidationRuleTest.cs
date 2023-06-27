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
using Moq;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation.Rules.System;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.MetaValidation.Rules.System
{
  [TestFixture]
  public class MaximumLengthSystemPropertyMetaValidationRuleTest
  {
    private MaximumLengthSystemPropertyMetaValidationRule _rule;
    private MaximumLengthValidator _maxLengthValidator1;
    private MaximumLengthValidator _maxLengthValidator2;
    private MaximumLengthValidator _maxLengthValidator3;

    private Mock<IMaximumLengthValidator> _maxLengthValidatorStubWithZeroLength1;
    private Mock<IMaximumLengthValidator> _maxLengthValidatorStubWithZeroLength2;

    [SetUp]
    public void SetUp ()
    {
      _maxLengthValidator1 = new MaximumLengthValidator(50, new InvariantValidationMessage("Fake Message"));
      _maxLengthValidator2 = new MaximumLengthValidator(60, new InvariantValidationMessage("Fake Message"));
      _maxLengthValidator3 = new MaximumLengthValidator(70, new InvariantValidationMessage("Fake Message"));

      _maxLengthValidatorStubWithZeroLength1 = new Mock<IMaximumLengthValidator>();
      _maxLengthValidatorStubWithZeroLength1
          .Setup(m => m.Max)
          .Returns(0);

      _maxLengthValidatorStubWithZeroLength2 = new Mock<IMaximumLengthValidator>();
      _maxLengthValidatorStubWithZeroLength2
          .Setup(m => m.Max)
          .Returns(0);

      _rule = new MaximumLengthSystemPropertyMetaValidationRule(PropertyInfoAdapter.Create(typeof(Customer).GetProperty("UserName")));
    }

    [Test]
    public void Validate_NoRules ()
    {
      var result = _rule.Validate(new MaximumLengthValidator[0]).Single();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_ValidRules ()
    {
      var result = _rule.Validate(new[] { _maxLengthValidator1 }).Single();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_WithTwoZeroLengthRules_ValidRules ()
    {
      var result = _rule.Validate(new[] { _maxLengthValidatorStubWithZeroLength1.Object, _maxLengthValidatorStubWithZeroLength2.Object }).Single();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_InvalidRules ()
    {
      var result = _rule.Validate(new[] { _maxLengthValidator2, _maxLengthValidator3 }).Single();

      Assert.That(result.IsValid, Is.False);
      Assert.That(
          result.Message,
          Is.EqualTo(
              "'MaximumLengthSystemPropertyMetaValidationRule' failed for member 'Remotion.Validation.UnitTests.TestDomain.Customer.UserName'."));
    }

    [Test]
    public void Validate_WithOneZeroAndOneGreaterZeroLengthRules_InvalidRules ()
    {
      var result = _rule.Validate(new[] { _maxLengthValidatorStubWithZeroLength1.Object, _maxLengthValidator1 }).Single();

      Assert.That(result.IsValid, Is.False);
      Assert.That(
          result.Message,
          Is.EqualTo(
              "'MaximumLengthSystemPropertyMetaValidationRule' failed for member 'Remotion.Validation.UnitTests.TestDomain.Customer.UserName'."));
    }
  }
}
