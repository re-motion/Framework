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
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// The <see cref="CommandInfo"/> object contains all information required for rendering a command as a hyperlink or a postback commad.
  /// </summary>
  public sealed class CommandInfo
  {
    /// <summary>
    /// Creates a <see cref="CommandInfo"/> for a hyperlink-based command.
    /// </summary>
    /// <param name="title">The text displayed as the command's title. Must not be empty.</param>
    /// <param name="accessKey">The <see cref="char"/> to use for keyboard-shortcut navigation.</param>
    /// <param name="href">The url to be opened when the command is clicked. Must not be <see langword="null" /> or empty.</param>
    /// <param name="target">The target where the url is opened.  Must not be empty.</param>
    /// <param name="onClick">An optional javascript hooked up to the click event. The script is only allowed to do housekeeping. Must not be empty.</param>
    public static CommandInfo CreateForLink (string title, string accessKey, string href, string target, string onClick)
    {
      ArgumentUtility.CheckNotEmpty ("title", title);
      ArgumentUtility.CheckNotNullOrEmpty ("href", href);
      ArgumentUtility.CheckNotEmpty ("target", target);
      ArgumentUtility.CheckNotEmpty ("onClick", onClick);

      return new CommandInfo (title, accessKey, href, target, onClick);
    }

    /// <summary>
    /// Creates a <see cref="CommandInfo"/> for a postback-based command.
    /// </summary>
    /// <param name="title">The <see cref="string"/> displayed as the commands title. Must not be empty.</param>
    /// <param name="accessKey">The <see cref="char"/> to use for keyboard-shortcut navigation.</param>
    /// <param name="onClick">The javascript hooked up to the click event. Must not be <see langword="null" /> or empty.</param>
    public static CommandInfo CreateForPostBack (string title, string accessKey, string onClick)
    {
      ArgumentUtility.CheckNotEmpty ("title", title);
      ArgumentUtility.CheckNotNullOrEmpty ("onClick", onClick);

      return new CommandInfo (title, accessKey, "#", null, onClick);
    }

    private readonly string _title;
    private readonly string _accessKey;
    private readonly string _href;
    private readonly string _target;
    private readonly string _onClick;

    private CommandInfo (string title, string accessKey, string href, string target, string onClick)
    {
      _title = title;
      _accessKey = accessKey;
      _href = href;
      _target = target;
      _onClick = onClick;
    }

    public string Href
    {
      get { return _href; }
    }

    public string Target
    {
      get { return _target; }
    }

    public string OnClick
    {
      get { return _onClick; }
    }

    public string Title
    {
      get { return _title; }
    }

    public string AccessKey
    {
      get { return _accessKey; }
    }

    public void AddAttributesToRender (HtmlTextWriter writer, IRenderingFeatures renderingFeatures)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      if (!string.IsNullOrEmpty (_href))
        writer.AddAttribute (HtmlTextWriterAttribute.Href, _href);

      if (!string.IsNullOrEmpty (_target))
        writer.AddAttribute (HtmlTextWriterAttribute.Target, _target);

      if (!string.IsNullOrEmpty (_onClick))
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, _onClick);

      if (!string.IsNullOrEmpty (_accessKey))
        writer.AddAttribute (HtmlTextWriterAttribute.Accesskey, _accessKey);

      if (!string.IsNullOrEmpty (_title))
        writer.AddAttribute (HtmlTextWriterAttribute.Title, _title);

      if (renderingFeatures.EnableDiagnosticMetadata)
        AddDiagnosticMetadataAttributes (writer);
    }

    private void AddDiagnosticMetadataAttributes (HtmlTextWriter writer)
    {
      writer.AddAttribute (DiagnosticMetadataAttributes.ControlType, "Command");
      writer.AddAttribute (DiagnosticMetadataAttributes.TriggersNavigation, IsTriggeringNavigation().ToString().ToLower());
      writer.AddAttribute (DiagnosticMetadataAttributes.TriggersPostBack, IsTriggeringPostBack().ToString().ToLower());
    }

    private bool IsTriggeringNavigation ()
    {
      return _href != "#";
    }

    private bool IsTriggeringPostBack()
    {
      return !IsTriggeringNavigation() && _onClick.Contains ("__doPostBack");
    }
  }
}