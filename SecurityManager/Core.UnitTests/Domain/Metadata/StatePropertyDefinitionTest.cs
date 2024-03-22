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
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class StatePropertyDefinitionTest : DomainTest
  {
    private MetadataTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new MetadataTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void GetState_ValidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty(0);

      StateDefinition actualState = stateProperty.GetState(MetadataTestHelper.Confidentiality_ConfidentialName);

      StateDefinition expectedState = _testHelper.CreateConfidentialState();
      MetadataObjectAssert.AreEqual(expectedState, actualState, "Confidential state");
    }

    [Test]
    public void GetState_InvalidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty(0);

      Assert.That(
          () => stateProperty.GetState("New"),
          Throws.ArgumentException.And.Message.StartsWith("The state 'New' is not defined for the property 'Confidentiality'."));
    }

    [Test]
    public void ContainsState_ValidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty(0);
      Assert.That(stateProperty.ContainsState(MetadataTestHelper.Confidentiality_ConfidentialName), Is.True);
    }

    [Test]
    public void ContainsState_InvalidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty(0);
      Assert.That(stateProperty.ContainsState("New"), Is.False);
    }

    [Test]
    public void GetState_ValidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty(0);

      StateDefinition actualState = stateProperty.GetState(MetadataTestHelper.Confidentiality_PrivateValue);

      StateDefinition expectedState = _testHelper.CreatePrivateState();
      MetadataObjectAssert.AreEqual(expectedState, actualState, "Private state");
    }

    [Test]
    public void GetState_InvalidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty(0);

      Assert.That(
          () => stateProperty.GetState(42),
          Throws.ArgumentException.And.Message.StartsWith("A state with the value 42 is not defined for the property 'Confidentiality'."));
    }

    [Test]
    public void ContainsState_ValidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty(0);
      Assert.That(stateProperty.ContainsState(MetadataTestHelper.Confidentiality_PrivateValue), Is.True);
    }

    [Test]
    public void ContainsState_InvalidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty(0);
      Assert.That(stateProperty.ContainsState(42), Is.False);
    }

    [Test]
    public void Indexer_ValidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty(0);

      StateDefinition actualState = stateProperty[MetadataTestHelper.Confidentiality_ConfidentialName];

      StateDefinition expectedState = _testHelper.CreateConfidentialState();
      MetadataObjectAssert.AreEqual(expectedState, actualState, "Confidential state");
    }

    [Test]
    public void GetDefinedStates ()
    {
      var state1 = _testHelper.CreateState("State 1", 1);
      var state2 = _testHelper.CreateState("State 2", 2);
      var state3 = _testHelper.CreateState("State 3", 3);
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty("NewProperty");
      stateProperty.AddState(state1);
      stateProperty.AddState(state3);
      stateProperty.AddState(state2);

      Assert.That(stateProperty.DefinedStates, Is.EqualTo(new[] { state1, state2, state3 }));
    }

    [Test]
    public void AddState ()
    {
      var state1 = _testHelper.CreateState("State 1", 1);
      var state2 = _testHelper.CreateState("State 2", 2);
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty("NewProperty");
      stateProperty.AddState(state1);
      stateProperty.AddState(state2);

      Assert.That(stateProperty.DefinedStates, Is.EqualTo(new[] { state1, state2 }));
    }

    [Test]
    public void AddState_DuplicateName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty("NewProperty");
      stateProperty.AddState(_testHelper.CreateState("State 1", 1));

      Assert.That(
          () => stateProperty.AddState(_testHelper.CreateState("State 1", 2)),
          Throws.ArgumentException.And.Message.StartsWith("A state with the name 'State 1' was already added to the property 'NewProperty'."));
    }

    [Test]
    public void AddState_DuplicateValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty("NewProperty");
      stateProperty.AddState(_testHelper.CreateState("State 1", 1));

      Assert.That(
          () => stateProperty.AddState(_testHelper.CreateState("State 2", 1)),
          Throws.ArgumentException.And.Message.StartsWith("A state with the value 1 was already added to the property 'NewProperty'."));
    }

    [Test]
    public void RemoveState ()
    {
      var state1 = _testHelper.CreateState("State 1", 1);
      var state2 = _testHelper.CreateState("State 2", 2);
      var state3 = _testHelper.CreateState("State 3", 3);
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty("NewProperty");
      stateProperty.AddState(state1);
      stateProperty.AddState(state2);
      stateProperty.AddState(state3);

      stateProperty.RemoveState(state2);

      Assert.That(stateProperty.DefinedStates, Is.EqualTo(new[] { state1, state3 }));
    }

    [Test]
    public void RemoveState_DeletesAssociatedStateCombinations ()
    {
      var state1 = _testHelper.CreateState("State 1", 1);
      var state2 = _testHelper.CreateState("State 2", 2);
      var state3 = _testHelper.CreateState("State 3", 3);
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty("NewProperty");
      stateProperty.AddState(state1);
      stateProperty.AddState(state2);
      stateProperty.AddState(state3);

      var securableClassDefinition1 = SecurableClassDefinition.NewObject();
      securableClassDefinition1.AddStateProperty(stateProperty);
      var acl1 = securableClassDefinition1.CreateStatefulAccessControlList();
      acl1.StateCombinations[0].AttachState(state1);
      acl1.CreateStateCombination().AttachState(state2);

      var securableClassDefinition2 = SecurableClassDefinition.NewObject();
      securableClassDefinition2.AddStateProperty(stateProperty);

      var acl2 = securableClassDefinition2.CreateStatefulAccessControlList();
      acl2.StateCombinations[0].AttachState(state2);
      acl2.CreateStateCombination().AttachState(state3);

      stateProperty.RemoveState(state2);

      Assert.That(acl1.StateCombinations.Count, Is.EqualTo(1));
      Assert.That(acl1.StateCombinations[0].GetStates(), Is.EqualTo(new[] { state1 }));

      Assert.That(acl2.StateCombinations.Count, Is.EqualTo(1));
      Assert.That(acl2.StateCombinations[0].GetStates(), Is.EqualTo(new[] { state3 }));
    }

    [Test]
    public void RemoveState_DeletesAssociatedAccessControlListIfDeletedStateCombinationWasLastStateCombination ()
    {
      var state1 = _testHelper.CreateState("State 1", 1);
      var state2 = _testHelper.CreateState("State 2", 2);
      var state3 = _testHelper.CreateState("State 3", 3);
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty("NewProperty");
      stateProperty.AddState(state1);
      stateProperty.AddState(state2);
      stateProperty.AddState(state3);

      var securableClassDefinition1 = SecurableClassDefinition.NewObject();
      securableClassDefinition1.AddStateProperty(stateProperty);
      var acl1 = securableClassDefinition1.CreateStatefulAccessControlList();
      acl1.StateCombinations[0].AttachState(state2);

      var acl2 = securableClassDefinition1.CreateStatefulAccessControlList();
      Assert.That(acl2.StateCombinations, Is.Not.Empty);

      var acl3 = securableClassDefinition1.CreateStatefulAccessControlList();
      acl3.StateCombinations[0].Delete();

      stateProperty.RemoveState(state2);

      Assert.That(acl1.State.IsInvalid, Is.True);

      Assert.That(acl2.State.IsNew, Is.True);
      Assert.That(acl2.StateCombinations.Count, Is.EqualTo(1));
      Assert.That(acl2.StateCombinations[0].GetStates(), Is.Empty);

      Assert.That(acl3.State.IsNew, Is.True);
      Assert.That(acl3.StateCombinations, Is.Empty);
    }

    [Test]
    public void RemoveState_StateNotFound ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty("NewProperty");
      stateProperty.AddState(_testHelper.CreateState("State 1", 1));

      Assert.That(
          () => stateProperty.RemoveState(_testHelper.CreateState("State 2", 2)),
          Throws.ArgumentException.And.Message.StartsWith("The state 'State 2' does not exist on the property 'NewProperty'."));
    }

    [Test]
    public void DeleteFailsIfStatePropertyDefinitionIsAssociatedWithSecurableClassDefinition ()
    {
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      var property =  _testHelper.CreateNewStateProperty("NewProperty");
      securableClassDefinition.AddStateProperty(property);

      var messge = "State property 'NewProperty' cannot be deleted because it is associated with at least one securable class definition.";
      Assert.That(() => property.Delete(), Throws.InvalidOperationException.And.Message.EqualTo(messge));
    }
  }
}
