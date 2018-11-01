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

/**
 * Script summary
 *
 * Arguments:
 *  [0]: HTMLElement    Target element
 *
 *
 * This script is supposed to fulfill two purposes:
 *  - It should return the absolute bounds (in relation to the outermost window)
 *    of the specified target element. This is done by getting the relative
 *    position inside of the current viewport (via .getBoundingClientRect())
 *    and then repeating the process for each iframe until the outermost frame
 *    is reached.
 *
 *  - It should return the absolute bounds (in relation to the outermost window)
 *    of the specified target elements's. But this area should be restricted to
 *    the visible area. This is done by walking parent elements upwards.
 *    Remember the first element that has an overflow - any following parents
 *    with an overflow restrict the size of the visible area. If any of the
 *    parents has a fixed position break at that element.
 *
 *  Note: Do not use line comments ( // ) as they will break the script. Use block
 *    comments instead. Exception: Lines that start with a // will be removed from
 *    the script. (Thats why the file header is allowed to exist with line comments.
 * 
 */
(function (element)
{
  /* Rectangle class */
  var Rectangle = (function ()
  {
    /* Rectangle constructor */
    function Rectangle (x, y, width, height)
    {
      this.X = x;
      this.Y = y;
      this.Width = width;
      this.Height = height;
    }

    /* Creates a new empty rectangle */
    Rectangle.Empty = function ()
    {
      return new Rectangle (0, 0, 0, 0);
    };

    /* Creates a new Rectangle from the specified ClientRect */
    Rectangle.FromClientRect = function (clientRect)
    {
      var bounds = {
        X : Math.round (clientRect.left),
        Y : Math.round (clientRect.top),
        W : Math.round (clientRect.left + clientRect.width) - Math.round (clientRect.left),
        H : Math.round (clientRect.top + clientRect.height) - Math.round (clientRect.top)
      };
      return new Rectangle (bounds.X, bounds.Y, bounds.W, bounds.H);
    };

    /* Creates a new Rectangle that is the intersection between the specified Rectangles*/
    Rectangle.Intersect = function (a, b)
    {
      var x1 = Math.max (a.X, b.X);
      var x2 = Math.min (a.X + a.Width, b.X + b.Width);
      var y1 = Math.max (a.Y, b.Y);
      var y2 = Math.min (a.Y + a.Height, b.Y + b.Height);
      if (x2 >= x1 && y2 >= y1)
        return new Rectangle (x1, y1, x2 - x1, y2 - y1);
      return Rectangle.Empty();
    };

    /* Clones the rectangle */
    Rectangle.prototype.Clone = function ()
    {
      return new Rectangle (this.X, this.Y, this.Width, this.Height);
    };

    /* Offsets the rectangle by x,y */
    Rectangle.prototype.Offset = function (x, y)
    {
      this.X += Math.round (x);
      this.Y += Math.round (y);
    };

    return Rectangle;
  }());

  /* Find the absolute element bounds by going up frames and returns the absolute bounds and the offset (relative to absolute) */
  function GetElementBounds (target)
  {
    /* Parses '34px' and returns 34 */
    function ParseProperty (value)
    {
      return parseInt (value.substr (0, value.length - "px".length), 10);
    }

    var initial = Rectangle.FromClientRect (target.getBoundingClientRect());
    var result = initial.Clone();

    /* loop through all frames until we have reached the top or if we hit cross-site boundaries */
    var currentWindow = target.ownerDocument.defaultView;
    while (currentWindow && currentWindow.parent && currentWindow !== currentWindow.parent && currentWindow.frameElement)
    {
      var frame = currentWindow.frameElement;
      currentWindow = currentWindow.parent;

      /* add the frame position to the result */
      var bounds = frame.getBoundingClientRect();
      result.Offset(bounds.left, bounds.top);

      /* include the border in the positioning */
      var style = currentWindow.getComputedStyle (frame);
      result.Offset(ParseProperty(style.borderLeftWidth), ParseProperty(style.borderTopWidth));

      /* include the padding in the positioning */
      result.Offset (ParseProperty (style.paddingLeft), ParseProperty (style.paddingTop));
    }
    return [result, { X : result.X - initial.X, Y : result.Y - initial.Y }, { Width: currentWindow.innerWidth, Height: currentWindow.innerHeight }];
  }

  /* Returns the client bounds restricted to the visible area and the parent bounds */
  function GetVisibleParentBounds (target)
  {
    var targetDocument = target.ownerDocument;
    var targetWindow = targetDocument.defaultView;

    var style = targetWindow.getComputedStyle(target);

    var isAbsolute = style.position === "absolute";
    var container = null;

    if (style.position === "fixed")
      return new Rectangle(0, 0, targetWindow.innerWidth, targetWindow.innerHeight);

    /* Walk through all parent elements */
    for (var parent = target; (parent = parent.parentElement) != null;)
    {
      var parentStyle = targetWindow.getComputedStyle(parent);

      /* disregard static elements if the origin element is absolute */
      if (isAbsolute && parentStyle.position === "static")
        continue;

      var bounds = Rectangle.FromClientRect (parent.getBoundingClientRect());

      /* If the parent is fixed we are done as the parent of a fixed element is the body */
      if (parentStyle.position === "fixed")
        return container == null ? bounds : Rectangle.Intersect(container, bounds);

      /* Test if the current element has a relevant overflow set */
      var overflow = parentStyle.overflow + parentStyle.overflowX + parentStyle.overflowY;
      if (/(auto|hidden|scroll)/.test (overflow))
        container = container == null ? bounds : Rectangle.Intersect (container, bounds);
    }

    return container || null;
  }

  /* Calculate element bounds and offset */
  var _temp = GetElementBounds(element);
  var elementBounds = _temp[0];
  var elementOffset = _temp[1];
  var windowSize = _temp[2];

  /* Calculate the visible parents bounds */
  var windowBounds = new Rectangle (0, 0, windowSize.Width, windowSize.Height);
  var visibleParentBounds = GetVisibleParentBounds(element) || windowBounds;
  visibleParentBounds.Offset(elementOffset.X, elementOffset.Y);
  visibleParentBounds = Rectangle.Intersect(visibleParentBounds, windowBounds);

  /* Return the result as JSON */
  var result = { ElementBounds : elementBounds, ParentBounds : visibleParentBounds };
  return JSON.stringify (result);
}) (arguments[0]);