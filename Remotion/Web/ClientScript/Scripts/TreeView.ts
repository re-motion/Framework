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
class WebTreeView
{
  public static Initialize ($treeView: JQuery): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsJQuery("$treeView", $treeView);

    $treeView.keydown (function (event)
    {
      WebTreeView.OnKeyDown (event, $treeView);
    });

    var $focusableTreeNode = $treeView.find('li[tabindex=0][role=treeitem]');
    var hasFocusableTreeNode = $focusableTreeNode.length > 0;
    if (!hasFocusableTreeNode)
    {
      var $selectedTreeNodes = $treeView.find ('li[aria-selected=true]');
      if ($selectedTreeNodes.length === 0)
      {
        var $topLevelTreeNodes = $treeView.find ('ul[role=tree]:first > li[role=treeitem]:first');
        if ($topLevelTreeNodes.length > 0)
        {
          $topLevelTreeNodes[0].setAttribute ('tabindex', '0');
        }
      }
      else
      {
        $selectedTreeNodes[0].setAttribute ('tabindex', '0');
      }
    }
  };

  public static OnKeyDown (event: JQueryKeyEventObject, $treeView: JQuery): void
  {
    ArgumentUtility.CheckNotNull ('event', event);
    ArgumentUtility.CheckNotNullAndTypeIsJQuery ('$treeView', $treeView);

    var $treeNodes = $treeView.find ('li[role=treeitem]');

    var activeTreeNodeIndex = -1;

    var activeTreeNode = document.activeElement!;
    var $activeTreeNode = $ (activeTreeNode);
    if (activeTreeNode != null &&
      TypeUtility.IsDefined (activeTreeNode.tagName) &&
      activeTreeNode.tagName.toUpperCase() === 'LI')
    {
      activeTreeNodeIndex = $treeNodes.index (activeTreeNode);
    }
    else
    {
      for (var i = 0; i < $treeNodes.length; i++)
      {
        if (($treeNodes[i] as any).tabindex === 0) // TODO RM-7686: Fix misspellings of tabIndex in the TypeScript codebase
        {
          activeTreeNodeIndex = i;
          break;
        }
      }
      if (activeTreeNodeIndex >= 0)
        $activeTreeNode = $ ($treeNodes[activeTreeNodeIndex]);
    }

    switch (event.keyCode)
    {
      case 9: // tab
        // exit TreeView
        return;
      case 13: //enter
      case 32: //space
        {
          event.preventDefault();
          event.stopPropagation();

          WebTreeView.SelectNode ($activeTreeNode);

          return;
        }
      case 37: // left arrow
        {
          // When focus is on an open node, closes the node.
          // When focus is on a child node that is also either an end node or a closed node, moves focus to its parent node.
          // When focus is on a root node that is also either an end node or a closed node, does nothing.

          event.preventDefault();
          event.stopPropagation();

          let hasChildren = $activeTreeNode.attr ('aria-expanded') === 'true';
          if (!hasChildren)
          {
            var $parentTreeNode = $activeTreeNode.parentsUntil ('ul[role=tree]', 'li').first();
            if ($parentTreeNode.length > 0)
            {
              let $newTreeNode = $parentTreeNode;
              WebTreeView.SetFocus ($newTreeNode, $activeTreeNode);
            }
          }
          else
          {
            WebTreeView.ToggleExpander ($activeTreeNode);
          }

          return;
        }
      case 39: // right arrow
        {
          // When focus is on a closed node, opens the node; focus does not move.
          // When focus is on a open node, moves focus to the first child node.
          // When focus is on an end node, does nothing.

          event.preventDefault();
          event.stopPropagation();

          let hasChildren = $activeTreeNode.attr ('aria-expanded') === 'true';
          if (hasChildren)
          {
            var $childrenGroup = $activeTreeNode.children ('ul');
            var $firstChild = $childrenGroup.children ('li:first');
            let $newTreeNode = $firstChild;
            WebTreeView.SetFocus ($newTreeNode, $activeTreeNode);
          }
          else
          {
            WebTreeView.ToggleExpander ($activeTreeNode);
          }

          return;
        }
      case 38: // up arrow
        {
          // Moves focus to the previous node that is focusable without opening or closing a node.
          let newTreeNodeIndex = activeTreeNodeIndex - 1;
          if (newTreeNodeIndex >= 0)
          {
            let $newTreeNode = $($treeNodes[newTreeNodeIndex]);
            WebTreeView.SetFocus ($newTreeNode, $activeTreeNode);
          }

          return;
        }
      case 40: // down arrow
        {
          // Moves focus to the next node that is focusable without opening or closing a node.
          let newTreeNodeIndex = activeTreeNodeIndex + 1;
          if (newTreeNodeIndex < $treeNodes.length)
          {
            let $newTreeNode = $($treeNodes[newTreeNodeIndex]);
            WebTreeView.SetFocus ($newTreeNode, $activeTreeNode);
          }

          return;
        }
      case 36: // home
        {
          // Moves focus to the first node in the tree without opening or closing a node.
          let newTreeNodeIndex = 0;
          if (newTreeNodeIndex < $treeNodes.length)
          {
            let $newTreeNode = $($treeNodes[newTreeNodeIndex]);
            WebTreeView.SetFocus ($newTreeNode, $activeTreeNode);
          }

          return;
        }
      case 35: // end
        {
          // Moves focus to the last node in the tree that is focusable without opening a node.
          let newTreeNodeIndex = $treeNodes.length -1;
          if (newTreeNodeIndex >= 0)
          {
            let $newTreeNode = $($treeNodes[newTreeNodeIndex]);
            WebTreeView.SetFocus ($newTreeNode, $activeTreeNode);
          }

          return;
        }
    }
  };

  public static SetFocus ($newTreeNode: JQuery, $oldTreeNode: JQuery): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsJQuery ('$newTreeNode', $newTreeNode);
    ArgumentUtility.CheckNotNullAndTypeIsJQuery ('$oldTreeNode', $oldTreeNode);

    $newTreeNode.attr('tabindex', 0);
    $newTreeNode.focus();
    $oldTreeNode.attr ('tabindex', -1);
  };

  public static ToggleExpander ($treeNode: JQuery): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsJQuery ('$treeNode', $treeNode);

    var expanderAnchor = $treeNode.find('span:first > a:first');
    expanderAnchor.click();
  };

  public static SelectNode ($treeNode: JQuery): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsJQuery ('$treeNode', $treeNode);

    $treeNode.attr ('tabindex', 0);
    $treeNode.focus();
    let headAnchor = $treeNode.find ('span:first a[id^=Head_]:first');
    headAnchor.click();
  };
}