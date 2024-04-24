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
using System.Drawing.Imaging;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides means of annotating or cropping a <see cref="Screenshot"/>.
  /// </summary>
  public class ScreenshotBuilder : IDisposable
  {
    private static readonly ILogger s_logger = LogManager.GetLogger<ScreenshotBuilder>();

    /// <summary>
    /// Returns <see langword="true" /> if the mouse cursor should be drawn onto the screenshot, otherwise <see langword="false" />.
    /// </summary>
    public bool DrawMouseCursor { get; set; }

    /// <summary>
    /// Returns the minimum <see cref="ElementVisibility"/> a screenshot element must have in order to be annotated / cropped.
    /// </summary>
    public ElementVisibility MinimumVisibility { get; set; }

    /// <summary>
    /// The <see cref="ScreenshotCreation.Screenshot"/> behind the <see cref="ScreenshotBuilder"/> instance.
    /// </summary>
    [NotNull]
    public Screenshot Screenshot { get; }

    /// <summary>
    /// The base layer of the image, containing the screenshot.
    /// </summary>
    [NotNull]
    protected ScreenshotLayer BaseLayer { get; }

    /// <summary>
    /// The annotation layer of the image, containing the annotations.
    /// </summary>
    [NotNull]
    protected ScreenshotLayer AnnotationLayer { get; }

    public ScreenshotBuilder ([NotNull] Screenshot screenshot, [NotNull] IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull("screenshot", screenshot);
      ArgumentUtility.CheckNotNull("locator", locator);

      Screenshot = screenshot;
      DrawMouseCursor = false;
      MinimumVisibility = ElementVisibility.FullyVisible;

      BaseLayer = ScreenshotLayer.Create(screenshot, locator);
      AnnotationLayer = ScreenshotLayer.CreateTransparent(screenshot, locator);
    }

    /// <summary>
    /// Annotates the whole image with <paramref name="annotation"/>.
    /// </summary>
    [NotNull]
    public ScreenshotBuilder Annotate ([NotNull] IScreenshotAnnotation annotation)
    {
      ArgumentUtility.CheckNotNull("annotation", annotation);

      AnnotationLayer.Annotate(annotation);

      return this;
    }

    /// <summary>
    /// Annotates <paramref name="target"/> with <paramref name="annotation"/>.
    /// </summary>
    [NotNull]
    public ScreenshotBuilder Annotate<T> (
        [NotNull] T target,
        [NotNull] IScreenshotElementResolver<T> resolver,
        [NotNull] IScreenshotAnnotation annotation,
        [CanBeNull] IScreenshotTransformation<T>? transformation = null,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("target", target);
      ArgumentUtility.CheckNotNull("resolver", resolver);
      ArgumentUtility.CheckNotNull("annotation", annotation);

      AnnotationLayer.Annotate(target, resolver, annotation, transformation, minimumElementVisibility);

      return this;
    }

    /// <summary>
    /// Crops the screenshot around <paramref name="target"/> using the specified <paramref name="cropping"/>.
    /// </summary>
    [NotNull]
    public ScreenshotBuilder Crop<T> (
        [NotNull] T target,
        [NotNull] IScreenshotElementResolver<T> resolver,
        [NotNull] IScreenshotCropping cropping,
        [CanBeNull] IScreenshotTransformation<T>? transformation = null,
        [CanBeNull] ElementVisibility? minimumElementVisibility = null)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("target", target);
      ArgumentUtility.CheckNotNull("resolver", resolver);
      ArgumentUtility.CheckNotNull("cropping", cropping);

      BaseLayer.Crop(target, resolver, cropping, transformation, minimumElementVisibility);
      AnnotationLayer.Crop(target, resolver, cropping, transformation, minimumElementVisibility);

      return this;
    }

    /// <summary>
    /// Saves the screenshot with all applied annotations and/or cropping under <paramref name="path"/>.
    /// </summary>
    public void Save ([NotNull] string path, bool @override = false)
    {
      ArgumentUtility.CheckNotNullOrEmpty("path", path);

      var isFileExisting = File.Exists(path);

      if (!@override && isFileExisting)
        throw new InvalidOperationException(string.Format("A screenshot with the file name '{0}' does already exist.", path));

      if (isFileExisting)
        s_logger.LogInformation("Overwriting existing screenshot with file name '{0}'.", path);

      var directory = Path.GetDirectoryName(path);
      if (directory != null)
        Directory.CreateDirectory(directory);

      using (var annotationImage = AnnotationLayer.CloneImage())
      using (var outputImage = BaseLayer.CloneImage())
      using (var outputGraphics = Graphics.FromImage(outputImage))
      {
        outputGraphics.DrawImage(annotationImage, Point.Empty);

        if (DrawMouseCursor && Screenshot.CursorInformation.IsVisible)
          Screenshot.CursorInformation.Draw(outputGraphics);

        outputImage.Save(path, ImageFormat.Png);
      }
    }

    /// <inheritdoc />
    public void Dispose ()
    {
      Screenshot.Dispose();
      BaseLayer.Dispose();
      AnnotationLayer.Dispose();
    }
  }
}
