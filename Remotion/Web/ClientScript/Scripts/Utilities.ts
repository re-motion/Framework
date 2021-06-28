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
  public static IsObject (value: unknown): value is Nullable<object>
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

  public static HasProperty<TTarget, TKey extends string>(target: TTarget, key: TKey): target is TTarget & Record<TKey, unknown>
  {
    return key in target;
  }

  public static HasPropertyOfType<TTarget, TKey extends string>(target: TTarget, key: TKey, type?: "string"): target is TTarget & Record<TKey, string>;
  public static HasPropertyOfType<TTarget, TKey extends string>(target: TTarget, key: TKey, type?: "number"): target is TTarget & Record<TKey, number>;
  public static HasPropertyOfType<TTarget, TKey extends string>(target: TTarget, key: TKey, type?: "bigint"): target is TTarget & Record<TKey, bigint>;
  public static HasPropertyOfType<TTarget, TKey extends string>(target: TTarget, key: TKey, type?: "boolean"): target is TTarget & Record<TKey, boolean>;
  public static HasPropertyOfType<TTarget, TKey extends string>(target: TTarget, key: TKey, type?: "symbol"): target is TTarget & Record<TKey, symbol>;
  public static HasPropertyOfType<TTarget, TKey extends string>(target: TTarget, key: TKey, type?: "undefined"): target is TTarget & Record<TKey, undefined>;
  public static HasPropertyOfType<TTarget, TKey extends string>(target: TTarget, key: TKey, type?: "object"): target is TTarget & Record<TKey, object>;
  public static HasPropertyOfType<TTarget, TKey extends string>(target: TTarget, key: TKey, type?: "function"): target is TTarget & Record<TKey, Function>;
  public static HasPropertyOfType<TTarget, TKey extends string>(target: TTarget, key: TKey, type?: string): target is TTarget & Record<TKey, unknown>
  {
    return TypeUtility.HasProperty(target, key) && typeof target[key] === type;
  }
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
    window.addEventListener("resize", function() { PageUtility.Instance.PrepareExecuteResizeHandlers(); });
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
      var element = document.querySelector<HTMLElement> (item.Selector);
      if (element != null)
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

type PageUtility_ResizeHandler = (element: HTMLElement) => void;

class PageUtility_ResizeHandlerItem
{
  public readonly Selector: string;
  public readonly Handler: PageUtility_ResizeHandler;

  constructor(selector: string, handler: PageUtility_ResizeHandler)
  {
    this.Selector = selector;
    this.Handler = handler;
  }
}

PageUtility.Instance = new PageUtility();

// TODO RM-7648: Update the type definitions for WebServiceProxy.Execute in the 'jquery.TypeScript.DefinitelyTyped' Nuget package
type WebServiceProxyInvokeFunction<TResult, TUserContext> = (
  servicePath: string,
  methodName: string,
  useGet?: boolean,
  params?: Nullable<Dictionary<string>>,
  onSuccess?: (result: TResult, userContext: TUserContext, methodName: string) => void,
  onFailure?: (error: Sys.Net.WebServiceError, userContext: TUserContext, methodName: string) => void,
  userContext?: TUserContext,
  timeout?: number,
  enableJsonp?: boolean,
  jsonpCallbackParameter?: string) => Sys.Net.WebRequest;

class WebServiceUtility
{
  static Execute<TResult>(serviceUrl: string, serviceMethod: string, params: Dictionary<string>, onSuccess: (result: TResult) => void, onError: (err: Sys.Net.WebServiceError) => void): void
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

