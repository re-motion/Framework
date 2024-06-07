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
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class BindableObjectImpelementation : UserTestBase
  {
    public override void TearDown ()
    {
      base.TearDown();
      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void GetDisplayName_WithLastNameAndFirstNameAndTitle ()
    {
      User user = CreateUser();
      user.LastName = "UserLastName";
      user.FirstName = "UserFirstName";
      user.Title = "UserTitle";

      Assert.That(user.DisplayName, Is.EqualTo("UserLastName UserFirstName, UserTitle"));
    }

    [Test]
    public void GetDisplayName_WithLastNameAndFirstName ()
    {
      User user = CreateUser();
      user.LastName = "UserLastName";
      user.FirstName = "UserFirstName";
      user.Title = null;

      Assert.That(user.DisplayName, Is.EqualTo("UserLastName UserFirstName"));
    }

    [Test]
    public void GetDisplayName_WithLastName ()
    {
      User user = CreateUser();
      user.LastName = "UserLastName";
      user.FirstName = null;
      user.Title = null;

      Assert.That(user.DisplayName, Is.EqualTo("UserLastName"));
    }

    [Test]
    public void SearchOwningGroups ()
    {
      var searchServiceStub = new Mock<ISearchAvailableObjectsService>();
      var args = new Mock<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService(typeof(GroupPropertyTypeSearchService), searchServiceStub.Object);
      IBusinessObjectClass userClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(User));
      IBusinessObjectReferenceProperty owningGroupProperty = (IBusinessObjectReferenceProperty)userClass.GetPropertyDefinition("OwningGroup");
      Assert.That(owningGroupProperty, Is.Not.Null);

      User user = CreateUser();
      var expected = new[] { new Mock<IBusinessObject>().Object };

      searchServiceStub.Setup(stub => stub.SupportsProperty(owningGroupProperty)).Returns(true);
      searchServiceStub.Setup(stub => stub.Search(user, owningGroupProperty, args.Object)).Returns(expected);

      Assert.That(owningGroupProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = owningGroupProperty.SearchAvailableObjects(user, args.Object);
      Assert.That(actual, Is.SameAs(expected));
    }
  }
}
