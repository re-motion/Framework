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
using System.Windows.Forms;
using NUnit.Framework;
using Remotion.ObjectBinding.Design.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Design.BindableObject
{
  [TestFixture]
  public class SearchFieldControllerTest
  {
    private TextBox _textBox;
    private Button _button;

    [SetUp]
    public void SetUp ()
    {
      _textBox = new TextBox();
      _button = new Button();
    }

    [TearDown]
    public void TearDown ()
    {
      _textBox.Dispose();
      _button.Dispose();
    }

    [Test]
    public void GetButtonIcon ()
    {
      Assert.That (_button.ImageList, Is.Null);

      SearchFieldController controller = new SearchFieldController (_textBox, _button);

      Assert.That (_button.ImageList, Is.Not.Null);
      Assert.That (_button.ImageList.Images.Count, Is.EqualTo (1));
      Assert.That (_button.ImageKey, Is.EqualTo (SearchFieldController.SearchIcons.Search.ToString()));
    }
  }
}
