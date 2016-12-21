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
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpanderAclFinderTest : AclToolsTestBase
  {
    [Test]
    public void AclFinderTest ()
    {
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var aclExpanderAclFinder = new AclExpanderAclFinder();
        var acls = aclExpanderAclFinder.FindAccessControlLists().ToList();

        Assert.That (new[] { Acl, Acl2 }, Is.SubsetOf (acls));
      }
    }
  }
}
