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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class BaseOneSideRelationProperty : MappingReflectionTestBase
  {
    private ClassDefinition _classDefinition;
    private Type _classType;

    public override void SetUp ()
    {
      base.SetUp();

      _classType = typeof (ClassWithVirtualRelationEndPoints);
      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: _classType);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne ()
    {
      DomainModelConstraintProviderStub
        .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToOne")))
        .Return (true);

      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("BaseBidirectionalOneToOne"));
      var relationEndPointReflector = CreateRelationEndPointReflector (propertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.That (relationEndPointDefinition.ClassDefinition, Is.SameAs (_classDefinition));
      Assert.That (relationEndPointDefinition.PropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BaseBidirectionalOneToOne"));
      Assert.That (relationEndPointDefinition.PropertyInfo.PropertyType, Is.SameAs (typeof (ClassWithRealRelationEndPoints)));
      Assert.That (relationEndPointDefinition.Cardinality, Is.EqualTo (CardinalityType.One));
      Assert.That (relationEndPointDefinition.RelationDefinition, Is.Null);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      DomainModelConstraintProviderStub
        .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToMany")))
        .Return (true);

      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("BaseBidirectionalOneToMany"));
      var relationEndPointReflector = CreateRelationEndPointReflector (propertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.That (relationEndPointDefinition.ClassDefinition, Is.SameAs (_classDefinition));
      Assert.That (relationEndPointDefinition.PropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BaseBidirectionalOneToMany"));
      Assert.That (relationEndPointDefinition.PropertyInfo.PropertyType, Is.SameAs (typeof (ObjectList<ClassWithRealRelationEndPoints>)));
      Assert.That (relationEndPointDefinition.Cardinality, Is.EqualTo (CardinalityType.Many));
      Assert.That (relationEndPointDefinition.RelationDefinition, Is.Null);
      Assert.That (relationEndPointDefinition.SortExpressionText, Is.EqualTo ("NoAttribute"));
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("BaseBidirectionalOneToOne"));
      var relationEndPointReflector = CreateRelationEndPointReflector (propertyInfo);

      Assert.That (relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.True);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("BaseBidirectionalOneToMany"));
      var relationEndPointReflector = CreateRelationEndPointReflector (propertyInfo);

      Assert.That (relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.True);
    }

    private RdbmsRelationEndPointReflector CreateRelationEndPointReflector (PropertyInfoAdapter propertyInfo)
    {
      return new RdbmsRelationEndPointReflector (
          _classDefinition,
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub);
    }
  }
}