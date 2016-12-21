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
using System.Windows.Forms.Design;
using NUnit.Framework;
using Remotion.ObjectBinding.Design;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Design
{
  [TestFixture]
  public class DropDownEditorBaseTest
  {
    private MockRepository _mockRepository;
    private ITypeDescriptorContext _mockTypeDescriptorContext;
    private IServiceProvider _mockServiceProvider;
    private IWindowsFormsEditorService _mockWindowsFormsEditorService;
    private MockDropDownEditorBase _mockDropDownEditorBase;
    private EditorControlBase _mockEditorControlBase;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _mockTypeDescriptorContext = _mockRepository.StrictMock<ITypeDescriptorContext>();
      _mockServiceProvider = _mockRepository.StrictMock<IServiceProvider>();
      _mockWindowsFormsEditorService = _mockRepository.StrictMock<IWindowsFormsEditorService>();
      _mockDropDownEditorBase = _mockRepository.PartialMock<MockDropDownEditorBase>();
      _mockEditorControlBase = _mockRepository.PartialMock<EditorControlBase> (_mockServiceProvider, _mockWindowsFormsEditorService);
    }

    [Test]
    public void EditValue ()
    {
      Expect.Call (_mockTypeDescriptorContext.Instance).Return (new object());
      Expect.Call (_mockServiceProvider.GetService (typeof (IWindowsFormsEditorService))).Return (_mockWindowsFormsEditorService);
      Expect.Call (_mockDropDownEditorBase.NewCreateEditorControl (_mockTypeDescriptorContext, _mockWindowsFormsEditorService))
          .Return (_mockEditorControlBase);
      using (_mockRepository.Ordered())
      {
        _mockEditorControlBase.Value = "The input value";
        _mockWindowsFormsEditorService.DropDownControl (_mockEditorControlBase);
        Expect.Call (_mockEditorControlBase.Value).Return ("The output value");
      }
      _mockRepository.ReplayAll();

      object actual = _mockDropDownEditorBase.EditValue (_mockTypeDescriptorContext, _mockServiceProvider, "The input value");

      _mockRepository.VerifyAll();

      Assert.That (actual, Is.EqualTo ("The output value"));
    }

    [Test]
    public void EditValue_WithoutTypeDescriptorContextInstance ()
    {
      Expect.Call (_mockTypeDescriptorContext.Instance).Return (null);
      _mockRepository.ReplayAll();

      object actual = _mockDropDownEditorBase.EditValue (_mockTypeDescriptorContext, _mockServiceProvider, "The input value");

      _mockRepository.VerifyAll();

      Assert.That (actual, Is.EqualTo ("The input value"));
    }

    [Test]
    public void EditValue_WithoutTypeDescriptorContext ()
    {
      _mockRepository.ReplayAll();

      object actual = _mockDropDownEditorBase.EditValue (null, _mockServiceProvider, "The input value");

      _mockRepository.VerifyAll();

      Assert.That (actual, Is.EqualTo ("The input value"));
    }

    [Test]
    public void EditValue_WithoutServiceProvider ()
    {
      Expect.Call (_mockTypeDescriptorContext.Instance).Return (new object());
      _mockRepository.ReplayAll();

      object actual = _mockDropDownEditorBase.EditValue (_mockTypeDescriptorContext, null, "The input value");

      _mockRepository.VerifyAll();

      Assert.That (actual, Is.EqualTo ("The input value"));
    }

    [Test]
    public void EditValue_WitoutEditorSerivce ()
    {
      Expect.Call (_mockTypeDescriptorContext.Instance).Return (new object());
      Expect.Call (_mockServiceProvider.GetService (typeof (IWindowsFormsEditorService))).Return (null);
      _mockRepository.ReplayAll();

      object actual = _mockDropDownEditorBase.EditValue (_mockTypeDescriptorContext, _mockServiceProvider, "The input value");

      _mockRepository.VerifyAll();

      Assert.That (actual, Is.EqualTo ("The input value"));
    }

    [Test]
    public void GetEditStyle ()
    {
      Expect.Call (_mockTypeDescriptorContext.Instance).Return (new object ());
      _mockRepository.ReplayAll ();

      UITypeEditorEditStyle actual = _mockDropDownEditorBase.GetEditStyle (_mockTypeDescriptorContext);

      _mockRepository.VerifyAll ();

      Assert.That (actual, Is.EqualTo (UITypeEditorEditStyle.DropDown));
    }

    [Test]
    public void GetEditStyle_WithoutTypeDescriptorContextInstance ()
    {
      Expect.Call (_mockTypeDescriptorContext.Instance).Return (null);
      _mockRepository.ReplayAll ();

      UITypeEditorEditStyle actual = _mockDropDownEditorBase.GetEditStyle (_mockTypeDescriptorContext);

      _mockRepository.VerifyAll ();

      Assert.That (actual, Is.EqualTo (UITypeEditorEditStyle.None));
    }

    [Test]
    public void GetEditStyle_WithoutTypeDescriptorContext ()
    {
      _mockRepository.ReplayAll ();

      UITypeEditorEditStyle actual = _mockDropDownEditorBase.GetEditStyle (null);

      _mockRepository.VerifyAll ();

      Assert.That (actual, Is.EqualTo (UITypeEditorEditStyle.None));
    }
  }
}
