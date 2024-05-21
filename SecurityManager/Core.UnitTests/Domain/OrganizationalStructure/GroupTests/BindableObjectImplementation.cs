// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
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

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupTests
{
  [TestFixture]
  public class BindableObjectImplementation : GroupTestBase
  {
    public override void TearDown ()
    {
      base.TearDown();
      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void Get_UniqueIdentifier ()
    {
      OrganizationalStructureFactory factory = new OrganizationalStructureFactory();
      Group group = factory.CreateGroup();

      Assert.IsNotEmpty(group.UniqueIdentifier);
    }

    [Test]
    public void GetDisplayName_WithShortName ()
    {
      Group group = CreateGroup();
      group.Name = "LongGroupName";
      group.ShortName = "ShortName";

      Assert.That(@group.DisplayName, Is.EqualTo("ShortName (LongGroupName)"));
    }

    [Test]
    public void GetDisplayName_NoShortName ()
    {
      Group group = CreateGroup();
      group.Name = "LongGroupName";
      group.ShortName = null;

      Assert.That(@group.DisplayName, Is.EqualTo("LongGroupName"));
    }

    [Test]
    public void SetAndGet_UniqueIdentifier ()
    {
      Group group = CreateGroup();

      group.UniqueIdentifier = "My Unique Identifier";

      Assert.That(@group.UniqueIdentifier, Is.EqualTo("My Unique Identifier"));
    }

    [Test]
    public void SetAndGet_UniqueIdentifierFromBusinessObjectWithIdentity ()
    {
      Group group = CreateGroup();
      IBusinessObjectWithIdentity businessObject = group;

      group.UniqueIdentifier = "My Unique Identifier";

      Assert.That(businessObject.UniqueIdentifier, Is.EqualTo(@group.ID.ToString()));
    }

    [Test]
    public void GetProperty_UniqueIdentifier ()
    {
      Group group = CreateGroup();
      IBusinessObjectWithIdentity businessObject = group;

      group.UniqueIdentifier = "My Unique Identifier";

      Assert.That(businessObject.GetProperty("UniqueIdentifier"), Is.EqualTo("My Unique Identifier"));
      Assert.That(businessObject.UniqueIdentifier, Is.EqualTo(@group.ID.ToString()));
    }

    [Test]
    public void SetProperty_UniqueIdentifier ()
    {
      Group group = CreateGroup();
      IBusinessObjectWithIdentity businessObject = group;

      businessObject.SetProperty("UniqueIdentifier", "My Unique Identifier");
      Assert.That(@group.UniqueIdentifier, Is.EqualTo("My Unique Identifier"));
      Assert.That(businessObject.UniqueIdentifier, Is.EqualTo(@group.ID.ToString()));
    }

    [Test]
    public void GetPropertyDefinition_UniqueIdentifier ()
    {
      Group group = CreateGroup();
      IBusinessObjectWithIdentity businessObject = group;
      group.UniqueIdentifier = "My Unique Identifier";

      IBusinessObjectProperty property = businessObject.BusinessObjectClass.GetPropertyDefinition("UniqueIdentifier");

      Assert.That(property, Is.InstanceOf(typeof(IBusinessObjectStringProperty)));
      Assert.That(businessObject.GetProperty(property), Is.EqualTo("My Unique Identifier"));
    }

    [Test]
    public void GetPropertyDefinitions_CheckForUniqueIdentifier ()
    {
      Group group = CreateGroup();
      IBusinessObjectWithIdentity businessObject = group;

      IBusinessObjectProperty[] properties = businessObject.BusinessObjectClass.GetPropertyDefinitions();

      bool isFound = false;
      foreach (PropertyBase property in properties)
      {
        if (property.Identifier == "UniqueIdentifier" && property.PropertyInfo.DeclaringType.Equals(TypeAdapter.Create(typeof(Group))))
        {
          isFound = true;
          break;
        }
      }

      Assert.IsTrue(isFound, "Property UnqiueIdentifier declared on Group was not found.");
    }

    [Test]
    public void SearchParentGroups ()
    {
      var searchServiceStub = new Mock<ISearchAvailableObjectsService>();
      var args = new Mock<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService(typeof(GroupPropertyTypeSearchService), searchServiceStub.Object);
      var groupClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(Group));
      var groupTypeProperty = (IBusinessObjectReferenceProperty)groupClass.GetPropertyDefinition("Parent");
      Assert.That(groupTypeProperty, Is.Not.Null);

      var group = CreateGroup();
      var expected = new[] { new Mock<IBusinessObject>().Object };

      searchServiceStub.Setup(stub => stub.SupportsProperty(groupTypeProperty)).Returns(true);
      searchServiceStub.Setup(stub => stub.Search(group, groupTypeProperty, args.Object)).Returns(expected);

      Assert.That(groupTypeProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = groupTypeProperty.SearchAvailableObjects(group, args.Object);
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void SearchGroupTypes ()
    {
      var searchServiceStub = new Mock<ISearchAvailableObjectsService>();
      var args = new Mock<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService(typeof(GroupTypePropertyTypeSearchService), searchServiceStub.Object);
      var groupClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(Group));
      var groupTypeProperty = (IBusinessObjectReferenceProperty)groupClass.GetPropertyDefinition("GroupType");
      Assert.That(groupTypeProperty, Is.Not.Null);

      var group = CreateGroup();
      var expected = new[] { new Mock<IBusinessObject>().Object };

      searchServiceStub.Setup(stub => stub.SupportsProperty(groupTypeProperty)).Returns(true);
      searchServiceStub.Setup(stub => stub.Search(group, groupTypeProperty, args.Object)).Returns(expected);

      Assert.That(groupTypeProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = groupTypeProperty.SearchAvailableObjects(group, args.Object);
      Assert.That(actual, Is.SameAs(expected));
    }
  }
}
