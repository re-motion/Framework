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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class BindableObjectImpelementation : UserTestBase
  {
    public override void TearDown ()
    {
      base.TearDown();
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void GetDisplayName_WithLastNameAndFirstNameAndTitle ()
    {
      User user = CreateUser();
      user.LastName = "UserLastName";
      user.FirstName = "UserFirstName";
      user.Title = "UserTitle";

      Assert.That (user.DisplayName, Is.EqualTo ("UserLastName UserFirstName, UserTitle"));
    }

    [Test]
    public void GetDisplayName_WithLastNameAndFirstName ()
    {
      User user = CreateUser();
      user.LastName = "UserLastName";
      user.FirstName = "UserFirstName";
      user.Title = null;

      Assert.That (user.DisplayName, Is.EqualTo ("UserLastName UserFirstName"));
    }

    [Test]
    public void GetDisplayName_WithLastName ()
    {
      User user = CreateUser();
      user.LastName = "UserLastName";
      user.FirstName = null;
      user.Title = null;

      Assert.That (user.DisplayName, Is.EqualTo ("UserLastName"));
    }

    [Test]
    public void SearchOwningGroups ()
    {
      ISearchAvailableObjectsService searchServiceStub = MockRepository.GenerateStub<ISearchAvailableObjectsService>();
      ISearchAvailableObjectsArguments args = MockRepository.GenerateStub<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService (typeof (GroupPropertyTypeSearchService), searchServiceStub);
      IBusinessObjectClass userClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (User));
      IBusinessObjectReferenceProperty owningGroupProperty = (IBusinessObjectReferenceProperty) userClass.GetPropertyDefinition ("OwningGroup");
      Assert.That (owningGroupProperty, Is.Not.Null);

      User user = CreateUser();
      var expected = new[] { MockRepository.GenerateStub<IBusinessObject> () };

      searchServiceStub.Stub (stub => stub.SupportsProperty (owningGroupProperty)).Return (true);
      searchServiceStub.Stub (stub => stub.Search (user, owningGroupProperty, args)).Return (expected);

      Assert.That (owningGroupProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = owningGroupProperty.SearchAvailableObjects (user, args);
      Assert.That (actual, Is.SameAs (expected));
    }
  }
}
