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
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests.Mixins.IntroducedRelations
{
  [TestFixture]
  public class When_ADerivedClass_ReceivesARelationViaMixin : MappingReflectionIntegrationTestBase
  {
    private ClassDefinition _derivedClass;
    private ClassDefinition _relationTarget;

    private PropertyInfoAdapter _relationEndPointPropertyInMixin;
    private PropertyInfoAdapter _relationEndPointPropertyInRelationTarget;

    public override void SetUp ()
    {
      base.SetUp();

      _derivedClass = GetClassDefinition(typeof(Derived));
      _relationTarget = GetClassDefinition(typeof(RelationTarget));

      _relationEndPointPropertyInMixin = GetPropertyInformation((MixinAddingRelation m) => m.RelationTarget);
      _relationEndPointPropertyInRelationTarget = GetPropertyInformation((RelationTarget t) => t.Derived);
    }

    [Test]
    public void ThereShouldBeARelationDefinition_BetweenTheDerivedClass_And_TheRelationTarget ()
    {
      var relationEndPointInDerivedClass = _derivedClass.ResolveRelationEndPoint(_relationEndPointPropertyInMixin);
      Assert.That(relationEndPointInDerivedClass, Is.Not.Null);
      var relationEndPointInRelationTargetClass = _relationTarget.ResolveRelationEndPoint(_relationEndPointPropertyInRelationTarget);
      Assert.That(relationEndPointInRelationTargetClass, Is.Not.Null);

      Assert.That(relationEndPointInDerivedClass.RelationDefinition, Is.SameAs(relationEndPointInRelationTargetClass.RelationDefinition));
      Assert.That(
          relationEndPointInDerivedClass.RelationDefinition.EndPointDefinitions,
          Is.EquivalentTo(new[] { relationEndPointInRelationTargetClass, relationEndPointInDerivedClass }));
    }
  }
}
