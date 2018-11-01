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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;


namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionTreeNodeTest : AclToolsTestBase
  {
    [Test]
    public void CtorTest ()
    {
      List<Role> children = ListObjectMother.New (Role, Role2, Role3);
      var aclExpansionTreeNode = new AclExpansionTreeNode<User, Role> (User, 17, children);
      Assert.That (aclExpansionTreeNode.Key, Is.EqualTo (User));
      Assert.That (aclExpansionTreeNode.NumberLeafNodes, Is.EqualTo (17));
      Assert.That (aclExpansionTreeNode.Children, Is.EqualTo (children));
    }

    [Test]
    public void FactoryTest ()
    {
      List<Role> children = ListObjectMother.New (Role, Role2, Role3);
      var aclExpansionTreeNode = AclExpansionTreeNode.New (User, 17, children);
      Assert.That (aclExpansionTreeNode.Key, Is.EqualTo (User));
      Assert.That (aclExpansionTreeNode.NumberLeafNodes, Is.EqualTo (17));
      Assert.That (aclExpansionTreeNode.Children, Is.EqualTo (children));
    }
  }
}