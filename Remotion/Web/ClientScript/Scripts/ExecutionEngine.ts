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

enum WxePage_DirtyStates
{
  CurrentFunction = 'CurrentFunction',
  RootFunction = 'RootFunction'
}

// The context contains all information required by the WXE page.
class WxePage_Context
{
  // The URL used to post the refresh request to.
  private _refreshUrl: Nullable<string> = null;
  // The timer used to invoke the refreshing.
  private _refreshTimer: Nullable<number> = null;

  private _httpStatusFunctionTimeout = 409;

  // The URL used to post the abort request to.
  private _abortUrl: Nullable<string>;
  private _isAbortEnabled: boolean;

  // The message displayed when the user attempts to submit while an abort is in progress. null to disable the message.
  private _statusIsAbortingMessage: Nullable<string>;
  // The message displayed when the user returns to a cached page. null to disable the message.
  private _statusIsCachedMessage: Nullable<string>;

  private _isCacheDetectionEnabled: boolean;

  private _wxePostBackSequenceNumberFieldID: Nullable<string>;
  private _dmaWxePostBackSequenceNumberFieldID : Nullable<string>;

  // isCacheDetectionEnabled: true to detect whether the user is viewing a cached page.
  // refreshInterval: The refresh interfal in milli-seconds. zero to disable refreshing.
  // refreshUrl: The URL used to post the refresh request to. Must not be null if refreshInterval is greater than zero.
  // abortUrl: The URL used to post the abort request to. null to disable the abort request.
  // statusIsAbortingMessage: The message displayed when the user attempts to submit while an abort is in progress. 
  //    null to disable the message.
  // statusIsCachedMessage: The message displayed when the user returns to a cached page. null to disable the message.
  // wxePostBackSequenceNumberFieldID: The ID of the WXE postback sequence number.
  // dmaWxePostBackSequenceNumberFieldID: The ID of the DMA WXE postback sequence number. null to disable the updating of
  //    the field.
  constructor (
    isCacheDetectionEnabled: boolean,
    refreshInterval: number, refreshUrl: Nullable<string>,
    abortUrl: Nullable<string>,
    statusIsAbortingMessage: Nullable<string>, statusIsCachedMessage: Nullable<string>,
    wxePostBackSequenceNumberFieldID: Nullable<string>,
    dmaWxePostBackSequenceNumberFieldID: Nullable<string>)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCacheDetectionEnabled', isCacheDetectionEnabled);
    ArgumentUtility.CheckNotNullAndTypeIsNumber('refreshInterval', refreshInterval);
    ArgumentUtility.CheckTypeIsString('refreshUrl', refreshUrl);
    ArgumentUtility.CheckTypeIsString('abortUrl', abortUrl);
    ArgumentUtility.CheckTypeIsString('statusIsAbortingMessage', statusIsAbortingMessage);
    ArgumentUtility.CheckTypeIsString('statusIsCachedMessage', statusIsCachedMessage);
    ArgumentUtility.CheckTypeIsString('wxePostBackSequenceNumberFieldID', wxePostBackSequenceNumberFieldID);
    ArgumentUtility.CheckTypeIsString('dmaWxePostBackSequenceNumberFieldID', dmaWxePostBackSequenceNumberFieldID);

    if (refreshInterval > 0)
    {
      ArgumentUtility.CheckNotNull('refreshUrl', refreshUrl);
      this._refreshUrl = refreshUrl;
      this._refreshTimer = window.setInterval(() => { this.Refresh(); }, refreshInterval);
    };

    this._abortUrl = abortUrl;
    this._isAbortEnabled = abortUrl != null;

    this._statusIsAbortingMessage = statusIsAbortingMessage;
    this._statusIsCachedMessage = statusIsCachedMessage;

    this._isCacheDetectionEnabled = isCacheDetectionEnabled;

