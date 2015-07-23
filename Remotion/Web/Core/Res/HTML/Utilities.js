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
function TypeUtility()
{
}

TypeUtility.IsObject = function (value)
{
    return typeof (value) == 'object';
};

TypeUtility.IsString = function (value)
{
    return typeof (value) == 'string';
};

TypeUtility.IsNumber = function (value)
{
    return typeof (value) == 'number';
};

TypeUtility.IsInteger = function (value)
{
  return TypeUtility.IsNumber (value) && value %1 === 0;
};

TypeUtility.IsBoolean = function (value)
{
    return typeof (value) == 'boolean';
};

TypeUtility.IsFunction = function (value)
{
    return typeof (value) == 'function';
};

TypeUtility.IsUndefined = function (value)
{
    return typeof (value) == 'undefined';
};

TypeUtility.IsDefined = function (value)
{
  return !TypeUtility.IsUndefined(value);
};

TypeUtility.IsNull = function (value)
{
  return TypeUtility.IsDefined(value) && value == null;
};


function StringUtility()
{
}

StringUtility.IsNullOrEmpty = function (value)
{
    ArgumentUtility.CheckTypeIsString('value', value);
    return TypeUtility.IsNull(value) || value.length == 0;
};


function ArgumentUtility()
{
}

// Checks that value is not null.
ArgumentUtility.CheckNotNull = function (name, value)
{
    if (TypeUtility.IsNull(value))
        throw ('Error: The value of parameter "' + name + '" is null.');
};

// Checks that value is not null and of type string.
ArgumentUtility.CheckTypeIsString = function (name, value)
{
    if (TypeUtility.IsNull(value))
        return;
    if (!TypeUtility.IsString(value))
        throw ('Error: The value of parameter "' + name + '" is not a string.');
};

// Checks that value is not null and of type string.
ArgumentUtility.CheckNotNullAndTypeIsString = function (name, value)
{
    ArgumentUtility.CheckNotNull(name, value);
    ArgumentUtility.CheckTypeIsString(name, value);
};

// Checks that value is not null and of type string.
ArgumentUtility.CheckTypeIsObject = function (name, value)
{
    if (TypeUtility.IsNull(value))
        return;
    if (!TypeUtility.IsObject(value))
        throw ('Error: The value of parameter "' + name + '" is not an object.');
};

// Checks that value is not null and of type string.
ArgumentUtility.CheckNotNullAndTypeIsObject = function (name, value)
{
    ArgumentUtility.CheckNotNull(name, value);
    ArgumentUtility.CheckTypeIsObject(name, value);
};

// Checks that value is not null and of type number.
ArgumentUtility.CheckTypeIsNumber = function (name, value)
{
    if (TypeUtility.IsNull(value))
        return;
    if (!TypeUtility.IsNumber(value))
        throw ('Error: The value of parameter "' + name + '" is not a number.');
};

// Checks that value is not null and of type number.
ArgumentUtility.CheckNotNullAndTypeIsNumber = function (name, value)
{
    ArgumentUtility.CheckNotNull(name, value);
    ArgumentUtility.CheckTypeIsNumber(name, value);
};

// Checks that value is not null and of type boolean.
ArgumentUtility.CheckTypeIsBoolean = function (name, value)
{
    if (TypeUtility.IsNull(value))
        return;
    if (!TypeUtility.IsBoolean(value))
        throw ('Error: The value of parameter "' + name + '" is not a boolean.');
};

// Checks that value is not null and of type boolean.
ArgumentUtility.CheckNotNullAndTypeIsBoolean = function (name, value)
{
    ArgumentUtility.CheckNotNull(name, value);
    ArgumentUtility.CheckTypeIsBoolean(name, value);
};

// Checks that value is not null and of type function.
ArgumentUtility.CheckTypeIsFunction = function (name, value)
{
    if (TypeUtility.IsNull(value))
        return;
    if (!TypeUtility.IsFunction(value))
        throw ('Error: The value of parameter "' + name + '" is not a function.');
};

// Checks that value is not null and of type function.
ArgumentUtility.CheckNotNullAndTypeIsFunction = function (name, value)
{
    ArgumentUtility.CheckNotNull(name, value);
    ArgumentUtility.CheckTypeIsFunction(name, value);
};

function BrowserUtility ()
{
}

BrowserUtility.GetIEVersion = function ()
{
  if ($.browser.msie)
  {
    var majorVersion;
    if (TypeUtility.IsDefined (window.document.documentMode))
      majorVersion = parseInt (window.document.documentMode);
    else
      majorVersion = parseInt ($.browser.version);

    return majorVersion;
  }
  else
  {
    return NaN;
  }
};

function PageUtility()
{
  var _resizeHandlers = new Array();
  var _resizeTimeoutID = null;
  var _resizeTimeoutInMilliSeconds = 50;

  $(document).ready(function()
  {
    $(window).bind('resize', function() { PageUtility.Instance.PrepareExecuteResizeHandlers(); });
  });

  this.RegisterResizeHandler = function (selector, handler)
  {
    ArgumentUtility.CheckNotNullAndTypeIsString ('selector', selector);
    ArgumentUtility.CheckNotNull ('handler', handler);

    for (var i = 0; i < _resizeHandlers.length; i++)
    {
      var item = _resizeHandlers[i];
      if (item.Selector == selector)
      {
        item.handler = handler;
        return;
      }
    }
    _resizeHandlers[_resizeHandlers.length] = new PageUtility_ResizeHandlerItem (selector, handler);
  };

  this.PrepareExecuteResizeHandlers = function ()
  {
    if (_resizeTimeoutID != null)
      window.clearTimeout (_resizeTimeoutID);

    _resizeTimeoutID = window.setTimeout (function () { PageUtility.Instance.ExecuteResizeHandlers(); }, _resizeTimeoutInMilliSeconds);
  };

  this.ExecuteResizeHandlers = function ()
  {
    var existingResizeHandlers = new Array();
    for (var i = 0; i < _resizeHandlers.length; i++)
    {
      var item = _resizeHandlers[i];
      var element = $ (item.Selector);
      if (element != null && element.length > 0)
      {
        item.Handler (element);
        existingResizeHandlers[existingResizeHandlers.length] = item;
      }
    }
    _resizeHandlers = existingResizeHandlers;
  };

  this.IsInDom = function (element)
  {
    ArgumentUtility.CheckNotNull('element', element);

    var html = window.document.body.parentNode;
    while (element)
    {
      if (element === html)
          return true;
      element = element.parentNode;
    }
    return false;
  }
}

function PageUtility_ResizeHandlerItem(selector, handler)
{
  this.Selector = selector;
  this.Handler = handler;
}

PageUtility.Instance = new PageUtility();
