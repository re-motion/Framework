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
function WebTabStrip()
{
}

WebTabStrip.Initialize = function ($tablist)
{
  ArgumentUtility.CheckNotNullAndTypeIsJQuery("$tablist", $tablist);

  $tablist.keydown (function (event)
  {
    WebTabStrip.OnKeyDown (event, $tablist);
  });
};

WebTabStrip.OnKeyDown = function (event, $tablist)
{
  ArgumentUtility.CheckNotNullAndTypeIsJQuery ('$tablist', $tablist);

  var $tabs = $tablist.find ('ul li a');

  var oldTabIndex = -1;

  var $oldTab = null;
  var activeTab = document.activeElement;
  if (activeTab != null && TypeUtility.IsDefined (activeTab.tagName) && activeTab.tagName.toUpperCase() === 'A')
  {
    $oldTab = $(activeTab);
    oldTabIndex = $tabs.index (activeTab);
  }
  else
  {
    for (var i = 0; i < $tabs.Length; i++)
    {
      if ($tabs[i].tabindex === 0)
      {
        oldTabIndex = i;
        break;
      }
    }
    if (oldTabIndex >= 0)
      $oldTab = $($tabs[oldTabIndex]);
  }

  var currentTabIndex = Math.max (0, oldTabIndex);

  switch (event.keyCode)
  {
    case 9: // tab
      // exit tab strip
      return;
    case 13: //enter
    case 32: //space
      {
        event.preventDefault();
        event.stopPropagation();

        if (currentTabIndex >= 0)
        {
          let $newTab = $ ($tabs[currentTabIndex]);
          $newTab.attr ('tabIndex', 0);
          $newTab.focus();
          if ($oldTab !== null && $oldTab[0] !== $newTab[0])
            $oldTab.attr ('tabIndex', -1);

          $newTab[0].click();
        }

        return;
      }
    case 37: // left arrow
      {
        event.preventDefault();
        event.stopPropagation();

        if (currentTabIndex > 0)
          currentTabIndex--;
        else
          currentTabIndex = $tabs.length - 1;

        let $newTab = $ ($tabs[currentTabIndex]);
        $newTab.focus();

        return;
      }
    case 39: // right arrow
      {
        event.preventDefault();
        event.stopPropagation();

        if (currentTabIndex < $tabs.length - 1)
          currentTabIndex++;
        else
          currentTabIndex = 0;

        var $newTab = $ ($tabs[currentTabIndex]);
        $newTab.focus();

        return;
      }
  }
}

