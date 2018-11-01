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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.UI.Controls
{
  public class DatePickerButton : WebControl, IDatePickerButton
  {
    private readonly Style _datePickerButtonStyle;

    public DatePickerButton ()
    {
      EnableClientScript = true;
      AlternateText = string.Empty;
      _datePickerButtonStyle = new Style();
    }

    public bool IsDesignMode { get; set; }

    public bool EnableClientScript { get; set; }

    public string AlternateText { get; set; }

    public Style DatePickerButtonStyle
    {
      get { return _datePickerButtonStyle; }
    }

    public string ContainerControlID { get; set; }

    public string TargetControlID { get; set; }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      if (!IsDesignMode)
        RegisterHtmlHeadContents (HtmlHeadAppender.Current);
    }
    
    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      var renderer = CreateRenderer();
      renderer.Render (CreateRenderingContext(writer));
    }

    IControl IDatePickerButton.Parent
    {
      get { return (IControl) Parent; }
    }

    public new IPage Page
    {
      get { return PageWrapper.CastOrCreate (base.Page); }
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents (htmlHeadAppender);
    }

    protected virtual IDatePickerButtonRenderer CreateRenderer ()
    {
      return SafeServiceLocator.Current.GetInstance<IDatePickerButtonRenderer> ();
    }

    protected virtual DatePickerButtonRenderingContext CreateRenderingContext (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      return new DatePickerButtonRenderingContext (Page.Context, writer, this);
    }

    string IControlWithDiagnosticMetadata.ControlType
    {
      get { return "DatePickerButton"; }
    }
  }
}
