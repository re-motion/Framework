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
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Resolvers
{
  /// <summary>
  /// Resolves <see cref="IWebElement"/>s for screenshot annotations.
  /// </summary>
  public class WebElementResolver : IScreenshotElementResolver<IWebElement>
  {
#pragma warning disable 0649
    [DataContract]
    private class ResultDto
    {
      [DataMember]
      public RectangleDto ElementBounds = null!;

      [DataMember]
      public RectangleDto ParentBounds = null!;

      public Rectangle GetElementBounds ()
      {
        return new Rectangle(ElementBounds.X, ElementBounds.Y, ElementBounds.Width, ElementBounds.Height);
      }

      public Rectangle GetParentBounds ()
      {
        return new Rectangle(ParentBounds.X, ParentBounds.Y, ParentBounds.Width, ParentBounds.Height);
      }
    }

    [DataContract]
    private class RectangleDto
    {
      [DataMember]
      public int X;

      [DataMember]
      public int Y;

      [DataMember]
      public int Width;

      [DataMember]
      public int Height;
    }
#pragma warning restore 0649

    private static class ScriptLoader
    {
      private static string? s_script;

      public static string Script
      {
        get { return s_script ?? RefreshScript(); }
      }

      private static string RefreshScript ()
      {
        var resourceName = typeof(WebElementResolver).FullName + "Script.js";
        using (var stream = typeof(WebElementResolver).Assembly.GetManifestResourceStream(resourceName))
        {
          if (stream == null)
            throw new InvalidOperationException("Could not find the WebElementResolver script.");

          // String builder in which the script will be loaded.
          var scriptBuilder = new StringBuilder();
          scriptBuilder.Append("return ");

          using (var reader = new StreamReader(stream))
          {
            string? line;
            while ((line = reader.ReadLine()) != null)
              if (!line.StartsWith("//"))
                scriptBuilder.Append(line);
          }

          s_script = scriptBuilder.ToString();
          return s_script;
        }
      }
    }

    /// <summary>
    /// Singleton instance of <see cref="WebElementResolver"/>.
    /// </summary>
    public static readonly WebElementResolver Instance = new WebElementResolver();

    private WebElementResolver ()
    {
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveBrowserCoordinates (IWebElement target)
    {
      ArgumentUtility.CheckNotNull("target", target);

      var executor = JavaScriptExecutor.GetJavaScriptExecutor(target);
      var rawResult = JavaScriptExecutor.ExecuteStatement<string>(executor, ScriptLoader.Script, target);
      var result = DataContractJsonSerializationUtility.Deserialize<ResultDto>(rawResult);
      Assertion.IsNotNull(result, "Could not deserialize javascript result '{0}'.", rawResult);

      var elementBounds = result.GetElementBounds();
      var parentBounds = result.GetParentBounds();
      var unresolvedBounds = elementBounds;
      var intersection = Rectangle.Intersect(elementBounds, parentBounds);

      ElementVisibility visibility;
      if (intersection.IsEmpty)
        visibility = ElementVisibility.NotVisible;
      else if (intersection == elementBounds)
        visibility = ElementVisibility.FullyVisible;
      else
        visibility = ElementVisibility.PartiallyVisible;

      return new ResolvedScreenshotElement(CoordinateSystem.Browser, elementBounds, visibility, parentBounds, unresolvedBounds);
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveDesktopCoordinates (IWebElement target, IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull("target", target);
      ArgumentUtility.CheckNotNull("locator", locator);

      var executor = JavaScriptExecutor.GetJavaScriptExecutor(target);
      var rawResult = JavaScriptExecutor.ExecuteStatement<string>(executor, ScriptLoader.Script, target);
      var result = DataContractJsonSerializationUtility.Deserialize<ResultDto>(rawResult);
      Assertion.IsNotNull(result, "Could not deserialize javascript result '{0}'.", rawResult);

      var elementBounds = result.GetElementBounds();
      var unresolvedBounds = elementBounds;
      var parentBounds = result.GetParentBounds();
      var intersection = Rectangle.Intersect(elementBounds, parentBounds);

      ElementVisibility visibility;
      if (intersection.IsEmpty)
        visibility = ElementVisibility.NotVisible;
      else if (intersection == elementBounds)
        visibility = ElementVisibility.FullyVisible;
      else
        visibility = ElementVisibility.PartiallyVisible;

      var window = locator.GetBrowserContentBounds(((IWrapsDriver)target).WrappedDriver);
      elementBounds.Offset(window.Location);
      parentBounds.Offset(window.Location);

      return new ResolvedScreenshotElement(CoordinateSystem.Browser, elementBounds, visibility, parentBounds, unresolvedBounds);
    }
  }
}
