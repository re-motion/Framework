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
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class InvalidRelationEndPointDefinitionBaseTest
  {
    private class TestableInvalidRelationEndPointDefinitionBase : InvalidRelationEndPointDefinitionBase
    {
      public TestableInvalidRelationEndPointDefinitionBase (TypeDefinition typeDefinition, string propertyName, Type propertyType)
          : base(typeDefinition, propertyName, propertyType)
      {
      }
    }

    private InvalidRelationEndPointDefinitionBase _invalidEndPointDefinition;
    private TypeDefinition _typeDefinition;

    [SetUp]
    public void SetUp ()
    {
      _typeDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(Order));
      _invalidEndPointDefinition = new TestableInvalidRelationEndPointDefinitionBase(_typeDefinition, "TestProperty", typeof(string));
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_invalidEndPointDefinition.TypeDefinition, Is.SameAs(_typeDefinition));
      Assert.That(_invalidEndPointDefinition.PropertyName, Is.EqualTo("TestProperty"));
      Assert.That(_invalidEndPointDefinition.PropertyInfo.DeclaringType, Is.SameAs(TypeAdapter.Create(typeof(Order))));
      Assert.That(_invalidEndPointDefinition.PropertyInfo.Name, Is.EqualTo("TestProperty"));
      Assert.That(_invalidEndPointDefinition.PropertyInfo.PropertyType, Is.SameAs(typeof(string)));
      Assert.That(_invalidEndPointDefinition.IsVirtual, Is.False);
      Assert.That(_invalidEndPointDefinition.IsAnonymous, Is.False);
    }

    [Test]
    public void RelationDefinition_NotSet ()
    {
      Assert.That(_invalidEndPointDefinition.HasRelationDefinitionBeenSet, Is.False);
      Assert.That(
          () => _invalidEndPointDefinition.RelationDefinition,
          Throws.InvalidOperationException.With.Message.EqualTo("RelationDefinition has not been set for this relation end point."));
    }

    [Test]
    public void SetRelationDefinition ()
    {
      var endPoint = new AnonymousRelationEndPointDefinition(_typeDefinition);
      var relationDefinition = new RelationDefinition("Test", endPoint, endPoint);

      _invalidEndPointDefinition.SetRelationDefinition(relationDefinition);

      Assert.That(_invalidEndPointDefinition.HasRelationDefinitionBeenSet, Is.True);
      Assert.That(_invalidEndPointDefinition.RelationDefinition, Is.SameAs(relationDefinition));
    }

  }
}
