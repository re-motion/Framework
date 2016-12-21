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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class PersistentMixinIntegrationTest : ClientTransactionBaseTest
  {
    [Test]
    public void ClassDefinitionIncludesPersistentProperties ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (TargetClassForPersistentMixin));
      Assert.That (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty"), Is.Not.Null);
      Assert.That (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".ExtraPersistentProperty"), Is.Not.Null);
    }

    [Test]
    public void ClassDefinitionIncludesPersistentPropertiesFromDerivedMixin ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (TargetClassForDerivedPersistentMixin));
      Assert.That (classDefinition.GetPropertyDefinition (typeof (DerivedMixinAddingSimplePersistentProperties).FullName + ".AdditionalPersistentProperty"), Is.Not.Null);
      Assert.That (classDefinition.GetPropertyDefinition (typeof (MixinAddingSimplePersistentProperties).FullName + ".PersistentProperty"), Is.Not.Null);
    }

    [Test]
    public void ClassDefinitionExcludesNonPersistentProperties ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (TargetClassForPersistentMixin));
      Assert.That (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".NonPersistentProperty"), Is.Null);
    }

    [Test]
    public void ClassDefinition_RealSide_Unmixed ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Computer));
      var relationProperty = classDefinition.GetPropertyDefinition (typeof (Computer).FullName + ".Employee");
      var relation = classDefinition.GetRelationEndPointDefinition (typeof (Computer).FullName + ".Employee").RelationDefinition;
      Assert.That (relationProperty, Is.Not.Null);
      Assert.That (relation, Is.Not.Null);
      Assert.That (relation.ID, Is.EqualTo (typeof (Computer) + ":" + typeof (Computer).FullName + ".Employee->" + typeof (Employee) + ".Computer"));
    }

    [Test]
    public void ClassDefinition_VirtualSide_Unmixed ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Employee));
      var relationProperty = classDefinition.GetPropertyDefinition (typeof (Employee).FullName + ".Computer");
      var relation = classDefinition.GetRelationEndPointDefinition (typeof (Employee).FullName + ".Computer").RelationDefinition;
      Assert.That (relationProperty, Is.Null);
      Assert.That (relation, Is.Not.Null);
      Assert.That (relation.ID, Is.EqualTo (typeof (Computer) + ":" + typeof (Computer).FullName + ".Employee->" + typeof (Employee).FullName + ".Computer"));
    }

    [Test]
    public void ClassDefinition_RealSide_MixedReal ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (TargetClassForPersistentMixin));
      var relationProperty = classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".RelationProperty");
      var relation = classDefinition.GetRelationEndPointDefinition (typeof (MixinAddingPersistentProperties).FullName + ".RelationProperty").RelationDefinition;
      Assert.That (relationProperty, Is.Not.Null);
      Assert.That (relation, Is.Not.Null);
      Assert.That (relation.ID, Is.EqualTo (string.Format (
          "{0}:{1}.RelationProperty->{2}.RelationProperty1",
          typeof (TargetClassForPersistentMixin).FullName,
          typeof (MixinAddingPersistentProperties).FullName, 
          typeof (RelationTargetForPersistentMixin).FullName)));
    }

    [Test]
    public void ClassDefinition_VirtualSide_MixedReal ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (RelationTargetForPersistentMixin));
      var relationProperty = classDefinition.GetPropertyDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty1");
      var relation = classDefinition.GetRelationEndPointDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty1").RelationDefinition;
      Assert.That (relationProperty, Is.Null);
      Assert.That (relation, Is.Not.Null);
      Assert.That (relation.ID, Is.EqualTo (string.Format (
          "{0}:{1}.RelationProperty->{2}.RelationProperty1",
          typeof (TargetClassForPersistentMixin).FullName,
          typeof (MixinAddingPersistentProperties).FullName,
          typeof (RelationTargetForPersistentMixin).FullName)));
    }

    [Test]
    public void ClassDefinition_RealSide_MixedVirtual ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (RelationTargetForPersistentMixin));
      var relationProperty = classDefinition.GetPropertyDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty2");
      var relation = classDefinition.GetRelationEndPointDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty2").RelationDefinition;
      Assert.That (relationProperty, Is.Not.Null);
      Assert.That (relation, Is.Not.Null);
      Assert.That (relation.ID, Is.EqualTo (typeof (RelationTargetForPersistentMixin).FullName + ":" + typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty2->"
                                            + typeof (MixinAddingPersistentProperties).FullName + ".VirtualRelationProperty"));
    }

    [Test]
    public void ClassDefinition_VirtualSide_MixedVirtual ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (TargetClassForPersistentMixin));
      var relationProperty = classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".VirtualRelationProperty");
      var relation = classDefinition.GetRelationEndPointDefinition (typeof (MixinAddingPersistentProperties).FullName + ".VirtualRelationProperty").RelationDefinition;
      Assert.That (relationProperty, Is.Null);
      Assert.That (relation, Is.Not.Null);
      Assert.That (relation.ID, Is.EqualTo (typeof (RelationTargetForPersistentMixin) + ":" + typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty2->"
                                            + typeof (MixinAddingPersistentProperties).FullName + ".VirtualRelationProperty"));
    }

    [Test]
    public void ClassDefinition_Unidirectional_OneClassTwoMixins ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (TargetClassWithTwoUnidirectionalMixins));
      var relationProperty1 = classDefinition.GetPropertyDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      var relationProperty2 = classDefinition.GetPropertyDefinition (typeof (MixinAddingUnidirectionalRelation2).FullName + ".Computer");
      var relation1 = classDefinition.GetRelationEndPointDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer").RelationDefinition;
      var relation2 = classDefinition.GetRelationEndPointDefinition (typeof (MixinAddingUnidirectionalRelation2).FullName + ".Computer").RelationDefinition;
      Assert.That (relationProperty1, Is.Not.Null);
      Assert.That (relationProperty2, Is.Not.Null);
      Assert.That (relation1, Is.Not.Null);
      Assert.That (relation2, Is.Not.Null);
      Assert.That (relation2, Is.Not.SameAs (relation1));
      Assert.That (relation1.ID, Is.EqualTo (string.Format (
          "{0}:{1}.Computer",
          typeof (TargetClassWithTwoUnidirectionalMixins).FullName,
          typeof (MixinAddingUnidirectionalRelation1).FullName)));
      Assert.That (relation2.ID, Is.EqualTo (string.Format (
          "{0}:{1}.Computer",
          typeof (TargetClassWithTwoUnidirectionalMixins).FullName,
          typeof (MixinAddingUnidirectionalRelation2).FullName)));
    }

    [Test]
    public void ClassDefinition_Unidirectional_TwoClassesOneMixin ()
    {
      var classDefinition1 = MappingConfiguration.Current.GetTypeDefinition (typeof (TargetClassWithUnidirectionalMixin1));
      var classDefinition2 = MappingConfiguration.Current.GetTypeDefinition (typeof (TargetClassWithUnidirectionalMixin2));
      var relationProperty1 = classDefinition1.GetPropertyDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      var relationProperty2 = classDefinition2.GetPropertyDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer");
      var relation1 = classDefinition1.GetRelationEndPointDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer").RelationDefinition;
      var relation2 = classDefinition2.GetRelationEndPointDefinition (typeof (MixinAddingUnidirectionalRelation1).FullName + ".Computer").RelationDefinition;
      Assert.That (relationProperty1, Is.Not.Null);
      Assert.That (relationProperty2, Is.Not.Null);
      Assert.That (relation1, Is.Not.Null);
      Assert.That (relation2, Is.Not.Null);
      Assert.That (relation2, Is.Not.SameAs (relation1));
      Assert.That (relation1.ID, Is.EqualTo (string.Format (
          "{0}:{1}.Computer",
          typeof (TargetClassWithUnidirectionalMixin1).FullName,
          typeof (MixinAddingUnidirectionalRelation1).FullName)));
      Assert.That (relation2.ID, Is.EqualTo (string.Format (
          "{0}:{1}.Computer",
          typeof (TargetClassWithUnidirectionalMixin2).FullName,
          typeof (MixinAddingUnidirectionalRelation1).FullName)));
    }

    [Test]
    public void RelationTargetClassDefinitionIncludesRelationProperty ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (RelationTargetForPersistentMixin));
      Assert.That (classDefinition.GetPropertyDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty"), Is.Null);
      Assert.That (classDefinition.GetRelationEndPointDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty1").RelationDefinition, Is.Not.Null);
    }
    
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type "
        +
        "'Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain.StubStorageTargetClassForPersistentMixin' was changed after the mapping "
        + "information was built.", MatchType = MessageMatch.Contains)]
    public void DynamicChangeInPersistentMixinConfigurationThrowsInNewObject ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        StubStorageTargetClassForPersistentMixin.NewObject();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type "
        +
        "'Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain.StubStorageTargetClassForPersistentMixin' was changed after the mapping "
        + "information was built.", MatchType = MessageMatch.Contains)]
    public void DynamicChangeInPersistentMixinConfigurationThrowsInGetObject ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        new ObjectID(typeof (StubStorageTargetClassForPersistentMixin), 13).GetObject<StubStorageTargetClassForPersistentMixin> ();
      }
    }

    [Test]
    public void DynamicChangeInNonPersistentMixinConfigurationDoesntMatter ()
    {
      StubStorageTargetClassForPersistentMixin.NewObject();
      new ObjectID(typeof (StubStorageTargetClassForPersistentMixin), 12).GetObject<StubStorageTargetClassForPersistentMixin> ();
    }

    [Test]
    public void UnidirectionalRelationProperty ()
    {
      var tc = TargetClassForPersistentMixin.NewObject();
      var relationTarget = RelationTargetForPersistentMixin.NewObject();
      var mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.UnidirectionalRelationProperty = relationTarget;
      Assert.That (mixin.UnidirectionalRelationProperty, Is.SameAs (relationTarget));
    }

    [Test]
    public void RelationPropertyRealSide ()
    {
      var tc = TargetClassForPersistentMixin.NewObject();
      var relationTarget = RelationTargetForPersistentMixin.NewObject();
      var mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.RelationProperty = relationTarget;
      Assert.That (mixin.RelationProperty, Is.SameAs (relationTarget));
      Assert.That (relationTarget.RelationProperty1, Is.SameAs (tc));
    }

    [Test]
    public void VirtualRelationProperty ()
    {
      var tc = TargetClassForPersistentMixin.NewObject();
      var relationTarget = RelationTargetForPersistentMixin.NewObject();
      var mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.VirtualRelationProperty = relationTarget;
      Assert.That (mixin.VirtualRelationProperty, Is.SameAs (relationTarget));
      Assert.That (relationTarget.RelationProperty2, Is.SameAs (tc));
    }

    [Test]
    public void CollectionProperty1Side ()
    {
      var tc = TargetClassForPersistentMixin.NewObject();
      var relationTarget1 = RelationTargetForPersistentMixin.NewObject();
      var relationTarget2 = RelationTargetForPersistentMixin.NewObject();
      var mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.CollectionProperty1Side.Add (relationTarget1);
      mixin.CollectionProperty1Side.Add (relationTarget2);
      Assert.That (mixin.CollectionProperty1Side[0], Is.SameAs (relationTarget1));
      Assert.That (mixin.CollectionProperty1Side[1], Is.SameAs (relationTarget2));
      Assert.That (relationTarget1.RelationProperty3, Is.SameAs (tc));
      Assert.That (relationTarget2.RelationProperty3, Is.SameAs (tc));
    }

    [Test]
    public void CollectionPropertyNSide ()
    {
      var tc1 = TargetClassForPersistentMixin.NewObject();
      var tc2 = TargetClassForPersistentMixin.NewObject();
      var relationTarget = RelationTargetForPersistentMixin.NewObject();
      var mixin1 = Mixin.Get<MixinAddingPersistentProperties> (tc1);
      var mixin2 = Mixin.Get<MixinAddingPersistentProperties> (tc2);
      mixin1.CollectionPropertyNSide = relationTarget;
      mixin2.CollectionPropertyNSide = relationTarget;
      Assert.That (mixin1.CollectionPropertyNSide, Is.SameAs (relationTarget));
      Assert.That (mixin2.CollectionPropertyNSide, Is.SameAs (relationTarget));
      Assert.That (relationTarget.RelationProperty4[0], Is.SameAs (tc1));
      Assert.That (relationTarget.RelationProperty4[1], Is.SameAs (tc2));
    }

    [Test]
    public void DerivedMixin ()
    {
      var tc = TargetClassForDerivedPersistentMixin.NewObject();
      var mixin = Mixin.Get<DerivedMixinAddingSimplePersistentProperties> (tc);
      mixin.AdditionalPersistentProperty = 12;
      Assert.That (mixin.AdditionalPersistentProperty, Is.EqualTo (12));
      mixin.PersistentProperty = 10;
      Assert.That (mixin.PersistentProperty, Is.EqualTo (10));
    }

    [Test]
    public void TargetClassAboveInheritanceRoot ()
    {
      var tc = InheritanceRootInheritingPersistentMixin.NewObject();
      var mixin = Mixin.Get<MixinAddingPersistentPropertiesAboveInheritanceRoot> (tc);
      mixin.PersistentProperty = 10;
      Assert.That (mixin.PersistentProperty, Is.EqualTo (10));
    }

    [Test]
    public void RelationOnTargetClassAboveInheritanceRoot ()
    {
      var tc = InheritanceRootInheritingPersistentMixin.NewObject();
      var mixin = Mixin.Get<MixinAddingPersistentPropertiesAboveInheritanceRoot> (tc);
      var relationTarget = RelationTargetForPersistentMixinAboveInheritanceRoot.NewObject();
      mixin.PersistentRelationProperty = relationTarget;
      Assert.That (mixin.PersistentRelationProperty, Is.SameAs (relationTarget));
    }

    [Test]
    public void BaseClassReceivingReferenceToDerivedClass ()
    {
      var tc = TargetClassReceivingReferenceToDerivedClass.NewObject();
      var mixin = Mixin.Get<MixinAddingReferenceToDerivedClass> (tc);
      var relationTarget = DerivedClassWithBaseReferenceViaMixin.NewObject();
      mixin.MyDerived.Add (relationTarget);
      Assert.That (relationTarget.MyBase, Is.SameAs (tc));
      relationTarget.MyBase = null;
      Assert.That (mixin.MyDerived, Is.Empty);
    }

    [Test]
    public void BaseClassReceivingTwoReferencesToDerivedClass ()
    {
      var tc = TargetClassReceivingTwoReferencesToDerivedClass.NewObject();
      var mixin1 = Mixin.Get<MixinAddingTwoReferencesToDerivedClass1> (tc);
      var mixin2 = Mixin.Get<MixinAddingTwoReferencesToDerivedClass2> (tc);
      var relationTarget = DerivedClassWithTwoBaseReferencesViaMixins.NewObject();
      mixin1.MyDerived1.Add (relationTarget);
      mixin2.MyDerived2.Add (relationTarget);
      Assert.That (relationTarget.MyBase1, Is.SameAs (tc));
      Assert.That (relationTarget.MyBase2, Is.SameAs (tc));
      relationTarget.MyBase1 = null;
      relationTarget.MyBase2 = null;
      Assert.That (mixin1.MyDerived1, Is.Empty);
      Assert.That (mixin2.MyDerived2, Is.Empty);
    }
  }
}