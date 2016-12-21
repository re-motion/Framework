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
using FluentValidation.Internal;
using FluentValidation.Resources;
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Validation.Globalization.UnitTests.TestDomain;
using Remotion.Validation.Globalization.UnitTests.TestHelpers;
using Rhino.Mocks;

namespace Remotion.Validation.Globalization.UnitTests
{
  [TestFixture]
  public class ValidationRuleGlobalizationServiceTest
  {
    private IDefaultMessageEvaluator _defaultMessageEvaluatorMock;
    private ValidationRuleGlobalizationService _service;
    private PropertyRule _propertyRule;
    private NotNullValidator _validator1;
    private NotEmptyValidator _validator2;
    private NotEqualValidator _validator3;
    private IErrorMessageGlobalizationService _validatorGlobalizationServiceMock;
    private IStringSource _errorMessageSource1;
    private IStringSource _errorMessageSource2;
    private IStringSource _errorMessageSource3;

    [SetUp]
    public void SetUp ()
    {
      _defaultMessageEvaluatorMock = MockRepository.GenerateStrictMock<IDefaultMessageEvaluator>();
      _validatorGlobalizationServiceMock = MockRepository.GenerateStrictMock<IErrorMessageGlobalizationService>();

      _validator1 = new NotNullValidator();
      _errorMessageSource1 = _validator1.ErrorMessageSource;
      _validator2 = new NotEmptyValidator (null);
      _errorMessageSource2 = _validator2.ErrorMessageSource;
      _validator3 = new NotEqualValidator ("test");
      _errorMessageSource3 = _validator3.ErrorMessageSource;

      _propertyRule = PropertyRule.Create (ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName));
      _propertyRule.AddValidator (_validator1);
      _propertyRule.AddValidator (_validator2);
      _propertyRule.AddValidator (_validator3);

      _service = new ValidationRuleGlobalizationService (_defaultMessageEvaluatorMock, _validatorGlobalizationServiceMock);
    }

    [Test]
    public void ApplyLocalization_ResourceNotFound_NoDefaultMessageAssigned ()
    {
      _defaultMessageEvaluatorMock.Expect (mock => mock.HasDefaultMessageAssigned (_validator1)).Return (false);
      _defaultMessageEvaluatorMock.Expect (mock => mock.HasDefaultMessageAssigned (_validator2)).Return (false);
      _defaultMessageEvaluatorMock.Expect (mock => mock.HasDefaultMessageAssigned (_validator3)).Return (false);

      _service.ApplyMetadata (_propertyRule, typeof (Customer));

      _defaultMessageEvaluatorMock.VerifyAllExpectations();
      Assert.That (_validator1.ErrorMessageSource, Is.SameAs (_errorMessageSource1));
      Assert.That (_validator2.ErrorMessageSource, Is.SameAs (_errorMessageSource2));
      Assert.That (_validator3.ErrorMessageSource, Is.SameAs (_errorMessageSource3));
      _validatorGlobalizationServiceMock.VerifyAllExpectations(); //lazy evaluation => has to placed after the assertions!
    }

    [Test]
    public void ApplyLocalization_ResourceFound_DefaultMessageAssigned ()
    {
      _defaultMessageEvaluatorMock.Expect (mock => mock.HasDefaultMessageAssigned (_validator1)).Return (true);
      _defaultMessageEvaluatorMock.Expect (mock => mock.HasDefaultMessageAssigned (_validator2)).Return (false);
      _defaultMessageEvaluatorMock.Expect (mock => mock.HasDefaultMessageAssigned (_validator3)).Return (true);
      _validatorGlobalizationServiceMock.Expect (mock => mock.GetErrorMessage (_validator1)).Return ("FakeErrorMsg1");
      _validatorGlobalizationServiceMock.Expect (mock => mock.GetErrorMessage (_validator3)).Return ("FakeErrorMsg2");

      _service.ApplyMetadata (_propertyRule, typeof (Customer));

      _defaultMessageEvaluatorMock.VerifyAllExpectations();
      Assert.That (_validator1.ErrorMessageSource, Is.Not.SameAs (_errorMessageSource1));
      Assert.That (_validator1.ErrorMessageSource, Is.TypeOf (typeof (ErrorMessageStringSource)));
      Assert.That (_validator1.ErrorMessageSource.GetString(), Is.EqualTo ("FakeErrorMsg1"));
      Assert.That (_validator2.ErrorMessageSource, Is.SameAs (_errorMessageSource2));
      Assert.That (_validator3.ErrorMessageSource, Is.Not.SameAs (_errorMessageSource3));
      Assert.That (_validator3.ErrorMessageSource, Is.TypeOf (typeof (ErrorMessageStringSource)));
      Assert.That (_validator3.ErrorMessageSource.GetString(), Is.EqualTo ("FakeErrorMsg2"));
      _validatorGlobalizationServiceMock.VerifyAllExpectations(); //lazy evaluation => has to placed after the assertions!
    }
  }
}