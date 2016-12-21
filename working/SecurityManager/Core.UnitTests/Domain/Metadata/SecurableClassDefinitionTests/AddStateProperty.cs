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

      securableClassDefinition.AddStateProperty (stateProperty);

      Assert.That (securableClassDefinition.StateProperties, Is.EqualTo (new[] { stateProperty }));
    }

    [Test]
    public void TouchesSecurableClassDefinition ()
    {
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        securableClassDefinition.EnsureDataAvailable();
        Assert.That (securableClassDefinition.State, Is.EqualTo (StateType.Unchanged));

        securableClassDefinition.AddStateProperty (StatePropertyDefinition.NewObject());

        Assert.That (securableClassDefinition.State, Is.EqualTo (StateType.Changed));
      }
    }

    [Test]
    public void FailsForExistingStateProperty ()
    {
      var stateProperty = StatePropertyDefinition.NewObject (Guid.NewGuid(), "Test");

      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.Name = "Class";
      securableClassDefinition.AddStateProperty (stateProperty);
      Assert.That (
          () => securableClassDefinition.AddStateProperty (stateProperty),
          Throws.ArgumentException
              .And.Message.StartsWith ("The property 'Test' has already been added to the securable class definition."));
    }
  }
}