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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Security;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.ObjectBinding;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class PositionTest : DomainTest
  {
    [Test]
    public void FindAll ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(ClientTransaction.CreateRootTransaction());
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        var positions = Position.FindAll();

        Assert.That(positions.Count(), Is.EqualTo(3));
      }
    }

    [Test]
    public void DeletePosition_WithAccessControlEntry ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Tenant tenant = dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(ClientTransaction.Current);
        User user = User.FindByTenant(tenant.GetHandle()).First();
        Role role = user.Roles[0];
        Position position = role.Position;
        AccessControlEntry ace = testHelper.CreateAceWithPosition(position);
        ClientTransaction.Current.Commit();

        position.Delete();

        ClientTransaction.Current.Commit();

        Assert.That(ace.State.IsInvalid, Is.True);
      }
    }

    [Test]
    public void DeletePosition_WithRole ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Tenant tenant = dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(ClientTransaction.Current);
        User user = User.FindByTenant(tenant.GetHandle()).First();
        Role role = user.Roles[0];
        Position position = role.Position;
        position.Delete();

        ClientTransaction.Current.Commit();

        Assert.That(role.State.IsInvalid, Is.True);
      }
    }

    [Test]
    public void DeletePosition_WithGroupTypePosition ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        GroupType groupType = testHelper.CreateGroupType("GroupType");
        Position position = testHelper.CreatePosition("Position");
        GroupTypePosition concretePosition = testHelper.CreateGroupTypePosition(groupType, position);

        position.Delete();

        Assert.That(concretePosition.State.IsInvalid, Is.True);
      }
    }

    [Test]
    public void GetDisplayName ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition("PositionName");

        Assert.That(position.DisplayName, Is.EqualTo("PositionName"));
      }
    }

    [Test]
    public void GetSecurityStrategy ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition("PositionName");

        IObjectSecurityStrategy objectSecurityStrategy = position.GetSecurityStrategy();
        Assert.That(objectSecurityStrategy, Is.Not.Null);
        Assert.That(objectSecurityStrategy, Is.InstanceOf(typeof(DomainObjectSecurityStrategyDecorator)));
        DomainObjectSecurityStrategyDecorator domainObjectSecurityStrategyDecorator = (DomainObjectSecurityStrategyDecorator)objectSecurityStrategy;
        Assert.That(domainObjectSecurityStrategyDecorator.RequiredSecurityForStates, Is.EqualTo(RequiredSecurityForStates.None));
      }
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition("PositionName");

        Assert.That(position.GetSecurityStrategy(), Is.SameAs(position.GetSecurityStrategy()));
      }
    }

    [Test]
    public void GetSecurableType ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition("PositionName");

        Assert.That(position.GetSecurableType(), Is.SameAs(typeof(Position)));
      }
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition("PositionName");
        IDomainObjectSecurityContextFactory factory = position;

        Assert.That(factory.IsInvalid, Is.False);
        Assert.That(factory.IsNew, Is.True);
        Assert.That(factory.IsDeleted, Is.False);

        position.Delete();

        Assert.That(factory.IsInvalid, Is.True);
      }
    }

    [Test]
    public void CreateSecurityContext ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition("PositionName");
        position.Delegation = Delegation.Enabled;

        ISecurityContext securityContext = ((ISecurityContextFactory)position).CreateSecurityContext();
        Assert.That(Type.GetType(securityContext.Class), Is.EqualTo(position.GetPublicDomainObjectType()));
        Assert.That(securityContext.Owner, Is.Null);
        Assert.That(securityContext.OwnerGroup, Is.Null);
        Assert.That(securityContext.OwnerTenant, Is.Null);
        Assert.That(securityContext.AbstractRoles, Is.Empty);
        Assert.That(securityContext.GetNumberOfStates(), Is.EqualTo(1));
        Assert.That(securityContext.GetState("Delegation"), Is.EqualTo(EnumWrapper.Get(Delegation.Enabled)));
      }
    }

    [Test]
    public void Get_UniqueIdentifier ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition("Position");

        Assert.IsNotEmpty(position.UniqueIdentifier);
      }
    }

    [Test]
    public void UniqueIdentifier_SameIdentifierTwice ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position1 = testHelper.CreatePosition("Position1");
        position1.UniqueIdentifier = "UID";

        Position position2 = testHelper.CreatePosition("Position2");
        position2.UniqueIdentifier = "UID";

        Assert.That(
            () => ClientTransactionScope.CurrentTransaction.Commit(),
            Throws.InstanceOf<RdbmsProviderException>());
      }
    }

    [Test]
    public void Initializ_SetsUniqueIdentityInsideSecurityFreeSection ()
    {
      var extensionStub = new Mock<IClientTransactionExtension>();
      bool propertyValueChangingCalled = false;
      extensionStub
          .Setup(_ => _.Key)
          .Returns("STUB");
      extensionStub
          .Setup(_ => _.PropertyValueChanging(It.IsAny<ClientTransaction>(), It.IsAny<DomainObject>(), It.IsAny<PropertyDefinition>(), It.IsAny<object>(), It.IsAny<object>()))
          .Callback(
              (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue) =>
              {
                if (propertyDefinition.PropertyInfo.Name == "UniqueIdentifier")
                {
                  propertyValueChangingCalled = true;
                  Assert.That(SecurityFreeSection.IsActive, Is.True);
                }
              });

      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        testHelper.Transaction.Extensions.Add(extensionStub.Object);
        Assert.That(SecurityFreeSection.IsActive, Is.False);
        testHelper.CreatePosition("Position1");
        Assert.That(SecurityFreeSection.IsActive, Is.False);
        Assert.That(propertyValueChangingCalled, Is.True);
      }
    }

    [Test]
    public void Get_Delegable_WithDelegationEnabled_ReturnsTrue ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition("Position");

        position.Delegation = Delegation.Enabled;
        Assert.That(position.Delegable, Is.True);
      }
    }

    [Test]
    public void Get_Delegable_WithDelegationDisabled_ReturnsFalse ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition("Position");

        position.Delegation = Delegation.Disabled;
        Assert.That(position.Delegable, Is.False);
      }
    }

    [Test]
    public void Set_Delegable_WithTrue_SetsDelegationToEnabled ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition("Position");

        position.Delegable = true;
        Assert.That(position.Delegation, Is.EqualTo(Delegation.Enabled));
      }
    }

    [Test]
    public void Set_Delegable_WithFalse_SetsDelegationToDisabled ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition("Position");

        position.Delegable = false;
        Assert.That(position.Delegation, Is.EqualTo(Delegation.Disabled));
      }
    }

    [Test]
    public void Get_Delegable_AsBusinessObjectProperty_WithDefaultValue_ReturnsNull ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition("Position");

        IBusinessObject businessObject = position;
        IBusinessObjectProperty property = businessObject.BusinessObjectClass.GetPropertyDefinition("Delegable");

        Assert.That(businessObject.GetProperty(property), Is.Null);
      }
    }

    [Test]
    public void Get_Delegable_AsBusinessObjectProperty_WithExplicitValue_ReturnsValue ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition("Position");

        IBusinessObject businessObject = position;
        IBusinessObjectProperty property = businessObject.BusinessObjectClass.GetPropertyDefinition("Delegable");

        var value = BooleanObjectMother.GetRandomBoolean();
        position.Delegable = value;
        Assert.That(businessObject.GetProperty(property), Is.EqualTo(value));
      }
    }
  }
}
