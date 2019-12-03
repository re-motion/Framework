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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Default extension methods for all re-motion provided <see cref="IControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>
  /// implementations.
  /// </summary>
  public static class FluentControlSelectorExtensions
  {
    /// <summary>
    /// Extension method for selecting a control by HTML <paramref name="id"/>.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control.</returns>
    /// <exception cref="WebTestException">If the control cannot be found.</exception>
    /// <remarks>
    /// Uses the <see cref="HtmlIDControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [NotNull]
    public static TControlObject GetByID<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string id)
        where TControlSelector : IHtmlIDControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      return fluentControlSelector.GetControl (new HtmlIDControlSelectionCommandBuilder<TControlSelector, TControlObject> (id));
    }

    /// <summary>
    /// Extension method for selecting a control by HTML <paramref name="id"/>.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control, or <see langword="null"/> if no control could be found.</returns>
    /// <remarks>
    /// Uses the <see cref="HtmlIDControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [CanBeNull]
    public static TControlObject GetByIDOrNull<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string id)
        where TControlSelector : IHtmlIDControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      return fluentControlSelector.GetControlOrNull (new HtmlIDControlSelectionCommandBuilder<TControlSelector, TControlObject> (id));
    }

    /// <summary>
    /// Extension method for checking if a control with the given HTML <paramref name="id"/> exists.
    /// </summary>
    /// <returns><see langword="true" /> if a control has been found; otherwise, <see langword="false" />.</returns>
    /// <exception cref="WebTestException">If the control cannot be found due to ambiguity.</exception>
    /// <remarks>
    /// Uses the <see cref="HtmlIDControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    public static bool ExistsByID<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string id)
        where TControlSelector : IHtmlIDControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      return fluentControlSelector.HasControl (new HtmlIDControlSelectionCommandBuilder<TControlSelector, TControlObject> (id));
    }

    /// <summary>
    /// Extension method for selecting a control by <paramref name="oneBasedIndex"/>.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control.</returns>
    /// <exception cref="WebTestException">If the control cannot be found.</exception>
    /// <remarks>
    /// Uses the <see cref="IndexControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [NotNull]
    public static TControlObject GetByIndex<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        int oneBasedIndex)
        where TControlSelector : IIndexControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);

      return fluentControlSelector.GetControl (new IndexControlSelectionCommandBuilder<TControlSelector, TControlObject> (oneBasedIndex));
    }

    /// <summary>
    /// Extension method for selecting a control by <paramref name="oneBasedIndex"/>.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control, or <see langword="null"/> if no control could be found.</returns>
    /// <remarks>
    /// Uses the <see cref="IndexControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [CanBeNull]
    public static TControlObject GetByIndexOrNull<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        int oneBasedIndex)
        where TControlSelector : IIndexControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);

      return fluentControlSelector.GetControlOrNull (new IndexControlSelectionCommandBuilder<TControlSelector, TControlObject> (oneBasedIndex));
    }

    /// <summary>
    /// Extension method for checking if a control with the given <paramref name="oneBasedIndex"/> exists.
    /// </summary>
    /// <returns><see langword="true" /> if a control has been found; otherwise, <see langword="false" />.</returns>
    /// <exception cref="WebTestException">If the control cannot be found due to ambiguity.</exception>
    /// <remarks>
    /// Uses the <see cref="IndexControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    public static bool ExistsByIndex<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        int oneBasedIndex)
        where TControlSelector : IIndexControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);

      return fluentControlSelector.HasControl (new IndexControlSelectionCommandBuilder<TControlSelector, TControlObject> (oneBasedIndex));
    }

    /// <summary>
    /// Extension method for selecting a control by its <paramref name="localID"/>.
    /// Note that selecting a control via the local ID can have inferior performance compared to other means of selection, 
    /// e.g. <see cref="GetByID{TControlSelector,TControlObject}"/>.
    /// </summary>
    /// <returns>The control object for the selected control.</returns>
    /// <exception cref="WebTestException">If the control cannot be found.</exception>
    /// <remarks>
    /// Uses the <see cref="LocalIDControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [NotNull]
    public static TControlObject GetByLocalID<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string localID)
        where TControlSelector : ILocalIDControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("localID", localID);

      return fluentControlSelector.GetControl (new LocalIDControlSelectionCommandBuilder<TControlSelector, TControlObject> (localID));
    }

    /// <summary>
    /// Extension method for selecting a control by its <paramref name="localID"/>.
    /// Note that selecting a control via the local ID can have inferior performance compared to other means of selection, 
    /// e.g. <see cref="GetByIDOrNull{TControlSelector,TControlObject}"/>.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control, or <see langword="null"/> if no control could be found.</returns>
    /// <remarks>
    /// Uses the <see cref="LocalIDControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [CanBeNull]
    public static TControlObject GetByLocalIDOrNull<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string localID)
        where TControlSelector : ILocalIDControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("localID", localID);

      return fluentControlSelector.GetControlOrNull (new LocalIDControlSelectionCommandBuilder<TControlSelector, TControlObject> (localID));
    }

    /// <summary>
    /// Extension method for checking if a control with the given <paramref name="localID"/> exists.
    /// Note that checking for the existence of a control via the local ID can have inferior performance compared to other means of existence checking, 
    /// e.g. <see cref="ExistsByID{TControlSelector,TControlObject}"/>.
    /// </summary>
    /// <returns><see langword="true" /> if a control has been found; otherwise, <see langword="false" />.</returns>
    /// <exception cref="WebTestException">If the control cannot be found due to ambiguity.</exception>
    /// <remarks>
    /// Uses the <see cref="LocalIDControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    public static bool ExistsByLocalID<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string localID)
        where TControlSelector : ILocalIDControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("localID", localID);

      return fluentControlSelector.HasControl (new LocalIDControlSelectionCommandBuilder<TControlSelector, TControlObject> (localID));
    }

    /// <summary>
    /// Extension method for selecting a control by <paramref name="title"/>.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control.</returns>
    /// <exception cref="WebTestException">If the control cannot be found.</exception>
    /// <remarks>
    /// Uses the <see cref="TitleControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [NotNull]
    public static TControlObject GetByTitle<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string title)
        where TControlSelector : ITitleControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("title", title);

      return fluentControlSelector.GetControl (new TitleControlSelectionCommandBuilder<TControlSelector, TControlObject> (title));
    }

    /// <summary>
    /// Extension method for selecting a control by <paramref name="title"/>.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control, or <see langword="null"/> if no control could be found.</returns>
    /// <remarks>
    /// Uses the <see cref="TitleControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [CanBeNull]
    public static TControlObject GetByTitleOrNull<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string title)
        where TControlSelector : ITitleControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("title", title);

      return fluentControlSelector.GetControlOrNull (new TitleControlSelectionCommandBuilder<TControlSelector, TControlObject> (title));
    }

    /// <summary>
    /// Extension method for checking if a control with the given <paramref name="title"/> exists.
    /// </summary>
    /// <returns><see langword="true" /> if a control has been found; otherwise, <see langword="false" />.</returns>
    /// <exception cref="WebTestException">If the control cannot be found due to ambiguity.</exception>
    /// <remarks>
    /// Uses the <see cref="TitleControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    public static bool ExistsByTitle<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string title)
        where TControlSelector : ITitleControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("title", title);

      return fluentControlSelector.HasControl (new TitleControlSelectionCommandBuilder<TControlSelector, TControlObject> (title));
    }

    /// <summary>
    /// Extension method for selecting the first matching control.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control.</returns>
    /// <exception cref="WebTestException">If the control cannot be found.</exception>
    /// <remarks>
    /// Uses the <see cref="FirstControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [NotNull]
    public static TControlObject First<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector)
        where TControlSelector : IFirstControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);

      return fluentControlSelector.GetControl (new FirstControlSelectionCommandBuilder<TControlSelector, TControlObject>());
    }

    /// <summary>
    /// Extension method for selecting the first matching control.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control, or <see langword="null"/> if no control could be found.</returns>
    /// <remarks>
    /// Uses the <see cref="FirstControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [CanBeNull]
    public static TControlObject FirstOrNull<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector)
        where TControlSelector : IFirstControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);

      return fluentControlSelector.GetControlOrNull (new FirstControlSelectionCommandBuilder<TControlSelector, TControlObject>());
    }

    /// <summary>
    /// Extension method for selecting the only matching control.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control.</returns>
    /// <exception cref="WebTestException">If multiple matching controls are found.</exception>
    /// <exception cref="WebTestException">If the control cannot be found.</exception>
    /// <remarks>
    /// Uses the <see cref="SingleControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [NotNull]
    public static TControlObject Single<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector)
        where TControlSelector : ISingleControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);

      return fluentControlSelector.GetControl (new SingleControlSelectionCommandBuilder<TControlSelector, TControlObject>());
    }

    /// <summary>
    /// Extension method for selecting the only matching control.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control, or <see langword="null"/> if no control could be found.</returns>
    /// <exception cref="WebTestException">If multiple matching controls are found.</exception>
    /// <remarks>
    /// Uses the <see cref="SingleControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [CanBeNull]
    public static TControlObject SingleOrNull<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector)
        where TControlSelector : ISingleControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);

      return fluentControlSelector.GetControlOrNull (new SingleControlSelectionCommandBuilder<TControlSelector, TControlObject>());
    }

    /// <summary>
    /// Extension method for selecting a control by <paramref name="text"/>.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control.</returns>
    /// <exception cref="WebTestException">If the control cannot be found.</exception>
    /// <remarks>
    /// Uses the <see cref="TextContentControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [NotNull]
    public static TControlObject GetByTextContent<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string text)
        where TControlSelector : ITextContentControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return fluentControlSelector.GetControl (new TextContentControlSelectionCommandBuilder<TControlSelector, TControlObject> (text));
    }

    /// <summary>
    /// Extension method for selecting a control by <paramref name="text"/> if it exists.
    /// </summary>
    /// <returns>The <see cref="ControlObject"/> for the selected control, or <see langword="null"/> if no control could be found.</returns>
    /// <remarks>
    /// Uses the <see cref="TextContentControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    [CanBeNull]
    public static TControlObject GetByTextContentOrNull<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string text)
        where TControlSelector : ITextContentControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return fluentControlSelector.GetControlOrNull (new TextContentControlSelectionCommandBuilder<TControlSelector, TControlObject> (text));
    }

    /// <summary>
    /// Extension method for checking if a control with the given <paramref name="text"/> exists.
    /// </summary>
    /// <returns><see langword="true" /> if a control has been found; otherwise, <see langword="false" />.</returns>
    /// <exception cref="WebTestException">If the control cannot be found due to ambiguity.</exception>
    /// <remarks>
    /// Uses the <see cref="TextContentControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>.
    /// </remarks>
    public static bool ExistsByTextContent<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string text)
        where TControlSelector : ITextContentControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return fluentControlSelector.HasControl (new TextContentControlSelectionCommandBuilder<TControlSelector, TControlObject> (text));
    }
  }
}