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
using System.Linq;
using System.Text;
using System.Web.UI;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Utilities
{
  /// <summary> Utility class for client-side scripts. </summary>
  [ImplementationFor (typeof (IScriptUtility), Lifetime = LifetimeKind.Singleton)]
  public class ScriptUtility : IScriptUtility
  {
    private readonly IInfrastructureResourceUrlFactory _infrastructureResourceUrlFactory;

    #region Obsolete

    /// <summary>
    ///   Used to register a client javascript script to be rendered  at the beginning of the HTML page.
    ///   The script is automatically surrounded by &lt;script&gt; tags.
    /// </summary>
    /// <param name="control"> 
    ///   The <see cref="Control"/> which the script file will be registered. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="key"> 
    ///   The key identifying the registered script file. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="javascript"> 
    ///   The client script that will be registered. Must not be <see langword="null"/> or empty. 
    /// </param>
    /// <seealso cref="Page.RegisterClientScriptBlock"/>
    [Obsolete ("Use IPage.ClientScript.RegisterClientScriptBlock (IControl, Type, string, string) instead.")]
    public static void RegisterClientScriptBlock (Control control, string key, string javascript)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("javascript", javascript);

      if (!string.IsNullOrEmpty (javascript))
        javascript += "\r\n";

      ScriptManager.RegisterClientScriptBlock (control, typeof (Page), key, javascript, true);
    }

    /// <summary>
    ///   Used to register a client javascript script to be rendered at the end of the HTML page. 
    ///   The script is automatically surrounded by &lt;script&gt; tags.
    /// </summary>
    /// <param name="control"> 
    ///   The <see cref="Control"/> for which the script file will be registered. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="key"> 
    ///   The key identifying the registered script block. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="javascript"> 
    ///   The client script that will be registered. Must not be <see langword="null"/> or empty. 
    /// </param>
    /// <seealso cref="ScriptManager.RegisterStartupScript(System.Web.UI.Control,System.Type,string,string,bool)"/>
    [Obsolete ("Use IPage.ClientScript.RegisterStartupScriptBlock (IControl, Type, string, string) instead.")]
    public static void RegisterStartupScriptBlock (Control control, string key, string javascript)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("javascript", javascript);

      if (!string.IsNullOrEmpty (javascript))
        javascript += "\r\n";

      ScriptManager.RegisterStartupScript (control, typeof (Page), key, javascript, true);
    }

    [Obsolete ("Use ScriptUtility.Instance.RegisterElementForBorderSpans", true)]
    public static void RegisterElementForBorderSpans ()
    {
      throw new NotImplementedException ("Use ScriptUtility.Instance.RegisterElementForBorderSpans");
    }

    /// <summary>
    /// Gets a flag that informs the caller if the <paramref name="control"/> will be part of the rendered output. This method only works during the
    /// Render cycle.
    /// </summary>
    [Obsolete ("The various methods for registering scripts now accept controls instead of the page, thus allowing filtering of the output by the surrounding UpdatePanel.")]
    public static bool IsPartOfRenderedOutput (Control control)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      var scriptManager = ScriptManager.GetCurrent (control.Page);
      if (scriptManager != null && scriptManager.IsInAsyncPostBack)
      {
        bool isInsidePartialRenderingUpdatePanel = control.CreateSequence (c => c.Parent)
            .Where (c => c is UpdatePanel && ((UpdatePanel) c).IsInPartialRendering)
            .Cast<UpdatePanel> ()
            .Any ();

        return isInsidePartialRenderingUpdatePanel;
      }
      else
      {
        return true;
      }
    }

    #endregion

    public static IScriptUtility Instance
    {
      get { return SafeServiceLocator.Current.GetInstance<IScriptUtility>(); }
    }

    /// <summary> Escapes special characters (e.g. <c>\n</c>) in the passed string. </summary>
    /// <param name="input"> The unescaped string. Must not be <see langword="null"/>. </param>
    /// <returns> The string with special characters escaped. </returns>
    /// <remarks>
    ///   This is required when adding client script to the page containing special characters. ASP.NET automatically 
    ///   escapes client scripts created by <see cref="O:System.Web.UI.ClientScriptManager.GetPostBackEventReference">ClientScript.GetPostBackEventReference</see>.
    /// </remarks>
    public static string EscapeClientScript (string input)
    {
      ArgumentUtility.CheckNotNull ("input", input);

      StringBuilder output = new StringBuilder (input.Length + 5);
      for (int idxChars = 0; idxChars < input.Length; idxChars++)
      {
        char c = input[idxChars];
        switch (c)
        {
          case '\t':
            {
              output.Append (@"\t");
              break;
            }
          case '\n':
            {
              output.Append (@"\n");
              break;
            }
          case '\r':
            {
              output.Append (@"\r");
              break;
            }
          case '"':
            {
              output.Append ("\\\"");
              break;
            }
          case '\'':
            {
              output.Append (@"\'");
              break;
            }
          case '\\':
            {
              if (idxChars > 0 && idxChars + 1 < input.Length)
              {
                char prevChar = input[idxChars - 1];
                char nextChar = input[idxChars + 1];
                if (prevChar == '<' && nextChar == '/')
                {
                  output.Append (c);
                  break;
                }
              }
              output.Append (@"\\");
              break;
            }
          case '\v':
            {
              output.Append (c);
              break;
            }
          case '\f':
            {
              output.Append (c);
              break;
            }
          default:
            {
              output.Append (c);
              break;
            }
        }
      }
      return output.ToString ();
    }

    public ScriptUtility (IInfrastructureResourceUrlFactory infrastructureResourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("infrastructureResourceUrlFactory", infrastructureResourceUrlFactory);
      
      _infrastructureResourceUrlFactory = infrastructureResourceUrlFactory;
    }

    public void RegisterJavaScriptInclude (IControl control, HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string key = typeof (ScriptUtility).FullName + "_StyleUtility";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        var url = _infrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Html, "StyleUtility.js");

        htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();
        htmlHeadAppender.RegisterJavaScriptInclude (key, url);

        control.Page.ClientScript.RegisterClientScriptBlock (control, typeof(ScriptUtility), key, "StyleUtility.AddBrowserSwitch();");
      }
    }

    public void RegisterElementForBorderSpans (IControl control, string jQuerySelectorForBorderSpanTarget)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNullOrEmpty ("jQuerySelectorForBorderSpanTarget", jQuerySelectorForBorderSpanTarget);

      string key = "BorderSpans_" + jQuerySelectorForBorderSpanTarget;
      string script = string.Format ("StyleUtility.CreateBorderSpans ('{0}');", jQuerySelectorForBorderSpanTarget);
      control.Page.ClientScript.RegisterStartupScriptBlock (control, typeof (ScriptUtility), key, script);
    }

    public void RegisterResizeOnElement (IControl control, string jquerySelector, string eventHandler)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNullOrEmpty ("jquerySelector", jquerySelector);
      ArgumentUtility.CheckNotNullOrEmpty ("eventHandler", eventHandler);

      string key = control.ClientID + "_ResizeEventHandler";
      string script = string.Format ("PageUtility.Instance.RegisterResizeHandler({0}, {1});", jquerySelector, eventHandler);
      control.Page.ClientScript.RegisterStartupScriptBlock (control, typeof (ScriptUtility), key, script);
    }
  }
}
