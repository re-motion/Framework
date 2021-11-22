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
using System.Reflection;
using System.Runtime.CompilerServices;
using Moq;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Context.Suppression;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context.FluentBuilders
{
  [TestFixture]
  public class MixinContextBuilderTest
  {
    private Mock<ClassContextBuilder> _parentBuilderMock;
    private MixinContextOrigin _origin;

    private MixinContextBuilder _mixinBuilder;

    [SetUp]
    public void SetUp ()
    {
      _parentBuilderMock = new Mock<ClassContextBuilder>(MockBehavior.Strict, typeof(object));
      _origin = MixinContextOriginObjectMother.Create();

      _mixinBuilder = new MixinContextBuilder(_parentBuilderMock.Object, typeof(BT2Mixin1), _origin);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_mixinBuilder.MixinType, Is.SameAs(typeof(BT2Mixin1)));
      Assert.That(_mixinBuilder.Parent, Is.SameAs(_parentBuilderMock.Object));
      Assert.That(_mixinBuilder.Origin, Is.SameAs(_origin));
      Assert.That(_mixinBuilder.Dependencies, Is.Empty);
      Assert.That(_mixinBuilder.MixinKind, Is.EqualTo(MixinKind.Extending));
      Assert.That(_mixinBuilder.IntroducedMemberVisiblity, Is.EqualTo(MemberVisibility.Private));
    }

    [Test]
    public void OfKind_Used ()
    {
      Assert.That(_mixinBuilder.OfKind(MixinKind.Used), Is.SameAs(_mixinBuilder));
      Assert.That(_mixinBuilder.MixinKind, Is.EqualTo(MixinKind.Used));
    }

    [Test]
    public void OfKind_Extending ()
    {
      Assert.That(_mixinBuilder.OfKind(MixinKind.Extending), Is.SameAs(_mixinBuilder));
      Assert.That(_mixinBuilder.MixinKind, Is.EqualTo(MixinKind.Extending));
    }

    [Test]
    public void WithDependency_NonGeneric ()
    {
      _mixinBuilder.WithDependency(typeof(BT1Mixin1));
      Assert.That(_mixinBuilder.Dependencies, Is.EquivalentTo(new object[] { typeof(BT1Mixin1) }));
    }

    [Test]
    public void WithDependency_Twice ()
    {
      Assert.That(
          () => _mixinBuilder.WithDependency(typeof(BT1Mixin1)).WithDependency(typeof(BT1Mixin1)),
          Throws.ArgumentException
              .With.Message.Contains(
                  "The mixin Remotion.Mixins.UnitTests.Core.TestDomain.BT2Mixin1 already has a "
                  + "dependency on type Remotion.Mixins.UnitTests.Core.TestDomain.BT1Mixin1."));
    }

    [Test]
    public void WithDependency_Generic ()
    {
      _mixinBuilder.WithDependency<BT1Mixin1>();
      Assert.That(_mixinBuilder.Dependencies, Is.EquivalentTo(new object[] { typeof(BT1Mixin1) }));
    }

    [Test]
    public void WithDependencies_NonGeneric ()
    {
      _mixinBuilder.WithDependencies(typeof(BT1Mixin1), typeof(BT1Mixin2));
      Assert.That(_mixinBuilder.Dependencies, Is.EquivalentTo(new object[] { typeof(BT1Mixin1), typeof(BT1Mixin2) }));
    }

    [Test]
    public void WithDependencies_Generic2 ()
    {
      _mixinBuilder.WithDependencies<BT1Mixin1, BT1Mixin2>();
      Assert.That(_mixinBuilder.Dependencies, Is.EquivalentTo(new object[] { typeof(BT1Mixin1), typeof(BT1Mixin2) }));
    }

    [Test]
    public void WithDependencies_Generic3 ()
    {
      _mixinBuilder.WithDependencies<BT1Mixin1, BT1Mixin2, BT2Mixin1>();
      Assert.That(_mixinBuilder.Dependencies, Is.EquivalentTo(new object[] { typeof(BT1Mixin1), typeof(BT1Mixin2), typeof(BT2Mixin1) }));
    }

    [Test]
    public void WithIntroducedMemberVisibility_Public ()
    {
      _mixinBuilder.WithIntroducedMemberVisibility(MemberVisibility.Public);
      Assert.That(_mixinBuilder.IntroducedMemberVisiblity, Is.EqualTo(MemberVisibility.Public));
    }

    [Test]
    public void WithIntroducedMemberVisibility_Private ()
    {
      _mixinBuilder.WithIntroducedMemberVisibility(MemberVisibility.Private);
      Assert.That(_mixinBuilder.IntroducedMemberVisiblity, Is.EqualTo(MemberVisibility.Private));
    }

    [Test]
    public void WithIntroducedMemberVisibility_ReturnsMixinBuilder ()
    {
      MixinContextBuilder result = _mixinBuilder.WithIntroducedMemberVisibility(MemberVisibility.Public);
      Assert.That(result, Is.SameAs(_mixinBuilder));
    }

    [Test]
    public void ReplaceMixin_NonGeneric ()
    {
      _parentBuilderMock
          .Setup(mock => mock.SuppressMixin(It.Is<IMixinSuppressionRule>(
              rule => ((MixinTreeReplacementSuppressionRule)rule).ReplacingMixinType == _mixinBuilder.MixinType
                  && ((MixinTreeReplacementSuppressionRule)rule).MixinBaseTypeToSuppress == typeof(int))))
          .Returns(_parentBuilderMock.Object)
          .Verifiable();

      var result = _mixinBuilder.ReplaceMixin(typeof(int));

      Assert.That(result, Is.SameAs(_mixinBuilder));
      _parentBuilderMock.Verify();
    }

    [Test]
    public void ReplaceMixin_Generic ()
    {
      var mixinContextBuilderPartialMock = CreatePartialMock();
      mixinContextBuilderPartialMock.Setup(mock => mock.ReplaceMixin(typeof(int))).Returns(mixinContextBuilderPartialMock.Object).Verifiable();

      var result = mixinContextBuilderPartialMock.Object.ReplaceMixin<int>();

      Assert.That(result, Is.SameAs(mixinContextBuilderPartialMock.Object));
      mixinContextBuilderPartialMock.Verify();
    }

    [Test]
    public void ReplaceMixins_NonGeneric ()
    {
      var mixinContextBuilderPartialMock = CreatePartialMock();
      mixinContextBuilderPartialMock.Setup(mock => mock.ReplaceMixin(typeof(int))).Returns(mixinContextBuilderPartialMock.Object).Verifiable();
      mixinContextBuilderPartialMock.Setup(mock => mock.ReplaceMixin(typeof(double))).Returns(mixinContextBuilderPartialMock.Object).Verifiable();

      var result = mixinContextBuilderPartialMock.Object.ReplaceMixins(typeof(int), typeof(double));

      Assert.That(result, Is.SameAs(mixinContextBuilderPartialMock.Object));
      mixinContextBuilderPartialMock.Verify();
    }

    [Test]
    public void ReplaceMixins_Generic2 ()
    {
      var mixinContextBuilderPartialMock = CreatePartialMock();
      mixinContextBuilderPartialMock.Setup(mock => mock.ReplaceMixin(typeof(int))).Returns(mixinContextBuilderPartialMock.Object).Verifiable();
      mixinContextBuilderPartialMock.Setup(mock => mock.ReplaceMixin(typeof(double))).Returns(mixinContextBuilderPartialMock.Object).Verifiable();

      var result = mixinContextBuilderPartialMock.Object.ReplaceMixins<int, double>();

      Assert.That(result, Is.SameAs(mixinContextBuilderPartialMock.Object));
      mixinContextBuilderPartialMock.Verify();
    }

    [Test]
    public void ReplaceMixins_Generic3 ()
    {
      var mixinContextBuilderPartialMock = CreatePartialMock();
      mixinContextBuilderPartialMock.Setup(mock => mock.ReplaceMixin(typeof(int))).Returns(mixinContextBuilderPartialMock.Object).Verifiable();
      mixinContextBuilderPartialMock.Setup(mock => mock.ReplaceMixin(typeof(double))).Returns(mixinContextBuilderPartialMock.Object).Verifiable();
      mixinContextBuilderPartialMock.Setup(mock => mock.ReplaceMixin(typeof(string))).Returns(mixinContextBuilderPartialMock.Object).Verifiable();

      var result = mixinContextBuilderPartialMock.Object.ReplaceMixins<int, double, string>();

      Assert.That(result, Is.SameAs(mixinContextBuilderPartialMock.Object));
      mixinContextBuilderPartialMock.Verify();
    }

    [Test]
    public void ReplaceMixin_SelfSuppressor ()
    {
      Assert.That(
          () => _mixinBuilder.ReplaceMixin(_mixinBuilder.MixinType),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Mixin type 'Remotion.Mixins.UnitTests.Core.TestDomain.BT2Mixin1' applied "
                  + "to target class 'System.Object' suppresses itself."));
    }

    [Test]
    public void ReplaceMixin_SelfSuppressor_GenericDefinition ()
    {
      var mixinBuilder = new MixinContextBuilder(_parentBuilderMock.Object, typeof(GenericMixin<object>), _origin);
      Assert.That(
          () => mixinBuilder.ReplaceMixin(typeof(GenericMixin<>)),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Mixin type "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixin`1[System.Object]' applied "
                  + "to target class 'System.Object' suppresses itself."));
    }

    [Test]
    public void ReplaceMixin_BaseSuppressor ()
    {
      _parentBuilderMock
          .Setup(mock => mock.SuppressMixin(It.Is<IMixinSuppressionRule>(
              rule => ((MixinTreeReplacementSuppressionRule)rule).ReplacingMixinType == _mixinBuilder.MixinType
                  && ((MixinTreeReplacementSuppressionRule)rule).MixinBaseTypeToSuppress == _mixinBuilder.MixinType.BaseType)))
          .Returns(_parentBuilderMock.Object)
          .Verifiable();

      var result = _mixinBuilder.ReplaceMixin(_mixinBuilder.MixinType.BaseType);

      Assert.That(result, Is.SameAs(_mixinBuilder));
      _parentBuilderMock.Verify();
    }

    [Test]
    public void BuildContext_NoDependencies ()
    {
      MixinContext mixinContext = _mixinBuilder.BuildMixinContext();
      Assert.That(mixinContext.ExplicitDependencies, Is.Empty);
    }

    [Test]
    public void BuildContext_ExplicitKind ()
    {
      MixinContext mixinContext = _mixinBuilder.BuildMixinContext();
      Assert.That(mixinContext.MixinKind, Is.EqualTo(MixinKind.Extending));
    }

    [Test]
    public void BuildContext_UsedKind ()
    {
      _mixinBuilder.OfKind(MixinKind.Used);
      MixinContext mixinContext = _mixinBuilder.BuildMixinContext();
      Assert.That(mixinContext.MixinKind, Is.EqualTo(MixinKind.Used));
    }

    [Test]
    public void BuildContext_PrivateVisibility ()
    {
      _mixinBuilder.WithIntroducedMemberVisibility(MemberVisibility.Private);
      MixinContext mixinContext = _mixinBuilder.BuildMixinContext();
      Assert.That(mixinContext.IntroducedMemberVisibility, Is.EqualTo(MemberVisibility.Private));
    }

    [Test]
    public void BuildContext_PublicVisibility ()
    {
      _mixinBuilder.WithIntroducedMemberVisibility(MemberVisibility.Public);
      MixinContext mixinContext = _mixinBuilder.BuildMixinContext();
      Assert.That(mixinContext.IntroducedMemberVisibility, Is.EqualTo(MemberVisibility.Public));
    }

    [Test]
    public void BuildContext_WithDependency ()
    {
      _mixinBuilder.WithDependency<IBT3Mixin4>();
      MixinContext context = _mixinBuilder.BuildMixinContext();
      Assert.That(context.ExplicitDependencies, Is.EqualTo(new[] {typeof(IBT3Mixin4)}));
    }

    [Test]
    public void BuildContext_Origin ()
    {
      MixinContext mixinContext = _mixinBuilder.BuildMixinContext();
      Assert.That(mixinContext.Origin, Is.EqualTo(_origin));
    }

    [Test]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void ParentMembers ()
    {
      _parentBuilderMock.Reset();

      var suppressionRuleStub = new Mock<IMixinSuppressionRule>();
      var r1 = new ClassContextBuilder(new MixinConfigurationBuilder(null), typeof(object));
      var r2 = new MixinConfiguration();
      var r3 = new Mock<IDisposable>(MockBehavior.Strict);
      var r4 = new MixinContextBuilder(r1, typeof(BT1Mixin1), _origin);
      var r5 = ClassContextObjectMother.Create(typeof(object));
      var origin = MixinContextOriginObjectMother.Create();

      IEnumerable<ClassContext> inheritedContexts = new ClassContext[0];

      var expectedInferredOrigin = MixinContextOrigin.CreateForMethod(MethodBase.GetCurrentMethod());

      var sequence = new MockSequence();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.Clear()).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddMixin(typeof(object), origin)).Returns(r4).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddMixin(typeof(object), expectedInferredOrigin)).Returns(r4).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddMixin<string>(origin)).Returns(r4).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddMixin<string>(expectedInferredOrigin)).Returns(r4).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddMixins(origin, typeof(BT1Mixin1), typeof(BT1Mixin2))).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddMixins(expectedInferredOrigin, typeof(BT1Mixin1), typeof(BT1Mixin2))).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddMixins<BT1Mixin1, BT1Mixin2>(origin)).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddMixins<BT1Mixin1, BT1Mixin2>(expectedInferredOrigin)).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>(origin)).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>(expectedInferredOrigin)).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddOrderedMixins(origin, typeof(BT1Mixin1), typeof(BT1Mixin2))).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddOrderedMixins(expectedInferredOrigin, typeof(BT1Mixin1), typeof(BT1Mixin2))).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddOrderedMixins<BT1Mixin1, BT1Mixin2>(origin)).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddOrderedMixins<BT1Mixin1, BT1Mixin2>(expectedInferredOrigin)).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddOrderedMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>(origin)).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddOrderedMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>(expectedInferredOrigin)).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.EnsureMixin(typeof(object), origin)).Returns(r4).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.EnsureMixin(typeof(object), expectedInferredOrigin)).Returns(r4).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.EnsureMixin<string>(origin)).Returns(r4).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.EnsureMixin<string>(expectedInferredOrigin)).Returns(r4).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.EnsureMixins(origin, typeof(BT1Mixin1), typeof(BT1Mixin2))).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.EnsureMixins(expectedInferredOrigin, typeof(BT1Mixin1), typeof(BT1Mixin2))).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.EnsureMixins<BT1Mixin1, BT1Mixin2>(origin)).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.EnsureMixins<BT1Mixin1, BT1Mixin2>(expectedInferredOrigin)).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.EnsureMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>(origin)).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.EnsureMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>(expectedInferredOrigin)).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddComposedInterface(typeof(IBT6Mixin1))).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddComposedInterface<IBT6Mixin1>()).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddComposedInterfaces(typeof(IBT6Mixin1), typeof(IBT6Mixin2))).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddComposedInterfaces<IBT6Mixin1, IBT6Mixin2>()).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddComposedInterfaces<IBT6Mixin1, IBT6Mixin2, IBT6Mixin3>()).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddComposedInterfaces<IBT6Mixin1, IBT6Mixin2, IBT6Mixin3>()).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.SuppressMixin(suppressionRuleStub.Object)).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.SuppressMixin(typeof(object))).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.SuppressMixin<string>()).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.SuppressMixins(typeof(BT1Mixin1), typeof(BT1Mixin2))).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.SuppressMixins<BT1Mixin1, BT1Mixin2>()).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.SuppressMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>()).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddMixinDependency(typeof(BT1Mixin1), typeof(BT1Mixin2))).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.AddMixinDependency<BT1Mixin1, BT1Mixin2>()).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.BuildClassContext(inheritedContexts)).Returns(r5).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.BuildClassContext()).Returns(r5).Verifiable();

      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.ForClass<object>()).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.ForClass<string>()).Returns(r1).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.BuildConfiguration()).Returns(r2).Verifiable();
      _parentBuilderMock.InSequence(sequence).Setup(mock => mock.EnterScope()).Returns(r3.Object).Verifiable();

      Assert.That(_mixinBuilder.Clear(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddMixin(typeof(object), origin), Is.SameAs(r4));
      Assert.That(_mixinBuilder.AddMixin(typeof(object)), Is.SameAs(r4));
      Assert.That(_mixinBuilder.AddMixin<string>(origin), Is.SameAs(r4));
      Assert.That(_mixinBuilder.AddMixin<string>(), Is.SameAs(r4));
      Assert.That(_mixinBuilder.AddMixins(origin, typeof(BT1Mixin1), typeof(BT1Mixin2)), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddMixins(typeof(BT1Mixin1), typeof(BT1Mixin2)), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddMixins<BT1Mixin1, BT1Mixin2>(origin), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddMixins<BT1Mixin1, BT1Mixin2>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>(origin), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddOrderedMixins(origin, typeof(BT1Mixin1), typeof(BT1Mixin2)), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddOrderedMixins(typeof(BT1Mixin1), typeof(BT1Mixin2)), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddOrderedMixins<BT1Mixin1, BT1Mixin2>(origin), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddOrderedMixins<BT1Mixin1, BT1Mixin2>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddOrderedMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>(origin), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddOrderedMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.EnsureMixin(typeof(object), origin), Is.SameAs(r4));
      Assert.That(_mixinBuilder.EnsureMixin(typeof(object)), Is.SameAs(r4));
      Assert.That(_mixinBuilder.EnsureMixin<string>(origin), Is.SameAs(r4));
      Assert.That(_mixinBuilder.EnsureMixin<string>(), Is.SameAs(r4));
      Assert.That(_mixinBuilder.EnsureMixins(origin, typeof(BT1Mixin1), typeof(BT1Mixin2)), Is.SameAs(r1));
      Assert.That(_mixinBuilder.EnsureMixins(typeof(BT1Mixin1), typeof(BT1Mixin2)), Is.SameAs(r1));
      Assert.That(_mixinBuilder.EnsureMixins<BT1Mixin1, BT1Mixin2>(origin), Is.SameAs(r1));
      Assert.That(_mixinBuilder.EnsureMixins<BT1Mixin1, BT1Mixin2>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.EnsureMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>(origin), Is.SameAs(r1));
      Assert.That(_mixinBuilder.EnsureMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddComposedInterface(typeof(IBT6Mixin1)), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddComposedInterface<IBT6Mixin1>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddComposedInterfaces(typeof(IBT6Mixin1), typeof(IBT6Mixin2)), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddComposedInterfaces<IBT6Mixin1, IBT6Mixin2>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddComposedInterfaces<IBT6Mixin1, IBT6Mixin2, IBT6Mixin3>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddComposedInterfaces<IBT6Mixin1, IBT6Mixin2, IBT6Mixin3>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.SuppressMixin(suppressionRuleStub.Object), Is.SameAs(r1));
      Assert.That(_mixinBuilder.SuppressMixin(typeof(object)), Is.SameAs(r1));
      Assert.That(_mixinBuilder.SuppressMixin<string>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.SuppressMixins(typeof(BT1Mixin1), typeof(BT1Mixin2)), Is.SameAs(r1));
      Assert.That(_mixinBuilder.SuppressMixins<BT1Mixin1, BT1Mixin2>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.SuppressMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddMixinDependency(typeof(BT1Mixin1), typeof(BT1Mixin2)), Is.SameAs(r1));
      Assert.That(_mixinBuilder.AddMixinDependency<BT1Mixin1, BT1Mixin2>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.BuildClassContext(inheritedContexts), Is.SameAs(r5));
      Assert.That(_mixinBuilder.BuildClassContext(), Is.SameAs(r5));

      Assert.That(_mixinBuilder.ForClass<object>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.ForClass<string>(), Is.SameAs(r1));
      Assert.That(_mixinBuilder.BuildConfiguration(), Is.SameAs(r2));
      Assert.That(_mixinBuilder.EnterScope(), Is.SameAs(r3.Object));

      _parentBuilderMock.Verify();
      r3.Verify();
    }

    private Mock<MixinContextBuilder> CreatePartialMock ()
    {
      return new Mock<MixinContextBuilder>(_parentBuilderMock.Object, typeof(object), _origin) { CallBase = true };
    }
  }
}
