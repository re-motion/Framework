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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation.SecurityTokenMatcherTests
{
  public class CompanyStructureHelper
  {
    private readonly OrganizationalStructureTestHelper _testHelper;

    public CompanyStructureHelper (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      _testHelper = new OrganizationalStructureTestHelper(clientTransaction);

      Build();
    }

    public Tenant CompanyTenant { get; private set; }
    public Position HeadPosition { get; private set; }
    public Position MemberPosition { get; private set; }
    public GroupType DivisionGroupType { get; private set; }
    public GroupType DepartmentGroupType { get; private set; }
    public GroupType TeamGroupType { get; private set; }
    public GroupTypePosition DivisionHead { get; private set; }
    public GroupTypePosition DivisionMember { get; private set; }
    public GroupTypePosition DepartmentHead { get; private set; }
    public GroupTypePosition DepartmentMember { get; private set; }
    public GroupTypePosition TeamHead { get; private set; }
    public GroupTypePosition TeamMember { get; private set; }
    public Group AustrianDivsion { get; private set; }
    public Group GermanDivision { get; private set; }
    public Group AustrianHumanResourcesDepartment { get; private set; }
    public Group AustrianFinanceDepartment { get; private set; }
    public Group AustrianProjectsDepartment { get; private set; }
    public Group GermanHumanResourcesDepartment { get; private set; }
    public Group AustrianCarTeam { get; private set; }
    public Group AustrianPlaneTeam { get; private set; }
    public Group AustrianAccountingTeam { get; private set; }
    public Group OwningGroup { get; private set; }
    public User CarTeamMember { get; private set; }

    private void Build ()
    {
      CompanyTenant = _testHelper.CreateTenant("Worldwide Corporation", Guid.NewGuid().ToString());

      HeadPosition = _testHelper.CreatePosition("Head");
      MemberPosition = _testHelper.CreatePosition("Member");

      DivisionGroupType = _testHelper.CreateGroupType("Division");
      DivisionHead = _testHelper.CreateGroupTypePosition(DivisionGroupType, HeadPosition);
      DivisionMember = _testHelper.CreateGroupTypePosition(DivisionGroupType, MemberPosition);
      AustrianDivsion = CreateGroup("Austria", null, CompanyTenant, DivisionGroupType);
      GermanDivision = CreateGroup("Germany", null, CompanyTenant, DivisionGroupType);

      DepartmentGroupType = _testHelper.CreateGroupType("Department");
      DepartmentHead = _testHelper.CreateGroupTypePosition(DepartmentGroupType, HeadPosition);
      DepartmentMember = _testHelper.CreateGroupTypePosition(DepartmentGroupType, MemberPosition);
      AustrianHumanResourcesDepartment = CreateGroup("Human Resources (Austria)", AustrianDivsion, CompanyTenant, DepartmentGroupType);
      AustrianFinanceDepartment = CreateGroup("Human Resources (Austria)", AustrianDivsion, CompanyTenant, DepartmentGroupType);
      AustrianProjectsDepartment = CreateGroup("Projects (Austria)", AustrianDivsion, CompanyTenant, DepartmentGroupType);
      GermanHumanResourcesDepartment = CreateGroup("Human Resources (Germany)", GermanDivision, CompanyTenant, DepartmentGroupType);

      TeamGroupType = _testHelper.CreateGroupType("Team");
      TeamHead = _testHelper.CreateGroupTypePosition(TeamGroupType, HeadPosition);
      TeamMember = _testHelper.CreateGroupTypePosition(TeamGroupType, MemberPosition);
      AustrianCarTeam = CreateGroup("Car Developers", AustrianProjectsDepartment, CompanyTenant, TeamGroupType);
      AustrianPlaneTeam = CreateGroup("Plane Developers", AustrianProjectsDepartment, CompanyTenant, TeamGroupType);
      AustrianAccountingTeam = CreateGroup("Accounts", AustrianFinanceDepartment, CompanyTenant, TeamGroupType);

      OwningGroup = CreateGroup("Users", null, CompanyTenant, null);

      CarTeamMember = CreateUser("CarTeamMember", OwningGroup, CompanyTenant);
      _testHelper.CreateRole(CarTeamMember, AustrianCarTeam, MemberPosition);
    }

    private User CreateUser (string userName, Group group, Tenant tenant)
    {
      return _testHelper.CreateUser(userName, "First", "Last", null, group, tenant);
    }

    private Group CreateGroup (string name, Group parentGroup, Tenant tenant, GroupType groupType)
    {
      Group group = _testHelper.CreateGroup(name, parentGroup, tenant);
      group.GroupType = groupType;

      return group;
    }
  }
}
