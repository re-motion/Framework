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

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class AnonymousRelationEndPointDefinitionTest : MappingReflectionTestBase
  {
    private TypeDefinition _clientDefinition;
    private AnonymousRelationEndPointDefinition _definition;

    public override void SetUp ()
    {
      base.SetUp();

      _clientDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Client));
      _definition = new AnonymousRelationEndPointDefinition(_clientDefinition);
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsInstanceOf<IRelationEndPointDefinition>(_definition);
      Assert.That(_definition.TypeDefinition, Is.SameAs(_clientDefinition));
      Assert.That(_definition.Cardinality, Is.EqualTo(CardinalityType.Many));
      Assert.That(_definition.IsMandatory, Is.EqualTo(false));
      Assert.That(_definition.IsVirtual, Is.EqualTo(true));
      Assert.That(_definition.PropertyName, Is.Null);
      Assert.That(_definition.PropertyInfo, Is.Null);
      Assert.That(_definition.IsAnonymous, Is.True);
    }

    [Test]
    public void RelationDefinition_NotSet ()
    {
      AnonymousRelationEndPointDefinition definition = new AnonymousRelationEndPointDefinition(MappingConfiguration.Current.GetTypeDefinition(typeof(Client)));

      Assert.That(definition.HasRelationDefinitionBeenSet, Is.False);
      Assert.That(
          () => definition.RelationDefinition,
          Throws.InvalidOperationException.With.Message.EqualTo("RelationDefinition has not been set for this relation end point."));
    }

    [Test]
    public void SetRelationDefinition ()
    {
      var propertyDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Location))
        ["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Location.Client"];
      var oppositeEndPoint = new RelationEndPointDefinition(propertyDefinition, true);
      var relationDefinition = new RelationDefinition("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Location.Client", _definition, oppositeEndPoint);

      _definition.SetRelationDefinition(relationDefinition);

      Assert.That(_definition.HasRelationDefinitionBeenSet, Is.True);
      Assert.That(_definition.RelationDefinition, Is.SameAs(relationDefinition));
    }
  }
}
