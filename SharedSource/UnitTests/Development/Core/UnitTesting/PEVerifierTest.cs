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
using System.IO;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.PEVerifyPathSources;
using Rhino.Mocks;

// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class PEVerifierTest
  {
    [Test]
    public void GetVerifierPath ()
    {
      var pathSourceStub = MockRepository.GenerateStub<IPEVerifyPathSource> ();
      pathSourceStub.Stub (stub => stub.GetPEVerifyPath (PEVerifyVersion.DotNet4)).Return ("test");
      var verifier = new PEVerifier (pathSourceStub);

      var path = verifier.GetVerifierPath (PEVerifyVersion.DotNet4);

      Assert.That (path, Is.EqualTo ("test"));
    }

    [Test]
    [ExpectedException (typeof (PEVerifyException), ExpectedMessage = "PEVerify for version 'DotNet4' could not be found. Locations searched:\r\nx")]
    public void GetVerifierPath_NotFound ()
    {
      var pathSourceStub = MockRepository.GenerateStub<IPEVerifyPathSource> ();
      pathSourceStub.Stub (stub => stub.GetPEVerifyPath (PEVerifyVersion.DotNet4)).Return (null);
      pathSourceStub.Stub (stub => stub.GetLookupDiagnostics (PEVerifyVersion.DotNet4)).Return ("x");
      var verifier = new PEVerifier (pathSourceStub);

      verifier.GetVerifierPath (PEVerifyVersion.DotNet4);
    }

    [Test]
    public void CreateDefault_FindsVerifier ()
    {
      var verifier = PEVerifier.CreateDefault ();

      var path = verifier.GetVerifierPath (PEVerifyVersion.DotNet2);

      Assert.That (path, Is.Not.Null);
      Assert.That (File.Exists (path), Is.True);
    }
    
    [Test]
    public void VerifyPEFile_MSCorlib ()
    {
      var verifier = PEVerifier.CreateDefault ();
      verifier.VerifyPEFile (typeof (object).Assembly);
    }

    [Test]
    [ExpectedException (typeof (PEVerifyException), ExpectedMessage = "PEVerify returned 1.", MatchType = MessageMatch.Contains)]
    public void VerifyPEFile_InvalidPath ()
    {
      var verifier = PEVerifier.CreateDefault ();
      verifier.VerifyPEFile ("Foobar whatever");
    }
  }
}
