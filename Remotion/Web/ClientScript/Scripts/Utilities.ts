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
class TypeUtility
{
  public static IsObject (value: unknown): value is object
  {
      return typeof (value) == 'object';
  };
  
  public static IsString (value: unknown): value is string
  {
      return typeof (value) == 'string';
  };
  
  public static IsNumber (value: unknown): value is number
  {
      return typeof (value) == 'number';
  };
  
  public static IsInteger (value: unknown): value is number
  {
    return TypeUtility.IsNumber (value) && value %1 === 0;
  };
  
  public static IsBoolean (value: unknown): value is boolean
  {
      return typeof (value) == 'boolean';
  };
  
  public static IsFunction (value: unknown): value is AnyFunction
  {
      return typeof (value) == 'function';
  };
  
  public static IsJQuery (value: unknown): value is JQuery
  {
    return value instanceof jQuery;
  };
  
  public static IsUndefined (value: unknown): value is undefined
  {
      return typeof (value) == 'undefined';
  };
  
  public static IsDefined (value: unknown): value is NotUndefined
  {
    return !TypeUtility.IsUndefined(value);
  };
  
  public static IsNull (value: unknown): value is null
  {
    return TypeUtility.IsDefined(value) && value == null;
  };
}


class StringUtility
{
  public static IsNullOrEmpty (value: Nullable<string>): value is null | ""
  {
      ArgumentUtility.CheckTypeIsString('value', value);
      return TypeUtility.IsNull(value) || value.length == 0;
  };
}


class ArgumentUtility
{
  // Checks that value is not null.
  public static CheckNotNull (name: string, value: unknown): asserts value is NotNull
  {
      if (TypeUtility.IsNull(value))
          throw ('Error: The value of parameter "' + name + '" is null.');
  };

  // Checks that value is not null and of type string.
  public static CheckTypeIsString (name: string, value: unknown): asserts value is Nullable<string>
  {
      if (TypeUtility.IsNull(value))
          return;
      if (!TypeUtility.IsString(value))
          throw ('Error: The value of parameter "' + name + '" is not a string.');
  };

  // Checks that value is not null and of type string.
  public static CheckNotNullAndTypeIsString (name: string, value: unknown): asserts value is string
  {
      ArgumentUtility.CheckNotNull(name, value);
      ArgumentUtility.CheckTypeIsString(name, value);
  };

  // Checks that value is not null and of type string.
  public static CheckTypeIsObject (name: string, value: unknown): asserts value is Nullable<object>
  {
      if (TypeUtility.IsNull(value))
          return;
      if (!TypeUtility.IsObject(value))
          throw ('Error: The value of parameter "' + name + '" is not an object.');
  };

  // Checks that value is not null and of type string.
  public static CheckNotNullAndTypeIsObject (name: string, value: unknown): asserts value is object
  {
      ArgumentUtility.CheckNotNull(name, value);
      ArgumentUtility.CheckTypeIsObject(name, value);
  };

  // Checks that value is not null and of type number.
  public static CheckTypeIsNumber (name: string, value: unknown): asserts value is Nullable<number>
  {
      if (TypeUtility.IsNull(value))
          return;
      if (!TypeUtility.IsNumber(value))
          throw ('Error: The value of parameter "' + name + '" is not a number.');
  };

  // Checks that value is not null and of type number.
  public static CheckNotNullAndTypeIsNumber (name: string, value: unknown): asserts value is number
  {
      ArgumentUtility.CheckNotNull(name, value);
      ArgumentUtility.CheckTypeIsNumber(name, value);
  };

  // Checks that value is not null and of type boolean.
  public static CheckTypeIsBoolean (name: string, value: unknown): asserts value is Nullable<boolean>
  {
      if (TypeUtility.IsNull(value))
          return;
      if (!TypeUtility.IsBoolean(value))
          throw ('Error: The value of parameter "' + name + '" is not a boolean.');
  };

  // Checks that value is not null and of type boolean.
  public static CheckNotNullAndTypeIsBoolean (name: string, value: unknown): asserts value is boolean
  {
      ArgumentUtility.CheckNotNull(name, value);
      ArgumentUtility.CheckTypeIsBoolean(name, value);
  };

  // Checks that value is not null and of type function.
  public static CheckTypeIsFunction (name: string, value: unknown): asserts value is Nullable<AnyFunction>
  {
      if (TypeUtility.IsNull(value))
          return;
      if (!TypeUtility.IsFunction(value))
          throw ('Error: The value of parameter "' + name + '" is not a function.');
  };

  // Checks that value is not null and of type function.
  public static CheckNotNullAndTypeIsFunction (name: string, value: unknown): asserts value is AnyFunction
  {
      ArgumentUtility.CheckNotNull(name, value);
      ArgumentUtility.CheckTypeIsFunction(name, value);
  };

  // Checks that value is not null and of type jquery.
  public static CheckTypeIsJQueryObject (name: string, value: unknown): asserts value is Nullable<JQuery>
  {
    if (TypeUtility.IsNull(value))
      return;
    if (!TypeUtility.IsJQuery(value))
      throw ('Error: The value of parameter "' + name + '" is not a jquery object.');
  };

