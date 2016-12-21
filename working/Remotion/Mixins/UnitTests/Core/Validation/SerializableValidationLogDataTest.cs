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
using Remotion.Development.UnitTesting;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.UnitTests.Core.Validation
{
  [TestFixture]
  public class SerializableValidationLogDataTest
  {
    [Test]
    public void Ctor_CopiesPropertiesFromValidationLogData ()
    {
      var log = new DefaultValidationLog();
      var rule = new DelegateValidationRule<TargetClassDefinition> (DummyRule);
      var definition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (object));
      log.ValidationStartsFor (definition);
      log.Succeed (rule);
      log.ValidationEndsFor (definition);

      var expected = log.GetData();
      var actual = expected.MakeSerializable();

      Assert.That (actual.NumberOfFailures, Is.EqualTo (expected.GetNumberOfFailures()));
      Assert.That (actual.NumberOfRulesExecuted, Is.EqualTo (expected.GetNumberOfRulesExecuted()));
      Assert.That (actual.NumberOfSuccesses, Is.EqualTo (expected.GetNumberOfSuccesses()));
      Assert.That (actual.NumberOfUnexpectedExceptions, Is.EqualTo (expected.GetNumberOfUnexpectedExceptions()));
      Assert.That (actual.NumberOfWarnings, Is.EqualTo (expected.GetNumberOfWarnings()));
    }

    [Test]
    public void Serializable ()
    {
      var log = new DefaultValidationLog();
      var rule = new DelegateValidationRule<TargetClassDefinition> (DummyRule);
      var definition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (object));
      log.ValidationStartsFor (definition);
      log.Succeed (rule);
      log.ValidationEndsFor (definition);

      var serializableValidationLogData = log.GetData().MakeSerializable();

      var deserialized = Serializer.SerializeAndDeserialize (serializableValidationLogData);
      Assert.That (deserialized.NumberOfFailures, Is.EqualTo (serializableValidationLogData.NumberOfFailures));
      Assert.That (deserialized.NumberOfRulesExecuted, Is.EqualTo (serializableValidationLogData.NumberOfRulesExecuted));
      Assert.That (deserialized.NumberOfSuccesses, Is.EqualTo (serializableValidationLogData.NumberOfSuccesses));
      Assert.That (deserialized.NumberOfUnexpectedExceptions, Is.EqualTo (serializableValidationLogData.NumberOfUnexpectedExceptions));
      Assert.That (deserialized.NumberOfWarnings, Is.EqualTo (serializableValidationLogData.NumberOfWarnings));
    }

    private void DummyRule (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
      throw new NotImplementedException();
    }
  }
}