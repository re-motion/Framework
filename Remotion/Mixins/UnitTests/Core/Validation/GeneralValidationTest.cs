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
using System.ComponentModel.Design;
using System.Linq;
using NUnit.Framework;
using Remotion.Development.Mixins.Validation;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.UnitTests.Core.Validation.ValidationTestDomain;
using Remotion.Mixins.Validation;
using Remotion.Mixins.Validation.Rules;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.Mixins.UnitTests.Core.Validation
{
  [TestFixture]
  public class GeneralValidationTest : ValidationTestBase
  {
    private IDisposable _configurationScope;

    [SetUp]
    public void SetUp ()
    {
      var validationTestDomainNamespace = typeof(AbstractMixinWithoutBase).Namespace;
      var globalTestDomainNamespace = typeof(BaseType1).Namespace;
      var typeDiscoveryService = SafeServiceLocator.Current.GetInstance<ITypeDiscoveryService>();
      var filteringTypeDiscoveryService = FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist(
          typeDiscoveryService, validationTestDomainNamespace, globalTestDomainNamespace);
      var types = filteringTypeDiscoveryService.GetTypes(null, false);
      _configurationScope = DeclarativeConfigurationBuilder.BuildConfigurationFromTypes(null, types.Cast<Type>()).EnterScope();
    }

    [TearDown]
    public void TearDown ()
    {
      _configurationScope.Dispose();
    }

    [Test]
    public void ValidationVisitsSomething ()
    {
      var log = MixinConfiguration.ActiveConfiguration.Validate();
      Assert.That(log.ResultCount > 1, Is.True);
    }

    [Test]
    public void ValidationDump ()
    {
      // This test shows validation errors to the console if there are any in the mixin test domain.

      var log = MixinConfiguration.ActiveConfiguration.Validate();
      ConsoleDumper.DumpValidationResults(log.GetResults());
    }

    [Test]
    public void ValidationResultDefinition ()
    {
      var log = MixinConfiguration.ActiveConfiguration.Validate();

      using (IEnumerator<ValidationResult> results = log.GetResults().GetEnumerator())
      {
        Assert.That(results.MoveNext(), Is.True);
        ValidationResult firstResult = results.Current;
        Assert.That(firstResult.ValidatedDefinition, Is.Not.Null);
      }
    }

    [Test]
    public void DefaultConfiguration_IsValid ()
    {
      var log = MixinConfiguration.ActiveConfiguration.Validate();
      AssertSuccess(log);
    }

    [Test]
    public void HasDefaultRules ()
    {
      var log = MixinConfiguration.ActiveConfiguration.Validate();
      Assert.That(log.GetNumberOfRulesExecuted() > 0, Is.True);
    }

    [Test]
    public void CollectsUnexpectedExceptions ()
    {
      TargetClassDefinition bc = DefinitionObjectMother.BuildUnvalidatedDefinition(typeof(DateTime));
      var log = Validator.Validate(bc, new ThrowingRuleSet());
      Assert.That(log.GetNumberOfUnexpectedExceptions() > 0, Is.True);
      var results = new List<ValidationResult>(log.GetResults());
      Assert.That(results[0].Exceptions[0].Exception is InvalidOperationException, Is.True);
    }

    [Test]
    public void DefaultConfiguration_EverythingIsVisitedOnce ()
    {
      var activeConfiguration = MixinConfiguration.ActiveConfiguration;

      ValidationLogData log;
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        log = activeConfiguration.Validate();
      }

      var validationResults = log.GetResults();
      var visitedDefinitions = new HashSet<IVisitableDefinition>();
      foreach (ValidationResult result in validationResults)
      {
        var definition = result.ValidatedDefinition;
        Assert.That(visitedDefinitions.Contains(definition), Is.False, definition.ToString());
        visitedDefinitions.Add(definition);
      }

      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType1));
      AssertVisitedEquivalent(validationResults, bt1);
      TargetClassDefinition bt3 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType3));
      AssertVisitedEquivalent(validationResults, bt3);
      TargetClassDefinition bt6 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType6));
      AssertVisitedEquivalent(validationResults, bt6);
      TargetClassDefinition btWithAdditionalDependencies =
          DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(TargetClassWithAdditionalDependencies));
      AssertVisitedEquivalent(validationResults, btWithAdditionalDependencies);
      TargetClassDefinition targetWithSuppressAttribute =
          DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(TargetClassSuppressingBT1Attribute));
      AssertVisitedEquivalent(validationResults, targetWithSuppressAttribute);
      TargetClassDefinition targetWithNonIntroducedAttribute =
          DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(TargetClassWithMixinNonIntroducingSimpleAttribute));
      AssertVisitedEquivalent(validationResults, targetWithSuppressAttribute);
      TargetClassDefinition targetClassWinningOverMixinAddingBT1AttributeToMember =
          DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(TargetClassWinningOverMixinAddingBT1AttributeToMember));
      AssertVisitedEquivalent(validationResults, targetClassWinningOverMixinAddingBT1AttributeToMember);

      MixinDefinition bt1m1 = bt1.Mixins[typeof(BT1Mixin1)];
      AssertVisitedEquivalent(validationResults, bt1m1);
      MixinDefinition bt1m2 = bt1.Mixins[typeof(BT1Mixin2)];
      AssertVisitedEquivalent(validationResults, bt1m2);
      MixinDefinition bt3m1 = bt3.Mixins[typeof(BT3Mixin1)];
      AssertVisitedEquivalent(validationResults, bt3m1);
      MixinDefinition bt3m2 = bt3.Mixins[typeof(BT3Mixin2)];
      AssertVisitedEquivalent(validationResults, bt3m2);
      MixinDefinition bt3m3 = bt3.GetMixinByConfiguredType(typeof(BT3Mixin3<,>));
      AssertVisitedEquivalent(validationResults, bt3m3);
      MixinDefinition bt3m4 = bt3.Mixins[typeof(BT3Mixin4)];
      AssertVisitedEquivalent(validationResults, bt3m4);
      MixinDefinition bt3m5 = bt3.Mixins[typeof(BT3Mixin5)];
      AssertVisitedEquivalent(validationResults, bt3m5);
      MixinDefinition mixinWithSuppressedAttribute = targetWithSuppressAttribute.Mixins[typeof(MixinAddingBT1Attribute)];
      AssertVisitedEquivalent(validationResults, mixinWithSuppressedAttribute);
      MixinDefinition mixinWithNonIntroducedAttribute = targetWithNonIntroducedAttribute.Mixins[typeof(MixinNonIntroducingSimpleAttribute)];
      AssertVisitedEquivalent(validationResults, mixinWithNonIntroducedAttribute);

      MethodDefinition m1 = bt1.Methods[typeof(BaseType1).GetMethod("VirtualMethod", Type.EmptyTypes)];
      AssertVisitedEquivalent(validationResults, m1);
      MethodDefinition m2 = bt1.Methods[typeof(BaseType1).GetMethod("VirtualMethod", new[] { typeof(string) })];
      AssertVisitedEquivalent(validationResults, m2);
      MethodDefinition m3 = bt1m1.Methods[typeof(BT1Mixin1).GetMethod("VirtualMethod")];
      AssertVisitedEquivalent(validationResults, m3);
      MethodDefinition m4 = bt1m1.Methods[typeof(BT1Mixin1).GetMethod("IntroducedMethod")];
      AssertVisitedEquivalent(validationResults, m4);
      MethodDefinition memberWinningOverMixinAddingAttribute =
          targetClassWinningOverMixinAddingBT1AttributeToMember.Methods[
              typeof(TargetClassWinningOverMixinAddingBT1AttributeToMember).GetMethod("VirtualMethod")];
      AssertVisitedEquivalent(validationResults, memberWinningOverMixinAddingAttribute);

      PropertyDefinition p1 = bt1.Properties[typeof(BaseType1).GetProperty("VirtualProperty")];
      AssertVisitedEquivalent(validationResults, p1);
      MethodDefinition m5 = p1.GetMethod;
      AssertVisitedEquivalent(validationResults, m5);
      MethodDefinition m6 = p1.SetMethod;
      AssertVisitedEquivalent(validationResults, m6);
      PropertyDefinition p2 = bt1m1.Properties[typeof(BT1Mixin1).GetProperty("VirtualProperty")];
      AssertVisitedEquivalent(validationResults, p2);

      EventDefinition e1 = bt1.Events[typeof(BaseType1).GetEvent("VirtualEvent")];
      AssertVisitedEquivalent(validationResults, e1);
      MethodDefinition m7 = e1.AddMethod;
      AssertVisitedEquivalent(validationResults, m7);
      MethodDefinition m8 = e1.RemoveMethod;
      AssertVisitedEquivalent(validationResults, m8);
      EventDefinition e2 = bt1m1.Events[typeof(BT1Mixin1).GetEvent("VirtualEvent")];
      AssertVisitedEquivalent(validationResults, e2);

      InterfaceIntroductionDefinition i1 = bt1m1.InterfaceIntroductions[typeof(IBT1Mixin1)];
      AssertVisitedEquivalent(validationResults, i1);
      MethodIntroductionDefinition im1 = i1.IntroducedMethods[typeof(IBT1Mixin1).GetMethod("IntroducedMethod")];
      AssertVisitedEquivalent(validationResults, im1);
      PropertyIntroductionDefinition im2 = i1.IntroducedProperties[typeof(IBT1Mixin1).GetProperty("IntroducedProperty")];
      AssertVisitedEquivalent(validationResults, im2);
      EventIntroductionDefinition im3 = i1.IntroducedEvents[typeof(IBT1Mixin1).GetEvent("IntroducedEvent")];
      AssertVisitedEquivalent(validationResults, im3);

      AttributeDefinition a1 = bt1.CustomAttributes.GetFirstItem(typeof(BT1Attribute));
      AssertVisitedEquivalent(validationResults, a1);
      AttributeDefinition a2 = bt1m1.CustomAttributes.GetFirstItem(typeof(BT1M1Attribute));
      AssertVisitedEquivalent(validationResults, a2);
      AttributeDefinition a3 = m1.CustomAttributes.GetFirstItem(typeof(BT1Attribute));
      AssertVisitedEquivalent(validationResults, a3);
      AttributeDefinition a4 = p1.CustomAttributes.GetFirstItem(typeof(BT1Attribute));
      AssertVisitedEquivalent(validationResults, a4);
      AttributeDefinition a5 = e1.CustomAttributes.GetFirstItem(typeof(BT1Attribute));
      AssertVisitedEquivalent(validationResults, a5);
      AttributeDefinition a6 = im1.ImplementingMember.CustomAttributes.GetFirstItem(typeof(BT1M1Attribute));
      AssertVisitedEquivalent(validationResults, a6);
      AttributeDefinition a7 = im2.ImplementingMember.CustomAttributes.GetFirstItem(typeof(BT1M1Attribute));
      AssertVisitedEquivalent(validationResults, a7);
      AttributeDefinition a8 = im3.ImplementingMember.CustomAttributes.GetFirstItem(typeof(BT1M1Attribute));
      AssertVisitedEquivalent(validationResults, a8);

      AttributeIntroductionDefinition ai1 = bt1.ReceivedAttributes.GetFirstItem(typeof(BT1M1Attribute));
      AssertVisitedEquivalent(validationResults, ai1);
      AttributeIntroductionDefinition ai2 = m1.ReceivedAttributes.GetFirstItem(typeof(BT1M1Attribute));
      AssertVisitedEquivalent(validationResults, ai2);

      RequiredNextCallTypeDefinition bc1 = bt3.RequiredNextCallTypes[typeof(IBaseType34)];
      AssertVisitedEquivalent(validationResults, bc1);
      RequiredMethodDefinition bcm1 = bc1.Methods[typeof(IBaseType34).GetMethod("IfcMethod")];
      AssertVisitedEquivalent(validationResults, bcm1);

      RequiredTargetCallTypeDefinition ft1 = bt3.RequiredTargetCallTypes[typeof(IBaseType32)];
      AssertVisitedEquivalent(validationResults, ft1);
      RequiredMethodDefinition fm1 = ft1.Methods[typeof(IBaseType32).GetMethod("IfcMethod")];
      AssertVisitedEquivalent(validationResults, fm1);

      RequiredMixinTypeDefinition rmt1 = btWithAdditionalDependencies.RequiredMixinTypes[typeof(IMixinWithAdditionalClassDependency)];
      AssertVisitedEquivalent(validationResults, rmt1);
      RequiredMixinTypeDefinition rmt2 = btWithAdditionalDependencies.RequiredMixinTypes[typeof(MixinWithNoAdditionalDependency)];
      AssertVisitedEquivalent(validationResults, rmt2);

      ComposedInterfaceDependencyDefinition cid1 = bt6.ComposedInterfaceDependencies[typeof(ICBT6Mixin1)];
      AssertVisitedEquivalent(validationResults, cid1);

      TargetCallDependencyDefinition td1 = bt3m1.TargetCallDependencies[typeof(IBaseType31)];
      AssertVisitedEquivalent(validationResults, td1);

      NextCallDependencyDefinition bd1 = bt3m1.NextCallDependencies[typeof(IBaseType31)];
      AssertVisitedEquivalent(validationResults, bd1);

      MixinDependencyDefinition md1 =
          btWithAdditionalDependencies.Mixins[typeof(MixinWithAdditionalClassDependency)].MixinDependencies[typeof(MixinWithNoAdditionalDependency)];
      AssertVisitedEquivalent(validationResults, md1);

      SuppressedAttributeIntroductionDefinition suppressedAttribute1 =
          mixinWithSuppressedAttribute.SuppressedAttributeIntroductions.GetFirstItem(typeof(BT1Attribute));
      AssertVisitedEquivalent(validationResults, suppressedAttribute1);

      NonAttributeIntroductionDefinition nonIntroducedAttribute1 =
          mixinWithNonIntroducedAttribute.NonAttributeIntroductions.GetFirstItem(typeof(SimpleAttribute));
      AssertVisitedEquivalent(validationResults, nonIntroducedAttribute1);
      NonAttributeIntroductionDefinition nonIntroducedAttribute2 = memberWinningOverMixinAddingAttribute.Overrides[0].NonAttributeIntroductions[0];
      AssertVisitedEquivalent(validationResults, nonIntroducedAttribute2);
    }

    [Test]
    public void ValidationException ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition(
          typeof(ClassOverridingSingleMixinMethod), typeof(AbstractMixinWithoutBase));

      var log = new DefaultValidationLog();
      var visitor = new ValidatingVisitor(log);
      new DefaultMethodRules().Install(visitor);
      definition.Accept(visitor);

      var validationLogData = log.GetData();
      var exception = new ValidationException(validationLogData);
      Assert.That(
          exception.Message,
          Is.EqualTo(
              "Some parts of the mixin configuration could not be validated."
              + Environment.NewLine
              + "MethodDefinition 'Remotion.Mixins.UnitTests.Core.Validation.ValidationTestDomain.AbstractMixinWithoutBase.AbstractMethod', 6 rules "
              + "executed"
              + Environment.NewLine
              + "Context: Remotion.Mixins.UnitTests.Core.Validation.ValidationTestDomain.AbstractMixinWithoutBase -> "
              + "Remotion.Mixins.UnitTests.Core.TestDomain.ClassOverridingSingleMixinMethod"
              + Environment.NewLine
              + "  failures - 1"
              + Environment.NewLine
              + "    A target class overrides a method from one of its mixins, but the mixin is not derived from one of the Mixin<...> base classes. "
              + "(Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverridingMixinMethodsOnlyPossibleWhenMixinDerivedFromMixinBase)"
              + Environment.NewLine
));
    }

    [Test]
    public void Merge ()
    {
      IValidationLog sourceLog = new DefaultValidationLog();
      var exception = new Exception();

      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType1));
      TargetClassDefinition bt2 = DefinitionObjectMother.GetActiveTargetClassDefinition_Force(typeof(BaseType2));
      TargetClassDefinition bt3 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType3));
      TargetClassDefinition bt4 = DefinitionObjectMother.GetActiveTargetClassDefinition_Force(typeof(BaseType4));

      sourceLog.ValidationStartsFor(bt1);
      sourceLog.Succeed(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "Success", "Success"));
      sourceLog.Warn(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "Warn", "Warn"));
      sourceLog.Fail(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "Fail", "Fail"));
      sourceLog.UnexpectedException(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "Except", "Except"), exception);
      sourceLog.ValidationEndsFor(bt1);

      sourceLog.ValidationStartsFor(bt4);
      sourceLog.Succeed(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "Success2", "Success2"));
      sourceLog.Warn(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "Warn2", "Warn2"));
      sourceLog.Fail(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "Fail2", "Fail2"));
      sourceLog.UnexpectedException(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "Except2", "Except2"), exception);
      sourceLog.ValidationEndsFor(bt4);

      IValidationLog resultLog = new DefaultValidationLog();
      resultLog.ValidationStartsFor(bt2);
      resultLog.Succeed(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "0", "0"));
      resultLog.Warn(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "1", "1"));
      resultLog.Fail(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "2", "2"));
      resultLog.UnexpectedException(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "3", "3"), exception);
      resultLog.ValidationEndsFor(bt2);

      resultLog.ValidationStartsFor(bt1);
      resultLog.Succeed(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "4", "4"));
      resultLog.Warn(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "5", "5"));
      resultLog.Fail(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "6", "6"));
      resultLog.UnexpectedException(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "7", "7"), exception);
      resultLog.ValidationEndsFor(bt1);

      resultLog.ValidationStartsFor(bt3);
      resultLog.Succeed(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "8", "8"));
      resultLog.Warn(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "9", "9"));
      resultLog.Fail(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "10", "10"));
      resultLog.UnexpectedException(new DelegateValidationRule<TargetClassDefinition>(delegate { }, "11", "11"), exception);
      resultLog.ValidationEndsFor(bt3);

      var logData = resultLog.GetData();
      logData.Add(sourceLog.GetData());
      Assert.That(logData.GetNumberOfSuccesses(), Is.EqualTo(5));
      Assert.That(logData.GetNumberOfWarnings(), Is.EqualTo(5));
      Assert.That(logData.GetNumberOfFailures(), Is.EqualTo(5));
      Assert.That(logData.GetNumberOfUnexpectedExceptions(), Is.EqualTo(5));

      var results = new List<ValidationResult>(logData.GetResults());

      Assert.That(results.Count, Is.EqualTo(4));

      Assert.That(results[0].ValidatedDefinition, Is.EqualTo(bt2));
      Assert.That(results[0].Successes.Count, Is.EqualTo(1));
      Assert.That(results[0].Failures.Count, Is.EqualTo(1));
      Assert.That(results[0].Warnings.Count, Is.EqualTo(1));
      Assert.That(results[0].Exceptions.Count, Is.EqualTo(1));

      Assert.That(results[1].ValidatedDefinition, Is.EqualTo(bt1));

      Assert.That(results[1].Successes.Count, Is.EqualTo(2));
      Assert.That(results[1].Successes[0].Message, Is.EqualTo("4"));
      Assert.That(results[1].Successes[1].Message, Is.EqualTo("Success"));

      Assert.That(results[1].Warnings.Count, Is.EqualTo(2));
      Assert.That(results[1].Warnings[0].Message, Is.EqualTo("5"));
      Assert.That(results[1].Warnings[1].Message, Is.EqualTo("Warn"));

      Assert.That(results[1].Failures.Count, Is.EqualTo(2));
      Assert.That(results[1].Failures[0].Message, Is.EqualTo("6"));
      Assert.That(results[1].Failures[1].Message, Is.EqualTo("Fail"));

      Assert.That(results[1].Exceptions.Count, Is.EqualTo(2));
      Assert.That(results[1].Exceptions[0].Exception, Is.EqualTo(exception));
      Assert.That(results[1].Exceptions[1].Exception, Is.EqualTo(exception));

      Assert.That(results[2].ValidatedDefinition, Is.EqualTo(bt3));
      Assert.That(results[2].Successes.Count, Is.EqualTo(1));
      Assert.That(results[2].Failures.Count, Is.EqualTo(1));
      Assert.That(results[2].Warnings.Count, Is.EqualTo(1));
      Assert.That(results[2].Exceptions.Count, Is.EqualTo(1));

      Assert.That(results[3].ValidatedDefinition, Is.EqualTo(bt4));

      Assert.That(results[3].Successes.Count, Is.EqualTo(1));
      Assert.That(results[3].Successes[0].Message, Is.EqualTo("Success2"));

      Assert.That(results[3].Warnings.Count, Is.EqualTo(1));
      Assert.That(results[3].Warnings[0].Message, Is.EqualTo("Warn2"));

      Assert.That(results[3].Failures.Count, Is.EqualTo(1));
      Assert.That(results[3].Failures[0].Message, Is.EqualTo("Fail2"));

      Assert.That(results[3].Exceptions.Count, Is.EqualTo(1));
      Assert.That(results[3].Exceptions[0].Exception, Is.EqualTo(exception));
    }

    private void AssertVisitedEquivalent (IEnumerable<ValidationResult> validationResults, IVisitableDefinition expectedDefinition)
    {
      var match = validationResults.Any(result => result.ValidatedDefinition.FullName == expectedDefinition.FullName);
      var message = string.Format("Expected {0} '{1}' to be visited.", expectedDefinition.GetType().Name, expectedDefinition.FullName);
      Assert.That(match, message);
    }
  }
}
