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
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Context.Suppression;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.Context.FluentBuilders
{
  [TestFixture]
  public class MixinContextBuilderTest
  {
    private MockRepository _mockRepository;
    private ClassContextBuilder _parentBuilderMock;
    private MixinContextOrigin _origin;

    private MixinContextBuilder _mixinBuilder;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
      _parentBuilderMock = _mockRepository.StrictMock<ClassContextBuilder> (typeof (object));
      _origin = MixinContextOriginObjectMother.Create();
    
      _mixinBuilder = new MixinContextBuilder (_parentBuilderMock, typeof (BT2Mixin1), _origin);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_mixinBuilder.MixinType, Is.SameAs (typeof (BT2Mixin1)));
      Assert.That (_mixinBuilder.Parent, Is.SameAs (_parentBuilderMock));
      Assert.That (_mixinBuilder.Origin, Is.SameAs (_origin));
      Assert.That (_mixinBuilder.Dependencies, Is.Empty);
      Assert.That (_mixinBuilder.MixinKind, Is.EqualTo (MixinKind.Extending));
      Assert.That (_mixinBuilder.IntroducedMemberVisiblity, Is.EqualTo (MemberVisibility.Private));
    }

    [Test]
    public void OfKind_Used ()
    {
      Assert.That (_mixinBuilder.OfKind (MixinKind.Used), Is.SameAs (_mixinBuilder));
      Assert.That (_mixinBuilder.MixinKind, Is.EqualTo (MixinKind.Used));
    }

    [Test]
    public void OfKind_Extending ()
    {
      Assert.That (_mixinBuilder.OfKind (MixinKind.Extending), Is.SameAs (_mixinBuilder));
      Assert.That (_mixinBuilder.MixinKind, Is.EqualTo (MixinKind.Extending));
    }

    [Test]
    public void WithDependency_NonGeneric ()
    {
      _mixinBuilder.WithDependency (typeof (BT1Mixin1));
      Assert.That (_mixinBuilder.Dependencies, Is.EquivalentTo (new object[] { typeof (BT1Mixin1) }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The mixin Remotion.Mixins.UnitTests.Core.TestDomain.BT2Mixin1 already has a "
        + "dependency on type Remotion.Mixins.UnitTests.Core.TestDomain.BT1Mixin1.", MatchType = MessageMatch.Contains)]
    public void WithDependency_Twice ()
    {
      _mixinBuilder.WithDependency (typeof (BT1Mixin1)).WithDependency (typeof (BT1Mixin1));
    }

    [Test]
    public void WithDependency_Generic ()
    {
      _mixinBuilder.WithDependency<BT1Mixin1> ();
      Assert.That (_mixinBuilder.Dependencies, Is.EquivalentTo (new object[] { typeof (BT1Mixin1) }));
    }

    [Test]
    public void WithDependencies_NonGeneric ()
    {
      _mixinBuilder.WithDependencies (typeof (BT1Mixin1), typeof (BT1Mixin2));
      Assert.That (_mixinBuilder.Dependencies, Is.EquivalentTo (new object[] { typeof (BT1Mixin1), typeof (BT1Mixin2) }));
    }

    [Test]
    public void WithDependencies_Generic2 ()
    {
      _mixinBuilder.WithDependencies<BT1Mixin1, BT1Mixin2>();
      Assert.That (_mixinBuilder.Dependencies, Is.EquivalentTo (new object[] { typeof (BT1Mixin1), typeof (BT1Mixin2) }));
    }

    [Test]
    public void WithDependencies_Generic3 ()
    {
      _mixinBuilder.WithDependencies<BT1Mixin1, BT1Mixin2, BT2Mixin1> ();
      Assert.That (_mixinBuilder.Dependencies, Is.EquivalentTo (new object[] { typeof (BT1Mixin1), typeof (BT1Mixin2), typeof (BT2Mixin1) }));
    }

    [Test]
    public void WithIntroducedMemberVisibility_Public ()
    {
      _mixinBuilder.WithIntroducedMemberVisibility (MemberVisibility.Public);
      Assert.That (_mixinBuilder.IntroducedMemberVisiblity, Is.EqualTo (MemberVisibility.Public));
    }

    [Test]
    public void WithIntroducedMemberVisibility_Private ()
    {
      _mixinBuilder.WithIntroducedMemberVisibility (MemberVisibility.Private);
      Assert.That (_mixinBuilder.IntroducedMemberVisiblity, Is.EqualTo (MemberVisibility.Private));
    }

    [Test]
    public void WithIntroducedMemberVisibility_ReturnsMixinBuilder ()
    {
      MixinContextBuilder result = _mixinBuilder.WithIntroducedMemberVisibility (MemberVisibility.Public);
      Assert.That (result, Is.SameAs (_mixinBuilder));
    }

    [Test]
    public void ReplaceMixin_NonGeneric ()
    {
      _parentBuilderMock
          .Expect (mock => mock.SuppressMixin (Arg<IMixinSuppressionRule>.Matches (
              rule => ((MixinTreeReplacementSuppressionRule) rule).ReplacingMixinType == _mixinBuilder.MixinType 
                  && ((MixinTreeReplacementSuppressionRule) rule).MixinBaseTypeToSuppress == typeof (int))))
          .Return (_parentBuilderMock);
      _parentBuilderMock.Replay ();

      var result = _mixinBuilder.ReplaceMixin (typeof (int));

      Assert.That (result, Is.SameAs (_mixinBuilder));
      _parentBuilderMock.VerifyAllExpectations ();
    }

    [Test]
    public void ReplaceMixin_Generic ()
    {
      var mixinContextBuilderPartialMock = CreatePartialMock();
      mixinContextBuilderPartialMock.Expect (mock => mock.ReplaceMixin (typeof (int))).Return (mixinContextBuilderPartialMock);
      mixinContextBuilderPartialMock.Replay ();

      var result = mixinContextBuilderPartialMock.ReplaceMixin<int> ();

      Assert.That (result, Is.SameAs (mixinContextBuilderPartialMock));
      mixinContextBuilderPartialMock.VerifyAllExpectations ();
    }

    [Test]
    public void ReplaceMixins_NonGeneric ()
    {
      var mixinContextBuilderPartialMock = CreatePartialMock();
      mixinContextBuilderPartialMock.Expect (mock => mock.ReplaceMixin (typeof (int))).Return (mixinContextBuilderPartialMock);
      mixinContextBuilderPartialMock.Expect (mock => mock.ReplaceMixin (typeof (double))).Return (mixinContextBuilderPartialMock);
      mixinContextBuilderPartialMock.Replay ();

      var result = mixinContextBuilderPartialMock.ReplaceMixins (typeof (int), typeof (double));

      Assert.That (result, Is.SameAs (mixinContextBuilderPartialMock));
      mixinContextBuilderPartialMock.VerifyAllExpectations ();
    }

    [Test]
    public void ReplaceMixins_Generic2 ()
    {
      var mixinContextBuilderPartialMock = CreatePartialMock();
      mixinContextBuilderPartialMock.Expect (mock => mock.ReplaceMixin (typeof (int))).Return (mixinContextBuilderPartialMock);
      mixinContextBuilderPartialMock.Expect (mock => mock.ReplaceMixin (typeof (double))).Return (mixinContextBuilderPartialMock);
      mixinContextBuilderPartialMock.Replay ();

      var result = mixinContextBuilderPartialMock.ReplaceMixins<int, double>();

      Assert.That (result, Is.SameAs (mixinContextBuilderPartialMock));
      mixinContextBuilderPartialMock.VerifyAllExpectations ();
    }

    [Test]
    public void ReplaceMixins_Generic3 ()
    {
      var mixinContextBuilderPartialMock = CreatePartialMock();
      mixinContextBuilderPartialMock.Expect (mock => mock.ReplaceMixin (typeof (int))).Return (mixinContextBuilderPartialMock);
      mixinContextBuilderPartialMock.Expect (mock => mock.ReplaceMixin (typeof (double))).Return (mixinContextBuilderPartialMock);
      mixinContextBuilderPartialMock.Expect (mock => mock.ReplaceMixin (typeof (string))).Return (mixinContextBuilderPartialMock);
      mixinContextBuilderPartialMock.Replay ();

      var result = mixinContextBuilderPartialMock.ReplaceMixins<int, double, string> ();

      Assert.That (result, Is.SameAs (mixinContextBuilderPartialMock));
      mixinContextBuilderPartialMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Mixin type 'Remotion.Mixins.UnitTests.Core.TestDomain.BT2Mixin1' applied "
        + "to target class 'System.Object' suppresses itself.")]
    public void ReplaceMixin_SelfSuppressor ()
    {
      _mixinBuilder.ReplaceMixin (_mixinBuilder.MixinType);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Mixin type "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixin`1[System.Object]' applied "
        + "to target class 'System.Object' suppresses itself.")]
    public void ReplaceMixin_SelfSuppressor_GenericDefinition ()
    {
      var mixinBuilder = new MixinContextBuilder (_parentBuilderMock, typeof (GenericMixin<object>), _origin);
      mixinBuilder.ReplaceMixin (typeof (GenericMixin<>));
    }

    [Test]
    public void ReplaceMixin_BaseSuppressor ()
    {
      _parentBuilderMock
          .Expect (mock => mock.SuppressMixin (Arg<IMixinSuppressionRule>.Matches (
              rule => ((MixinTreeReplacementSuppressionRule) rule).ReplacingMixinType == _mixinBuilder.MixinType
                  && ((MixinTreeReplacementSuppressionRule) rule).MixinBaseTypeToSuppress == _mixinBuilder.MixinType.BaseType)))
          .Return (_parentBuilderMock);
      _parentBuilderMock.Replay ();

      var result = _mixinBuilder.ReplaceMixin (_mixinBuilder.MixinType.BaseType);

      Assert.That (result, Is.SameAs (_mixinBuilder));
      _parentBuilderMock.VerifyAllExpectations ();
    }

    [Test]
    public void BuildContext_NoDependencies ()
    {
      MixinContext mixinContext = _mixinBuilder.BuildMixinContext ();
      Assert.That(mixinContext.ExplicitDependencies, Is.Empty);
    }

    [Test]
    public void BuildContext_ExplicitKind ()
    {
      MixinContext mixinContext = _mixinBuilder.BuildMixinContext ();
      Assert.That (mixinContext.MixinKind, Is.EqualTo (MixinKind.Extending));
    }

    [Test]
    public void BuildContext_UsedKind ()
    {
      _mixinBuilder.OfKind (MixinKind.Used);
      MixinContext mixinContext = _mixinBuilder.BuildMixinContext ();
      Assert.That (mixinContext.MixinKind, Is.EqualTo (MixinKind.Used));
    }

    [Test]
    public void BuildContext_PrivateVisibility ()
    {
      _mixinBuilder.WithIntroducedMemberVisibility (MemberVisibility.Private);
      MixinContext mixinContext = _mixinBuilder.BuildMixinContext ();
      Assert.That (mixinContext.IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Private));
    }

    [Test]
    public void BuildContext_PublicVisibility ()
    {
      _mixinBuilder.WithIntroducedMemberVisibility (MemberVisibility.Public);
      MixinContext mixinContext = _mixinBuilder.BuildMixinContext ();
      Assert.That (mixinContext.IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Public));
    }

    [Test]
    public void BuildContext_WithDependency ()
    {
      _mixinBuilder.WithDependency<IBT3Mixin4>();
      MixinContext context = _mixinBuilder.BuildMixinContext ();
      Assert.That (context.ExplicitDependencies, Is.EqualTo (new[] {typeof (IBT3Mixin4)}));
    }

    [Test]
    public void BuildContext_Origin ()
    {
      MixinContext mixinContext = _mixinBuilder.BuildMixinContext ();
      Assert.That (mixinContext.Origin, Is.EqualTo (_origin));
    }

    [Test]
    [MethodImpl (MethodImplOptions.NoInlining)]
    public void ParentMembers ()
    {
      _mockRepository.BackToRecordAll ();

      var suppressionRuleStub = MockRepository.GenerateStub<IMixinSuppressionRule> ();
      var r1 = new ClassContextBuilder (new MixinConfigurationBuilder (null), typeof (object));
      var r2 = new MixinConfiguration ();
      var r3 = _mockRepository.StrictMock<IDisposable> ();
      var r4 = new MixinContextBuilder (r1, typeof (BT1Mixin1), _origin);
      var r5 = ClassContextObjectMother.Create(typeof (object));
      var origin = MixinContextOriginObjectMother.Create();

      IEnumerable<ClassContext> inheritedContexts = new ClassContext[0];

      var expectedInferredOrigin = MixinContextOrigin.CreateForMethod (MethodBase.GetCurrentMethod ());

      using (_mockRepository.Ordered ())
      {
        _parentBuilderMock.Expect (mock => mock.Clear ()).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddMixin (typeof (object), origin)).Return (r4);
        _parentBuilderMock.Expect (mock => mock.AddMixin (typeof (object), expectedInferredOrigin)).Return (r4);
        _parentBuilderMock.Expect (mock => mock.AddMixin<string> (origin)).Return (r4);
        _parentBuilderMock.Expect (mock => mock.AddMixin<string> (expectedInferredOrigin)).Return (r4);
        _parentBuilderMock.Expect (mock => mock.AddMixins (origin, typeof (BT1Mixin1), typeof (BT1Mixin2))).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddMixins (expectedInferredOrigin, typeof (BT1Mixin1), typeof (BT1Mixin2))).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddMixins<BT1Mixin1, BT1Mixin2> (origin)).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddMixins<BT1Mixin1, BT1Mixin2> (expectedInferredOrigin)).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> (origin)).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> (expectedInferredOrigin)).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddOrderedMixins (origin, typeof (BT1Mixin1), typeof (BT1Mixin2))).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddOrderedMixins (expectedInferredOrigin, typeof (BT1Mixin1), typeof (BT1Mixin2))).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddOrderedMixins<BT1Mixin1, BT1Mixin2> (origin)).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddOrderedMixins<BT1Mixin1, BT1Mixin2> (expectedInferredOrigin)).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddOrderedMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> (origin)).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddOrderedMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> (expectedInferredOrigin)).Return (r1);
        _parentBuilderMock.Expect (mock => mock.EnsureMixin (typeof (object), origin)).Return (r4);
        _parentBuilderMock.Expect (mock => mock.EnsureMixin (typeof (object), expectedInferredOrigin)).Return (r4);
        _parentBuilderMock.Expect (mock => mock.EnsureMixin<string> (origin)).Return (r4);
        _parentBuilderMock.Expect (mock => mock.EnsureMixin<string> (expectedInferredOrigin)).Return (r4);
        _parentBuilderMock.Expect (mock => mock.EnsureMixins (origin, typeof (BT1Mixin1), typeof (BT1Mixin2))).Return (r1);
        _parentBuilderMock.Expect (mock => mock.EnsureMixins (expectedInferredOrigin, typeof (BT1Mixin1), typeof (BT1Mixin2))).Return (r1);
        _parentBuilderMock.Expect (mock => mock.EnsureMixins<BT1Mixin1, BT1Mixin2> (origin)).Return (r1);
        _parentBuilderMock.Expect (mock => mock.EnsureMixins<BT1Mixin1, BT1Mixin2> (expectedInferredOrigin)).Return (r1);
        _parentBuilderMock.Expect (mock => mock.EnsureMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> (origin)).Return (r1);
        _parentBuilderMock.Expect (mock => mock.EnsureMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> (expectedInferredOrigin)).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddComposedInterface (typeof (IBT6Mixin1))).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddComposedInterface<IBT6Mixin1>()).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddComposedInterfaces (typeof (IBT6Mixin1), typeof (IBT6Mixin2))).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddComposedInterfaces<IBT6Mixin1, IBT6Mixin2> ()).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddComposedInterfaces<IBT6Mixin1, IBT6Mixin2, IBT6Mixin3> ()).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddComposedInterfaces<IBT6Mixin1, IBT6Mixin2, IBT6Mixin3> ()).Return (r1);
        _parentBuilderMock.Expect (mock => mock.SuppressMixin (suppressionRuleStub)).Return (r1);
        _parentBuilderMock.Expect (mock => mock.SuppressMixin (typeof (object))).Return (r1);
        _parentBuilderMock.Expect (mock => mock.SuppressMixin<string> ()).Return (r1);
        _parentBuilderMock.Expect (mock => mock.SuppressMixins (typeof (BT1Mixin1), typeof (BT1Mixin2))).Return (r1);
        _parentBuilderMock.Expect (mock => mock.SuppressMixins<BT1Mixin1, BT1Mixin2> ()).Return (r1);
        _parentBuilderMock.Expect (mock => mock.SuppressMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> ()).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddMixinDependency (typeof (BT1Mixin1), typeof (BT1Mixin2))).Return (r1);
        _parentBuilderMock.Expect (mock => mock.AddMixinDependency<BT1Mixin1, BT1Mixin2> ()).Return (r1);
        _parentBuilderMock.Expect (mock => mock.BuildClassContext (inheritedContexts)).Return (r5);
        _parentBuilderMock.Expect (mock => mock.BuildClassContext ()).Return (r5);

        _parentBuilderMock.Expect (mock => mock.ForClass<object> ()).Return (r1);
        _parentBuilderMock.Expect (mock => mock.ForClass<string> ()).Return (r1);
        _parentBuilderMock.Expect (mock => mock.BuildConfiguration ()).Return (r2);
        _parentBuilderMock.Expect (mock => mock.EnterScope ()).Return (r3);
      }

      _mockRepository.ReplayAll ();

      Assert.That (_mixinBuilder.Clear (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddMixin (typeof (object), origin), Is.SameAs (r4));
      Assert.That (_mixinBuilder.AddMixin (typeof (object)), Is.SameAs (r4));
      Assert.That (_mixinBuilder.AddMixin<string> (origin), Is.SameAs (r4));
      Assert.That (_mixinBuilder.AddMixin<string> (), Is.SameAs (r4));
      Assert.That (_mixinBuilder.AddMixins (origin, typeof (BT1Mixin1), typeof (BT1Mixin2)), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddMixins (typeof (BT1Mixin1), typeof (BT1Mixin2)), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddMixins<BT1Mixin1, BT1Mixin2> (origin), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddMixins<BT1Mixin1, BT1Mixin2> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> (origin), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddOrderedMixins (origin, typeof (BT1Mixin1), typeof (BT1Mixin2)), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddOrderedMixins (typeof (BT1Mixin1), typeof (BT1Mixin2)), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddOrderedMixins<BT1Mixin1, BT1Mixin2> (origin), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddOrderedMixins<BT1Mixin1, BT1Mixin2> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddOrderedMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> (origin), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddOrderedMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.EnsureMixin (typeof (object), origin), Is.SameAs (r4));
      Assert.That (_mixinBuilder.EnsureMixin (typeof (object)), Is.SameAs (r4));
      Assert.That (_mixinBuilder.EnsureMixin<string> (origin), Is.SameAs (r4));
      Assert.That (_mixinBuilder.EnsureMixin<string> (), Is.SameAs (r4));
      Assert.That (_mixinBuilder.EnsureMixins (origin, typeof (BT1Mixin1), typeof (BT1Mixin2)), Is.SameAs (r1));
      Assert.That (_mixinBuilder.EnsureMixins (typeof (BT1Mixin1), typeof (BT1Mixin2)), Is.SameAs (r1));
      Assert.That (_mixinBuilder.EnsureMixins<BT1Mixin1, BT1Mixin2> (origin), Is.SameAs (r1));
      Assert.That (_mixinBuilder.EnsureMixins<BT1Mixin1, BT1Mixin2> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.EnsureMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> (origin), Is.SameAs (r1));
      Assert.That (_mixinBuilder.EnsureMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddComposedInterface (typeof (IBT6Mixin1)), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddComposedInterface<IBT6Mixin1> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddComposedInterfaces (typeof (IBT6Mixin1), typeof (IBT6Mixin2)), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddComposedInterfaces<IBT6Mixin1, IBT6Mixin2> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddComposedInterfaces<IBT6Mixin1, IBT6Mixin2, IBT6Mixin3> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddComposedInterfaces<IBT6Mixin1, IBT6Mixin2, IBT6Mixin3> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.SuppressMixin (suppressionRuleStub), Is.SameAs (r1));
      Assert.That (_mixinBuilder.SuppressMixin (typeof (object)), Is.SameAs (r1));
      Assert.That (_mixinBuilder.SuppressMixin<string> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.SuppressMixins (typeof (BT1Mixin1), typeof (BT1Mixin2)), Is.SameAs (r1));
      Assert.That (_mixinBuilder.SuppressMixins<BT1Mixin1, BT1Mixin2> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.SuppressMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddMixinDependency (typeof (BT1Mixin1), typeof (BT1Mixin2)), Is.SameAs (r1));
      Assert.That (_mixinBuilder.AddMixinDependency<BT1Mixin1, BT1Mixin2> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.BuildClassContext (inheritedContexts), Is.SameAs (r5));
      Assert.That (_mixinBuilder.BuildClassContext (), Is.SameAs (r5));

      Assert.That (_mixinBuilder.ForClass<object> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.ForClass<string> (), Is.SameAs (r1));
      Assert.That (_mixinBuilder.BuildConfiguration (), Is.SameAs (r2));
      Assert.That (_mixinBuilder.EnterScope (), Is.SameAs (r3));

      _mockRepository.VerifyAll ();
    }

    private MixinContextBuilder CreatePartialMock ()
    {
      return _mockRepository.PartialMock<MixinContextBuilder> (_parentBuilderMock, typeof (object), _origin);
    }
  }
}
