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
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.RuleBuilders
{
  [TestFixture]
  public class RemovingPropertyValidationRuleBuilderTest
  {
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollectorMock;
    private RemovingPropertyValidationRuleBuilder<Customer, string> _addingPropertyValidationBuilder;

    [SetUp]
    public void SetUp ()
    {
      _removingPropertyValidationRuleCollectorMock = MockRepository.GenerateStrictMock<IRemovingPropertyValidationRuleCollector>();
      _addingPropertyValidationBuilder = new RemovingPropertyValidationRuleBuilder<Customer, string> (_removingPropertyValidationRuleCollectorMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_addingPropertyValidationBuilder.RemovingPropertyValidationRuleCollector, Is.SameAs (_removingPropertyValidationRuleCollectorMock));
    }

    [Test]
    public void RemoveValidator ()
    {
      _removingPropertyValidationRuleCollectorMock.Expect (mock => mock.RegisterValidator (typeof (StubPropertyValidator), null));

      _addingPropertyValidationBuilder.Validator (typeof (StubPropertyValidator));

      _removingPropertyValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void RemoveValidator_Generic ()
    {
      _removingPropertyValidationRuleCollectorMock.Expect (mock => mock.RegisterValidator (typeof (StubPropertyValidator), null));

      _addingPropertyValidationBuilder.Validator<StubPropertyValidator>();

      _removingPropertyValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void RemoveValidator_CollectorTypeOverload ()
    {
      _removingPropertyValidationRuleCollectorMock.Expect (
          mock => mock.RegisterValidator (typeof (StubPropertyValidator), typeof (CustomerValidationRuleCollector1)));

      _addingPropertyValidationBuilder.Validator (typeof (StubPropertyValidator), typeof (CustomerValidationRuleCollector1));

      _removingPropertyValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void RemoveValidator_CollectorTypeOverload_Generic ()
    {
      _removingPropertyValidationRuleCollectorMock.Expect (
          mock => mock.RegisterValidator (typeof (StubPropertyValidator), typeof (CustomerValidationRuleCollector1)));

      _addingPropertyValidationBuilder.Validator<StubPropertyValidator, CustomerValidationRuleCollector1>();

      _removingPropertyValidationRuleCollectorMock.VerifyAllExpectations();
    }
  }
}