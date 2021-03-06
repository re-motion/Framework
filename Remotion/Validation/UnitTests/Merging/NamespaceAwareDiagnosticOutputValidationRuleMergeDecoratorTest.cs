﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Merging
{
  [TestFixture]
  public class NamespaceAwareDiagnosticOutputValidationRuleMergeDecoratorTest
  {
    private MemoryAppender _memoryAppender;
    private ILogContext _logContextStub;
    private IValidationRuleCollectorMerger _wrappedMergerStub;
    private NamespaceAwareDiagnosticOutputValidationRuleMergeDecorator _diagnosticOutputValidationRuleMergeDecorator;
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
      logManagerStub.Stub (stub => stub.GetLogger (typeof (DiagnosticOutputValidationRuleMergeDecorator))).Return (log);

      _logContextStub = MockRepository.GenerateStub<ILogContext>();
      _wrappedMergerStub = MockRepository.GenerateStub<IValidationRuleCollectorMerger>();
      _validatorFormatterStub = MockRepository.GenerateStub<IValidatorFormatter>();

      _diagnosticOutputValidationRuleMergeDecorator = new NamespaceAwareDiagnosticOutputValidationRuleMergeDecorator (
          _wrappedMergerStub,
          _validatorFormatterStub,
          logManagerStub);
    }

    [Test]
    public void Merge_NoValidationCollectors ()
    {
      var collectors = Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>();
      _wrappedMergerStub
          .Stub (stub => stub.Merge (collectors))
          .Return (
              new ValidationCollectorMergeResult (
                  new IAddingPropertyValidationRuleCollector[0],
                  new IAddingObjectValidationRuleCollector[0],
                  _logContextStub));

      CheckLoggingMethod (() => _diagnosticOutputValidationRuleMergeDecorator.Merge (collectors), "\r\nAFTER MERGE:", 0);
      CheckLoggingMethod (() => _diagnosticOutputValidationRuleMergeDecorator.Merge (collectors), "\r\nBEFORE MERGE:", 1);
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
                                             new ValidationRuleCollectorInfo (
                                                 collector1,
                                                 typeof (ValidationAttributesBasedValidationRuleCollectorProvider))
                                         },
                                         new[] { new ValidationRuleCollectorInfo (collector2, typeof (ApiBasedValidationRuleCollectorProvider)) }
                                     };

      var userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.UserName);
      var lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName);
      var stubValidator1 = new NotNullValidator (new InvariantValidationMessage ("Fake Message"));
      var stubValidator2 = new NotEmptyValidator (new InvariantValidationMessage ("Fake Message"));
      var stubValidator3 = new NotEqualValidator ("test", new InvariantValidationMessage ("Fake Message"));
      var stubValidator4 = new StubPropertyValidator();

      _validatorFormatterStub.Stub (
          stub => stub.Format (Arg<IPropertyValidator>.Matches (c => c.GetType() == typeof (NotNullValidator)), Arg<Func<Type, string>>.Is.Anything))
          .Return ("Remotion.Validation.Validators.NotNullValidator");
      _validatorFormatterStub.Stub (
          stub => stub.Format (Arg<IPropertyValidator>.Matches (c => c.GetType() == typeof (LengthValidator)), Arg<Func<Type, string>>.Is.Anything))
          .Return ("Remotion.Validation.Validators.LengthValidator");
      _validatorFormatterStub.Stub (
          stub => stub.Format (Arg<IPropertyValidator>.Matches (c => c.GetType() == typeof (NotEmptyValidator)), Arg<Func<Type, string>>.Is.Anything))
          .Return ("Remotion.Validation.Validators.NotEmptyValidator");
      _validatorFormatterStub.Stub (
          stub => stub.Format (Arg<IPropertyValidator>.Matches (c => c.GetType() == typeof (NotEqualValidator)), Arg<Func<Type, string>>.Is.Anything))
          .Return ("Remotion.Validation.Validators.NotEqualValidator");
      _validatorFormatterStub.Stub (
          stub =>
              stub.Format (Arg<IPropertyValidator>.Matches (c => c.GetType() == typeof (StubPropertyValidator)), Arg<Func<Type, string>>.Is.Anything))
          .Return ("Remotion.Validation.UnitTests.TestHelpers.StubPropertyValidator");

      var userNamePropertyRule = AddingPropertyValidationRuleCollector.Create (userNameExpression, typeof (IValidationRuleCollector<>));
      userNamePropertyRule.RegisterValidator (_ => stubValidator1);
      userNamePropertyRule.RegisterValidator (_ => stubValidator1);
      userNamePropertyRule.RegisterValidator (_ => stubValidator2);
      var lastNamePropertyRule = AddingPropertyValidationRuleCollector.Create (lastNameExpression, typeof (IValidationRuleCollector<>));
      lastNamePropertyRule.RegisterValidator (_ => stubValidator3);

      var noPropertyRuleStub = new AddingPropertyValidationRuleCollectorStub();
      noPropertyRuleStub.RegisterValidator (_ => stubValidator4);

      var removingPropertyRuleStub1 = MockRepository.GenerateStub<IRemovingPropertyValidationRuleCollector>();
      removingPropertyRuleStub1.Stub (stub => stub.CollectorType).Return (typeof (CustomerValidationRuleCollector1));
      var removingPropertyRuleStub2 = MockRepository.GenerateStub<IRemovingPropertyValidationRuleCollector>();
      removingPropertyRuleStub2.Stub (stub => stub.CollectorType).Return (typeof (CustomerValidationRuleCollector2));

      var logContextInfo1 = new PropertyValidatorLogContextInfo (
          stubValidator2,
          new[]
          {
              new RemovingPropertyValidatorRegistration (typeof (NotEmptyValidator), null, null, removingPropertyRuleStub1),
              new RemovingPropertyValidatorRegistration (typeof (NotEmptyValidator), null, null, removingPropertyRuleStub1),
              new RemovingPropertyValidatorRegistration (typeof (NotEmptyValidator), null, null, removingPropertyRuleStub2)
          });
      var logContextInfo2 = new PropertyValidatorLogContextInfo (
          stubValidator1,
          new[] { new RemovingPropertyValidatorRegistration (typeof (NotNullValidator), null, null, removingPropertyRuleStub2) });
      var logContextInfo3 = new PropertyValidatorLogContextInfo (
          stubValidator3,
          new[] { new RemovingPropertyValidatorRegistration (typeof (NotEqualValidator), null, null, removingPropertyRuleStub1) });

      _logContextStub.Stub (stub => stub.GetLogContextInfos (userNamePropertyRule)).Return (new[] { logContextInfo1, logContextInfo2 });
      _logContextStub.Stub (stub => stub.GetLogContextInfos (lastNamePropertyRule)).Return (new[] { logContextInfo3 });
      _logContextStub.Stub (stub => stub.GetLogContextInfos (noPropertyRuleStub)).Return (new PropertyValidatorLogContextInfo[0]);

      var addingComponentPropertyRules = new IAddingPropertyValidationRuleCollector[] { userNamePropertyRule, lastNamePropertyRule, noPropertyRuleStub };
      var addingObjectValidationRuleCollectors = new IAddingObjectValidationRuleCollector[] { /* TODO RM-5906: test object rules */ };
      _wrappedMergerStub
          .Stub (stub => stub.Merge (validationCollectorInfos))
          .Return (new ValidationCollectorMergeResult (addingComponentPropertyRules, addingObjectValidationRuleCollectors, _logContextStub));

      var expectedAfterMerge =
          "\r\nAFTER MERGE:"
          + "\r\n"
          + "\r\n    -> Remotion.Validation.UnitTests.TestDomain.Customer#UserName"
          + "\r\n        VALIDATORS:"
          + "\r\n        -> Remotion.Validation.Validators.NotNullValidator (x2)"
          + "\r\n        -> Remotion.Validation.Validators.NotEmptyValidator (x1)"
          + "\r\n        MERGE LOG:"
          + "\r\n        -> 'Remotion.Validation.Validators.NotEmptyValidator' was removed from collectors 'CustomerValidationRuleCollector1, CustomerValidationRuleCollector2'"
          + "\r\n        -> 'Remotion.Validation.Validators.NotNullValidator' was removed from collector 'CustomerValidationRuleCollector2'"
          + "\r\n"
          + "\r\n    -> Remotion.Validation.UnitTests.TestDomain.Person#LastName"
          + "\r\n        VALIDATORS:"
          + "\r\n        -> Remotion.Validation.Validators.NotEqualValidator (x1)"
          + "\r\n        MERGE LOG:"
          + "\r\n        -> 'Remotion.Validation.Validators.NotEqualValidator' was removed from collector 'CustomerValidationRuleCollector1'"
          + "\r\n"
          + "\r\n    -> Remotion.Validation.UnitTests.Implementation.AddingPropertyValidationRuleCollectorStub+DomainType#DomainProperty"
          + "\r\n        VALIDATORS:"
          + "\r\n        -> Remotion.Validation.UnitTests.TestHelpers.StubPropertyValidator (x1)";
      //TODO RM-5906: test IObjectValidator
      CheckLoggingMethod (() => _diagnosticOutputValidationRuleMergeDecorator.Merge (validationCollectorInfos), expectedAfterMerge, 0);

      var expectedBeforeMerge =
          "\r\nBEFORE MERGE:"
          + "\r\n"
          + "\r\n-> Remotion.Validation.Providers.ValidationAttributesBasedValidationRuleCollectorProvider#Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseTypeCollector1"
          + "\r\n"
          + "\r\n    -> Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseType#Property1"
          + "\r\n        ADDED NON-REMOVABLE VALIDATORS:"
          + "\r\n        -> Remotion.Validation.Validators.NotNullValidator (x1)"
          + "\r\n        -> Remotion.Validation.Validators.NotEqualValidator (x1)"
          + "\r\n"
          + "\r\n    -> Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseType#Property2"
          + "\r\n        ADDED REMOVABLE VALIDATORS:"
          + "\r\n        -> Remotion.Validation.Validators.LengthValidator (x1)"
          + "\r\n        ADDED META VALIDATION RULES:"
          + "\r\n        -> Remotion.Validation.UnitTests.TestDomain.ValidationRules.MaxLengthPropertyMetaValidationRule"
          + "\r\n"
          + "\r\n-> Remotion.Validation.Providers.ApiBasedValidationRuleCollectorProvider#Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseTypeCollector2"
          + "\r\n"
          + "\r\n    -> Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseType#Property2"
          + "\r\n        REMOVED VALIDATORS:"
          + "\r\n        -> Remotion.Validation.Validators.NotEmptyValidator#Conditional (x1)"
          + "\r\n        -> Remotion.Validation.Validators.MaximumLengthValidator#Remotion.Validation.UnitTests.Implementation.TestDomain.TypeWithoutBaseTypeCollector1 (x1)";
      //TODO RM-5906: test IObjectValidator
      CheckLoggingMethod (() => _diagnosticOutputValidationRuleMergeDecorator.Merge (validationCollectorInfos), expectedBeforeMerge, 1);
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