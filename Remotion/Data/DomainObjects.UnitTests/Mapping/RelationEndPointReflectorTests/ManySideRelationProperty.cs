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
  public class ManySideRelationProperty : MappingReflectionTestBase
  {
    private ClassDefinition _classDefinition;
    private Type _classType;

    public override void SetUp ()
    {
      base.SetUp();

      _classType = typeof (ClassWithRealRelationEndPoints);
      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: _classType);
    }

    [Test]
    public void GetMetadata_ForOptional ()
    {
      DomainModelConstraintProviderStub
        .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "NoAttribute")))
        .Return (true);

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("NoAttribute");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      Assert.That (actual.IsMandatory, Is.False);
    }

    [Test]
    public void GetMetadata_ForMandatory ()
    {
      DomainModelConstraintProviderStub
        .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "NotNullable")))
        .Return (false);

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("NotNullable");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      Assert.That (actual.IsMandatory, Is.True);
    }

    [Test]
    public void GetMetadata_Unidirectional ()
    {
      DomainModelConstraintProviderStub
        .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Unidirectional")))
        .Return (true);

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("Unidirectional");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.That (relationEndPointDefinition.ClassDefinition, Is.SameAs (_classDefinition));
      Assert.That (relationEndPointDefinition.PropertyDefinition, Is.SameAs (GetPropertyDefinition ("Unidirectional")));
      Assert.That (relationEndPointDefinition.RelationDefinition, Is.Null);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne ()
    {
      DomainModelConstraintProviderStub
        .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
        .Return (true);

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BidirectionalOneToOne");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.That (relationEndPointDefinition.ClassDefinition, Is.SameAs (_classDefinition));
      Assert.That (relationEndPointDefinition.PropertyDefinition, Is.SameAs (GetPropertyDefinition ("BidirectionalOneToOne")));
      Assert.That (relationEndPointDefinition.RelationDefinition, Is.Null);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      DomainModelConstraintProviderStub
        .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
        .Return (true);

      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BidirectionalOneToMany");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefinition = (RelationEndPointDefinition) actual;
      Assert.That (relationEndPointDefinition.ClassDefinition, Is.SameAs (_classDefinition));
      Assert.That (relationEndPointDefinition.PropertyDefinition, Is.SameAs (GetPropertyDefinition ("BidirectionalOneToMany")));
      Assert.That (relationEndPointDefinition.RelationDefinition, Is.Null);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_Unidirectional ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("Unidirectional");

      Assert.That (relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.False);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BidirectionalOneToOne");

      Assert.That (relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.False);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BidirectionalOneToMany");

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
      var propertyInfo =
          PropertyInfoAdapter.Create (_classType.GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

      return new PropertyReflector (
          _classDefinition,
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub);
    }

    private PropertyDefinition GetPropertyDefinition (string propertyName)
    {
      return _classDefinition.MyPropertyDefinitions[string.Format ("{0}.{1}", _classType.FullName, propertyName)];
    }
  }
}