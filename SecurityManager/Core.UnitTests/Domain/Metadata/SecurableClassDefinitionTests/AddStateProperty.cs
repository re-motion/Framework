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

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata.SecurableClassDefinitionTests
{
  [TestFixture]
  public class AddStateProperty : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp();

      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    [Test]
    public void AddsStateProperty ()
    {
      var stateProperty = StatePropertyDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      securableClassDefinition.AddStateProperty(stateProperty);

      Assert.That(securableClassDefinition.StateProperties, Is.EqualTo(new[] { stateProperty }));
    }

    [Test]
    public void TouchesSecurableClassDefinition ()
    {
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        securableClassDefinition.EnsureDataAvailable();
        Assert.That(securableClassDefinition.State.IsUnchanged, Is.True);

        securableClassDefinition.AddStateProperty(StatePropertyDefinition.NewObject());

        Assert.That(securableClassDefinition.State.IsChanged, Is.True);
      }
    }

    [Test]
    public void FailsForExistingStateProperty ()
    {
      var stateProperty = StatePropertyDefinition.NewObject(Guid.NewGuid(), "Test");

      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.Name = "Class";
      securableClassDefinition.AddStateProperty(stateProperty);
      Assert.That(
          () => securableClassDefinition.AddStateProperty(stateProperty),
          Throws.ArgumentException
              .And.Message.StartsWith("The property 'Test' has already been added to the securable class definition."));
    }
  }
}
