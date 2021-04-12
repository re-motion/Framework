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

// Orignal license header:
//
// Copyright (c) 2006 Brandon Aaron (http://brandonaaron.net)
// Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php) 
// and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
// 
// $LastChangedDate: 2007-07-21 18:44:59 -0500 (Sat, 21 Jul 2007) $
// $Rev: 2446 $
// 
// Version 2.1.1

// ************************************************
// Significantly modified for re-motion
// ************************************************

(function ($)
{
  var isActiveXSupported = false;
  try
  {
    new ActiveXObject ('htmlfile'); // Test if ActiveX controls are supported.
    isActiveXSupported = true;
  }
  catch (e)
  {
  }

  $.fn.iFrameShim = isActiveXSupported
      ? function (s)
      {
        var iframe = $ ("<iframe/>")
            .attr ('src', 'javascript:""')
            .attr ('frameborder', '0')
            .attr ('tabindex', '-1')
            .attr ('aria-hidden', 'true') // for proper annotation for ARIA
            .attr ('aria-label', '') // overrides the reading of the src-text with a blank text
            .addClass ('bgiframe')
            .css ({
              display : 'block',
              position : 'absolute',
              'z-index' : -1,
              top : '0px',
              left : '0px',
              width : '100%',
              height : '100%',
              filter : "filter:Alpha(Opacity='0')"
          });

        return this.each (function () {
            if ($ ('> iframe.bgiframe', this).length == 0)
              iframe.prependTo (this);
        });
      }
      : function (s) { return this; };
})(Remotion.jQuery);