// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.IO;
using NUnit.Framework;
using Remotion.Utilities;

#nullable enable
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
      File.WriteAllText(c_testFileName, "File content");
    }

    [TearDown]
    public void TearDown ()
    {
      if (File.Exists(c_testFileName))
        File.Delete(c_testFileName);
    }

    [Test]
    public void MoveAndWaitForCompletionWithSameFileName ()
    {
      Assert.That(File.Exists(c_testFileName), Is.True);

      FileUtility.MoveAndWaitForCompletion(c_testFileName, c_testFileName);

      Assert.That(File.Exists(c_testFileName), Is.True);
    }

    [Test]
    public void MoveAndWaitForCompletionWithSameFile ()
    {
      Assert.That(File.Exists(c_testFileName), Is.True);
      Assert.That(File.Exists(Path.GetFullPath(c_testFileName)), Is.True);

      FileUtility.MoveAndWaitForCompletion(c_testFileName, Path.GetFullPath(c_testFileName));

      Assert.That(File.Exists(c_testFileName), Is.True);
      Assert.That(File.Exists(Path.GetFullPath(c_testFileName)), Is.True);
    }
  }
}
