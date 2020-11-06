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
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.RoleCollectors
{
  [TestFixture]
  public class RemovingPropertyValidatorRegistrationTest
  {
    [Test]
    public void Initialization ()
    {
      var validatorType = typeof (NotEqualValidator);
      var removingPropertyValidationRuleCollectorStub = MockRepository.GenerateStub<IRemovingPropertyValidationRuleCollector>();

      var removingPropertyValidatorRegistration = new RemovingPropertyValidatorRegistration (
          validatorType,
          null,
          removingPropertyValidationRuleCollectorStub);

      Assert.That (removingPropertyValidatorRegistration.ValidatorType, Is.SameAs (validatorType));
      Assert.That (removingPropertyValidatorRegistration.CollectorTypeToRemoveFrom, Is.Null);
      Assert.That (
          removingPropertyValidatorRegistration.RemovingPropertyValidationRuleCollector,
          Is.SameAs (removingPropertyValidationRuleCollectorStub));
    }

    [Test]
    public void Initialization_WithCollectorTypeToRemoveFromIsNull ()
    {
      var validatorType = typeof (NotEqualValidator);
      var collectorTypeToRemoveFrom = typeof (CustomerValidationRuleCollector1);
      var removingPropertyValidationRuleCollectorStub = MockRepository.GenerateStub<IRemovingPropertyValidationRuleCollector>();

      var removingPropertyValidatorRegistration = new RemovingPropertyValidatorRegistration (
          validatorType,
          collectorTypeToRemoveFrom,
          removingPropertyValidationRuleCollectorStub);

      Assert.That (removingPropertyValidatorRegistration.ValidatorType, Is.SameAs (validatorType));
      Assert.That (removingPropertyValidatorRegistration.CollectorTypeToRemoveFrom, Is.SameAs (collectorTypeToRemoveFrom));
      Assert.That (
          removingPropertyValidatorRegistration.RemovingPropertyValidationRuleCollector,
          Is.SameAs (removingPropertyValidationRuleCollectorStub));
    }
  }
}