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
using Remotion.Development.Web.ResourceHosting;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Web.ResourceHosting
{
  [TestFixture]
  public class RootMappingVirtualDirectoryTest
  {

    [Test]
    public void Directories_ReturnsMappedPaths ()
    {
      var factoryStub = MockRepository.GenerateStub<Func<string, ResourceVirtualDirectory>>();

      var directory= new RootMappingVirtualDirectory (
          "~/test/",
          new[]
          {
              new ResourcePathMapping ("dir1", "testResourceFolder"),
              new ResourcePathMapping ("dir2", "testResourceFolder"),
          },
          new DirectoryInfo ("c:\\temp"),
          factoryStub);

      var expectedVirtualDirectory1 = new ResourceVirtualDirectory("~/test/dir1/", new DirectoryInfo("c:\\temp\\dir1"));
      var expectedVirtualDirectory2 = new ResourceVirtualDirectory("~/test/dir2/", new DirectoryInfo("c:\\temp\\dir2"));

      factoryStub.Stub (_ => _ ("~/test/dir1/")).Return(expectedVirtualDirectory1);
      factoryStub.Stub (_ => _ ("~/test/dir2/")).Return(expectedVirtualDirectory2);

      Assert.That (directory.Directories, Is.EqualTo (new[] { expectedVirtualDirectory1, expectedVirtualDirectory2 }));
    }
  }
}