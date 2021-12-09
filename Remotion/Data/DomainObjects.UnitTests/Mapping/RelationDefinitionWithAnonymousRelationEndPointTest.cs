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
  public class RelationDefinitionWithAnonymousRelationEndPointTest : MappingReflectionTestBase
  {
    private RelationDefinition _relation;
    private AnonymousRelationEndPointDefinition _clientEndPoint;
    private RelationEndPointDefinition _locationEndPoint;

    public override void SetUp ()
    {
      base.SetUp();

      _relation = FakeMappingConfiguration.Current.RelationDefinitions[
        "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Location:Remotion.Data.DomainObjects.UnitTests.Mapping."
        +"TestDomain.Integration.Location.Client"];
      _clientEndPoint = (AnonymousRelationEndPointDefinition)_relation.EndPointDefinitions[0];
      _locationEndPoint = (RelationEndPointDefinition)_relation.EndPointDefinitions[1];
    }

    [Test]
    public void GetOppositeEndPointDefinition ()
    {
      Assert.That(
          _relation.GetOppositeEndPointDefinition(typeof(Location), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Location.Client"),
          Is.SameAs(_clientEndPoint));
      Assert.That(_relation.GetOppositeEndPointDefinition(typeof(Client), null), Is.SameAs(_locationEndPoint));
    }

    [Test]
    public void IsEndPoint ()
    {
      Assert.That(_relation.IsEndPoint(typeof(Location), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Location.Client"), Is.True);
      Assert.That(_relation.IsEndPoint(typeof(Client), null), Is.True);

      Assert.That(_relation.IsEndPoint(typeof(Location), null), Is.False);
      Assert.That(_relation.IsEndPoint(typeof(Client), "Client"), Is.False);
    }
  }
}
