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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Security;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupTests
{
  [TestFixture]
  public class Initialization : GroupTestBase
  {
    [Test]
    public void Initializ_SetsUniqueIdentityInsideSecurityFreeSection ()
    {
      var extensionStub = new Mock<IClientTransactionExtension>();
      bool propertyValueChangingCalled = false;
      extensionStub
          .Setup(_ => _.Key)
          .Returns("STUB");
      extensionStub
          .Setup(_ => _.PropertyValueChanging(It.IsAny<ClientTransaction>(), It.IsAny<DomainObject>(), It.IsAny<PropertyDefinition>(), It.IsAny<object>(), It.IsAny<object>()))
          .Callback(
              (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue) =>
              {
                if (propertyDefinition.PropertyInfo.Name == "UniqueIdentifier" && (string)newValue != "id")
                {
                  propertyValueChangingCalled = true;
                  Assert.That(SecurityFreeSection.IsActive, Is.True);
                }
              });

      var tenant = TestHelper.CreateTenant("tenant", "tenantID");
      TestHelper.Transaction.Extensions.Add(extensionStub.Object);
      Assert.That(SecurityFreeSection.IsActive, Is.False);
      TestHelper.CreateGroup("test", "id", null, tenant);
      Assert.That(SecurityFreeSection.IsActive, Is.False);
      Assert.That(propertyValueChangingCalled, Is.True);
    }
  }
}
