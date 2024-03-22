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
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.MetaValidation
{
  [TestFixture]
  public class ObjectMetaValidationRuleValidatorTest
  {
    private Mock<IValidationRuleCollector> _collectorStub1;
    private Mock<IValidationRuleCollector> _collectorStub2;
    private ObjectMetaValidationRuleValidator _validator;
    private Mock<IObjectMetaValidationRuleCollector> _objectMetaValidationRuleCollectorStub1;
    private Mock<IObjectMetaValidationRuleCollector> _objectMetaValidationRuleCollectorStub2;
    private Mock<IObjectMetaValidationRuleCollector> _objectMetaValidationRuleCollectorStub3;
    private Mock<IObjectMetaValidationRuleCollector> _objectMetaValidationRuleCollectorStub4;
    private Mock<IObjectValidator> _objectValidatorStub1;
    private Mock<IObjectValidator> _objectValidatorStub2;
    private Mock<IObjectValidator> _objectValidatorStub3;
    private Mock<IObjectValidator> _objectValidatorStub4;
    private Mock<IObjectValidator> _objectValidatorStub5;

    [SetUp]
    public void SetUp ()
    {
      _collectorStub1 = new Mock<IValidationRuleCollector>();
      _collectorStub2 = new Mock<IValidationRuleCollector>();

      _objectMetaValidationRuleCollectorStub1 = new Mock<IObjectMetaValidationRuleCollector>();
      _objectMetaValidationRuleCollectorStub2 = new Mock<IObjectMetaValidationRuleCollector>();
      _objectMetaValidationRuleCollectorStub3 = new Mock<IObjectMetaValidationRuleCollector>();
      _objectMetaValidationRuleCollectorStub4 = new Mock<IObjectMetaValidationRuleCollector>();

      _collectorStub1
          .Setup(stub => stub.ObjectMetaValidationRules)
          .Returns(new[] { _objectMetaValidationRuleCollectorStub1.Object, _objectMetaValidationRuleCollectorStub2.Object });
      _collectorStub2.Setup(stub => stub.ObjectMetaValidationRules).Returns(new[] { _objectMetaValidationRuleCollectorStub3.Object });

      _objectValidatorStub1 = new Mock<IObjectValidator>();
      _objectValidatorStub2 = new Mock<IObjectValidator>();
      _objectValidatorStub3 = new Mock<IObjectValidator>();
      _objectValidatorStub4 = new Mock<IObjectValidator>();
      _objectValidatorStub5 = new Mock<IObjectValidator>();

      _validator = new ObjectMetaValidationRuleValidator(
          new[]
          {
              _objectMetaValidationRuleCollectorStub1.Object, _objectMetaValidationRuleCollectorStub2.Object, _objectMetaValidationRuleCollectorStub3.Object,
              _objectMetaValidationRuleCollectorStub4.Object
          });
    }

    [Test]
    public void Validate ()
    {
      var filteredTypeStub = new Mock<ITypeInformation>();
      var objectRule1 = AddingObjectValidationRuleCollector.Create<Customer>(typeof(CustomerValidationRuleCollector1));
      var objectRule2 = AddingObjectValidationRuleCollector.Create<Person>(typeof(CustomerValidationRuleCollector1));
      var objectRule3 = AddingObjectValidationRuleCollector.Create<Person>(typeof(CustomerValidationRuleCollector2));
      var filteredObjectRule = new Mock<IAddingObjectValidationRuleCollector>();
      filteredObjectRule.Setup(_ => _.ValidatedType).Returns(filteredTypeStub.Object);
      filteredObjectRule.Setup(_ => _.Validators).Returns(new[] { new Mock<IObjectValidator>().Object });

      objectRule1.RegisterValidator(_ => _objectValidatorStub1.Object);
      objectRule1.RegisterValidator(_ => _objectValidatorStub2.Object);
      objectRule2.RegisterValidator(_ => _objectValidatorStub3.Object);
      objectRule2.RegisterValidator(_ => _objectValidatorStub4.Object);
      objectRule3.RegisterValidator(_ => _objectValidatorStub5.Object);

      var metaValidationRuleMock1 = new Mock<IObjectMetaValidationRule>(MockBehavior.Strict);
      var metaValidationRuleMock2 = new Mock<IObjectMetaValidationRule>(MockBehavior.Strict);
      var metaValidationRuleMock3 = new Mock<IObjectMetaValidationRule>(MockBehavior.Strict);
      var metaValidationRuleMock4 = new Mock<IObjectMetaValidationRule>(MockBehavior.Strict);

      _objectMetaValidationRuleCollectorStub1.Setup(stub => stub.ValidatedType).Returns(TypeAdapter.Create(typeof(Customer)));
      _objectMetaValidationRuleCollectorStub1
          .Setup(stub => stub.MetaValidationRules)
          .Returns(new[] { metaValidationRuleMock1.Object, metaValidationRuleMock2.Object });

      _objectMetaValidationRuleCollectorStub2.Setup(stub => stub.ValidatedType).Returns(TypeAdapter.Create(typeof(Person)));
      _objectMetaValidationRuleCollectorStub2.Setup(stub => stub.MetaValidationRules).Returns(new[] { metaValidationRuleMock3.Object });

      _objectMetaValidationRuleCollectorStub3.Setup(stub => stub.ValidatedType).Returns(TypeAdapter.Create(typeof(Person)));
      _objectMetaValidationRuleCollectorStub3.Setup(stub => stub.MetaValidationRules).Returns(new IObjectMetaValidationRule[0]);

      _objectMetaValidationRuleCollectorStub4.Setup(stub => stub.ValidatedType).Returns(TypeAdapter.Create(typeof(Employee)));
      _objectMetaValidationRuleCollectorStub4.Setup(stub => stub.MetaValidationRules).Returns(new[] { metaValidationRuleMock4.Object });

      var resultItem1 = MetaValidationRuleValidationResult.CreateValidResult();
      var resultItem2 = MetaValidationRuleValidationResult.CreateValidResult();
      var resultItem3 = MetaValidationRuleValidationResult.CreateInvalidResult("Error Mock 2");
      var resultItem4 = MetaValidationRuleValidationResult.CreateValidResult();

      metaValidationRuleMock1
          .Setup(mock => mock.Validate(new[] { _objectValidatorStub1.Object, _objectValidatorStub2.Object }))
          .Returns(new[] { resultItem1 })
          .Verifiable();
      metaValidationRuleMock2
          .Setup(mock => mock.Validate(new[] { _objectValidatorStub1.Object, _objectValidatorStub2.Object }))
          .Returns(new[] { resultItem2, resultItem3 })
          .Verifiable();
      metaValidationRuleMock3
          .Setup(mock => mock.Validate(new[] { _objectValidatorStub3.Object, _objectValidatorStub4.Object, _objectValidatorStub5.Object }))
          .Returns(new[] { resultItem4 })
          .Verifiable();
      metaValidationRuleMock4
          .Setup(mock => mock.Validate(new IObjectValidator[0]))
          .Returns(new MetaValidationRuleValidationResult[0])
          .Verifiable();

      var result = _validator.Validate(new[] { objectRule1, objectRule2, filteredObjectRule.Object, objectRule3 }).ToArray();

      metaValidationRuleMock1.Verify();
      metaValidationRuleMock2.Verify();
      metaValidationRuleMock3.Verify();
      Assert.That(result, Is.EquivalentTo(new[] { resultItem1, resultItem2, resultItem3, resultItem4 }));
    }
  }
}
