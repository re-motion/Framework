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
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryPropertiesEnumerationValueFilterTests
{
  [TestFixture]
  public class NotSupportedProperty : AccessControlEntryPropertiesEnumerationValueFilterTestBase
  {
    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The property 'GroupHierarchyCondition' is not supported by the "
        + "'Remotion.SecurityManager.Domain.AccessControl.AccessControlEntryPropertiesEnumerationValueFilter'.")]
    public void InvalidValue ()
    {
      var ace = CreateAceForStateless();
      var property = GetPropertyDefinition (ace, "GroupHierarchyCondition");
      Filter.IsEnabled (CreateEnumValueInfo ((GroupHierarchyCondition) 1000), ace, property);
    }
  }
}
