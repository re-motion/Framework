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
using Remotion.SecurityManager.Domain;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class RevisionTest : DomainTest
  {
    [Test]
    public void GetRevision ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateEmptyDomain();

      Assert.That(ClientTransaction.CreateRootTransaction().QueryManager.GetScalar(Revision.GetGetRevisionQuery(new RevisionKey())), Is.Null);
    }

    [Test]
    public void IncrementRevision ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateEmptyDomain();

      var oldValue = ClientTransaction.CreateRootTransaction().QueryManager.GetScalar(Revision.GetGetRevisionQuery(new RevisionKey()));

      ClientTransaction.CreateRootTransaction().QueryManager.GetScalar(Revision.GetIncrementRevisionQuery(new RevisionKey()));

      var newValue = ClientTransaction.CreateRootTransaction().QueryManager.GetScalar(Revision.GetGetRevisionQuery(new RevisionKey()));
      Assert.That(oldValue, Is.Not.EqualTo(newValue));
    }
  }
}
