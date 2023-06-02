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
using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class DiagnosticInformationValidatorFormatterDecoratorTest
  {
    private Mock<IValidatorFormatter> _fallBackValidatorFormatterMock;
    private DiagnosticInformationValidatorFormatterDecorator _formatter;
    private Func<Type, string> _typeNameFormatter;

    [SetUp]
    public void SetUp ()
    {
      _fallBackValidatorFormatterMock = new Mock<IValidatorFormatter>(MockBehavior.Strict);
      _typeNameFormatter = t => t.Name;

      _formatter = new DiagnosticInformationValidatorFormatterDecorator(_fallBackValidatorFormatterMock.Object);
    }

    [Test]
    public void Format_NotNullValidator ()
    {
      var validator = new NotNullValidator(new InvariantValidationMessage("Fake Message"));

      var result = _formatter.Format(validator, _typeNameFormatter);

      Assert.That(result, Is.EqualTo("NotNullValidator"));
    }

    [Test]
    public void Format_NotEmptyBinaryValidator ()
    {
      var validator = new NotEmptyBinaryValidator(new InvariantValidationMessage("Fake Message"));

      var result = _formatter.Format(validator, _typeNameFormatter);

      Assert.That(result, Is.EqualTo("NotEmptyBinaryValidator"));
    }

    [Test]
    public void Format_NotEmptyListValidator ()
    {
      var validator = new NotEmptyCollectionValidator(new InvariantValidationMessage("Fake Message"));

      var result = _formatter.Format(validator, _typeNameFormatter);

      Assert.That(result, Is.EqualTo("NotEmptyCollectionValidator"));
    }

    [Test]
    public void Format_NotEmptyOrWhitespaceValidator ()
    {
      var validator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Fake Message"));

      var result = _formatter.Format(validator, _typeNameFormatter);

      Assert.That(result, Is.EqualTo("NotEmptyOrWhitespaceValidator"));
    }

    [Test]
    public void Format_LengthValidator ()
    {
      var validator = new LengthValidator(5, 10, new InvariantValidationMessage("Fake Message"));

      Assert.That(_formatter.Format(validator, _typeNameFormatter), Is.EqualTo("LengthValidator { MinLength = '5', MaxLength = '10' }"));
    }

    [Test]
    public void Format_MinimumLengthValidator ()
    {
      var validator = new MinimumLengthValidator(2, new InvariantValidationMessage("Fake Message"));

      Assert.That(_formatter.Format(validator, _typeNameFormatter), Is.EqualTo("MinimumLengthValidator { MinLength = '2' }"));
    }

    [Test]
    public void Format_MaximumLengthValidator ()
    {
      var validator = new MaximumLengthValidator(12, new InvariantValidationMessage("Fake Message"));

      Assert.That(_formatter.Format(validator, _typeNameFormatter), Is.EqualTo("MaximumLengthValidator { MaxLength = '12' }"));
    }

    [Test]
    public void Format_ExactLengthValidator ()
    {
      var validator = new ExactLengthValidator(4, new InvariantValidationMessage("Fake Message"));

      Assert.That(_formatter.Format(validator, _typeNameFormatter), Is.EqualTo("ExactLengthValidator { Length = '4' }"));
    }

    [Test]
    public void Format_IRangeValidators ()
    {
      var validator1 = new ExclusiveRangeValidator(3, 8, new InvariantValidationMessage("Fake Message"));
      var validator2 = new InclusiveRangeValidator(5, 9, new InvariantValidationMessage("Fake Message"));

      Assert.That(_formatter.Format(validator1, _typeNameFormatter), Is.EqualTo("ExclusiveRangeValidator { From = '3', To = '8' }"));
      Assert.That(_formatter.Format(validator2, _typeNameFormatter), Is.EqualTo("InclusiveRangeValidator { From = '5', To = '9' }"));
    }

    [Test]
    public void Format_IValueComparisonValidators ()
    {
      var validator1 = new EqualValidator(5, new InvariantValidationMessage("Fake Message"));
      var validator2 = new NotEqualValidator(10, new InvariantValidationMessage("Fake Message"));
      var validator3 = new GreaterThanValidator(8, new InvariantValidationMessage("Fake Message"));
      var validator4 = new GreaterThanOrEqualValidator(7, new InvariantValidationMessage("Fake Message"));
      var validator5 = new LessThanValidator(2, new InvariantValidationMessage("Fake Message"));
      var validator6 = new LessThanOrEqualValidator(1, new InvariantValidationMessage("Fake Message"));

      Assert.That(_formatter.Format(validator1, _typeNameFormatter), Is.EqualTo("EqualValidator { ValueToCompare = '5' }"));
      Assert.That(_formatter.Format(validator2, _typeNameFormatter), Is.EqualTo("NotEqualValidator { ValueToCompare = '10' }"));
      Assert.That(_formatter.Format(validator3, _typeNameFormatter), Is.EqualTo("GreaterThanValidator { ValueToCompare = '8' }"));
      Assert.That(_formatter.Format(validator4, _typeNameFormatter), Is.EqualTo("GreaterThanOrEqualValidator { ValueToCompare = '7' }"));
      Assert.That(_formatter.Format(validator5, _typeNameFormatter), Is.EqualTo("LessThanValidator { ValueToCompare = '2' }"));
      Assert.That(_formatter.Format(validator6, _typeNameFormatter), Is.EqualTo("LessThanOrEqualValidator { ValueToCompare = '1' }"));
    }

    [Test]
    public void Format_IRegularExpressionValidators ()
    {
      var validator = new RegularExpressionValidator(new Regex("expression"), new InvariantValidationMessage("Fake Message"));

      Assert.That(_formatter.Format(validator, _typeNameFormatter), Is.EqualTo("RegularExpressionValidator { Expression = 'expression' }"));
    }

    [Test]
    public void Format_Fallback ()
    {
      var validator = new StubPropertyValidator();
      _fallBackValidatorFormatterMock.Setup(mock => mock.Format(validator, _typeNameFormatter)).Returns("FakeResult").Verifiable();

      var result = _formatter.Format(validator, _typeNameFormatter);

      _fallBackValidatorFormatterMock.Verify();
      Assert.That(result, Is.EqualTo("FakeResult"));
    }
  }
}
