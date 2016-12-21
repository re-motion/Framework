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
using Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private DatabaseFixtures _dbFixtures;
    private OrganizationalStructureTestHelper _testHelper;

    [SetUp]
    public void SetUp ()
    {
      try
      {
        _testHelper = new OrganizationalStructureTestHelper();

        _dbFixtures = new DatabaseFixtures();
        var tenant = _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (_testHelper.Transaction);

        using (_testHelper.Transaction.EnterNonDiscardingScope())
        {
          Tenant child = _testHelper.CreateTenant ("Child", "UID: TenantChild");
          child.IsAbstract = true;
          child.Parent = tenant;
          Tenant grandChild = _testHelper.CreateTenant ("GrandChild", "UID: TenantGrandChild");
          grandChild.Parent = child;
          ClientTransaction.Current.Commit();
        }
      }
      catch (Exception e)
      {
        Console.WriteLine (e);
        throw;
      }
    }
  }
}