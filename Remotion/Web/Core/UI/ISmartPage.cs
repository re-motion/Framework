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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.SmartPageImplementation;

namespace Remotion.Web.UI
{
  /// <summary>
  ///   This interface represents a page that has a dirty-state and can prevent multiple postbacks.
  /// </summary>
  /// <include file='..\doc\include\UI\ISmartPage.xml' path='ISmartPage/Class/*' />
  public interface ISmartPage: IPage
  {
    /// <summary>
    /// Gets an <see cref="ISmartPageClientScriptManager"/> object used to manage, register, and add scripts to the page.
    /// </summary>
    /// <returns>An <see cref="ISmartPageClientScriptManager"/> object.</returns>
    new ISmartPageClientScriptManager ClientScript { get; }

    /// <summary> Gets the post back data for the page. </summary>
    NameValueCollection? GetPostBackCollection ();

    /// <summary> Gets or sets a flag describing whether the page is dirty. </summary>
    /// <value> <see langword="true"/> if the page requires saving. Defaults to <see langword="false"/>.  </value>
    bool IsDirty { get; set; }

    /// <summary>
    /// Evaluates the page's dirty state and returns a list of identifiers, one for each distinct dirty state condition.
    /// </summary>
    /// <param name="requestedStates">
    /// Optional list of dirty state conditions the caller is interested in. When this parameter is set, the implementation may optimize the evaluation of the dirty state
    /// to only test for a specific dirty state condition if it was actually requested.
    /// </param>
    /// <returns>
    /// The set of dirty states. Note that the result may contain values that have not been requested by the caller.
    /// </returns>
    /// <remarks>
    ///   Evaluates if either <see cref="IsDirty"/> is <see langword="true" />
    ///   or if any control registered using <see cref="RegisterControlForDirtyStateTracking"/> has values that must be persisted before the user leaves the page.
    /// </remarks>
    IEnumerable<string> GetDirtyStates (IReadOnlyCollection<string>? requestedStates = null);

    /// <summary>
    ///   Registers a control implementing <see cref="IEditableControl"/> for tracking of it's server- and client-side
    ///   dirty state.
    /// </summary>
    /// <param name="control"> A control implementing <see cref="IEditableControl"/> that will be tracked. </param>
    void RegisterControlForDirtyStateTracking (IEditableControl control);

    /// <summary>
    ///   Registers a <see cref="Control.ClientID"/> for the tracking of the controls client-side dirty state.
    /// </summary>
    /// <param name="clientID"> The ID of an HTML input/textarea/select element. </param>
    /// <remarks>
    ///   Note that this is only required for controls that do not implement <see cref="IEditableControl"/>
    ///   and therefor cannot be registered using <see cref="RegisterControlForDirtyStateTracking"/>.
    /// </remarks>
    void RegisterControlForClientSideDirtyStateTracking (string clientID);

    /// <summary>
    ///   Gets a flag that determines whether the dirty state will be taken into account when displaying the abort 
    ///   confirmation dialog.
    /// </summary>
    /// <value> 
    ///   <see langword="true"/> to invoke <see cref="GetDirtyStates"/> and track changes on the client.
    /// </value>
    bool IsDirtyStateTrackingEnabled { get; }

    /// <summary>
    ///   Gets or sets a flag that determines whether to display a confirmation dialog before leaving the page. 
    ///  </summary>
    /// <value> <see langword="true"/> to display the confirmation dialog. </value>
    /// <remarks> 
    ///   If <see cref="IsDirtyStateTrackingEnabled"/> evaluates <see langword="true"/>, a confirmation will only be 
    ///   displayed if the page is dirty.
    /// </remarks>
    bool IsAbortConfirmationEnabled { get; }

    /// <summary> Gets the message displayed when the user attempts to abort the WXE Function. </summary>
    /// <remarks> 
    ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="SmartPageInfo"/>. 
    /// </remarks>
    WebString AbortMessage { get; }

    /// <summary> Gets the message displayed when the user attempts to submit while the page is already submitting. </summary>
    /// <remarks> 
    ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="SmartPageInfo"/>. 
    /// </remarks>
    WebString StatusIsSubmittingMessage { get; }

    /// <summary>
    ///   Gets a flag whether the status messages (i.e. is-submitting) will be displayed when the user tries to e.g. postback while a request is being processed.
    /// </summary>
    bool AreStatusMessagesEnabled { get; }

    /// <summary>
    ///   Gets a flag whether a queued submit should be executed or aborted upon completion of the postback.
    /// </summary>
    bool IsQueuedSubmitToBeAborted { get; }

    /// <summary> 
    ///   Registers Java Script functions to be executed when the respective <paramref name="pageEvent"/> is raised.
    /// </summary>
    /// <include file='..\doc\include\UI\ISmartPage.xml' path='ISmartPage/RegisterClientSidePageEventHandler/*' />
    void RegisterClientSidePageEventHandler (SmartPageEvents pageEvent, string key, string function);

    /// <summary>
    ///   Regisiters a Java Script function used to evaluate whether to continue with the submit.
    ///   Signature: <c>bool Function (isAborting, hasSubmitted, hasUnloaded, isCached)</c>
    /// </summary>
    string? CheckFormStateFunction { get; set; }

    /// <summary>
    /// Registers individual event arguments for a control as a synchronous postback target.
    /// </summary>
    /// <param name="control">The <see cref="Control"/> for which a synchronous postback target is registered. Must not be <see langword="null" />.</param>
    /// <param name="eventArguments">The event argument to register. Must not be <see langword="null" /> or empty.</param>
    /// <remarks>
    /// The <paramref name="control"/> must not be registered as a synchronous postback target in addition to registering individual event arguments.
    /// </remarks>
    void RegisterCommandForSynchronousPostBack ([NotNull]Control control, [NotNull]string eventArguments);

    /// <summary>
    /// Registers a control as a synchronous postback target.
    /// </summary>
    /// <param name="control">The <see cref="Control"/> registered for a synchronous postback. Must not be <see langword="null" />.</param>
    /// <remarks>
    /// The <paramref name="control"/> must not be registered as a synchronous postback target in addition to registering individual event arguments.
    /// </remarks>
    void RegisterControlForSynchronousPostBack ([NotNull] Control control);

    /// <summary> Saves the ControlState and the ViewState of the ASP.NET page. </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    void SaveAllState ();
  }
}
