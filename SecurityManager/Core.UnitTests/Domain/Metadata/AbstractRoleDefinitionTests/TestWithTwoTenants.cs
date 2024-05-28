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
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata.AbstractRoleDefinitionTests
{
  [TestFixture]
  public class TestWithTwoTenants : DomainTest
  {
    private DatabaseFixtures _dbFixtures;

    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();

      _dbFixtures = new DatabaseFixtures();
      _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(ClientTransaction.CreateRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp();

      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
    }

    [Test]
    public void Find_EmptyResult ()
    {
      var result = AbstractRoleDefinition.Find(new EnumWrapper[0]);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void Find_ValidOneAbstractRole ()
    {
      var abstractRoles = new[] { EnumWrapper.Get(ProjectRoles.QualityManager) };
      var result = AbstractRoleDefinition.Find(abstractRoles);

      Assert.That(result.Count, Is.EqualTo(1));
      Assert.That(result[0].Name, Is.EqualTo(abstractRoles[0].Name));
    }

    [Test]
    public void Find_ValidTwoAbstractRoles ()
    {
      var abstractRoles = new[] { EnumWrapper.Get(ProjectRoles.QualityManager), EnumWrapper.Get(ProjectRoles.Developer) };
      var result = AbstractRoleDefinition.Find(abstractRoles);

      Assert.That(result.Count, Is.EqualTo(2));
      Assert.That(result[0].Name, Is.EqualTo(abstractRoles[1].Name));
      Assert.That(result[1].Name, Is.EqualTo(abstractRoles[0].Name));
    }

    [Test]
    public void FindAll_TwoFound ()
    {
      var result = AbstractRoleDefinition.FindAll();

      Assert.That(result.Count, Is.EqualTo(2));
      for (int i = 0; i < result.Count; i++)
      {
        AbstractRoleDefinition abstractRole = result[i];
        Assert.That(abstractRole.Index, Is.EqualTo(i), "Wrong Index.");
      }
    }
  }
}
