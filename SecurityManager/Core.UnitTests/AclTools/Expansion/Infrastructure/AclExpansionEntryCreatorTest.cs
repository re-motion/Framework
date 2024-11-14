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
using Moq;
using NUnit.Framework;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.Infrastructure
{
  [TestFixture]
  public class AclExpansionEntryCreatorTest : AclToolsTestBase
  {
    [Test]
    public void GetAccessTypesTest ()
    {
      var ace = TestHelper.CreateAceWithNoMatchingRestrictions();
      AttachAccessTypeReadWriteDelete(ace, true, false, true);
      Assert.That(ace.Validate().IsValid);
      TestHelper.CreateStatefulAcl(ace);

      var aclExpansionEntryCreator = new AclExpansionEntryCreator();
      //AclProbe aclProbe;
      //AccessTypeStatistics accessTypeStatistics;
      var accessTypesResult =
        aclExpansionEntryCreator.GetAccessTypes(new UserRoleAclAceCombination(Role2, ace)); //, out aclProbe, out accessTypeStatistics);

      Assert.That(accessTypesResult.AccessInformation.AllowedAccessTypes, Is.EquivalentTo(new[] { ReadAccessType, DeleteAccessType }));
      Assert.That(accessTypesResult.AccessInformation.DeniedAccessTypes, Is.EquivalentTo(new[] { WriteAccessType }));
    }

    [Test]
    public void GetAccessTypesDoesNotModifyUserRoles ()
    {
      var aclExpansionEntryCreator = new AclExpansionEntryCreator();
      //AclProbe aclProbe;
      //AccessTypeStatistics accessTypeStatistics;
      User.Roles.Add(Role2);
      var userRoleAclAceCombination = new UserRoleAclAceCombination(Role, Ace);
      var accessTypesResult = aclExpansionEntryCreator.GetAccessTypes(userRoleAclAceCombination); //, out aclProbe, out accessTypeStatistics);
      Assert.That(User.Roles, Is.EquivalentTo(new[] { Role, Role2 }));
      Assert.That(accessTypesResult.AclProbe.SecurityToken.Principal.Roles, Is.EquivalentTo(new[] { Role }).Using(PrincipalRoleComparer.Instance));
    }


    [Test]
    public void CreateAclExpansionEntryTest ()
    {
      var userRoleAclAce = new UserRoleAclAceCombination(Role, Ace);
      var allowedAccessTypes = new[] { WriteAccessType, DeleteAccessType };
      var deniedAccessTypes = new[] { ReadAccessType };
      AccessInformation accessInformation = new AccessInformation(allowedAccessTypes, deniedAccessTypes);

      var aclExpansionEntryCreatorMock = new Mock<AclExpansionEntryCreator>() { CallBase = true };
      var aclProbe = AclProbe.CreateAclProbe(User, Role, Ace);
      var accessTypeStatisticsMock = new Mock<AccessTypeStatistics>(MockBehavior.Strict);
      accessTypeStatisticsMock.Setup(x => x.IsInAccessTypesContributingAces(userRoleAclAce.Ace)).Returns(true).Verifiable();

      aclExpansionEntryCreatorMock
          .Setup(x => x.GetAccessTypes(userRoleAclAce))
          .Returns(new AclExpansionEntryCreator_GetAccessTypesResult(accessInformation, aclProbe, accessTypeStatisticsMock.Object))
          .Verifiable();

      var aclExpansionEntry = aclExpansionEntryCreatorMock.Object.CreateAclExpansionEntry(userRoleAclAce);

      aclExpansionEntryCreatorMock.Verify();
      accessTypeStatisticsMock.Verify();

      Assert.That(aclExpansionEntry.User, Is.EqualTo(userRoleAclAce.User));
      Assert.That(aclExpansionEntry.Role, Is.EqualTo(userRoleAclAce.Role));
      Assert.That(aclExpansionEntry.AccessControlList, Is.EqualTo(userRoleAclAce.Acl));
      Assert.That(aclExpansionEntry.AccessConditions, Is.EqualTo(aclProbe.AccessConditions));
      Assert.That(aclExpansionEntry.AllowedAccessTypes, Is.EqualTo(allowedAccessTypes));
      Assert.That(aclExpansionEntry.DeniedAccessTypes, Is.EqualTo(deniedAccessTypes));
    }





  }
}
