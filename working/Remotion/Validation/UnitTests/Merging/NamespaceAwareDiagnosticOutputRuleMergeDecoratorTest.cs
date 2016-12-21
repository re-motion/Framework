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
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using NUnit.Framework;
using Remotion.Logging;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.Providers;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.Implementation;
using Remotion.Validation.UnitTests.Implementation.TestDomain;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Rhino.Mocks;
using LogManager = log4net.LogManager;

namespace Remotion.Validation.UnitTests.Merging
{
  [TestFixture]
  public class NamespaceAwareDiagnosticOutputRuleMergeDecoratorTest
  {
    private MemoryAppender _memoryAppender;
    private ILogContext _logContextStub;
    private IValidationCollectorMerger _wrappedMergerStub;
    private NamespaceAwareDiagnosticOutputRuleMergeDecorator _diagnosticOutputRuleMergeDecorator;
    private IValidatorFormatter _validatorFormatterStub;

    [SetUp]
    public void SetUp ()
    {
      _memoryAppender = new MemoryAppender();
      var hierarchy = new Hierarchy();
      ((IBasicRepositoryConfigurator) hierarchy).Configure (_memoryAppender);
      var logger = hierarchy.GetLogger ("The Name");
      var log = new Log4NetLog (logger);
      var logManagerStub = MockRepository.GenerateStub<ILogManager>();
      logManagerStub.Stub (stub => stub.GetLogger (typeof (DiagnosticOutputRuleMergeDecorator))).Return (log);

      _logContextStub = MockRepository.GenerateStub<ILogContext>();
      _wrappedMergerStub = MockRepository.GenerateStub<IValidationCollectorMerger>();
      _validatorFormatterStub = MockRepository.GenerateStub<IValidatorFormatter>();

      _diagnosticOutputRuleMergeDecorator = new NamespaceAwareDiagnosticOutputRuleMergeDecorator (
          _wrappedMergerStub,
          _validatorFormatterStub,
          logManagerStub);
    }

    [Test]
    public void Merge_NoValidationCollectors ()
    {
      var collectors = Enumerable.Empty<IEnumerable<ValidationCollectorInfo>>();
      _wrappedMergerStub.Stub (stub => stub.Merge (collectors)).Return (new ValidationCollectorMergeResult(new IValidationRule[0], _logContextStub));

      CheckLoggingMethod (() => _diagnosticOutputRuleMergeDecorator.Merge (collectors), "\r\nAFTER MERGE:", 0);
      CheckLoggingMethod (() => _diagnosticOutputRuleMergeDecorator.Merge (collectors), "\r\nBEFORE MERGE:", 1);
    }

    [Test]
    public void Merge_WithValidationCollectors ()
    {
      var collector1 = new TypeWithoutBaseTypeCollector1();
      var collector2 = new TypeWithoutBaseTypeCollector2();
      var validationCollectorInfos = new[]
                                     {
                                         new[]
                                         {
                                             new ValidationCollectorInfo (
                                                 collector1,
                                                 typeof (ValidationAttributesBasedCollectorProvider))
                                         },
                                         new[] { new ValidationCollectorInfo (collector2, typeof (ApiBasedComponentValidationCollectorProvider)) }
                                     };

      var userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.UserName);
      var lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName);
      var stubValidator1 = new NotNullValidator();
      var stubValidator2 = new NotEmptyValidator (null);
      var stubValidator3 = new NotEqualValidator ("test");
      var stubValidator4 = new StubPropertyValidator();

      _validatorFormatterStub.Stub (
          stub => stub.Format (Arg<IPropertyValidator>.Matches (c => c.GetType() == typeof (NotNullValidator)), Arg<Func<Type, string>>.Is.Anything))
          .Return ("FluentValidation.Validators.NotNullValidator");
      _validatorFormatterStub.Stub (
          stub => stub.Format (Arg<IPropertyValidator>.Matches (c => c.GetType() == typeof (LengthValidator)), Arg<Func<Type, string>>.Is.Anything))
          .Return ("FluentValidation.Validators.LengthValidator");
      _validatorFormatterStub.Stub (
          stub => stub.Format (Arg<IPropertyValidator>.Matches (c => c.GetType() == typeof (NotEmptyValidator)), Arg<Func<Type, string>>.Is.Anything))
          .Return ("FluentValidation.Validators.NotEmptyValidator");
      _validatorFormatterStub.Stub (
          stub => stub.Format (Arg<IPropertyValidator>.Matches (c => c.GetType() == typeof (NotEqualValidator)), Arg<Func<Type, string>>.Is.Anything))
          .Return ("FluentValidation.Validators.NotEqualValidator");
      _validatorFormatterStub.Stub (
          stub =>
              stub.Format (Arg<IPropertyValidator>.Matches (c => c.GetType() == typeof (StubPropertyValidator)), Arg<Func<Type, string>>.Is.Anything))
          .Return ("Remotion.Validation.UnitTests.TestHelpers.StubPropertyValidator");

      var userNamePropertyRule = PropertyRule.Create (userNameExpression);
      userNamePropertyRule.AddValidator (stubValidator1);
      userNamePropertyRule.AddValidator (stubValidator1);
      userNamePropertyRule.AddValidator (stubValidator2);
      var lastNamePropertyRule = PropertyRule.Create (lastNameExpression);
      lastNamePropertyRule.AddValidator (stubValidator3);

      var noPropertyRuleStub = new ValidationRuleStub();
      noPropertyRuleStub.AddValidator (stubValidator4);

