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
using Moq;
using NUnit.Framework;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestDomain.Validators;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.RoleCollectors
{
  [TestFixture]
  public class RemovingObjectValidatorRegistrationTest
  {
    [Test]
    public void Initialization ()
    {
      var validatorType = new Mock<IObjectValidator>().Object.GetType();
      var collectorTypeToRemoveFrom = typeof(CustomerValidationRuleCollector1);
      Func<IObjectValidator, bool> validatorPredicate = _ => false;
      var removingObjectValidationRuleCollectorStub = new Mock<IRemovingObjectValidationRuleCollector>();

      var removingObjectValidatorRegistration = new RemovingObjectValidatorRegistration(
          validatorType,
          collectorTypeToRemoveFrom,
          validatorPredicate,
          removingObjectValidationRuleCollectorStub.Object);

      Assert.That(removingObjectValidatorRegistration.ValidatorType, Is.SameAs(validatorType));
      Assert.That(removingObjectValidatorRegistration.CollectorTypeToRemoveFrom, Is.SameAs(collectorTypeToRemoveFrom));
      Assert.That(removingObjectValidatorRegistration.ValidatorPredicate, Is.SameAs(validatorPredicate));
      Assert.That(
          removingObjectValidatorRegistration.RemovingObjectValidationRuleCollector,
          Is.SameAs(removingObjectValidationRuleCollectorStub.Object));
    }

    [Test]
    public void Initialization_WithCollectorTypeToRemoveFromIsNull ()
    {
      var validatorType = new Mock<IObjectValidator>().Object.GetType();
      Func<IObjectValidator, bool> validatorPredicate = _ => false;
      var removingObjectValidationRuleCollectorStub = new Mock<IRemovingObjectValidationRuleCollector>();

      var removingObjectValidatorRegistration = new RemovingObjectValidatorRegistration(
          validatorType,
          collectorTypeToRemoveFrom: null,
          validatorPredicate,
          removingObjectValidationRuleCollectorStub.Object);

      Assert.That(removingObjectValidatorRegistration.ValidatorType, Is.SameAs(validatorType));
      Assert.That(removingObjectValidatorRegistration.CollectorTypeToRemoveFrom, Is.Null);
      Assert.That(removingObjectValidatorRegistration.ValidatorPredicate, Is.SameAs(validatorPredicate));
      Assert.That(
          removingObjectValidatorRegistration.RemovingObjectValidationRuleCollector,
          Is.SameAs(removingObjectValidationRuleCollectorStub.Object));
    }

    [Test]
    public void Initialization_WithValidatorPredicateIsNull ()
    {
      var validatorType = new Mock<IObjectValidator>().Object.GetType();
      var collectorTypeToRemoveFrom = typeof(CustomerValidationRuleCollector1);
      var removingObjectValidationRuleCollectorStub = new Mock<IRemovingObjectValidationRuleCollector>();

      var Object = new RemovingObjectValidatorRegistration(
          validatorType,
          collectorTypeToRemoveFrom,
          validatorPredicate: null,
          removingObjectValidationRuleCollectorStub.Object);

      Assert.That(Object.ValidatorType, Is.SameAs(validatorType));
      Assert.That(Object.CollectorTypeToRemoveFrom, Is.SameAs(collectorTypeToRemoveFrom));
      Assert.That(Object.ValidatorPredicate, Is.Null);
      Assert.That(
          Object.RemovingObjectValidationRuleCollector,
          Is.SameAs(removingObjectValidationRuleCollectorStub.Object));
    }
  }
}
