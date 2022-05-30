﻿// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
//
(function () 
{
  function fixCellHeaderStyle() 
  {
    var maximumLength = 0;
    document.querySelectorAll('.header').forEach(function(headerRow) 
    {
      headerRow.querySelectorAll(':scope > .titleCellVertical').forEach(function(headerCell) 
      {
        var currentLength = headerCell.textContent.length;
        if (currentLength > maximumLength) 
          maximumLength = currentLength;
      });
    });

    if (maximumLength > 0) 
    {
      var headerHeight = 1 + maximumLength * 0.5 + 'em';
      var styleElement = document.createElement('style');
      var textStyle = 'th.header, th.titleCellVertical, tr.header, tr.titleCellVertical {height: ' + headerHeight + ' !important;}';
      styleElement.setAttribute("type", "text/css");
      var textElement = document.createTextNode(textStyle);
      styleElement.appendChild(textElement);
      var headElement = document.getElementsByTagName('head')[0];
      headElement.appendChild(styleElement);
    }
  }

  if (document.readyState != 'loading') 
  {
    fixCellHeaderStyle();
  }
  else 
  {
    document.addEventListener('DOMContentLoaded', fixCellHeaderStyle);
  }
})();