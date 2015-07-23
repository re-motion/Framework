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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StatelessAccessControlListTest:DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope ();
    }

    [Test]
    public void GetClass ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateClassDefinition ("SecurableClass");
      StatelessAccessControlList acl = _testHelper.CreateStatelessAcl (classDefinition);

      Assert.That (acl.Class, Is.SameAs (classDefinition));
    }

    [Test]
    public void OnCommitting_WithChangedAcl_RegistersClassForCommit ()
    {
      var classDefinition = _testHelper.CreateClassDefinition ("SecurableClass");
      var acl = _testHelper.CreateStatelessAcl (classDefinition);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That (GetDataContainer ((DomainObject) sender).HasBeenMarkedChanged, Is.True);
        };
        acl.RegisterForCommit();

        ClientTransaction.Current.Commit();

        Assert.That (commitOnClassWasCalled, Is.True);
      }
    }

    [Test]
    public void OnCommitting_WithDeletedAcl_RegistersClassForCommit ()
    {
      var classDefinition = _testHelper.CreateClassDefinition ("SecurableClass");
      var acl = _testHelper.CreateStatelessAcl (classDefinition);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That (GetDataContainer ((DomainObject) sender).HasBeenMarkedChanged, Is.True);
        };
        acl.Delete();

        ClientTransaction.Current.Commit();

        Assert.That (commitOnClassWasCalled, Is.True);
      }
    }
  }
}
