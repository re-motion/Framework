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
using System.Reflection;
using MixinXRef.Utility;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Utility
{
  [TestFixture]
  public class IdentifierGeneratorTest
  {
    private IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;

    [SetUp]
    public void SetUp ()
    {
      _assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();
    }

    [Test]
    public void GetIdentifier ()
    {
      var identifier = _assemblyIdentifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);

      Assert.That (identifier, Is.EqualTo ("0"));
    }

    [Test]
    public void GetIdentifier_Twice ()
    {
      _assemblyIdentifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);
      var identifier = _assemblyIdentifierGenerator.GetIdentifier (typeof (object).Assembly);

      Assert.That (identifier, Is.EqualTo ("1"));
    }

    [Test]
    public void GetIdentifier_TwiceOnSameAssembly ()
    {
      var identifier1 = _assemblyIdentifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);
      var identifier2 = _assemblyIdentifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);

      Assert.That (identifier1, Is.EqualTo (identifier2));
    }

    [Test]
    public void GetIdentifier2_ForExistingValue ()
    {
      var identifierGenerator = new IdentifierGenerator<string>();

      var expectedOuput = identifierGenerator.GetIdentifier ("test-value");

      var output = identifierGenerator.GetIdentifier ("test-value", "does not matter");

      Assert.That (output, Is.EqualTo (expectedOuput));
    }

    [Test]
    public void GetIdentifier2_ForNonExistingValue ()
    {
      var identifierGenerator = new IdentifierGenerator<string>();

      var output = identifierGenerator.GetIdentifier ("test-value", "default value if not present");

      Assert.That (output, Is.EqualTo ("default value if not present"));
    }
  }
}