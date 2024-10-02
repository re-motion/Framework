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
using Microsoft.Extensions.Logging;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Logging;

/// <summary>
/// Creates instances of <see cref="ILogger"/> that will defer resolving the logger until the first log operation is performed.
/// </summary>
/// <remarks>
/// The <see cref="LazyLoggerFactory"/> relies on the <see cref="IServiceLocator"/> for resolving the <see cref="ILoggerFactory"/>.
/// The resolved <see cref="ILoggerFactory"/> will then create the respective <see cref="ILogger"/> instance.
/// </remarks>
public static class LazyLoggerFactory
{
  /// <summary>
  /// Creates a new <see cref="ILogger"/> instance using the full name of the given <typeparamref name="T"/>.
  /// </summary>
  /// <typeparam name="T">The <see cref="Type"/> used for building the logger's category name.</typeparam>
  /// <returns>An <see cref="LazyLogger"/> wrapping the created <see cref="ILogger"/>.</returns>
  /// <remarks>
  /// The <see cref="ILogger"/> will be resolved via the <see cref="IServiceLocator"/> when the first log operation is performed. 
  /// </remarks>
  public static ILogger CreateLogger<T> ()
  {
    return CreateLogger(typeof(T));
  }

  /// <summary>
  /// Creates a new <see cref="ILogger"/> instance using the full name of the given <paramref name="type"/>.
  /// </summary>
  /// <param name="type">The <see cref="Type"/> used for building the logger's category name.</param>
  /// <returns>An <see cref="LazyLogger"/> wrapping the created <see cref="ILogger"/>.</returns>
  /// <remarks>
  /// The <see cref="ILogger"/> will be resolved via the <see cref="IServiceLocator"/> when the first log operation is performed. 
  /// </remarks>
  public static ILogger CreateLogger (Type type)
  {
    ArgumentUtility.CheckNotNull(nameof(type), type);

    return new LazyLogger(new Lazy<ILogger>(() => SafeServiceLocator.Current.GetInstance<ILoggerFactory>().CreateLogger(type)));
  }

  /// <summary>
  /// Creates a new <see cref="LazyLogger"/> instance using the <paramref name="categoryName"/>. 
  /// </summary>
  /// <param name="categoryName">The category name for messages produced by the logger.</param>
  /// <returns>An <see cref="LazyLogger"/> wrapping the created <see cref="ILogger"/>.</returns>
  /// <remarks>
  /// The <see cref="ILogger"/> will be resolved via the <see cref="IServiceLocator"/> when the first log operation is performed. 
  /// </remarks>
  public static ILogger CreateLogger (string categoryName)
  {
    ArgumentUtility.CheckNotNull(nameof(categoryName), categoryName);

    return new LazyLogger(new Lazy<ILogger>(() => SafeServiceLocator.Current.GetInstance<ILoggerFactory>().CreateLogger(categoryName)));
  }
}
