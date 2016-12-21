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
using System.Text;
using System.Web.UI;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Provides a common base class for Quirks Mode renderers or <see cref="BocReferenceValue"/> and <see cref="BocAutoCompleteReferenceValue"/>.
  /// </summary>
  public abstract class BocReferenceValueQuirksModeRendererBase<TControl, TRenderingContext> : BocQuirksModeRendererBase<TControl>
    where TControl : IBocReferenceValueBase
    where TRenderingContext : BocRenderingContext<TControl>
  {
    protected BocReferenceValueQuirksModeRendererBase (IResourceUrlFactory resourceUrlFactory)
        : base(resourceUrlFactory)
    {
    }

    protected abstract void RenderContents (TRenderingContext renderingContext);

    protected virtual void RegisterJavaScriptFiles (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();

      string scriptKey = typeof (BocReferenceValueQuirksModeRendererBase<,>).FullName + "_Script";
      htmlHeadAppender.RegisterJavaScriptInclude (
          scriptKey,
          ResourceUrlFactory.CreateResourceUrl (typeof (BocReferenceValueQuirksModeRendererBase<,>), ResourceType.Html, "BocReferenceValueBase.js"));
    }

    protected void Render (TRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (renderingContext, false);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      RenderContents (renderingContext);

      renderingContext.Writer.RenderEndTag ();

      RegisterInitializationScript (renderingContext);

      if (!string.IsNullOrEmpty (renderingContext.Control.IconServicePath))
      {
        CheckScriptManager (
            renderingContext.Control,
            "{0} '{1}' requires that the page contains a ScriptManager because the IconServicePath is set.",
            renderingContext.Control.GetType().Name,
            renderingContext.Control.ID);
      }
    }

    private void RegisterInitializationScript (BocRenderingContext<TControl> renderingContext)
    {
      string key = typeof (BocReferenceValueQuirksModeRendererBase<,>).FullName + "_InitializeGlobals";

      if (renderingContext.Control.Page.ClientScript.IsClientScriptBlockRegistered (typeof (BocReferenceValueQuirksModeRendererBase<,>), key))
        return;

      var nullIcon = IconInfo.CreateSpacer (ResourceUrlFactory);

      var script = new StringBuilder (1000);
      script.Append ("BocReferenceValueBase.InitializeGlobals(");
      script.AppendFormat ("'{0}'", nullIcon.Url);
      script.Append (");");

      renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (
          renderingContext.Control, typeof (BocReferenceValueQuirksModeRendererBase<,>), key, script.ToString ());
    }

    protected string GetIconServicePath (RenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      if (!renderingContext.Control.IsIconEnabled())
        return null;

      var iconServicePath = renderingContext.Control.IconServicePath;

      if (string.IsNullOrEmpty (iconServicePath))
        return null;
      return renderingContext.Control.ResolveClientUrl (iconServicePath);
    }

    protected string GetIconContextAsJson (BusinessObjectIconWebServiceContext iconServiceContext)
    {
      if (iconServiceContext == null)
        return null;
      
      var jsonBuilder = new StringBuilder (1000);

      jsonBuilder.Append ("{ ");
      jsonBuilder.Append ("businessObjectClass : ");
      AppendStringValueOrNullToScript (jsonBuilder, iconServiceContext.BusinessObjectClass);
      jsonBuilder.Append (", ");
      jsonBuilder.Append ("arguments : ");
      AppendStringValueOrNullToScript (jsonBuilder, iconServiceContext.Arguments);
      jsonBuilder.Append (" }");

      return jsonBuilder.ToString ();
    }

    protected string GetCommandInfoAsJson (BocRenderingContext<TControl> renderingContext)
    {
      var command = renderingContext.Control.Command;
      if (command == null)
        return null;

      if (command.Show == CommandShow.ReadOnly)
        return null;

      var postBackEvent = GetPostBackEvent (renderingContext);
      var commandInfo = command.GetCommandInfo (postBackEvent, new[] { "-0-" }, "", null, new NameValueCollection(), false);

      if (commandInfo == null)
        return null;

      var jsonBuilder = new StringBuilder (1000);

      jsonBuilder.Append ("{ ");

      jsonBuilder.Append ("href : ");
      string href = commandInfo.Href.Replace ("-0-", "{0}");
      AppendStringValueOrNullToScript (jsonBuilder, href);

      jsonBuilder.Append (", ");

      jsonBuilder.Append ("target : ");
      string target = commandInfo.Target;
      AppendStringValueOrNullToScript (jsonBuilder, target);

      jsonBuilder.Append (", ");

      jsonBuilder.Append ("onClick : ");
      string onClick = commandInfo.OnClick;
      AppendStringValueOrNullToScript (jsonBuilder, onClick);

      jsonBuilder.Append (", ");

      jsonBuilder.Append ("title : ");
      string title = commandInfo.Title;
      AppendStringValueOrNullToScript (jsonBuilder, title);

      jsonBuilder.Append (" }");

      return jsonBuilder.ToString ();
    }

    protected string GetPostBackEvent (BocRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      if (renderingContext.Control.IsDesignMode)
        return "";

      string argument = BocReferenceValueBase.CommandArgumentName;
      return renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, argument) + ";";
    }

    protected virtual string GetDropDownButtonName (BocRenderingContext<TControl> renderingContext)
    {
      return renderingContext.Control.ClientID + "_DropDownButton";
    }
  }
}