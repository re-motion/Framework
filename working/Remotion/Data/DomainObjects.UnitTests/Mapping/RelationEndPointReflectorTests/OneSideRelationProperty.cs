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
  public class OneSideRelationProperty : MappingReflectionTestBase
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
    public void GetMetadata_ForOptional ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("NoAttribute"));
      var relationEndPointReflector = CreateRelationEndPointReflector (propertyInfo);

      DomainModelConstraintProviderStub.Stub (stub => stub.IsNullable (propertyInfo)).Return (true);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (VirtualRelationEndPointDefinition), actual);
      Assert.That (actual.PropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NoAttribute"));
      Assert.That (actual.IsMandatory, Is.False);
      DomainModelConstraintProviderStub.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_ForMandatory ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("NotNullable"));
      var relationEndPointReflector = CreateRelationEndPointReflector (propertyInfo);

      DomainModelConstraintProviderStub.Stub (stub => stub.IsNullable (propertyInfo)).Return (false);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (VirtualRelationEndPointDefinition), actual);
      Assert.That (actual.PropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NotNullable"));
      Assert.That (actual.IsMandatory, Is.True);
      DomainModelConstraintProviderStub.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("BidirectionalOneToOne"));
      var relationEndPointReflector = CreateRelationEndPointReflector (propertyInfo);

      DomainModelConstraintProviderStub.Stub (stub => stub.IsNullable (propertyInfo)).Return (true);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.That (relationEndPointDefinition.ClassDefinition, Is.SameAs (_classDefinition));
      Assert.That (relationEndPointDefinition.PropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToOne"));
      Assert.That (relationEndPointDefinition.PropertyInfo.PropertyType, Is.SameAs (typeof (ClassWithRealRelationEndPoints)));
      Assert.That (relationEndPointDefinition.Cardinality, Is.EqualTo (CardinalityType.One));
      Assert.That (relationEndPointDefinition.RelationDefinition, Is.Null);
      DomainModelConstraintProviderStub.VerifyAllExpectations();
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("BidirectionalOneToMany"));
      var relationEndPointReflector = CreateRelationEndPointReflector (propertyInfo);

      DomainModelConstraintProviderStub.Stub (stub => stub.IsNullable (propertyInfo)).Return (true);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOf (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.That (relationEndPointDefinition.ClassDefinition, Is.SameAs (_classDefinition));
      Assert.That (relationEndPointDefinition.PropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToMany"));
      Assert.That (relationEndPointDefinition.PropertyInfo.PropertyType, Is.SameAs (typeof (ObjectList<ClassWithRealRelationEndPoints>)));
      Assert.That (relationEndPointDefinition.Cardinality, Is.EqualTo (CardinalityType.Many));
      Assert.That (relationEndPointDefinition.RelationDefinition, Is.Null);
      Assert.That (relationEndPointDefinition.SortExpressionText, Is.EqualTo ("NoAttribute"));
      DomainModelConstraintProviderStub.VerifyAllExpectations();
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("BidirectionalOneToOne"));
      var relationEndPointReflector = CreateRelationEndPointReflector (propertyInfo);

      Assert.That (relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.True);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      var propertyInfo = PropertyInfoAdapter.Create (_classType.GetProperty ("BidirectionalOneToMany"));
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