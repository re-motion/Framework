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
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Resolvers;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Transformations;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent
{
  /// <summary>
  /// Provides helper methods for the fluent screenshot interface.
  /// </summary>
  public static class FluentUtility
  {
    /// <summary>
    /// Provides a shortcut for annotating an <see cref="IFluentScreenshotElement{T}"/> element on a <see cref="ScreenshotBuilder"/>.
    /// </summary>
    public static void AnnotateFluent<T> (
        [NotNull] ScreenshotBuilder builder,
        [NotNull] IFluentScreenshotElement<T> fluentTarget,
        [NotNull] IScreenshotAnnotation annotation)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("builder", builder);
      ArgumentUtility.CheckNotNull("fluentTarget", fluentTarget);
      ArgumentUtility.CheckNotNull("annotation", annotation);

      builder.Annotate(
          fluentTarget,
          FluentResolver<T>.Instance,
          annotation,
          new FluentTransformation<T>(fluentTarget),
          fluentTarget.MinimumElementVisibility);
    }

    /// <summary>
    /// Clones the specified <paramref name="fluentTarget"/> replacing all specified properties.
    /// </summary>
    public static FluentScreenshotElement<T> CloneWith<T> (
        [NotNull] IFluentScreenshotElement<T> fluentTarget,
        OptionalParameter<T> target = default(OptionalParameter<T>),
        [CanBeNull] IScreenshotElementResolver<T>? resolver = null,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("fluentTarget", fluentTarget);
      if (target.HasValue && target.Value == null)
        throw new ArgumentNullException("target", "Value of optional parameter cannot be null.");

      return new FluentScreenshotElement<T>(
          Assertion.IsNotNull(target.GetValueOrDefault(fluentTarget.Target)),
          resolver ?? fluentTarget.Resolver,
          minimumElementVisibility ?? fluentTarget.MinimumElementVisibility);
    }

    /// <summary>
    /// Creates a new <see cref="FluentScreenshotElement{T}"/> using <paramref name="target"/> and <paramref name="resolver"/> 
    /// inheriting all properties that are not overridden from <paramref name="fluentTarget"/>.
    /// </summary>
    public static FluentScreenshotElement<TTarget> CloneFor<TSource, TTarget> (
        [NotNull] IFluentScreenshotElement<TSource> fluentTarget,
        [NotNull] TTarget target,
        [NotNull] IScreenshotElementResolver<TTarget> resolver,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
        where TSource : notnull
        where TTarget : notnull
    {
      ArgumentUtility.CheckNotNull("fluentTarget", fluentTarget);
      ArgumentUtility.CheckNotNull("target", target);
      ArgumentUtility.CheckNotNull("resolver", resolver);

      return new FluentScreenshotElement<TTarget>(
          target,
          resolver,
          minimumElementVisibility ?? fluentTarget.MinimumElementVisibility);
    }

    /// <summary>
    /// Creates a new <see cref="FluentScreenshotElement{T}"/> for an <see cref="AutomationElement"/>.
    /// </summary>
    public static FluentScreenshotElement<AutomationElement> CreateFluentAutomationElement (
        [NotNull] AutomationElement automationElement,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
    {
      ArgumentUtility.CheckNotNull("automationElement", automationElement);

      return new FluentScreenshotElement<AutomationElement>(automationElement, AutomationElementResolver.Instance, minimumElementVisibility);
    }

    /// <summary>
    /// Creates a new <see cref="FluentScreenshotElement{T}"/> for a <see cref="ControlObject"/>.
    /// </summary>
    public static FluentScreenshotElement<T> CreateFluentControlObject<T> (
        [NotNull] T controlObject,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
        where T : ControlObject
    {
      ArgumentUtility.CheckNotNull("controlObject", controlObject);

      return new FluentScreenshotElement<T>(controlObject, ControlObjectResolver.Instance, minimumElementVisibility);
    }

    /// <summary>
    /// Creates a new <see cref="FluentScreenshotElement{T}"/> for an <see cref="ElementScope"/>.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> CreateFluentElementScope (
        [NotNull] ElementScope element,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
    {
      ArgumentUtility.CheckNotNull("element", element);

      return new FluentScreenshotElement<ElementScope>(element, ElementScopeResolver.Instance, minimumElementVisibility);
    }

    /// <summary>
    /// Creates a new <see cref="FluentScreenshotElement{T}"/> for a <see cref="Rectangle"/>.
    /// </summary>
    public static FluentScreenshotElement<Rectangle> CreateFluentRectangle (
        Rectangle rectangle,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
    {
      return new FluentScreenshotElement<Rectangle>(rectangle, RectangleResolver.Instance, minimumElementVisibility);
    }

    /// <summary>
    /// Creates a new <see cref="FluentScreenshotElement{T}"/> for an <see cref="IWebElement"/>.
    /// </summary>
    public static FluentScreenshotElement<IWebElement> CreateFluentWebElement (
        [NotNull] IWebElement webElement,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
    {
      ArgumentUtility.CheckNotNull("webElement", webElement);

      return new FluentScreenshotElement<IWebElement>(webElement, WebElementResolver.Instance, minimumElementVisibility);
    }

    /// <summary>
    /// Provides a shortcut for cropping an <see cref="IFluentScreenshotElement{T}"/> element on a <see cref="ScreenshotBuilder"/>.
    /// </summary>
    public static void CropFluent<T> (
        [NotNull] ScreenshotBuilder builder,
        [NotNull] IFluentScreenshotElement<T> fluentTarget,
        [NotNull] IScreenshotCropping cropping)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("builder", builder);
      ArgumentUtility.CheckNotNull("fluentTarget", fluentTarget);
      ArgumentUtility.CheckNotNull("cropping", cropping);

      builder.Crop(fluentTarget, FluentResolver<T>.Instance, cropping, new FluentTransformation<T>(fluentTarget));
    }
  }
}
