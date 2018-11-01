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
using NUnit.Framework;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.Utilities
{
  [TestFixture]
  public class VirtualPathUtilityTests
  {
    private IControl _controlStub;

    [SetUp]
    public void SetUp ()
    {
      _controlStub = MockRepository.GenerateStub<IControl>();
      _controlStub.AppRelativeTemplateSourceDirectory = "~/base/Path";
    }

    [Test]
    public void GetVirtualPath_RelativePath ()
    {
      Assert.That (VirtualPathUtility.GetVirtualPath (_controlStub, "relative/path/file.txt"), Is.EqualTo ("~/base/Path/relative/path/file.txt"));
    }

    [Test]
    public void GetVirtualPath_VirtualPath ()
    {
      Assert.That (VirtualPathUtility.GetVirtualPath (_controlStub, "~/virtual/path/file.txt"), Is.EqualTo ("~/virtual/path/file.txt"));
    }

    [Test]
    public void GetVirtualPath_FilePath ()
    {
      Assert.That (VirtualPathUtility.GetVirtualPath (_controlStub, "file.txt"), Is.EqualTo ("~/base/Path/file.txt"));
    }

    [Test]
    public void GetVirtualPath_HierchyPath ()
    {
      Assert.That (VirtualPathUtility.GetVirtualPath (_controlStub, "../relative/file.txt"), Is.EqualTo ("~/base/relative/file.txt"));
    }

    [Test]
    public void GetVirtualPath_AppRelativeTemplateSourceDirectory_Empty ()
    {
      var controlStub = new WebButton();
      controlStub.ID = "ControlID";
      controlStub.AppRelativeTemplateSourceDirectory = "";
      Assert.That (
          () => VirtualPathUtility.GetVirtualPath (controlStub, "../relative/file.txt"),
          Throws.InvalidOperationException
              .And.Message.StartsWith ("The 'AppRelativeTemplateSourceDirectory' property of the WebButton 'ControlID' is not set."));
    }

    [Test]
    public void GetVirtualPath_AppRelativeTemplateSourceDirectory_Null ()
    {
      var controlStub = new WebButton();
      controlStub.ID = "ControlID";
      controlStub.AppRelativeTemplateSourceDirectory = null;
      Assert.That (() => VirtualPathUtility.GetVirtualPath (controlStub, "../relative/file.txt"), Throws.InvalidOperationException);
    }
  }
}