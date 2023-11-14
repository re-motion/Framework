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

// The context contains all information required by the smart page.
// theFormID: The ID of the HTML Form on the page.
// isDirtyStateTrackingEnabled: true if the page should watch the form-fields for changes.
// isDirty: true if the page is dirty (client or server-side)
// abortMessage: The message displayed when the user attempts to leave the page. null to disable the message.
// statusIsSubmittingMessage: The message displayed when the user attempts to submit while a submit is already in 
//    progress. null to disable the message.
// smartScrollingFieldID: The ID of the hidden field containing the smart scrolling data.
// smartFocusFieldID: The ID of the hidden field containing the smart focusing data.
// checkFormStateFunctionName: The name of the function used to evaluate whether to submit the form.
//    null if no external logic should be incorporated.
// eventHandlers: The hashtable of eventhandlers: Hashtable < event-key, Array < event-handler > >
function SmartPage_Context(
    theFormID,
    isDirtyStateTrackingEnabled,
    abortMessage, statusIsSubmittingMessage,
    smartScrollingFieldID, smartFocusFieldID,
    checkFormStateFunctionName)
{
  ArgumentUtility.CheckNotNullAndTypeIsString('theFormID', theFormID);
  ArgumentUtility.CheckNotNullAndTypeIsBoolean('isDirtyStateTrackingEnabled', isDirtyStateTrackingEnabled);
  ArgumentUtility.CheckTypeIsString('abortMessage', abortMessage);
  ArgumentUtility.CheckTypeIsString('statusIsSubmittingMessage', statusIsSubmittingMessage);
  ArgumentUtility.CheckTypeIsString('smartScrollingFieldID', smartScrollingFieldID);
  ArgumentUtility.CheckTypeIsString('smartFocusFieldID', smartFocusFieldID);
  ArgumentUtility.CheckTypeIsString('checkFormStateFunctionName', checkFormStateFunctionName);

  var _theForm;

  var _isDirtyStateTrackingEnabled = isDirtyStateTrackingEnabled;
  var _isDirty = false;

  // The message displayed when the user attempts to leave the page.
  // null to disable the message.
  var _abortMessage = abortMessage;
  var _isAbortConfirmationEnabled = abortMessage != null;

  var _submitState = null;
  var _hasSubmitted = false;
  // Special flag to support the OnBeforeUnload part
  var _isSubmittingBeforeUnload = false;
  // The message displayed when the user attempts to submit while a submit is already in progress.
  // null to disable the message.
  var _statusIsSubmittingMessage = statusIsSubmittingMessage;
  var _abortQueuedSubmit = false;
  var _lastManualSubmitter = null;

  var _isAborting = false;
  var _isCached = false;
  // Special flag to support the OnBeforeUnload part
  var _isAbortingBeforeUnload = false;
  // Special flag to support conditional logic during OnBeforeUnload
  var _isOnBeforeUnloadExecuting = false;
  // Special flag to support conditional logic during OnUnload
  var _isOnUnloadExecuting = false;

  // The name of the function used to evaluate whether to submit the form.
  // null if no external logic should be incorporated.
  var _checkFormStateFunctionName = checkFormStateFunctionName;

  var _statusMessageWindow = null;
  var _hasUnloaded = false;

  var _aspnetFormOnSubmit = null;
  var _aspnetDoPostBack = null;
  // Sepcial flag to support the Form.OnSubmit event being executed by the ASP.NET __doPostBack function.
  var _isExecutingDoPostBack = false;

  // The hidden field containing the smart scrolling data.
  var _smartScrollingFieldID = null;
  // The hidden field containing the smart focusing data.
  var _smartFocusFieldID = null;

  var _activeElement = null;
  // The hashtable of eventhandlers: Hashtable < event-key, Array < event-handler > >
  var _eventHandlers = new Array();
  var _trackedIDs = new Array();
  var _synchronousPostBackCommands = new Array();

  var _cacheStateHasSubmitted = 'hasSubmitted';
  var _cacheStateHasLoaded = 'hasLoaded';

  var _loadHandler = function () { SmartPage_Context.Instance.OnLoad(); };
  var _beforeUnloadHandler = function () { return SmartPage_Context.Instance.OnBeforeUnload(); };
  var _unloadHandler = function () { return SmartPage_Context.Instance.OnUnload(); };
  var _scrollHandler = function () { SmartPage_Context.Instance.OnScroll(); };
  var _resizeHandler = function () { SmartPage_Context.Instance.OnResize(); };
  var _formSubmitHandler = function () { return SmartPage_Context.Instance.OnFormSubmit(); };
  var _formClickHandler = function (evt) { return SmartPage_Context.Instance.OnFormClick(evt); };
  var _doPostBackHandler = function (eventTarget, eventArg) { SmartPage_Context.Instance.DoPostBack(eventTarget, eventArg); };
  var _valueChangedHandler = function (evt) { SmartPage_Context.Instance.OnValueChanged(evt); };
  var _mouseDownHandler = function (evt) { SmartPage_Context.Instance.OnMouseDown(evt); };
  var _mouseUpHandler = function (evt) { SmartPage_Context.Instance.OnMouseUp(evt); };

  this.Init = function ()
  {
    _theForm = window.document.forms[theFormID];
    {
      if (_theForm == null)
        throw ('"' + theFormID + '" does not specify a Form.');
    }

    if (smartScrollingFieldID != null)
    {
      if (_theForm.elements[smartScrollingFieldID] == null)
        throw ('"' + smartScrollingFieldID + '" does not specify a element of Form "' + _theForm.id + '".');
    }

    if (smartFocusFieldID != null)
    {
      if (_theForm.elements[smartFocusFieldID] == null)
        throw ('"' + smartFocusFieldID + '" does not specify a element of Form "' + _theForm.id + '".');
    }

    _smartScrollingFieldID = smartScrollingFieldID;
    _smartFocusFieldID = smartFocusFieldID;

    AttachPageLevelEventHandlers();
  };

  this.set_AbortQueuedSubmit = function (value)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('value', value);
    _abortQueuedSubmit = value;
  };

  this.set_EventHandlers = function (eventHandlers)
  {
    ArgumentUtility.CheckTypeIsObject('eventHandlers', eventHandlers);
    _eventHandlers = eventHandlers;
  };

  this.set_TrackedIDs = function (trackedIDs)
  {
    ArgumentUtility.CheckTypeIsObject('trackedIDs', trackedIDs);
    _trackedIDs = trackedIDs;
  };

  this.set_SynchronousPostBackCommands = function (synchronousPostBackCommands)
  {
    ArgumentUtility.CheckTypeIsObject('synchronousPostBackCommands', synchronousPostBackCommands);
    _synchronousPostBackCommands = synchronousPostBackCommands;
  };

  // Attaches the event handlers to the page's events.
  function AttachPageLevelEventHandlers()
  {
    RemoveEventHandler(window, 'load', _loadHandler);
    AddEventHandler(window, 'load', _loadHandler);

    // IE, Mozilla 1.7, Firefox 0.9
    window.onbeforeunload = _beforeUnloadHandler;

    if (TypeUtility.IsDefined(window.onpagehide))
      window.onpagehide = _unloadHandler;
    else
      window.onunload = _unloadHandler;

    RemoveEventHandler(window, 'scroll', _scrollHandler);
    AddEventHandler(window, 'scroll', _scrollHandler);

    RemoveEventHandler(window.document, 'mousedown', _mouseDownHandler);
    AddEventHandler(window.document, 'mousedown', _mouseDownHandler);

    RemoveEventHandler(window.document, 'mouseup', _mouseUpHandler);
    AddEventHandler(window.document, 'mouseup', _mouseUpHandler);

    PageUtility.Instance.RegisterResizeHandler('#' + _theForm.id, _resizeHandler);

    _aspnetFormOnSubmit = _theForm.onsubmit;
    _theForm.onsubmit = _formSubmitHandler;
    _theForm.onclick = _formClickHandler;
    if (TypeUtility.IsDefined(window.__doPostBack))
    {
      _aspnetDoPostBack = window.__doPostBack;
      window.__doPostBack = _doPostBackHandler;
    }
  };


  // Called after page's html content is complete.
  // Used to perform initalization code that only requires complete the HTML source but not necessarily all images.
  this.OnStartUp = function (isAsynchronous, isDirty)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAsynchronous', isAsynchronous);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isDirty', isDirty);

    _isDirty = isDirty;

    if (_isDirtyStateTrackingEnabled)
      AttachDataChangedEventHandlers();

    var pageRequestManager = GetPageRequestManager();
    if (pageRequestManager != null)
    {
      Sys.WebForms.PageRequestManager.prototype._updatePanel = Sys$WebForms$PageRequestManager$_updatePanel;
      pageRequestManager.add_endRequest(SmartPage_PageRequestManager_endRequest);
    }
  };

  // Replacement for Sys.WebForms.PageRequestManager.prototype._updatePanel
  function Sys$WebForms$PageRequestManager$_updatePanel(updatePanelElement, rendering)
  {
    for (var updatePanelID in this._scriptDisposes)
    {
      if (this._elementContains(updatePanelElement, document.getElementById(updatePanelID)))
      {
        var disposeScripts = this._scriptDisposes[updatePanelID];
        for (var i = 0, l = disposeScripts.length; i < l; i++)
        {
          eval(disposeScripts[i]);
        }
        delete this._scriptDisposes[updatePanelID];
      }
    }
    if (TypeUtility.IsDefined(Sys.Application.disposeElement)) // .NET 4.0 AJAX library
    {
      Sys.Application.disposeElement(updatePanelElement, true);
    }
    else
    {
      throw "Unsupported AJAX library detected.";
    }
    $(updatePanelElement).empty().append(rendering);
  }

  function SmartPage_PageRequestManager_endRequest(sender, args)
  {
    if (args.get_error() != undefined && args.get_error().httpStatusCode == '500')
    {
      var errorMessage = args.get_error().message;
      args.set_errorHandled(true);

      var errorBody = '<div class="SmartPageErrorBody"><div>' + errorMessage + '</div></div>';

      SmartPage_Context.Instance.ShowMessage("SmartPageServerErrorMessage", errorBody);
    }
  }

  // Attached the OnValueChanged event handler to all form data elements listed in _trackedIDs.
  function AttachDataChangedEventHandlers()
  {
    for (var i = 0; i < _trackedIDs.length; i++)
    {
      var id = _trackedIDs[i];
      var element = _theForm.elements[id];
      if (element == null)
        continue;

      var tagName = element.tagName.toLowerCase();

      if (tagName == 'input')
      {
        var type = element.type.toLowerCase();
        if (type == 'text' || type == 'hidden')
        {
          RemoveEventHandler(element, 'change', _valueChangedHandler);
          AddEventHandler(element, 'change', _valueChangedHandler);
        }
        else if (type == 'checkbox' || type == 'radio')
        {
          RemoveEventHandler(element, 'click', _valueChangedHandler);
          AddEventHandler(element, 'click', _valueChangedHandler);
        }
      }
      else if (tagName == 'textarea' || tagName == 'select')
      {
        RemoveEventHandler(element, 'change', _valueChangedHandler);
        AddEventHandler(element, 'change', _valueChangedHandler);
      }
    }
  };

  // Event handler attached to the change event of tracked form elements
  this.OnValueChanged = function ()
  {
    _isDirty = true;
  };

  this.OnMouseDown = function (e)
  {
    var isLeftButton = e.button === 0;
    var target = GetSubmitTarget (e.target);
    if (isLeftButton && target !== null)
    {
      _lastManualSubmitter = target;
    }
  };

  this.OnMouseUp = function (e)
  {
    var lastTarget = _lastManualSubmitter;
    _lastManualSubmitter = null;

    if (lastTarget == null)
      return;

    if (IsRooted (lastTarget))
      return;

    var currentTarget = GetSubmitTarget (e.target);
    if (currentTarget == null)
    {
      if (window.console)
        window.console.log ("A click on a submit-target was aborted because the submit target is no longer available while handling the mouse-up event. No action will be taken.");

      return;
    }

    var executeClick = false;
    if (TypeUtility.IsDefined (currentTarget.id) && TypeUtility.IsDefined (lastTarget.id) && !StringUtility.IsNullOrEmpty (lastTarget.id))
    {
      executeClick = currentTarget.id === lastTarget.id;
    }
    else
    {
      var lastTargetContent = lastTarget.innerHTML;
      var currentTargetContent = currentTarget.innerHTML;
      executeClick = currentTargetContent === lastTargetContent;
    }

    if (executeClick)
    {
      if (IsJavaScriptAnchor (currentTarget))
      {
        var href = currentTarget.href;
        eval (href);
      }
      else
      {
        $(currentTarget).trigger ('click');
      }
    }
  }

  // Backs up the smart scrolling and smart focusing data for the next post back.
  this.Backup = function ()
  {
    if (_smartScrollingFieldID != null)
      _theForm.elements[_smartScrollingFieldID].value = SmartScrolling_Backup();
    if (_smartFocusFieldID != null)
      _theForm.elements[_smartFocusFieldID].value = SmartFocus_Backup();
  };

  // Restores the smart scrolling and smart focusing data from the previous post back.
  this.Restore = function ()
  {
    if (_smartScrollingFieldID != null)
      SmartScrolling_Restore(_theForm.elements[_smartScrollingFieldID].value);
    if (_smartFocusFieldID != null)
      SmartFocus_Restore(_theForm.elements[_smartFocusFieldID].value);
  };

  // Event handler for window.OnLoad
  this.OnLoad = function ()
  {
    var pageRequestManager = GetPageRequestManager();
    if (pageRequestManager != null)
    {
      pageRequestManager.remove_pageLoaded(SmartPage_PageRequestManager_pageLoaded);
      pageRequestManager.add_pageLoaded(SmartPage_PageRequestManager_pageLoaded);
    }

    var isAsynchronous = false;
    this.PageLoaded(isAsynchronous);
  };

  this.PageRequestManager_pageLoaded = function (sender, args)
  {
    var isAsynchronous = sender && sender.get_isInAsyncPostBack();
    if (isAsynchronous)
    {
      this.PageLoaded(isAsynchronous);
    }
  };

  this.PageLoaded = function (isAsynchronous)
  {
    if (!isAsynchronous)
      this.CheckIfCached();

    this.Restore();

    ExecuteEventHandlers (_eventHandlers['onloading'], _hasSubmitted, _isCached, isAsynchronous);

    var isSubmitting = false;
    if (this.IsSubmitting() && _submitState.NextSubmitState != null && _submitState.NextSubmitState.Submitter != null && !_abortQueuedSubmit)
    {
      var nextSubmitState = _submitState.NextSubmitState;
      _submitState = null;
      if (!StringUtility.IsNullOrEmpty(nextSubmitState.EventTarget))
      {
        isSubmitting = true;
        setTimeout (function() { window.__doPostBack(nextSubmitState.EventTarget, nextSubmitState.EventArgument); }, 0);
      }
      else
      {
        var nextSubmitterID = nextSubmitState.Submitter.id;
        var submitterElement = document.getElementById(nextSubmitterID);
        if (submitterElement != null)
        {
          var isButton = false;
          var tagName = submitterElement.tagName.toLowerCase();
          if (tagName === 'input')
          {
            var type = submitterElement.type.toLowerCase();
            isButton = type === 'submit' || type === 'button';
          }
          else if (tagName === 'button')
          {
            isButton = true;
          }

          if (isButton)
          {
            isSubmitting = true;
            setTimeout (function () { $ (submitterElement).trigger ("click"); }, 0);
          }
        }
      }
    }

    if (_abortQueuedSubmit)
      _lastManualSubmitter = null;

    if (!isSubmitting)
    {
      this.ClearIsSubmitting (isAsynchronous);
      _isSubmittingBeforeUnload = false;
      this.HideStatusMessage();

      ExecuteEventHandlers (_eventHandlers['onloaded'], _hasSubmitted, _isCached, isAsynchronous);
    }
  };

  // Determines whether the page was loaded from cache.
  this.CheckIfCached = function ()
  {
    var field = _theForm.SmartPage_CacheDetectionField;
    if (field.value == _cacheStateHasSubmitted)
    {
      _hasSubmitted = true;
      _isCached = true;
    }
    else if (field.value == _cacheStateHasLoaded)
    {
      _isCached = true;
    }
    else
    {
      this.SetCacheDetectionFieldLoaded();
    }
  };

  // Marks the page as loaded.
  this.SetCacheDetectionFieldLoaded = function ()
  {
    var field = _theForm.SmartPage_CacheDetectionField;
    field.value = _cacheStateHasLoaded;
  };

  // Marks the page as submitted.
  this.SetCacheDetectionFieldSubmitted = function ()
  {
    var field = _theForm.SmartPage_CacheDetectionField;
    field.value = _cacheStateHasSubmitted;
  };

  // Event handler for window.OnBeforeUnload.
  // __doPostBack
  // {
  //   Form.submit()
  //   {
  //     OnBeforeUnload()
  //   } 
  // }
  // Wait For Response
  // OnUnload()
  this.OnBeforeUnload = function ()
  {
    _isOnBeforeUnloadExecuting = true;
    try
    {
      _isAbortingBeforeUnload = false;
      var displayAbortConfirmation = false;

      if (!_hasUnloaded
        && !_isCached
        && !_isSubmittingBeforeUnload
        && !_isAborting && _isAbortConfirmationEnabled)
      {
        var submitterElement = GetSubmitterOrActiveElement();
        var isJavaScriptAnchor = IsJavaScriptAnchor(submitterElement);
        var isAbortConfirmationRequired = !isJavaScriptAnchor
          && (!_isDirtyStateTrackingEnabled || _isDirty);

        if (isAbortConfirmationRequired)
        {
          _isAbortingBeforeUnload = true;
          displayAbortConfirmation = true;
        }
      }
      else if (_isSubmittingBeforeUnload)
      {
        _isSubmittingBeforeUnload = false;
      }

      ExecuteEventHandlers(_eventHandlers['onbeforeunload']);
      if (displayAbortConfirmation)
      {
        // IE alternate/official version: window.event.returnValue = SmartPage_Context.Instance.AbortMessage
        return _abortMessage;
      }
    }
    finally
    {
      _isOnBeforeUnloadExecuting = false;
    }
  };

  // Event handler for window.OnUnload.
  this.OnUnload = function ()
  {
    _isOnUnloadExecuting = true;
    try
    {
      if ((!this.IsSubmitting() || _isAbortingBeforeUnload) && !_isAborting)
      {
        _isAborting = true;
        ExecuteEventHandlers (_eventHandlers['onabort'], _hasSubmitted, _isCached);
        _isAbortingBeforeUnload = false;
      }
      ExecuteEventHandlers (_eventHandlers['onunload']);
      _hasUnloaded = true;
      this.ClearIsSubmitting (false);
      _isAborting = false;

      _theForm = null;
      _activeElement = null;

      _loadHandler = null;
      _beforeUnloadHandler = null;
      _unloadHandler = null;
      _scrollHandler = null;
      _resizeHandler = null;
      _formSubmitHandler = null;
      _formClickHandler = null;
      _doPostBackHandler = null;
      _valueChangedHandler = null;
    }
    finally
    {
      _isOnUnloadExecuting = false;
    }
  };

  // Override for the ASP.NET __doPostBack method.
  this.DoPostBack = function (eventTarget, eventArgument)
  {
    // Debugger space
    var dummy = 0;
    var continueRequest = this.CheckFormState();
    if (continueRequest)
    {
      try
      {
        _isExecutingDoPostBack = true;
        _theForm.__EVENTTARGET.value = eventTarget;
        _theForm.__EVENTARGUMENT.value = eventArgument;

        var submitState = this.SetIsSubmitting(false, eventTarget, eventArgument);
        // Abort the postback if there is already a postback in progress
        if (submitState.NextSubmitState !== null)
          return;

        _isSubmittingBeforeUnload = true;

        this.Backup();

        ExecuteEventHandlers(_eventHandlers['onpostback'], eventTarget, eventArgument);
        this.SetCacheDetectionFieldSubmitted();

        _aspnetDoPostBack(eventTarget, eventArgument);
      }
      finally
      {
        _theForm.__EVENTTARGET.value = '';
        _theForm.__EVENTARGUMENT.value = '';
        _isExecutingDoPostBack = false;
      }
    }
  };

  // Event handler for Form.Submit.
  this.OnFormSubmit = function ()
  {
    if (_isExecutingDoPostBack)
    {
      if (_aspnetFormOnSubmit != null)
        return _aspnetFormOnSubmit();
      else
        return true;
    }
    else
    {
      var postBackSettings = GetPostBackSettings();
      if (postBackSettings != null && postBackSettings.async)
      {
        if (IsSynchronousPostBackRequired(postBackSettings))
        {
          this.DoPostBack(postBackSettings.asyncTarget, _theForm.__EVENTARGUMENT.value);
          return false;
        }
        else
        {
          var eventTarget = postBackSettings.asyncTarget === _theForm.__EVENTTARGET.value ? postBackSettings.asyncTarget : '';
          var eventArgument = eventTarget !== '' ? _theForm.__EVENTARGUMENT.value : '';
          var continueRequest = this.CheckFormState();
          var submitState = this.SetIsSubmitting(true, eventTarget, eventArgument);
          if (continueRequest && submitState.NextSubmitState === null)
          {
            _isSubmittingBeforeUnload = true;

            this.Backup();
            ExecuteEventHandlers(_eventHandlers['onpostback'], eventTarget, eventArgument);
            return true;
          }
          else
          {
            return false;
          }
        }
      }

      var continueRequest = this.CheckFormState();
      if (this.IsSubmitting() && continueRequest)
      {
        // This Code path is taken for synchronous requests that are not triggered via form-submit
        // and a postback is already in progress. In this case, the previous post state should be cleared.
        _theForm.__EVENTTARGET.value = '';
        _theForm.__EVENTARGUMENT.value = '';
      }

      var submitState = this.SetIsSubmitting(false, '', '');
      if (continueRequest && submitState.NextSubmitState === null)
      {
        _isSubmittingBeforeUnload = true;

        this.Backup();

        var eventSource = GetSubmitterOrActiveElement();
        var eventSourceID = (eventSource != null) ? eventSource.id : null;
        ExecuteEventHandlers(_eventHandlers['onpostback'], eventSourceID, '');
        this.SetCacheDetectionFieldSubmitted();

        return true;
      }
      else
      {
        return false;
      }
    }
  };

  // Event handler for Form.OnClick.
  this.OnFormClick = function (evt)
  {
    var postBackSettings = GetPostBackSettings();
    if (postBackSettings != null && IsSynchronousPostBackRequired(postBackSettings))
      return true;

    var eventSource = GetEventSource(evt);
    if (IsJavaScriptAnchor(eventSource))
    {
      var continueRequest = this.CheckFormState();
      if (!continueRequest)
        return false;
      else
        return void (0);
    }
    else
    {
      return void (0);
    }
  };

  // returns: true to continue with request.
  this.CheckFormState = function ()
  {
    if (_aspnetFormOnSubmit != null && !_aspnetFormOnSubmit())
      return false;

    var continueRequest = true;
    var fct = null;
    if (_checkFormStateFunctionName != null)
      fct = GetFunctionPointer(_checkFormStateFunctionName);
    if (fct != null)
    {
      try
      {
        continueRequest = fct(_isAborting, _hasSubmitted, _hasUnloaded, _isCached);
      }
      catch (e)
      {
      }
    }

    if (!continueRequest)
      return false;

    if (!this.IsSubmitting())
      return true;

    var isAsyncAutoPostback = _submitState.IsAsynchronous && _submitState.IsAutoPostback;
    var isTriggeredByCurrentSubmitter = _submitState.Submitter === GetActiveElement();
    var hasQueuedSubmit = _submitState.NextSubmitState != null;
    if (!hasQueuedSubmit && isAsyncAutoPostback && !isTriggeredByCurrentSubmitter)
      return true;

    this.ShowStatusIsSubmittingMessage();
    return false;
  };

  // Event handler for Window.OnScroll.
  this.OnScroll = function ()
  {
    if (_statusMessageWindow != null)
      AlignStatusMessage(_statusMessageWindow);
    ExecuteEventHandlers(_eventHandlers['onscroll']);
  };

  // Event handler for Window.OnResize.
  this.OnResize = function ()
  {
    if (_statusMessageWindow != null)
      AlignStatusMessage(_statusMessageWindow);
    ExecuteEventHandlers(_eventHandlers['onresize']);
  };

  // Sends an AJAX request to the server.
  // successHandler: function (args { Status }), called when the request succeeds
  // errorHandler: function (args { Status }), called when the request fails
  this.SendOutOfBandRequest = function (url, successHandler, errorHandler)
  {
    ArgumentUtility.CheckNotNullAndTypeIsString('url', url);
    ArgumentUtility.CheckNotNullAndTypeIsFunction('successHandler', successHandler);
    ArgumentUtility.CheckNotNullAndTypeIsFunction('errorHandler', errorHandler);

    if ((_isOnBeforeUnloadExecuting || _isOnUnloadExecuting) && navigator.sendBeacon)
    {
      var result = navigator.sendBeacon (url, null);
      if (result)
      {
        let args = { Status : 200 };
        successHandler (args);
      }
      else
      {
        let args = { Status: 400 };
        errorHandler (args);
      }
    }
    else
    {
      var xhr = new XMLHttpRequest();

      var readStateDone = 4;
      var httpStatusSuccess = 299;
      var method = 'GET';
      var isAsyncCall = true;

      xhr.open (method, url, isAsyncCall);
      xhr.onreadystatechange = function ()
      {
        if (this.readyState === readStateDone)
        {
          var args = { Status : this.status };
          if (this.status > 0 && this.status <= httpStatusSuccess)
            successHandler (args);
          else
            errorHandler (args);
        }
      };
      xhr.send();
    }
  };

  function AddEventHandler(object, eventType, handler)
  {
    $(object).bind(eventType, handler);
    return;
  };

  function RemoveEventHandler(object, eventType, handler)
  {
    $(object).unbind(eventType, handler);
    return;
  }

  // Executes the event handlers.
  // eventHandlers: an array of event handlers.
  function ExecuteEventHandlers(eventHandlers)
  {
    if (eventHandlers == null)
      return;

    for (var i = 0; i < eventHandlers.length; i++)
    {
      var eventHandler = GetFunctionPointer(eventHandlers[i]);
      if (eventHandler != null)
      {
        var eventHandlerArguments = Array.prototype.slice.call(ExecuteEventHandlers.arguments, 1);

        try
        {
          eventHandler.apply(null, eventHandlerArguments);
        }
        catch (e)
        {
        }
      }
    }
  };

  // Evaluates the string and returns the specified function.
  // A String pointing to a valid function.
  // Returns: The function or null if the function could not be found.
  function GetFunctionPointer(functionName)
  {
    ArgumentUtility.CheckTypeIsString('functionName', functionName);
    if (StringUtility.IsNullOrEmpty(functionName))
      return null;
    var fct = null;
    try
    {
      fct = eval(functionName);
    }
    catch (e)
    {
      return null;
    }
    if (TypeUtility.IsFunction(fct))
      return fct;
    else
      return null;
  };

  // Shows the status message informing the user that the page is already submitting.
  this.ShowStatusIsSubmittingMessage = function ()
  {
    if (_statusIsSubmittingMessage != null)
      this.ShowMessage('SmartPageStatusIsSubmittingMessage', _statusIsSubmittingMessage);
  };

  //  Shows a status message in the window using a DIV
  this.ShowMessage = function (id, message)
  {
    ArgumentUtility.CheckNotNullAndTypeIsString ('id', id);
    ArgumentUtility.CheckNotNullAndTypeIsString ('message', message);

    if (_statusMessageWindow == null)
    {
      var statusMessageWindow = window.document.createElement('DIV');
      statusMessageWindow.id = id;
      statusMessageWindow.style.width = '50%';
      statusMessageWindow.style.height = '50%';
      statusMessageWindow.style.left = '25%';
      statusMessageWindow.style.top = '25%';
      statusMessageWindow.style.position = 'absolute';

      var statusMessageBlock = window.document.createElement ('DIV');
      statusMessageBlock.style.width = '100%';
      statusMessageBlock.style.height = '100%';
      statusMessageBlock.style.left = '0';
      statusMessageBlock.style.top = '0';
      statusMessageBlock.style.position = 'absolute';
      statusMessageBlock.innerHTML =
        '<table style="border:none; height:100%; width:100%"><tr><td style="text-align:center;">' +
        message +
        '</td></tr></table>';
      statusMessageWindow.appendChild (statusMessageBlock);
      $(statusMessageBlock).iFrameShim({ top: '0px', left: '0px', width: '100%', height: '100%' });

      window.document.body.appendChild (statusMessageWindow, _theForm);
      AlignStatusMessage (statusMessageWindow);
      _statusMessageWindow = statusMessageWindow;
    }
  };

  // (Re-)Aligns the status message in the window.
  function AlignStatusMessage(message)
  {
    ArgumentUtility.CheckNotNullAndTypeIsObject('message', message);

    var scrollLeft = window.document.body.scrollLeft;
    var scrollTop = window.document.body.scrollTop;
    var windowWidth = window.document.body.clientWidth;
    var windowHeight = window.document.body.clientHeight;

    message.style.left = windowWidth / 2 - message.offsetWidth / 2 + scrollLeft;
    message.style.top = windowHeight / 2 - message.offsetHeight / 2 + scrollTop;
  };

  this.HideStatusMessage = function ()
  {
    if (_statusMessageWindow != null)
    {
      window.document.body.removeChild(_statusMessageWindow);
      _statusMessageWindow = null;
    }
  };

  // Returns the document.activeElement and uses the hovered element as fallback if the activeElement is not set.
  function GetActiveElement()
  {
    var activeElement = window.document.activeElement;
    if (!TypeUtility.IsNull (activeElement) && !TypeUtility.IsUndefined (activeElement.tagName) && activeElement.tagName.toLowerCase() !== 'body')
      return activeElement;

    // WebKit does not set activeElement if the element is selected using the mouse

    var hoverElement = $('input:hover, button:hover, a:hover');
    if (hoverElement.length > 0)
      return hoverElement[0];

    return null;
  }

  function GetDoPostBackSubmitterElement ()
  {
    var postBackSettings = GetPostBackSettings();
    if (postBackSettings != null && postBackSettings.async && postBackSettings.sourceElement != null)
    {
      if (IsFocusableTag (postBackSettings.sourceElement.tagName))
        return postBackSettings.sourceElement;
    }

    var eventTarget = _theForm.__EVENTTARGET.value;
    if (eventTarget != '')
    {
      var eventTargetElement = _theForm.elements[eventTarget];
      if (eventTargetElement != null)
        return eventTargetElement;
    }

    return null;
  }

  function GetSubmitterOrActiveElement ()
  {
    return GetDoPostBackSubmitterElement() || GetActiveElement();
  }

  // Determines whether the elements of the specified tag can receive the focus.
  function IsFocusableTag(tagName)
  {
    ArgumentUtility.CheckTypeIsString('tagName', tagName);
    if (StringUtility.IsNullOrEmpty(tagName))
      return false;
    tagName = tagName.toLowerCase();
    return (tagName == 'a' ||
            tagName == 'button' ||
            tagName == 'input' ||
            tagName == 'textarea' ||
            tagName == 'select');
  };

  // Determines whether the element (or it's parent) is an anchor tag 
  // and if javascript is used as the HREF.
  function IsJavaScriptAnchor(element)
  {
    if (TypeUtility.IsNull(element) || TypeUtility.IsUndefined(element.tagName))
      return false;
    ArgumentUtility.CheckTypeIsObject('element', element);

    var tagName = element.tagName.toLowerCase();
    if (tagName == 'a'
        && TypeUtility.IsDefined(element.href) && element.href != null
        && element.href.substring(0, 'javascript:'.length).toLowerCase() == 'javascript:')
    {
      return true;
    }
    else if (tagName == 'input'
             || tagName == 'select'
             || tagName == 'textarea'
             || tagName == 'button'
             || tagName == 'li'
             || tagName == 'p'
             || tagName == 'div'
             || tagName == 'td'
             || tagName == 'table'
             || tagName == 'form'
             || tagName == 'body'
             || tagName == 'html')
    {
      return false;
    }
    else
    {
      return IsJavaScriptAnchor(element.parentNode);
    }
  };

  // Gets the button, input-submit, input-button, or anchor element associated with the current element or null if the element is not a valid target.
  function GetSubmitTarget (element)
  {
    if (TypeUtility.IsNull (element))
      return null;

    if (TypeUtility.IsUndefined (element.tagName))
      return null;

    ArgumentUtility.CheckTypeIsObject('element', element);

    var tagName = element.tagName.toLowerCase();
    if (tagName === 'button')
      return element;
    if (tagName === 'a')
      return element;
    if (tagName === 'input')
    {
      if (!TypeUtility.IsDefined (element.type))
        return null;
      var type = element.type.toLowerCase();
      if (type === 'submit')
        return element;
      if (type === 'button')
        return element;
      return null;
    }

    if (tagName === 'select')
        return null;
    if (tagName === 'textarea')
        return null;
    if (tagName === 'li')
        return null;
    if (tagName === 'p')
        return null;
    if (tagName === 'div')
        return null;
    if (tagName === 'td')
        return null;
    if (tagName === 'table')
        return null;
    if (tagName === 'form')
        return null;
    if (tagName === 'body')
        return null;
    if (tagName === 'html')
      return null;
    return GetSubmitTarget (element.parentNode);
  };

  // Returns true if the element is still attached to the document.
  function IsRooted (element)
  {
    if (TypeUtility.IsNull (element))
      return false;

    if (TypeUtility.IsUndefined (element.tagName))
      return false;

    var tagName = element.tagName.toLowerCase();
    if (tagName === 'html')
      return true;

    if (TypeUtility.IsUndefined (element.parentNode))
      return false;

    return IsRooted (element.parentNode);
  };

  // Gets the source element for the event.
  // evt: the event object. Used for Mozilla browsers.
  function GetEventSource(evt)
  {
    var e = TypeUtility.IsUndefined(evt) ? window.event : evt;
    if (e == null)
      return null;

    if (TypeUtility.IsDefined(e.target) && e.target != null)
      return e.target;
    else if (TypeUtility.IsDefined(e.srcElement) && e.srcElement != null)
      return e.srcElement;
    else
      return null;
  };

  function GetPageRequestManager ()
  {
    if (!TypeUtility.IsDefined (window.Sys))
      return null;
    if (!TypeUtility.IsDefined(Sys.WebForms))
      return null;
    if (!TypeUtility.IsDefined(Sys.WebForms.PageRequestManager))
      return null;
    return Sys.WebForms.PageRequestManager.getInstance();
  };

  function GetPostBackSettings()
  {
    var pageRequestManager = GetPageRequestManager();
    if (pageRequestManager == null)
      return null;
    return pageRequestManager._postBackSettings;
  }

  function IsSynchronousPostBackRequired(postBackSettings)
  {
    ArgumentUtility.CheckNotNull (postBackSettings);

    if (postBackSettings.async == false)
      return true;

    return Array.contains(_synchronousPostBackCommands, postBackSettings.asyncTarget + '|' + _theForm.__EVENTARGUMENT.value);
  };

  this.SetIsSubmitting = function (isAsynchronous, eventTarget, eventArgument)
  {
    var submitterElement = GetSubmitterOrActiveElement();
    if (submitterElement != null)
    {
      $(submitterElement).addClass('SmartPageSubmitter');
    }

    $('html').addClass('SmartPageBusy');

    var isAutoPostback = false;
    if (submitterElement != null)
    {
      var tagName = submitterElement.tagName.toLowerCase();
      if (tagName === 'input')
      {
        var type = submitterElement.type.toLowerCase();
        isAutoPostback = type !== 'submit' && type !== 'button';
      }
      else if (tagName === 'textarea')
      {
        isAutoPostback = true;
      }
      else if (tagName === 'select')
      {
        isAutoPostback = true;
      }
    }

    var submitState = {
      CancelSubmit: false,
      IsAsynchronous: isAsynchronous,
      IsAutoPostback: isAutoPostback,
      Submitter: submitterElement,
      EventTarget: eventTarget,
      EventArgument: eventArgument,
      NextSubmitState : null
    };
    if (_submitState == null)
      _submitState = submitState;
    else if (_submitState.IsAutoPostback && !isAutoPostback)
      _submitState.NextSubmitState = submitState;
    else
      _submitState.NextSubmitState = { CancelSubmit: true, Submitter: null };

    return _submitState;
  };

  this.ClearIsSubmitting = function (isAsynchronous)
  {
    if (isAsynchronous)
    {
      if (_submitState != null && _submitState.Submitter != null && _submitState.Submitter.ownerDocument != null)
      {
        $(_submitState.Submitter).removeClass('SmartPageSubmitter');
      }
      //setTimeout (function ()
    //{
      var html = $ ('html');
      html.removeClass('SmartPageBusy');

        // Needed in IE8, Firefox 23, Chrome 4
        // Does not work in Safari 4 for Windows
        // Does not work in IE10, IE11
        var cursorBackUp = html.css ('cursor');
        html.css ('cursor', 'auto !important');
        //setTimeout (function ()
        //{
          html.css ('cursor', cursorBackUp);
        //}, 0);
    //}, 0);
    }

    _submitState = null;
  };

  this.IsSubmitting = function ()
  {
    return _submitState != null;
  };

  this.DisableAbortConfirmation = function ()
  {
    _isAbortConfirmationEnabled = false;
  };

  this.ShowAbortConfirmation = function ()
  {
    if (_isAbortConfirmationEnabled && (!_isDirtyStateTrackingEnabled || _isDirty))
      return window.confirm(_abortMessage);
    else
      return true;
  };

  // Perform initialization
  this.Init();
}

// The single instance of the SmartPage_Context object
SmartPage_Context.Instance = null;

// Called after page's html content is complete.
function SmartPage_OnStartUp(isAsynchronous, isDirty)
{
  SmartPage_Context.Instance.OnStartUp(isAsynchronous, isDirty);
}

function SmartPage_PageRequestManager_pageLoaded(sender, args)
{
  SmartPage_Context.Instance.PageRequestManager_pageLoaded(sender, args);
}

function RenderThisHtml(theHtml)
{
  document.write(theHtml);
}
