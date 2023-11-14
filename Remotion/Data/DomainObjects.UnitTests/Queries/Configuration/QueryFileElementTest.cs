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
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.Configuration
{
  [TestFixture]
  public class QueryFileElementTest
  {
    [Test]
    public void GetRootedPath_WithFullPath_ReturnsFullPath ()
    {
      string path = @"c:\foo\bar.txt";
      Assert.That(QueryFileElement.GetRootedPath(path), Is.EqualTo(path));
    }

    [Test]
    public void GetRootedPath_WithRootedRelativePath_ReturnsFullPath ()
    {
      string path = @"\foo\bar.txt";
      string fullPath = Path.GetFullPath(path);
      Assert.That(QueryFileElement.GetRootedPath(path), Is.EqualTo(fullPath));
    }

    [Test]
    public void GetRootedPath_WithUnrootedPath_ReturnsPathRelativeToAppBase ()
    {
      string path = @"foo\bar.txt";
      string fullPath = Path.Combine(AppContext.BaseDirectory, @"foo\bar.txt");
      Assert.That(QueryFileElement.GetRootedPath(path), Is.EqualTo(fullPath));
    }

    [Test]
    public void GetRootedPath_WithUnrootedPath_ReturnsPathRelativeToAppBase_InSeparateAddDomain ()
    {
#if NETFRAMEWORK
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = @"c:\";
      setup.DynamicBase = Path.GetTempPath();

      new AppDomainRunner(setup, delegate
      {
        string path = @"foo\bar.txt";
        string fullPath = Path.Combine(AppContext.BaseDirectory, @"foo\bar.txt");
        Assert.That(QueryFileElement.GetRootedPath(path), Is.EqualTo(fullPath));
      }).Run();
#else
      using (new ChangedApplicationBaseDirectorySection(@"c:\"))
      {
        string path = @"foo\bar.txt";
        string fullPath = Path.Combine(AppContext.BaseDirectory, @"foo\bar.txt");
        Assert.That(QueryFileElement.GetRootedPath(path), Is.EqualTo(fullPath));
      }
#endif
    }
  }
}
