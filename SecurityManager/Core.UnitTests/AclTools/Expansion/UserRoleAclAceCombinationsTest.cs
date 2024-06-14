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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class UserRoleAclAceCombinationsTest : AclToolsTestBase
  {
    [Test]
    public void CtorTest ()
    {
      var userFinderMock = new Mock<IAclExpanderUserFinder>();
      var aclFinderMock = new Mock<IAclExpanderAclFinder>();

      var userRoleAclAceCombinations = new UserRoleAclAceCombinationFinder(userFinderMock.Object,aclFinderMock.Object);
      Assert.That(Remotion.Development.UnitTesting.PrivateInvoke.GetNonPublicField(userRoleAclAceCombinations, "_userFinder"), Is.EqualTo(userFinderMock.Object));
      Assert.That(Remotion.Development.UnitTesting.PrivateInvoke.GetNonPublicField(userRoleAclAceCombinations, "_accessControlListFinder"), Is.EqualTo(aclFinderMock.Object));
    }


    [Test]
    public void EnumeratorTest ()
    {
      // Prepare to serve some User|s
      var users = ListObjectMother.New(User, User2, User3);
      var userFinderStub = new Mock<IAclExpanderUserFinder>();
      userFinderStub.Setup(stub => stub.FindUsers()).Returns(users).Verifiable();

      // Prepare to serve some Acl|s
      var acls = ListObjectMother.New<AccessControlList>(Acl, Acl2);
      var aclFinderStub = new Mock<IAclExpanderAclFinder>();
      aclFinderStub.Setup(stub => stub.FindAccessControlLists()).Returns(acls).Verifiable();

      // Assert that our test set is not too small.
      var numberRoles = users.SelectMany(x => x.Roles).Count();
      Assert.That(numberRoles, Is.GreaterThanOrEqualTo(11));
      var numberAces = acls.SelectMany(x => x.AccessControlEntries).Count();
      Assert.That(numberAces, Is.GreaterThanOrEqualTo(5));


      // Assert that the result set is the outer product of the participation sets.
      var userRoleAclAceCombinationsExpected = from user in users
                                               from role in user.Roles
                                               from acl in acls
                                               from ace in acl.AccessControlEntries
                                               select new UserRoleAclAceCombination(role, ace);

      var userRoleAclAceCombinations = new UserRoleAclAceCombinationFinder(userFinderStub.Object, aclFinderStub.Object);
      Assert.That(userRoleAclAceCombinations.ToArray(), Is.EquivalentTo(userRoleAclAceCombinationsExpected.ToArray()));
    }


    [Test]
    public void CompoundValueEqualityComparerTest ()
    {
      var userRoleAclAceCombination = new UserRoleAclAceCombination(Role, Ace);
      Assert.That(UserRoleAclAceCombination.Comparer.GetEqualityParticipatingObjects(userRoleAclAceCombination),
        Is.EqualTo(new object[] { Role, Ace }));
    }

    [Test]
    public void EqualityTest ()
    {
      var userRoleAclAceCombination = new UserRoleAclAceCombination(Role, Ace);
      var userRoleAclAceCombinationSame = new UserRoleAclAceCombination(Role, Ace);
      Assert.That(userRoleAclAceCombination,Is.EqualTo((Object)userRoleAclAceCombination));
      Assert.That(userRoleAclAceCombination, Is.EqualTo((Object)userRoleAclAceCombinationSame));
      Assert.That(userRoleAclAceCombinationSame, Is.EqualTo((Object)userRoleAclAceCombination));
    }

    [Test]
    public void InEqualityTest ()
    {
      var userRoleAclAceCombination = new UserRoleAclAceCombination(Role2, Ace3);
      var userRoleAclAceCombinationDifferent0 = new UserRoleAclAceCombination(Role2, Ace);
      var userRoleAclAceCombinationDifferent1 = new UserRoleAclAceCombination(Role, Ace3);
      Assert.That(userRoleAclAceCombination, Is.Not.EqualTo(userRoleAclAceCombinationDifferent0));
      Assert.That(userRoleAclAceCombination, Is.Not.EqualTo(userRoleAclAceCombinationDifferent1));
      Assert.That(userRoleAclAceCombinationDifferent0, Is.Not.EqualTo(userRoleAclAceCombination));
      Assert.That(userRoleAclAceCombinationDifferent1, Is.Not.EqualTo(userRoleAclAceCombination));
    }

    [Test]
    public void EqualsUserRoleAclAceCombinationTest ()
    {
      var userRoleAclAceCombination = new UserRoleAclAceCombination(Role, Ace);
      var userRoleAclAceCombinationSame = new UserRoleAclAceCombination(Role, Ace);
      Assert.That(userRoleAclAceCombination.Equals(userRoleAclAceCombination), Is.True);
      Assert.That(userRoleAclAceCombination.Equals(userRoleAclAceCombinationSame), Is.True);
      Assert.That(userRoleAclAceCombinationSame.Equals(userRoleAclAceCombination), Is.True);
    }


    [Test]
    public void GetHashCodeTest ()
    {
      Assert.That((new UserRoleAclAceCombination(Role3, Ace)).GetHashCode(), Is.EqualTo((new UserRoleAclAceCombination(Role3, Ace)).GetHashCode()));
      Assert.That((new UserRoleAclAceCombination(Role3, Ace2)).GetHashCode(), Is.EqualTo((new UserRoleAclAceCombination(Role3, Ace2)).GetHashCode()));
      Assert.That((new UserRoleAclAceCombination(Role, Ace3)).GetHashCode(), Is.EqualTo((new UserRoleAclAceCombination(Role, Ace3)).GetHashCode()));
    }

  }
}
