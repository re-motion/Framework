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

type SmartPage_OutOfBandRequestSuccessHandler = (args: { Status: number}) => void;
type SmartPage_OutOfBandRequestErrorHandler = (args: { Status: number}) => void;

type SmartPage_SubmitState = {
  CancelSubmit: boolean;
  IsAsynchronous: boolean;
  IsAutoPostback: boolean;
  Submitter: Nullable<HTMLElement>;
  EventTarget: string;
  EventArgument: string;
  NextSubmitState: Nullable<SmartPage_SubmitState>;
};

type SmartPage_CheckFormState = (_isAborting: boolean, _hasSubmitted: boolean, _hasUnloaded: boolean, _isCached: boolean) => boolean;

type SmartPage_OnLoadingEventHandler = (this: null, hasSubmitted: boolean, isCached: boolean, isAsynchronous: boolean) => void;
type SmartPage_OnLoadedEventHandler = (this: null, hasSubmitted: boolean, isCached: boolean, isAsynchronous: boolean) => void;
type SmartPage_OnBeforeUnloadEventHandler = (this: null) => void;
type SmartPage_OnAbortEventHandler = (this: null, hasSubmitted: boolean, isCached: boolean) => void;
type SmartPage_OnUnloadEventHandler = (this: null) => void;
type SmartPage_OnPostbackEventHandler = (this: null, eventTarget: Nullable<string>, eventObject: string) => void;

type SmartPage_EventMap = {
  onloading: SmartPage_OnLoadingEventHandler,
  onloaded: SmartPage_OnLoadedEventHandler,
  onbeforeunload: SmartPage_OnBeforeUnloadEventHandler,
  onabort: SmartPage_OnAbortEventHandler,
  onunload: SmartPage_OnUnloadEventHandler,
  onpostback: SmartPage_OnPostbackEventHandler,
};
type SmartPage_Event = keyof SmartPage_EventMap;

enum SmartPage_CacheDetection
{
  Undefined,
  HasSubmitted,
  HasLoaded,
}

enum SmartPage_DirtyStates
{
  CurrentPage = 'CurrentPage',
  ClientSide = 'ClientSide'
}

// The context contains all information required by the smart page.
class SmartPage_Context
{
  private _theForm: HTMLFormElement;

  private _hasUnconditionalAbortConfirmation: boolean;
  private _dirtyStates: Set<string> = new Set<string>();

  // The message displayed when the user attempts to leave the page.
  // null to disable the message.
  private _abortMessage: Nullable<string>;
  private _isAbortConfirmationEnabled: boolean;

  // TODO RM-7700: Use a discriminated union for the submit state in the SmartPage_Context
  private _submitState: Nullable<SmartPage_SubmitState> = null;
  private _hasSubmitted: boolean = false;
  // Special flag to support the OnBeforeUnload part
  private _isSubmittingBeforeUnload: boolean = false;
  // The message displayed when the user attempts to submit while a submit is already in progress.
  // null to disable the message.
  private _statusIsSubmittingMessage: Nullable<string>;
  private _abortQueuedSubmit: boolean = false;
  private _lastManualSubmitter: Nullable<HTMLElement> = null;

  private _isAborting: boolean = false;
  private _isCached: boolean = false;
  // Special flag to support the OnBeforeUnload part
  private _isAbortingBeforeUnload: boolean = false;
  // Special flag to support conditional logic during OnBeforeUnload
  private _isOnBeforeUnloadExecuting: boolean = false;
  // Special flag to support conditional logic during OnPageHide
  private _isOnPageHideExecuting: boolean = false;

  // The name of the function used to evaluate whether to submit the form.
  // null if no external logic should be incorporated.
  private _checkFormStateFunctionName: Nullable<string>;

  private _statusMessageWindow: Nullable<HTMLElement> = null;
  private _hasUnloaded: boolean = false;

  /**
   * In Firefox, the load event will be triggered right after the postback, which is not the
   * case in other browsers and causes logic bugs. As such, we manually disable the load event
   * for a short amount of time right after a postback happens. The onload event should already
   * be queued by the browser due to the post back so we can use 0ms delay for the timeout.
   */
  private _ignoreOnLoadEventImmediatelyAfterPostBack: boolean = false;

  private _aspnetFormOnSubmit: Nullable<() => boolean> = null;
  private _aspnetDoPostBack: Nullable<Sys.WebForms.DoPostBack> = null;
  // Sepcial flag to support the Form.OnSubmit event being executed by the ASP.NET __doPostBack function.
  private _isExecutingDoPostBack: boolean = false;

  // The hidden field containing the smart scrolling data.
  private _smartScrollingFieldID: Nullable<string> = null;
  // The hidden field containing the smart focusing data.
  private _smartFocusFieldID: Nullable<string> = null;
  // The hidden field containing the page token guid.
  private _smartPageTokenFieldID: string;

  private _activeElement = null;
  // The hashtable of eventhandlers: Hashtable < event-key, Array < event-handler > >
  private _eventHandlers: string[][] = [];
  private _trackedIDs: string[] = [];
  private _synchronousPostBackCommands: string[] = [];

  private _pageTokenSessionStoragePrefix = 'pt_';
  private _pageTokenEntryCountKey = 'pageTokenEntryCount';

  private _pageTokenDeletionThreshold: number = 150;
  private _pageTokenDeletionKeepCount: number = 100;

