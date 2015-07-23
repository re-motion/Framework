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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation.SecurityTokenMatcherTests
{
  [TestFixture]
  public class AceWithInvalidValues : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value 'Undefined' is not a valid value for matching the 'GroupHierarchyCondition'.")]
    public void GroupHierarchyCondition_UndefinedValue ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup();
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Undefined;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value 'Parent' is not a valid value for matching the 'GroupHierarchyCondition'.")]
    public void GroupHierarchyCondition_Parent ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup ();
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Parent;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value 'Children' is not a valid value for matching the 'GroupHierarchyCondition'.")]
    public void GroupHierarchyCondition_Children ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup ();
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Children;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value 'Undefined' is not a valid value for matching the 'TenantHierarchyCondition'.")]
    public void TenantHierarchyCondition_UndefinedValue ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Tenant owningTenant = _companyHelper.CompanyTenant;

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (user, owningTenant);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant ();
      ace.TenantHierarchyCondition = TenantHierarchyCondition.Undefined;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The value 'Parent' is not a valid value for matching the 'TenantHierarchyCondition'.")]
    public void TenantHierarchyCondition_Parent ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Tenant owningTenant = _companyHelper.CompanyTenant;

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (user, owningTenant);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant ();
      ace.TenantHierarchyCondition = TenantHierarchyCondition.Parent;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (ace);

      matcher.MatchesToken (token);
    }
  }
}
