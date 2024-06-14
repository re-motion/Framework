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
using Remotion.SecurityManager.AclTools.Expansion;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpanderUserFinderTest : AclToolsTestBase
  {
    [Test]
    public void FindAllUsersTest ()
    {
      var userFinder = new AclExpanderUserFinder();
      var users = userFinder.FindUsers();
      Assert.That(users.Count, Is.EqualTo(7));
    }


    [Test]
    public void FirstNameFilterTest ()
    {
      const string firstName = "test";
      var userFinder = new AclExpanderUserFinder(firstName, null, null);

      var users = userFinder.FindUsers();
      Assert.That(users.Count, Is.EqualTo(1));
      Assert.That(users[0].FirstName, Is.EqualTo(firstName));
    }

    [Test]
    public void LastNameFilterTest ()
    {
      const string lastName = "user2";
      var userFinder = new AclExpanderUserFinder(null, lastName, null);

      var users = userFinder.FindUsers();

      Assert.That(users.Count, Is.EqualTo(2));
      Assert.That(users[0].LastName, Is.EqualTo(lastName));
      Assert.That(users[1].LastName, Is.EqualTo(lastName));
    }

    [Test]
    public void UserNameFilterTest ()
    {
      const string userName = "group0/user1";
      var userFinder = new AclExpanderUserFinder(null, null, userName);

      var users = userFinder.FindUsers();
      Assert.That(users.Count, Is.EqualTo(1));
      Assert.That(users[0].UserName, Is.EqualTo(userName));
    }

    [Test]
    public void AllNamesFilterTest ()
    {
      const string firstName = "User";
      const string lastName = "Tenant 2";
      const string userName = "User.Tenant2";
      var userFinder = new AclExpanderUserFinder(firstName, lastName, userName);

      var users = userFinder.FindUsers();
      Assert.That(users.Count, Is.EqualTo(1));
      Assert.That(users[0].FirstName, Is.EqualTo(firstName));
      Assert.That(users[0].LastName, Is.EqualTo(lastName));
      Assert.That(users[0].UserName, Is.EqualTo(userName));
    }
  }
}
