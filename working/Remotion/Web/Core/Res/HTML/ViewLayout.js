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
function ViewLayout()
{
}

ViewLayout.AdjustActiveViewContent = function (viewContent)
{
  var viewContentBorder = viewContent.children().eq (0);
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

ViewLayout.AdjustSingleView = function(containerElement) 
{
  var viewContent = containerElement.children().eq(0).children().eq(1);
  ViewLayout.AdjustActiveViewContent(viewContent);
};

ViewLayout.AdjustTabbedMultiView = function(containerElement) 
{
  var viewContent = containerElement.children().eq(0).children().eq(2);
  ViewLayout.AdjustActiveViewContent(viewContent);
};

