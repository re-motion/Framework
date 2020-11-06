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
using Remotion.Validation.Implementation;
using Rhino.Mocks;
using System.Linq;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class DefaultLogContextTest
  {
    private DefaultLogContext _logContext;
    private IAddingPropertyValidationRuleCollector _addedPropertyValidationRuleCollectorStub1;
    private IAddingPropertyValidationRuleCollector _addedPropertyValidationRuleCollectorStub2;
    private IPropertyValidator _propertyValidatorStub1;
    private IPropertyValidator _propertyValidatorStub2;
    private IPropertyValidator _propertyValidatorStub3;
    private IAddingObjectValidationRuleCollector _addedObjectValidationRuleCollectorStub1;
    private IAddingObjectValidationRuleCollector _addedObjectValidationRuleCollectorStub2;
    private IObjectValidator _objectValidatorStub1;
    private IObjectValidator _objectValidatorStub2;
    private IObjectValidator _objectValidatorStub3;

    [SetUp]
    public void SetUp ()
    {
      _addedPropertyValidationRuleCollectorStub1 = MockRepository.GenerateStub<IAddingPropertyValidationRuleCollector>();
      _addedPropertyValidationRuleCollectorStub2 = MockRepository.GenerateStub<IAddingPropertyValidationRuleCollector>();

      _propertyValidatorStub1 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub2 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub3 = MockRepository.GenerateStub<IPropertyValidator>();

      _addedObjectValidationRuleCollectorStub1 = MockRepository.GenerateStub<IAddingObjectValidationRuleCollector>();
      _addedObjectValidationRuleCollectorStub2 = MockRepository.GenerateStub<IAddingObjectValidationRuleCollector>();

      _objectValidatorStub1 = MockRepository.GenerateStub<IObjectValidator>();
      _objectValidatorStub2 = MockRepository.GenerateStub<IObjectValidator>();
      _objectValidatorStub3 = MockRepository.GenerateStub<IObjectValidator>();

      _logContext = new DefaultLogContext();
    }

    [Test]
    public void GetLogContextInfos_WithPropertyAndNoLogContextInfoAdded ()
    {
      var result = _logContext.GetLogContextInfos (_addedPropertyValidationRuleCollectorStub1);

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetLogContextInfos_WithPropertyAndLogContextInfoAdded ()
    {
      var removingPropertyValidatorRegistrations1 = new RemovingPropertyValidatorRegistration[0];
      var removingPropertyValidatorRegistrations2 = new RemovingPropertyValidatorRegistration[0];
      var removingPropertyValidatorRegistrations3 = new RemovingPropertyValidatorRegistration[0];

      _logContext.ValidatorRemoved (_propertyValidatorStub1, removingPropertyValidatorRegistrations1, _addedPropertyValidationRuleCollectorStub1);
      _logContext.ValidatorRemoved (_propertyValidatorStub2, removingPropertyValidatorRegistrations2, _addedPropertyValidationRuleCollectorStub1);
      _logContext.ValidatorRemoved (_propertyValidatorStub3, removingPropertyValidatorRegistrations3, _addedPropertyValidationRuleCollectorStub2);

      var result1 = _logContext.GetLogContextInfos (_addedPropertyValidationRuleCollectorStub1).ToArray();
      Assert.That (result1.Count(), Is.EqualTo (2));
      Assert.That (result1[0].RemovedValidator, Is.SameAs (_propertyValidatorStub1));
      Assert.That (result1[0].RemovingPropertyValidatorRegistrations, Is.SameAs (removingPropertyValidatorRegistrations1));
      Assert.That (result1[1].RemovedValidator, Is.SameAs (_propertyValidatorStub2));
      Assert.That (result1[1].RemovingPropertyValidatorRegistrations, Is.SameAs (removingPropertyValidatorRegistrations2));

      var result2 = _logContext.GetLogContextInfos (_addedPropertyValidationRuleCollectorStub2).ToArray();
      Assert.That (result2.Count (), Is.EqualTo (1));
      Assert.That (result2[0].RemovedValidator, Is.SameAs (_propertyValidatorStub3));
      Assert.That (result2[0].RemovingPropertyValidatorRegistrations, Is.SameAs (removingPropertyValidatorRegistrations3));
    }

    [Test]
    public void GetLogContextInfos_WithObjectAndNoLogContextInfoAdded ()
    {
      var result = _logContext.GetLogContextInfos (_addedObjectValidationRuleCollectorStub1);

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetLogContextInfos_WithObjectAndLogContextInfoAdded ()
    {
      var validatorRegistrationWithContext1 = new ObjectValidatorRegistrationWithContext[0];
      var validatorRegistrationWithContext2 = new ObjectValidatorRegistrationWithContext[0];
      var validatorRegistrationWithContext3 = new ObjectValidatorRegistrationWithContext[0];

      _logContext.ValidatorRemoved (_objectValidatorStub1, validatorRegistrationWithContext1, _addedObjectValidationRuleCollectorStub1);
      _logContext.ValidatorRemoved (_objectValidatorStub2, validatorRegistrationWithContext2, _addedObjectValidationRuleCollectorStub1);
      _logContext.ValidatorRemoved (_objectValidatorStub3, validatorRegistrationWithContext3, _addedObjectValidationRuleCollectorStub2);

      var result1 = _logContext.GetLogContextInfos (_addedObjectValidationRuleCollectorStub1).ToArray();
      Assert.That (result1.Count(), Is.EqualTo (2));
      Assert.That (result1[0].RemovedValidator, Is.SameAs (_objectValidatorStub1));
      Assert.That (result1[0].RemovingObjectValidatorRegistrationsWithContext, Is.SameAs (validatorRegistrationWithContext1));
      Assert.That (result1[1].RemovedValidator, Is.SameAs (_objectValidatorStub2));
      Assert.That (result1[1].RemovingObjectValidatorRegistrationsWithContext, Is.SameAs (validatorRegistrationWithContext2));

      var result2 = _logContext.GetLogContextInfos (_addedObjectValidationRuleCollectorStub2).ToArray();
      Assert.That (result2.Count (), Is.EqualTo (1));
      Assert.That (result2[0].RemovedValidator, Is.SameAs (_objectValidatorStub3));
      Assert.That (result2[0].RemovingObjectValidatorRegistrationsWithContext, Is.SameAs (validatorRegistrationWithContext3));
    }
  }
}