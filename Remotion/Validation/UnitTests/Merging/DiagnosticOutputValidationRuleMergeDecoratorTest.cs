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
using log4net.Appender;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using Moq;
using NUnit.Framework;
using Remotion.Logging;
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
    private MemoryAppender _memoryAppender;
    private Mock<IValidationRuleCollectorMerger> _wrappedMergerStub;
    private DiagnosticOutputValidationRuleMergeDecorator _diagnosticOutputValidationRuleMergeDecorator;
    private Mock<ILogContext> _logContextStub;
    private Mock<IValidatorFormatter> _validatorFormatterStub;

    [SetUp]
    public void SetUp ()
    {
      _memoryAppender = new MemoryAppender();
      var hierarchy = new Hierarchy();
      ((IBasicRepositoryConfigurator)hierarchy).Configure(_memoryAppender);
      var logger = hierarchy.GetLogger("The Name");
      var log = new Log4NetLog(logger);
      var logManagerStub = new Mock<ILogManager>();
      logManagerStub.Setup(stub => stub.GetLogger(typeof(DiagnosticOutputValidationRuleMergeDecorator))).Returns(log);

      _logContextStub = new Mock<ILogContext>();
      _wrappedMergerStub = new Mock<IValidationRuleCollectorMerger>();
      _validatorFormatterStub = new Mock<IValidatorFormatter>();

      _diagnosticOutputValidationRuleMergeDecorator =
          new DiagnosticOutputValidationRuleMergeDecorator(_wrappedMergerStub.Object, _validatorFormatterStub.Object, logManagerStub.Object);
    }

    [Test]
    public void Merge_NoValidationCollectors ()
    {
      var collectors = Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>();
      _wrappedMergerStub
          .Setup(stub => stub.Merge(collectors))
          .Returns(new ValidationCollectorMergeResult(new IAddingPropertyValidationRuleCollector[0], new IAddingObjectValidationRuleCollector[0], _logContextStub.Object));

      CheckLoggingMethod(() => _diagnosticOutputValidationRuleMergeDecorator.Merge(collectors), "\r\nAFTER MERGE:", 0);
      CheckLoggingMethod(() => _diagnosticOutputValidationRuleMergeDecorator.Merge(collectors), "\r\nBEFORE MERGE:", 1);
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
          "\r\nAFTER MERGE:"
          + "\r\n"
          + "\r\n    -> Remotion.Validation.UnitTests.TestDomain.Customer#UserName"
          + "\r\n        VALIDATORS:"
          + "\r\n        -> NotNullValidator (x2)"
          + "\r\n        -> NotEmptyOrWhitespaceValidator (x1)"
          + "\r\n        MERGE LOG:"
          + "\r\n        -> 'NotEmptyOrWhitespaceValidator' was removed from collectors 'CustomerValidationRuleCollector1, CustomerValidationRuleCollector2'"
          + "\r\n        -> 'NotNullValidator' was removed from collector 'CustomerValidationRuleCollector2'"
          + "\r\n"
          + "\r\n    -> Remotion.Validation.UnitTests.TestDomain.Person#LastName"
          + "\r\n        VALIDATORS:"
          + "\r\n        -> NotEqualValidator (x1)"
          + "\r\n        MERGE LOG:"
          + "\r\n        -> 'NotEqualValidator' was removed from collector 'CustomerValidationRuleCollector1'"
          + "\r\n"
          + "\r\n    -> Remotion.Validation.UnitTests.Implementation.AddingPropertyValidationRuleCollectorStub+DomainType#DomainProperty"
          + "\r\n        VALIDATORS:"
          + "\r\n        -> StubPropertyValidator (x1)";
      //TODO RM-5906: test IObjectValidator
      CheckLoggingMethod(() => _diagnosticOutputValidationRuleMergeDecorator.Merge(validationCollectorInfos), expectedAfterMerge, 0);

      var expectedBeforeMerge =
          "\r\nBEFORE MERGE:"
          + "\r\n"
          + "\r\n-> ValidationAttributesBasedValidationRuleCollectorProvider#TypeWithoutBaseTypeCollector1"
          + "\r\n"
          + "\r\n    -> Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseType#Property1"
          + "\r\n        ADDED NON-REMOVABLE VALIDATORS:"
          + "\r\n        -> NotNullValidator (x1)"
          + "\r\n        -> NotEqualValidator (x1)"
          + "\r\n"
          + "\r\n    -> Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseType#Property2"
          + "\r\n        ADDED REMOVABLE VALIDATORS:"
          + "\r\n        -> MaximumLengthValidator (x1)"
          + "\r\n        ADDED META VALIDATION RULES:"
          + "\r\n        -> MaxLengthPropertyMetaValidationRule"
          + "\r\n"
          + "\r\n-> ApiBasedValidationRuleCollectorProvider#TypeWithoutBaseTypeCollector2"
          + "\r\n"
          + "\r\n    -> Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseType#Property2"
          + "\r\n        REMOVED VALIDATORS:"
          + "\r\n        -> NotEmptyOrWhitespaceValidator#Conditional (x1)"
          + "\r\n        -> MaximumLengthValidator#TypeWithoutBaseTypeCollector1 (x1)";
      //TODO RM-5906: test IObjectValidator
      CheckLoggingMethod(() => _diagnosticOutputValidationRuleMergeDecorator.Merge(validationCollectorInfos), expectedBeforeMerge, 1);
    }

    private IEnumerable<LoggingEvent> GetLoggingEvents ()
    {
      return _memoryAppender.GetEvents();
    }

    private void CheckLoggingMethod (Action action, string expectedMessage, int loggingEventIndex)
    {
      action();
      var loggingEvents = GetLoggingEvents().ToArray();

      Assert.That(loggingEvents[loggingEventIndex].RenderedMessage, Is.EqualTo(expectedMessage));
    }
  }
}
