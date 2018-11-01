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
      var securityClient = _securityTestHelper.CreatedStubbedSecurityClient<User> (SecurityManagerAccessTypes.AssignRole);
      var methodInformation = MethodInfoAdapter.Create(typeof (User).GetProperty ("Roles").GetSetMethod (true));

      Assert.That (securityClient.HasPropertyWriteAccess (_user, methodInformation), Is.True);
    }

    [Test]
    public void Roles_PropertyWriteAccessDenied ()
    {
      var securityClient = _securityTestHelper.CreatedStubbedSecurityClient<User>();
      var methodInformation = MethodInfoAdapter.Create(typeof (User).GetProperty ("Roles").GetSetMethod (true));

      Assert.That (securityClient.HasPropertyWriteAccess (_user, methodInformation), Is.False);
    }

    [Test]
    public void SubstitutedBy_PropertyWriteAccessGranted ()
    {
      var securityClient = _securityTestHelper.CreatedStubbedSecurityClient<User> (SecurityManagerAccessTypes.AssignSubstitute);
      var methodInformation = MethodInfoAdapter.Create(typeof (User).GetProperty ("SubstitutedBy").GetSetMethod (true));

      Assert.That (securityClient.HasPropertyWriteAccess (_user, methodInformation), Is.True);
    }

    [Test]
    public void SubstitutedBy_PropertyWriteAccessDenied ()
    {
      var securityClient = _securityTestHelper.CreatedStubbedSecurityClient<User> ();
      var methodInformation = MethodInfoAdapter.Create(typeof (User).GetProperty ("SubstitutedBy").GetSetMethod (true));

      Assert.That (securityClient.HasPropertyWriteAccess (_user, methodInformation), Is.False);
    }
  }
}