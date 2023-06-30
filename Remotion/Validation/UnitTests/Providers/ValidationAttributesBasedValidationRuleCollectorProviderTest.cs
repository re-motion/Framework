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
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Providers
{
  [TestFixture]
  public class ValidationAttributesBasedValidationRuleCollectorProviderTest
  {
    private TestableValidationAttributesBasedValidationRuleCollectorProvider _provider;
    private Mock<IValidationMessageFactory> _validationMessageFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _validationMessageFactoryStub = new Mock<IValidationMessageFactory>();
      _provider = new TestableValidationAttributesBasedValidationRuleCollectorProvider(_validationMessageFactoryStub.Object);
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void GetValidationRuleCollectors_TODO_RM_5906 ()
    {
    }

    [Test]
    public void GetValidationRuleCollectors_SetValidationMessageFromFactory ()
    {
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsNotNull<IPropertyValidator>(),
                  It.IsNotNull<IPropertyInformation>()))
          .Returns(validationMessageStub.Object);

      var collectors = _provider.GetValidationRuleCollectors(new[] { typeof(Customer) });
      Assert.That(collectors, Is.Not.Empty);

      var addingComponentPropertyRules = collectors.SelectMany(c => c.ToArray()).SelectMany(c => c.Collector.AddedPropertyRules).ToArray();
      Assert.That(addingComponentPropertyRules, Is.Not.Empty);
      Assert.That(addingComponentPropertyRules.OfType<AddingPropertyValidationRuleCollector<Customer, string>>().Count(), Is.EqualTo(2));
      Assert.That(addingComponentPropertyRules.OfType<AddingPropertyValidationRuleCollector<Customer, Address>>(), Is.Empty);
      Assert.That(addingComponentPropertyRules.OfType<AddingPropertyValidationRuleCollector<Customer, ICollection<Address>>>(), Is.Empty);

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");
      var validators = addingComponentPropertyRules.SelectMany(r => r.Validators).ToArray();
      Assert.That(validators, Is.Not.Empty);
      Assert.That(validators, Is.All.InstanceOf<IPropertyValidator>());
      Assert.That(validators.OfType<MaximumLengthValidator>().First().ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
      Assert.That(validators.OfType<NotNullValidator>().First().ValidationMessage.ToString, Is.EqualTo("Stub Message"));
    }

    [Test]
    public void CreatePropertyRuleReflector ()
    {
      var result = _provider.CreatePropertyRuleReflectors(new [] { typeof(Customer) });

      Assert.That(result, Is.Not.Null);
      var propertyReflectors = result[typeof(Customer)].ToArray();
      Assert.That(propertyReflectors.Count(), Is.EqualTo(7));
      CollectionAssert.AllItemsAreInstancesOfType(propertyReflectors, typeof(ValidationAttributesBasedPropertyRuleReflector));
    }
  }
}
