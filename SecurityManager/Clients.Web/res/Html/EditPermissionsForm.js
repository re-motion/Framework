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