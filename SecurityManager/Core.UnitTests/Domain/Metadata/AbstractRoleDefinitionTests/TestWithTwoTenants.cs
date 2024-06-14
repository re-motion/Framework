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
