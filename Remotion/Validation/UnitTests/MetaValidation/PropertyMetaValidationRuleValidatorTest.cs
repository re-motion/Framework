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
using Remotion.Utilities;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.MetaValidation
{
  [TestFixture]
  public class PropertyMetaValidationRuleValidatorTest
  {
    private Mock<IValidationRuleCollector> _collectorStub1;
    private Mock<IValidationRuleCollector> _collectorStub2;
    private PropertyMetaValidationRuleValidator _validator;
    private Mock<IPropertyMetaValidationRuleCollector> _propertyMetaValidationRuleCollectorStub1;
    private Mock<IPropertyMetaValidationRuleCollector> _propertyMetaValidationRuleCollectorStub2;
    private Mock<IPropertyMetaValidationRuleCollector> _propertyMetaValidationRuleCollectorStub3;
    private Mock<IPropertyMetaValidationRuleCollector> _propertyMetaValidationRuleCollectorStub4;
    private Mock<IPropertyValidator> _propertyValidatorStub1;
    private Mock<IPropertyValidator> _propertyValidatorStub2;
    private Mock<IPropertyValidator> _propertyValidatorStub3;
    private Mock<IPropertyValidator> _propertyValidatorStub4;
    private Mock<IPropertyValidator> _propertyValidatorStub5;
    private Mock<ISystemPropertyMetaValidationRuleProvider> _systemPropertyMetaValidationRuleProviderStub;
    private Mock<ISystemPropertyMetaValidationRuleProviderFactory> _systemPropertyMetaRuleProviderFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _collectorStub1 = new Mock<IValidationRuleCollector>();
      _collectorStub2 = new Mock<IValidationRuleCollector>();

      _propertyMetaValidationRuleCollectorStub1 = new Mock<IPropertyMetaValidationRuleCollector>();
      _propertyMetaValidationRuleCollectorStub2 = new Mock<IPropertyMetaValidationRuleCollector>();
      _propertyMetaValidationRuleCollectorStub3 = new Mock<IPropertyMetaValidationRuleCollector>();
      _propertyMetaValidationRuleCollectorStub4 = new Mock<IPropertyMetaValidationRuleCollector>();

      _collectorStub1
          .Setup(stub => stub.PropertyMetaValidationRules)
          .Returns(new[] { _propertyMetaValidationRuleCollectorStub1.Object, _propertyMetaValidationRuleCollectorStub2.Object });
      _collectorStub2.Setup(stub => stub.PropertyMetaValidationRules).Returns(new[] { _propertyMetaValidationRuleCollectorStub3.Object });

      _propertyValidatorStub1 = new Mock<IPropertyValidator>();
      _propertyValidatorStub2 = new Mock<IPropertyValidator>();
      _propertyValidatorStub3 = new Mock<IPropertyValidator>();
      _propertyValidatorStub4 = new Mock<IPropertyValidator>();
      _propertyValidatorStub5 = new Mock<IPropertyValidator>();

      _systemPropertyMetaRuleProviderFactoryStub = new Mock<ISystemPropertyMetaValidationRuleProviderFactory>();
      _systemPropertyMetaValidationRuleProviderStub = new Mock<ISystemPropertyMetaValidationRuleProvider>();

      _validator =
          new PropertyMetaValidationRuleValidator(
              new[]
              {
                  _propertyMetaValidationRuleCollectorStub1.Object, _propertyMetaValidationRuleCollectorStub2.Object,
                  _propertyMetaValidationRuleCollectorStub3.Object, _propertyMetaValidationRuleCollectorStub4.Object
              },
              _systemPropertyMetaRuleProviderFactoryStub.Object);
    }

    [Test]
    public void Validate ()
    {
      var userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.UserName);
      var lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Person, string>(c => c.LastName);
      var otherPropertyExpression = ExpressionHelper.GetTypedMemberExpression<Person, DateTime>(c => c.Birthday);
      var filteredPropertyStub = new Mock<IPropertyInformation>();

      var propertyRule1 = AddingPropertyValidationRuleCollector.Create(userNameExpression, typeof(CustomerValidationRuleCollector1));
      var propertyRule2 = AddingPropertyValidationRuleCollector.Create(lastNameExpression, typeof(CustomerValidationRuleCollector1));
      var propertyRule3 = AddingPropertyValidationRuleCollector.Create(lastNameExpression, typeof(CustomerValidationRuleCollector2));
      var filteredPropertyRuleStub = new Mock<IAddingPropertyValidationRuleCollector>();
      filteredPropertyRuleStub.Setup(_ => _.Property).Returns(filteredPropertyStub.Object);
      filteredPropertyRuleStub.Setup(_ => _.Validators).Returns(new[] { new Mock<IPropertyValidator>().Object });

      propertyRule1.RegisterValidator(_ => _propertyValidatorStub1.Object);
      propertyRule1.RegisterValidator(_ => _propertyValidatorStub2.Object);
      propertyRule2.RegisterValidator(_ => _propertyValidatorStub3.Object);
      propertyRule2.RegisterValidator(_ => _propertyValidatorStub4.Object);
      propertyRule3.RegisterValidator(_ => _propertyValidatorStub5.Object);

      var systemMetaValidationRuleMock1 = new Mock<IPropertyMetaValidationRule>(MockBehavior.Strict);
      var systemMetaValidationRuleMock2 = new Mock<IPropertyMetaValidationRule>(MockBehavior.Strict);

      _systemPropertyMetaRuleProviderFactoryStub.Setup(stub => stub.Create(It.IsAny<IPropertyInformation>())).Returns(_systemPropertyMetaValidationRuleProviderStub.Object);
      _systemPropertyMetaValidationRuleProviderStub
          .Setup(stub => stub.GetSystemPropertyMetaValidationRules())
          .Returns(new[] { systemMetaValidationRuleMock1.Object, systemMetaValidationRuleMock2.Object });

      var metaValidationRuleMock1 = new Mock<IPropertyMetaValidationRule>(MockBehavior.Strict);
      var metaValidationRuleMock2 = new Mock<IPropertyMetaValidationRule>(MockBehavior.Strict);
      var metaValidationRuleMock3 = new Mock<IPropertyMetaValidationRule>(MockBehavior.Strict);
      var metaValidationRuleMock4 = new Mock<IPropertyMetaValidationRule>(MockBehavior.Strict);

      _propertyMetaValidationRuleCollectorStub1.Setup(stub => stub.Property).Returns(PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty(userNameExpression)));
      _propertyMetaValidationRuleCollectorStub1.Setup(stub => stub.MetaValidationRules).Returns(new[] { metaValidationRuleMock1.Object, metaValidationRuleMock2.Object });

      _propertyMetaValidationRuleCollectorStub2.Setup(stub => stub.Property).Returns(PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty(lastNameExpression)));
      _propertyMetaValidationRuleCollectorStub2.Setup(stub => stub.MetaValidationRules).Returns(new[] { metaValidationRuleMock3.Object });

      _propertyMetaValidationRuleCollectorStub3.Setup(stub => stub.Property).Returns(PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty(lastNameExpression)));
      _propertyMetaValidationRuleCollectorStub3.Setup(stub => stub.MetaValidationRules).Returns(new IPropertyMetaValidationRule[0]);

      _propertyMetaValidationRuleCollectorStub4
          .Setup(stub => stub.Property).Returns(PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty(otherPropertyExpression)));
      _propertyMetaValidationRuleCollectorStub4
          .Setup(stub => stub.MetaValidationRules).Returns(new[] { metaValidationRuleMock4.Object });

      systemMetaValidationRuleMock1
          .Setup(mock => mock.Validate(new[] { _propertyValidatorStub1.Object, _propertyValidatorStub2.Object }))
          .Returns(new[] { MetaValidationRuleValidationResult.CreateInvalidResult("Error System Mock 1") })
          .Verifiable();
      systemMetaValidationRuleMock2
          .Setup(mock => mock.Validate(new[] { _propertyValidatorStub1.Object, _propertyValidatorStub2.Object }))
          .Returns(new[] { MetaValidationRuleValidationResult.CreateValidResult() })
          .Verifiable();
      metaValidationRuleMock1
          .Setup(mock => mock.Validate(new[] { _propertyValidatorStub1.Object, _propertyValidatorStub2.Object }))
          .Returns(new[] { MetaValidationRuleValidationResult.CreateValidResult() })
          .Verifiable();
      metaValidationRuleMock2
          .Setup(mock => mock.Validate(new[] { _propertyValidatorStub1.Object, _propertyValidatorStub2.Object }))
          .Returns(new[] { MetaValidationRuleValidationResult.CreateValidResult(), MetaValidationRuleValidationResult.CreateInvalidResult("Error Mock 2") })
          .Verifiable();

      systemMetaValidationRuleMock1
          .Setup(mock => mock.Validate(new[] { _propertyValidatorStub3.Object, _propertyValidatorStub4.Object, _propertyValidatorStub5.Object }))
          .Returns(new[] { MetaValidationRuleValidationResult.CreateValidResult() })
          .Verifiable();
      systemMetaValidationRuleMock2
          .Setup(mock => mock.Validate(new[] { _propertyValidatorStub3.Object, _propertyValidatorStub4.Object, _propertyValidatorStub5.Object }))
          .Returns(new[] { MetaValidationRuleValidationResult.CreateValidResult() })
          .Verifiable();
      metaValidationRuleMock3
          .Setup(mock => mock.Validate(new[] { _propertyValidatorStub3.Object, _propertyValidatorStub4.Object, _propertyValidatorStub5.Object }))
          .Returns(new[] { MetaValidationRuleValidationResult.CreateValidResult() })
          .Verifiable();

      systemMetaValidationRuleMock1
          .Setup(mock =>mock.Validate(new IPropertyValidator[0]))
          .Returns(new MetaValidationRuleValidationResult[0])
          .Verifiable();
      systemMetaValidationRuleMock2
          .Setup(mock =>mock.Validate(new IPropertyValidator[0]))
          .Returns(new MetaValidationRuleValidationResult[0])
          .Verifiable();
      metaValidationRuleMock4
          .Setup(mock => mock.Validate(new IPropertyValidator[0]))
          .Returns(new MetaValidationRuleValidationResult[0])
          .Verifiable();

      var result = _validator.Validate(new [] { propertyRule1, propertyRule2, filteredPropertyRuleStub.Object, propertyRule3 }).ToArray();

      systemMetaValidationRuleMock1.Verify();
      systemMetaValidationRuleMock2.Verify();
      metaValidationRuleMock1.Verify();
      metaValidationRuleMock2.Verify();
      metaValidationRuleMock3.Verify();
      Assert.That(result.Count(), Is.EqualTo(8));
      Assert.That(result[0].IsValid, Is.False);
      Assert.That(result[1].IsValid, Is.True);
      Assert.That(result[2].IsValid, Is.True);
      Assert.That(result[3].IsValid, Is.True);
      Assert.That(result[4].IsValid, Is.False);
      Assert.That(result[5].IsValid, Is.True);
      Assert.That(result[6].IsValid, Is.True);
      Assert.That(result[7].IsValid, Is.True);
    }
  }
}
