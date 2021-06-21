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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Design;

namespace Remotion.ObjectBinding.UnitTests.Design
{
  [TestFixture]
  public class DropDownEditorBaseTest
  {
    private Mock<ITypeDescriptorContext> _mockTypeDescriptorContext;
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IWindowsFormsEditorService> _mockWindowsFormsEditorService;
    private Mock<MockDropDownEditorBase> _mockDropDownEditorBase;
    private Mock<EditorControlBase> _mockEditorControlBase;

    [SetUp]
    public void SetUp ()
    {
      _mockTypeDescriptorContext = new Mock<ITypeDescriptorContext> (MockBehavior.Strict);
      _mockServiceProvider = new Mock<IServiceProvider> (MockBehavior.Strict);
      _mockWindowsFormsEditorService = new Mock<IWindowsFormsEditorService> (MockBehavior.Strict);
      _mockDropDownEditorBase = new Mock<MockDropDownEditorBase>() { CallBase = true };
      _mockEditorControlBase = new Mock<EditorControlBase> (_mockServiceProvider.Object, _mockWindowsFormsEditorService.Object) { CallBase = true };
    }

    [Test]
    public void EditValue ()
    {
      _mockTypeDescriptorContext.Setup (_ => _.Instance).Returns (new object()).Verifiable();
      _mockServiceProvider.Setup (_ => _.GetService (typeof (IWindowsFormsEditorService))).Returns (_mockWindowsFormsEditorService.Object).Verifiable();
      _mockDropDownEditorBase.Setup (_ => _.NewCreateEditorControl (_mockTypeDescriptorContext.Object, _mockWindowsFormsEditorService.Object))
          .Returns (_mockEditorControlBase.Object)
          .Verifiable();

      _mockEditorControlBase.SetupProperty (_ => _.Value);
      _mockWindowsFormsEditorService
          .Setup (_ => _.DropDownControl (_mockEditorControlBase.Object))
          .Callback (
              (Control control) =>
              {
                Assert.That (_mockEditorControlBase.Object.Value, Is.EqualTo ("The input value"));
                _mockEditorControlBase.Object.Value = "The output value";
              })
          .Verifiable();

      object actual = _mockDropDownEditorBase.Object.EditValue (_mockTypeDescriptorContext.Object, _mockServiceProvider.Object, "The input value");

      _mockTypeDescriptorContext.Verify();
      _mockServiceProvider.Verify();
      _mockWindowsFormsEditorService.Verify();
      _mockDropDownEditorBase.Verify();
      _mockEditorControlBase.Verify();

      Assert.That (actual, Is.EqualTo ("The output value"));
    }

    [Test]
    public void EditValue_WithoutTypeDescriptorContextInstance ()
    {
      _mockTypeDescriptorContext.Setup (_ => _.Instance).Returns ((object) null).Verifiable();

      object actual = _mockDropDownEditorBase.Object.EditValue (_mockTypeDescriptorContext.Object, _mockServiceProvider.Object, "The input value");

      _mockTypeDescriptorContext.Verify();
      _mockServiceProvider.Verify();
      _mockWindowsFormsEditorService.Verify();
      _mockDropDownEditorBase.Verify();
      _mockEditorControlBase.Verify();

      Assert.That (actual, Is.EqualTo ("The input value"));
    }

    [Test]
    public void EditValue_WithoutTypeDescriptorContext ()
    {
      object actual = _mockDropDownEditorBase.Object.EditValue (null, _mockServiceProvider.Object, "The input value");

      _mockTypeDescriptorContext.Verify();
      _mockServiceProvider.Verify();
      _mockWindowsFormsEditorService.Verify();
      _mockDropDownEditorBase.Verify();
      _mockEditorControlBase.Verify();

      Assert.That (actual, Is.EqualTo ("The input value"));
    }

    [Test]
    public void EditValue_WithoutServiceProvider ()
    {
      _mockTypeDescriptorContext.Setup (_ => _.Instance).Returns (new object()).Verifiable();

      object actual = _mockDropDownEditorBase.Object.EditValue (_mockTypeDescriptorContext.Object, null, "The input value");

      _mockTypeDescriptorContext.Verify();
      _mockServiceProvider.Verify();
      _mockWindowsFormsEditorService.Verify();
      _mockDropDownEditorBase.Verify();
      _mockEditorControlBase.Verify();

      Assert.That (actual, Is.EqualTo ("The input value"));
    }

    [Test]
    public void EditValue_WitoutEditorSerivce ()
    {
      _mockTypeDescriptorContext.Setup (_ => _.Instance).Returns (new object()).Verifiable();
      _mockServiceProvider.Setup (_ => _.GetService (typeof (IWindowsFormsEditorService))).Returns ((object) null).Verifiable();

      object actual = _mockDropDownEditorBase.Object.EditValue (_mockTypeDescriptorContext.Object, _mockServiceProvider.Object, "The input value");

      _mockTypeDescriptorContext.Verify();
      _mockServiceProvider.Verify();
      _mockWindowsFormsEditorService.Verify();
      _mockDropDownEditorBase.Verify();
      _mockEditorControlBase.Verify();

      Assert.That (actual, Is.EqualTo ("The input value"));
    }

    [Test]
    public void GetEditStyle ()
    {
      _mockTypeDescriptorContext.Setup (_ => _.Instance).Returns (new object ()).Verifiable();

      UITypeEditorEditStyle actual = _mockDropDownEditorBase.Object.GetEditStyle (_mockTypeDescriptorContext.Object);

      _mockTypeDescriptorContext.Verify();
      _mockServiceProvider.Verify();
      _mockWindowsFormsEditorService.Verify();
      _mockDropDownEditorBase.Verify();
      _mockEditorControlBase.Verify();

      Assert.That (actual, Is.EqualTo (UITypeEditorEditStyle.DropDown));
    }

    [Test]
    public void GetEditStyle_WithoutTypeDescriptorContextInstance ()
    {
      _mockTypeDescriptorContext.Setup (_ => _.Instance).Returns ((object) null).Verifiable();

      UITypeEditorEditStyle actual = _mockDropDownEditorBase.Object.GetEditStyle (_mockTypeDescriptorContext.Object);

      _mockTypeDescriptorContext.Verify();
      _mockServiceProvider.Verify();
      _mockWindowsFormsEditorService.Verify();
      _mockDropDownEditorBase.Verify();
      _mockEditorControlBase.Verify();

      Assert.That (actual, Is.EqualTo (UITypeEditorEditStyle.None));
    }

    [Test]
    public void GetEditStyle_WithoutTypeDescriptorContext ()
    {
      UITypeEditorEditStyle actual = _mockDropDownEditorBase.Object.GetEditStyle (null);

      _mockTypeDescriptorContext.Verify();
      _mockServiceProvider.Verify();
      _mockWindowsFormsEditorService.Verify();
      _mockDropDownEditorBase.Verify();
      _mockEditorControlBase.Verify();

      Assert.That (actual, Is.EqualTo (UITypeEditorEditStyle.None));
    }
  }
}
