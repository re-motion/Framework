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
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestDomain.Validators;
using Remotion.Validation.UnitTests.TestHelpers;

namespace Remotion.Validation.UnitTests.RoleCollectors
{
  [TestFixture]
  public class RemovingObjectValidationRuleCollectorTest
  {
    private IRemovingObjectValidationRuleCollector _removingObjectValidationRuleCollector;

    [SetUp]
    public void SetUp ()
    {
      _removingObjectValidationRuleCollector = RemovingObjectValidationRuleCollector.Create<Customer> (typeof (CustomerValidationRuleCollector1));
    }

    [Test]
    public void Initialization_PropertyDeclaredInSameClass ()
    {
      Assert.That (_removingObjectValidationRuleCollector.ValidatedType, Is.EqualTo (TypeAdapter.Create (typeof (Customer))));
      Assert.That (_removingObjectValidationRuleCollector.CollectorType, Is.EqualTo (typeof (CustomerValidationRuleCollector1)));
      Assert.That (_removingObjectValidationRuleCollector.Validators.Any(), Is.False);
    }

    [Test]
    public void RegisterValidator ()
    {
      _removingObjectValidationRuleCollector.RegisterValidator (typeof (StubObjectValidator));
      _removingObjectValidationRuleCollector.RegisterValidator (typeof (FakeCustomerValidator), typeof (CustomerValidationRuleCollector1));

      Assert.That (_removingObjectValidationRuleCollector.Validators.Count(), Is.EqualTo (2));

      Assert.That (_removingObjectValidationRuleCollector.Validators.ElementAt (0).ValidatorType, Is.EqualTo (typeof (StubObjectValidator)));
      Assert.That (_removingObjectValidationRuleCollector.Validators.ElementAt (0).CollectorTypeToRemoveFrom, Is.Null);
      Assert.That (
          _removingObjectValidationRuleCollector.Validators.ElementAt (0).RemovingObjectValidationRuleCollector,
          Is.SameAs (_removingObjectValidationRuleCollector));

      Assert.That (_removingObjectValidationRuleCollector.Validators.ElementAt (1).ValidatorType, Is.EqualTo (typeof (FakeCustomerValidator)));
      Assert.That (
          _removingObjectValidationRuleCollector.Validators.ElementAt (1).CollectorTypeToRemoveFrom,
          Is.EqualTo (typeof (CustomerValidationRuleCollector1)));
      Assert.That (
          _removingObjectValidationRuleCollector.Validators.ElementAt (1).RemovingObjectValidationRuleCollector,
          Is.SameAs (_removingObjectValidationRuleCollector));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void RegisterValidator_ValidatorTypeDoesNotImplementIObjectValidator_ThrowsArgumentException ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void RegisterValidator_CollectorTypeDoesNotImplementIValidationRuleCollector_ThrowsArgumentException ()
    {
    }

    [Test]
    public void To_String ()
    {
      var result = _removingObjectValidationRuleCollector.ToString();

      Assert.That (
          result,
          Is.EqualTo ("RemovingObjectValidationRuleCollector: Remotion.Validation.UnitTests.TestDomain.Customer"));
    }
  }
}