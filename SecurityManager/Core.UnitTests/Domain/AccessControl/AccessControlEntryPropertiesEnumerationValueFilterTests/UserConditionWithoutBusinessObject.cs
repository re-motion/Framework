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
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryPropertiesEnumerationValueFilterTests
{
  [TestFixture]
  public class UserConditionWithoutBusinessObject : AccessControlEntryPropertiesEnumerationValueFilterTestBase
  {
    private IBusinessObjectEnumerationProperty _property;
    private IBusinessObject _businessObjectNull;

    public override void SetUp ()
    {
      base.SetUp();
      _businessObjectNull = null;
      _property = GetPropertyDefinition(CreateAceForStateless(), "UserCondition");
    }

    [Test]
    public void None ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(UserCondition.None), _businessObjectNull, _property), Is.True);
    }

    [Test]
    public void None_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(UserCondition.None), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void Owner ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(UserCondition.Owner), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void Owner_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(UserCondition.Owner), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void SpecificUser ()
    {
      Assert.That(SafeServiceLocator.Current.GetInstance<IAccessControlSettings>().DisableSpecificUser, Is.False);
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(UserCondition.SpecificUser), _businessObjectNull, _property), Is.True);
    }

    [Test]
    public void SpecificUser_Disabled ()
    {
      Assert.That(SafeServiceLocator.Current.GetInstance<IAccessControlSettings>().DisableSpecificUser, Is.False);
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(UserCondition.SpecificUser), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void SpecificUser_DisabledFromConfiguration ()
    {
      var filter = new AccessControlEntryPropertiesEnumerationValueFilter(AccessControlSettings.Create(disableSpecificUser: true));
      Assert.That(filter.IsEnabled(CreateEnumValueInfo(UserCondition.SpecificUser), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void SpecificPosition ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(UserCondition.SpecificPosition), _businessObjectNull, _property), Is.True);
    }

    [Test]
    public void SpecificPosition_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(UserCondition.SpecificPosition), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void InvalidValue ()
    {
      Assert.That(
          () => Filter.IsEnabled(CreateEnumValueInfo((UserCondition)1000), _businessObjectNull, _property),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The value '1000' is not a valid value for 'UserCondition'."));
    }
  }
}
