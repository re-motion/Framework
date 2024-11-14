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
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryPropertiesEnumerationValueFilterTests
{
  [TestFixture]
  public class TenantConditionWithoutBusinessObject : AccessControlEntryPropertiesEnumerationValueFilterTestBase
  {
    private IBusinessObjectEnumerationProperty _property;
    private IBusinessObject _businessObjectNull;

    public override void SetUp ()
    {
      base.SetUp();
      _businessObjectNull = null;
      _property = GetPropertyDefinition(CreateAceForStateless(), "TenantCondition");
    }

    [Test]
    public void None ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(TenantCondition.None), _businessObjectNull, _property), Is.True);
    }

    [Test]
    public void None_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(TenantCondition.None), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void OwningTenant ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(TenantCondition.OwningTenant), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void OwningTenant_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(TenantCondition.OwningTenant), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void SpecificTenant ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(TenantCondition.SpecificTenant), _businessObjectNull, _property), Is.True);
    }

    [Test]
    public void SpecificTenant_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(TenantCondition.SpecificTenant), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void InvalidValue ()
    {
      Assert.That(
          () => Filter.IsEnabled(CreateEnumValueInfo((TenantCondition)1000), _businessObjectNull, _property),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The value '1000' is not a valid value for 'TenantCondition'."));
    }
  }
}
