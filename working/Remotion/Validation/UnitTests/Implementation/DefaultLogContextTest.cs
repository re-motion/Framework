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
using Remotion.Validation.Rules;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class DefaultLogContextTest
  {
    private DefaultLogContext _logContext;
    private IAddingComponentPropertyRule _addedPropertyRuleStub1;
    private IAddingComponentPropertyRule _addedPropertyRuleStub2;
    private IPropertyValidator _propertyValidatorStub1;
    private IPropertyValidator _propertyValidatorStub2;
    private IPropertyValidator _propertyValidatorStub3;

    [SetUp]
    public void SetUp ()
    {
      _addedPropertyRuleStub1 = MockRepository.GenerateStub<IAddingComponentPropertyRule>();
      _addedPropertyRuleStub2 = MockRepository.GenerateStub<IAddingComponentPropertyRule> ();
      
      _propertyValidatorStub1 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub2 = MockRepository.GenerateStub<IPropertyValidator> ();
      _propertyValidatorStub3 = MockRepository.GenerateStub<IPropertyValidator> ();

      _logContext = new DefaultLogContext();
    }

    [Test]
    public void GetLogContextInfos_NoLogContextInfoAdded ()
    {
      var result = _logContext.GetLogContextInfos (_addedPropertyRuleStub1);

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetLogContextInfos_LogContextInfoAdded ()
    {
      var validatorRegistrationWithContext1 = new ValidatorRegistrationWithContext[0];
      var validatorRegistrationWithContext2 = new ValidatorRegistrationWithContext[0];
      var validatorRegistrationWithContext3 = new ValidatorRegistrationWithContext[0];

      _logContext.ValidatorRemoved (_propertyValidatorStub1, validatorRegistrationWithContext1, _addedPropertyRuleStub1);
      _logContext.ValidatorRemoved (_propertyValidatorStub2, validatorRegistrationWithContext2, _addedPropertyRuleStub1);
      _logContext.ValidatorRemoved (_propertyValidatorStub3, validatorRegistrationWithContext3, _addedPropertyRuleStub2);

      var result1 = _logContext.GetLogContextInfos (_addedPropertyRuleStub1).ToArray();
      Assert.That (result1.Count(), Is.EqualTo (2));
      Assert.That (result1[0].RemvovedValidator, Is.SameAs (_propertyValidatorStub1));
      Assert.That (result1[0].RemovingValidatorRegistrationsWithContext, Is.SameAs (validatorRegistrationWithContext1));
      Assert.That (result1[1].RemvovedValidator, Is.SameAs (_propertyValidatorStub2));
      Assert.That (result1[1].RemovingValidatorRegistrationsWithContext, Is.SameAs (validatorRegistrationWithContext2));

      var result2 = _logContext.GetLogContextInfos (_addedPropertyRuleStub2).ToArray();
      Assert.That (result2.Count (), Is.EqualTo (1));
      Assert.That (result2[0].RemvovedValidator, Is.SameAs (_propertyValidatorStub3));
      Assert.That (result2[0].RemovingValidatorRegistrationsWithContext, Is.SameAs (validatorRegistrationWithContext3));
    }
  }
}