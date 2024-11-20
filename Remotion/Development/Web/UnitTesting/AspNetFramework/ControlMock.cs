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
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.Development.Web.UnitTesting.AspNetFramework
{
  public class ControlMock : Control
  {
    // types

    // static members and constants

    // member fields

    private string? _valueInViewState;
    private string? _valueInControlState;

    // construction and disposing

    public ControlMock ()
    {
    }

    // methods and properties

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      Assertion.IsNotNull(Page, "Page is null for control '{0}'", ID!);
      Page.RegisterRequiresControlState(this);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? ValueInViewState
    {
      get { return _valueInViewState; }
      set { _valueInViewState = value; }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? ValueInControlState
    {
      get { return _valueInControlState; }
      set { _valueInControlState = value; }
    }

    protected override void LoadViewState (object? savedState)
    {
      _valueInViewState = (string?)savedState;
    }

    protected override object? SaveViewState ()
    {
      return _valueInViewState;
    }

    protected override void LoadControlState (object? savedState)
    {
      _valueInControlState = (string?)savedState;
    }

    protected override object? SaveControlState ()
    {
      return _valueInControlState;
    }

    protected override void Render (HtmlTextWriter writer)
    {
      writer.RenderBeginTag( HtmlTextWriterTag.Div);
      writer.Write("ValueInViewState: {0}", _valueInViewState);
      writer.WriteBreak();
      writer.Write("ValueInControlState: {0}", _valueInControlState);
      writer.RenderEndTag();
    }
  }
}
