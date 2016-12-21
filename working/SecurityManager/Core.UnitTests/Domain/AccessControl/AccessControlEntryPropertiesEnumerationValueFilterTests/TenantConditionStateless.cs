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
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryPropertiesEnumerationValueFilterTests
{
  [TestFixture]
  public class TenantConditionStateless : AccessControlEntryPropertiesEnumerationValueFilterTestBase
  {
    private IBusinessObjectEnumerationProperty _property;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp();
      _ace = CreateAceForStateless();
      _property = GetPropertyDefinition (_ace, "TenantCondition");
    }

    [Test]
    public void None ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo (TenantCondition.None), _ace, _property), Is.True);
    }

    [Test]
    public void None_Disabled ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo_Disabled (TenantCondition.None), _ace, _property), Is.False);
    }

    [Test]
    public void OwningTenant ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo (TenantCondition.OwningTenant), _ace, _property), Is.False);
    }

    [Test]
    public void OwningTenant_Disabled ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo_Disabled (TenantCondition.OwningTenant), _ace, _property), Is.False);
    }

    [Test]
    public void SpecificTenant ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo (TenantCondition.SpecificTenant), _ace, _property), Is.True);
    }

    [Test]
    public void SpecificTenant_Disabled ()
    {
      Assert.That (Filter.IsEnabled (CreateEnumValueInfo_Disabled (TenantCondition.SpecificTenant), _ace, _property), Is.False);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The value '1000' is not a valid value for 'TenantCondition'.")]
    public void InvalidValue ()
    {
      Filter.IsEnabled (CreateEnumValueInfo ((TenantCondition) 1000), _ace, _property);
    }
  }
}
