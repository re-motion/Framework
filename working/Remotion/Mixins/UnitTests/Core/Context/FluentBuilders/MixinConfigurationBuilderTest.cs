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
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Mixins.UnitTests.Core.Context.FluentBuilders
{
  [TestFixture]
  public class MixinConfigurationBuilderTest
  {
    private MixinContextOrigin _mixinContextOrigin;

    [SetUp]
    public void SetUp ()
    {
      _mixinContextOrigin = MixinContextOriginObjectMother.Create();
    }

    [Test]
    public void Intitialization_WithoutParent ()
    {
      var builder = new MixinConfigurationBuilder (null);
      Assert.That (builder.ParentConfiguration, Is.Null);
      Assert.That (builder.ClassContextBuilders, Is.Empty);
      MixinConfiguration configuration = builder.BuildConfiguration();
      Assert.That (configuration.ClassContexts.Count, Is.EqualTo (0));
    }

    [Test]
    public void Intitialization_WithParent ()
    {
      var parent = new MixinConfiguration (new ClassContextCollection (ClassContextObjectMother.Create(typeof (string))));

      var builder = new MixinConfigurationBuilder (parent);
      Assert.That (builder.ParentConfiguration, Is.SameAs (parent));
      Assert.That (builder.ClassContextBuilders, Is.Empty);

      MixinConfiguration configuration = builder.BuildConfiguration ();
      Assert.That (configuration.ClassContexts.Count, Is.EqualTo (1));
      Assert.That (configuration.ClassContexts.ContainsWithInheritance (typeof (string)), Is.True);
    }

    [Test]
    public void ForClass_NonGeneric ()
    {
      var builder = new MixinConfigurationBuilder (null);
      ClassContextBuilder classBuilder = builder.ForClass (typeof (BaseType1));
      Assert.That (classBuilder.TargetType, Is.SameAs (typeof (BaseType1)));
      Assert.That (classBuilder.Parent, Is.SameAs (builder));
      Assert.That (builder.ClassContextBuilders, Has.Member(classBuilder));
    }

    [Test]
    public void ForClass_Twice()
    {
      var builder = new MixinConfigurationBuilder (null);
      ClassContextBuilder classBuilder = builder.ForClass (typeof (BaseType1));
      ClassContextBuilder classBuilder2 = builder.ForClass (typeof (BaseType1));
      Assert.That (classBuilder2, Is.SameAs (classBuilder));
    }

    [Test]
    public void ForClass_Generic ()
    {
      var builder = new MixinConfigurationBuilder (null);
      ClassContextBuilder classBuilder = builder.ForClass<BaseType1> ();
      Assert.That (classBuilder.TargetType, Is.SameAs (typeof (BaseType1)));
      Assert.That (classBuilder.Parent, Is.SameAs (builder));
      Assert.That (builder.ClassContextBuilders, Has.Member(classBuilder));
    }

    [Test]
    public void BuildConfiguration ()
    {
      var builder = new MixinConfigurationBuilder (null);
      builder.ForClass<BaseType1> ();
      MixinConfiguration configuration = builder.BuildConfiguration();
      Assert.That (configuration.ClassContexts.Count, Is.EqualTo (1));
      Assert.That (configuration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)), Is.True);
    }

    [Test]
    public void BuildConfiguration_IncludesParentConfiguration_WithClassContext_Unmodified ()
    {
      var existingContext = ClassContextObjectMother.Create(typeof (BaseType1), typeof (BT1Mixin1));
      var parentConfiguration = new MixinConfiguration (new ClassContextCollection (existingContext));

      var builder = new MixinConfigurationBuilder (parentConfiguration);

      MixinConfiguration configuration = builder.BuildConfiguration ();
      Assert.That (configuration.ClassContexts.Count, Is.EqualTo (1));

      var classContext = configuration.GetContext (typeof (BaseType1));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (BT1Mixin1)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (BT1Mixin2)), Is.False);
    }

    [Test]
    public void BuildConfiguration_IncludesParentConfiguration_WithClassContext_Modified ()
    {
      var existingContext = ClassContextObjectMother.Create(typeof (BaseType1), typeof (BT1Mixin1));
      var parentConfiguration = new MixinConfiguration (new ClassContextCollection (existingContext));

      var builder = new MixinConfigurationBuilder (parentConfiguration);
      builder.ForClass<BaseType1> ().AddMixin<BT1Mixin2> ();
      
      MixinConfiguration configuration = builder.BuildConfiguration ();
      Assert.That (configuration.ClassContexts.Count, Is.EqualTo (1));

      var classContext = configuration.GetContext (typeof (BaseType1));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (BT1Mixin1)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (BT1Mixin2)), Is.True);
    }

    [Test]
    public void BuildConfiguration_IncludesParentConfiguration_WithComposedInterfaces ()
    {
      var existingClassContext = new ClassContext (typeof (BaseType3), new MixinContext[0], new[] { typeof (IBaseType31) });
      var parentConfiguration = new MixinConfiguration (new ClassContextCollection (existingClassContext));
      
      var builder = new MixinConfigurationBuilder (parentConfiguration);

      MixinConfiguration configuration = builder.BuildConfiguration ();
      Assert.That (configuration.GetContext (typeof (BaseType3)).ComposedInterfaces, Has.Member (typeof (IBaseType31)));
    }

    [Test]
    public void EnterScope ()
    {
      MixinConfiguration previousConfiguration = MixinConfiguration.ActiveConfiguration;
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType4)), Is.False);
      using (new MixinConfigurationBuilder (null).ForClass<BaseType4> ().EnterScope ())
      {
        Assert.That (MixinConfiguration.ActiveConfiguration, Is.Not.SameAs (previousConfiguration));
        Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType4)), Is.True);
      }
      Assert.That (MixinConfiguration.ActiveConfiguration, Is.SameAs (previousConfiguration));
    }

    [Test]
    public void ClassContextInheritance_Base_FromParentConfiguration ()
    {
      MixinConfiguration parentConfiguration =
          new MixinConfigurationBuilder (null).ForClass<NullTarget>().AddMixin (typeof (NullMixin)).BuildConfiguration();
      MixinConfiguration configuration =
          new MixinConfigurationBuilder (parentConfiguration).ForClass<DerivedNullTarget>().AddMixin (typeof (NullMixin2)).BuildConfiguration();
      ClassContext derivedContext = configuration.GetContext (typeof (DerivedNullTarget));
      Assert.That (derivedContext.Mixins.Count, Is.EqualTo (2));
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
    }

    [Test]
    public void ClassContextInheritance_TypeDefinition_FromParentConfiguration ()
    {
      MixinConfiguration parentConfiguration =
          new MixinConfigurationBuilder (null).ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin)).BuildConfiguration ();
      MixinConfiguration configuration =
          new MixinConfigurationBuilder (parentConfiguration).ForClass<GenericTargetClass<int>> ().AddMixin (typeof (NullMixin2)).BuildConfiguration ();
      ClassContext derivedContext = configuration.GetContext (typeof (GenericTargetClass<int>));
      Assert.That (derivedContext.Mixins.Count, Is.EqualTo (2));
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
    }

    [Test]
    public void ClassContextInheritance_BaseAndTypeDefinition_FromParentConfiguration ()
    {
      MixinConfiguration parentConfiguration = new MixinConfigurationBuilder (null)
          .ForClass (typeof (DerivedGenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<int>)).AddMixin (typeof (NullMixin2))
          .ForClass (typeof (GenericTargetClass<int>)).AddMixin (typeof (NullMixin3))
          .BuildConfiguration ();
      MixinConfiguration configuration = new MixinConfigurationBuilder (parentConfiguration)
          .ForClass<DerivedGenericTargetClass<int>> ().AddMixin (typeof (NullMixin4))
          .BuildConfiguration ();
      ClassContext derivedContext = configuration.GetContext (typeof (DerivedGenericTargetClass<int>));
      Assert.That (derivedContext.Mixins.Count, Is.EqualTo (4));
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin3)), Is.True);
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin4)), Is.True);
    }

    [Test]
    public void ClassContextInheritance_WithOverrides_FromParentConfiguration ()
    {
      MixinConfiguration parentConfiguration = new MixinConfigurationBuilder (null)
          .ForClass (typeof (NullTarget)).AddMixin (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .BuildConfiguration ();
      MixinConfiguration configuration = new MixinConfigurationBuilder (parentConfiguration)
          .ForClass<DerivedNullTarget> ().AddMixin (typeof (DerivedNullMixin))
          .ForClass<GenericTargetClass<int>> ().AddMixin (typeof (DerivedNullMixin))
          .BuildConfiguration ();

      ClassContext derivedContext1 = configuration.GetContext (typeof (DerivedNullTarget));
      Assert.That (derivedContext1.Mixins.Count, Is.EqualTo (1));
      Assert.That (derivedContext1.Mixins.ContainsKey (typeof (DerivedNullMixin)), Is.True);
      Assert.That (derivedContext1.Mixins.ContainsKey (typeof (NullMixin)), Is.False);

      ClassContext derivedContext2 = configuration.GetContext (typeof (GenericTargetClass<int>));
      Assert.That (derivedContext2.Mixins.Count, Is.EqualTo (1));
      Assert.That (derivedContext2.Mixins.ContainsKey (typeof (DerivedNullMixin)), Is.True);
      Assert.That (derivedContext2.Mixins.ContainsKey (typeof (NullMixin)), Is.False);
    }

    [Test]
    public void ClassContextInheritance_Base_FromSameConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass<DerivedNullTarget> ().AddMixin (typeof (NullMixin2))
          .ForClass<NullTarget> ().AddMixin (typeof (NullMixin))
          .BuildConfiguration ();
      ClassContext derivedContext = configuration.GetContext (typeof (DerivedNullTarget));
      Assert.That (derivedContext.Mixins.Count, Is.EqualTo (2));
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
    }

    [Test]
    public void ClassContextInheritance_TypeDefinition_FromSameConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass<GenericTargetClass<int>> ().AddMixin (typeof (NullMixin2))
          .ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .BuildConfiguration ();
      ClassContext derivedContext = configuration.GetContext (typeof (GenericTargetClass<int>));
      Assert.That (derivedContext.Mixins.Count, Is.EqualTo (2));
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
    }

    [Test]
    public void ClassContextInheritance_BaseAndTypeDefinition_FromSameConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass<DerivedGenericTargetClass<int>> ().AddMixin (typeof (NullMixin4))
          .ForClass (typeof (DerivedGenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<int>)).AddMixin (typeof (NullMixin2))
          .ForClass (typeof (GenericTargetClass<int>)).AddMixin (typeof (NullMixin3))
          .BuildConfiguration ();
      ClassContext derivedContext = configuration.GetContext (typeof (DerivedGenericTargetClass<int>));
      Assert.That (derivedContext.Mixins.Count, Is.EqualTo (4));
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin3)), Is.True);
      Assert.That (derivedContext.Mixins.ContainsKey (typeof (NullMixin4)), Is.True);
    }

    [Test]
    public void ClassContextInheritance_WithOverrides_FromSameConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass (typeof (NullTarget)).AddMixin (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .ForClass<DerivedNullTarget> ().AddMixin (typeof (DerivedNullMixin))
          .ForClass<GenericTargetClass<int>> ().AddMixin (typeof (DerivedNullMixin))
          .BuildConfiguration ();

      ClassContext derivedContext1 = configuration.GetContext (typeof (DerivedNullTarget));
      Assert.That (derivedContext1.Mixins.Count, Is.EqualTo (1));
      Assert.That (derivedContext1.Mixins.ContainsKey (typeof (DerivedNullMixin)), Is.True);
      Assert.That (derivedContext1.Mixins.ContainsKey (typeof (NullMixin)), Is.False);

      ClassContext derivedContext2 = configuration.GetContext (typeof (GenericTargetClass<int>));
      Assert.That (derivedContext2.Mixins.Count, Is.EqualTo (1));
      Assert.That (derivedContext2.Mixins.ContainsKey (typeof (DerivedNullMixin)), Is.True);
      Assert.That (derivedContext2.Mixins.ContainsKey (typeof (NullMixin)), Is.False);
    }

    [Test]
    public void AddMixinToClass ()
    {
      var mockRepository = new MockRepository();
      var builder = mockRepository.StrictMock<MixinConfigurationBuilder>((MixinConfiguration) null);

      var targetType = typeof (object);
      var mixinType = typeof (string);
      var explicitDependencies = new[] { typeof (int) };
      var suppressedMixins = new[] { typeof (double) };

      var classBuilderMock = mockRepository.StrictMock<ClassContextBuilder> (builder, targetType);
      var mixinBuilderMock = mockRepository.StrictMock<MixinContextBuilder> (classBuilderMock, mixinType, _mixinContextOrigin);

      using (mockRepository.Ordered ())
      {
        builder
            .Expect (
                mock => mock.AddMixinToClass (
                    MixinKind.Extending, targetType, mixinType, MemberVisibility.Private, explicitDependencies, suppressedMixins, _mixinContextOrigin))
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        builder.Expect (mock => mock.ForClass (targetType)).Return (classBuilderMock);
        classBuilderMock.Expect (mock => mock.AddMixin (mixinType, _mixinContextOrigin)).Return (mixinBuilderMock);
        mixinBuilderMock.Expect (mock => mock.OfKind (MixinKind.Extending)).Return (mixinBuilderMock);
        mixinBuilderMock.Expect (mock => mock.WithDependencies (explicitDependencies)).Return (mixinBuilderMock);
        mixinBuilderMock.Expect (mock => mock.ReplaceMixins (suppressedMixins)).Return (mixinBuilderMock);
      }

      mockRepository.ReplayAll ();
      var returnedBuilder = builder.AddMixinToClass (
          MixinKind.Extending, targetType, mixinType, MemberVisibility.Private, explicitDependencies, suppressedMixins, _mixinContextOrigin);
      Assert.That (returnedBuilder, Is.SameAs (builder));
      mockRepository.VerifyAll ();
    }

    [Test]
    [MethodImpl (MethodImplOptions.NoInlining)]
    public void AddMixinToClass_WithoutOrigin ()
    {
      var mockRepository = new MockRepository ();
      var builder = mockRepository.StrictMock<MixinConfigurationBuilder> ((MixinConfiguration) null);

      var targetType = typeof (object);
      var mixinType = typeof (string);
      var explicitDependencies = new[] { typeof (int) };
      var suppressedMixins = new[] { typeof (double) };

      var expectedOrigin = MixinContextOrigin.CreateForMethod (MethodBase.GetCurrentMethod());

      builder
          .Expect (
              mock => mock.AddMixinToClass (
                  MixinKind.Extending, targetType, mixinType, MemberVisibility.Private, explicitDependencies, suppressedMixins, expectedOrigin))
          .Return (builder);

      mockRepository.ReplayAll ();

      var returnedBuilder = builder.AddMixinToClass (
          MixinKind.Extending, targetType, mixinType, MemberVisibility.Private, explicitDependencies, suppressedMixins);

      mockRepository.VerifyAll ();
      Assert.That (returnedBuilder, Is.SameAs (builder));
    }

    [Test]
    public void AddMixinToClass_Used ()
    {
      var mockRepository = new MockRepository ();
      var builder = mockRepository.StrictMock<MixinConfigurationBuilder> ((MixinConfiguration) null);

      Type targetType = typeof (object);
      Type mixinType = typeof (string);
      var explicitDependencies = new Type[0];
      var suppressedMixins = new Type[0];

      var classBuilderMock = mockRepository.StrictMock<ClassContextBuilder> (builder, targetType);
      var mixinBuilderMock = mockRepository.StrictMock<MixinContextBuilder> (classBuilderMock, mixinType, _mixinContextOrigin);

      using (mockRepository.Ordered ())
      {
        builder
            .Expect (
                mock => mock.AddMixinToClass (
                    MixinKind.Used, targetType, mixinType, MemberVisibility.Private, explicitDependencies, suppressedMixins, _mixinContextOrigin))
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        builder.Expect (mock => mock.ForClass (targetType)).Return (classBuilderMock);
        classBuilderMock.Expect (mock => mock.AddMixin (mixinType, _mixinContextOrigin)).Return (mixinBuilderMock);
        mixinBuilderMock.Expect (mock => mock.OfKind (MixinKind.Used)).Return (mixinBuilderMock);
        mixinBuilderMock.Expect (mock => mock.WithDependencies (explicitDependencies)).Return (mixinBuilderMock);
        mixinBuilderMock.Expect (mock => mock.ReplaceMixins (suppressedMixins)).Return (mixinBuilderMock);
      }
      mockRepository.ReplayAll ();
      
      var returnedBuilder = builder.AddMixinToClass (
          MixinKind.Used, targetType, mixinType, MemberVisibility.Private, explicitDependencies, suppressedMixins, _mixinContextOrigin);

      mockRepository.VerifyAll ();
      Assert.That (returnedBuilder, Is.SameAs (builder));
    }

    [Test]
    public void AddMixinToClass_PublicMemberVisibility ()
    {
      var mockRepository = new MockRepository ();
      var builder = mockRepository.StrictMock<MixinConfigurationBuilder> ((MixinConfiguration) null);

      Type targetType = typeof (object);
      Type mixinType = typeof (string);
      var explicitDependencies = new Type[0];
      var suppressedMixins = new Type[0];

      var classBuilderMock = mockRepository.StrictMock<ClassContextBuilder> (builder, targetType);
      var mixinBuilderMock = mockRepository.StrictMock<MixinContextBuilder> (classBuilderMock, mixinType, _mixinContextOrigin);

      using (mockRepository.Ordered ())
      {
        builder
            .Expect (
                mock => mock.AddMixinToClass (
                    MixinKind.Used, targetType, mixinType, MemberVisibility.Public, explicitDependencies, suppressedMixins, _mixinContextOrigin))
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        builder.Expect (mock => mock.ForClass (targetType)).Return (classBuilderMock);
        classBuilderMock.Expect (mock => mock.AddMixin (mixinType, _mixinContextOrigin)).Return (mixinBuilderMock);
        mixinBuilderMock.Expect (mock => mock.OfKind (MixinKind.Used)).Return (mixinBuilderMock);
        mixinBuilderMock.Expect (mock => mock.WithDependencies (explicitDependencies)).Return (mixinBuilderMock);
        mixinBuilderMock.Expect (mock => mock.ReplaceMixins (suppressedMixins)).Return (mixinBuilderMock);
      }
      mockRepository.ReplayAll ();

      var returnedBuilder = builder.AddMixinToClass (
          MixinKind.Used, targetType, mixinType, MemberVisibility.Public, explicitDependencies, suppressedMixins, _mixinContextOrigin);

      mockRepository.VerifyAll ();
      Assert.That (returnedBuilder, Is.SameAs (builder));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Mixin type 'System.Int32' applied to target class 'System.Object' "
        + "suppresses itself.")]
    public void AddMixinToClass_WithSelfSuppressor ()
    {
      var builder = new MixinConfigurationBuilder (null);
      builder.AddMixinToClass (
          MixinKind.Extending, typeof (object), typeof (int), MemberVisibility.Private, new Type[0], new[] { typeof (int) }, _mixinContextOrigin);
    }

    [Test]
    public void AddMixinToClass_WithBaseSuppressor ()
    {
      var builder = new MixinConfigurationBuilder (null);
      builder.AddMixinToClass (
          MixinKind.Extending,
          typeof (object),
          typeof (DerivedNullMixin),
          MemberVisibility.Private,
          new Type[0],
          new[] { typeof (NullMixin) },
          _mixinContextOrigin);
    }
  }
}
