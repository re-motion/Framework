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
      var accessConditions = new AclExpansionAccessConditions();
      Assert.That(accessConditions.AbstractRole, Is.Null);
      Assert.That(accessConditions.IsAbstractRoleRequired, Is.EqualTo(false));
      Assert.That(accessConditions.HasOwningGroupCondition, Is.EqualTo(false));
      Assert.That(accessConditions.HasOwningTenantCondition, Is.EqualTo(false));
      Assert.That(accessConditions.IsOwningUserRequired, Is.EqualTo(false));

      Assert.That(accessConditions.OwningGroup, Is.EqualTo(null));
      Assert.That(accessConditions.GroupHierarchyCondition, Is.EqualTo(GroupHierarchyCondition.Undefined));
    }


    [Test]
    public void EqualsByCheckingCompoundValueEqualityComparerParticipatingObjects ()
    {
      var a = new AclExpansionAccessConditions();
      var equalityObjects = AclExpansionAccessConditions.EqualityComparer.GetEqualityParticipatingObjects(a);
      Assert.That(equalityObjects, Is.EqualTo(new object[] { a.AbstractRole, a.OwningGroup, a.OwningTenant,
        a.GroupHierarchyCondition, a.TenantHierarchyCondition, a.IsOwningUserRequired }));
    }


    [Test]
    public void GetHashCodeTest ()
    {
      var aclExpansionAccessConditions = new AclExpansionAccessConditions();
      Assert.That(aclExpansionAccessConditions.GetHashCode(),
        Is.EqualTo(AclExpansionAccessConditions.EqualityComparer.GetHashCode(aclExpansionAccessConditions)));
    }
  }
}
