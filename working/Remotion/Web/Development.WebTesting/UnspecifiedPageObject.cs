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

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Represents a generic page object which is returned by control object interactions (e.g. a click on an HtmlAnchorControlObject). The user of
  /// the framework must specify which actual page object he is expecting by calling one of the various Expect* methods or extension methods.
  /// </summary>
  public class UnspecifiedPageObject : WebTestObject<ControlObjectContext>
  {
    /// <param name="context">Context of the <see cref="ControlObject"/> which triggered the interaction.</param>
    public UnspecifiedPageObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns a <see cref="PageObject"/> of type <typeparamref name="TPageObject"/>. It is implicitly assumed that the actual page matches the
    /// expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject Expect<TPageObject> () where TPageObject : PageObject
    {
      return Expect<TPageObject> (poc => { });
    }

    /// <summary>
    /// Returns a <see cref="PageObject"/> of type <typeparamref name="TPageObject"/>. An assertion given by
    /// <paramref name="actualMatchesExpectedPageAssertion"/> checks whether the actual page matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="actualMatchesExpectedPageAssertion">Assertion which determines whether the correct page is shown.</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject Expect<TPageObject> ([NotNull] Action<PageObjectContext> actualMatchesExpectedPageAssertion) where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNull ("actualMatchesExpectedPageAssertion", actualMatchesExpectedPageAssertion);

      var newContext = Context.CloneForNewPage();
      return AssertActualMatchesExpectedPageAndReturnNewPageObject<TPageObject> (newContext, actualMatchesExpectedPageAssertion);
    }

    /// <summary>
    /// Returns a <see cref="PageObject"/> of type <typeparamref name="TPageObject"/> - located on a new window specified by the given
    /// <paramref name="windowLocator"/>. It is implicitly assumed that the actual page on that window matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="windowLocator">Title of the new window (or a uniquely identifying part of the title).</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject ExpectNewWindow<TPageObject> ([NotNull] string windowLocator)
        where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);

      return ExpectNewWindow<TPageObject> (windowLocator, po => { });
    }

    /// <summary>
    /// Returns a <see cref="PageObject"/> of type <typeparamref name="TPageObject"/> - located on a new window specified by the given
    /// <paramref name="windowLocator"/>. A conditon given by <paramref name="actualMatchesExpectedPageAssertion"/> checks whether the actual page on
    /// that window matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="windowLocator">Title of the new window (or a uniquely identifying part of the title).</param>
    /// <param name="actualMatchesExpectedPageAssertion">Assertion which determines whether the correct page is shown.</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject ExpectNewWindow<TPageObject> (
        [NotNull] string windowLocator,
        [NotNull] Action<PageObjectContext> actualMatchesExpectedPageAssertion)
        where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);
      ArgumentUtility.CheckNotNull ("actualMatchesExpectedPageAssertion", actualMatchesExpectedPageAssertion);

      var newContext = Context.CloneForNewWindow (windowLocator);
      return AssertActualMatchesExpectedPageAndReturnNewPageObject<TPageObject> (newContext, actualMatchesExpectedPageAssertion);
    }

    /// <summary>
    /// Returns a <see cref="PageObject"/> of type <typeparamref name="TPageObject"/> - located on a new popup window specified by the given
    /// <paramref name="windowLocator"/>. It is implicitly assumed that the actual page on that popup window matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="windowLocator">Title of the new popup window (or a uniquely identifying part of the title).</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject ExpectNewPopupWindow<TPageObject> ([NotNull] string windowLocator) where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);

      return ExpectNewPopupWindow<TPageObject> (windowLocator, po => { });
    }

    /// <summary>
    /// Returns a <see cref="PageObject"/> of type <typeparamref name="TPageObject"/> - located on a new popup window specified by the given
    /// <paramref name="windowLocator"/>. A conditon given by <paramref name="actualMatchesExpectedPageAssertion"/> checks whether the actual page on
    /// that popup window matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="windowLocator">Title of the new popup window (or a uniquely identifying part of the title).</param>
    /// <param name="actualMatchesExpectedPageAssertion">Assertion which determines whether the correct page is shown.</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject ExpectNewPopupWindow<TPageObject> (
        [NotNull] string windowLocator,
        [NotNull] Action<PageObjectContext> actualMatchesExpectedPageAssertion)
        where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);
      ArgumentUtility.CheckNotNull ("actualMatchesExpectedPageAssertion", actualMatchesExpectedPageAssertion);

      var newContext = Context.CloneForNewPopupWindow (windowLocator);
      return AssertActualMatchesExpectedPageAndReturnNewPageObject<TPageObject> (newContext, actualMatchesExpectedPageAssertion);
    }

    private static TPageObject AssertActualMatchesExpectedPageAndReturnNewPageObject<TPageObject> (
        PageObjectContext newPageObjectContext,
        Action<PageObjectContext> actualMatchesExpectedPageAssertion)
        where TPageObject : PageObject
    {
      actualMatchesExpectedPageAssertion (newPageObjectContext);
      return (TPageObject) Activator.CreateInstance (typeof (TPageObject), new object[] { newPageObjectContext });
    }
  }
}