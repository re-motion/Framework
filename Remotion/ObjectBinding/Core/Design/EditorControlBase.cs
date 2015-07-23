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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design
{
  [TypeDescriptionProvider (typeof (EditorControlBaseClassProvider))]
  public abstract class EditorControlBase : UserControl
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly IWindowsFormsEditorService _editorService;

    protected EditorControlBase (IServiceProvider provider, IWindowsFormsEditorService editorService)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("editorService", editorService);
      
      _serviceProvider = provider;
      _editorService = editorService;
    }

    protected EditorControlBase ()
    {
    }

    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public abstract object Value { get; set; }

    public IServiceProvider ServiceProvider
    {
      get { return _serviceProvider; }
    }

    public IWindowsFormsEditorService EditorService
    {
      get { return _editorService; }
    }
  }
}
