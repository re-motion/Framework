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
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.UnitTests.Core.Validation
{
  [TestFixture]
  public class ValidationResultTest
  {
    private TargetClassDefinition _parentDefinition;
    private MixinDefinition _nestedDefinition;
    private MethodDefinition _nestedNestedDefinition;

    [SetUp]
    public void SetUp ()
    {
      _parentDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (object));
      _nestedDefinition = DefinitionObjectMother.CreateMixinDefinition (_parentDefinition, typeof (string));
      _nestedNestedDefinition = DefinitionObjectMother.CreateMethodDefinition (_nestedDefinition, ReflectionObjectMother.GetSomeMethod());
    }

    [Test]
    public void GetDefinitionContextPath_NonNested ()
    {
      var validationResult = new ValidationResult (_parentDefinition);
      Assert.That (validationResult.GetDefinitionContextPath(), Is.EqualTo (""));
    }

    [Test]
    public void GetDefinitionContextPath_NestedOnce ()
    {
      var validationResult = new ValidationResult (_nestedDefinition);

      Assert.That (validationResult.GetDefinitionContextPath(), Is.EqualTo ("System.Object"));
    }

    [Test]
    public void GetDefinitionContextPath_NestedTwice ()
    {
      var validationResult = new ValidationResult (_nestedNestedDefinition);

      Assert.That (validationResult.GetDefinitionContextPath (), Is.EqualTo ("System.String -> System.Object"));
    }
  }
}