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
using Remotion.Validation.Implementation;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class DefaultLogContextTest
  {
    private DefaultLogContext _logContext;
    private Mock<IAddingPropertyValidationRuleCollector> _addedPropertyValidationRuleCollectorStub1;
    private Mock<IAddingPropertyValidationRuleCollector> _addedPropertyValidationRuleCollectorStub2;
    private Mock<IPropertyValidator> _propertyValidatorStub1;
    private Mock<IPropertyValidator> _propertyValidatorStub2;
    private Mock<IPropertyValidator> _propertyValidatorStub3;
    private Mock<IAddingObjectValidationRuleCollector> _addedObjectValidationRuleCollectorStub1;
    private Mock<IAddingObjectValidationRuleCollector> _addedObjectValidationRuleCollectorStub2;
    private Mock<IObjectValidator> _objectValidatorStub1;
    private Mock<IObjectValidator> _objectValidatorStub2;
    private Mock<IObjectValidator> _objectValidatorStub3;

    [SetUp]
    public void SetUp ()
    {
      _addedPropertyValidationRuleCollectorStub1 = new Mock<IAddingPropertyValidationRuleCollector>();
      _addedPropertyValidationRuleCollectorStub2 = new Mock<IAddingPropertyValidationRuleCollector>();

      _propertyValidatorStub1 = new Mock<IPropertyValidator>();
      _propertyValidatorStub2 = new Mock<IPropertyValidator>();
      _propertyValidatorStub3 = new Mock<IPropertyValidator>();

      _addedObjectValidationRuleCollectorStub1 = new Mock<IAddingObjectValidationRuleCollector>();
      _addedObjectValidationRuleCollectorStub2 = new Mock<IAddingObjectValidationRuleCollector>();

      _objectValidatorStub1 = new Mock<IObjectValidator>();
      _objectValidatorStub2 = new Mock<IObjectValidator>();
      _objectValidatorStub3 = new Mock<IObjectValidator>();

      _logContext = new DefaultLogContext();
    }

    [Test]
    public void GetLogContextInfos_WithPropertyAndNoLogContextInfoAdded ()
    {
      var result = _logContext.GetLogContextInfos(_addedPropertyValidationRuleCollectorStub1.Object);

      Assert.That(result.Any(), Is.False);
    }

    [Test]
    public void GetLogContextInfos_WithPropertyAndLogContextInfoAdded ()
    {
      var removingPropertyValidatorRegistrations1 = new RemovingPropertyValidatorRegistration[0];
      var removingPropertyValidatorRegistrations2 = new RemovingPropertyValidatorRegistration[0];
      var removingPropertyValidatorRegistrations3 = new RemovingPropertyValidatorRegistration[0];

      _logContext.ValidatorRemoved(_propertyValidatorStub1.Object, removingPropertyValidatorRegistrations1, _addedPropertyValidationRuleCollectorStub1.Object);
      _logContext.ValidatorRemoved(_propertyValidatorStub2.Object, removingPropertyValidatorRegistrations2, _addedPropertyValidationRuleCollectorStub1.Object);
      _logContext.ValidatorRemoved(_propertyValidatorStub3.Object, removingPropertyValidatorRegistrations3, _addedPropertyValidationRuleCollectorStub2.Object);

      var result1 = _logContext.GetLogContextInfos(_addedPropertyValidationRuleCollectorStub1.Object).ToArray();
      Assert.That(result1.Count(), Is.EqualTo(2));
      Assert.That(result1[0].RemovedValidator, Is.SameAs(_propertyValidatorStub1.Object));
      Assert.That(result1[0].RemovingPropertyValidatorRegistrations, Is.SameAs(removingPropertyValidatorRegistrations1));
      Assert.That(result1[1].RemovedValidator, Is.SameAs(_propertyValidatorStub2.Object));
      Assert.That(result1[1].RemovingPropertyValidatorRegistrations, Is.SameAs(removingPropertyValidatorRegistrations2));

      var result2 = _logContext.GetLogContextInfos(_addedPropertyValidationRuleCollectorStub2.Object).ToArray();
      Assert.That(result2.Count(), Is.EqualTo(1));
      Assert.That(result2[0].RemovedValidator, Is.SameAs(_propertyValidatorStub3.Object));
      Assert.That(result2[0].RemovingPropertyValidatorRegistrations, Is.SameAs(removingPropertyValidatorRegistrations3));
    }

    [Test]
    public void GetLogContextInfos_WithObjectAndNoLogContextInfoAdded ()
    {
      var result = _logContext.GetLogContextInfos(_addedObjectValidationRuleCollectorStub1.Object);

      Assert.That(result.Any(), Is.False);
    }

    [Test]
    public void GetLogContextInfos_WithObjectAndLogContextInfoAdded ()
    {
      var removingObjectValidatorRegistrations1 = new RemovingObjectValidatorRegistration[0];
      var removingObjectValidatorRegistrations2 = new RemovingObjectValidatorRegistration[0];
      var removingObjectValidatorRegistrations3 = new RemovingObjectValidatorRegistration[0];

      _logContext.ValidatorRemoved(_objectValidatorStub1.Object, removingObjectValidatorRegistrations1, _addedObjectValidationRuleCollectorStub1.Object);
      _logContext.ValidatorRemoved(_objectValidatorStub2.Object, removingObjectValidatorRegistrations2, _addedObjectValidationRuleCollectorStub1.Object);
      _logContext.ValidatorRemoved(_objectValidatorStub3.Object, removingObjectValidatorRegistrations3, _addedObjectValidationRuleCollectorStub2.Object);

      var result1 = _logContext.GetLogContextInfos(_addedObjectValidationRuleCollectorStub1.Object).ToArray();
      Assert.That(result1.Count(), Is.EqualTo(2));
      Assert.That(result1[0].RemovedValidator, Is.SameAs(_objectValidatorStub1.Object));
      Assert.That(result1[0].RemovingObjectValidatorRegistrations, Is.SameAs(removingObjectValidatorRegistrations1));
      Assert.That(result1[1].RemovedValidator, Is.SameAs(_objectValidatorStub2.Object));
      Assert.That(result1[1].RemovingObjectValidatorRegistrations, Is.SameAs(removingObjectValidatorRegistrations2));

      var result2 = _logContext.GetLogContextInfos(_addedObjectValidationRuleCollectorStub2.Object).ToArray();
      Assert.That(result2.Count(), Is.EqualTo(1));
      Assert.That(result2[0].RemovedValidator, Is.SameAs(_objectValidatorStub3.Object));
      Assert.That(result2[0].RemovingObjectValidatorRegistrations, Is.SameAs(removingObjectValidatorRegistrations3));
    }
  }
}