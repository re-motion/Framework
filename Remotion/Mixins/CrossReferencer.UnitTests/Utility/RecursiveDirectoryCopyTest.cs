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

namespace Remotion.Mixins.CrossReferencer.UnitTests.Utility
{
  [TestFixture]
  public class RecursiveDirectoryCopyTest
  {
    private const string sourceDirectoryPath = "sourceDirectory";
    private const string destinationDirectoryPath = "destinationDirectory";

    [TearDown]
    public void TearDown ()
    {
      // clean up when test fails
      if (Directory.Exists(sourceDirectoryPath))
        Directory.Delete(sourceDirectoryPath, true);
      if (Directory.Exists(destinationDirectoryPath))
        Directory.Delete(destinationDirectoryPath, true);
    }

    [Test]
    public void CopyTo_SourceDirectoryDoesNotExist ()
    {
      var sourceDirectory = new DirectoryInfo("nonExistingDirectory");
      try
      {
        sourceDirectory.CopyTo("doesNotMatter");
        Assert.Fail("expected exception was not thrown");
      }
      catch (DirectoryNotFoundException ex)
      {
        Assert.That(ex.Message, Is.EqualTo("source directory '" + sourceDirectory.FullName + "' not found "));
      }
    }

    [Test]
    public void CopyTo_DestinationDirectoryDoesNotExist ()
    {
      var subDirectoryPath = "sub1" + Path.DirectorySeparatorChar + "sub2";
      var testFilePath = Path.Combine(subDirectoryPath, "testFile.txt");

      var sourceDirectory = new DirectoryInfo(sourceDirectoryPath);
      sourceDirectory.Create();
      sourceDirectory.CreateSubdirectory(subDirectoryPath);
      File.Create(Path.Combine(sourceDirectoryPath, testFilePath)).Close();

      Assert.That(Directory.Exists(destinationDirectoryPath), Is.False);
      sourceDirectory.CopyTo(destinationDirectoryPath);
      Assert.That(File.Exists(Path.Combine(destinationDirectoryPath, testFilePath)), Is.True);

      Directory.Delete(sourceDirectoryPath, true);
      Directory.Delete(destinationDirectoryPath, true);
    }

    [Test]
    public void CopyTo_DestinationDirectoryDoesExist ()
    {
      var subDirectoryPath = "sub1" + Path.DirectorySeparatorChar + "sub2";
      var testFilePath = Path.Combine(subDirectoryPath, "testFile.txt");
      var sourceFilePath = Path.Combine(sourceDirectoryPath, testFilePath);
      var destinationFilePath = Path.Combine(destinationDirectoryPath, testFilePath);

      var sourceDirectory = new DirectoryInfo(sourceDirectoryPath);
      sourceDirectory.Create();
      sourceDirectory.CreateSubdirectory(subDirectoryPath);
      File.Create(sourceFilePath).Close();

      // call copyTo a second time, wanted behavior: silently overwrite directories and files
      Assert.That(Directory.Exists(destinationDirectoryPath), Is.False);
      sourceDirectory.CopyTo(destinationDirectoryPath);
      var lastWriteTime1 = File.GetLastWriteTime(destinationFilePath);

      // last write time of the copied file is 'inherited' from source file, so we have to update it
      System.Threading.Thread.Sleep(10);
      File.Create(sourceFilePath).Close();

      Assert.That(Directory.Exists(destinationDirectoryPath), Is.True);
      sourceDirectory.CopyTo(destinationDirectoryPath);
      var lastWriteTime2 = File.GetLastWriteTime(destinationFilePath);

      // creation time does not change, when file is overwritten -> use last write time
      Assert.That(lastWriteTime1, Is.LessThan(lastWriteTime2));

      Directory.Delete(sourceDirectoryPath, true);
      Directory.Delete(destinationDirectoryPath, true);
    }
  }
}
