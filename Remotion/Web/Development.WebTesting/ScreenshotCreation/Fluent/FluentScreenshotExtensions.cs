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
using System.Threading;
using Coypu;
using JetBrains.Annotations;
using Microsoft.Maui.Graphics;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations;
using Remotion.Web.Development.WebTesting.SystemDrawingImitators;
using Remotion.Web.Development.WebTesting.Utilities;
using SkiaSharp;

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
        [CanBeNull] IFluentScreenshotElement? parent = null,
        [CanBeNull] Rectangle? parentContainer = null)
    {
      ArgumentUtility.CheckNotNull("automationElement", automationElement);

      return FluentUtility.CreateFluentAutomationElement(automationElement);
    }

    /// <summary>
    /// Starts the fluent screenshot interface for the specified <paramref name="controlObject"/>.
    /// </summary>
    public static FluentScreenshotElement<T> ForControlObjectScreenshot<T> (
        [NotNull] this T controlObject,
        [CanBeNull] IFluentScreenshotElement? parent = null,
        [CanBeNull] Rectangle? parentContainer = null)
        where T : ControlObject
    {
      ArgumentUtility.CheckNotNull("controlObject", controlObject);

      return FluentUtility.CreateFluentControlObject(controlObject);
    }

    /// <summary>
    /// Starts the fluent screenshot interface for the specified <paramref name="elementScope"/>.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> ForElementScopeScreenshot (
        [NotNull] this ElementScope elementScope,
        [CanBeNull] IFluentScreenshotElement? parent = null,
        [CanBeNull] Rectangle? parentContainer = null)
    {
      ArgumentUtility.CheckNotNull("elementScope", elementScope);

      return FluentUtility.CreateFluentElementScope(elementScope);
    }

    /// <summary>
    /// Starts the fluent screenshot interface for the specified <paramref name="webElement"/>.
    /// </summary>
    public static FluentScreenshotElement<IWebElement> ForWebElementScreenshot (
        [NotNull] this IWebElement webElement,
        [CanBeNull] IFluentScreenshotElement? parent = null,
        [CanBeNull] Rectangle? parentContainer = null)
    {
      ArgumentUtility.CheckNotNull("webElement", webElement);

      return FluentUtility.CreateFluentWebElement(webElement);
    }

    /// <summary>
    /// Scrolls the target element into view.
    /// </summary>
    public static FluentScreenshotElement<T> ScrollIntoView<T> ([NotNull] this FluentScreenshotElement<T> element)
        where T : ElementScope
    {
      ArgumentUtility.CheckNotNull("element", element);

      var elementScope = element.GetTarget();
      JavaScriptExecutor.GetJavaScriptExecutor(elementScope).ExecuteScript("arguments[0].scrollIntoView(true);", elementScope.Native);

      // The scrolling sometimes takes some time - 250ms should be enough.
      Thread.Sleep(250);

      return element;
    }

    /// <summary>
    /// Returns a new <see cref="IFluentScreenshotElement{T}"/> where the <see cref="IFluentScreenshotElement.MinimumElementVisibility"/> is set to
    /// at least <see cref="ElementVisibility"/>.<see cref="ElementVisibility.PartiallyVisible"/>.
    /// </summary>
    public static IFluentScreenshotElement<T> AllowPartialVisibility<T> ([NotNull] this IFluentScreenshotElement<T> fluentElement)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("fluentElement", fluentElement);

      return new AllowPartialVisibilityFluentScreenshotElementDecorator<T>(fluentElement);
    }

    /// <summary>
    /// Returns the target element of the specified <paramref name="fluentElement"/>.
    /// </summary>
    public static T GetTarget<T> ([NotNull] this IFluentScreenshotElement<T> fluentElement)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("fluentElement", fluentElement);

      return fluentElement.Target;
    }

    /// <summary>
    /// Returns the target element of the specified <paramref name="fluentElement"/>.
    /// </summary>
    public static T GetTarget<T> ([NotNull] this IFluentScreenshotElementWithCovariance<T> fluentElement)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("fluentElement", fluentElement);

      return fluentElement.Target;
    }

    /// <summary>
    /// Returns the target element of the specified <paramref name="fluentElement"/>.
    /// </summary>
    public static T GetTarget<T> ([NotNull] this FluentScreenshotElement<T> fluentElement)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("fluentElement", fluentElement);

      return GetTarget((IFluentScreenshotElementWithCovariance<T>)fluentElement);
    }

    /// <summary>
    /// Annotates the specified <paramref name="fluentTarget"/> with a <see cref="ScreenshotBoxAnnotation"/>.
    /// </summary>
    public static void AnnotateBox<T> (
        [NotNull] this ScreenshotBuilder builder,
        [NotNull] IFluentScreenshotElement<T> fluentTarget,
        [CanBeNull] Pen? pen = null,
        [CanBeNull] WebPadding? padding = null,
        [CanBeNull] Brush? brush = null)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("builder", builder);
      ArgumentUtility.CheckNotNull("fluentTarget", fluentTarget);

      IScreenshotAnnotation annotation = new ScreenshotBoxAnnotation(
          pen ?? Pens.Red,
          padding ?? WebPadding.None,
          brush);

      FluentUtility.AnnotateFluent(builder, fluentTarget, annotation);
    }

    /// <summary>
    /// Annotates the specified <paramref name="fluentTarget"/> with a <see cref="ScreenshotBadgeAnnotation"/>.
    /// </summary>
    public static void AnnotateBadge<T> (
        [NotNull] this ScreenshotBuilder builder,
        [NotNull] IFluentScreenshotElement<T> fluentTarget,
        [NotNull] string content,
        [CanBeNull] WebPadding? contentPadding = null,
        [CanBeNull] Font? font = null,
        [CanBeNull] Brush? contentBrush = null,
        [CanBeNull] Pen? borderPen = null,
        [CanBeNull] Brush? backgroundBrush = null,
        [CanBeNull] Size? translation = null,
        [CanBeNull] bool? forceCircle = null)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("builder", builder);
      ArgumentUtility.CheckNotNull("fluentTarget", fluentTarget);
      ArgumentUtility.CheckNotNull("content", content);

      IScreenshotAnnotation annotation = new ScreenshotBadgeAnnotation(
          content,
          contentPadding ?? new WebPadding(3, 3, 3, 0),
          font ?? new Font("Arial", 14),
          contentBrush ?? new SolidBrush(SKColors.White),
          borderPen ?? Pens.White,
          backgroundBrush ?? new SolidBrush(SKColors.Red),
          translation ?? Size.Empty,
          forceCircle ?? true);

      FluentUtility.AnnotateFluent(builder, fluentTarget, annotation);
    }

    /// <summary>
    /// Annotates the specified <paramref name="fluentTarget"/> with a <see cref="ScreenshotTextAnnotation"/>.
    /// </summary>
    public static void AnnotateText<T> (
        [NotNull] this ScreenshotBuilder builder,
        [NotNull] IFluentScreenshotElement<T> fluentTarget,
        [NotNull] string content,
        [CanBeNull] Font? font = null,
        [CanBeNull] Brush? foregroundBrush = null,
        [CanBeNull] Brush? backgroundBrush = null,
        [CanBeNull] StringFormat? stringFormat = null,
        [CanBeNull] ContentAlignment? contentAlignment = null,
        [CanBeNull] WebPadding? padding = null,
        [CanBeNull] float? maxWidth = null,
        [CanBeNull] float? maxHeight = null)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("builder", builder);
      ArgumentUtility.CheckNotNull("fluentTarget", fluentTarget);
      ArgumentUtility.CheckNotNull("content", content);

      IScreenshotAnnotation annotation = new ScreenshotTextAnnotation(
          content,
          font ?? SystemFonts.DefaultFont,
          foregroundBrush ?? new SolidBrush(SKColors.Red),
          backgroundBrush,
          stringFormat ?? StringFormat.GenericDefault,
          contentAlignment ?? ContentAlignment.MiddleCenter,
          padding ?? WebPadding.None,
          maxWidth,
          maxHeight);

      FluentUtility.AnnotateFluent(builder, fluentTarget, annotation);
    }

    /// <summary>
    /// Crops screenshot around the specified <paramref name="fluentTarget"/>.
    /// </summary>
    public static void Crop<T> (
        [NotNull] this ScreenshotBuilder builder,
        [NotNull] IFluentScreenshotElement<T> fluentTarget,
        [CanBeNull] WebPadding? padding = null,
        bool isRestrictedByParent = true,
        bool isRestrictedByImageBounds = true)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("builder", builder);
      ArgumentUtility.CheckNotNull("fluentTarget", fluentTarget);

      var screenshotCropping = new ScreenshotCropping(padding ?? WebPadding.None);
      screenshotCropping.IsRestrictedByParent = isRestrictedByParent;
      screenshotCropping.IsRestrictedByImageBounds = isRestrictedByImageBounds;
      FluentUtility.CropFluent(builder, fluentTarget, screenshotCropping);
    }

    /// <summary>
    /// Free-draws onto the <see cref="Graphics"/> object of the screenshot.
    /// </summary>
    public static void Freedraw ([NotNull] this ScreenshotBuilder builder, [NotNull] Action<Graphics, ResolvedScreenshotElement> drawAction)
    {
      ArgumentUtility.CheckNotNull("builder", builder);
      ArgumentUtility.CheckNotNull("drawAction", drawAction);

      builder.Annotate(new ScreenshotCustomAnnotation(drawAction));
    }

    /// <summary>
    /// Free-draws onto the <see cref="Graphics"/> object of the screenshot, targeting <paramref name="fluentTarget"/>.
    /// </summary>
    public static void Freedraw<T> (
        [NotNull] this ScreenshotBuilder builder,
        [NotNull] IFluentScreenshotElement<T> fluentTarget,
        [NotNull] Action<Graphics, ResolvedScreenshotElement> drawAction)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("builder", builder);
      ArgumentUtility.CheckNotNull("fluentTarget", fluentTarget);
      ArgumentUtility.CheckNotNull("drawAction", drawAction);

      FluentUtility.AnnotateFluent(builder, fluentTarget, new ScreenshotCustomAnnotation(drawAction));
    }
  }
}
