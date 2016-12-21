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
  public class ValidationExceptionTest
  {
    [Test]
    public void Serialization ()
    {
      var log = new DefaultValidationLog();
      var rule = new DelegateValidationRule<TargetClassDefinition> (DummyRule);

      var definition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (object));
      log.ValidationStartsFor (definition);
      log.Succeed (rule);
      log.ValidationEndsFor (definition);

      var exception = new ValidationException (log.GetData());

      var deserializedException = Serializer.SerializeAndDeserialize (exception);
      Assert.That (deserializedException.Message, Is.EqualTo (exception.Message));
      Assert.That (deserializedException.ValidationLogData, Is.Not.Null);
    }

    private void DummyRule (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
      throw new NotImplementedException();
    }
  }
}