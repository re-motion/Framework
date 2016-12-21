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
using System.Linq;
using System.Web.Hosting;
using NUnit.Framework;
using Remotion.Development.Web.ResourceHosting;

namespace Remotion.Development.UnitTests.Web.ResourceHosting
{
  [TestFixture]
  public class ResourceVirtualDirectoryTest
  {
    private string _testDirectory;

    [SetUp]
    public void SetUp ()
    {
      _testDirectory = Path.Combine (Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory (_testDirectory);
    }

    [TearDown]
    public void TearDown ()
    {
      Directory.Delete (_testDirectory, true);
    }

    [Test]
    public void Directories ()
    {
      var expectedDirectory1 = Directory.CreateDirectory (Path.Combine (_testDirectory, "subDirectory1"));
      var expectedDirectory2 = Directory.CreateDirectory (Path.Combine (_testDirectory, "subDirectory2"));

      var resourceVirtualDirectory = new ResourceVirtualDirectory ("~/res/test/", new DirectoryInfo (_testDirectory));

      var actual = resourceVirtualDirectory.Directories.Cast<ResourceVirtualDirectory>();

      Assert.That (
          actual.Select (d => d.PhysicalPath),
          Is.EquivalentTo (
              new[]
              {
                  expectedDirectory1.FullName,
                  expectedDirectory2.FullName
              }));
    }

    [Test]
    public void Files ()
    {
      var expectedFile1 = Path.Combine (_testDirectory, "expetedFile1.txt");
      var expectedFile2 = Path.Combine (_testDirectory, "expetedFile2.txt");
      File.WriteAllText (expectedFile1, "hello");
      File.WriteAllText (expectedFile2, "hello");

      var resourceVirtualDirectory = new ResourceVirtualDirectory ("~/res/test/", new DirectoryInfo (_testDirectory));

      var actual = resourceVirtualDirectory.Files.Cast<ResourceVirtualFile>();

      Assert.That (
          actual.Select (d => d.PhysicalPath),
          Is.EquivalentTo (
              new[]
              {
                  expectedFile1,
                  expectedFile2
              }));
    }

    [Test]
    public void Children ()
    {
      var expectedDirectory1 = Directory.CreateDirectory (Path.Combine (_testDirectory, "subDirectory1"));
      var expectedFile1 = Path.Combine (_testDirectory, "expetedFile1.txt");
      File.WriteAllText (expectedFile1, "hello");
      
      var resourceVirtualDirectory = new ResourceVirtualDirectory ("~/res/test/", new DirectoryInfo (_testDirectory));

      var actual = resourceVirtualDirectory.Children.Cast<VirtualFileBase>().ToArray();
      Assert.That (actual.Count(), Is.EqualTo (2));

      Assert.That (actual.OfType<ResourceVirtualFile>().Single().PhysicalPath, Is.EqualTo (expectedFile1));
      Assert.That (actual.OfType<ResourceVirtualDirectory>().Single().PhysicalPath, Is.EqualTo (expectedDirectory1.FullName));
    }
  }
}