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
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.TenantTests
{
  [TestFixture]
  public class Common : TenantTestBase
  {
    [Test]
    public void UniqueIdentifier_SameIdentifierTwice ()
    {
      string tenantUniqueIdentifier = Guid.NewGuid().ToString();
      TestHelper.CreateTenant("TestTenant1", tenantUniqueIdentifier);
      TestHelper.CreateTenant("TestTenant2", tenantUniqueIdentifier);
      Assert.That(
          () => ClientTransactionScope.CurrentTransaction.Commit(),
          Throws.InstanceOf<RdbmsProviderException>());
    }
  }
}
