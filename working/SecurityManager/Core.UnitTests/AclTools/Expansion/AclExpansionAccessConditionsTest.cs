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
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionAccessConditionsTest : AclToolsTestBase
  {
    [Test]
    public void DefaultCtor ()
    {
      var accessConditions = new AclExpansionAccessConditions ();
      Assert.That (accessConditions.AbstractRole, Is.Null);
      Assert.That (accessConditions.IsAbstractRoleRequired, Is.EqualTo(false));
      Assert.That (accessConditions.HasOwningGroupCondition, Is.EqualTo (false));
      Assert.That (accessConditions.HasOwningTenantCondition, Is.EqualTo (false));
      Assert.That (accessConditions.IsOwningUserRequired, Is.EqualTo (false));

      Assert.That (accessConditions.OwningGroup, Is.EqualTo (null));
      Assert.That (accessConditions.GroupHierarchyCondition, Is.EqualTo (GroupHierarchyCondition.Undefined));
    }


    [Test]
    public void EqualsByCheckingCompoundValueEqualityComparerParticipatingObjects ()
    {
      var a = new AclExpansionAccessConditions ();
      var equalityObjects = AclExpansionAccessConditions.EqualityComparer.GetEqualityParticipatingObjects (a);
      Assert.That (equalityObjects, Is.EqualTo (new object[] { a.AbstractRole, a.OwningGroup, a.OwningTenant, 
        a.GroupHierarchyCondition, a.TenantHierarchyCondition, a.IsOwningUserRequired }));
    }


    [Test]
    public void GetHashCodeTest ()
    {
      var aclExpansionAccessConditions = new AclExpansionAccessConditions ();
      Assert.That (aclExpansionAccessConditions.GetHashCode(), 
        Is.EqualTo (AclExpansionAccessConditions.EqualityComparer.GetHashCode (aclExpansionAccessConditions)));
    }
  }
}
