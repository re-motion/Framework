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
class WebTabStrip
{
  public static Initialize (tabStripOrSelector: CssSelectorOrElement<HTMLElement>): void
  {
    ArgumentUtility.CheckNotNull('tablistOrSelector', tabStripOrSelector);

    const tabStrip = ElementResolverUtility.ResolveSingle (tabStripOrSelector);

    tabStrip.addEventListener ('keydown', function (event)
    {
      WebTabStrip.OnKeyDown (event, tabStrip);
    });
  };

  private static OnKeyDown (event: KeyboardEvent, tabStrip: HTMLElement): void
  {
    const tabs = Array.from (tabStrip.querySelectorAll<HTMLElement> ('ul li a'));

    let oldTabIndex = -1;

    let oldTab = null;
    const activeTab = document.activeElement as Nullable<HTMLElement>;
    if (activeTab != null && TypeUtility.IsDefined (activeTab.tagName) && activeTab.tagName.toUpperCase() === 'A')
    {
      oldTab = activeTab;
      oldTabIndex = tabs.indexOf (activeTab);
    }
    else
    {
      for (let i = 0; i < tabs.length; i++)
      {
        // TODO RM-7686: Fix misspellings of tabIndex in the TypeScript codebase
        if ((tabs[i] as any).tabindex === 0)
        {
          oldTabIndex = i;
          break;
        }
      }
      if (oldTabIndex >= 0)
        oldTab = tabs[oldTabIndex];
    }

    let currentTabIndex = Math.max (0, oldTabIndex);

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
            let newTab = tabs[currentTabIndex];
            newTab.setAttribute ('tabindex', '0');
            newTab.focus();
            if (oldTab !== null && oldTab !== newTab)
              oldTab.setAttribute ('tabindex', '-1');

            newTab.click();
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
            currentTabIndex = tabs.length - 1;

          let newTab = tabs[currentTabIndex];
          newTab.focus();

          return;
        }
      case 39: // right arrow
        {
          event.preventDefault();
          event.stopPropagation();

          if (currentTabIndex < tabs.length - 1)
            currentTabIndex++;
          else
            currentTabIndex = 0;

          const newTab = tabs[currentTabIndex];
          newTab.focus();

          return;
        }
    }
  }
}
