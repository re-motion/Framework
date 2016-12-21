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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Security;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupTests
{
  [TestFixture]
  public class Initialization : GroupTestBase
  {
    [Test]
    public void Initializ_SetsUniqueIdentityInsideSecurityFreeSection ()
    {
      var extensionStub = MockRepository.GenerateStub<IClientTransactionExtension>();
      bool propertyValueChangingCalled = false;
      extensionStub.Stub (_ => _.Key).Return ("STUB");
      extensionStub.Stub (_ => _.PropertyValueChanging (null, null, null, null, null)).IgnoreArguments().WhenCalled (
          mi =>
          {
            var propertyDefinition = ((PropertyDefinition) mi.Arguments[2]);
            var newValue = ((string) mi.Arguments[4]);
            if (propertyDefinition.PropertyInfo.Name == "UniqueIdentifier" && newValue != "id")
            {
              propertyValueChangingCalled = true;
              Assert.That (SecurityFreeSection.IsActive, Is.True);
            }
          });

      var tenant = TestHelper.CreateTenant ("tenant", "tenantID");
      TestHelper.Transaction.Extensions.Add (extensionStub);
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      TestHelper.CreateGroup ("test", "id", null, tenant);
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      Assert.That (propertyValueChangingCalled, Is.True);
    }
  }
}