  private _loadHandler = function () { SmartPage_Context.Instance!.OnLoad(); };
  private _beforeUnloadHandler = function () { return SmartPage_Context.Instance!.OnBeforeUnload(); };
  private _pagehideHandler = function (evt: PageTransitionEvent) { return SmartPage_Context.Instance!.OnPageHide(evt); };
  private _formSubmitHandler = function () { return SmartPage_Context.Instance!.OnFormSubmit(); };
  private _formClickHandler = function (evt: MouseEvent) { return SmartPage_Context.Instance!.OnFormClick(evt); };
  private _doPostBackHandler = function (eventTarget: string, eventArg: string) { SmartPage_Context.Instance!.DoPostBack(eventTarget, eventArg); };
  private _valueChangedHandler = function (evt: Event) { SmartPage_Context.Instance!.OnValueChanged(evt); };
  private _mouseDownHandler = function (evt: MouseEvent) { SmartPage_Context.Instance!.OnMouseDown(evt); };
  private _mouseUpHandler = function (evt: MouseEvent) { SmartPage_Context.Instance!.OnMouseUp(evt); };

  // theFormID: The ID of the HTML Form on the page.
  // hasUnconditionalAbortConfirmation: true if the page should always display the abort configuration regardless of the dirty state.
  // abortMessage: The message displayed when the user attempts to leave the page. null to disable the message.
  // statusIsSubmittingMessage: The message displayed when the user attempts to submit while a submit is already in 
  //    progress. null to disable the message.
  // smartScrollingFieldID: The ID of the hidden field containing the smart scrolling data.
  // smartFocusFieldID: The ID of the hidden field containing the smart focusing data.
  // checkFormStateFunctionName: The name of the function used to evaluate whether to submit the form.
  //    null if no external logic should be incorporated.
  // eventHandlers: The hashtable of eventhandlers: Hashtable < event-key, Array < event-handler > >
  constructor (
      theFormID: string,
      hasUnconditionalAbortConfirmation: boolean,
      abortMessage: Nullable<string>, statusIsSubmittingMessage: Nullable<string>,
      smartScrollingFieldID: Nullable<string>, smartFocusFieldID: Nullable<string>,
      smartPageTokenFieldID: string, checkFormStateFunctionName: Nullable<string>)
  {
    ArgumentUtility.CheckNotNullAndTypeIsString('theFormID', theFormID);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasUnconditionalAbortConfirmation', hasUnconditionalAbortConfirmation);
    ArgumentUtility.CheckTypeIsString('abortMessage', abortMessage);
    ArgumentUtility.CheckTypeIsString('statusIsSubmittingMessage', statusIsSubmittingMessage);
    ArgumentUtility.CheckTypeIsString('smartScrollingFieldID', smartScrollingFieldID);
    ArgumentUtility.CheckTypeIsString('smartFocusFieldID', smartFocusFieldID);
    ArgumentUtility.CheckNotNullAndTypeIsString('smartPageTokenFieldID', smartPageTokenFieldID);
    ArgumentUtility.CheckTypeIsString('checkFormStateFunctionName', checkFormStateFunctionName);

    this._hasUnconditionalAbortConfirmation = hasUnconditionalAbortConfirmation;

    this._abortMessage = abortMessage;
    this._isAbortConfirmationEnabled = abortMessage != null;

    this._statusIsSubmittingMessage = statusIsSubmittingMessage;

    this._checkFormStateFunctionName = checkFormStateFunctionName;

    this._theForm = window.document.forms[theFormID as any] as HTMLFormElement;
    {
      if (this._theForm == null)
        throw ('"' + theFormID + '" does not specify a Form.');
    }

    if (smartScrollingFieldID != null)
      this.GetFormElement(smartScrollingFieldID); // Ensures that the form element exists

    if (smartFocusFieldID != null)
      this.GetFormElement(smartFocusFieldID); // Ensures that the form element exists

    this._smartScrollingFieldID = smartScrollingFieldID;
    this._smartFocusFieldID = smartFocusFieldID;
    this._smartPageTokenFieldID = smartPageTokenFieldID

    this.AttachPageLevelEventHandlers();
  }

