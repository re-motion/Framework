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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class Initialize : SecurityManagerPrincipalTestBase
  {
    public override void SetUp ()
    {
      base.SetUp();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
    }

    [Test]
    public void Initialize_SetsMembers ()
    {
      User user = User.FindByUserName("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions().First();

      SecurityManagerPrincipal principal = CreateSecurityManagerPrincipal(tenant, user, null, substitution);

      Assert.That(principal.Tenant.ID, Is.EqualTo(tenant.ID));
      Assert.That(principal.Tenant, Is.Not.SameAs(tenant));

      Assert.That(principal.User.ID, Is.EqualTo(user.ID));
      Assert.That(principal.User, Is.Not.SameAs(user));

      Assert.That(principal.Substitution.ID, Is.EqualTo(substitution.ID));
      Assert.That(principal.Substitution, Is.Not.SameAs(substitution));
    }

    [Test]
    public void Initialize_WithInconsistentSubstitutionData1_ThrowsArgumentException ()
    {
      User user = User.FindByUserName("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions().First();

      Assert.That(
          () => CreateSecurityManagerPrincipal(tenant, user, null, null, substitution.SubstitutedUser),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "When the 'substitutedUserHandle' or the 'substitutedRoleHandles' are set, the 'substitutionHandle' must also be specified.", "substitutionHandle"));
    }

    [Test]
    public void Initialize_WithInconsistentSubstitutionData2_ThrowsArgumentException ()
    {
      User user = User.FindByUserName("substituting.user");
      Tenant tenant = user.Tenant;

      Assert.That(
          () => CreateSecurityManagerPrincipal(tenant, user, null, null, null, new Role[0]),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "When the 'substitutedUserHandle' or the 'substitutedRoleHandles' are set, the 'substitutionHandle' must also be specified.", "substitutionHandle"));
    }
  }
}
