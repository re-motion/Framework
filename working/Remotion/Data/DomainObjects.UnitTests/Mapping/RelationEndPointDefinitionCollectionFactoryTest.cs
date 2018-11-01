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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class RelationEndPointDefinitionCollectionFactoryTest
  {
    private RelationEndPointDefinitionCollectionFactory _factory;
    private IMappingObjectFactory _mappingObjectFactoryMock;
    private IMemberInformationNameResolver _memberInformationNameResolverMock;

    [SetUp]
    public void SetUp ()
    {
      _mappingObjectFactoryMock = MockRepository.GenerateStrictMock<IMappingObjectFactory>();
      _memberInformationNameResolverMock = MockRepository.GenerateStrictMock<IMemberInformationNameResolver>();
      _factory = new RelationEndPointDefinitionCollectionFactory (
          _mappingObjectFactoryMock,
          _memberInformationNameResolverMock,
          new PropertyMetadataReflector());
    }

    [Test]
    public void CreateRelationEndPointDefinitionCollection ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (OrderTicket));
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID (classDefinition);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      var fakeRelationEndPoint = new RelationEndPointDefinition (propertyDefinition, false);

      var expectedPropertyInfo = PropertyInfoAdapter.Create (typeof (OrderTicket).GetProperty ("Order"));
      
      _mappingObjectFactoryMock
          .Expect (
              mock =>
              mock.CreateRelationEndPointDefinition (
                  Arg.Is (classDefinition), Arg.Is (PropertyInfoAdapter.Create (expectedPropertyInfo.PropertyInfo))))
          .Return (fakeRelationEndPoint);
      _mappingObjectFactoryMock.Replay();

      var result = _factory.CreateRelationEndPointDefinitionCollection (classDefinition);

      _mappingObjectFactoryMock.VerifyAllExpectations();
      _memberInformationNameResolverMock.VerifyAllExpectations();
      Assert.That (result.Count, Is.EqualTo (1));
      Assert.That (result[0], Is.SameAs (fakeRelationEndPoint));
    }
  }
}