    this._wxePostBackSequenceNumberFieldID = wxePostBackSequenceNumberFieldID;
    this._dmaWxePostBackSequenceNumberFieldID = dmaWxePostBackSequenceNumberFieldID;
  }

  // Handles the page loading event.
  public OnLoading (hasSubmitted: boolean, isCached: boolean, isAsynchronous: boolean): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCached', isCached);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAsynchronous', isAsynchronous);

    if (this._dmaWxePostBackSequenceNumberFieldID != null)
    {
      const dmaWxePostBackSequenceNumberField = document.getElementById(this._dmaWxePostBackSequenceNumberFieldID) as HTMLInputElement;
      const postBackSequenceNumber = (document.getElementById(this._wxePostBackSequenceNumberFieldID!) as HTMLInputElement).value;
      dmaWxePostBackSequenceNumberField.value = postBackSequenceNumber;
    }

    if (!isAsynchronous && this._refreshTimer != null)
    {
      // The lock remains alive until the page is reloaded using a GET-request or synchronous post-back.
      this.EstablishPageKeepAliveLock();
    }
  };

  // Handles the page loaded event.
  public OnLoaded (hasSubmitted: boolean, isCached: boolean, isAsynchronous: boolean): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCached', isCached);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAsynchronous', isAsynchronous);

    if (this._isCacheDetectionEnabled
        && (isCached || hasSubmitted))
    {
      this.ShowStatusIsCachedMessage();
    }
  };

  public OnUnload(): void
  {
    this.Dispose();
  }

  // Handles the page abort event.
  public OnAbort (hasSubmitted: boolean, isCached: boolean): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCached', isCached);

    if (this._isAbortEnabled && this._abortUrl !== null
        && (this._isCacheDetectionEnabled && (!isCached || hasSubmitted)))
    {
      const successHandler: SmartPage_OutOfBandRequestSuccessHandler = function (args) {};
      const errorHandler: SmartPage_OutOfBandRequestErrorHandler = function (args) {};
      SmartPage_Context.Instance!.SendOutOfBandRequest(this._abortUrl, successHandler, errorHandler);
    }
  };

  public Dispose(): void
  {
    if (this._refreshTimer != null)
      window.clearInterval(this._refreshTimer);
  }

  // Handles the refresh timer events
  public Refresh(): void
  {
    const successHandler: SmartPage_OutOfBandRequestSuccessHandler = function (args) {};
    const errorHandler: SmartPage_OutOfBandRequestErrorHandler = (args) =>
    {
      if (args.Status === this._httpStatusFunctionTimeout)
      {
        if (window.console)
          window.console.warn ('WXE function has timed out. Stopping refresh- and abort-requests.');

        if (this._refreshTimer !== null)
        {
          window.clearInterval (this._refreshTimer);
          this._refreshTimer = null;
        }
        this._isAbortEnabled = false;
      }
    };
    SmartPage_Context.Instance!.SendOutOfBandRequest(this._refreshUrl + '&WxePage_Garbage=' + Math.random(), successHandler, errorHandler);
  };

  // Evaluates whether the postback request should continue.
  // returns: true to continue with request
  public CheckFormState (isAborting: boolean, hasSubmitted: boolean, hasUnloaded: boolean, isCached: boolean): boolean
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAborting', isAborting);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasUnloaded', hasUnloaded);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCached', isCached);

    if (this._isCacheDetectionEnabled
        && (isCached || hasSubmitted || hasUnloaded))
    {
      this.ShowStatusIsCachedMessage();
      return false;
    }
    if (isAborting)
    {
      this.ShowStatusIsAbortingMessage();
      return false;
    }
    else
    {
      return true;
    }
  };

  // Shows the "page is aborting" message
  public ShowStatusIsAbortingMessage(): void
  {
    if (this._statusIsAbortingMessage != null)
      SmartPage_Context.Instance!.ShowMessage('WxeStatusIsAbortingMessage', this._statusIsAbortingMessage);
  };

  // Shows the "page is cached" message
  public ShowStatusIsCachedMessage(): void
  {
    if (this._statusIsCachedMessage != null)
      SmartPage_Context.Instance!.ShowMessage('WxeStatusIsCachedMessage', this._statusIsCachedMessage);
  };

  public EstablishPageKeepAliveLock = function(): void
  {
    const lockName = btoa("" + (Math.random() * 10e18));
    let lockIndex = 0;

    function onLockTaken(lock: Nullable<Lock>)
    {
      if (lock === null)
      {
        lockIndex++;
        navigator.locks!.request(lockName + lockIndex, { ifAvailable: true }, onLockTaken);
        return null;
      }
      else
      {
        return new Promise(function (resolve, reject) { });
      }
    }

    // The navigator.locks API is only available for secure origins, i.e. sites using HTTPS.
    // The navigator.locks API is only available in Chromium-based browsers, versions 69.* and later.
    if (navigator.locks && navigator.locks.request)
    {
      navigator.locks.request(lockName + lockIndex, { ifAvailable: true }, onLockTaken);
    }
  }

  // The single instance of the WxePage_Context object
  private static _instance: Nullable<WxePage_Context> = null;

  public static GetInstance(): Nullable<WxePage_Context>
  {
    // TODO RM-7695: Throw an exception when WxePage_Context.GetInstance() is called before initialization
    return WxePage_Context._instance;
  }

  public static SetInstance (instance: WxePage_Context): void
  {
    ArgumentUtility.CheckNotNull('instance', instance);

    if (WxePage_Context._instance != null)
      WxePage_Context._instance.Dispose();

    WxePage_Context._instance = instance;
  }
}

function WxePage_OnLoading(hasSubmitted: boolean, isCached: boolean, isAsynchronous: boolean): void
{
  WxePage_Context.GetInstance()!.OnLoading(hasSubmitted, isCached, isAsynchronous);
}

function WxePage_OnLoaded(hasSubmitted: boolean, isCached: boolean, isAsynchronous: boolean): void
{
  WxePage_Context.GetInstance()!.OnLoaded(hasSubmitted, isCached, isAsynchronous);
}

function WxePage_OnUnload(): void
{
  WxePage_Context.GetInstance()!.OnUnload();
}

function WxePage_OnAbort(hasSubmitted: boolean, isCached: boolean): void
{
  WxePage_Context.GetInstance()!.OnAbort(hasSubmitted, isCached);
}

function WxePage_CheckFormState(isAborting: boolean, hasSubmitted: boolean, hasUnloaded: boolean, isCached: boolean): boolean
{
  return WxePage_Context.GetInstance()!.CheckFormState(isAborting, hasSubmitted, hasUnloaded, isCached);
}
