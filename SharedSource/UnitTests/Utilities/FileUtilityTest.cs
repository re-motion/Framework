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
using Remotion.Utilities;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public partial class FileUtilityTest
  {
    private const string c_testFileName = "FileUtilityTest_testfile.txt";

    [SetUp]
    public void SetUp ()
    {
      File.WriteAllText (c_testFileName, "File content");
    }

    [TearDown]
    public void TearDown ()
    {
      if (File.Exists (c_testFileName))
        File.Delete (c_testFileName);
    }

    [Test]
    public void MoveAndWaitForCompletionWithSameFileName ()
    {
      Assert.That (File.Exists (c_testFileName), Is.True);

      FileUtility.MoveAndWaitForCompletion (c_testFileName, c_testFileName);

      Assert.That (File.Exists (c_testFileName), Is.True);
    }

    [Test]
    public void MoveAndWaitForCompletionWithSameFile ()
    {
      Assert.That (File.Exists (c_testFileName), Is.True);
      Assert.That (File.Exists (Path.GetFullPath (c_testFileName)), Is.True);

      FileUtility.MoveAndWaitForCompletion (c_testFileName, Path.GetFullPath (c_testFileName));

      Assert.That (File.Exists (c_testFileName), Is.True);
      Assert.That (File.Exists (Path.GetFullPath (c_testFileName)), Is.True);
    }
  }
}
