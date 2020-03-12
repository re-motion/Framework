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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Transformations;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides means of annotating or cropping a <see cref="Screenshot"/>.
  /// </summary>
  public class ScreenshotBuilder
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (ScreenshotBuilder));

    private class TransformationHelper<T> : IDisposable
    {
      private readonly ScreenshotTransformationContext<T> _context;
      private readonly IScreenshotTransformation<T> _transformation;

      public TransformationHelper (
          ScreenshotManipulation manipulation,
          Graphics graphics,
          IScreenshotElementResolver<T> resolver,
          T target,
          CoordinateSystem coordinateSystem,
          IScreenshotTransformation<T> transformation,
          IBrowserContentLocator locator)
      {
        var resolvedElement = Resolve (resolver, target, locator, coordinateSystem);
        var context = new ScreenshotTransformationContext<T> (manipulation, graphics, resolver, target, resolvedElement);

        context = transformation.BeginApply (context);

        var parentBounds = context.ResolvedElement.ParentBounds;
        if (parentBounds.HasValue)
        {
          _context =
              context.CloneWith (
                  resolvedElement:
                      resolvedElement.CloneWith (elementBounds: Rectangle.Intersect (parentBounds.Value, context.ResolvedElement.ElementBounds)));
        }
        else
        {
          _context = context;
        }

        _transformation = transformation;
      }

      public ScreenshotTransformationContext<T> Context
      {
        get { return _context; }
      }

      public void Dispose ()
      {
        _transformation.EndApply (_context);
      }

      private ResolvedScreenshotElement Resolve (
          IScreenshotElementResolver<T> resolver,
          T target,
          IBrowserContentLocator locator,
          CoordinateSystem coordinateSystem)
      {
        switch (coordinateSystem)
        {
          case CoordinateSystem.Browser:
            return resolver.ResolveBrowserCoordinates (target);
          case CoordinateSystem.Desktop:
            return resolver.ResolveDesktopCoordinates (target, locator);
          default:
            throw new ArgumentOutOfRangeException ("coordinateSystem", coordinateSystem, null);
        }
      }
    }

    private readonly IBrowserContentLocator _locator;

    private Graphics _graphics;
    private Rectangle _imageBounds;
    private Size _normalizationVector;
    private Screenshot _screenshot;

    private bool _drawMouseCursor;
    private ElementVisibility _minimumVisibility;

    public ScreenshotBuilder ([NotNull] Screenshot screenshot, [NotNull] IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull ("screenshot", screenshot);
      ArgumentUtility.CheckNotNull ("locator", locator);

      _screenshot = screenshot;
      _locator = locator;

      _graphics = Graphics.FromImage (_screenshot.Image);

      _drawMouseCursor = false;
      _minimumVisibility = ElementVisibility.FullyVisible;

      PrepareScreenshot();
    }

    /// <summary>
    /// Returns <see langword="true" /> if the mouse cursor should be drawn onto the screenshot, otherwise <see langword="false" />.
    /// </summary>
    public bool DrawMouseCursor
    {
      get { return _drawMouseCursor; }
      set { _drawMouseCursor = value; }
    }

    /// <summary>
    /// Returns the minimum <see cref="ElementVisibility"/> a screenshot element must have in order to be annotated / cropped.
    /// </summary>
    public ElementVisibility MinimumVisibility
    {
      get { return _minimumVisibility; }
      set { _minimumVisibility = value; }
    }

    /// <summary>
    /// The <see cref="ScreenshotCreation.Screenshot"/> behind the <see cref="ScreenshotBuilder"/> instance.
    /// </summary>
    public Screenshot Screenshot
    {
      get { return _screenshot; }
    }

    /// <summary>
    /// Annotates the whole image with <paramref name="annotation"/>.
    /// </summary>
    public ScreenshotBuilder Annotate ([NotNull] IScreenshotAnnotation annotation)
    {
      ArgumentUtility.CheckNotNull ("annotation", annotation);

      var resolvedElement = new ResolvedScreenshotElement (_screenshot.CoordinateSystem, _imageBounds, ElementVisibility.FullyVisible, _imageBounds);
      annotation.Draw (_graphics, resolvedElement);

      return this;
    }

    /// <summary>
    /// Annotates <paramref name="target"/> with <paramref name="annotation"/>.
    /// </summary>
    public ScreenshotBuilder Annotate<T> (
        [NotNull] T target,
        [NotNull] IScreenshotElementResolver<T> resolver,
        [NotNull] IScreenshotAnnotation annotation,
        [CanBeNull] IScreenshotTransformation<T> transformation = null,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("resolver", resolver);
      ArgumentUtility.CheckNotNull ("annotation", annotation);

      using (var helper = CreateTransformationHelper (ScreenshotManipulation.Annotate, resolver, target, transformation))
      {
        var context = helper.Context;

        ValidateResolvedElement (context.ResolvedElement, minimumElementVisibility);

        annotation.Draw (context.Graphics, context.ResolvedElement);
      }

      return this;
    }

    /// <summary>
    /// Crops the screenshot around <paramref name="target"/> using the specified <paramref name="cropping"/>.
    /// </summary>
    public ScreenshotBuilder Crop<T> (
        [NotNull] T target,
        [NotNull] IScreenshotElementResolver<T> resolver,
        [NotNull] IScreenshotCropping cropping,
        [CanBeNull] IScreenshotTransformation<T> transformation = null,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("resolver", resolver);
      ArgumentUtility.CheckNotNull ("cropping", cropping);


      using (var helper = CreateTransformationHelper (ScreenshotManipulation.Annotate, resolver, target, transformation))
      {
        var context = helper.Context;

        ValidateResolvedElement (context.ResolvedElement, minimumElementVisibility);

        var croppingRegion = cropping.ApplyOnElement (_imageBounds, context.ResolvedElement);

        if (croppingRegion.IsEmpty)
          throw new InvalidOperationException ("Cropping must result in an image which is not empty.");

        CropRectangle (croppingRegion);
      }

      return this;
    }

    /// <summary>
    /// Saves the screenshot with all applied annotations and/or cropping under <paramref name="path"/>.
    /// </summary>
    public void Save ([NotNull] string path, bool @override = false)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);

      if (_drawMouseCursor && _screenshot.CursorInformation.IsVisible)
        _screenshot.CursorInformation.Draw (_graphics);

      var isFileExisting = File.Exists (path);

      if (!@override && isFileExisting)
        throw new InvalidOperationException (string.Format ("A screenshot with the file name '{0}' does already exist.", path));

      if (isFileExisting)
        s_log.InfoFormat ("Overwriting existing screenshot with file name '{0}'.", path);

      var directory = Path.GetDirectoryName (path);
      if (Directory.Exists (directory))
        Directory.CreateDirectory (directory);

      _graphics.Flush();
      _screenshot.Image.Save (path, ImageFormat.Png);
    }

    private TransformationHelper<T> CreateTransformationHelper<T> (
        ScreenshotManipulation manipulation,
        IScreenshotElementResolver<T> resolver,
        T target,
        [CanBeNull] IScreenshotTransformation<T> transformation)
    {
      return new TransformationHelper<T> (
          manipulation,
          _graphics,
          resolver,
          target,
          _screenshot.CoordinateSystem,
          transformation ?? EmptyTransformation<T>.Instance,
          _locator);
    }

    private void CropRectangle (Rectangle croppingRectangle)
    {
      var image = _screenshot.Image;

      var newImage = new Bitmap (image, croppingRectangle.Size);
      var newGraphics = Graphics.FromImage (newImage);

      if (_graphics != null)
      {
        _graphics.Flush();
        _graphics.Dispose();
        _graphics = null;
      }

      var newImageBounds = new Rectangle (Point.Empty, croppingRectangle.Size);

      var normalizedCroppingRectangle = new Rectangle (croppingRectangle.Location + _normalizationVector, croppingRectangle.Size);
      newGraphics.FillRectangle (Brushes.Transparent, newImageBounds);
      newGraphics.DrawImage (image, newImageBounds, normalizedCroppingRectangle, GraphicsUnit.Pixel);

      var newScreenshot = new Screenshot (
          newImage,
          new Size (croppingRectangle.Location),
          new[] { croppingRectangle },
          _screenshot.CursorInformation,
          _screenshot.CoordinateSystem);

      _screenshot.Dispose();
      _screenshot = newScreenshot;
      _graphics = newGraphics;

      PrepareScreenshot();
    }

    private void PrepareScreenshot ()
    {
      _normalizationVector = new Size (-_screenshot.DesktopOffset.Width, -_screenshot.DesktopOffset.Height);

      var transformationMatrix = new Matrix();
      transformationMatrix.Translate (_normalizationVector.Width, _normalizationVector.Height);
      _graphics.Transform = transformationMatrix;

      _imageBounds = new Rectangle (
          _screenshot.DesktopOffset.Width,
          _screenshot.DesktopOffset.Height,
          _screenshot.Image.Width,
          _screenshot.Image.Height);
    }

    private void ValidateResolvedElement (ResolvedScreenshotElement resolvedElement, ElementVisibility? minimumElementVisibility)
    {
      var visibility = minimumElementVisibility ?? _minimumVisibility;

      if (resolvedElement.ElementVisibility < visibility)
      {
        throw new InvalidOperationException (
            string.Format (
                "The visibility of the resolved element is smaller than the specified minimum visibility. (visibility: {0}; required: {1})",
                resolvedElement.ElementVisibility,
                _minimumVisibility));
      }

      if (resolvedElement.ElementBounds.Width == 0 || resolvedElement.ElementBounds.Height == 0)
        throw new InvalidOperationException ("The specified target is empty.");

      if (!_screenshot.Contains (resolvedElement.ElementBounds))
        throw new InvalidOperationException ("The specified target is not part of the screenshot.");
    }
  }
}