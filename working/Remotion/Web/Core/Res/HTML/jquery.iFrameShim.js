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
(function ($)
{
  $.fn.iFrameShim = function (s)
  {
    if ($.browser.mozilla)
      return this;

    var iframe = $("<iframe/>")
        .attr('src', 'javascript:false')
        .attr('frameborder', '0')
        .attr('tabindex', '-1')
        .addClass('bgiframe')
        .css({
            display: 'block',
            position: 'absolute',
            'z-index': -1,
            top: '0px',
            left: '0px',
            width: '100%',
            height: '100%',
            filter: "filter:Alpha(Opacity='0')"
        });

    return this.each(function ()
    {
      if ($('> iframe.bgiframe', this).length == 0)
        iframe.prependTo(this);
    });
    return this;
  };

})(jQuery);