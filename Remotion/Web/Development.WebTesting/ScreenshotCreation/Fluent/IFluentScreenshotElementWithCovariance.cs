using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent
{
  /// <summary>
  /// Represents an object of type <typeparamref name="T"/> in the fluent screenshot API.
  /// </summary>
  public interface IFluentScreenshotElementWithCovariance<out T>
  {
    /// <summary>
    /// The underlying object that is wrapped by this <see cref="IFluentScreenshotElementWithCovariance{T}"/>.
    /// </summary>
    [NotNull]
    T Target { get; }
  }
}