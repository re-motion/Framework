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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering a <see cref="BocList"/> object.
  /// </summary>
  /// <include file='..\..\..\..\doc\include\UI\Controls\BocListRenderer.xml' path='BocListRenderer/Class/*'/>
  /// <seealso cref="BocListNavigationBlockQuirksModeRenderer"/>
  public class BocListQuirksModeRenderer : BocQuirksModeRendererBase<IBocList>, IBocListRenderer
  {
    private const string c_defaultMenuBlockWidth = "70pt";
    private const string c_defaultMenuBlockOffset = "5pt";

    private readonly IBocListMenuBlockRenderer _menuBlockRenderer;
    private readonly IBocListNavigationBlockRenderer _navigationBlockRenderer;
    private readonly IBocListTableBlockRenderer _tableBlockRenderer;
    private readonly BocListQuirksModeCssClassDefinition _cssClasses;

    /// <summary>
    /// Initializes the renderer with the <see cref="BocList"/> to render and the <see cref="HtmlTextWriter"/> to render it to.
    /// </summary>
    /// <param name="cssClasses">The <see cref="BocListQuirksModeCssClassDefinition"/> containing the CSS classes to apply to the rendered elements.</param>
    /// <param name="tableBlockRenderer">The <see cref="IBocListTableBlockRenderer"/> responsible for rendering the table-part of the <see cref="BocList"/>.</param>
    /// <param name="navigationBlockRenderer">The <see cref="IBocListNavigationBlockRenderer"/> responsible for rendering the navigation-part of the <see cref="BocList"/>.</param>
    /// <param name="menuBlockRenderer">The <see cref="IBocListMenuBlockRenderer"/> responsible for rendering the menu-part of the <see cref="BocList"/>.</param>
    /// <param name="resourceUrlFactory">The <see cref="IResourceUrlFactory"/> responsible for creating URLs for resources.</param>
    public BocListQuirksModeRenderer (
        BocListQuirksModeCssClassDefinition cssClasses,
        IBocListTableBlockRenderer tableBlockRenderer,
        IBocListNavigationBlockRenderer navigationBlockRenderer,
        IBocListMenuBlockRenderer menuBlockRenderer,
        IResourceUrlFactory resourceUrlFactory)
      :base(resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);
      ArgumentUtility.CheckNotNull ("tableBlockRenderer", tableBlockRenderer);
      ArgumentUtility.CheckNotNull ("navigationBlockRenderer", navigationBlockRenderer);
      ArgumentUtility.CheckNotNull ("menuBlockRenderer", menuBlockRenderer);

      _cssClasses = cssClasses;
      _tableBlockRenderer = tableBlockRenderer;
      _navigationBlockRenderer = navigationBlockRenderer;
      _menuBlockRenderer = menuBlockRenderer;
    }

    private IBocListMenuBlockRenderer MenuBlockRenderer
    {
      get { return _menuBlockRenderer; }
    }

    private IBocListNavigationBlockRenderer NavigationBlockRenderer
    {
      get { return _navigationBlockRenderer; }
    }

    private IBocListTableBlockRenderer TableBlockRenderer
    {
      get { return _tableBlockRenderer; }
    }

    private Action<BocListRenderingContext> RenderTopLevelColumnGroup { get; set; }

    public BocListQuirksModeCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public override sealed string CssClassBase
    {
      get { return CssClasses.Base; }
    }

    public override sealed string CssClassDisabled
    {
      get { return CssClasses.Disabled; }
    }

    public override sealed string CssClassReadOnly
    {
      get { return CssClasses.ReadOnly; }
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, EditableRowControlFactory editableRowControlFactory)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();

      string styleFileKey = typeof (BocListQuirksModeRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (styleFileKey))
      {
        var styleUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocListQuirksModeRenderer), ResourceType.Html, "BocList.css");
        htmlHeadAppender.RegisterStylesheetLink (styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }

      string scriptFileKey = typeof (BocListQuirksModeRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (scriptFileKey))
      {
        var scriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocListQuirksModeRenderer), ResourceType.Html, "BocList.js");
        htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);
      }

      editableRowControlFactory.RegisterHtmlHeadContents (htmlHeadAppender);
    }

    /// <summary>
    /// Renders the <see cref="BocList"/> to the <see cref="HtmlTextWriter"/> in the Writer property.
    /// </summary>
    /// <remarks>
    /// This method provides the outline table of the <see cref="BocList"/>, creating three areas:
    /// <list type="bullet">
    /// <item><description>A table block displaying the title and data rows. See <see cref="IBocListTableBlockRenderer"/>.</description></item>
    /// <item><description>A menu block containing the available commands. See <see cref="IBocListMenuBlockRenderer"/></description></item>
    /// <item><description>A navigation block to browse through pages of data rows. See <see cref="IBocListNavigationBlockRenderer"/>.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="BocListMenuBlockQuirksModeRenderer"/>
    /// <seealso cref="BocListNavigationBlockQuirksModeRenderer"/>
    public void Render (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      RegisterInitializeGlobalsScript (renderingContext);

      AddAttributesToRender (renderingContext, false);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      RenderContents (renderingContext);

      renderingContext.Writer.RenderEndTag ();

      RegisterInitializeListScript (renderingContext);
    }

    protected virtual void RenderContents (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      //  Render list block / menu block
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Table);

      RenderTopLevelColumnGroup = (ctx) => RenderTopLevelColumnGroupForLegacyBrowser (ctx);
      if (!ControlHelper.IsDesignMode (renderingContext.Control))
      {
        bool isXmlRequired = (renderingContext.HttpContext != null) && ControlHelper.IsXmlConformResponseTextRequired (renderingContext.HttpContext);
        if (isXmlRequired)
          RenderTopLevelColumnGroup = (ctx) => RenderTopLevelColumnGroupForXmlBrowser(ctx);
      }

      RenderTopLevelColumnGroup (renderingContext);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      //  List Block
      renderingContext.Writer.AddStyleAttribute ("vertical-align", "top");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);

      TableBlockRenderer.Render (renderingContext);

      if (renderingContext.Control.HasNavigator)
        NavigationBlockRenderer.Render (renderingContext);

      renderingContext.Writer.RenderEndTag ();

      if (renderingContext.Control.HasMenuBlock)
      {
        //  Menu Block
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.MenuBlock);
        renderingContext.Writer.AddStyleAttribute ("vertical-align", "top");
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);
        MenuBlockRenderer.Render (renderingContext);
        renderingContext.Writer.RenderEndTag ();
      }

      renderingContext.Writer.RenderEndTag (); //  TR
      renderingContext.Writer.RenderEndTag (); //  Table
    }

    private void RenderTopLevelColumnGroupForLegacyBrowser (BocListRenderingContext renderingContext)
    {
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

      //  Left: list block
      renderingContext.Writer.WriteBeginTag ("col"); //  Required because RenderBeginTag(); RenderEndTag();
      //  writes empty tags, which is not valid for col in HTML 4.01
      renderingContext.Writer.Write (">");

      if (renderingContext.Control.HasMenuBlock)
      {
        //  Right: menu block
        renderingContext.Writer.WriteBeginTag ("col");
        renderingContext.Writer.Write (" style=\"");

        string menuBlockWidth = c_defaultMenuBlockWidth;
        if (!renderingContext.Control.MenuBlockWidth.IsEmpty)
          menuBlockWidth = renderingContext.Control.MenuBlockWidth.ToString ();
        renderingContext.Writer.WriteStyleAttribute ("width", menuBlockWidth);

        string menuBlockOffset = c_defaultMenuBlockOffset;
        if (!renderingContext.Control.MenuBlockOffset.IsEmpty)
          menuBlockOffset = renderingContext.Control.MenuBlockOffset.ToString ();
        renderingContext.Writer.WriteStyleAttribute ("padding-left", menuBlockOffset);

        renderingContext.Writer.Write ("\">");
      }

      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderTopLevelColumnGroupForXmlBrowser (BocListRenderingContext renderingContext)
    {
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

      // Left: list block
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Col);
      renderingContext.Writer.RenderEndTag ();

      if (renderingContext.Control.HasMenuBlock)
      {
        //  Right: menu block
        string menuBlockWidth = c_defaultMenuBlockWidth;
        if (!renderingContext.Control.MenuBlockWidth.IsEmpty)
          menuBlockWidth = renderingContext.Control.MenuBlockWidth.ToString ();

        string menuBlockOffset = c_defaultMenuBlockOffset;
        if (!renderingContext.Control.MenuBlockOffset.IsEmpty)
          menuBlockOffset = renderingContext.Control.MenuBlockOffset.ToString ();

        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, menuBlockWidth);
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.PaddingLeft, menuBlockOffset);
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Col);
        renderingContext.Writer.RenderEndTag ();
      }

      renderingContext.Writer.RenderEndTag ();
    }

    private void RegisterInitializeGlobalsScript (BocListRenderingContext renderingContext)
    {
      if (!renderingContext.Control.HasClientScript)
        return;

      string startUpScriptKey = typeof (IBocList).FullName + "_Startup";
      if (!renderingContext.Control.Page.ClientScript.IsStartupScriptRegistered (typeof (BocListQuirksModeRenderer), startUpScriptKey))
      {
        string script = string.Format (
            "BocList_InitializeGlobals ('{0}', '{1}');",
            CssClasses.DataRow,
            CssClasses.DataRowSelected);
        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (BocListQuirksModeRenderer), startUpScriptKey, script);
      }
    }

    private void RegisterInitializeListScript (BocListRenderingContext renderingContext)
    {
      if (renderingContext.Control.HasClientScript && renderingContext.Control.IsSelectionEnabled)
      {
        //  Render the init script for the client side selection handling
        int startIndex = 0;
        int count = 0;
        if (renderingContext.Control.IsPagingEnabled)
        {
          startIndex = renderingContext.Control.PageSize.Value * renderingContext.Control.CurrentPageIndex;
          count = renderingContext.Control.PageSize.Value;
        }
        else if (renderingContext.Control.Value != null)
        {
          count = renderingContext.Control.Value.Count;
        }

        bool hasClickSensitiveRows = renderingContext.Control.IsSelectionEnabled && !renderingContext.Control.EditModeController.IsRowEditModeActive &&
                                     renderingContext.Control.AreDataRowsClickSensitive();

        const string scriptTemplate = "BocList_InitializeList ( $('#{0}')[0], '{1}', '{2}', {3}, {4}, {5}, {6}, {7});";
        string script = string.Format (
            scriptTemplate,
            renderingContext.Control.ClientID,
            renderingContext.Control.GetSelectorControlName ().Replace('$', '_'),
            renderingContext.Control.GetSelectAllControlName(),
            startIndex,
            count,
            (int) renderingContext.Control.Selection,
            hasClickSensitiveRows ? "true" : "false",
            renderingContext.Control.GetSelectionChangedHandlerScript());

        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (
            renderingContext.Control,
            typeof (BocListTableBlockQuirksModeRenderer),
            typeof (BocList).FullName + "_" + renderingContext.Control.ClientID
            + "_InitializeListScript",
            script);
      }
    }
  }
}