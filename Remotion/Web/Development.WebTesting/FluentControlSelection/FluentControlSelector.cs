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
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Entry point for the fluent control selection. It allows you to write extension methods alike:
  /// <code>
  /// public static FluentControlSelection&lt;MyControlSelectorForMyControlObject, MyControlObject&gt; GetMyControl (this IControlHost host)
  /// {
  ///   return new FluentControlSelection&lt;MyControlSelectorForMyControlObject, MyControlObject&gt; (host, new MyControlSelectorForMyControlObject());
  /// }
  /// </code>
  /// which allow you to select controls using the following call structure:
  /// <code>
  /// pageOrOtherControlHost.GetMyControl().ById("myId");
  /// </code>
  /// See <see cref="FluentControlSelectorExtensions"/> for other <c>By*</c>-methods, or write your own.
  /// </summary>
  /// <typeparam name="TControlSelector"></typeparam>
  /// <typeparam name="TControlObject"></typeparam>
  public class FluentControlSelector<TControlSelector, TControlObject> : IFluentControlSelector<TControlSelector, TControlObject>
      where TControlSelector : IControlSelector
      where TControlObject : ControlObject
  {
    private readonly IControlHost _host;
    private readonly TControlSelector _controlSelector;

    public FluentControlSelector (IControlHost host, TControlSelector controlSelector)
    {
      ArgumentUtility.CheckNotNull ("host", host);
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);

      _host = host;
      _controlSelector = controlSelector;
    }

    /// <summary>
    /// Performs the selection and returns the actual control.
    /// </summary>
    /// <param name="selectionCommandBuilder">The selection command builder which is combined with the <see cref="IControlSelector"/>.</param>
    /// <returns>The selected control object.</returns>
    /// <exception cref="AmbiguousException">If multiple matching controls are found.</exception>
    /// <exception cref="MissingHtmlException">If the control cannot be found.</exception>
    TControlObject IFluentControlSelector<TControlSelector, TControlObject>.GetControl (
        IControlSelectionCommandBuilder<TControlSelector, TControlObject> selectionCommandBuilder)
    {
      ArgumentUtility.CheckNotNull ("selectionCommandBuilder", selectionCommandBuilder);

      return _host.GetControl (selectionCommandBuilder.Using (_controlSelector));
    }
  }
}