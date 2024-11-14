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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.RoleTests;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.SubstitutionTests
{
  [TestFixture]
  public class CommitSubstitution : RoleTestBase
  {
    private User _substitutedUser;
    private Substitution _substitution;

    public override void SetUp ()
    {
      base.SetUp();

      var tenant = TestHelper.CreateTenant("TestTenant", "UID: testTenant");
      var userGroup = TestHelper.CreateGroup("UserGroup", Guid.NewGuid().ToString(), null, tenant);
      _substitutedUser = TestHelper.CreateUser("user", "Firstname", "Lastname", "Title", userGroup, tenant);

      var substitutingUser = TestHelper.CreateUser("substitutingUser", "Firstname", "Lastname", "Title", userGroup, tenant);
      _substitution = Substitution.NewObject();
      _substitution.SubstitutedUser = _substitutedUser;
      _substitution.SubstitutingUser = substitutingUser;

      ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope();
    }

    [Test]
    public void WithSubstitutionChanged_RegistersSubstitutedUserForCommit ()
    {
      bool commitOnClassWasCalled = false;
      _substitutedUser.Committing += (sender, e) =>
      {
        commitOnClassWasCalled = true;
        Assert.That(GetDataContainer((DomainObject)sender).HasBeenMarkedChanged, Is.True);
      };
      _substitution.RegisterForCommit();
      ClientTransaction.Current.Commit();

      Assert.That(commitOnClassWasCalled, Is.True);
    }

    [Test]
    public void WithSubstitutionDeleted_RegistersSubstitutedUserForCommit ()
    {
      bool commitOnClassWasCalled = false;
      _substitutedUser.Committing += (sender, e) =>
      {
        commitOnClassWasCalled = true;
        Assert.That(GetDataContainer((DomainObject)sender).HasBeenMarkedChanged, Is.True);
      };
      _substitution.Delete();
      ClientTransaction.Current.Commit();

      Assert.That(commitOnClassWasCalled, Is.True);
    }
  }
}
