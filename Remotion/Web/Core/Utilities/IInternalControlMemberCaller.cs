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
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.Web.Utilities
{
  /// <summary>
  /// Defines an API for performing operations on ASP.NET WebForms which are only possible by invoking non-public APIs.
  /// </summary>
  /// <seealso cref="InternalControlMemberCaller"/>
  public interface IInternalControlMemberCaller
  {
    void SetControlState (Control control, ControlState value);
    ControlState GetControlState (Control control);
    void InitRecursive (Control control, Control namingContainer);

    /// <summary> Encapsulates the invocation of <see cref="Control"/>'s LoadViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be restored. </param>
    /// <param name="viewState"> The view state object used for restoring. </param>
    void LoadViewStateRecursive (Control target, object viewState);

    /// <summary> Encapsulates the invocation of <see cref="Control"/>'s SaveViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be saved. </param>
    /// <returns> The view state object for <paramref name="target"/>. </returns>
    object SaveViewStateRecursive (Control target);

    /// <summary>Encapsulates the invocation of <see cref="Page"/>'s SaveAllState method.</summary>
    /// <param name="page">The <see cref="Page"/> for which SaveAllState will be invoked. Must not be <see langword="null" />.</param>
    void SaveAllState (Page page);

    /// <summary>Encapsulates the invocation of <see cref="Control"/>'s SaveChildControlState method.</summary>
    /// <param name="control">The <see cref="Control"/> for which SaveChildControlState will be invoked. Must not be <see langword="null" />.</param>
    IDictionary? SaveChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer : Control, INamingContainer;

    /// <summary>Encapsulates the invocation of <see cref="Control"/>'s SaveControlStateInternal method.</summary>
    /// <param name="control">The <see cref="Control"/> for which SaveControlStateInternal will be invoked. Must not be <see langword="null" />.</param>
    object SaveControlStateInternal (Control control);

    /// <summary>Returns the control states for all controls that are child-controls of the passed <see cref="Control"/>.</summary>
    IDictionary? GetChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer : Control, INamingContainer;

    /// <summary>Sets the control states for the child control of the passed <see cref="Control"/>.</summary>
    void SetChildControlState<TNamingContainer> (TNamingContainer control, IDictionary? newControlState)
        where TNamingContainer : Control, INamingContainer;

    /// <summary>Sets the control states for the child control of the passed <see cref="Control"/>.</summary>
    void ClearChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer : Control, INamingContainer;

    /// <summary>Encapsulates the get-access the the <see cref="Page"/>'s PageStatePersister property.</summary>
    PageStatePersister GetPageStatePersister (Page page);

    string SetCollectionReadOnly (ControlCollection collection, string? exceptionMessage);

    /// <summary>Encapsulates the get-access the the <see cref="RadioButtonList"/>'s ControlToRepeat property.</summary>
    RadioButton GetControlToRepeat (RadioButtonList radioButtonList);
  }
}
