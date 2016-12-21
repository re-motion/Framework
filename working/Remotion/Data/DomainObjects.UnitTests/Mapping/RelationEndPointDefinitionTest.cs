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
    private VirtualRelationEndPointDefinition _customerEndPoint;
    private RelationEndPointDefinition _orderEndPoint;

    public override void SetUp ()
    {
      base.SetUp ();

      RelationDefinition customerToOrder = FakeMappingConfiguration.Current.RelationDefinitions[
        "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order:Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain."
        + "Integration.Order.Customer->Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain."
        + "Integration.Customer.Orders"];

      _customerEndPoint = (VirtualRelationEndPointDefinition) customerToOrder.GetEndPointDefinition (
          "Customer", "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer.Orders");

      _orderEndPoint = (RelationEndPointDefinition) customerToOrder.GetEndPointDefinition (
          "Order", "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.Customer");
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Relation definition error: Property 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Company.Name' of class 'Company' is of type "
        + "'System.String', but non-virtual properties must be of type 'Remotion.Data.DomainObjects.ObjectID'.")]
    public void Initialization_PropertyOfWrongType ()
    {
      var companyDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Company)];
      var propertyDefinition = companyDefinition["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Company.Name"];

      new RelationEndPointDefinition (propertyDefinition, false);
    }

    [Test]
    public void Initialize ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Order));
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID (classDefinition);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection { propertyDefinition });
      var endPoint = new RelationEndPointDefinition (propertyDefinition, true);

      Assert.That (endPoint.PropertyInfo, Is.SameAs (propertyDefinition.PropertyInfo));
    }

    [Test]
    public void IsAnonymous ()
    {
      Assert.That (_orderEndPoint.IsAnonymous, Is.False);
    }

    [Test]
    public void RelationDefinitionNull ()
    {
      var classDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof (OrderTicket)];
      var propertyDefinition = classDefinition["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderTicket.Order"];

      var definition = new RelationEndPointDefinition (propertyDefinition, true);

      Assert.That (definition.RelationDefinition, Is.Null);
    }

    [Test]
    public void RelationDefinitionNotNull ()
    {
      Assert.That (_orderEndPoint.RelationDefinition, Is.Not.Null);
    }
  }
}
