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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Reflection;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.TenantTests
{
  [TestFixture]
  public class BindableObjectImplementation : TenantTestBase
  {
    [Test]
    public void Get_UniqueIdentifier ()
    {
      Tenant tenant = TestHelper.CreateTenant("TestTenant", "UID: testTenant");

      Assert.That(tenant.UniqueIdentifier, Is.Not.Empty);
    }

    [Test]
    public void GetDisplayName ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenantname", "UID");

      Assert.That(tenant.DisplayName, Is.EqualTo("Tenantname"));
    }

    [Test]
    public void GetAndSet_UniqueIdentifier ()
    {
      Tenant tenant = TestHelper.CreateTenant("TestTenant", string.Empty);

      tenant.UniqueIdentifier = "My Unique Identifier";

      Assert.That(tenant.UniqueIdentifier, Is.EqualTo("My Unique Identifier"));
    }

    [Test]
    public void GetAndSet_UniqueIdentifierFromBusinessObjectWithIdentity ()
    {
      Tenant tenant = TestHelper.CreateTenant("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;

      tenant.UniqueIdentifier = "My Unique Identifier";

      Assert.That(businessObject.UniqueIdentifier, Is.EqualTo(tenant.ID.ToString()));
    }

    [Test]
    public void GetProperty_UniqueIdentifier ()
    {
      Tenant tenant = TestHelper.CreateTenant("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;

      tenant.UniqueIdentifier = "My Unique Identifier";

      Assert.That(businessObject.GetProperty("UniqueIdentifier"), Is.EqualTo("My Unique Identifier"));
      Assert.That(businessObject.UniqueIdentifier, Is.EqualTo(tenant.ID.ToString()));
    }

    [Test]
    public void SetProperty_UniqueIdentifier ()
    {
      Tenant tenant = TestHelper.CreateTenant("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;

      businessObject.SetProperty("UniqueIdentifier", "My Unique Identifier");
      Assert.That(tenant.UniqueIdentifier, Is.EqualTo("My Unique Identifier"));
      Assert.That(businessObject.UniqueIdentifier, Is.EqualTo(tenant.ID.ToString()));
    }

    [Test]
    public void GetPropertyDefinition_UniqueIdentifier ()
    {
      Tenant tenant = TestHelper.CreateTenant("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;
      tenant.UniqueIdentifier = "My Unique Identifier";

      IBusinessObjectProperty property = businessObject.BusinessObjectClass.GetPropertyDefinition("UniqueIdentifier");

      Assert.That(property, Is.InstanceOf(typeof(IBusinessObjectStringProperty)));
      Assert.That(businessObject.GetProperty(property), Is.EqualTo("My Unique Identifier"));
    }

    [Test]
    public void GetPropertyDefinitions_CheckForUniqueIdentifier ()
    {
      Tenant tenant = TestHelper.CreateTenant("TestTenant", string.Empty);
      IBusinessObjectWithIdentity businessObject = tenant;

      IBusinessObjectProperty[] properties = businessObject.BusinessObjectClass.GetPropertyDefinitions();

      bool isFound = false;
      foreach (PropertyBase property in properties)
      {
        if (property.Identifier == "UniqueIdentifier" && property.PropertyInfo.DeclaringType == TypeAdapter.Create(typeof(Tenant)))
        {
          isFound = true;
          break;
        }
      }

      Assert.That(isFound, Is.True, "Property UnqiueIdentifier declared on Tenant was not found.");
    }

    [Test]
    public void SearchParents ()
    {
      var searchServiceStub = new Mock<ISearchAvailableObjectsService>();
      var args = new Mock<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService(typeof(TenantPropertyTypeSearchService), searchServiceStub.Object);
      IBusinessObjectClass tenantClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(Tenant));
      IBusinessObjectReferenceProperty parentProperty = (IBusinessObjectReferenceProperty)tenantClass.GetPropertyDefinition("Parent");
      Assert.That(parentProperty, Is.Not.Null);

      Tenant tenant = TestHelper.CreateTenant("TestTenant", string.Empty);
      var expected = new[] { new Mock<IBusinessObject>().Object };

      searchServiceStub.Setup(stub => stub.SupportsProperty(parentProperty)).Returns(true);
      searchServiceStub.Setup(stub => stub.Search(tenant, parentProperty, args.Object)).Returns(expected);

      Assert.That(parentProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = parentProperty.SearchAvailableObjects(tenant, args.Object);
      Assert.That(actual, Is.SameAs(expected));
    }
  }
}
