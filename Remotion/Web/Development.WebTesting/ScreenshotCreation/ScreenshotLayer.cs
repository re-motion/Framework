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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Transformations;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Represents a manipulable image layer for a <see cref="Screenshot"/>.
  /// </summary>
  public class ScreenshotLayer : IDisposable
  {
    /// <summary>
    /// Creates a base layer for a given <see cref="Screenshot"/>.
    /// </summary>
    [NotNull]
    public static ScreenshotLayer Create ([NotNull] Screenshot screenshot, [NotNull] IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull("screenshot", screenshot);
      ArgumentUtility.CheckNotNull("locator", locator);

      return new ScreenshotLayer(screenshot, locator);
    }

    /// <summary>
    /// Creates a transparent layer with the size of the given <see cref="Screenshot"/>.
    /// </summary>
    [NotNull]
    public static ScreenshotLayer CreateTransparent ([NotNull] Screenshot screenshot, [NotNull] IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull("screenshot", screenshot);
      ArgumentUtility.CheckNotNull("locator", locator);

      var bitmapOfScreenshotSize = new Bitmap(screenshot.Image.Width, screenshot.Image.Height);
      bitmapOfScreenshotSize.MakeTransparent();

      return new ScreenshotLayer(screenshot, locator, bitmapOfScreenshotSize);
    }

    private readonly IBrowserContentLocator _locator;

    private Image _layerImage;
    private Graphics _layerGraphics;
    private Rectangle _imageBounds;
    private Size _normalizationVector;

    private Size _screenshotOffset;
    private Rectangle[] _screenshotBounds;

    private readonly CoordinateSystem _coordinateSystem;

    private ScreenshotLayer (Screenshot screenshot, IBrowserContentLocator locator)
        : this(screenshot, locator, (Image)screenshot.Image.Clone())
    {
    }

    private ScreenshotLayer (Screenshot screenshot, IBrowserContentLocator locator, Image imageOverride)
    {
      _locator = locator;
      _layerImage = imageOverride;
      _layerGraphics = Graphics.FromImage(_layerImage);

      _screenshotOffset = screenshot.DesktopOffset;
      _screenshotBounds = screenshot.ScreenshotBounds;
      _coordinateSystem = screenshot.CoordinateSystem;

      PrepareScreenshotLayer();
    }

    /// <summary>
    /// Annotates the whole layer with <paramref name="annotation"/>.
    /// </summary>
    public void Annotate ([NotNull] IScreenshotAnnotation annotation)
    {
      ArgumentUtility.CheckNotNull("annotation", annotation);

      var resolvedElement = new ResolvedScreenshotElement(_coordinateSystem, _imageBounds, ElementVisibility.FullyVisible, _imageBounds, _imageBounds);
      annotation.Draw(_layerGraphics, resolvedElement);
    }

    /// <summary>
    /// Annotates <paramref name="target"/> with <paramref name="annotation"/>.
    /// </summary>
    public void Annotate<T> (
        [NotNull] T target,
        [NotNull] IScreenshotElementResolver<T> resolver,
        [NotNull] IScreenshotAnnotation annotation,
        [CanBeNull] IScreenshotTransformation<T>? transformation = null,
        ElementVisibility? minimumElementVisibility = null)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("target", target);
      ArgumentUtility.CheckNotNull("resolver", resolver);
      ArgumentUtility.CheckNotNull("annotation", annotation);

      using (var helper = CreateTransformationHelper(ScreenshotManipulation.Annotate, resolver, target, transformation))
      {
        var context = helper.Context;

        ValidateResolvedElement(context.ResolvedElement, minimumElementVisibility);

        annotation.Draw(context.Graphics, context.ResolvedElement);
      }
    }

    /// <summary>
    /// Crops the <see cref="ScreenshotLayer"/> around <paramref name="target"/> using the specified <paramref name="cropping"/>.
    /// </summary>
    public void Crop<T> (
        [NotNull] T target,
        [NotNull] IScreenshotElementResolver<T> resolver,
        [NotNull] IScreenshotCropping cropping,
        [CanBeNull] IScreenshotTransformation<T>? transformation = null,
        ElementVisibility? minimumElementVisibility = null)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("target", target);
      ArgumentUtility.CheckNotNull("resolver", resolver);
      ArgumentUtility.CheckNotNull("cropping", cropping);


      using (var helper = CreateTransformationHelper(ScreenshotManipulation.Annotate, resolver, target, transformation))
      {
        var context = helper.Context;

        ValidateResolvedElement(context.ResolvedElement, minimumElementVisibility);

        var croppingRegion = cropping.ApplyOnElement(_imageBounds, context.ResolvedElement);

        if (croppingRegion.IsEmpty)
          throw new InvalidOperationException("Cropping must result in an image which is not empty.");

        CropRectangle(croppingRegion);
      }
    }

    /// <summary>
    /// Gets a copy of the image content of the layer.
    /// </summary>
    [NotNull]
    public Image CloneImage ()
    {
      _layerGraphics.Flush();
      return (Image)_layerImage.Clone();
    }

    /// <inheritdoc />
    public void Dispose ()
    {
      _layerImage.Dispose();
      _layerGraphics.Dispose();
    }

    private void PrepareScreenshotLayer ()
    {
      _normalizationVector = new Size(-_screenshotOffset.Width, -_screenshotOffset.Height);

      var transformationMatrix = new Matrix();
      transformationMatrix.Translate(_normalizationVector.Width, _normalizationVector.Height);
      _layerGraphics.Transform = transformationMatrix;

      _imageBounds = new Rectangle(
          _screenshotOffset.Width,
          _screenshotOffset.Height,
          _layerImage.Width,
          _layerImage.Height);
    }

    private void CropRectangle (Rectangle croppingRectangle)
    {
      _layerGraphics.Flush(FlushIntention.Sync);

      var newImage = new Bitmap(croppingRectangle.Size.Width, croppingRectangle.Size.Height);
      var newGraphics = Graphics.FromImage(newImage);
      var newImageBounds = new Rectangle(Point.Empty, croppingRectangle.Size);

      var normalizedCroppingRectangle = new Rectangle(croppingRectangle.Location + _normalizationVector, croppingRectangle.Size);
      newGraphics.FillRectangle(Brushes.Transparent, newImageBounds);
      newGraphics.DrawImage(_layerImage, newImageBounds, normalizedCroppingRectangle, GraphicsUnit.Pixel);

      _screenshotOffset = new Size(croppingRectangle.Location);
      _screenshotBounds = new[] { croppingRectangle };

      _layerImage?.Dispose();
      _layerImage = newImage;

      _layerGraphics?.Dispose();
      _layerGraphics = newGraphics;

      PrepareScreenshotLayer();
    }

    private void ValidateResolvedElement (ResolvedScreenshotElement resolvedElement, ElementVisibility? minimumElementVisibility)
    {
      var requiredVisibility = minimumElementVisibility ?? ElementVisibility.FullyVisible;

      var elementVisibility = resolvedElement.ElementVisibility;
      var elementBounds = resolvedElement.ElementBounds;

      if (elementBounds.Width == 0 || elementBounds.Height == 0)
        throw new InvalidOperationException("The specified target is empty.");

      // ElementVisibility does not account for the screenshot bounds so we need to check the bounds and adapt it.
      // The screenshot bounds can form an arbitrary polygon when monitors are not aligned or have different sizes.
      // A correct bounds check for a polygon is complex, so we take shortcut and assume the element bounds are always
      // fully contained on a single screen and thus a single rectangle in _screenshotBounds.
      var restrictedElementBounds = _screenshotBounds
          .Select(e => Rectangle.Intersect(e, elementBounds))
          .Where(e => !e.IsEmpty)
          .ToArray();

      if (restrictedElementBounds.Length == 0)
      {
        // None of the screenshot bounds intersects the element -> not visible
        elementVisibility = ElementVisibility.NotVisible;
      }
      else if (restrictedElementBounds.Length == 1)
      {
        // Element is on exactly one screen -> partially visible if the bounds changed
        if (elementBounds != restrictedElementBounds[0] && elementVisibility == ElementVisibility.FullyVisible)
          elementVisibility = ElementVisibility.PartiallyVisible;
      }
      else
      {
        throw new InvalidOperationException("Cannot do a bounds check as the resolved element is not fully contained within a single screenshot bound.");
      }

      if (elementVisibility < requiredVisibility)
      {
        throw new InvalidOperationException(
            string.Format(
                "The visibility of the resolved element is smaller than the specified minimum visibility. (visibility: {0}; required: {1})",
                elementVisibility,
                requiredVisibility));
      }
    }

    private ScreenshotTransformationHelper<T> CreateTransformationHelper<T> (
        ScreenshotManipulation manipulation,
        IScreenshotElementResolver<T> resolver,
        T target,
        [CanBeNull] IScreenshotTransformation<T>? transformation)
        where T : notnull
    {
      return new ScreenshotTransformationHelper<T>(
          manipulation,
          _layerGraphics,
          resolver,
          target,
          _coordinateSystem,
          transformation ?? EmptyTransformation<T>.Instance,
          _locator);
    }
  }
}