  // Checks that value is not null and of type jquery.
  public static CheckNotNullAndTypeIsJQuery (name: string, value: unknown): asserts value is JQuery
  {
    ArgumentUtility.CheckNotNull(name, value);
    ArgumentUtility.CheckTypeIsJQueryObject(name, value);
  };
}

class BrowserUtility
{
  public static GetIEVersion(): number
  {
    var isIE8OrLater = TypeUtility.IsDefined (window.document.documentMode);
    if (isIE8OrLater)
    {
      var majorVersion = parseInt (window.document.documentMode as any as string); // TODO RM-7631: window.document.documentMode does not need to be converted to a number
      return majorVersion;
    }
    else if (navigator.appVersion.indexOf ('MSIE 7.0;') !== -1)
    {
      return 7;
    }
    else
    {
      return Number.NaN;
    }
  }
}

class PageUtility
{
  public static Instance: PageUtility;

  private _resizeHandlers: PageUtility_ResizeHandlerItem[] = new Array();
  private _resizeTimeoutID: Nullable<number> = null;
  private _resizeTimeoutInMilliSeconds = 50;

  constructor()
  {
    $(document).ready(function()
    {
      $(window).bind('resize', function() { PageUtility.Instance.PrepareExecuteResizeHandlers(); });
    });
  }

  public RegisterResizeHandler (selector: string, handler: PageUtility_ResizeHandler): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsString ('selector', selector);
    ArgumentUtility.CheckNotNull ('handler', handler);

    for (var i = 0; i < this._resizeHandlers.length; i++)
    {
      var item = this._resizeHandlers[i];
      if (item.Selector == selector)
      {
        (item as any).handler = handler; // TODO 7632: PageUtility.RegisterResizeHandler assigns wrong property
        return;
      }
    }
    this._resizeHandlers[this._resizeHandlers.length] = new PageUtility_ResizeHandlerItem (selector, handler);
  };

  public PrepareExecuteResizeHandlers(): void
  {
    if (this._resizeTimeoutID != null)
      window.clearTimeout (this._resizeTimeoutID);

    this._resizeTimeoutID = window.setTimeout (function () { PageUtility.Instance.ExecuteResizeHandlers(); }, this._resizeTimeoutInMilliSeconds);
  };

  public ExecuteResizeHandlers(): void
  {
    var existingResizeHandlers = new Array();
    for (var i = 0; i < this._resizeHandlers.length; i++)
    {
      var item = this._resizeHandlers[i];
      var element = $ (item.Selector);
      if (element != null && element.length > 0)
      {
        item.Handler (element);
        existingResizeHandlers[existingResizeHandlers.length] = item;
      }
    }
    this._resizeHandlers = existingResizeHandlers;
  };

  public IsInDom (element: HTMLElement): boolean
  {
    ArgumentUtility.CheckNotNull('element', element);

    var html = window.document.body.parentNode;
    let node: Nullable<Node & ParentNode> = element;
    while (node)
    {
      if (node === html)
          return true;
      node = node.parentNode;
    }
    return false;
  }
}

type PageUtility_ResizeHandler = (element: JQuery) => void;

class PageUtility_ResizeHandlerItem
{
  public readonly Selector: string;
  public Handler;

  constructor(selector: string, handler: PageUtility_ResizeHandler)
  {
    this.Selector = selector;
    this.Handler = handler;
  }
}

PageUtility.Instance = new PageUtility();

class WebServiceUtility
{
  static Execute<T>(serviceUrl: string, serviceMethod: string, params: object, onSuccess: (result: T) => void, onError: (err: Sys.Net.WebServiceError) => void): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsString ('serviceUrl', serviceUrl);
    ArgumentUtility.CheckNotNullAndTypeIsString ('serviceMethod', serviceMethod);
    ArgumentUtility.CheckNotNullAndTypeIsObject ('params', params);
    ArgumentUtility.CheckNotNullAndTypeIsFunction ('onSuccess', onSuccess);
    ArgumentUtility.CheckNotNullAndTypeIsFunction ('onError', onError);

    if (Sys === undefined)
      throw "'Sys' namespace is undefined. System.Web.ScriptManager has not been included on the page.";

    // re-motion: replaced jQuery AJAX call with .NET call because of the following problem:
    //           when extending the parameter list with the necessary arguments for the web service method call,
    //           the JSON object is serialized to "key=value;" format, but the service expects JSON format ("{ key: value, ... }")
    //           see http://encosia.com/2008/06/05/3-mistakes-to-avoid-when-using-jquery-with-aspnet-ajax/ 
    //           under "JSON, objects, and strings: oh my!" for details.

    let executingRequest: Nullable<Sys.Net.WebRequest> = Sys.Net.WebServiceProxy.invoke (
      serviceUrl,
      serviceMethod,
      false,
      params,
      function (result: string) {
        executingRequest = null;
        onSuccess (result);
      },
      function (err: Sys.Net.WebServiceError) {
        executingRequest = null;
        var isTimedOut = err.get_timedOut();
        var isAborting = !isTimedOut && err.get_statusCode() === -1;
        if (!isAborting)
        {
          onError (err);
        }
      });
  };
}