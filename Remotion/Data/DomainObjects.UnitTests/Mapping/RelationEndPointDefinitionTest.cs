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
  public class RelationEndPointDefinitionTest : MappingReflectionTestBase
  {
    private DomainObjectCollectionRelationEndPointDefinition _customerEndPoint;
    private RelationEndPointDefinition _orderEndPoint;

    public override void SetUp ()
    {
      base.SetUp();

      RelationDefinition customerToOrder = FakeMappingConfiguration.Current.RelationDefinitions[
        "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order:Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain."
        + "Integration.Order.Customer->Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain."
        + "Integration.Customer.Orders"];

      _customerEndPoint = (DomainObjectCollectionRelationEndPointDefinition)customerToOrder.GetEndPointDefinition(
          typeof(Customer), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer.Orders");

      _orderEndPoint = (RelationEndPointDefinition)customerToOrder.GetEndPointDefinition(
          typeof(Order), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.Customer");
    }

    [Test]
    public void Initialization_PropertyOfWrongType ()
    {
      var companyDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof(Company)];
      var propertyDefinition = companyDefinition["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Company.Name"];
      Assert.That(
          () => new RelationEndPointDefinition(propertyDefinition, false),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Relation definition error: Property 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Company.Name' of type "
                  + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Company' is of type "
                  + "'System.String', but non-virtual properties must be of type 'Remotion.Data.DomainObjects.ObjectID'."));
    }

    [Test]
    public void Initialize ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(Order));
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID(typeDefinition);
      typeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection { propertyDefinition });
      var endPoint = new RelationEndPointDefinition(propertyDefinition, true);

      Assert.That(endPoint.PropertyInfo, Is.SameAs(propertyDefinition.PropertyInfo));
    }

    [Test]
    public void IsAnonymous ()
    {
      Assert.That(_orderEndPoint.IsAnonymous, Is.False);
    }

    [Test]
    public void RelationDefinition_NotSet ()
    {
      var typeDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof(OrderTicket)];
      var propertyDefinition = typeDefinition["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderTicket.Order"];

      var definition = new RelationEndPointDefinition(propertyDefinition, true);

      Assert.That(definition.HasRelationDefinitionBeenSet, Is.False);
      Assert.That(
          () => definition.RelationDefinition,
          Throws.InvalidOperationException.With.Message.EqualTo("RelationDefinition has not been set for this relation end point."));
    }

    [Test]
    public void RelationDefinition_NotNull ()
    {
      Assert.That(_orderEndPoint.HasRelationDefinitionBeenSet, Is.True);
      Assert.That(_orderEndPoint.RelationDefinition, Is.Not.Null);
    }
  }
}
