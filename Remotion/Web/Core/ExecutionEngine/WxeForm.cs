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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.ExecutionEngine
{

  [DesignTimeVisible(false)]
  public class WxeForm : HtmlForm, IPostBackDataHandler, IControl
  {
    private static readonly object s_loadPostDataEvent = new object();

    public static WxeForm Replace (HtmlForm htmlForm)
    {
      WxeForm newForm = new WxeForm();

      if (!string.IsNullOrEmpty(htmlForm.Method))
        newForm.Method = htmlForm.Method;
      if (!string.IsNullOrEmpty(htmlForm.Enctype))
        newForm.Enctype = htmlForm.Enctype;
      if (!string.IsNullOrEmpty(htmlForm.Target))
        newForm.Target = htmlForm.Target;
      if (!string.IsNullOrEmpty(htmlForm.DefaultButton))
        newForm.DefaultButton = htmlForm.DefaultButton;
      if (!string.IsNullOrEmpty(htmlForm.DefaultFocus))
        newForm.DefaultFocus = htmlForm.DefaultFocus;

      while (htmlForm.Controls.Count > 0)
        newForm.Controls.Add(htmlForm.Controls[0]);

      Control? parent = htmlForm.Parent;
      if (parent != null)
      {
        int htmlFormIndex = parent.Controls.IndexOf(htmlForm);
        if (htmlFormIndex >= 0)
        {
          parent.Controls.RemoveAt(htmlFormIndex);
          parent.Controls.AddAt(htmlFormIndex, newForm);
        }
        else
        {
          parent.Controls.Add(newForm);
        }
      }

      newForm.ID = htmlForm.ID;
      newForm.Name = htmlForm.Name;
      return newForm;
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      Page!.RegisterRequiresPostBack(this);
    }

    /// <summary> Calls the <see cref="OnLoadPostData"/> method. </summary>
    bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      return OnLoadPostData(postDataKey, postCollection);
    }

    /// <summary> Calls the <see cref="RaisePostDataChangedEvent"/> method. </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent ()
    {
      RaisePostDataChangedEvent();
    }

    /// <summary> Fires the <see cref="LoadPostData"/> event. </summary>
    protected virtual bool OnLoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      EventHandler? eventHandler = (EventHandler?)Events[s_loadPostDataEvent];
      if (eventHandler != null)
        eventHandler(this, EventArgs.Empty);
      return false;
    }

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected virtual void RaisePostDataChangedEvent ()
    {
    }

    /// <summary> Occurs during the load post data phase. </summary>
    [Browsable(false)]
    public event EventHandler LoadPostData
    {
      add { Events.AddHandler(s_loadPostDataEvent, value); }
      remove { Events.RemoveHandler(s_loadPostDataEvent, value); }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender(e);
    }

    protected override void RenderAttributes (HtmlTextWriter writer)
    {
      writer.WriteAttribute("onreset", "return false;");

      string action = WxeContext.Current!.GetPath(WxeContext.Current.QueryString); // TODO RM-8118: not null assertion
      writer.WriteAttribute("action", action, true);
      Attributes.Remove("action");

      // from HtmlForm
      writer.WriteAttribute("name", this.Name);
      base.Attributes.Remove("name");
      writer.WriteAttribute("method", this.Method);
      base.Attributes.Remove("method");
      //  writer.WriteAttribute("action", this.GetActionAttribute(), true);
      //  base.Attributes.Remove("action");
      //  string text1 = this.Page.ClientOnSubmitEvent;
      //  if ((text1 != null) && (text1.Length > 0))
      //  {
      //    if (base.Attributes["onsubmit"] != null)
      //    {
      //      text1 = text1 + base.Attributes["onsubmit"];
      //      base.Attributes.Remove("onsubmit");
      //    }
      //    writer.WriteAttribute("language", "javascript");
      //    writer.WriteAttribute("onsubmit", text1);
      //  }
      if (this.ID == null)
        writer.WriteAttribute("id", this.ClientID);

      // from HtmlContainerControl
      this.ViewState.Remove("innerhtml");

      // from HtmlControl
      if (this.ID != null)
        writer.WriteAttribute("id", this.ClientID);

      if (Page != null)
      {
        if (((Page.Request != null) && (Page.Request.Browser.EcmaScriptVersion.Major > 0)) && ((Page.Request.Browser.W3CDomVersion.Major > 0) && (DefaultButton.Length > 0)))
        {
          Control? defaultButton = FindControl(DefaultButton);
          if (defaultButton == null)
          {
            char[] chArray1 = new char[] { '$', ':' };
            if (this.DefaultButton.IndexOfAny(chArray1) != -1)
            {
              defaultButton = Page.FindControl(DefaultButton);
            }
          }
          if (!(defaultButton is IButtonControl))
            throw new InvalidOperationException(string.Format("The DefaultButton of '{0}' must be the ID of a control of type IButtonControl.", new object?[] { ID }));

          //Page.ClientScript.RegisterDefaultButtonScript (defaultButton, writer, false);
          RegisterDefaultButtonScript(defaultButton, writer, false);
        }
      }
      base.EnsureID();

      this.Attributes.Render(writer);
    }

    protected override void Render (HtmlTextWriter writer)
    {
      var scriptManager = ScriptManager.GetCurrent(Page!.WrappedInstance); // TODO RM-8118: not null assertion
      if (!Remotion.Web.UI.HtmlHeadAppender.Current.HasAppended && (scriptManager == null || !scriptManager.IsInAsyncPostBack))
      {
        throw new WxeException(
            "The Remotion.Web.UI.Controls.HtmlHeadContents control is missing on the page. Please add this control to the 'head' section of the document "
            + "or specify the runat=server attribute for the 'head' section to allow for an automatically generated HtmlHeadContents control.");
      }

      if (Page.MaintainScrollPositionOnPostBack)
      {
        throw new WxeException(
            "Enabling the ASP.NET smart navigation by setting System.Web.UI.Page.MaintainScrollPositionOnPostBack to true is not supported on WXE Pages. "
            + "Use the smart navigation feature (SmartPage.EnableSmartScrolling, enabled by default) integrated into re-motion framework instead.");
      }

      base.Render(writer);
    }

    //  protected override void RenderChildren(HtmlTextWriter writer)
    //  {
    //    writer.AddAttribute (HtmlTextWriterAttribute.Id, "wxeContentArea");
    //    writer.RenderBeginTag (HtmlTextWriterTag.Div);
    //
    //    base.RenderChildren (writer);
    //
    //    writer.RenderEndTag();
    //  }

    private void RegisterDefaultButtonScript (Control button, HtmlTextWriter writer, bool useAddAttribute)
    {
      string? dummy = Page!.ClientScript.GetPostBackEventReference(new PostBackOptions(button));
      string script = "javascript:return WebForm_FireDefaultButton(event, '" + button.ClientID + "')";
      if (useAddAttribute)
        writer.AddAttribute("onkeypress", script);
      else
        writer.WriteAttribute("onkeypress", script);
    }

    public new IPage? Page
    {
      get { return PageWrapper.CastOrCreate(base.Page); }
    }
  }

}
