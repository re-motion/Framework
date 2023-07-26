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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Security;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.TenantTests
{
  [TestFixture]
  public class Initialization : TenantTestBase
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
          .Setup(_ => _.PropertyValueChanging(It.IsAny<ClientTransaction>(), It.IsAny<IDomainObject>(), It.IsAny<PropertyDefinition>(), It.IsAny<object>(), It.IsAny<object>()))
          .Callback(
              (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue) =>
              {
                if (propertyDefinition.PropertyInfo.Name == "UniqueIdentifier" && (string)newValue != "id")
                {
                  propertyValueChangingCalled = true;
                  Assert.That(SecurityFreeSection.IsActive, Is.True);
                }
              });

      TestHelper.Transaction.Extensions.Add(extensionStub.Object);
      Assert.That(SecurityFreeSection.IsActive, Is.False);
      TestHelper.CreateTenant("test", "id");
      Assert.That(SecurityFreeSection.IsActive, Is.False);
      Assert.That(propertyValueChangingCalled, Is.True);
    }
  }
}
