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
  public static AdjustActiveViewContent (viewContent: JQuery): void
  {
    var viewContentBorder = viewContent.children('div').eq (0);
    var viewBottomControls = viewContent.next();

    var viewContentBorderHeight = viewContentBorder.outerHeight (true) - viewContentBorder.height();
    var viewBottomControlsBorderHeight = viewBottomControls.outerHeight (true) - viewBottomControls.height();

    var viewContentOffset = viewContent.offset();
    var viewTop = viewContentOffset == null ? 0 : viewContentOffset.top;

    var viewBottomControlsOffset = viewBottomControls.offset();
    var bottomTop = viewBottomControlsOffset == null ? 0 : viewBottomControlsOffset.top;

    var viewNewHeight = bottomTop - viewTop - viewContentBorderHeight - viewBottomControlsBorderHeight;

    viewContentBorder.height (viewNewHeight);
  };

  public static AdjustSingleView (containerElement: JQuery): void
  {
    var viewContent = containerElement.children().eq(0).children().eq(1);
    ViewLayout.AdjustActiveViewContent(viewContent);
  };

  public static AdjustTabbedMultiView (containerElement: JQuery): void
  {
    var viewContent = containerElement.children().eq(0).children().eq(2);
    ViewLayout.AdjustActiveViewContent(viewContent);

    if (BrowserUtility.GetIEVersion() > 0)
    {
      // For Internet Explorer + JAWS 2018ff, the tabindex-attribute on the table root will break a table with a scrollable header part.
      var contentBorder = viewContent.children ('div[tabindex]').eq (0);
      if (contentBorder.length > 0)
      {
        contentBorder.removeAttr ('tabindex');
        var contentLabelledBy = contentBorder.attr ('aria-labelledby') || '';
        var caption = viewContent.children ('span[aria-hidden]').eq (0);
        caption.attr ('tabindex', 0);
        caption.attr ('aria-labelledby', contentLabelledBy + ' ' + caption[0].id);
      }
    }
  };
}
