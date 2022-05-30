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
  public class GroupConditionWithoutBusinessObject : AccessControlEntryPropertiesEnumerationValueFilterTestBase
  {
    private IBusinessObjectEnumerationProperty _property;
    private IBusinessObject _businessObjectNull;

    public override void SetUp ()
    {
      base.SetUp();
      _businessObjectNull = null;
      _property = GetPropertyDefinition(CreateAceForStateless(), "GroupCondition");
    }

    [Test]
    public void None ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(GroupCondition.None), _businessObjectNull, _property), Is.True);
    }

    [Test]
    public void None_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(GroupCondition.None), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void OwningGroup ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(GroupCondition.OwningGroup), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void OwningGroup_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(GroupCondition.OwningGroup), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void SpecificGroup ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(GroupCondition.SpecificGroup), _businessObjectNull, _property), Is.True);
    }

    [Test]
    public void SpecificGroup_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(GroupCondition.SpecificGroup), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void AnyGroupWithSpecificGroupType ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(GroupCondition.AnyGroupWithSpecificGroupType), _businessObjectNull, _property), Is.True);
    }

    [Test]
    public void AnyGroupWithSpecificGroupType_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(GroupCondition.AnyGroupWithSpecificGroupType), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void BranchOfOwningGroup ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo(GroupCondition.BranchOfOwningGroup), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void BranchOfOwningGroup_Disabled ()
    {
      Assert.That(Filter.IsEnabled(CreateEnumValueInfo_Disabled(GroupCondition.BranchOfOwningGroup), _businessObjectNull, _property), Is.False);
    }

    [Test]
    public void InvalidValue ()
    {
      Assert.That(
          () => Filter.IsEnabled(CreateEnumValueInfo((GroupCondition)1000), _businessObjectNull, _property),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The value '1000' is not a valid value for 'GroupCondition'."));
    }
  }
}
