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
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
{
  [TestFixture]
  public class AttributeDefinitionBuilderTest
  {
    [Test]
    public void Attributes ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.BuildUnvalidatedDefinition(typeof(ClassWithManyAttributes),
                                                                                             typeof(ClassWithManyAttributes));
      MixinDefinition mixin = targetClass.Mixins[typeof(ClassWithManyAttributes)];

      CheckAttributes(targetClass);
      CheckAttributes(mixin);

      CheckAttributes(targetClass.Methods[typeof(ClassWithManyAttributes).GetMethod("Foo")]);
      CheckAttributes(mixin.Methods[typeof(ClassWithManyAttributes).GetMethod("Foo")]);
    }

    [Test]
    public void ExtendsAttributeIsIgnored ()
    {
      MixinDefinition bt1m1 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType1)).Mixins[typeof(BT1Mixin1)];
      Assert.That(bt1m1.CustomAttributes.ContainsKey(typeof(ExtendsAttribute)), Is.False);
    }

    [Test]
    public void UsesAttributeIsIgnored ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType3));
      Assert.That(bt1.CustomAttributes.ContainsKey(typeof(UsesAttribute)), Is.False);
    }

    [Test]
    public void SuppressAttributeIsNotIgnored ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.GetActiveTargetClassDefinition_Force(typeof(ClassWithSuppressAttribute));
      Assert.That(classDefinition.CustomAttributes.ContainsKey(typeof(SuppressAttributesAttribute)), Is.True);
    }

    [Test]
    public void OverrideAttributeIsIgnored ()
    {
      MixinDefinition bt1m1 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType1)).Mixins[typeof(BT1Mixin1)];
      Assert.That(bt1m1.Methods[typeof(BT1Mixin1).GetMethod("VirtualMethod")].CustomAttributes.ContainsKey(typeof(OverrideTargetAttribute)), Is.False);
    }

    [Test]
    public void InternalAttributesAreIgnored ()
    {
      var context = ClassContextObjectMother.Create(typeof(ClassWithInternalAttribute));
      var definition = TargetClassDefinitionFactory.CreateAndValidate(context);
      Assert.That(definition.CustomAttributes.ContainsKey(typeof(InternalStuffAttribute)), Is.False);
    }

    [Test]
    public void CopyAttributes_OnClass ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(MixinIndirectlyAddingAttribute)).EnterScope())
      {
        MixinDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget)).Mixins[typeof(MixinIndirectlyAddingAttribute)];
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(CopyCustomAttributesAttribute)), Is.False);
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(AttributeWithParameters)), Is.True);

        var attributes = new List<AttributeDefinition>(definition.CustomAttributes[typeof(AttributeWithParameters)]);

        Assert.That(attributes.Count, Is.EqualTo(1));
        Assert.That(attributes[0].IsCopyTemplate, Is.True);

        Assert.That(attributes[0].AttributeType, Is.EqualTo(typeof(AttributeWithParameters)));
        Assert.That(
            attributes[0].Data.Constructor, Is.EqualTo(typeof(AttributeWithParameters).GetConstructor(new[] { typeof(int), typeof(string) })));
        Assert.That(attributes[0].DeclaringDefinition, Is.EqualTo(definition));

        Assert.That(attributes[0].Data.ConstructorArguments.Count, Is.EqualTo(2));
        Assert.That(attributes[0].Data.ConstructorArguments[0], Is.EqualTo(1));
        Assert.That(attributes[0].Data.ConstructorArguments[1], Is.EqualTo("bla"));

        var namedArgumentData = attributes[0].Data.NamedArguments.Select(n => new { n.MemberInfo, n.Value }).ToArray();
        var expectedNamedArgumentData =
            new[]
            {
                new { MemberInfo = (MemberInfo)typeof(AttributeWithParameters).GetField("Field"), Value = (object)5 },
                new { MemberInfo = (MemberInfo)typeof(AttributeWithParameters).GetProperty("Property"), Value = (object)4 }
            };
        Assert.That(namedArgumentData, Is.EquivalentTo(expectedNamedArgumentData));
      }
    }

    [Test]
    public void CopyFilteredAttributes_OnClass ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(MixinIndirectlyAddingFilteredAttributes)).EnterScope())
      {
        MixinDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget)).Mixins[typeof(MixinIndirectlyAddingFilteredAttributes)];
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(CopyCustomAttributesAttribute)), Is.False);
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(AttributeWithParameters)), Is.False);
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(AttributeWithParameters2)), Is.True);
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(AttributeWithParameters3)), Is.True);

        var attributes = new List<AttributeDefinition>(definition.CustomAttributes[typeof(AttributeWithParameters2)]);
        Assert.That(attributes.Count, Is.EqualTo(2));

        attributes = new List<AttributeDefinition>(definition.CustomAttributes[typeof(AttributeWithParameters3)]);
        Assert.That(attributes.Count, Is.EqualTo(1));
      }
    }

    [Test]
    public void CopyNonInheritedAttributes ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(MixinIndirectlyAddingNonInheritedAttribute)).EnterScope())
      {
        MixinDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget)).Mixins[typeof(MixinIndirectlyAddingNonInheritedAttribute)];
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(CopyCustomAttributesAttribute)), Is.False);
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(NonInheritableAttribute)), Is.True);
      }
    }

    [Test]
    public void CopyNonInheritedAttributesFromSelf ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(MixinIndirectlyAddingNonInheritedAttributeFromSelf)).EnterScope())
      {
        MixinDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget)).Mixins[typeof(MixinIndirectlyAddingNonInheritedAttributeFromSelf)];
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(CopyCustomAttributesAttribute)), Is.False);
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(NonInheritableAttribute)), Is.True);
      }
    }

    [Test]
    public void CopyNonInheritedAttributesFromSelf_DosntIntroduceDuplicates ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(MixinIndirectlyAddingInheritedAttributeFromSelf)).EnterScope())
      {
        MixinDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget)).Mixins[typeof(MixinIndirectlyAddingInheritedAttributeFromSelf)];
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(AttributeWithParameters)), Is.True);
        Assert.That(new List<AttributeDefinition>(definition.CustomAttributes[typeof(AttributeWithParameters)]).Count, Is.EqualTo(1));
      }
    }

    [Test]
    public void CopyAttributes_OnMember ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(MixinIndirectlyAddingAttribute)).EnterScope())
      {
        MethodDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget))
            .Mixins[typeof(MixinIndirectlyAddingAttribute)]
            .Methods[typeof(MixinIndirectlyAddingAttribute).GetMethod("ToString")];

        Assert.That(definition.CustomAttributes.ContainsKey(typeof(CopyCustomAttributesAttribute)), Is.False);
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(AttributeWithParameters)), Is.True);

        var attributes = new List<AttributeDefinition>(definition.CustomAttributes[typeof(AttributeWithParameters)]);

        Assert.That(attributes.Count, Is.EqualTo(1));
        Assert.That(attributes[0].AttributeType, Is.EqualTo(typeof(AttributeWithParameters)));
        Assert.That(attributes[0].Data.Constructor, Is.EqualTo(typeof(AttributeWithParameters).GetConstructor(new[] { typeof(int) })));

        Assert.That(attributes[0].IsCopyTemplate, Is.True);
        Assert.That(attributes[0].Data.ConstructorArguments.Count, Is.EqualTo(1));
        Assert.That(attributes[0].Data.ConstructorArguments[0], Is.EqualTo(4));

        Assert.That(attributes[0].Data.NamedArguments.Count, Is.EqualTo(0));
      }
    }

    [Test]
    public void CopyFilteredAttributes_OnMember ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(MixinIndirectlyAddingFilteredAttributes)).EnterScope())
      {
        MethodDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget))
            .Mixins[typeof(MixinIndirectlyAddingFilteredAttributes)]
            .Methods[typeof(MixinIndirectlyAddingFilteredAttributes).GetMethod("ToString")];

        Assert.That(definition.CustomAttributes.ContainsKey(typeof(CopyCustomAttributesAttribute)), Is.False);
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(AttributeWithParameters)), Is.False);
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(AttributeWithParameters2)), Is.True);
        Assert.That(definition.CustomAttributes.ContainsKey(typeof(AttributeWithParameters3)), Is.True);

        var attributes = new List<AttributeDefinition>(definition.CustomAttributes[typeof(AttributeWithParameters2)]);
        Assert.That(attributes.Count, Is.EqualTo(2));

        attributes = new List<AttributeDefinition>(definition.CustomAttributes[typeof(AttributeWithParameters3)]);
        Assert.That(attributes.Count, Is.EqualTo(1));
      }
    }

    [Test]
    public void CopyAttributes_Ambiguous ()
    {
      var builder = new AttributeDefinitionBuilder(DefinitionObjectMother.CreateMixinDefinition(typeof(MixinWithAmbiguousSource)));
      var method = typeof(MixinWithAmbiguousSource).GetMethod("ToString", BindingFlags.NonPublic | BindingFlags.Instance);
      var data = CustomAttributeData.GetCustomAttributes(method).Select(d => (ICustomAttributeData)new CustomAttributeDataAdapter(d));
      Assert.That(
          () => builder.Apply(method, data, true),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.Matches(
                  "The CopyCustomAttributes attribute on "
                  + ".*MixinWithAmbiguousSource.ToString specifies an ambiguous attribute "
                  + "source: The source member string Source matches several members on type "
                  + ".*MixinWithAmbiguousSource."));
    }

    [Test]
    public void CopyAttributes_Unknown ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(MixinWithUnknownSource)).EnterScope())
      {
        Assert.That(
            () => DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget)),
            Throws.InstanceOf<ConfigurationException>()
                .With.Message.Matches(
                    "The CopyCustomAttributes attribute on "
                    + ".*MixinWithUnknownSource.ToString specifies an unknown attribute "
                    + "source .*MixinWithUnknownSource.Source."));
      }
    }

    [Test]
    public void CopyAttributes_Invalid ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(MixinWithInvalidSourceType)).EnterScope())
      {
        Assert.That(
            () => DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget)),
            Throws.InstanceOf<ConfigurationException>()
                .With.Message.Matches(
                    "The CopyCustomAttributes attribute on "
                    + ".*MixinWithInvalidSourceType.ToString specifies an attribute source "
                    + ".*MixinWithInvalidSourceType of a different member kind."));
      }
    }

    private static void CheckAttributes (IAttributableDefinition attributableDefinition)
    {
      Assert.That(attributableDefinition.CustomAttributes.ContainsKey(typeof(TagAttribute)), Is.True);
      Assert.That(attributableDefinition.CustomAttributes.GetItemCount(typeof(TagAttribute)), Is.EqualTo(2));

      var attributes = new List<AttributeDefinition>(attributableDefinition.CustomAttributes);
      var attributes2 = new List<AttributeDefinition>(attributableDefinition.CustomAttributes[typeof(TagAttribute)]);
      foreach (AttributeDefinition attribute in attributes2)
      {
        Assert.That(attributes.Contains(attribute), Is.True);
      }

      AttributeDefinition attribute1 = attributes.Find(
          delegate (AttributeDefinition a)
          {
            Assert.That(a.AttributeType, Is.EqualTo(typeof(TagAttribute)));
            return a.Data.Constructor.Equals(typeof(TagAttribute).GetConstructor(Type.EmptyTypes));
          });
      Assert.That(attribute1, Is.Not.Null);
      Assert.That(attribute1.IsCopyTemplate, Is.False);
      Assert.That(attribute1.Data.ConstructorArguments.Count, Is.EqualTo(0));
      Assert.That(attribute1.Data.NamedArguments.Count, Is.EqualTo(0));
      Assert.That(attribute1.DeclaringDefinition, Is.SameAs(attributableDefinition));

      AttributeDefinition attribute2 = attributes.Find(
          delegate (AttributeDefinition a)
          {
            Assert.That(a.AttributeType, Is.EqualTo(typeof(TagAttribute)));
            return a.Data.Constructor.Equals(typeof(TagAttribute).GetConstructor(new[] { typeof(string) }));
          });
      Assert.That(attribute2, Is.Not.Null);
      Assert.That(attribute2.IsCopyTemplate, Is.False);
      Assert.That(attribute2.Data.ConstructorArguments.Count, Is.EqualTo(1));
      Assert.That(attribute2.Data.ConstructorArguments[0], Is.EqualTo("Class!"));

      var namedArgumentData = attribute2.Data.NamedArguments.Select(n => new { n.MemberInfo, n.Value }).ToArray();
      var expectedNamedArgumentData =
          new[]
            {
                new { MemberInfo = (MemberInfo)typeof(TagAttribute).GetField("Named"), Value = (object)5 }
            };
      Assert.That(namedArgumentData, Is.EquivalentTo(expectedNamedArgumentData));

      Assert.That(attribute2.DeclaringDefinition, Is.SameAs(attributableDefinition));
      Assert.That(attribute2.Parent, Is.SameAs(attributableDefinition));
    }

    // ReSharper disable UnusedMember.Local
    // ReSharper disable UnusedParameter.Local

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class TagAttribute : Attribute
    {
      public TagAttribute () { }
      public TagAttribute (string s) { }

      public int Named;
    }

    [Tag]
    [Tag("Class!", Named = 5)]
    private class ClassWithManyAttributes
    {
      [Tag]
      [Tag("Class!", Named = 5)]
      public void Foo ()
      {
      }
    }

    class InternalStuffAttribute : Attribute { }

    [InternalStuff]
    public class ClassWithInternalAttribute { }

    [IgnoreForMixinConfiguration]
    public class MixinWithAmbiguousSource
    {
      private void Source () { }
      private void Source (int i) { }

      [OverrideTarget]
      [CopyCustomAttributes(typeof(MixinWithAmbiguousSource), "Source")]
      protected new string ToString ()
      {
        return "";
      }
    }

    [IgnoreForMixinConfiguration]
    [CopyCustomAttributes(typeof(MixinWithSelfSource))]
    public class MixinWithSelfSource
    {
    }

    [IgnoreForMixinConfiguration]
    public class MixinWithInvalidSourceType
    {
      [OverrideTarget]
      [CopyCustomAttributes(typeof(MixinWithInvalidSourceType))]
      protected new string ToString ()
      {
        return "";
      }
    }

    [IgnoreForMixinConfiguration]
    public class MixinWithUnknownSource
    {
      [OverrideTarget]
      [CopyCustomAttributes(typeof(MixinWithUnknownSource), "Source")]
      protected new string ToString ()
      {
        return "";
      }
    }
    // ReSharper restore UnusedMember.Local
    // ReSharper restore UnusedParameter.Local
  }
}
