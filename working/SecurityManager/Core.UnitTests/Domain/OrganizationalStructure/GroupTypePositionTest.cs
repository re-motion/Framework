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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupTypePositionTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();
    }

    public override void TearDown ()
    {
      base.TearDown();
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }
    
    [Test]
    public void GetDisplayName_WithGroupTypeAndPosition ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition ();

      Assert.That (groupTypePosition.DisplayName, Is.EqualTo ("GroupTypeName / PositionName"));
    }

    [Test]
    public void GetDisplayName_WithoutPosition ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition ();
      groupTypePosition.Position = null;

      Assert.That (groupTypePosition.DisplayName, Is.EqualTo ("GroupTypeName / "));
    }

    [Test]
    public void GetDisplayName_WithoutGroupType ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition ();
      groupTypePosition.GroupType = null;

      Assert.That (groupTypePosition.DisplayName, Is.EqualTo (" / PositionName"));
    }

    [Test]
    public void GetDisplayName_WithoutGroupTypeAndWithoutPosition ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition ();
      groupTypePosition.GroupType = null;
      groupTypePosition.Position = null;

      Assert.That (groupTypePosition.DisplayName, Is.EqualTo (" / "));
    }

    [Test]
    public void SearchGroupTypes ()
    {
      var searchServiceStub = MockRepository.GenerateStub<ISearchAvailableObjectsService>();
      var args = MockRepository.GenerateStub<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService (typeof (GroupTypePropertyTypeSearchService), searchServiceStub);
      var groupTypePositionClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (GroupTypePosition));
      var groupTypeProperty = (IBusinessObjectReferenceProperty) groupTypePositionClass.GetPropertyDefinition ("GroupType");
      Assert.That (groupTypeProperty, Is.Not.Null);

      var groupTypePosition = CreateGroupTypePosition();
      var expected = new[] { MockRepository.GenerateStub<IBusinessObject> () };

      searchServiceStub.Stub (stub => stub.SupportsProperty (groupTypeProperty)).Return (true);
      searchServiceStub.Stub (stub => stub.Search (groupTypePosition, groupTypeProperty, args)).Return (expected);

      Assert.That (groupTypeProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = groupTypeProperty.SearchAvailableObjects (groupTypePosition, args);
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void SearchPositions ()
    {
      var searchServiceStub = MockRepository.GenerateStub<ISearchAvailableObjectsService>();
      var args = MockRepository.GenerateStub<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService (typeof (PositionPropertyTypeSearchService), searchServiceStub);
      var groupTypePositionClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (GroupTypePosition));
      var positionProperty = (IBusinessObjectReferenceProperty) groupTypePositionClass.GetPropertyDefinition ("Position");
      Assert.That (positionProperty, Is.Not.Null);

      var groupTypePosition = CreateGroupTypePosition();
      var expected = new[] { MockRepository.GenerateStub<IBusinessObject> () };

      searchServiceStub.Stub (stub => stub.SupportsProperty (positionProperty)).Return (true);
      searchServiceStub.Stub (stub => stub.Search (groupTypePosition, positionProperty, args)).Return (expected);

      Assert.That (positionProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = positionProperty.SearchAvailableObjects (groupTypePosition, args);
      Assert.That (actual, Is.SameAs (expected));
    }

    private static GroupTypePosition CreateGroupTypePosition ()
    {
      OrganizationalStructureFactory factory = new OrganizationalStructureFactory ();

      GroupTypePosition groupTypePosition = GroupTypePosition.NewObject();

      groupTypePosition.GroupType = GroupType.NewObject();
      groupTypePosition.GroupType.Name = "GroupTypeName";

      groupTypePosition.Position = factory.CreatePosition ();
      groupTypePosition.Position.Name = "PositionName";

      return groupTypePosition;
    }
  }
}
