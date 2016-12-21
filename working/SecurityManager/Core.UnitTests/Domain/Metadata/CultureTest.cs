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
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class CultureTest : DomainTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
    
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateAndCommitSecurableClassDefinitionWithLocalizedNames (ClientTransaction.CreateRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void Find_Existing ()
    {
      Culture foundCulture = Culture.Find ("de");

      Assert.That (foundCulture, Is.Not.Null);
      Assert.That (foundCulture.State, Is.Not.EqualTo (StateType.New));
      Assert.That (foundCulture.CultureName, Is.EqualTo ("de"));
    }

    [Test]
    public void Find_NotExisting ()
    {
      Culture foundCulture = Culture.Find ("hu");

      Assert.That (foundCulture, Is.Null);
    }
  }
}
