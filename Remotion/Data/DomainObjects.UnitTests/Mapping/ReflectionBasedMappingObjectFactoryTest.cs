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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class ReflectionBasedMappingObjectFactoryTest : StandardMappingTest
  {
    private ReflectionBasedMemberInformationNameResolver _mappingMemberInformationNameResolver;
    private ThrowingDomainObjectCreator _domainObjectCreator;

    private ReflectionBasedMappingObjectFactory _factory;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _mappingMemberInformationNameResolver = new ReflectionBasedMemberInformationNameResolver();
      _domainObjectCreator = new ThrowingDomainObjectCreator();

      _factory = new ReflectionBasedMappingObjectFactory(
          _mappingMemberInformationNameResolver,
          new ClassIDProvider(),
          new PropertyMetadataReflector(),
          new DomainModelConstraintProvider(),
          new LegacyPropertyDefaultValueProvider(),
          new SortExpressionDefinitionProvider(),
          _domainObjectCreator);
    }

    [Test]
    public void CreateClassDefinition ()
    {
      var result = _factory.CreateClassDefinition(typeof(Order), null);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.ClassType, Is.SameAs(typeof(Order)));
      Assert.That(result.InstanceCreator, Is.SameAs(_domainObjectCreator));
      Assert.That(result.BaseClass, Is.Null);
    }

    [Test]
    public void CreateClassDefinition_WithBaseClass ()
    {
      var companyClass = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(Company));
      companyClass.SetPropertyDefinitions(new PropertyDefinitionCollection());
      companyClass.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());
      var result = _factory.CreateClassDefinition(typeof(Customer), companyClass);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.ClassType, Is.SameAs(typeof(Customer)));
      Assert.That(result.InstanceCreator, Is.SameAs(_domainObjectCreator));
      Assert.That(result.BaseClass, Is.SameAs(companyClass));
    }

    [Test]
    public void CreatePropertyDefinition ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Order), baseClass: null);
      var propertyInfo = PropertyInfoAdapter.Create(typeof(Order).GetProperty("OrderItems"));

      var result = _factory.CreatePropertyDefinition(classDefinition, propertyInfo);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.PropertyInfo, Is.SameAs(propertyInfo));
    }

    [Test]
    public void CreateRelationDefinition ()
    {
      var orderClassDefinition = MappingConfiguration.Current.GetClassDefinition("Order");
      var orderItemClassDefinition = MappingConfiguration.Current.GetClassDefinition("OrderItem");

      var result =
          _factory.CreateRelationDefinition(
              new[] { orderClassDefinition, orderItemClassDefinition }.ToDictionary(cd => cd.ClassType),
              orderItemClassDefinition,
              orderItemClassDefinition.MyRelationEndPointDefinitions[0].PropertyInfo);

      Assert.That(result, Is.Not.Null);
      Assert.That(
          result.EndPointDefinitions[0],
          Is.SameAs(orderItemClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"]));
      Assert.That(
          result.EndPointDefinitions[1],
          Is.SameAs(orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"]));
    }

    [Test]
    public void CreateRelationEndPointDefinition ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Order), baseClass: null);
      var propertyInfo = PropertyInfoAdapter.Create(typeof(Order).GetProperty("OrderItems"));

      var result = _factory.CreateRelationEndPointDefinition(classDefinition, propertyInfo);

      Assert.That(result, Is.TypeOf(typeof(DomainObjectCollectionRelationEndPointDefinition)));
      Assert.That(((DomainObjectCollectionRelationEndPointDefinition)result).PropertyInfo, Is.SameAs(propertyInfo));
    }

    [Test]
    public void CreateClassDefinitionCollection ()
    {
      var result = _factory.CreateClassDefinitionCollection(new[] { typeof(Order), typeof(Company) });

      Assert.That(result.Length, Is.EqualTo(2));
      Assert.That(result.Any(cd => cd.ClassType == typeof(Order)));
      Assert.That(result.Any(cd => cd.ClassType == typeof(Company)));
    }

    [Test]
    public void CreatePropertyDefinitionCollection ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Order), baseClass: null);
      var propertyInfo1 = PropertyInfoAdapter.Create(typeof(Order).GetProperty("OrderNumber"));
      var propertyInfo2 = PropertyInfoAdapter.Create(typeof(Order).GetProperty("DeliveryDate"));

      var result = _factory.CreatePropertyDefinitionCollection(classDefinition, new[] { propertyInfo1, propertyInfo2 });

      Assert.That(result.Count, Is.EqualTo(2));
      Assert.That(result[0].PropertyInfo, Is.SameAs(propertyInfo1));
      Assert.That(result[1].PropertyInfo, Is.SameAs(propertyInfo2));
    }

    [Test]
    public void CreateRelationDefinitionCollection ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(OrderItem));
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(classDefinition, typeof(OrderItem), "Order");
      var endPoint = new RelationEndPointDefinition(propertyDefinition, false);
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));
      classDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { endPoint }, true));

      var result = _factory.CreateRelationDefinitionCollection(new[] { classDefinition }.ToDictionary(cd => cd.ClassType));

      Assert.That(result.Count(), Is.EqualTo(1));
      Assert.That(
          result.First().ID,
          Is.EqualTo(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderItem:"
              +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderItem.Order->"
              + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderItems"));
    }

    [Test]
    public void CreateRelationEndPointDefinitionCollection ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(OrderTicket), baseClass: null);
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(classDefinition, typeof(OrderTicket), "Order");
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));

      var result = _factory.CreateRelationEndPointDefinitionCollection(classDefinition);

      Assert.That(result.Count, Is.EqualTo(1));
      Assert.That(
          ((RelationEndPointDefinition)result[0]).PropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderTicket.Order"));
    }
  }
}
