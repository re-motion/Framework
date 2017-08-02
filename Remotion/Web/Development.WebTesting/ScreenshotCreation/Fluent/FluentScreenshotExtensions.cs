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
using System.Windows.Automation;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent
{
  /// <summary>
  /// Extension methods for <see cref="IFluentScreenshotElement{T}"/>s.
  /// </summary>
  public static class FluentScreenshotExtensions
  {
    /// <summary>
    /// Starts the fluent screenshot interface for the specified <paramref name="automationElement"/>.
    /// </summary>
    public static FluentScreenshotElement<AutomationElement> ForAutomationElementScreenshot (
        [NotNull] this AutomationElement automationElement,
        [CanBeNull] IFluentScreenshotElement parent = null,
        [CanBeNull] Rectangle? parentContainer = null)
    {
      ArgumentUtility.CheckNotNull ("automationElement", automationElement);

      return FluentUtility.CreateFluentAutomationElement (automationElement);
    }

    /// <summary>
    /// Starts the fluent screenshot interface for the specified <paramref name="controlObject"/>.
    /// </summary>
    public static FluentScreenshotElement<T> ForControlObjectScreenshot<T> (
        [NotNull] this T controlObject,
        [CanBeNull] IFluentScreenshotElement parent = null,
        [CanBeNull] Rectangle? parentContainer = null)
        where T : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlObject", controlObject);

      return FluentUtility.CreateFluentControlObject (controlObject);
    }

    /// <summary>
    /// Starts the fluent screenshot interface for the specified <paramref name="elementScope"/>.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> ForElementScopeScreenshot (
        [NotNull] this ElementScope elementScope,
        [CanBeNull] IFluentScreenshotElement parent = null,
        [CanBeNull] Rectangle? parentContainer = null)
    {
      ArgumentUtility.CheckNotNull ("elementScope", elementScope);

      return FluentUtility.CreateFluentElementScope (elementScope);
    }

    /// <summary>
    /// Starts the fluent screenshot interface for the specified <paramref name="webElement"/>.
    /// </summary>
    public static FluentScreenshotElement<IWebElement> ForWebElementScreenshot (
        [NotNull] this IWebElement webElement,
        [CanBeNull] IFluentScreenshotElement parent = null,
        [CanBeNull] Rectangle? parentContainer = null)
    {
      ArgumentUtility.CheckNotNull ("webElement", webElement);

      return FluentUtility.CreateFluentWebElement (webElement);
    }

    /// <summary>
    /// Returns the target element of the specified <paramref name="fluentElement"/>.
    /// </summary>
    public static T GetTarget<T> ([NotNull] this IFluentScreenshotElement<T> fluentElement)
    {
      ArgumentUtility.CheckNotNull ("fluentElement", fluentElement);

      return fluentElement.Target;
    }

    /// <summary>
    /// Annotates the specified <paramref name="fluentTarget"/> with a box.
    /// </summary>
    public static void AnnotateBox<T> (
        [NotNull] this ScreenshotBuilder builder,
        [NotNull] IFluentScreenshotElement<T> fluentTarget,
        [CanBeNull] Pen pen = null,
        [CanBeNull] WebPadding? padding = null,
        [CanBeNull] Brush brush = null)
    {
      ArgumentUtility.CheckNotNull ("builder", builder);
      ArgumentUtility.CheckNotNull ("fluentTarget", fluentTarget);

      IScreenshotAnnotation annotation = new ScreenshotBoxAnnotation (
          pen ?? Pens.Red,
          padding ?? WebPadding.None,
          brush);

      FluentUtility.AnnotateFluent (builder, fluentTarget, annotation);
    }

    /// <summary>
    /// Annotates the specified <paramref name="fluentTarget"/> with the specified text.
    /// </summary>
    public static void AnnotateText<T> (
        [NotNull] this ScreenshotBuilder builder,
        [NotNull] IFluentScreenshotElement<T> fluentTarget,
        [NotNull] string content,
        [CanBeNull] Font font = null,
        [CanBeNull] Brush foregroundBrush = null,
        [CanBeNull] Brush backgroundBrush = null,
        [CanBeNull] StringFormat stringFormat = null,
        [CanBeNull] ContentAlignment? contentAlignment = null,
        [CanBeNull] WebPadding? padding = null,
        [CanBeNull] float? maxWidth = null,
        [CanBeNull] float? maxHeight = null)
    {
      ArgumentUtility.CheckNotNull ("builder", builder);
      ArgumentUtility.CheckNotNull ("fluentTarget", fluentTarget);
      ArgumentUtility.CheckNotNull ("content", content);

      IScreenshotAnnotation annotation = new ScreenshotTextAnnotation (
          content,
          font ?? SystemFonts.DefaultFont,
          foregroundBrush ?? Brushes.Red,
          backgroundBrush,
          stringFormat ?? StringFormat.GenericDefault,
          contentAlignment ?? ContentAlignment.MiddleCenter,
          padding ?? WebPadding.None,
          maxWidth,
          maxHeight);

      FluentUtility.AnnotateFluent (builder, fluentTarget, annotation);
    }

    /// <summary>
    /// Crops screenshot around the specified <paramref name="fluentTarget"/>.
    /// </summary>
    public static void Crop<T> (
        [NotNull] this ScreenshotBuilder builder,
        [NotNull] IFluentScreenshotElement<T> fluentTarget,
        [CanBeNull] WebPadding? padding = null)
    {
      ArgumentUtility.CheckNotNull ("builder", builder);
      ArgumentUtility.CheckNotNull ("fluentTarget", fluentTarget);

      FluentUtility.CropFluent (builder, fluentTarget, new ScreenshotCropping (padding ?? WebPadding.None));
    }

    /// <summary>
    /// Free-draws onto the <see cref="Graphics"/> object of the screenshot.
    /// </summary>
    public static void Freedraw ([NotNull] this ScreenshotBuilder builder, [NotNull] Action<Graphics, ResolvedScreenshotElement> drawAction)
    {
      ArgumentUtility.CheckNotNull ("builder", builder);
      ArgumentUtility.CheckNotNull ("drawAction", drawAction);

      builder.Annotate (new ScreenshotCustomAnnotation (drawAction));
    }

    /// <summary>
    /// Free-draws onto the <see cref="Graphics"/> object of the screenshot, targeting <paramref name="fluentTarget"/>.
    /// </summary>
    public static void Freedraw<T> (
        [NotNull] this ScreenshotBuilder builder,
        [NotNull] IFluentScreenshotElement<T> fluentTarget,
        [NotNull] Action<Graphics, ResolvedScreenshotElement> drawAction)
    {
      ArgumentUtility.CheckNotNull ("builder", builder);
      ArgumentUtility.CheckNotNull ("fluentTarget", fluentTarget);
      ArgumentUtility.CheckNotNull ("drawAction", drawAction);

      FluentUtility.AnnotateFluent (builder, fluentTarget, new ScreenshotCustomAnnotation (drawAction));
    }
  }
}