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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class GenericManySideRelationProperty : MappingReflectionTestBase
  {
    private ClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (ClosedGenericClassWithRealRelationEndPoints));
    }

    [Test]
    public void GetMetadata_Unidirectional ()
    {
      DomainModelConstraintProviderStub
        .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseUnidirectional")))
        .Return (true);

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseUnidirectional");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.That (relationEndPointDefinition.ClassDefinition, Is.SameAs (_classDefinition));
      Assert.That (relationEndPointDefinition.PropertyDefinition, Is.SameAs (GetPropertyDefinition ("BaseUnidirectional")));
      Assert.That (relationEndPointDefinition.RelationDefinition, Is.Null);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne ()
    {
      DomainModelConstraintProviderStub
        .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToOne")))
        .Return (true);

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToOne");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.That (relationEndPointDefinition.ClassDefinition, Is.SameAs (_classDefinition));
      Assert.That (relationEndPointDefinition.PropertyDefinition, Is.SameAs (GetPropertyDefinition ("BaseBidirectionalOneToOne")));
      Assert.That (relationEndPointDefinition.RelationDefinition, Is.Null);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      DomainModelConstraintProviderStub
        .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToMany")))
        .Return (true);

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToMany");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.That (relationEndPointDefinition.ClassDefinition, Is.SameAs (_classDefinition));
      Assert.That (relationEndPointDefinition.PropertyDefinition, Is.SameAs (GetPropertyDefinition ("BaseBidirectionalOneToMany")));
      Assert.That (relationEndPointDefinition.RelationDefinition, Is.Null);
    }


    [Test]
    public void IsVirtualEndRelationEndpoint_Unidirectional ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseUnidirectional");

      Assert.That (relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.False);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToOne");

      Assert.That (relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.False);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToMany");

      Assert.That (relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.False);
    }

    private RdbmsRelationEndPointReflector CreateRelationEndPointReflector (string propertyName)
    {
      PropertyReflector propertyReflector = CreatePropertyReflector (propertyName);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      return new RdbmsRelationEndPointReflector (
          _classDefinition,
          propertyReflector.PropertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub);
    }

    private PropertyReflector CreatePropertyReflector (string property)
    {
      Type type = typeof (ClosedGenericClassWithRealRelationEndPoints);
      var propertyInfo =
          PropertyInfoAdapter.Create (type.GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

      return new PropertyReflector (
          _classDefinition,
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub);
    }

    private PropertyDefinition GetPropertyDefinition (string propertyName)
    {
      return _classDefinition.MyPropertyDefinitions[
          string.Format ("{0}.{1}", typeof (GenericClassWithRealRelationEndPointsNotInMapping<>).FullName, propertyName)];
    }
  }
}