    let executingRequest: Nullable<Sys.Net.WebRequest> = (Sys.Net.WebServiceProxy.invoke as unknown as WebServiceProxyInvokeFunction<TResult, never>) (
      serviceUrl,
      serviceMethod,
      false,
      params,
      function (result: TResult, userContext: never, methodName: string) {
        executingRequest = null;
        onSuccess (result);
      },
      function (err: Sys.Net.WebServiceError, userContext: never, methodName: string) {
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

class ElementResolverUtility
{
  // Resolves any provided css selector, checks the element for null and returns it.
  public static ResolveSingle<TElement extends Element> (selectorOrElement: CssSelectorOrElement<TElement>, context?: ParentNode): TElement
  {
    ArgumentUtility.CheckNotNull("selectorOrElement", selectorOrElement);

    if (TypeUtility.IsString(selectorOrElement))
    {
      const queryContext = context ? context : window.document;
      const resolvedElement = queryContext.querySelector(selectorOrElement) as TElement;
      if (!resolvedElement)
        throw ('Error: Cannot find an element specified by selector "' + selectorOrElement + '".');

      return resolvedElement;
    }
    else if (TypeUtility.IsObject(selectorOrElement))
    {
      return selectorOrElement;
    }
    else
    {
      throw ('Error: The type of parameter "selectorOrElements" is "' + (typeof selectorOrElement) + '" but "string" or "object" was expected.');
    }
  }

  // Resolves any provided css selector, and returns the found elements as an array.
  public static ResolveMultiple<TElement extends Element> (selectorOrElements: CssSelectorOrElements<TElement>, context?: ParentNode): TElement[]
  {
    ArgumentUtility.CheckNotNull("selectorOrElements", selectorOrElements);

    if (TypeUtility.IsString(selectorOrElements))
    {
      const queryContext = context ? context : window.document;
      return Array.from(queryContext.querySelectorAll(selectorOrElements));
    }
    else if (Array.isArray(selectorOrElements))
    {
      return selectorOrElements;
    }
    else if (TypeUtility.IsObject(selectorOrElements))
    {
      return [selectorOrElements];
    }
    else
    {
      throw ('Error: The type of parameter "selectorOrElements" is "' + (typeof selectorOrElements) + '" but "string", "object", or "array" was expected.');
    }
  }
}

type CssOrientation = "Top" | "Right" | "Bottom" | "Left";

class LayoutUtility
{
  public static GetHeight(element: Element)
  {
    return LayoutUtility.GetElementDimension(element, "height");
  }

  public static GetWidth(element: Element)
  {
    return LayoutUtility.GetElementDimension(element, "width");
  }

  private static GetElementDimension(element: Element, dimension: "width" | "height")
  {
    const style = window.getComputedStyle(element);

    // jQuery 1.6.4 uses the offsetWidth/offsetHeight properties to calculate element dimensions.
    // The offsetXXX properties are rounded, while the style properties are not.
    // We round here to make sure that we are compatible with the original behavior, but we do
    // not use the offsetXXX properties for the calculation because they introduce unnecessary
    // complexity for backwards compatibility that we don't need (IE).
    const value = Math.round(parseFloat(style[dimension])) || 0;
    
    // For content-box: value is already correct
    // For border-box: we need to remove padding and border from the value
    if (style.boxSizing !== "border-box")
      return value;
    
    const orientations: CssOrientation[] = dimension === "width"
      ? ["Left", "Right"]
      : ["Top", "Bottom"];

    let adjustment = 0.0;
    for (const orientation of orientations)
    {
      // Subtract padding
      const paddingProperty = `padding${orientation}` as `padding${CssOrientation}`;
      adjustment -= parseFloat(style[paddingProperty]) || 0;

      // Subtract border
      const borderProperty = `border${orientation}Width` as `border${CssOrientation}Width`;
      adjustment -= parseFloat(style[borderProperty]) || 0;
    }

    return value + adjustment;
  }

  public static GetOffset(element: HTMLElement)
  {
    const boundingRectangle = element.getBoundingClientRect();
    const window = element.ownerDocument.defaultView;
    return {
      left: (window?.pageXOffset ?? 0) + boundingRectangle.left,
      top: (window?.pageYOffset ?? 0) + boundingRectangle.top
    }
  }

  public static GetOuterHeight(element: HTMLElement)
  {
    const style = window.getComputedStyle(element);

    // Margins on the style are always in px e.g. "34px"
    // parseInt ignores the "px" suffix so we do not have to remove here
    return element.offsetHeight + parseInt(style.marginTop) + parseInt(style.marginBottom);
  }

  public static IsVisible(element: HTMLElement)
  {
    return element.offsetWidth !== 0 || element.offsetHeight !== 0 || element.getClientRects().length !== 0;
  }
}