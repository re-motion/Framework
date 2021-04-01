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
class ViewLayout
{
  public static AdjustActiveViewContent (viewContent: HTMLElement): void
  {
    var viewContentBorder = viewContent.querySelector<HTMLDivElement> (':scope > div')!;
    var viewBottomControls = viewContent.nextElementSibling as HTMLElement;

    var viewContentBorderHeight = LayoutUtility.GetOuterHeight (viewContentBorder) - LayoutUtility.GetHeight (viewContentBorder);
    var viewBottomControlsBorderHeight = LayoutUtility.GetOuterHeight (viewBottomControls) - LayoutUtility.GetHeight (viewBottomControls);

    var viewContentOffset = LayoutUtility.GetOffset (viewContent);
    var viewTop = viewContentOffset == null ? 0 : viewContentOffset.top;

    var viewBottomControlsOffset = LayoutUtility.GetOffset (viewBottomControls);
    var bottomTop = viewBottomControlsOffset == null ? 0 : viewBottomControlsOffset.top;

    var viewNewHeight = bottomTop - viewTop - viewContentBorderHeight - viewBottomControlsBorderHeight;

    viewContentBorder.style.height = viewNewHeight + "px";
  };

  // TODO RM-7670 this signature needs to be adjusted after Utilities.ts/ExecuteResizeHandlers() has been adapted to not use JQuery anymore.
  public static AdjustSingleView ($containerElement: JQuery): void
  {
    var containerElement = $containerElement[0];

    var viewContent = containerElement.children[0].children[1] as HTMLElement;
    ViewLayout.AdjustActiveViewContent (viewContent);
  };

  // TODO RM-7670 this signature needs to be adjusted after Utilities.ts/ExecuteResizeHandlers() has been adapted to not use JQuery anymore.
  public static AdjustTabbedMultiView ($containerElement: JQuery): void
  {
    var containerElement = $containerElement[0];

    var viewContent = containerElement.children[0].children[2] as HTMLElement;
    ViewLayout.AdjustActiveViewContent (viewContent);

    if (BrowserUtility.GetIEVersion() > 0)
    {
      // For Internet Explorer + JAWS 2018ff, the tabindex-attribute on the table root will break a table with a scrollable header part.
      var contentBorder = viewContent.querySelector (':scope > div[tabindex]');
      if (contentBorder !== null)
      {
        contentBorder.removeAttribute ('tabindex');
        var contentLabelledBy = contentBorder.getAttribute ('aria-labelledby') || '';
        var caption = viewContent.querySelector (':scope > span[aria-hidden]')!;
        caption.setAttribute ('tabindex', '0');
        caption.setAttribute ('aria-labelledby', contentLabelledBy + ' ' + caption.id);
      }
    }
  };
}
