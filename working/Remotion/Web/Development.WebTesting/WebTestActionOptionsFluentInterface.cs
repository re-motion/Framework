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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ModalDialogHandlers;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Entry point of the fluent builder interface for <see cref="IWebTestActionOptions"/> objects.
  /// </summary>
  public static class Opt
  {
    /// <summary>
    /// See <see cref="IWebTestActionOptionsFluentInterface.ContinueWhen"/> for more information.
    /// </summary>
    public static WebTestActionOptionsFluentInterface ContinueWhen ([NotNull] ICompletionDetectionStrategy completionDetectionStrategy)
    {
      return new WebTestActionOptionsFluentInterface().ContinueWhen (completionDetectionStrategy);
    }

    /// <summary>
    /// See <see cref="IWebTestActionOptionsFluentInterface.ContinueWhenAll"/> for more information.
    /// </summary>
    public static WebTestActionOptionsFluentInterface ContinueWhenAll ([NotNull] params ICompletionDetectionStrategy[] completionDetectionStrategies)
    {
      return new WebTestActionOptionsFluentInterface().ContinueWhenAll (completionDetectionStrategies);
    }

    /// <summary>
    /// See <see cref="IWebTestActionOptionsFluentInterface.ContinueImmediately"/> for more information.
    /// </summary>
    public static WebTestActionOptionsFluentInterface ContinueImmediately ()
    {
      return new WebTestActionOptionsFluentInterface().ContinueImmediately();
    }

    /// <summary>
    /// See <see cref="IWebTestActionOptionsFluentInterface.AcceptModalDialog"/> for more information.
    /// </summary>
    public static WebTestActionOptionsFluentInterface AcceptModalDialog ()
    {
      return new WebTestActionOptionsFluentInterface().AcceptModalDialog();
    }

    /// <summary>
    /// See <see cref="IWebTestActionOptionsFluentInterface.CancelModalDialog"/> for more information.
    /// </summary>
    public static WebTestActionOptionsFluentInterface CancelModalDialog ()
    {
      return new WebTestActionOptionsFluentInterface().CancelModalDialog();
    }
  }

  /// <summary>
  /// Fluent builder interface for <see cref="IWebTestActionOptions"/> objects.
  /// </summary>
  public interface IWebTestActionOptionsFluentInterface
  {
    // Note: The Options class should implement all methods statically.

    /// <summary>
    /// Specifies the <paramref name="completionDetectionStrategy"/> to use.
    /// </summary>
    WebTestActionOptionsFluentInterface ContinueWhen ([NotNull] ICompletionDetectionStrategy completionDetectionStrategy);

    /// <summary>
    /// Specifies multiple <paramref name="completionDetectionStrategies"/> to use.
    /// </summary>
    WebTestActionOptionsFluentInterface ContinueWhenAll ([NotNull] params ICompletionDetectionStrategy[] completionDetectionStrategies);

    /// <summary>
    /// Specifies that the <see cref="NullCompletionDetectionStrategy"/> should be used.
    /// </summary>
    WebTestActionOptionsFluentInterface ContinueImmediately ();

    /// <summary>
    /// Specifies to accept the modal browser dialog.
    /// </summary>
    WebTestActionOptionsFluentInterface AcceptModalDialog ();

    /// <summary>
    /// Specifies to cancel the modal browser dialog.
    /// </summary>
    WebTestActionOptionsFluentInterface CancelModalDialog ();
  }

  /// <summary>
  /// Implementation class of the fluent builder interface for <see cref="IWebTestActionOptions"/> objects.
  /// </summary>
  public class WebTestActionOptionsFluentInterface : IWebTestActionOptionsFluentInterface, IWebTestActionOptions
  {
    private readonly WebTestActionOptions _actionOptions = new WebTestActionOptions();

    /// <inheritdoc/>
    public WebTestActionOptionsFluentInterface ContinueWhen ([NotNull] ICompletionDetectionStrategy completionDetectionStrategy)
    {
      ArgumentUtility.CheckNotNull ("completionDetectionStrategy", completionDetectionStrategy);
      Assertion.IsNull (_actionOptions.CompletionDetectionStrategy, "You cannot specify multiple completion detector strategies.");

      _actionOptions.CompletionDetectionStrategy = completionDetectionStrategy;

      return this;
    }

    /// <inheritdoc/>
    public WebTestActionOptionsFluentInterface ContinueWhenAll (params ICompletionDetectionStrategy[] completionDetectionStrategies)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("completionDetectionStrategies", completionDetectionStrategies);
      Assertion.IsNull (_actionOptions.CompletionDetectionStrategy, "You cannot completion detection strategies multiple times.");

      _actionOptions.CompletionDetectionStrategy = new CompoundCompletionDetectionStrategy (completionDetectionStrategies);

      return this;
    }

    /// <inheritdoc/>
    public WebTestActionOptionsFluentInterface ContinueImmediately ()
    {
      Assertion.IsNull (_actionOptions.CompletionDetectionStrategy, "You cannot specify multiple completion detector strategies.");

      _actionOptions.CompletionDetectionStrategy = new NullCompletionDetectionStrategy();

      return this;
    }

    /// <inheritdoc/>
    public WebTestActionOptionsFluentInterface AcceptModalDialog ()
    {
      Assertion.IsNull (_actionOptions.ModalDialogHandler, "You cannot specify multiple modal dialog handlers.");

      _actionOptions.ModalDialogHandler = new AcceptModalDialogHandler();

      return this;
    }

    /// <inheritdoc/>
    public WebTestActionOptionsFluentInterface CancelModalDialog ()
    {
      Assertion.IsNull (_actionOptions.ModalDialogHandler, "You cannot specify multiple modal dialog handlers.");

      _actionOptions.ModalDialogHandler = new CancelModalDialogHandler();

      return this;
    }

    /// <inheritdoc/>
    ICompletionDetectionStrategy IWebTestActionOptions.CompletionDetectionStrategy
    {
      get { return _actionOptions.CompletionDetectionStrategy; }
      set { _actionOptions.CompletionDetectionStrategy = value; }
    }

    /// <inheritdoc/>
    IModalDialogHandler IWebTestActionOptions.ModalDialogHandler
    {
      get { return _actionOptions.ModalDialogHandler; }
      set { _actionOptions.ModalDialogHandler = value; }
    }
  }
}