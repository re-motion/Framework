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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupTypePositionTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp();
      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
    }

    public override void TearDown ()
    {
      base.TearDown();
      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void GetDisplayName_WithGroupTypeAndPosition ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition();

      Assert.That(groupTypePosition.DisplayName, Is.EqualTo("GroupTypeName / PositionName"));
    }

    [Test]
    public void GetDisplayName_WithoutPosition ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition();
      groupTypePosition.Position = null;

      Assert.That(groupTypePosition.DisplayName, Is.EqualTo("GroupTypeName / "));
    }

    [Test]
    public void GetDisplayName_WithoutGroupType ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition();
      groupTypePosition.GroupType = null;

      Assert.That(groupTypePosition.DisplayName, Is.EqualTo(" / PositionName"));
    }

    [Test]
    public void GetDisplayName_WithoutGroupTypeAndWithoutPosition ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition();
      groupTypePosition.GroupType = null;
      groupTypePosition.Position = null;

      Assert.That(groupTypePosition.DisplayName, Is.EqualTo(" / "));
    }

    [Test]
    public void SearchGroupTypes ()
    {
      var searchServiceStub = new Mock<ISearchAvailableObjectsService>();
      var args = new Mock<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService(typeof(GroupTypePropertyTypeSearchService), searchServiceStub.Object);
      var groupTypePositionClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(GroupTypePosition));
      var groupTypeProperty = (IBusinessObjectReferenceProperty)groupTypePositionClass.GetPropertyDefinition("GroupType");
      Assert.That(groupTypeProperty, Is.Not.Null);

      var groupTypePosition = CreateGroupTypePosition();
      var expected = new[] { new Mock<IBusinessObject>().Object };

      searchServiceStub.Setup(stub => stub.SupportsProperty(groupTypeProperty)).Returns(true);
      searchServiceStub.Setup(stub => stub.Search(groupTypePosition, groupTypeProperty, args.Object)).Returns(expected);

      Assert.That(groupTypeProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = groupTypeProperty.SearchAvailableObjects(groupTypePosition, args.Object);
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void SearchPositions ()
    {
      var searchServiceStub = new Mock<ISearchAvailableObjectsService>();
      var args = new Mock<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService(typeof(PositionPropertyTypeSearchService), searchServiceStub.Object);
      var groupTypePositionClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(GroupTypePosition));
      var positionProperty = (IBusinessObjectReferenceProperty)groupTypePositionClass.GetPropertyDefinition("Position");
      Assert.That(positionProperty, Is.Not.Null);

      var groupTypePosition = CreateGroupTypePosition();
      var expected = new[] { new Mock<IBusinessObject>().Object };

      searchServiceStub.Setup(stub => stub.SupportsProperty(positionProperty)).Returns(true);
      searchServiceStub.Setup(stub => stub.Search(groupTypePosition, positionProperty, args.Object)).Returns(expected);

      Assert.That(positionProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = positionProperty.SearchAvailableObjects(groupTypePosition, args.Object);
      Assert.That(actual, Is.SameAs(expected));
    }

    private static GroupTypePosition CreateGroupTypePosition ()
    {
      OrganizationalStructureFactory factory = new OrganizationalStructureFactory();

      GroupTypePosition groupTypePosition = GroupTypePosition.NewObject();

      groupTypePosition.GroupType = GroupType.NewObject();
      groupTypePosition.GroupType.Name = "GroupTypeName";

      groupTypePosition.Position = factory.CreatePosition();
      groupTypePosition.Position.Name = "PositionName";

      return groupTypePosition;
    }
  }
}
