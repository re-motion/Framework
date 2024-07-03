// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.IO;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.PEVerifyPathSources;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class PEVerifierTest
  {
    [Test]
    public void GetVerifierPath ()
    {
      var pathSourceStub = new Mock<IPEVerifyPathSource>();
      pathSourceStub.Setup(stub => stub.GetPEVerifyPath(PEVerifyVersion.DotNet4)).Returns("test");
      var verifier = new PEVerifier(pathSourceStub.Object);

      var path = verifier.GetVerifierPath(PEVerifyVersion.DotNet4);

      Assert.That(path, Is.EqualTo("test"));
    }

    [Test]
    public void GetVerifierPath_NotFound ()
    {
      var pathSourceStub = new Mock<IPEVerifyPathSource>();
      pathSourceStub.Setup(stub => stub.GetPEVerifyPath(PEVerifyVersion.DotNet4)).Returns((string?)null);
      pathSourceStub.Setup(stub => stub.GetLookupDiagnostics(PEVerifyVersion.DotNet4)).Returns("x");
      var verifier = new PEVerifier(pathSourceStub.Object);
      Assert.That(
          () => verifier.GetVerifierPath(PEVerifyVersion.DotNet4),
          Throws.InstanceOf<PEVerifyException>()
              .With.Message.EqualTo(
                  "PEVerify for version 'DotNet4' could not be found. Locations searched:\r\nx"));
    }

    [Test]
    public void CreateDefault_FindsVerifier ()
    {
      var verifier = PEVerifier.CreateDefault();

      var path = verifier.GetVerifierPath(PEVerifyVersion.DotNet4);

      Assert.That(path, Is.Not.Null);
      Assert.That(File.Exists(path), Is.True);
    }

    [Test]
    public void VerifyPEFile_MSCorlib ()
    {
#if !NETFRAMEWORK
      Assert.Ignore("PEVerify is not supported for .NET 5 assemblies.");
#endif
      var verifier = PEVerifier.CreateDefault();
      verifier.VerifyPEFile(typeof(object).Assembly);
    }

    [Test]
    public void VerifyPEFile_InvalidPath ()
    {
      var verifier = PEVerifier.CreateDefault();
      Assert.That(
          () => verifier.VerifyPEFile("Foobar whatever"),
          Throws.InstanceOf<PEVerifyException>()
              .With.Message.Contains("PEVerify returned 1."));
    }
  }
}
