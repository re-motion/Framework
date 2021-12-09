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
  public class ClassDefinitionWithNullEndPointTest : MappingReflectionTestBase
  {
    // types

    // static members and constants

    // member fields

    private TypeDefinition _clientClass;
    private AnonymousRelationEndPointDefinition _clientEndPoint;
    private TypeDefinition _locationClass;
    private RelationEndPointDefinition _locationEndPoint;

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp();

      _clientClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof(Client)];
      _locationClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof(Location)];

      RelationDefinition relation = FakeMappingConfiguration.Current.RelationDefinitions[
          "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Location:Remotion.Data.DomainObjects.UnitTests.Mapping."
          + "TestDomain.Integration.Location.Client"];
      _clientEndPoint = (AnonymousRelationEndPointDefinition)relation.EndPointDefinitions[0];
      _locationEndPoint = (RelationEndPointDefinition)relation.EndPointDefinitions[1];
    }

    [Test]
    public void GetRelationEndPointDefinitions ()
    {
      Assert.That(_locationClass.GetRelationEndPointDefinitions(), Has.Member(_locationEndPoint));
      Assert.That(_clientClass.GetRelationEndPointDefinitions(), Has.No.Member(_clientEndPoint));
    }

    [Test]
    public void GetMyRelationEndPointDefinitions ()
    {
      Assert.That(_locationClass.MyRelationEndPointDefinitions, Has.Member(_locationEndPoint));
      Assert.That(_clientClass.MyRelationEndPointDefinitions, Has.No.Member(_clientEndPoint));
    }
  }
}
