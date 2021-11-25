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
  public class UserConditionStateful : AccessControlEntryPropertiesEnumerationValueFilterTestBase
  {
    private IBusinessObjectEnumerationProperty _property;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp();
      _ace = CreateAceForStateful();
      _property = GetPropertyDefinition(_ace, "UserCondition");
    }

    [Test]
    public void None ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(UserCondition.None), _ace, _property), Is.True);
    }

    [Test]
    public void None_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(UserCondition.None), _ace, _property), Is.False);
    }

    [Test]
    public void Owner ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(UserCondition.Owner), _ace, _property), Is.True);
    }

    [Test]
    public void Owner_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(UserCondition.Owner), _ace, _property), Is.False);
    }

    [Test]
    public void SpecificUser ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(UserCondition.SpecificUser), _ace, _property), Is.True);
    }

    [Test]
    public void SpecificUser_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(UserCondition.SpecificUser), _ace, _property), Is.False);
    }

    [Test]
    public void SpecificPosition ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(UserCondition.SpecificPosition), _ace, _property), Is.True);
    }

    [Test]
    public void SpecificPosition_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(UserCondition.SpecificPosition), _ace, _property), Is.False);
    }

    [Test]
    public void InvalidValue ()
    {
      Assert.That(
          () => Filter.IsEnabled(CreateEnumValueInfo((UserCondition)1000), _ace, _property),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The value '1000' is not a valid value for 'UserCondition'."));
    }
  }
}
