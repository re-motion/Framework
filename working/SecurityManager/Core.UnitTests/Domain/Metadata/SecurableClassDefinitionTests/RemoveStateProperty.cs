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
  public class RemoveStateProperty : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp();

      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    [Test]
    public void RemovesStateProperty ()
    {
      var stateProperty0 = StatePropertyDefinition.NewObject();
      var stateProperty1 = StatePropertyDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();

      securableClassDefinition.AddStateProperty (stateProperty0);
      securableClassDefinition.AddStateProperty (stateProperty1);

      securableClassDefinition.RemoveStateProperty (stateProperty0);

      Assert.That (securableClassDefinition.StateProperties, Is.EqualTo (new[] { stateProperty1 }));
    }

    [Test]
    public void RemoveStateProperty_DeletesAssociatedStateCombinations ()
    {
      var state1 = StateDefinition.NewObject ("Test", 1);
      var state2 = StateDefinition.NewObject();
      var stateProperty = StatePropertyDefinition.NewObject();
      stateProperty.AddState (state1);

      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.AddStateProperty (stateProperty);
      var acl = securableClassDefinition.CreateStatefulAccessControlList();
      acl.StateCombinations[0].AttachState (state1);
      acl.CreateStateCombination().AttachState (state2);

      securableClassDefinition.RemoveStateProperty (stateProperty);

      Assert.That (acl.StateCombinations.Count, Is.EqualTo (1));
      Assert.That (acl.StateCombinations[0].GetStates(), Is.EqualTo (new[] { state2 }));
    }

    [Test]
    public void RemoveStateProperty_DeletesAssociatedAccessControlListIfDeletedStateCombinationWasLastStateCombination ()
    {
      var state1 = StateDefinition.NewObject ("Test", 1);
      var state2 = StateDefinition.NewObject();
      var stateProperty = StatePropertyDefinition.NewObject();
      stateProperty.AddState (state1);

      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.AddStateProperty (stateProperty);
      var acl1 = securableClassDefinition.CreateStatefulAccessControlList();
      acl1.StateCombinations[0].AttachState (state1);

      var acl2 = securableClassDefinition.CreateStatefulAccessControlList();
      Assert.That (acl2.StateCombinations, Is.Not.Empty);

      var acl3 = securableClassDefinition.CreateStatefulAccessControlList();
      acl3.StateCombinations[0].Delete();

      var acl4 = securableClassDefinition.CreateStatefulAccessControlList();
      acl4.StateCombinations[0].AttachState (state2);

      securableClassDefinition.RemoveStateProperty (stateProperty);

      Assert.That (acl1.State, Is.EqualTo (StateType.Invalid));

      Assert.That (acl2.State, Is.EqualTo (StateType.New));
      Assert.That (acl2.StateCombinations.Count, Is.EqualTo (1));
      Assert.That (acl2.StateCombinations[0].GetStates(), Is.Empty);

      Assert.That (acl3.State, Is.EqualTo (StateType.New));
      Assert.That (acl3.StateCombinations, Is.Empty);

      Assert.That (acl4.State, Is.EqualTo (StateType.New));
      Assert.That (acl4.StateCombinations.Count, Is.EqualTo (1));
      Assert.That (acl4.StateCombinations[0].GetStates(), Is.EqualTo (new[] { state2 }));
    }

    [Test]
    public void TouchesSecurableClassDefinition ()
    {
      var stateProperty = StatePropertyDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.AddStateProperty (stateProperty);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        securableClassDefinition.EnsureDataAvailable();
        Assert.That (securableClassDefinition.State, Is.EqualTo (StateType.Unchanged));

        securableClassDefinition.RemoveStateProperty (stateProperty);

        Assert.That (securableClassDefinition.State, Is.EqualTo (StateType.Changed));
      }
    }

    [Test]
    public void FailsForNonExistentStateProperty ()
    {
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.Name = "Class";
      securableClassDefinition.AddStateProperty (StatePropertyDefinition.NewObject());
      securableClassDefinition.AddStateProperty (StatePropertyDefinition.NewObject());
      Assert.That (
          () => securableClassDefinition.RemoveStateProperty (StatePropertyDefinition.NewObject (Guid.NewGuid(), "Test")),
          Throws.ArgumentException
              .And.Message.StartsWith ("The property 'Test' does not exist on the securable class definition."));
    }
  }
}