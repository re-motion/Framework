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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessControlEntryValidationResultTest
  {
    [Test]
    public void IsValid_Valid ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      Assert.That(result.IsValid, Is.True);
      Assert.That(result.GetErrors(), Is.Empty);
    }

    [Test]
    public void IsValid_NotValid ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      result.SetError(AccessControlEntryValidationError.IsSpecificTenantMissing);

      Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void SetError ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      result.SetError(AccessControlEntryValidationError.IsSpecificTenantMissing);

      Assert.That(result.GetErrors(), Is.EquivalentTo(new[] { AccessControlEntryValidationError.IsSpecificTenantMissing }));
    }

    [Test]
    public void SetError_SameTwice ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      result.SetError(AccessControlEntryValidationError.IsSpecificTenantMissing);
      result.SetError(AccessControlEntryValidationError.IsSpecificTenantMissing);

      Assert.That(result.GetErrors(), Is.EquivalentTo(new[] { AccessControlEntryValidationError.IsSpecificTenantMissing }));
    }

    [Test]
    public void SetError_DifferentValues ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      result.SetError(AccessControlEntryValidationError.IsSpecificTenantMissing);
      result.SetError(AccessControlEntryValidationError.IsSpecificGroupMissing);

      Assert.That(
          result.GetErrors(),
          Is.EquivalentTo(
              new[] { AccessControlEntryValidationError.IsSpecificTenantMissing, AccessControlEntryValidationError.IsSpecificGroupMissing }));
    }

    [Test]
    public void GetErrors ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      result.SetError(AccessControlEntryValidationError.IsSpecificGroupMissing);
      result.SetError(AccessControlEntryValidationError.IsSpecificTenantMissing);

      Assert.That(
          result.GetErrors(),
          Is.EqualTo(new[] { AccessControlEntryValidationError.IsSpecificTenantMissing, AccessControlEntryValidationError.IsSpecificGroupMissing }));
    }

    [Test]
    public void GetErrorMessage ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      result.SetError(AccessControlEntryValidationError.IsSpecificGroupMissing);
      result.SetError(AccessControlEntryValidationError.IsSpecificTenantMissing);

      using (new CultureScope(""))
      {
        Assert.That(
            result.GetErrorMessage(),
            Is.EqualTo(
                "The access control entry is in an invalid state:\r\n"
                + "  The TenantCondition property is set to SpecificTenant, but no SpecificTenant is assigned.\r\n"
                + "  The GroupCondition property is set to SpecificGroup, but no SpecificGroup is assigned."));
      }
    }
  }
}
