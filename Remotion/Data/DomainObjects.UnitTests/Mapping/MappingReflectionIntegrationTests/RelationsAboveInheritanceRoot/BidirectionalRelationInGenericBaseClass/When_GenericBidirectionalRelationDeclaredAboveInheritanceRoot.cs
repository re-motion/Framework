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

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests.RelationsAboveInheritanceRoot.BidirectionalRelationInGenericBaseClass
{
  // Filename has reached max-length
  [TestFixture]
  public class When_GenericBidirectionalRelationDeclaredAboveInheritanceRoot : MappingReflectionIntegrationTestBase
  {
    private ClassDefinition _inheritanceRootClass;
    private ClassDefinition _relationTarget;

    private PropertyInfoAdapter _propertyOnClassAboveInheritanceRoot;
    private PropertyInfoAdapter _propertyOnRelationTarget;

    public override void SetUp ()
    {
      base.SetUp();

      _inheritanceRootClass = GetClassDefinition(typeof(InheritanceRoot));
      _relationTarget = GetClassDefinition(typeof(RelationTarget));

      _propertyOnClassAboveInheritanceRoot = GetPropertyInformation(
          (GenericClassAboveInheritanceRoot<RelationTarget> c) => c.RelationPropertyOnClassAboveInheritanceRoot);
      _propertyOnRelationTarget = GetPropertyInformation((RelationTarget t) => t.RelationProperty);
    }

    [Test]
    public void GivenThat_TheRelationTarget_PointsBackToTheInheritanceRoot_WhichIsNotTheTypeDeclaringTheOppositeProperty ()
    {
      Assert.That(_propertyOnRelationTarget.PropertyType, Is.SameAs(_inheritanceRootClass.Type));
      Assert.That(_propertyOnClassAboveInheritanceRoot.DeclaringType, Is.Not.SameAs(_inheritanceRootClass.Type));
    }

    [Test]
    public void ThereShouldBeAValidRelation_BetweenTheInheritanceRoot_AndTheRelationTarget ()
    {
      var endPointOnInheritanceRoot = _inheritanceRootClass.ResolveRelationEndPoint(_propertyOnClassAboveInheritanceRoot);
      Assert.That(endPointOnInheritanceRoot, Is.Not.Null);

      var endPointOnRelationTarget = _relationTarget.ResolveRelationEndPoint(_propertyOnRelationTarget);
      Assert.That(endPointOnRelationTarget, Is.Not.Null);

      Assert.That(endPointOnInheritanceRoot.RelationDefinition, Is.SameAs(endPointOnRelationTarget.RelationDefinition));
    }
  }
}
