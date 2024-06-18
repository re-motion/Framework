// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Collections.Generic;
using MixinXRef.Utility;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class ReadonlyIdentifierGeneratorTest
  {
    [Test]
    public void GetIdentifier_NonExistingItem ()
    {
      var identifierGenerator = new IdentifierGenerator<string>();

      var readonlyIdentifierGenerator = new ReadonlyIdentifierGenerator<string>(identifierGenerator, "dummy-value");

      var output = readonlyIdentifierGenerator.GetIdentifier ("key-1");

      Assert.That (output, Is.EqualTo ("dummy-value"));
    }

    [Test]
    public void GetIdentifier_ForExistingItem ()
    {
      var identifierGenerator = new IdentifierGenerator<string>();

      var expectedOutput = identifierGenerator.GetIdentifier("value-1");

      var readonlyIdentifierGenerator = new ReadonlyIdentifierGenerator<string>(identifierGenerator, "does not matter");

      var output = readonlyIdentifierGenerator.GetIdentifier ("value-1");

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void GetIdentifier2_NonExistingItem()
    {
      var identifierGenerator = new IdentifierGenerator<string>();

      var readonlyIdentifierGenerator = new ReadonlyIdentifierGenerator<string>(identifierGenerator, "does not matter EITHER");

      var output = readonlyIdentifierGenerator.GetIdentifier("key-1", "default value");

      Assert.That(output, Is.EqualTo("default value"));
    }

    [Test]
    public void GetIdentifier2_ForExistingItem()
    {
      var identifierGenerator = new IdentifierGenerator<string>();

      var expectedOutput = identifierGenerator.GetIdentifier("value-1");

      var readonlyIdentifierGenerator = new ReadonlyIdentifierGenerator<string>(identifierGenerator, "does not matter");

      var output = readonlyIdentifierGenerator.GetIdentifier("value-1", "does not matter EITHER");

      Assert.That(output, Is.EqualTo(expectedOutput));
    }
  }
}