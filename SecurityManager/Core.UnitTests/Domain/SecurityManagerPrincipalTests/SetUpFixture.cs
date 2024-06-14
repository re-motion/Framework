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

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      try
      {
        _testHelper = new OrganizationalStructureTestHelper();

        _dbFixtures = new DatabaseFixtures();
        var tenant = _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(_testHelper.Transaction);

        using (_testHelper.Transaction.EnterNonDiscardingScope())
        {
          Tenant child = _testHelper.CreateTenant("Child", "UID: TenantChild");
          child.IsAbstract = true;
          child.Parent = tenant;
          Tenant grandChild = _testHelper.CreateTenant("GrandChild", "UID: TenantGrandChild");
          grandChild.Parent = child;
          ClientTransaction.Current.Commit();
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }
  }
}
