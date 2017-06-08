// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
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

      var path = verifier.GetVerifierPath (PEVerifyVersion.DotNet4);

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
