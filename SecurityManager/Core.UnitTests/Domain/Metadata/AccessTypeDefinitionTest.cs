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
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class AccessTypeDefinitionTest : DomainTest
  {
    private MetadataTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new MetadataTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void DeleteFailsIfStateDefinitionIsAssociatedWithStateProperty ()
    {
      var accessType = _testHelper.CreateAccessTypeCreate(0);
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.AddAccessType(accessType);

      var messge = "Access type 'Create|Remotion.Security.GeneralAccessTypes, Remotion.Security' "
                   + "cannot be deleted because it is associated with at least one securable class definition.";
      Assert.That(() => accessType.Delete(), Throws.InvalidOperationException.And.Message.EqualTo(messge));
    }
  }
}
