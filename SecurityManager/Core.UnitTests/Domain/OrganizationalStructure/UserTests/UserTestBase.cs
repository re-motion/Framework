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
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  public abstract class UserTestBase : DomainTest
  {
    private OrganizationalStructureTestHelper _testHelper;

    protected User CreateUser ()
    {
      Tenant tenant = _testHelper.CreateTenant("TestTenant", "UID: testTenant");
      Group group = _testHelper.CreateGroup("TestGroup", "UID: testGroup", null, tenant);
      User user = _testHelper.CreateUser("test.user", "Test", "User", "Ing.", group, tenant);

      return user;
    }

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    public OrganizationalStructureTestHelper TestHelper
    {
      get { return _testHelper; }
    }
  }
}
