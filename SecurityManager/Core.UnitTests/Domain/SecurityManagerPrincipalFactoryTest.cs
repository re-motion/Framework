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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class SecurityManagerPrincipalFactoryTest
  {
    [Test]
    public void Create_WithMinimumData ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var dbFixtures = new DatabaseFixtures();
        var tenant = dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(ClientTransaction.Current);
        var user = User.FindByTenant(tenant.GetHandle()).First();

        var factory = new SecurityManagerPrincipalFactory();

        var principal = factory.Create(tenant.GetHandle(), user.GetHandle(), null);

        Assert.That(principal, Is.TypeOf<SecurityManagerPrincipal>());
        Assert.That(principal.Tenant.ID, Is.EqualTo(tenant.ID));
        Assert.That(principal.User.ID, Is.EqualTo(user.ID));
        Assert.That(principal.Roles, Is.Null);
        Assert.That(principal.Substitution, Is.Null);
      }
    }

    [Test]
    public void Create_WithCompleteData ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var dbFixtures = new DatabaseFixtures();
        var tenant = dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(ClientTransaction.Current);
        var user = User.FindByTenant(tenant.GetHandle()).AsEnumerable().First(u => u.Roles.Any() && u.GetActiveSubstitutions().Any());
        var substitution = user.GetActiveSubstitutions().First(s => s.SubstitutedRole != null);

        var factory = new SecurityManagerPrincipalFactory();

        var principal = factory.Create(
            tenant.GetHandle(),
            user.GetHandle(),
            substitution.GetHandle());

        Assert.That(principal, Is.TypeOf<SecurityManagerPrincipal>());
        Assert.That(principal.Tenant.ID, Is.EqualTo(tenant.ID));
        Assert.That(principal.User.ID, Is.EqualTo(user.ID));
        Assert.That(principal.Roles, Is.Null);
        Assert.That(principal.Substitution, Is.Not.Null);
        var securityPrincipal = principal.GetSecurityPrincipal();

        Assert.That(securityPrincipal.SubstitutedUser, Is.EqualTo(substitution.SubstitutedUser.UserName));
        Assert.That(
            securityPrincipal.SubstitutedRoles,
            Is.EquivalentTo(
                new ISecurityPrincipalRole[]
                {
                  new SecurityPrincipalRole(
                      substitution.SubstitutedRole.Group.UniqueIdentifier,
                      substitution.SubstitutedRole.Position.UniqueIdentifier)
                }));
      }
    }
  }
}
