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

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class PermissionTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void OnCommitting_WithChangedPermission_RegistersClassForCommit ()
    {
      var classDefinition = _testHelper.CreateClassDefinition ("SecurableClass");
      var acl = _testHelper.CreateStatelessAcl (classDefinition);
      var ace = _testHelper.CreateAceWithOwningUser();
      acl.AccessControlEntries.Add (ace);
      var accessType = _testHelper.AttachJournalizeAccessType (classDefinition);
      ace.AllowAccess (accessType);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That (GetDataContainer ((DomainObject) sender).HasBeenMarkedChanged, Is.True);
        };
        ace.GetPermissions()[0].RegisterForCommit();

        ClientTransaction.Current.Commit();

        Assert.That (commitOnClassWasCalled, Is.True);
      }
    }

    [Test]
    public void OnCommitting_WithDeletedPermission_RegistersClassForCommit ()
    {
      var classDefinition = _testHelper.CreateClassDefinition ("SecurableClass");
      var acl = _testHelper.CreateStatelessAcl (classDefinition);
      var ace = _testHelper.CreateAceWithOwningUser();
      acl.AccessControlEntries.Add (ace);
      var accessType = _testHelper.AttachJournalizeAccessType (classDefinition);
      ace.AllowAccess (accessType);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That (GetDataContainer ((DomainObject) sender).HasBeenMarkedChanged, Is.True);
        };
        ace.GetPermissions()[0].Delete();

        ClientTransaction.Current.Commit();

        Assert.That (commitOnClassWasCalled, Is.True);
      }
    }
  }
}