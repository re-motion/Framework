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
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestHelpers;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class FluentValidationValidatorFormatterDecoratorTest
  {
    private IValidatorFormatter _fallBackValidatorFormatterMock;
    private FluentValidationValidatorFormatterDecorator _formatter;
    private Func<Type, string> _typeNameFormatter;

    [SetUp]
    public void SetUp ()
    {
      _fallBackValidatorFormatterMock = MockRepository.GenerateStrictMock<IValidatorFormatter>();
      _typeNameFormatter = t => t.Name;

      _formatter = new FluentValidationValidatorFormatterDecorator (_fallBackValidatorFormatterMock);
    }

    [Test]
    public void Format_NotNullValidator ()
    {
      var validator = new NotNullValidator();

      var result = _formatter.Format (validator, _typeNameFormatter);

      Assert.That (result, Is.EqualTo ("NotNullValidator"));
    }

    [Test]
    public void Format_NotEmptyValidator ()
    {
      var validator = new NotEmptyValidator (null);

      var result = _formatter.Format (validator, _typeNameFormatter);

      Assert.That (result, Is.EqualTo ("NotEmptyValidator"));
    }

    [Test]
    public void Format_EmailValidator ()
    {
      var validator = new EmailValidator();

      var result = _formatter.Format (validator, _typeNameFormatter);

      Assert.That (result, Is.EqualTo ("EmailValidator"));
    }

    [Test]
    public void Format_CreditCardValidator ()
    {
      var validator = new CreditCardValidator();

      var result = _formatter.Format (validator, _typeNameFormatter);

      Assert.That (result, Is.EqualTo ("CreditCardValidator"));
    }

    [Test]
    public void Format_ILengthValidators ()
    {
      var validator1 = new LengthValidator (5, 10);
      var validator2 = new MaximumLengthValidator (12);
      var validator3 = new MinimumLengthValidator (2);
      var validator4 = new ExactLengthValidator (4);

      Assert.That (_formatter.Format (validator1, _typeNameFormatter), Is.EqualTo ("LengthValidator { MinLength = '5', MaxLength = '10' }"));
      Assert.That (_formatter.Format (validator2, _typeNameFormatter), Is.EqualTo ("MaximumLengthValidator { MinLength = '0', MaxLength = '12' }"));
      Assert.That (_formatter.Format (validator3, _typeNameFormatter), Is.EqualTo ("MinimumLengthValidator { MinLength = '2', MaxLength = '-1' }"));
      Assert.That (_formatter.Format (validator4, _typeNameFormatter), Is.EqualTo ("ExactLengthValidator { MinLength = '4', MaxLength = '4' }"));
    }

    [Test]
    public void Format_IBetweenValidators ()
    {
      var validator1 = new ExclusiveBetweenValidator (3, 8);
      var validator2 = new InclusiveBetweenValidator (5, 9);

      Assert.That (_formatter.Format (validator1, _typeNameFormatter), Is.EqualTo ("ExclusiveBetweenValidator { From = '3', To = '8' }"));
      Assert.That (_formatter.Format (validator2, _typeNameFormatter), Is.EqualTo ("InclusiveBetweenValidator { From = '5', To = '9' }"));
    }

    [Test]
    public void Format_IComparisonValidators ()
    {
      var validator1 = new EqualValidator (5);
      var validator2 = new NotEqualValidator (10);
      var validator3 = new GreaterThanValidator (8);
      var validator4 = new GreaterThanOrEqualValidator (7);
      var validator5 = new LessThanValidator (2);
      var validator6 = new LessThanOrEqualValidator (1);
      var validator7 = new EqualValidator (o => o, typeof (Customer).GetProperty ("UserName"));

      Assert.That (_formatter.Format (validator1, _typeNameFormatter), Is.EqualTo ("EqualValidator { ValueToCompare = '5' }"));
      Assert.That (_formatter.Format (validator2, _typeNameFormatter), Is.EqualTo ("NotEqualValidator { ValueToCompare = '10' }"));
      Assert.That (_formatter.Format (validator3, _typeNameFormatter), Is.EqualTo ("GreaterThanValidator { ValueToCompare = '8' }"));
      Assert.That (_formatter.Format (validator4, _typeNameFormatter), Is.EqualTo ("GreaterThanOrEqualValidator { ValueToCompare = '7' }"));
      Assert.That (_formatter.Format (validator5, _typeNameFormatter), Is.EqualTo ("LessThanValidator { ValueToCompare = '2' }"));
      Assert.That (_formatter.Format (validator6, _typeNameFormatter), Is.EqualTo ("LessThanOrEqualValidator { ValueToCompare = '1' }"));
      Assert.That (_formatter.Format (validator7, _typeNameFormatter), Is.EqualTo ("EqualValidator { MemberToCompare = 'Customer.UserName\r\n' }"));
    }

    [Test]
    public void Format_IRegularExpressionValidators ()
    {
      var validator = new RegularExpressionValidator ("expression");

      Assert.That (_formatter.Format (validator, _typeNameFormatter), Is.EqualTo ("RegularExpressionValidator { Expression = 'expression' }"));
    }

    [Test]
    public void Format_Fallback ()
    {
      var validator = new StubPropertyValidator();
      _fallBackValidatorFormatterMock.Expect (mock => mock.Format (validator, _typeNameFormatter)).Return ("FakeResult");

      var result = _formatter.Format (validator, _typeNameFormatter);

      _fallBackValidatorFormatterMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("FakeResult"));
    }
  }
}