  public set_AbortQueuedSubmit (value: boolean): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('value', value);
    this._abortQueuedSubmit = value;
  };

  public set_EventHandlers (eventHandlers: string[][]): void
  {
    ArgumentUtility.CheckTypeIsObject('eventHandlers', eventHandlers);
    this._eventHandlers = eventHandlers;
  };

  public set_TrackedIDs (trackedIDs: string[]): void
  {
    ArgumentUtility.CheckTypeIsObject('trackedIDs', trackedIDs);
    this._trackedIDs = trackedIDs;
  };

  public set_SynchronousPostBackCommands (synchronousPostBackCommands: string[]): void
  {
    ArgumentUtility.CheckTypeIsObject('synchronousPostBackCommands', synchronousPostBackCommands);
    this._synchronousPostBackCommands = synchronousPostBackCommands;
  };

  // Attaches the event handlers to the page's events.
  private AttachPageLevelEventHandlers()
  {
    window.removeEventListener('load', this._loadHandler);
    window.addEventListener('load', this._loadHandler);

    // IE, Mozilla 1.7, Firefox 0.9
    window.onbeforeunload = this._beforeUnloadHandler;

    window.onpagehide = this._pagehideHandler;

    window.document.removeEventListener('mousedown', this._mouseDownHandler);
    window.document.addEventListener('mousedown', this._mouseDownHandler);

    window.document.removeEventListener('mouseup', this._mouseUpHandler);
    window.document.addEventListener('mouseup', this._mouseUpHandler);

    this._aspnetFormOnSubmit = this._theForm.onsubmit as Nullable<() => boolean>;
    this._theForm.onsubmit = this._formSubmitHandler;
    this._theForm.onclick = this._formClickHandler;
    if (TypeUtility.IsDefined(window.__doPostBack))
    {
      this._aspnetDoPostBack = window.__doPostBack;
      window.__doPostBack = this._doPostBackHandler;
    }
  };


  // Called after page's html content is complete.
  // Used to perform initalization code that only requires complete the HTML source but not necessarily all images.
  public OnStartUp (isAsynchronous: boolean, dirtyStates: Set<string>): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAsynchronous', isAsynchronous);
    ArgumentUtility.CheckNotNullAndTypeIsObject('dirtyStates', dirtyStates);

    this._dirtyStates = dirtyStates;

    this.AttachDataChangedEventHandlers();

    const pageRequestManager = this.GetPageRequestManager();
    if (pageRequestManager != null)
    {
      pageRequestManager.add_endRequest(this.SmartPage_PageRequestManager_endRequest);
    }
  };

  public SmartPage_PageRequestManager_endRequest = (sender: unknown, args: Sys.WebForms.EndRequestEventArgs) =>
  {
    if (args.get_error())
    {
      let statusCode = (args.get_error() as any).httpStatusCode;

      if (!statusCode || statusCode === 200)
      {
        this.HandleRequestError("undefined", args);
      }
      else
      {
        this.HandleRequestError(statusCode, args);
      }
    }
  }

  private HandleRequestError(statusCode: string, args: Sys.WebForms.EndRequestEventArgs): void
  {
    let response = args.get_response();
    let errorMessage = args.get_error().message;

    //@ts-ignore
    let serverError: string = Sys.WebForms.Res.PRM_ServerError;
    let isSynchronousError = errorMessage.includes(serverError.substring(0, serverError.indexOf(':')));

    let errorBody = '<div class="SmartPageErrorBody"><div>';

    const errorMessageWithoutExceptionTypePrefix = errorMessage.substring(errorMessage.indexOf(':') + 1);
    errorBody += errorMessageWithoutExceptionTypePrefix;

    if (isSynchronousError)
    {
      errorBody += ', ' + response.get_statusText();
      errorBody += '\n';

      errorBody += '<iframe sandbox="" src="data:text/html;base64,';
      errorBody += btoa(response.get_responseData());
      errorBody += '" ></iframe>';
    }

    errorBody += '</div></div>';

    args.set_errorHandled(true);
    SmartPage_Context.Instance!.ShowMessage("SmartPageServerErrorMessage", errorBody);
  }

  // Attached the OnValueChanged event handler to all form data elements listed in _trackedIDs.
  private AttachDataChangedEventHandlers(): void
  {
    for (let i = 0; i < this._trackedIDs.length; i++)
    {
      const id = this._trackedIDs[i];
      const element = this.GetFormElementOrNull(id);
      if (element == null)
        continue;

      const tagName = element.tagName.toLowerCase();

      if (tagName == 'input')
      {
        const type = (element as HTMLInputElement).type.toLowerCase();
        if (type == 'text' || type == 'hidden')
        {
          element.removeEventListener('change', this._valueChangedHandler);
          element.addEventListener('change', this._valueChangedHandler);
        }
        else if (type == 'checkbox' || type == 'radio')
        {
          element.removeEventListener('click', this._valueChangedHandler);
          element.addEventListener('click', this._valueChangedHandler);
        }
      }
      else if (tagName == 'textarea' || tagName == 'select')
      {
        element.removeEventListener('change', this._valueChangedHandler);
        element.addEventListener('change', this._valueChangedHandler);
      }
    }
  };

  // Event handler attached to the change event of tracked form elements
  public OnValueChanged(e: Event): void
  {
    this._dirtyStates.add(SmartPage_DirtyStates.ClientSide);
  };

  public OnMouseDown (e: MouseEvent): void
  {
    const isLeftButton = e.button === 0;
    const target = this.GetSubmitTarget (e.target as Nullable<HTMLElement>);
    if (isLeftButton && target !== null)
    {
      this._lastManualSubmitter = target;
    }
  };

  public OnMouseUp (e: MouseEvent): void
  {
    const lastTarget = this._lastManualSubmitter;
    this._lastManualSubmitter = null;

    if (lastTarget == null)
      return;

    if (this.IsRooted (lastTarget))
      return;

    const currentTarget = this.GetSubmitTarget (e.target as Nullable<HTMLElement>);
    if (currentTarget == null)
    {
      if (window.console)
        window.console.log ("A click on a submit-target was aborted because the submit target is no longer available while handling the mouse-up event. No action will be taken.");

      return;
    }

    let executeClick = false;
    if (TypeUtility.IsDefined (currentTarget.id) && TypeUtility.IsDefined (lastTarget.id) && !StringUtility.IsNullOrEmpty (lastTarget.id))
    {
      executeClick = currentTarget.id === lastTarget.id;
    }
    else
    {
      const lastTargetContent = lastTarget.innerHTML;
      const currentTargetContent = currentTarget.innerHTML;
      executeClick = currentTargetContent === lastTargetContent;
    }

    if (executeClick)
    {
      if (this.IsJavaScriptAnchor (currentTarget))
      {
        const href = (currentTarget as HTMLAnchorElement).href;
        eval (href);
      }
      else
      {
        currentTarget.click();
      }
    }
  }

  // Backs up the smart scrolling and smart focusing data for the next post back.
  public Backup(): void
  {
    if (this._smartScrollingFieldID != null)
      this.GetFormElement(this._smartScrollingFieldID).value = SmartScrolling.Backup();
    if (this._smartFocusFieldID != null)
      this.GetFormElement(this._smartFocusFieldID).value = SmartFocus.Backup();
  };

  // Restores the smart scrolling and smart focusing data from the previous post back.
  public Restore(): void
  {
    if (this._smartScrollingFieldID != null)
      SmartScrolling.Restore(this.GetFormElement(this._smartScrollingFieldID).value);
    if (this._smartFocusFieldID != null)
      SmartFocus.Restore(this.GetFormElement(this._smartFocusFieldID).value);
  };

  // Event handler for window.OnLoad
  public OnLoad(): void
  {
    if (this._ignoreOnLoadEventImmediatelyAfterPostBack)
      return;

    const pageRequestManager = this.GetPageRequestManager();
    if (pageRequestManager != null)
    {
      pageRequestManager.remove_pageLoaded(this.PageRequestManager_pageLoaded.bind(this));
      pageRequestManager.add_pageLoaded(this.PageRequestManager_pageLoaded.bind(this));
    }

    const isAsynchronous = false;
    this.PageLoaded(isAsynchronous);
  };

  private PageRequestManager_pageLoaded (sender: Sys.WebForms.PageRequestManager, args: Sys.WebForms.PageLoadedEventArgs): void
  {
    const isAsynchronous = sender && sender.get_isInAsyncPostBack();
    if (isAsynchronous)
    {
      this.PageLoaded(isAsynchronous);
    }
  };

  public PageLoaded (isAsynchronous: boolean): void
  {
    if (!isAsynchronous)
      this.CheckIfCached();

    this.Restore();

    this.ExecuteEventHandlers ('onloading', this._hasSubmitted, this._isCached, isAsynchronous);

    let isSubmitting = false;
    if (this.IsSubmitting() && this._submitState!.NextSubmitState != null && this._submitState!.NextSubmitState.Submitter != null && !this._abortQueuedSubmit)
    {
      const nextSubmitState = this._submitState!.NextSubmitState;
      this._submitState = null;
      if (!StringUtility.IsNullOrEmpty(nextSubmitState.EventTarget))
      {
        isSubmitting = true;
        setTimeout (function() { window.__doPostBack(nextSubmitState.EventTarget, nextSubmitState.EventArgument); }, 0);
      }
      else
      {
        const nextSubmitterID = nextSubmitState.Submitter!.id;
        const submitterElement = document.getElementById(nextSubmitterID);
        if (submitterElement != null)
        {
          let isButton = false;
          const tagName = submitterElement.tagName.toLowerCase();
          if (tagName === 'input')
          {
            const type = (submitterElement as HTMLInputElement).type.toLowerCase();
            isButton = type === 'submit' || type === 'button';
          }
          else if (tagName === 'button')
          {
            isButton = true;
          }

          if (isButton)
          {
            isSubmitting = true;
            setTimeout (function () { submitterElement.click(); }, 0);
          }
        }
      }
    }

    if (this._abortQueuedSubmit)
      this._lastManualSubmitter = null;

    if (!isSubmitting)
    {
      this.ClearIsSubmitting (isAsynchronous);
      this._isSubmittingBeforeUnload = false;
      this.HideStatusMessage();

      this.ExecuteEventHandlers ('onloaded', this._hasSubmitted, this._isCached, isAsynchronous);
    }
  };

  private GetPageToken(): string
  {
    return this.GetFormElement(this._smartPageTokenFieldID).value;
  }

  // Determines whether the page was loaded from cache.
  public CheckIfCached(): void
  {
    const pageToken = this.GetPageToken();
    const pageTokenValue = window.sessionStorage.getItem(this._pageTokenSessionStoragePrefix + pageToken);
    const cacheDetection = SmartPage_CacheDetection[pageTokenValue as keyof typeof SmartPage_CacheDetection];

    if (cacheDetection !== undefined)
    {
      window.history.pushState(history.state, "");
    }

    if (cacheDetection === SmartPage_CacheDetection.HasSubmitted)
    {
      this._hasSubmitted = true;
      this._isCached = true;
    }
    else if (cacheDetection === SmartPage_CacheDetection.HasLoaded)
    {
      this._isCached = true;
    }
    else
    {
      this.SetCacheDetectionFieldLoaded();
    }
  };

  // Marks the page as loaded.
  public SetCacheDetectionFieldLoaded(): void
  {
    this.SetCacheDetectionField(SmartPage_CacheDetection.HasLoaded);
  };

  // Marks the page as submitted.
  public SetCacheDetectionFieldSubmitted(): void
  {
    this.SetCacheDetectionField(SmartPage_CacheDetection.HasSubmitted);
  };

  private SetCacheDetectionField(cacheDetectionValue: SmartPage_CacheDetection): void
  {
    const tokenKey = this._pageTokenSessionStoragePrefix + this.GetPageToken();

    if (window.sessionStorage.getItem(tokenKey) === null)
    {
      this.IncreasePageTokenSessionStorageEntryCount();
      this.ClearOldPageTokenSessionStorageEntries();
    }

    window.sessionStorage.setItem(tokenKey, SmartPage_CacheDetection[cacheDetectionValue]);
  }

  private IncreasePageTokenSessionStorageEntryCount(): void
  {
    const currentCount = window.sessionStorage.getItem(this._pageTokenEntryCountKey);
    if (currentCount === null)
      window.sessionStorage.setItem(this._pageTokenEntryCountKey, (1).toString());
    else
      window.sessionStorage.setItem(this._pageTokenEntryCountKey, (parseInt(currentCount) + 1).toString());
  }

  private ClearOldPageTokenSessionStorageEntries(): void
  {
    const pageTokenEntryCountValue = window.sessionStorage.getItem(this._pageTokenEntryCountKey);

    if (pageTokenEntryCountValue === null)
      return;

    const pageTokenEntryCount = parseInt(pageTokenEntryCountValue);

    if (pageTokenEntryCount <= this._pageTokenDeletionThreshold)
      return;

    const pageTokenEntries = Object.keys(window.sessionStorage).filter(t => t.startsWith(this._pageTokenSessionStoragePrefix));
    const pageTokensToDelete = pageTokenEntries
      .map(t => ({ key: t, number: parseInt(t.substring(this._pageTokenSessionStoragePrefix.length)) }))
      .sort((a, b) => (a.number > b.number) ? 1 : -1)
      .splice(0, pageTokenEntryCount - this._pageTokenDeletionKeepCount);
    setTimeout(
      () =>
      {
        pageTokensToDelete.forEach(t => window.sessionStorage.removeItem(t.key));
        window.sessionStorage.setItem(this._pageTokenEntryCountKey, this._pageTokenDeletionKeepCount.toString());
      }, 0);
  }

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
  public OnBeforeUnload(): Optional<Nullable<string>>
  {
    this._isOnBeforeUnloadExecuting = true;
    try
    {
      this._isAbortingBeforeUnload = false;
      let displayAbortConfirmation = false;

      if (!this._hasUnloaded
        && !this._isCached
        && !this._isSubmittingBeforeUnload
        && !this._isAborting && this._isAbortConfirmationEnabled)
      {
        const submitterElement = this.GetSubmitterOrActiveElement();
        const isJavaScriptAnchor = this.IsJavaScriptAnchor(submitterElement);
        const isAbortConfirmationRequired = !isJavaScriptAnchor
          && (this._hasUnconditionalAbortConfirmation || this.IsDirty(undefined));

        if (isAbortConfirmationRequired)
        {
          this._isAbortingBeforeUnload = true;
          displayAbortConfirmation = true;
        }
      }
      else if (this._isSubmittingBeforeUnload)
      {
        this._isSubmittingBeforeUnload = false;
      }

      this.ExecuteEventHandlers('onbeforeunload');
      if (displayAbortConfirmation)
      {
        // IE alternate/official version: window.event.returnValue = SmartPage_Context.Instance.AbortMessage
        return this._abortMessage;
      }
    }
    finally
    {
      this._isOnBeforeUnloadExecuting = false;
    }
  };

  // Event handler for window.OnPageHide.
  public OnPageHide(evt: PageTransitionEvent)
  {
    this._isOnPageHideExecuting = true;
    try
    {
      if ((!this.IsSubmitting() || this._isAbortingBeforeUnload) && !this._isAborting)
      {
        this._isAborting = true;
        this.ExecuteEventHandlers ('onabort', this._hasSubmitted, this._isCached);
        this._isAbortingBeforeUnload = false;
      }
      this.ExecuteEventHandlers ('onunload');
      this._hasUnloaded = true;
      this.ClearIsSubmitting (false);
      this._isAborting = false;

      this._activeElement = null;
    }
    finally
    {
      this._isOnPageHideExecuting = false;
    }
  };

  // Override for the ASP.NET __doPostBack method.
  private DoPostBack (eventTarget: string, eventArgument: string): void
  {
    // Debugger space
    const dummy = 0;
    const continueRequest = this.CheckFormState();
    if (continueRequest)
    {
      try
      {
        this._isExecutingDoPostBack = true;
        this._theForm.__EVENTTARGET.value = eventTarget;
        this._theForm.__EVENTARGUMENT.value = eventArgument;

        const submitState = this.SetIsSubmitting(false, eventTarget, eventArgument);
        // Abort the postback if there is already a postback in progress
        if (submitState.NextSubmitState !== null)
          return;

        this._isSubmittingBeforeUnload = true;

        this.Backup();

        this.ExecuteEventHandlers('onpostback', eventTarget, eventArgument);
        this.SetCacheDetectionFieldSubmitted();

        this._aspnetDoPostBack!(eventTarget, eventArgument);

        // See field doc comment on why we disable the onload comment
        this._ignoreOnLoadEventImmediatelyAfterPostBack = true;
        setTimeout(() => this._ignoreOnLoadEventImmediatelyAfterPostBack = false, 0);
      }
      finally
      {
        this._theForm.__EVENTTARGET.value = '';
        this._theForm.__EVENTARGUMENT.value = '';
        this._isExecutingDoPostBack = false;
      }
    }
  };

  // Event handler for Form.Submit.
  public OnFormSubmit(): boolean
  {
    if (this._isExecutingDoPostBack)
    {
      if (this._aspnetFormOnSubmit != null)
        return this._aspnetFormOnSubmit();
      else
        return true;
    }
    else
    {
      const postBackSettings = this.GetPostBackSettings();
      if (postBackSettings != null && postBackSettings.async)
      {
        if (this.IsSynchronousPostBackRequired(postBackSettings))
        {
          this.DoPostBack(postBackSettings.asyncTarget!, this._theForm.__EVENTARGUMENT.value);
          return false;
        }
        else
        {
          const eventTarget = postBackSettings.asyncTarget === this._theForm.__EVENTTARGET.value ? postBackSettings.asyncTarget : '';
          const eventArgument = eventTarget !== '' ? this._theForm.__EVENTARGUMENT.value : '';
          const continueRequest = this.CheckFormState();
          const submitState = this.SetIsSubmitting(true, eventTarget, eventArgument);
          if (continueRequest && submitState.NextSubmitState === null)
          {
            this._isSubmittingBeforeUnload = true;

            this.Backup();
            this.ExecuteEventHandlers('onpostback', eventTarget, eventArgument);
            return true;
          }
          else
          {
            return false;
          }
        }
      }

      const continueRequest = this.CheckFormState();
      if (this.IsSubmitting() && continueRequest)
      {
        // This Code path is taken for synchronous requests that are not triggered via form-submit
        // and a postback is already in progress. In this case, the previous post state should be cleared.
        this._theForm.__EVENTTARGET.value = '';
        this._theForm.__EVENTARGUMENT.value = '';
      }

      const submitState = this.SetIsSubmitting(false, '', '');
      if (continueRequest && submitState.NextSubmitState === null)
      {
        this._isSubmittingBeforeUnload = true;

        this.Backup();

        const eventSource = this.GetSubmitterOrActiveElement();
        const eventSourceID = (eventSource != null) ? eventSource.id : null;
        this.ExecuteEventHandlers('onpostback', eventSourceID, '');
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
  public OnFormClick (evt: Nullable<MouseEvent>): Optional<boolean>
  {
    const postBackSettings = this.GetPostBackSettings();
    if (postBackSettings != null && this.IsSynchronousPostBackRequired(postBackSettings))
      return true;

    const eventSource = this.GetEventSource(evt);
    if (this.IsJavaScriptAnchor(eventSource))
    {
      const continueRequest = this.CheckFormState();
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
  public CheckFormState(): boolean
  {
    if (this._aspnetFormOnSubmit != null && !this._aspnetFormOnSubmit())
      return false;

    let continueRequest = true;
    const fct: Nullable<SmartPage_CheckFormState> = this._checkFormStateFunctionName
        ? this.GetFunctionPointer<SmartPage_CheckFormState>(this._checkFormStateFunctionName)
        : null;
    if (fct != null)
    {
      try
      {
        continueRequest = fct(this._isAborting, this._hasSubmitted, this._hasUnloaded, this._isCached);
      }
      catch (e)
      {
      }
    }

    if (!continueRequest)
      return false;

    if (!this._submitState)
      return true;

    const isAsyncAutoPostback = this._submitState.IsAsynchronous && this._submitState.IsAutoPostback;
    const isTriggeredByCurrentSubmitter = this._submitState.Submitter === this.GetActiveElement();
    const hasQueuedSubmit = this._submitState.NextSubmitState != null;
    if (!hasQueuedSubmit && isAsyncAutoPostback && !isTriggeredByCurrentSubmitter)
      return true;

    this.ShowStatusIsSubmittingMessage();
    return false;
  };

  // Sends an AJAX request to the server.
  // successHandler: function (args { Status }), called when the request succeeds
  // errorHandler: function (args { Status }), called when the request fails
  public SendOutOfBandRequest (url: string, successHandler: SmartPage_OutOfBandRequestSuccessHandler, errorHandler: SmartPage_OutOfBandRequestErrorHandler): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsString('url', url);
    ArgumentUtility.CheckNotNullAndTypeIsFunction('successHandler', successHandler);
    ArgumentUtility.CheckNotNullAndTypeIsFunction('errorHandler', errorHandler);

    if ((this._isOnBeforeUnloadExecuting || this._isOnPageHideExecuting) && navigator.sendBeacon)
    {
      const result = navigator.sendBeacon (url, null);
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
      const xhr = new XMLHttpRequest();

      const readStateDone = 4;
      const httpStatusSuccess = 299;
      const method = 'GET';
      const isAsyncCall = true;

      xhr.open (method, url, isAsyncCall);
      xhr.onreadystatechange = function ()
      {
        if (this.readyState === readStateDone)
        {
          const args = { Status : this.status };
          if (this.status > 0 && this.status <= httpStatusSuccess)
            successHandler (args);
          else
            errorHandler (args);
        }
      };
      xhr.send();
    }
  };

  // Executes the event handlers.
  // eventHandlers: an array of event handlers.
  private ExecuteEventHandlers<TEventName extends SmartPage_Event>(eventName: TEventName, ...args: Parameters<SmartPage_EventMap[TEventName]>)
  {
    const eventHandlers = (this._eventHandlers as unknown as { [K in SmartPage_Event]: Optional<Nullable<string[]>> })[eventName];
    if (!eventHandlers)
      return;

    for (let i = 0; i < eventHandlers.length; i++)
    {
      const eventHandler = this.GetFunctionPointer<(...args: Parameters<SmartPage_EventMap[TEventName]>) => void>(eventHandlers[i]);
      if (eventHandler != null)
      {
        try
        {
          eventHandler.apply(null, args);
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
  private GetFunctionPointer<TFunction extends AnyFunction>(functionName: string): Nullable<TFunction>
  {
    ArgumentUtility.CheckTypeIsString('functionName', functionName);
    if (StringUtility.IsNullOrEmpty(functionName))
      return null;
    let fct = null;
    try
    {
      fct = eval(functionName);
    }
    catch (e)
    {
      return null;
    }
    if (TypeUtility.IsFunction(fct))
      return fct as TFunction;
    else
      return null;
  };

  // Shows the status message informing the user that the page is already submitting.
  public ShowStatusIsSubmittingMessage(): void
  {
    if (this._statusIsSubmittingMessage != null)
      this.ShowMessage('SmartPageStatusIsSubmittingMessage', this._statusIsSubmittingMessage);
  };

  //  Shows a status message in the window using a DIV
  public ShowMessage (id: string, message: string): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsString ('id', id);
    ArgumentUtility.CheckNotNullAndTypeIsString ('message', message);

    if (this._statusMessageWindow == null)
    {
      const statusMessageWindow = window.document.createElement('DIV');
      statusMessageWindow.id = id;
      statusMessageWindow.style.width = '50%';
      statusMessageWindow.style.height = '50%';
      statusMessageWindow.style.left = '25%';
      statusMessageWindow.style.top = '25%';
      statusMessageWindow.style.position = 'absolute';

      const statusMessageBlock = window.document.createElement ('DIV');
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

      window.document.body.appendChild (statusMessageWindow);
      this.AlignStatusMessage (statusMessageWindow);
      this._statusMessageWindow = statusMessageWindow;
    }
  };

  // (Re-)Aligns the status message in the window.
  private AlignStatusMessage (message: HTMLElement): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsObject('message', message);

    const scrollLeft = window.document.body.scrollLeft;
    const scrollTop = window.document.body.scrollTop;
    const windowWidth = window.document.body.clientWidth;
    const windowHeight = window.document.body.clientHeight;

    // TODO RM-7699: SmartPage_Context.AlignStatusMessage incorrectly assigns styles to the specified message element
    message.style.left = (windowWidth / 2 - message.offsetWidth / 2 + scrollLeft) as any;
    message.style.top = (windowHeight / 2 - message.offsetHeight / 2 + scrollTop) as any;
  };

  public HideStatusMessage(): void
  {
    if (this._statusMessageWindow != null)
    {
      window.document.body.removeChild(this._statusMessageWindow);
      this._statusMessageWindow = null;
    }
  };

  // Returns the document.activeElement and uses the hovered element as fallback if the activeElement is not set.
  private GetActiveElement(): Nullable<HTMLElement>
  {
    const activeElement = window.document.activeElement;
    if (!TypeUtility.IsNull (activeElement) && !TypeUtility.IsUndefined (activeElement.tagName) && activeElement.tagName.toLowerCase() !== 'body')
      return activeElement as HTMLElement;

    // WebKit does not set activeElement if the element is selected using the mouse

    return document.querySelector<HTMLElement>('input:hover, button:hover, a:hover');
  }

  private GetDoPostBackSubmitterElement(): Nullable<HTMLElement>
  {
    const postBackSettings = this.GetPostBackSettings();
    if (postBackSettings != null && postBackSettings.async && postBackSettings.sourceElement != null)
    {
      if (this.IsFocusableTag (postBackSettings.sourceElement.tagName))
        return postBackSettings.sourceElement;
    }

    const eventTarget = this._theForm.__EVENTTARGET.value;
    if (eventTarget != '')
    {
      const eventTargetElement = this.GetFormElementOrNull(eventTarget);
      if (eventTargetElement != null)
        return eventTargetElement;
    }

    return null;
  }

  private GetSubmitterOrActiveElement (): Nullable<HTMLElement>
  {
    return this.GetDoPostBackSubmitterElement() || this.GetActiveElement();
  }

  // Determines whether the elements of the specified tag can receive the focus.
  private IsFocusableTag(tagName: Nullable<string>): boolean
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
  private IsJavaScriptAnchor(element: Nullable<HTMLElement>): boolean
  {
    if (TypeUtility.IsNull(element) || TypeUtility.IsUndefined(element.tagName))
      return false;
    ArgumentUtility.CheckTypeIsObject('element', element);

    const tagName = element.tagName.toLowerCase();
    if (tagName == 'a'
        && TypeUtility.IsDefined((element as HTMLAnchorElement).href) && (element as HTMLAnchorElement).href != null
        && (element as HTMLAnchorElement).href.substring(0, 'javascript:'.length).toLowerCase() == 'javascript:')
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
      return this.IsJavaScriptAnchor(element.parentNode! as HTMLElement);
    }
  };

  // Gets the button, input-submit, input-button, or anchor element associated with the current element or null if the element is not a valid target.
  private GetSubmitTarget (element: Nullable<HTMLElement>): Nullable<HTMLElement>
  {
    if (TypeUtility.IsNull (element))
      return null;

    if (TypeUtility.IsUndefined (element.tagName))
      return null;

    ArgumentUtility.CheckTypeIsObject('element', element);

    const tagName = element.tagName.toLowerCase();
    if (tagName === 'button')
      return element;
    if (tagName === 'a')
      return element;
    if (tagName === 'input')
    {
      if (!TypeUtility.HasPropertyOfType (element, "type", "string"))
        return null;
      const type = element.type.toLowerCase();
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
    return this.GetSubmitTarget (element.parentNode as Nullable<HTMLElement>);
  };

  // Returns true if the element is still attached to the document.
  private IsRooted (element: Nullable<HTMLElement>): boolean
  {
    if (TypeUtility.IsNull (element))
      return false;

    if (TypeUtility.IsUndefined (element.tagName))
      return false;

    const tagName = element.tagName.toLowerCase();
    if (tagName === 'html')
      return true;

    if (TypeUtility.IsUndefined (element.parentNode))
      return false;

    return this.IsRooted (element.parentNode as Nullable<HTMLElement>);
  };

  // Gets the source element for the event.
  // evt: the event object. Used for Mozilla browsers.
  private GetEventSource (evt: Optional<Nullable<Event>>): Nullable<HTMLElement>
  {
    const e = TypeUtility.IsUndefined(evt) ? window.event : evt;
    if (e == null)
      return null;

    if (TypeUtility.IsDefined(e.target) && e.target != null)
      return e.target as HTMLElement;
    else if (TypeUtility.IsDefined(e.srcElement) && e.srcElement != null)
      return e.srcElement as HTMLElement;
    else
      return null;
  };

  private GetPageRequestManager(): Nullable<Sys.WebForms.PageRequestManager>
  {
    if (!TypeUtility.IsDefined (window.Sys))
      return null;
    if (!TypeUtility.IsDefined(Sys.WebForms))
      return null;
    if (!TypeUtility.IsDefined(Sys.WebForms.PageRequestManager))
      return null;
    return Sys.WebForms.PageRequestManager.getInstance();
  };

  private GetPostBackSettings(): Nullable<Sys.WebForms.PostBackSettings>
  {
    const pageRequestManager = this.GetPageRequestManager();
    if (pageRequestManager == null || !TypeUtility.HasPropertyOfType(pageRequestManager, "_postBackSettings", "object"))
      return null;
    return pageRequestManager._postBackSettings as Sys.WebForms.PostBackSettings;
  }

  private IsSynchronousPostBackRequired(postBackSettings: Nullable<Sys.WebForms.PostBackSettings>): boolean
  {
    ArgumentUtility.CheckNotNull ("postBackSettings", postBackSettings);

    if (postBackSettings.async == false)
      return true;

    return Array.contains(this._synchronousPostBackCommands, postBackSettings.asyncTarget + '|' + this._theForm.__EVENTARGUMENT.value);
  };

  public SetIsSubmitting (isAsynchronous: boolean, eventTarget: string, eventArgument: string): SmartPage_SubmitState
  {
    const submitterElement = this.GetSubmitterOrActiveElement();
    if (submitterElement != null)
    {
      submitterElement.classList.add('SmartPageSubmitter');
    }

    document.documentElement.classList.add('SmartPageBusy');

    let isAutoPostback = false;
    if (submitterElement != null)
    {
      const tagName = submitterElement.tagName.toLowerCase();
      if (tagName === 'input')
      {
        const type = (submitterElement as HTMLInputElement).type.toLowerCase();
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

    const submitState: SmartPage_SubmitState = {
      CancelSubmit: false,
      IsAsynchronous: isAsynchronous,
      IsAutoPostback: isAutoPostback,
      Submitter: submitterElement,
      EventTarget: eventTarget,
      EventArgument: eventArgument,
      NextSubmitState : null
    };
    if (this._submitState == null)
    {
      this._submitState = submitState;
    }
    else if (this._submitState.NextSubmitState == null)
    {
      if (this._submitState.IsAutoPostback && !isAutoPostback)
      {
        this._submitState.NextSubmitState = submitState;
      }
      else
      {
        // TODO RM-7700: Use a discriminated union for the submit state in the SmartPage_Context
        this._submitState.NextSubmitState = { CancelSubmit: true, Submitter: null } as any;
      }
    }

    return this._submitState;
  };

  public ClearIsSubmitting (isAsynchronous: boolean): void
  {
    if (isAsynchronous)
    {
      if (this._submitState != null && this._submitState.Submitter != null && this._submitState.Submitter.ownerDocument != null)
      {
        this._submitState.Submitter.classList.remove('SmartPageSubmitter');
      }

      const html = document.documentElement;
      html.classList.remove('SmartPageBusy');

      //setTimeout (function ()
    //{
      //const html = document.documentElement;
      html.classList.remove('SmartPageBusy');

        // Needed in IE8, Firefox 23, Chrome 4
        // Does not work in Safari 4 for Windows
        // Does not work in IE10, IE11
        const cursorBackUp = window.getComputedStyle(html)["cursor"];
        html.style.cursor = 'auto !important';
        //setTimeout (function ()
        //{
          html.style.cursor = cursorBackUp;
        //}, 0);
    //}, 0);
    }

    this._submitState = null;
  };

  public IsSubmitting(): boolean
  {
    return this._submitState != null;
  };

  public IsDirty(conditions: Optional<string[]>): boolean
  {
    if (this._dirtyStates == null)
      return false;

    if (conditions)
      return conditions.some(c => this._dirtyStates.has(c));

    return this._dirtyStates.size > 0;
  }

  public DisableAbortConfirmation(): void
  {
    this._isAbortConfirmationEnabled = false;
  };

  public ShowAbortConfirmation(conditions: Optional<string[]>): boolean
  {
    if (this._isAbortConfirmationEnabled && (this._hasUnconditionalAbortConfirmation || this.IsDirty(conditions)))
      return window.confirm(this._abortMessage!);
    else
      return true;
  };

  private GetFormElement(name: string): HTMLInputElement
  {
    const formElement = this.GetFormElementOrNull(name);
    if (formElement == null)
      throw ('"' + name + '" does not specify a element of Form "' + this._theForm.id + '".');

    return formElement;
  }

  private GetFormElementOrNull(name: string): Nullable<HTMLInputElement>
  {
    const element = this._theForm.elements[name as any] as Optional<Nullable<HTMLInputElement>> || null;
    if (element instanceof RadioNodeList)
    {
      console.error(`${element.length} elements were found for the given id '${name}'. The first found element was returned to ensure graceful execution.`);
      return element[0] as HTMLInputElement;
    }

    return element;
  }

  // The single instance of the SmartPage_Context object
  public static Instance: Nullable<SmartPage_Context> = null; // TODO RM-7696: Convert SmartPage_Context.Instance to a get method and throw an exception when it is called before initialization
}
