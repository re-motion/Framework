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

// The context contains all information required by the WXE page.
// isCacheDetectionEnabled: true to detect whether the user is viewing a cached page.
// refreshInterval: The refresh interfal in milli-seconds. zero to disable refreshing.
// refreshUrl: The URL used to post the refresh request to. Must not be null if refreshInterval is greater than zero.
// abortUrl: The URL used to post the abort request to. null to disable the abort request.
// statusIsAbortingMessage: The message displayed when the user attempts to submit while an abort is in progress. 
//    null to disable the message.
// statusIsCachedMessage: The message displayed when the user returns to a cached page. null to disable the message.
function WxePage_Context(
      isCacheDetectionEnabled,
      refreshInterval, refreshUrl,
      abortUrl,
      statusIsAbortingMessage, statusIsCachedMessage)
{
  ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCacheDetectionEnabled', isCacheDetectionEnabled);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('refreshInterval', refreshInterval);
  ArgumentUtility.CheckTypeIsString('refreshUrl', refreshUrl);
  ArgumentUtility.CheckTypeIsString('abortUrl', abortUrl);
  ArgumentUtility.CheckTypeIsString('statusIsAbortingMessage', statusIsAbortingMessage);
  ArgumentUtility.CheckTypeIsString('statusIsCachedMessage', statusIsCachedMessage);

  // The URL used to post the refresh request to.
  var _refreshUrl = null;
  // The timer used to invoke the refreshing.
  var _refreshTimer = null;
  if (refreshInterval > 0)
  {
    ArgumentUtility.CheckNotNull('refreshUrl', refreshUrl);
    _refreshUrl = refreshUrl;
    _refreshTimer = window.setInterval(function() { WxePage_Context._instance.Refresh(); }, refreshInterval);
  };

  // The URL used to post the abort request to.
  var _abortUrl = abortUrl;
  var _isAbortEnabled = abortUrl != null;

  // The message displayed when the user attempts to submit while an abort is in progress. null to disable the message.
  var _statusIsAbortingMessage = statusIsAbortingMessage;
  // The message displayed when the user returns to a cached page. null to disable the message.
  var _statusIsCachedMessage = statusIsCachedMessage;

  var _isCacheDetectionEnabled = isCacheDetectionEnabled;

  // Handles the page load event.
  this.OnLoad = function(hasSubmitted, isCached)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCached', isCached);

    if (_isCacheDetectionEnabled
        && (isCached || hasSubmitted))
    {
      this.ShowStatusIsCachedMessage();
    }
  };

  this.OnUnload = function()
  {
    this.Dispose();
  }

  // Handles the page abort event.
  this.OnAbort = function(hasSubmitted, isCached)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCached', isCached);

    if (_isAbortEnabled
        && (_isCacheDetectionEnabled && (!isCached || hasSubmitted)))
    {
      SmartPage_Context.Instance.SendOutOfBandRequest(_abortUrl);
    }
  };

  this.Dispose = function()
  {
    if (_refreshTimer != null)
      window.clearInterval(_refreshTimer);
  }

  // Handles the refresh timer events
  this.Refresh = function()
  {
    SmartPage_Context.Instance.SendOutOfBandRequest(_refreshUrl + '&WxePage_Garbage=' + Math.random())
  };

  // Evaluates whether the postback request should continue.
  // returns: true to continue with request
  this.CheckFormState = function(isAborting, hasSubmitted, hasUnloaded, isCached)
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAborting', isAborting);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasSubmitted', hasSubmitted);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('hasUnloaded', hasUnloaded);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isCached', isCached);

    if (_isCacheDetectionEnabled
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
  this.ShowStatusIsAbortingMessage = function()
  {
    if (_statusIsAbortingMessage != null)
      SmartPage_Context.Instance.ShowMessage('WxeStatusIsAbortingMessage', _statusIsAbortingMessage);
  };

  // Shows the "page is cached" message
  this.ShowStatusIsCachedMessage = function()
  {
    if (_statusIsCachedMessage != null)
      SmartPage_Context.Instance.ShowMessage('WxeStatusIsCachedMessage', _statusIsCachedMessage);
  };
}

// The single instance of the WxePage_Context object
WxePage_Context._instance = null;

WxePage_Context.GetInstance = function()
{
  return WxePage_Context._instance;
}

WxePage_Context.SetInstance = function(instance)
{
  ArgumentUtility.CheckNotNull('instance', instance);

  if (WxePage_Context._instance != null)
    WxePage_Context._instance.Dispose();

  WxePage_Context._instance = instance;
}

function WxePage_OnLoad(hasSubmitted, isCached)
{
  WxePage_Context._instance.OnLoad(hasSubmitted, isCached);
}

function WxePage_OnUnload()
{
  WxePage_Context._instance.OnUnload();
}

function WxePage_OnAbort(hasSubmitted, isCached)
{
  WxePage_Context._instance.OnAbort(hasSubmitted, isCached);
}

function WxePage_CheckFormState(isAborting, hasSubmitted, hasUnloaded, isCached)
{
  return WxePage_Context._instance.CheckFormState(isAborting, hasSubmitted, hasUnloaded, isCached);
}
