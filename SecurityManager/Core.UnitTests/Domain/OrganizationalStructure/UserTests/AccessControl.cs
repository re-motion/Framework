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
using Remotion.Reflection;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class AccessControl : UserTestBase
  {
    private SecurityTestHelper _securityTestHelper;
    private User _user;

    public override void SetUp ()
    {
      base.SetUp();
      _securityTestHelper = new SecurityTestHelper();
      _user = CreateUser();

      ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope();
    }

    [Test]
    public void Roles_PropertyWriteAccessGranted ()
    {
      var securityClient = _securityTestHelper.CreatedStubbedSecurityClient<User>(SecurityManagerAccessTypes.AssignRole);
      var methodInformation = MethodInfoAdapter.Create(typeof(User).GetProperty("Roles").GetSetMethod(true));

      Assert.That(securityClient.HasPropertyWriteAccess(_user, methodInformation), Is.True);
    }

    [Test]
    public void Roles_PropertyWriteAccessDenied ()
    {
      var securityClient = _securityTestHelper.CreatedStubbedSecurityClient<User>();
      var methodInformation = MethodInfoAdapter.Create(typeof(User).GetProperty("Roles").GetSetMethod(true));

      SimulateExistingObjectForSecurityTest(_user);

      Assert.That(securityClient.HasPropertyWriteAccess(_user, methodInformation), Is.False);
    }

    [Test]
    public void SubstitutedBy_PropertyWriteAccessGranted ()
    {
      var securityClient = _securityTestHelper.CreatedStubbedSecurityClient<User>(SecurityManagerAccessTypes.AssignSubstitute);
      var methodInformation = MethodInfoAdapter.Create(typeof(User).GetProperty("SubstitutedBy").GetSetMethod(true));

      Assert.That(securityClient.HasPropertyWriteAccess(_user, methodInformation), Is.True);
    }

    [Test]
    public void SubstitutedBy_PropertyWriteAccessDenied ()
    {
      var securityClient = _securityTestHelper.CreatedStubbedSecurityClient<User>();
      var methodInformation = MethodInfoAdapter.Create(typeof(User).GetProperty("SubstitutedBy").GetSetMethod(true));

      SimulateExistingObjectForSecurityTest(_user);

      Assert.That(securityClient.HasPropertyWriteAccess(_user, methodInformation), Is.False);
    }
  }
}
