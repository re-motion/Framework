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
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Security.Development;

namespace Remotion.Security.UnitTests.Development
{
  [TestFixture]
  public class FakeSecurityProviderTest
  {
    [Test]
    public void IsNull_WithDefaultBehavior_ReturnsTrue ()
    {
      var fakeSecurityProvider = new FakeSecurityProvider();

      Assert.That(fakeSecurityProvider.IsNull, Is.True);
    }

    [Test]
    public void IsNull_WithOverriddenBehavior_ReturnsValueFromOverride ()
    {
      var fakeSecurityProvider = new FakeSecurityProvider();

      var expectedResult = BooleanObjectMother.GetRandomBoolean();

      var securityProviderStub = new Mock<ISecurityProvider>();
      securityProviderStub.Setup(_ => _.IsNull).Returns(expectedResult);
      fakeSecurityProvider.SetCustomSecurityProvider(securityProviderStub.Object);

      Assert.That(fakeSecurityProvider.IsNull, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetAccess_WithDefaultBehavior_ReturnsEmptyArray ()
    {
      var fakeSecurityProvider = new FakeSecurityProvider();

      var contextStub = Mock.Of<ISecurityContext>();
      var principalStub = Mock.Of<ISecurityPrincipal>();

      Assert.That(fakeSecurityProvider.GetAccess(contextStub, principalStub), Is.Empty);
    }

    [Test]
    public void GetAccess_WithOverriddenBehavior_ReturnsValueFromOverride ()
    {
      var fakeSecurityProvider = new FakeSecurityProvider();

      var contextStub = Mock.Of<ISecurityContext>();
      var principalStub = Mock.Of<ISecurityPrincipal>();

      var expectedResult = new AccessType[2];

      var securityProviderStub = new Mock<ISecurityProvider>();
      securityProviderStub.Setup(_ => _.GetAccess(contextStub, principalStub)).Returns(expectedResult);
      fakeSecurityProvider.SetCustomSecurityProvider(securityProviderStub.Object);

      Assert.That(fakeSecurityProvider.GetAccess(contextStub, principalStub), Is.SameAs(expectedResult));
    }

    [Test]
    public void ResetCustomSecurityProvider ()
    {
      var fakeSecurityProvider = new FakeSecurityProvider();

      var contextStub = Mock.Of<ISecurityContext>();
      var principalStub = Mock.Of<ISecurityPrincipal>();

      var securityProviderStub = new Mock<ISecurityProvider>();
      securityProviderStub.Setup(_ => _.IsNull).Returns(false);
      securityProviderStub.Setup(_ => _.GetAccess(contextStub, principalStub)).Returns(new AccessType[2]);
      fakeSecurityProvider.SetCustomSecurityProvider(securityProviderStub.Object);

      Assert.That(fakeSecurityProvider.IsNull, Is.False);
      Assert.That(fakeSecurityProvider.GetAccess(contextStub, principalStub), Is.Not.Empty);

      securityProviderStub.Setup(_ => _.IsNull).Throws(new AssertionException("Should not be called."));
      securityProviderStub.Setup(_ => _.GetAccess(contextStub, principalStub)).Throws(new AssertionException("Should not be called."));

      fakeSecurityProvider.ResetCustomSecurityProvider();

      Assert.That(fakeSecurityProvider.IsNull, Is.True);
      Assert.That(fakeSecurityProvider.GetAccess(contextStub, principalStub), Is.Empty);
    }
  }
}
