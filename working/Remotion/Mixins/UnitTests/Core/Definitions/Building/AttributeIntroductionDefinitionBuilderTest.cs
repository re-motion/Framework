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
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
{
  [TestFixture]
  public class AttributeIntroductionDefinitionBuilderTest
  {
    [Test]
    public void MixinsIntroduceAttributes ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      Assert.That (bt1.CustomAttributes.Count, Is.EqualTo (2));
      Assert.That (bt1.CustomAttributes.ContainsKey (typeof (BT1Attribute)), Is.True);
      Assert.That (bt1.CustomAttributes.ContainsKey (typeof (DefaultMemberAttribute)), Is.True);

      MixinDefinition mixin1 = bt1.Mixins[typeof (BT1Mixin1)];
      Assert.That (mixin1.CustomAttributes.Count, Is.EqualTo (1));
      Assert.That (mixin1.CustomAttributes.ContainsKey (typeof (BT1M1Attribute)), Is.True);
      Assert.That (mixin1.AttributeIntroductions.Count, Is.EqualTo (1));
      Assert.That (mixin1.AttributeIntroductions.ContainsKey (typeof (BT1M1Attribute)), Is.True);

      MixinDefinition mixin2 = bt1.Mixins[typeof (BT1Mixin2)];
      Assert.That (mixin2.CustomAttributes.Count, Is.EqualTo (0));
      Assert.That (mixin2.AttributeIntroductions.Count, Is.EqualTo (0));

      Assert.That (bt1.ReceivedAttributes.Count, Is.EqualTo (1));
      Assert.That (bt1.ReceivedAttributes[0].Attribute, Is.SameAs (mixin1.CustomAttributes[0]));
      Assert.That (bt1.ReceivedAttributes[0], Is.SameAs (mixin1.AttributeIntroductions[0]));
      Assert.That (bt1.ReceivedAttributes[0].Parent, Is.SameAs (mixin1));
      Assert.That (bt1.ReceivedAttributes[0].FullName, Is.EqualTo (mixin1.CustomAttributes[0].FullName));
      Assert.That (bt1.ReceivedAttributes[0].AttributeType, Is.EqualTo (mixin1.CustomAttributes[0].AttributeType));
    }

    [Test]
    public void MixinsIntroduceAttributes_InheritedFromBase ()
    {
      TargetClassDefinition target = DefinitionObjectMother.GetTargetClassDefinition (typeof (NullTarget), typeof (MixinInheritingAttributes));

      var mixin = target.Mixins[typeof (MixinInheritingAttributes)];
      Assert.That (mixin.CustomAttributes.Keys, Has.Member (typeof (InheritableAttribute)));
      Assert.That (mixin.CustomAttributes.Keys, Has.No.Member (typeof (NonInheritableAttribute)));

      var attribute = mixin.CustomAttributes[typeof (InheritableAttribute)].Single();
      Assert.That (mixin.AttributeIntroductions.Select (i => i.Attribute), Has.Member (attribute));
      var attributeIntroduction = mixin.AttributeIntroductions[typeof (InheritableAttribute)].Single();
      Assert.That (target.ReceivedAttributes, Has.Member (attributeIntroduction));

      var member = mixin.Methods[typeof (MixinInheritingAttributes).GetMethod ("ToString")];
      Assert.That (member.CustomAttributes.Keys, Has.Member (typeof (InheritableAttribute)));
      Assert.That (member.CustomAttributes.Keys, Has.No.Member (typeof (NonInheritableAttribute)));

      var memberAttribute = member.CustomAttributes[typeof (InheritableAttribute)].Single ();
      Assert.That (member.AttributeIntroductions.Select (i => i.Attribute), Has.Member (memberAttribute));
      var memberAttributeIntroduction = member.AttributeIntroductions[typeof (InheritableAttribute)].Single ();
      Assert.That (member.Base.ReceivedAttributes, Has.Member (memberAttributeIntroduction));
    }

    [Test]
    public void MixinsIntroduceAttributesToMembers ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      MethodDefinition member = bt1.Methods[typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes)];
      Assert.That (member.CustomAttributes.Count, Is.EqualTo (1));
      Assert.That (member.ReceivedAttributes.Count, Is.EqualTo (1));

      MixinDefinition mixin = bt1.Mixins[typeof (BT1Mixin1)];
      MethodDefinition mixinMember = mixin.Methods[typeof (BT1Mixin1).GetMethod ("VirtualMethod")];

      Assert.That (member.ReceivedAttributes.ContainsKey (typeof (BT1M1Attribute)), Is.True);
      Assert.That (mixinMember.CustomAttributes.Count, Is.EqualTo (1));
      Assert.That (member.ReceivedAttributes[0].Attribute, Is.SameAs (mixinMember.CustomAttributes[0]));
      Assert.That (member.ReceivedAttributes[0].Parent, Is.SameAs (mixinMember));
      Assert.That (member.ReceivedAttributes[0].FullName, Is.EqualTo (mixinMember.CustomAttributes[0].FullName));
      Assert.That (member.ReceivedAttributes[0].AttributeType, Is.EqualTo (mixinMember.CustomAttributes[0].AttributeType));
    }

    [Test]
    public void MultipleAttributes ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (
          typeof (BaseTypeWithAllowMultiple), typeof (MixinAddingAllowMultipleToClassAndMember), typeof (MixinAddingAllowMultipleToClassAndMember2));
      Assert.That (definition.ReceivedAttributes.GetItemCount (typeof (MultiAttribute)), Is.EqualTo (3));
      Assert.That (definition.CustomAttributes.GetItemCount (typeof (MultiAttribute)), Is.EqualTo (1));
    }

    [Test]
    public void MultipleAttributesOnMembers ()
    {
      TargetClassDefinition bt1 =
          DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseTypeWithAllowMultiple),
                                                             typeof (MixinAddingAllowMultipleToClassAndMember), typeof (MixinAddingAllowMultipleToClassAndMember2));
      MethodDefinition member = bt1.Methods[typeof (BaseTypeWithAllowMultiple).GetMethod ("Foo")];

      Assert.That (member.CustomAttributes.Count, Is.EqualTo (1));
      Assert.That (member.Overrides.Count, Is.EqualTo (2));
      Assert.That (member.ReceivedAttributes.Count, Is.EqualTo (2));

      MixinDefinition mixin1 = bt1.Mixins[typeof (MixinAddingAllowMultipleToClassAndMember)];
      MethodDefinition mixinMember1 = mixin1.Methods[typeof (MixinAddingAllowMultipleToClassAndMember).GetMethod ("Foo")];
      Assert.That (mixinMember1.CustomAttributes.Count, Is.EqualTo (1));

      MixinDefinition mixin2 = bt1.Mixins[typeof (MixinAddingAllowMultipleToClassAndMember2)];
      MethodDefinition mixinMember2 = mixin2.Methods[typeof (MixinAddingAllowMultipleToClassAndMember).GetMethod ("Foo")];
      Assert.That (mixinMember2.CustomAttributes.Count, Is.EqualTo (1));

      Assert.That (member.ReceivedAttributes.ContainsKey (typeof (MultiAttribute)), Is.True);
      Assert.That (member.ReceivedAttributes.GetItemCount (typeof (MultiAttribute)), Is.EqualTo (2));

      List<AttributeDefinition> attributes =
          new List<AttributeIntroductionDefinition> (member.ReceivedAttributes[typeof (MultiAttribute)])
              .ConvertAll<AttributeDefinition> (delegate (AttributeIntroductionDefinition intro) { return intro.Attribute; });
      Assert.That (attributes, Is.EquivalentTo (new AttributeDefinition[] { mixinMember1.CustomAttributes[0], mixinMember2.CustomAttributes[0] }));
    }

    [Test]
    public void NonInheritedAttributesAreNotIntroduced ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (
          typeof (BaseType1), typeof (MixinAddingNonInheritedAttribute));
      Assert.That (definition.ReceivedAttributes.GetItemCount (typeof (NonInheritableAttribute)), Is.EqualTo (0));
    }

    [Test]
    public void WithNonMultipleInheritedAttributesTheTargetClassWins ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (
          typeof (BaseType1), typeof (MixinAddingBT1Attribute));
      Assert.That (definition.ReceivedAttributes.GetItemCount (typeof (BT1Attribute)), Is.EqualTo (0));
      Assert.That (definition.CustomAttributes.GetItemCount (typeof (BT1Attribute)), Is.EqualTo (1));
    }

    [Test]
    public void WithNonMultipleInheritedAttributesOnMemberTheTargetClassWins ()
    {
      MethodDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (
          typeof (BaseType1), typeof (MixinAddingBT1AttributeToMember)).Methods[typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes)];
      Assert.That (definition.ReceivedAttributes.GetItemCount (typeof (BT1Attribute)), Is.EqualTo (0));
      Assert.That (definition.CustomAttributes.GetItemCount (typeof (BT1Attribute)), Is.EqualTo (1));
    }

    [Test]
    public void ImplicitlyNonIntroducedAttribute ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<BaseType1> ().AddMixins (typeof (MixinAddingBT1Attribute)).EnterScope ())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (BT1Attribute)), Is.False);

        NonAttributeIntroductionDefinition nonIntroductionDefinition =
            definition.Mixins[typeof (MixinAddingBT1Attribute)].NonAttributeIntroductions.GetFirstItem (typeof (BT1Attribute));
        Assert.That (nonIntroductionDefinition.IsExplicitlySuppressed, Is.False);
        Assert.That (nonIntroductionDefinition.IsShadowed, Is.True);
        Assert.That (nonIntroductionDefinition.Attribute, Is.SameAs (definition.Mixins[typeof (MixinAddingBT1Attribute)].CustomAttributes.GetFirstItem (typeof (BT1Attribute))));
        Assert.That (nonIntroductionDefinition.Parent, Is.SameAs (definition.Mixins[typeof (MixinAddingBT1Attribute)]));
      }
    }

    [Test]
    public void ExplicitlyNonIntroducedAttribute ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<NullTarget> ().AddMixins (typeof (MixinNonIntroducingSimpleAttribute)).EnterScope ())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (SimpleAttribute)), Is.False);

        NonAttributeIntroductionDefinition nonIntroductionDefinition =
            definition.Mixins[typeof (MixinNonIntroducingSimpleAttribute)].NonAttributeIntroductions.GetFirstItem (typeof (SimpleAttribute));
        Assert.That (nonIntroductionDefinition.IsExplicitlySuppressed, Is.True);
        Assert.That (nonIntroductionDefinition.IsShadowed, Is.False);
        Assert.That (nonIntroductionDefinition.Attribute, Is.SameAs (definition.Mixins[typeof (MixinNonIntroducingSimpleAttribute)].CustomAttributes.GetFirstItem (typeof (SimpleAttribute))));
        Assert.That (nonIntroductionDefinition.Parent, Is.SameAs (definition.Mixins[typeof (MixinNonIntroducingSimpleAttribute)]));
      }
    }

    [Test]
    public void IndirectAttributeIntroduction_ViaCopy ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<NullTarget> ().Clear ().AddMixins (typeof (MixinIndirectlyAddingAttribute)).EnterScope ())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)), Is.False);
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (AttributeWithParameters)), Is.True);

        List<AttributeIntroductionDefinition> introductions =
            new List<AttributeIntroductionDefinition> (definition.ReceivedAttributes[typeof (AttributeWithParameters)]);
        List<AttributeDefinition> attributes =
            new List<AttributeDefinition> (definition.Mixins[typeof (MixinIndirectlyAddingAttribute)].CustomAttributes[typeof (AttributeWithParameters)]);

        Assert.That (introductions.Count, Is.EqualTo (1));
        Assert.That (attributes.Count, Is.EqualTo (1));

        Assert.That (introductions[0].Attribute, Is.SameAs (attributes[0]));
      }
    }

    [Test]
    public void IndirectAttributeIntroduction_ViaCopy_OfAttributesInheritedFromCopyBase ()
    {
      var mixinType = typeof (MixinIndirectlyAddingAttributeInheritedFromAttributeSourceBase);
      var target = DefinitionObjectMother.GetTargetClassDefinition (typeof (NullTarget), mixinType);
      
      var mixin = target.Mixins[mixinType];
      Assert.That (mixin.CustomAttributes.Keys, Has.Member (typeof (InheritableAttribute)));
      Assert.That (mixin.CustomAttributes.Keys, Has.No.Member (typeof (NonInheritableAttribute)));

      var mixinAttribute = mixin.CustomAttributes[typeof (InheritableAttribute)].Single();
      Assert.That (target.ReceivedAttributes.Keys, Has.Member (typeof (InheritableAttribute)));
      Assert.That (target.ReceivedAttributes.Keys, Has.No.Member (typeof (NonInheritableAttribute)));
      Assert.That (target.ReceivedAttributes[typeof (InheritableAttribute)].Single ().Attribute, Is.SameAs (mixinAttribute));

      var member = mixin.Methods[mixinType.GetMethod ("ToString")];
      Assert.That (member.CustomAttributes.Keys, Has.Member (typeof (InheritableAttribute)));
      Assert.That (member.CustomAttributes.Keys, Has.No.Member (typeof (NonInheritableAttribute)));

      var memberAttribute = member.CustomAttributes[typeof (InheritableAttribute)].Single();
      Assert.That (member.Base.ReceivedAttributes.Keys, Has.Member (typeof (InheritableAttribute)));
      Assert.That (member.Base.ReceivedAttributes.Keys, Has.No.Member (typeof (NonInheritableAttribute)));
      Assert.That (member.Base.ReceivedAttributes[typeof (InheritableAttribute)].Single ().Attribute, Is.SameAs (memberAttribute));
    }

    [Test]
    public void IndirectAttributeIntroduction_OfNonInheritedAttribute_ViaCopy ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<NullTarget> ().Clear ().AddMixins (typeof (MixinIndirectlyAddingNonInheritedAttribute)).EnterScope ())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)), Is.False);
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (NonInheritableAttribute)), Is.True);
      }
    }

    [Test]
    public void IndirectAttributeIntroduction_ViaCopy_OnMember ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<NullTarget> ().Clear ().AddMixins (typeof (MixinIndirectlyAddingAttribute)).EnterScope ())
      {
        MethodDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget)).Methods[typeof (object).GetMethod ("ToString")];
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (CopyCustomAttributesAttribute)), Is.False);
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (AttributeWithParameters)), Is.True);

        List<AttributeIntroductionDefinition> introductions =
            new List<AttributeIntroductionDefinition> (definition.ReceivedAttributes[typeof (AttributeWithParameters)]);
        List<AttributeDefinition> attributes = new List<AttributeDefinition> (
            definition.Overrides[typeof (MixinIndirectlyAddingAttribute)].DeclaringClass.Methods[typeof (MixinIndirectlyAddingAttribute).GetMethod ("ToString")].CustomAttributes[typeof (AttributeWithParameters)]);

        Assert.That (introductions.Count, Is.EqualTo (1));
        Assert.That (attributes.Count, Is.EqualTo (1));

        Assert.That (introductions[0].Attribute, Is.SameAs (attributes[0]));
      }
    }

    [Test]
    public void IntroducedAttribute_SuppressedByTargetClass ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<TargetClassSuppressingBT1Attribute> ().AddMixin<MixinAddingBT1Attribute> ().EnterScope ())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (TargetClassSuppressingBT1Attribute));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (BT1Attribute)), Is.False);
        
        Assert.That (definition.Mixins[typeof(MixinAddingBT1Attribute)].SuppressedAttributeIntroductions.ContainsKey (typeof (BT1Attribute)), Is.True);

        SuppressedAttributeIntroductionDefinition[] suppressedAttributes =
            definition.Mixins[typeof (MixinAddingBT1Attribute)].SuppressedAttributeIntroductions[typeof (BT1Attribute)].ToArray ();
        Assert.That (suppressedAttributes.Length, Is.EqualTo (1));
        Assert.That (suppressedAttributes[0].Attribute,
                     Is.SameAs (definition.Mixins[typeof (MixinAddingBT1Attribute)].CustomAttributes.GetFirstItem (typeof (BT1Attribute))));
        Assert.That (suppressedAttributes[0].AttributeType, Is.EqualTo (typeof (BT1Attribute)));
        Assert.That (suppressedAttributes[0].FullName, Is.EqualTo (typeof (BT1Attribute).FullName));
        Assert.That (suppressedAttributes[0].Parent, Is.SameAs (definition));
        Assert.That (suppressedAttributes[0].Suppressor, Is.SameAs (definition.CustomAttributes.GetFirstItem (typeof (SuppressAttributesAttribute))));
        Assert.That (suppressedAttributes[0].Target, Is.SameAs (definition));
      }
    }

    [Test]
    public void IntroducedAttribute_NotSuppressedDueToType ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<TargetClassSuppressingBT1Attribute> ().AddMixin<MixinAddingSimpleAttribute> ().EnterScope ())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (TargetClassSuppressingBT1Attribute));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (SimpleAttribute)), Is.True);
      }
    }

    [Test]
    public void IntroducedAttribute_SuppressedByMixin ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<NullTarget> ().AddMixin<MixinAddingBT1Attribute> ().AddMixin<MixinSuppressingBT1Attribute> ().EnterScope ())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (BT1Attribute)), Is.False);
        Assert.That (definition.Mixins[typeof (MixinAddingBT1Attribute)].SuppressedAttributeIntroductions.ContainsKey (typeof (BT1Attribute)), Is.True);

        SuppressedAttributeIntroductionDefinition[] suppressedAttributes =
            definition.Mixins[typeof (MixinAddingBT1Attribute)].SuppressedAttributeIntroductions[typeof (BT1Attribute)].ToArray ();
        Assert.That (suppressedAttributes.Length, Is.EqualTo (1));
        Assert.That (suppressedAttributes[0].Attribute,
                     Is.SameAs (definition.Mixins[typeof (MixinAddingBT1Attribute)].CustomAttributes.GetFirstItem (typeof (BT1Attribute))));
        Assert.That (suppressedAttributes[0].AttributeType, Is.EqualTo (typeof (BT1Attribute)));
        Assert.That (suppressedAttributes[0].FullName, Is.EqualTo (typeof (BT1Attribute).FullName));
        Assert.That (suppressedAttributes[0].Parent, Is.SameAs (definition));
        Assert.That (suppressedAttributes[0].Suppressor,
                     Is.SameAs (definition.Mixins[typeof (MixinSuppressingBT1Attribute)].CustomAttributes.GetFirstItem (typeof (SuppressAttributesAttribute))));
        Assert.That (suppressedAttributes[0].Target, Is.SameAs (definition));
      }
    }
    
    [Test]
    public void IntroducedAttribute_NoSelfSuppress ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<NullTarget> ().AddMixin<MixinSuppressingAndAddingBT1Attribute> ().EnterScope ())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        Assert.That (definition.ReceivedAttributes.ContainsKey (typeof (BT1Attribute)), Is.True);
      }
    }

    [NonInheritable]
    [Inheritable]
    public class BaseClassWithAttributes
    {
      [NonInheritable]
      [Inheritable]
      public virtual new string ToString ()
      {
        return null;
      }
    }

    public class MixinInheritingAttributes : BaseClassWithAttributes
    {
      [OverrideTarget]
      public override string ToString ()
      {
        return base.ToString ();
      }
    }
  }

  [CopyCustomAttributes (typeof (AttributeSourceWithInheritance))]
  public class MixinIndirectlyAddingAttributeInheritedFromAttributeSourceBase
  {
    public class AttributeSourceWithInheritance : AttributeSourceBase
    {
      public override void AttributeSourceMethod ()
      {
      }
    }

    [Inheritable]
    [NonInheritable]
    public class AttributeSourceBase
    {
      [Inheritable]
      [NonInheritable]
      public virtual void AttributeSourceMethod ()
      {
      }
    }

    [OverrideTarget]
    [CopyCustomAttributes (typeof (AttributeSourceWithInheritance), "AttributeSourceMethod")]
    public new string ToString ()
    {
      return "";
    }
  }
}
