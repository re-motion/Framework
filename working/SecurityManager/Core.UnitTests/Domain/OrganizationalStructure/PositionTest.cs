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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Security;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class PositionTest : DomainTest
  {
    [Test]
    public void FindAll ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var positions = Position.FindAll();

        Assert.That (positions.Count(), Is.EqualTo (3));
      }
    }

    [Test]
    public void DeletePosition_WithAccessControlEntry ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        Tenant tenant = dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.Current);
        User user = User.FindByTenant (tenant.GetHandle()).First();
        Role role = user.Roles[0];
        Position position = role.Position;
        AccessControlEntry ace = testHelper.CreateAceWithPosition (position);
        ClientTransaction.Current.Commit();

        position.Delete();

        ClientTransaction.Current.Commit();

        Assert.That (ace.State, Is.EqualTo (StateType.Invalid));
      }
    }

    [Test]
    public void DeletePosition_WithRole ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Tenant tenant = dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.Current);
        User user = User.FindByTenant (tenant.GetHandle()).First();
        Role role = user.Roles[0];
        Position position = role.Position;
        position.Delete ();

        ClientTransaction.Current.Commit ();

        Assert.That (role.State, Is.EqualTo (StateType.Invalid));
      }
    }

    [Test]
    public void DeletePosition_WithGroupTypePosition ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        GroupType groupType = testHelper.CreateGroupType ("GroupType");
        Position position = testHelper.CreatePosition ("Position");
        GroupTypePosition concretePosition = testHelper.CreateGroupTypePosition (groupType, position);

        position.Delete();

        Assert.That (concretePosition.State, Is.EqualTo (StateType.Invalid));
      }
    }

    [Test]
    public void GetDisplayName ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("PositionName");

        Assert.That (position.DisplayName, Is.EqualTo ("PositionName"));
      }
    }

    [Test]
    public void GetSecurityStrategy ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition ("PositionName");

        IObjectSecurityStrategy objectSecurityStrategy = position.GetSecurityStrategy();
        Assert.That (objectSecurityStrategy, Is.Not.Null);
        Assert.IsInstanceOf (typeof (DomainObjectSecurityStrategyDecorator), objectSecurityStrategy);
        DomainObjectSecurityStrategyDecorator domainObjectSecurityStrategyDecorator = (DomainObjectSecurityStrategyDecorator) objectSecurityStrategy;
        Assert.That (domainObjectSecurityStrategyDecorator.RequiredSecurityForStates, Is.EqualTo (RequiredSecurityForStates.None));
      }
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition ("PositionName");

        Assert.That (position.GetSecurityStrategy(), Is.SameAs (position.GetSecurityStrategy()));
      }
    }

    [Test]
    public void GetSecurableType ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition ("PositionName");

        Assert.That (position.GetSecurableType(), Is.SameAs (typeof (Position)));
      }
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("PositionName");
        IDomainObjectSecurityContextFactory factory = position;

        Assert.That (factory.IsInvalid, Is.False);
        Assert.That (factory.IsNew, Is.True);
        Assert.That (factory.IsDeleted, Is.False);

        position.Delete();

        Assert.That (factory.IsInvalid, Is.True);
      }
    }

    [Test]
    public void CreateSecurityContext ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("PositionName");
        position.Delegation = Delegation.Enabled;

        ISecurityContext securityContext = ((ISecurityContextFactory) position).CreateSecurityContext();
        Assert.That (Type.GetType (securityContext.Class), Is.EqualTo (position.GetPublicDomainObjectType()));
        Assert.That (securityContext.Owner, Is.Null);
        Assert.That (securityContext.OwnerGroup, Is.Null);
        Assert.That (securityContext.OwnerTenant, Is.Null);
        Assert.That (securityContext.AbstractRoles, Is.Empty);
        Assert.That (securityContext.GetNumberOfStates (), Is.EqualTo (1));
        Assert.That (securityContext.GetState ("Delegation"), Is.EqualTo (EnumWrapper.Get (Delegation.Enabled)));
      }
    }

    [Test]
    public void Get_UniqueIdentifier ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("Position");

        Assert.IsNotEmpty (position.UniqueIdentifier);
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UniqueIdentifier_SameIdentifierTwice ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position1 = testHelper.CreatePosition ("Position1");
        position1.UniqueIdentifier = "UID";

        Position position2 = testHelper.CreatePosition ("Position2");
        position2.UniqueIdentifier = "UID";

        ClientTransactionScope.CurrentTransaction.Commit();
      }
    }

    [Test]
    public void Initializ_SetsUniqueIdentityInsideSecurityFreeSection ()
    {
      var extensionStub = MockRepository.GenerateStub<IClientTransactionExtension>();
      bool propertyValueChangingCalled = false;
      extensionStub.Stub (_ => _.Key).Return ("STUB");
      extensionStub.Stub (_ => _.PropertyValueChanging (null, null, null, null, null)).IgnoreArguments().WhenCalled (
          mi =>
          {
            var propertyDefinition = ((PropertyDefinition) mi.Arguments[2]);
            if (propertyDefinition.PropertyInfo.Name == "UniqueIdentifier")
            {
              propertyValueChangingCalled = true;
              Assert.That (SecurityFreeSection.IsActive, Is.True);
            }
          });

      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        testHelper.Transaction.Extensions.Add (extensionStub);
        Assert.That (SecurityFreeSection.IsActive, Is.False);
        testHelper.CreatePosition ("Position1");
        Assert.That (SecurityFreeSection.IsActive, Is.False);
        Assert.That (propertyValueChangingCalled, Is.True);
      }
    }
  }
}
