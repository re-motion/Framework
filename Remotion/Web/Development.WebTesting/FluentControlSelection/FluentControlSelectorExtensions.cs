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
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Default extension methods for all re-motion-provided <see cref="IControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>
  /// implementations.
  /// </summary>
  public static class FluentControlSelectorExtensions
  {
    /// <summary>
    /// Extension method for selecting a control by HTML ID (using the
    /// <see cref="HtmlIDControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>).
    /// </summary>
    public static TControlObject ByID<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string id)
        where TControlSelector : IHtmlIDControlSelector<TControlObject> where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      return fluentControlSelector.GetControl (new HtmlIDControlSelectionCommandBuilder<TControlSelector, TControlObject> (id));
    }

    /// <summary>
    /// Extension method for selecting a control by index (using the
    /// <see cref="IndexControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>).
    /// </summary>
    public static TControlObject ByIndex<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        int index)
        where TControlSelector : IIndexControlSelector<TControlObject> where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);

      return fluentControlSelector.GetControl (new IndexControlSelectionCommandBuilder<TControlSelector, TControlObject> (index));
    }

    /// <summary>
    /// Extension method for selecting a control by its local ID (using the
    /// <see cref="LocalIDControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>).
    /// </summary>
    public static TControlObject ByLocalID<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string localID)
        where TControlSelector : ILocalIDControlSelector<TControlObject> where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("localID", localID);

      return fluentControlSelector.GetControl (new LocalIDControlSelectionCommandBuilder<TControlSelector, TControlObject> (localID));
    }

    /// <summary>
    /// Extension method for selecting a control by title (using the
    /// <see cref="TitleControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>).
    /// </summary>
    public static TControlObject ByTitle<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string title)
        where TControlSelector : ITitleControlSelector<TControlObject> where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("title", title);

      return fluentControlSelector.GetControl (new TitleControlSelectionCommandBuilder<TControlSelector, TControlObject> (title));
    }

    /// <summary>
    /// Extension method for selecting the first matching control (using the
    /// <see cref="FirstControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>).
    /// </summary>
    public static TControlObject First<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector)
        where TControlSelector : IFirstControlSelector<TControlObject> where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);

      return fluentControlSelector.GetControl (new FirstControlSelectionCommandBuilder<TControlSelector, TControlObject>());
    }

    /// <summary>
    /// Extension method for selecting the only matching control (using the
    /// <see cref="SingleControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>).
    /// </summary>
    public static TControlObject Single<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector)
        where TControlSelector : ISingleControlSelector<TControlObject> where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);

      return fluentControlSelector.GetControl (new SingleControlSelectionCommandBuilder<TControlSelector, TControlObject>());
    }

    /// <summary>
    /// Extension method for selecting a control by text (using the
    /// <see cref="TextContentControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>).
    /// </summary>
    public static TControlObject ByTextContent<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string text)
        where TControlSelector : ITextContentControlSelector<TControlObject> where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return fluentControlSelector.GetControl (new TextContentControlSelectionCommandBuilder<TControlSelector, TControlObject> (text));
    }
  }
}