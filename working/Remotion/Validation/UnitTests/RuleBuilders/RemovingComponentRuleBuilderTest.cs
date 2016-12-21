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
  public class RemovingComponentRuleBuilderTest
  {
    private IRemovingComponentPropertyRule _removingComponentPropertyRuleMock;
    private RemovingComponentRuleBuilder<Customer, string> _addingComponentBuilder;

    [SetUp]
    public void SetUp ()
    {
      _removingComponentPropertyRuleMock = MockRepository.GenerateStrictMock<IRemovingComponentPropertyRule>();
      _addingComponentBuilder = new RemovingComponentRuleBuilder<Customer, string> (_removingComponentPropertyRuleMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_addingComponentBuilder.RemovingComponentPropertyRule, Is.SameAs (_removingComponentPropertyRuleMock));
    }

    [Test]
    public void RemoveValidator ()
    {
      _removingComponentPropertyRuleMock.Expect (mock => mock.RegisterValidator (typeof (StubPropertyValidator), null));

      _addingComponentBuilder.Validator (typeof (StubPropertyValidator));

      _removingComponentPropertyRuleMock.VerifyAllExpectations();
    }

    [Test]
    public void RemoveValidator_Generic ()
    {
      _removingComponentPropertyRuleMock.Expect (mock => mock.RegisterValidator (typeof (StubPropertyValidator), null));

      _addingComponentBuilder.Validator<StubPropertyValidator>();

      _removingComponentPropertyRuleMock.VerifyAllExpectations();
    }

    [Test]
    public void RemoveValidator_CollectorTypeOverload ()
    {
      _removingComponentPropertyRuleMock.Expect (
          mock => mock.RegisterValidator (typeof (StubPropertyValidator), typeof (CustomerValidationCollector1)));

      _addingComponentBuilder.Validator (typeof (StubPropertyValidator), typeof (CustomerValidationCollector1));

      _removingComponentPropertyRuleMock.VerifyAllExpectations();
    }

    [Test]
    public void RemoveValidator_CollectorTypeOverload_Generic ()
    {
      _removingComponentPropertyRuleMock.Expect (
          mock => mock.RegisterValidator (typeof (StubPropertyValidator), typeof (CustomerValidationCollector1)));

      _addingComponentBuilder.Validator<StubPropertyValidator, CustomerValidationCollector1>();

      _removingComponentPropertyRuleMock.VerifyAllExpectations();
    }
  }
}