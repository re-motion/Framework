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
      base.SetUp ();

      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();

      User user = User.FindByUserName ("substituting.user");
      _userHandle = user.GetHandle();
      _tenantHandle = user.Tenant.GetHandle();
      _substitutionIDs = user.GetActiveSubstitutions().Select (s => s.ID).ToArray();
      Assert.That (_substitutionIDs.Length, Is.EqualTo (2));
    }

    public override void TearDown ()
    {
      base.TearDown ();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
    }

    [Test]
    public void ExcludeInactiveSubstitutions ()
    {
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (_tenantHandle, _userHandle, null);

      Assert.That (principal.GetActiveSubstitutions().Select (s => s.ID), Is.EquivalentTo (_substitutionIDs));
    }
  }
}