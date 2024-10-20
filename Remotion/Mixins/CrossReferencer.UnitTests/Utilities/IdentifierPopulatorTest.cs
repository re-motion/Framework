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
using Remotion.Mixins.CrossReferencer.Utilities;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Utilities
{
  [TestFixture]
  public class IdentifierPopulatorTest
  {
    [Test]
    public void GetReadonlyIdentifierGenerator ()
    {
      var testStrings = new[] { "test-value-1", "test-value-2" };

      var identifierGenerator = new IdentifierGenerator<string>();
      identifierGenerator.GetIdentifier(testStrings[0]);
      identifierGenerator.GetIdentifier(testStrings[1]);

      var expectedOutput = identifierGenerator.GetReadonlyIdentiferGenerator("default-value");

      var output = new IdentifierPopulator<string>(testStrings).GetReadonlyIdentifierGenerator("default-value");

      Assert.That(output.GetIdentifier(testStrings[0]), Is.EqualTo(expectedOutput.GetIdentifier(testStrings[0])));
      Assert.That(output.GetIdentifier(testStrings[1]), Is.EqualTo(expectedOutput.GetIdentifier(testStrings[1])));
      Assert.That(output.GetIdentifier("not-present!"), Is.EqualTo(expectedOutput.GetIdentifier("not-present!")));
    }
  }
}
