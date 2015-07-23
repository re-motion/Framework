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

      var tenant = TestHelper.CreateTenant ("TestTenant", "UID: testTenant");
      var userGroup = TestHelper.CreateGroup ("UserGroup", Guid.NewGuid().ToString(), null, tenant);
      _substitutedUser = TestHelper.CreateUser ("user", "Firstname", "Lastname", "Title", userGroup, tenant);

      var substitutingUser = TestHelper.CreateUser ("substitutingUser", "Firstname", "Lastname", "Title", userGroup, tenant);
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
        Assert.That (GetDataContainer ((DomainObject) sender).HasBeenMarkedChanged, Is.True);
      };
      _substitution.RegisterForCommit();
      ClientTransaction.Current.Commit();

      Assert.That (commitOnClassWasCalled, Is.True);
    }

    [Test]
    public void WithSubstitutionDeleted_RegistersSubstitutedUserForCommit ()
    {
      bool commitOnClassWasCalled = false;
      _substitutedUser.Committing += (sender, e) =>
      {
        commitOnClassWasCalled = true;
        Assert.That (GetDataContainer ((DomainObject) sender).HasBeenMarkedChanged, Is.True);
      };
      _substitution.Delete();
      ClientTransaction.Current.Commit();

      Assert.That (commitOnClassWasCalled, Is.True);
    }
  }
}