      var removingPropertyRuleStub1 = MockRepository.GenerateStub<IRemovingComponentPropertyRule>();
      removingPropertyRuleStub1.Stub (stub => stub.CollectorType).Return (typeof (CustomerValidationCollector1));
      var removingPropertyRuleStub2 = MockRepository.GenerateStub<IRemovingComponentPropertyRule>();
      removingPropertyRuleStub2.Stub (stub => stub.CollectorType).Return (typeof (CustomerValidationCollector2));

      var logContextInfo1 = new LogContextInfo (
          stubValidator2,
          new[]
          {
              new ValidatorRegistrationWithContext (new ValidatorRegistration (typeof (NotEmptyValidator), null), removingPropertyRuleStub1),
              new ValidatorRegistrationWithContext (new ValidatorRegistration (typeof (NotEmptyValidator), null), removingPropertyRuleStub1),
              new ValidatorRegistrationWithContext (new ValidatorRegistration (typeof (NotEmptyValidator), null), removingPropertyRuleStub2)
          });
      var logContextInfo2 = new LogContextInfo (
          stubValidator1,
          new[] { new ValidatorRegistrationWithContext (new ValidatorRegistration (typeof (NotNullValidator), null), removingPropertyRuleStub2) });
      var logContextInfo3 = new LogContextInfo (
          stubValidator3,
          new[] { new ValidatorRegistrationWithContext (new ValidatorRegistration (typeof (NotEqualValidator), null), removingPropertyRuleStub1) });

      _logContextStub.Stub (stub => stub.GetLogContextInfos (userNamePropertyRule)).Return (new[] { logContextInfo1, logContextInfo2 });
      _logContextStub.Stub (stub => stub.GetLogContextInfos (lastNamePropertyRule)).Return (new[] { logContextInfo3 });
      _logContextStub.Stub (stub => stub.GetLogContextInfos (noPropertyRuleStub)).Return (new LogContextInfo[0]);

      var fakeValidationRules = new IValidationRule[] { userNamePropertyRule, lastNamePropertyRule, noPropertyRuleStub };
      _wrappedMergerStub.Stub (
          stub =>
              stub.Merge (
                  validationCollectorInfos)).Return (new ValidationCollectorMergeResult(fakeValidationRules, _logContextStub));

      var expectedAfterMerge =
          "\r\nAFTER MERGE:"
          + "\r\n\r\n    -> Remotion.Validation.UnitTests.TestDomain.Customer#UserName"
          + "\r\n        VALIDATORS:"
          + "\r\n        -> FluentValidation.Validators.NotNullValidator (x2)"
          + "\r\n        -> FluentValidation.Validators.NotEmptyValidator (x1)"
          + "\r\n        MERGE LOG:"
          + "\r\n        -> 'FluentValidation.Validators.NotEmptyValidator' was removed from collectors 'CustomerValidationCollector1, CustomerValidationCollector2'"
          + "\r\n        -> 'FluentValidation.Validators.NotNullValidator' was removed from collector 'CustomerValidationCollector2'"
          + "\r\n\r\n    -> Remotion.Validation.UnitTests.TestDomain.Person#LastName"
          + "\r\n        VALIDATORS:"
          + "\r\n        -> FluentValidation.Validators.NotEqualValidator (x1)"
          + "\r\n        MERGE LOG:"
          + "\r\n        -> 'FluentValidation.Validators.NotEqualValidator' was removed from collector 'CustomerValidationCollector1'"
          + "\r\n\r\n    -> Remotion.Validation.UnitTests.Implementation.ValidationRuleStub"
          + "\r\n        VALIDATORS:"
          + "\r\n        -> Remotion.Validation.UnitTests.TestHelpers.StubPropertyValidator (x1)";
      CheckLoggingMethod (() => _diagnosticOutputRuleMergeDecorator.Merge (validationCollectorInfos), expectedAfterMerge, 0);

      var expectedBeforeMerge =
          "\r\nBEFORE MERGE:"
          + "\r\n\r\n-> Remotion.Validation.Providers.ValidationAttributesBasedCollectorProvider#Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseTypeCollector1"
          + "\r\n\r\n    -> Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseType#Property1"
          + "\r\n        ADDED HARD CONSTRAINT VALIDATORS:"
          + "\r\n        -> FluentValidation.Validators.NotNullValidator (x1)"
          + "\r\n        -> FluentValidation.Validators.NotEqualValidator (x1)"
          + "\r\n\r\n    -> Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseType#Property2"
          + "\r\n        ADDED SOFT CONSTRAINT VALIDATORS:"
          + "\r\n        -> FluentValidation.Validators.LengthValidator (x1)"
          + "\r\n        ADDED META VALIDATION RULES:"
          + "\r\n        -> Remotion.Validation.UnitTests.TestDomain.ValidationRules.MaxLengthMetaValidationRule"
          + "\r\n\r\n-> Remotion.Validation.Providers.ApiBasedComponentValidationCollectorProvider#Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseTypeCollector2"
          + "\r\n\r\n    -> Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseType#Property2"
          + "\r\n        REMOVED VALIDATORS:"
          + "\r\n        -> FluentValidation.Validators.NotEmptyValidator (x1)"
          + "\r\n        -> FluentValidation.Validators.MaximumLengthValidator#Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseTypeCollector1 (x1)";
      CheckLoggingMethod (() => _diagnosticOutputRuleMergeDecorator.Merge (validationCollectorInfos), expectedBeforeMerge, 1);
    }

    private IEnumerable<LoggingEvent> GetLoggingEvents ()
    {
      return _memoryAppender.GetEvents();
    }

    private void CheckLoggingMethod (Action action, string expectedMessage, int loggingEventIndex)
    {
      action();
      var loggingEvents = GetLoggingEvents().ToArray();

      Assert.That (loggingEvents[loggingEventIndex].RenderedMessage, Is.EqualTo (expectedMessage));
    }
  }
}