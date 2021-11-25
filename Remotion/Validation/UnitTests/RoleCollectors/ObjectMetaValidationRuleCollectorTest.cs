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
using Remotion.Reflection;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;

namespace Remotion.Validation.UnitTests.RoleCollectors
{
  [TestFixture]
  public class ObjectMetaValidationRuleCollectorTest
  {
    private ObjectMetaValidationRuleCollector _ruleCollector;

    [SetUp]
    public void SetUp ()
    {
      _ruleCollector = ObjectMetaValidationRuleCollector.Create<Customer>(typeof(CustomerValidationRuleCollector1));
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_ruleCollector.ValidatedType, Is.SameAs(TypeAdapter.Create(typeof(Customer))));
      Assert.That(_ruleCollector.CollectorType, Is.SameAs(typeof(CustomerValidationRuleCollector1)));
      Assert.That(_ruleCollector.MetaValidationRules.Any(), Is.False);
    }

    [Test]
    public void RegisterMetaValidationRule ()
    {
      var metaValidationRuleStub1 = new Mock<IObjectMetaValidationRule>();
      var metaValidationRuleStub2 = new Mock<IObjectMetaValidationRule>();
      Assert.That(_ruleCollector.MetaValidationRules.Count(), Is.EqualTo(0));

      _ruleCollector.RegisterMetaValidationRule(metaValidationRuleStub1.Object);
      _ruleCollector.RegisterMetaValidationRule(metaValidationRuleStub2.Object);

      Assert.That(_ruleCollector.MetaValidationRules.Count(), Is.EqualTo(2));
      Assert.That(
          _ruleCollector.MetaValidationRules,
          Is.EquivalentTo(new[] { metaValidationRuleStub1.Object, metaValidationRuleStub2.Object }));
    }

    [Test]
    public void To_String ()
    {
      var result = _ruleCollector.ToString();

      Assert.That(
          result,
          Is.EqualTo(
              "ObjectMetaValidationRuleCollector: Remotion.Validation.UnitTests.TestDomain.Customer"));
    }
  }
}