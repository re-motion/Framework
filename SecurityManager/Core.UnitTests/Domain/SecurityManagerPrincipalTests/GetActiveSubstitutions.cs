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
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class GetActiveSubstitutions : SecurityManagerPrincipalTestBase
  {
    private IDomainObjectHandle<Tenant> _tenantHandle;
    private IDomainObjectHandle<User> _userHandle;
    private ObjectID[] _substitutionIDs;

    public override void SetUp ()
    {
      base.SetUp();

      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();

      User user = User.FindByUserName("substituting.user");
      _userHandle = user.GetHandle();
      _tenantHandle = user.Tenant.GetHandle();
      _substitutionIDs = user.GetActiveSubstitutions().Select(s => s.ID).ToArray();
      Assert.That(_substitutionIDs.Length, Is.EqualTo(2));
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
    }

    [Test]
    public void ExcludeInactiveSubstitutions ()
    {
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal(_tenantHandle, _userHandle, null, null, null, null);

      Assert.That(principal.GetActiveSubstitutions().Select(s => s.ID), Is.EquivalentTo(_substitutionIDs));
    }
  }
}
