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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Development.RhinoMocks.UnitTesting;
using Rhino.Mocks;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Development.RhinoMocks.UnitTesting
{
  [TestFixture]
  public class ListArgExtensionsTest
  {
    [Test]
    public void Equivalent ()
    {
      var myMock = MockRepository.GenerateMock<IMyInterface>();

      var equal = false;
      var equivalent = false;
      var different = false;

      myMock.Expect (mock => mock.SomeMethod (Arg<IEnumerable<string>>.List.Equivalent ("a", "b", "c")))
            .WhenCalled (mi => equal = true);
      myMock.Expect (mock => mock.SomeMethod (Arg<IEnumerable<string>>.List.Equivalent ("d", "e", "f")))
            .WhenCalled (mi => equivalent = true);
      myMock.Expect (mock => mock.SomeMethod (Arg<IEnumerable<string>>.List.Equivalent ("g", "h", "i")))
            .WhenCalled (mi => different = true);

      myMock.SomeMethod (new[] { "a", "b", "c" });
      myMock.SomeMethod (new List<string> { "f", "e", "d" });
      myMock.SomeMethod (new[] { "g", "h", "j" });

      Assert.That (equal, Is.True);
      Assert.That (equivalent, Is.True);
      Assert.That (different, Is.False);
      Assert.That (() => myMock.VerifyAllExpectations(), Throws.Exception);
    }
  }

  public interface IMyInterface
  {
    void SomeMethod (IEnumerable<string> parameters);
  }
}