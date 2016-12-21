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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Context.Serialization;
using Remotion.Mixins.Context.Suppression;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.Context
{
  [TestFixture]
  public class ClassContextTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Object was tried to be added twice", MatchType = MessageMatch.Contains)]
    public void ConstructorThrowsOnDuplicateMixinContexts ()
    {
      ClassContextObjectMother.Create(typeof (string), typeof (object), typeof (object));
    }

    [Test]
    public void ConstructorWithMixinParameters()
    {
      var context = ClassContextObjectMother.Create(typeof (BaseType1), typeof (BT1Mixin1), typeof (BT1Mixin2));
      Assert.That (context.Mixins.Count, Is.EqualTo (2));
      Assert.That (context.Mixins.ContainsKey (typeof (BT1Mixin1)), Is.True);
      Assert.That (context.Mixins.ContainsKey (typeof (BT1Mixin2)), Is.True);
      Assert.That (context.Mixins.ContainsKey (typeof (BT2Mixin1)), Is.False);
    }

    [Test]
    public void ConstructorWithMixinParameters_DefaultValues ()
    {
      var context = ClassContextObjectMother.Create(typeof (BaseType1));
      Assert.That (context.Mixins, Is.Empty);
    }

    [Test]
    public void Mixins ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (BaseType7));

      Assert.That (classContext.Mixins.ContainsKey (typeof (BT7Mixin1)), Is.False);
      MixinContext mixinContext = classContext.Mixins[typeof (BT7Mixin1)];
      Assert.That (mixinContext, Is.Null);

      classContext = ClassContextObjectMother.Create(typeof (BaseType7), typeof (BT7Mixin1));
      Assert.That (classContext.Mixins.ContainsKey (typeof (BT7Mixin1)), Is.True);
      mixinContext = classContext.Mixins[typeof (BT7Mixin1)];
      Assert.That (classContext.Mixins[typeof (BT7Mixin1)], Is.SameAs (mixinContext));
    }

    [Test]
    public void ComposedInterfaces_Empty()
    {
      var context = new ClassContext (typeof (BaseType5), new MixinContext[0], new Type[0]);
      Assert.That (context.ComposedInterfaces.Count, Is.EqualTo (0));
      Assert.That (context.ComposedInterfaces, Is.Empty);
    }

    [Test]
    public void ComposedInterfaces_NonEmpty ()
    {
      var context = new ClassContext (typeof (BaseType5), new MixinContext[0], new[] { typeof (IBT5MixinC1) });
      Assert.That (context.ComposedInterfaces.Count, Is.EqualTo (1));
      Assert.That (context.ComposedInterfaces, Has.Member (typeof (IBT5MixinC1)));
      Assert.That (context.ComposedInterfaces, Has.Member (typeof (IBT5MixinC1)));
    }

    [Test]
    public void IsEmpty_True ()
    {
      var context = ClassContextObjectMother.Create(typeof (BaseType1));
      Assert.That (context.IsEmpty (), Is.True);
    }

    [Test]
    public void IsEmpty_False_Mixins ()
    {
      var context = ClassContextObjectMother.Create(typeof (BaseType1), typeof (BT1Mixin1));
      Assert.That (context.IsEmpty (), Is.False);
    }

    [Test]
    public void IsEmpty_False_ComposedInterfaces ()
    {
      var context = new ClassContext (typeof (BaseType1), new MixinContext[0], new[] { typeof (ICBT6Mixin3) });
      Assert.That (context.IsEmpty (), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The mixin type System.Object was tried to be added twice.\r\n"
                                                                      + "Parameter name: mixinTypes")]
    public void ConstructorThrows_OnDuplicateMixinTypes ()
    {
      ClassContextObjectMother.Create(typeof (string), typeof (object), typeof (object));
    }

    [Test]
    public void DuplicateComposedInterfacesAreIgnored ()
    {
      var context = new ClassContext (typeof (BaseType5), new MixinContext[0], new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC1) });
      Assert.That (context.ComposedInterfaces.Count, Is.EqualTo (1));
    }

    [Test]
    public void SpecializeWithTypeArguments ()
    {
      ClassContext original = new ClassContextBuilder (typeof (List<>)).AddMixin<BT1Mixin1>().WithDependency<IBaseType2>().BuildClassContext();

      ClassContext specialized = original.SpecializeWithTypeArguments (new[] { typeof (int) });
      Assert.That (specialized, Is.Not.Null);
      Assert.That (specialized.Type, Is.EqualTo (typeof (List<int>)));
      Assert.That (specialized.Mixins.ContainsKey (typeof (BT1Mixin1)), Is.True);
      Assert.That (specialized.Mixins[typeof (BT1Mixin1)].ExplicitDependencies, Has.Member (typeof (IBaseType2)));
    }

    [Test]
    public void GenericTypesNotTransparentlyConvertedToTypeDefinitions ()
    {
      var context = ClassContextObjectMother.Create(typeof (List<int>));
      Assert.That (context.Type, Is.EqualTo (typeof (List<int>)));
    }

    [Test]
    public void ContainsAssignableMixin ()
    {
      var context = ClassContextObjectMother.Create(typeof (object), typeof (IList<int>));

      Assert.That (context.Mixins.ContainsKey (typeof (IList<int>)), Is.True);
      Assert.That (context.Mixins.ContainsAssignableMixin (typeof (IList<int>)), Is.True);

      Assert.That (context.Mixins.ContainsKey (typeof (ICollection<int>)), Is.False);
      Assert.That (context.Mixins.ContainsAssignableMixin (typeof (ICollection<int>)), Is.True);

      Assert.That (context.Mixins.ContainsKey (typeof (object)), Is.False);
      Assert.That (context.Mixins.ContainsAssignableMixin (typeof (object)), Is.True);

      Assert.That (context.Mixins.ContainsKey (typeof (List<int>)), Is.False);
      Assert.That (context.Mixins.ContainsAssignableMixin (typeof (List<int>)), Is.False);

      Assert.That (context.Mixins.ContainsKey (typeof (IList<>)), Is.False);
      Assert.That (context.Mixins.ContainsAssignableMixin (typeof (List<>)), Is.False);
    }

    [Test]
    public void CloneForSpecificType ()
    {
      var mixins = new[] { CreateBT1Mixin1Context(), CreateBT2Mixin2Context() };
      var interfaces = new[] { typeof (ICBT6Mixin1), typeof (ICBT6Mixin2)};
      var source = new ClassContext (typeof (BaseType1), mixins, interfaces);
      
      var clone = source.CloneForSpecificType (typeof (BaseType2));

      Assert.That (clone, Is.Not.EqualTo (source));
      Assert.That(clone.Mixins, Is.EquivalentTo(mixins));
      Assert.That (clone.ComposedInterfaces, Is.EquivalentTo (interfaces));
      Assert.That (clone.Type, Is.EqualTo (typeof (BaseType2)));
      Assert.That (source.Type, Is.EqualTo (typeof (BaseType1)));
    }

    [Test]
    public void Equals_True ()
    {
      var c1 =
          new ClassContext (
              typeof (BaseType1),
              new[] { CreateBT1Mixin1Context(), CreateBT2Mixin2Context() },
              new[] {typeof (IBT5MixinC1), typeof (IBT5MixinC2)});
      var c2 =
          new ClassContext (
              typeof (BaseType1),
              new[] { CreateBT1Mixin1Context(), CreateBT2Mixin2Context() },
              new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      var c3 =
          new ClassContext (
              typeof (BaseType1),
              new[] { CreateBT2Mixin2Context(), CreateBT1Mixin1Context() },
              new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      var c4 =
          new ClassContext (
              typeof (BaseType1),
              new[] { CreateBT1Mixin1Context(), CreateBT2Mixin2Context() },
              new[] { typeof (IBT5MixinC2), typeof (IBT5MixinC1) });

      Assert.That (c1, Is.EqualTo (c1));
      Assert.That (c2, Is.EqualTo (c1));
      Assert.That (c3, Is.EqualTo (c1));
      Assert.That (c4, Is.EqualTo (c1));

      var c5 = ClassContextObjectMother.Create(typeof (BaseType1));
      var c6 = ClassContextObjectMother.Create(typeof (BaseType1));
      Assert.That (c6, Is.EqualTo (c5));
    }

    [Test]
    public void Equals_False_ClassType ()
    {
      var c1 = ClassContextObjectMother.Create(typeof (BaseType1));
      var c2 = ClassContextObjectMother.Create(typeof (BaseType2));

      Assert.That (c2, Is.Not.EqualTo (c1));
    }

    [Test]
    public void Equals_False_Mixins ()
    {
      var c1 = ClassContextObjectMother.Create(typeof (BaseType1), typeof (BT1Mixin1));
      var c3 = ClassContextObjectMother.Create(typeof (BaseType1), typeof (BT1Mixin2));
      var c4 = ClassContextObjectMother.Create(typeof (BaseType1), typeof (BT1Mixin1), typeof (BT1Mixin2));

      Assert.That (c3, Is.Not.EqualTo (c1));
      Assert.That (c4, Is.Not.EqualTo (c1));
      Assert.That (c4, Is.Not.EqualTo (c3));
    }

    [Test]
    public void Equals_False_ComposedInterfaces ()
    {
      var c1 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[0],
              new[] { typeof (IBT5MixinC1) });
      var c2 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[0],
              new[] { typeof (IBT5MixinC2) });
      var c3 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[0],
              new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });

      Assert.That (c2, Is.Not.EqualTo (c1));
      Assert.That (c3, Is.Not.EqualTo (c1));
      Assert.That (c3, Is.Not.EqualTo (c2));
    }

    [Test]
    public void GetHashCode_Equal ()
    {
      var c1 =
          new ClassContext (
              typeof (BaseType1),
              new[] { CreateBT1Mixin1Context(), CreateBT2Mixin2Context() },
              new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      var c2 =
          new ClassContext (
              typeof (BaseType1),
              new[] { CreateBT1Mixin1Context(), CreateBT2Mixin2Context() },
              new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      var c3 =
          new ClassContext (
              typeof (BaseType1),
              new[] { CreateBT2Mixin2Context(), CreateBT1Mixin1Context() },
              new[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      var c4 =
          new ClassContext (
              typeof (BaseType1),
              new[] { CreateBT1Mixin1Context(), CreateBT2Mixin2Context() },
              new[] { typeof (IBT5MixinC2), typeof (IBT5MixinC1) });

      Assert.That (c2.GetHashCode (), Is.EqualTo (c1.GetHashCode ()));
      Assert.That (c3.GetHashCode (), Is.EqualTo (c1.GetHashCode ()));
      Assert.That (c4.GetHashCode (), Is.EqualTo (c1.GetHashCode ()));

      var c5 = ClassContextObjectMother.Create(typeof (BaseType1));
      var c6 = ClassContextObjectMother.Create(typeof (BaseType1));
      Assert.That (c6.GetHashCode (), Is.EqualTo (c5.GetHashCode ()));
    }

    [Test]
    public void Serialize()
    {
      var serializer = MockRepository.GenerateMock<IClassContextSerializer> ();
      var context = new ClassContext (typeof (BaseType1), new[] { CreateBT1Mixin1Context() }, new[] { typeof (int), typeof (string) });
      context.Serialize (serializer);

      serializer.AssertWasCalled (mock => mock.AddClassType (context.Type));
      serializer.AssertWasCalled (mock => mock.AddMixins (context.Mixins));
      serializer.AssertWasCalled (mock => mock.AddComposedInterfaces (context.ComposedInterfaces));
    }

    [Test]
    public void Deserialize ()
    {
      var expectedContext = new ClassContext (typeof (BaseType1), new[] { CreateBT1Mixin1Context() }, new[] { typeof (int), typeof (string) });

      var deserializer = MockRepository.GenerateMock<IClassContextDeserializer> ();
      deserializer.Expect (mock => mock.GetClassType ()).Return (expectedContext.Type);
      deserializer.Expect (mock => mock.GetMixins ()).Return (expectedContext.Mixins);
      deserializer.Expect (mock => mock.GetComposedInterfaces ()).Return (expectedContext.ComposedInterfaces);

      var context = ClassContext.Deserialize (deserializer);

      Assert.That (context, Is.EqualTo (expectedContext));
    }

    [Test]
    public void Serialization_IsUpToDate ()
    {
      var properties = typeof (ClassContext).GetProperties (BindingFlags.Public | BindingFlags.Instance);
      Assert.That (typeof (IClassContextSerializer).GetMethods ().Length, Is.EqualTo (properties.Length));
      Assert.That (typeof (IClassContextDeserializer).GetMethods ().Length, Is.EqualTo (properties.Length));
    }

    [Test]
    public void SuppressMixins ()
    {
      var ruleStub1 = MockRepository.GenerateStub<IMixinSuppressionRule> ();
      ruleStub1
          .Stub (stub => stub.RemoveAffectedMixins (Arg<Dictionary<Type, MixinContext>>.Is.Anything))
          .WhenCalled (mi => ((Dictionary<Type, MixinContext>) mi.Arguments[0]).Remove (typeof (int)));
      
      var ruleStub2 = MockRepository.GenerateStub<IMixinSuppressionRule> ();
      ruleStub2
          .Stub (stub => stub.RemoveAffectedMixins (Arg<Dictionary<Type, MixinContext>>.Is.Anything))
          .WhenCalled (mi => ((Dictionary<Type, MixinContext>) mi.Arguments[0]).Remove (typeof (double)));

      var original = ClassContextObjectMother.Create(typeof (NullTarget), typeof (int), typeof (double), typeof (string));
      
      var result = original.SuppressMixins (new[] { ruleStub1, ruleStub2 });

      Assert.That (result.Mixins.Select (mc => mc.MixinType).ToArray (), Is.EquivalentTo (new[] { typeof (string) }));
    }

    [Test]
    public void ApplyMixinDependencies ()
    {
      var originalMixinContext1 = MixinContextObjectMother.Create (mixinType: typeof (string), explicitDependencies: new[] { typeof (int) });
      var originalMixinContext2 = MixinContextObjectMother.Create (mixinType: typeof (DateTime), explicitDependencies: new[] { typeof (double) });
      var originalMixinContext3 = MixinContextObjectMother.Create (mixinType: typeof (object), explicitDependencies: new[] { typeof (decimal) });
      var originalClassContext = ClassContextObjectMother.Create (typeof (NullTarget), originalMixinContext1, originalMixinContext2, originalMixinContext3);

      var dependencies = 
          new[] 
          { 
            new MixinDependencySpecification (typeof (string), new[] { typeof (int), typeof (float), typeof (long) }),
            new MixinDependencySpecification (typeof (object), new[] { typeof (byte) }),
            new MixinDependencySpecification (typeof (string), new[] { typeof (Enum) })
          };

      var result = originalClassContext.ApplyMixinDependencies (dependencies);

      Assert.That (result, Is.Not.EqualTo (originalClassContext));
      var expectedResult = new ClassContext (
          originalClassContext.Type,
          new[]
          {
              new MixinContext (
                  originalMixinContext1.MixinKind,
                  originalMixinContext1.MixinType,
                  originalMixinContext1.IntroducedMemberVisibility,
                  new[] { typeof (int), typeof (float), typeof (long), typeof (Enum) },
                  originalMixinContext1.Origin),
              originalMixinContext2,
              new MixinContext (
                  originalMixinContext3.MixinKind,
                  originalMixinContext3.MixinType,
                  originalMixinContext3.IntroducedMemberVisibility,
                  new[] { typeof (decimal), typeof (byte) },
                  originalMixinContext3.Origin)
          },
          originalClassContext.ComposedInterfaces);
      Assert.That (result, Is.EqualTo (expectedResult));

      Assert.That (originalClassContext.Mixins[typeof (string)].ExplicitDependencies, Has.No.Member (typeof (float)), "Original is not changed");
    }

    [Test]
    public void ApplyMixinDependencies_NotFound ()
    {
      var originalMixinContext1 = MixinContextObjectMother.Create (mixinType: typeof (object), explicitDependencies: new[] { typeof (int) });
      var originalClassContext = ClassContextObjectMother.Create (typeof (NullTarget), originalMixinContext1);
      var dependencies = new[] { new MixinDependencySpecification (typeof (string), new[] { typeof (float) })};

      Assert.That (
          () => originalClassContext.ApplyMixinDependencies (dependencies), 
          Throws.InvalidOperationException.With.Message.EqualTo (
              "The mixin 'System.String' is not configured for class 'Remotion.Mixins.UnitTests.Core.TestDomain.NullTarget'."));
    }

    [Test]
    public void ApplyMixinDependencies_GenericMixins ()
    {
      var originalMixinContext1 = MixinContextObjectMother.Create (mixinType: typeof (List<>), explicitDependencies: new[] { typeof (int) });
      var originalMixinContext2 = MixinContextObjectMother.Create (mixinType: typeof (Dictionary<int, string>), explicitDependencies: new[] { typeof (double) });
      var originalClassContext = ClassContextObjectMother.Create (typeof (NullTarget), originalMixinContext1, originalMixinContext2);

      var dependencies =
          new[] 
          { 
            new MixinDependencySpecification (typeof (List<>), new[] { typeof (float) }),
            new MixinDependencySpecification (typeof (Dictionary<,>), new[] { typeof (byte) })
          };

      var result = originalClassContext.ApplyMixinDependencies (dependencies);

      var expectedResult = new ClassContext (
          originalClassContext.Type,
          new[]
          {
              new MixinContext (
                  originalMixinContext1.MixinKind,
                  originalMixinContext1.MixinType,
                  originalMixinContext1.IntroducedMemberVisibility,
                  new[] { typeof (int), typeof (float) },
                  originalMixinContext1.Origin),
              new MixinContext (
                  originalMixinContext2.MixinKind,
                  originalMixinContext2.MixinType,
                  originalMixinContext2.IntroducedMemberVisibility,
                  new[] { typeof (double), typeof (byte) },
                  originalMixinContext2.Origin)
          },
          originalClassContext.ComposedInterfaces);
      Assert.That (result, Is.EqualTo (expectedResult));
    }

    [Test]
    public void ApplyMixinDependencies_GenericMixin_NotFound ()
    {
      var originalMixinContext1 = MixinContextObjectMother.Create (mixinType: typeof (List<>), explicitDependencies: new[] { typeof (int) });
      var originalClassContext = ClassContextObjectMother.Create (typeof (NullTarget), originalMixinContext1);
      var dependencies = new[] { new MixinDependencySpecification (typeof (List<int>), new[] { typeof (float) }) };

      Assert.That (
          () => originalClassContext.ApplyMixinDependencies (dependencies),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "The mixin 'System.Collections.Generic.List`1[System.Int32]' is not configured for class "
              + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullTarget'."));
    }

    [Test]
    public void ApplyMixinDependencies_GenericMixin_Ambiguous ()
    {
      var originalMixinContext1 = MixinContextObjectMother.Create (mixinType: typeof (List<int>), explicitDependencies: new[] { typeof (int) });
      var originalMixinContext2 = MixinContextObjectMother.Create (mixinType: typeof (List<string>), explicitDependencies: new[] { typeof (int) });
      var originalClassContext = ClassContextObjectMother.Create (typeof (NullTarget), originalMixinContext1, originalMixinContext2);
      var dependencies = new[] { new MixinDependencySpecification (typeof (List<>), new[] { typeof (float) }) };

      Assert.That (
          () => originalClassContext.ApplyMixinDependencies (dependencies),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "The dependency specification for 'System.Collections.Generic.List`1[T]' applied to class "
              + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullTarget' is ambiguous; matching mixins: "
              + "'System.Collections.Generic.List`1[System.Int32]', 'System.Collections.Generic.List`1[System.String]'."));
    }

    private static MixinContext CreateBT1Mixin1Context ()
    {
      return MixinContextObjectMother.Create (mixinType: typeof (BT1Mixin1));
    }

    private static MixinContext CreateBT2Mixin2Context ()
    {
      return MixinContextObjectMother.Create (mixinType: typeof (BT1Mixin2));
    }
  }
}
