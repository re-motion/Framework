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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.Providers;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.Implementation;
using Remotion.Validation.UnitTests.Implementation.TestDomain;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Merging
{
  [TestFixture]
  public class DiagnosticOutputValidationRuleMergeDecoratorTest
  {
    private FakeLogCollector _fakeLogCollector;
    private Mock<IValidationRuleCollectorMerger> _wrappedMergerStub;
    private DiagnosticOutputValidationRuleMergeDecorator _diagnosticOutputValidationRuleMergeDecorator;
    private Mock<ILogContext> _logContextStub;
    private Mock<IValidatorFormatter> _validatorFormatterStub;

    [SetUp]
    public void SetUp ()
    {
      _fakeLogCollector = new FakeLogCollector();
      FakeLoggerProvider fakeLoggerProvider = new FakeLoggerProvider(_fakeLogCollector);

      _logContextStub = new Mock<ILogContext>();
      _wrappedMergerStub = new Mock<IValidationRuleCollectorMerger>();
      _validatorFormatterStub = new Mock<IValidatorFormatter>();

      _diagnosticOutputValidationRuleMergeDecorator =
          new DiagnosticOutputValidationRuleMergeDecorator(_wrappedMergerStub.Object, _validatorFormatterStub.Object, new LoggerFactory(new[] { fakeLoggerProvider }));
    }

    [Test]
    public void Merge_NoValidationCollectors ()
    {
      var collectors = Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>();
      _wrappedMergerStub
          .Setup(stub => stub.Merge(collectors))
          .Returns(new ValidationCollectorMergeResult(new IAddingPropertyValidationRuleCollector[0], new IAddingObjectValidationRuleCollector[0], _logContextStub.Object));

      CheckLoggingMethod(() => _diagnosticOutputValidationRuleMergeDecorator.Merge(collectors), $"{Environment.NewLine}AFTER MERGE:", 0);
      CheckLoggingMethod(() => _diagnosticOutputValidationRuleMergeDecorator.Merge(collectors), $"{Environment.NewLine}BEFORE MERGE:", 1);
    }

    [Test]
    public void Merge_WithValidationCollectors ()
    {
      var collector1 = new TypeWithoutBaseTypeCollector1();
      var collector2 = new TypeWithoutBaseTypeCollector2();
      var validationCollectorInfos =
          new[]
          {
              new[] { new ValidationRuleCollectorInfo(collector1, typeof(ValidationAttributesBasedValidationRuleCollectorProvider)) },
              new[] { new ValidationRuleCollectorInfo(collector2, typeof(ApiBasedValidationRuleCollectorProvider)) }
          };

      var userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.UserName);
      var lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.LastName);
      var stubValidator1 = new NotNullValidator(new InvariantValidationMessage("Fake Message"));
      var stubValidator2 = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Fake Message"));
      var stubValidator3 = new NotEqualValidator("test", new InvariantValidationMessage("Fake Message"));
      var stubValidator4 = new StubPropertyValidator();
      var stubValidator5 = new NotNullValidator(new InvariantValidationMessage("Fake Message"));

      var userNamePropertyRule = AddingPropertyValidationRuleCollector.Create(userNameExpression, typeof(IValidationRuleCollector<>));
      userNamePropertyRule.RegisterValidator(_ => stubValidator1);
      userNamePropertyRule.RegisterValidator(_ => stubValidator5);
      userNamePropertyRule.RegisterValidator(_ => stubValidator2);
      var lastNamePropertyRule = AddingPropertyValidationRuleCollector.Create(lastNameExpression, typeof(IValidationRuleCollector<>));
      lastNamePropertyRule.RegisterValidator(_ => stubValidator3);

      var noPropertyRuleStub = new AddingPropertyValidationRuleCollectorStub();
      noPropertyRuleStub.RegisterValidator(_ => stubValidator4);

      var removingPropertyRuleStub1 = new Mock<IRemovingPropertyValidationRuleCollector>();
      removingPropertyRuleStub1.Setup(stub => stub.CollectorType).Returns(typeof(CustomerValidationRuleCollector1));
      var removingPropertyRuleStub2 = new Mock<IRemovingPropertyValidationRuleCollector>();
      removingPropertyRuleStub2.Setup(stub => stub.CollectorType).Returns(typeof(CustomerValidationRuleCollector2));

      var logContextInfo1 = new PropertyValidatorLogContextInfo(
          stubValidator2,
          new[]
          {
              new RemovingPropertyValidatorRegistration(typeof(NotEmptyOrWhitespaceValidator), null, null, removingPropertyRuleStub1.Object),
              new RemovingPropertyValidatorRegistration(typeof(NotEmptyOrWhitespaceValidator), null, null, removingPropertyRuleStub1.Object),
              new RemovingPropertyValidatorRegistration(typeof(NotEmptyOrWhitespaceValidator), null, null, removingPropertyRuleStub2.Object)
          });
      var logContextInfo2 = new PropertyValidatorLogContextInfo(
          stubValidator1,
          new[]
          { new RemovingPropertyValidatorRegistration(typeof(NotNullValidator), null, null, removingPropertyRuleStub2.Object) });
      var logContextInfo3 = new PropertyValidatorLogContextInfo(
          stubValidator3,
          new[]
          { new RemovingPropertyValidatorRegistration(typeof(NotEqualValidator), null, null, removingPropertyRuleStub1.Object) });

      _validatorFormatterStub
          .Setup(stub => stub.Format(It.Is<IPropertyValidator>(c => c.GetType() == typeof(NotNullValidator)), It.IsAny<Func<Type, string>>()))
          .Returns("NotNullValidator");
      _validatorFormatterStub
          .Setup(stub => stub.Format(It.Is<IPropertyValidator>(c => c.GetType() == typeof(MaximumLengthValidator)), It.IsAny<Func<Type, string>>()))
          .Returns("MaximumLengthValidator");
      _validatorFormatterStub
          .Setup(stub => stub.Format(It.Is<IPropertyValidator>(c => c.GetType() == typeof(NotEmptyOrWhitespaceValidator)), It.IsAny<Func<Type, string>>()))
          .Returns("NotEmptyOrWhitespaceValidator");
      _validatorFormatterStub
          .Setup(stub => stub.Format(It.Is<IPropertyValidator>(c => c.GetType() == typeof(NotEqualValidator)), It.IsAny<Func<Type, string>>()))
          .Returns("NotEqualValidator");
      _validatorFormatterStub
          .Setup(stub => stub.Format(It.Is<IPropertyValidator>(c => c.GetType() == typeof(StubPropertyValidator)), It.IsAny<Func<Type, string>>()))
          .Returns("StubPropertyValidator");

      _logContextStub.Setup(stub => stub.GetLogContextInfos(userNamePropertyRule)).Returns(new[] { logContextInfo1, logContextInfo2 });
      _logContextStub.Setup(stub => stub.GetLogContextInfos(lastNamePropertyRule)).Returns(new[] { logContextInfo3 });
      _logContextStub.Setup(stub => stub.GetLogContextInfos(noPropertyRuleStub)).Returns(new PropertyValidatorLogContextInfo[0]);

      var addingPropertyValidationRuleCollectors = new IAddingPropertyValidationRuleCollector[] { userNamePropertyRule, lastNamePropertyRule, noPropertyRuleStub };
      var addingObjectValidationRuleCollectors = new IAddingObjectValidationRuleCollector[]
                                                 {
                                                     /* TODO RM-5906: test object rules */
                                                 };
      _wrappedMergerStub
          .Setup(stub => stub.Merge(validationCollectorInfos))
          .Returns(new ValidationCollectorMergeResult(addingPropertyValidationRuleCollectors, addingObjectValidationRuleCollectors, _logContextStub.Object));

      var expectedAfterMerge =
          """

          AFTER MERGE:

              -> Remotion.Validation.UnitTests.TestDomain.Customer#UserName
                  VALIDATORS:
                  -> NotNullValidator (x2)
                  -> NotEmptyOrWhitespaceValidator (x1)
                  MERGE LOG:
                  -> 'NotEmptyOrWhitespaceValidator' was removed from collectors 'CustomerValidationRuleCollector1, CustomerValidationRuleCollector2'
                  -> 'NotNullValidator' was removed from collector 'CustomerValidationRuleCollector2'

              -> Remotion.Validation.UnitTests.TestDomain.Person#LastName
                  VALIDATORS:
                  -> NotEqualValidator (x1)
                  MERGE LOG:
                  -> 'NotEqualValidator' was removed from collector 'CustomerValidationRuleCollector1'

              -> Remotion.Validation.UnitTests.Implementation.AddingPropertyValidationRuleCollectorStub+DomainType#DomainProperty
                  VALIDATORS:
                  -> StubPropertyValidator (x1)
          """.ReplaceLineEndings();
      //TODO RM-5906: test IObjectValidator
      CheckLoggingMethod(() => _diagnosticOutputValidationRuleMergeDecorator.Merge(validationCollectorInfos), expectedAfterMerge, 0);

      var expectedBeforeMerge =
          """

          BEFORE MERGE:

          -> ValidationAttributesBasedValidationRuleCollectorProvider#TypeWithoutBaseTypeCollector1

              -> Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseType#Property1
                  ADDED NON-REMOVABLE VALIDATORS:
                  -> NotNullValidator (x1)
                  -> NotEqualValidator (x1)

              -> Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseType#Property2
                  ADDED REMOVABLE VALIDATORS:
                  -> MaximumLengthValidator (x1)
                  ADDED META VALIDATION RULES:
                  -> MaxLengthPropertyMetaValidationRule

          -> ApiBasedValidationRuleCollectorProvider#TypeWithoutBaseTypeCollector2

              -> Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseType#Property2
                  REMOVED VALIDATORS:
                  -> NotEmptyOrWhitespaceValidator#Conditional (x1)
                  -> MaximumLengthValidator#TypeWithoutBaseTypeCollector1 (x1)
          """.ReplaceLineEndings();
      //TODO RM-5906: test IObjectValidator
      CheckLoggingMethod(() => _diagnosticOutputValidationRuleMergeDecorator.Merge(validationCollectorInfos), expectedBeforeMerge, 1);
    }

    private void CheckLoggingMethod (Action action, string expectedMessage, int loggingEventIndex)
    {
      action();
      var loggingEvents = _fakeLogCollector.GetSnapshot();

      Assert.That(loggingEvents[loggingEventIndex].Message, Is.EqualTo(expectedMessage));
    }
  }
}
