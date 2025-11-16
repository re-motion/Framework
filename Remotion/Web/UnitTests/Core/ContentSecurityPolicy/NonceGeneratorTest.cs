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
using NUnit.Framework;
using Remotion.Web.ContentSecurityPolicy;

namespace Remotion.Web.UnitTests.Core.ContentSecurityPolicy
{
  [TestFixture]
  public class NonceGeneratorTest
  {
    private INonceGenerator _nonceGenerator;

    [SetUp]
    public void SetUp ()
    {
      _nonceGenerator = new NonceGenerator();
    }

    [Test]
    public void GenerateAlphaNumericNonce_ReturnsNonNullValue ()
    {
      var nonce = _nonceGenerator.GenerateAlphaNumericNonce();

      Assert.That(nonce, Is.Not.Null);
    }

    [Test]
    public void GenerateAlphaNumericNonce_ReturnsUniqueValues ()
    {
      var nonce1 = _nonceGenerator.GenerateAlphaNumericNonce();
      var nonce2 = _nonceGenerator.GenerateAlphaNumericNonce();

      Assert.That(nonce1, Is.Not.EqualTo(nonce2));
    }
  }
}
