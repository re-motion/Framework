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
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class for all control objects. Much like <see cref="PageObject"/>s, control objects hide the actual HTML structure from the web test
  /// developer and provide a semantic interface instead. In contrast to <see cref="PageObject"/>s, control objects represent a specific
  /// ASP.NET (custom) control and not a whole page.
  /// </summary>
  public abstract class ControlObject : WebTestObject<ControlObjectContext>
  {
    protected ControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the control's HTML ID.
    /// </summary>
    /// <exception cref="MissingHtmlException">If the <see cref="Scope"/>'s root DOM element does not have an ID attribute.</exception>
    public string GetHtmlID ()
    {
      return Scope.Id;
    }

    /// <summary>
    /// Allows accessing child controls of the control.
    /// </summary>
    public IControlHost Children
    {
      get { return new ControlHost (Context); }
    }

    /// <summary>
    /// Merges the <paramref name="userDefinedWebTestActionOptions"/> with the given <paramref name="scope"/>'s default
    /// <see cref="IWebTestActionOptions"/>.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> with which the subsequent action is going to interact.</param>
    /// <param name="userDefinedWebTestActionOptions">User-defined <see cref="IWebTestActionOptions"/>.</param>
    protected IWebTestActionOptions MergeWithDefaultActionOptions (
        [NotNull] ElementScope scope,
        [CanBeNull] IWebTestActionOptions userDefinedWebTestActionOptions)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      if (userDefinedWebTestActionOptions == null) // prevent complicated null handling
        userDefinedWebTestActionOptions = new WebTestActionOptions();

      return new WebTestActionOptions
             {
                 CompletionDetectionStrategy =
                     userDefinedWebTestActionOptions.CompletionDetectionStrategy ?? GetDefaultCompletionDetectionStrategy (scope),
                 ModalDialogHandler = userDefinedWebTestActionOptions.ModalDialogHandler
             };
    }

    /// <summary>
    /// Returns a sensible default <see cref="ICompletionDetectionStrategy"/> for the given <paramref name="scope"/>. This allows the user to omit
    /// passing a user-defined strategy in most cases.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> with which the subsequent action is going to interact.</param>
    /// <returns>A sensible default <see cref="ICompletionDetectionStrategy"/> or <see langword="null" /> if none exists.</returns>
    protected abstract ICompletionDetectionStrategy GetDefaultCompletionDetectionStrategy ([NotNull] ElementScope scope);

    /// <summary>
    /// Convinience method for creating a new <see cref="UnspecifiedPageObject"/>.
    /// </summary>
    protected UnspecifiedPageObject UnspecifiedPage ()
    {
      return new UnspecifiedPageObject (Context);
    }
  }
}