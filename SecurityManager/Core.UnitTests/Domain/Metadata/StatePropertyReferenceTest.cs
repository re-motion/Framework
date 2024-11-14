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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class StatePropertyReferenceTest : DomainTest
  {
    private MetadataTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new MetadataTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void OnCommitting_WithChangedStatePropertyReference_RegistersClassForCommit ()
    {
      var classDefinition = SecurableClassDefinition.NewObject();
      var stateProperty = _testHelper.CreateFileStateProperty(0);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That(GetDataContainer((DomainObject)sender).HasBeenMarkedChanged, Is.True);
        };
        classDefinition.AddStateProperty(stateProperty);

        ClientTransaction.Current.Commit();

        Assert.That(commitOnClassWasCalled, Is.True);
      }
    }

    [Test]
    public void OnCommitting_WithDeletedStatePropertyReference_RegistersClassForCommit ()
    {
      var classDefinition = SecurableClassDefinition.NewObject();
      var stateProperty = _testHelper.CreateFileStateProperty(0);
      classDefinition.AddStateProperty(stateProperty);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That(GetDataContainer((DomainObject)sender).HasBeenMarkedChanged, Is.True);
        };
        classDefinition.RemoveStateProperty(stateProperty);

        ClientTransaction.Current.Commit();

        Assert.That(commitOnClassWasCalled, Is.True);
      }
    }
  }
}
