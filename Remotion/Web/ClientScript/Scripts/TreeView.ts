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
  public static Initialize (treeViewOrSelector: CssSelectorOrElement<HTMLElement>): void
  {
    ArgumentUtility.CheckNotNull ('treeViewOrSelector', treeViewOrSelector);

    const treeView = ElementResolverUtility.ResolveSingle (treeViewOrSelector);

    treeView.addEventListener ('keydown', function (event)
    {
      WebTreeView.OnKeyDown (event, treeView);
    });

    // The following code is responsible for making the whole tree view node clickable
    // Nodes in NovaGray are not full-width so we do this handling only in NovaViso
    if (!StyleUtility.IsNovaGray)
    {
      const nodes = [...treeView.querySelectorAll ('.treeViewNode')];
      for (const node of nodes)
      {
        const linkElement = node.querySelector (':scope > .treeViewNodeHead > a[onclick], :scope > .treeViewNodeHeadSelected > a[onclick]');
        if (linkElement)
        {
          linkElement.addEventListener('click', ev =>
          {
            ev.stopPropagation();
          });

          node.addEventListener ('click', ev =>
          {
            // Prevent recursion through bubbling
            if (!ev.isTrusted)
              return;

            if (ev.target === linkElement)
              return;

            linkElement.dispatchEvent (new MouseEvent (ev.type, ev));
            ev.preventDefault();
            ev.stopPropagation();
          });
          node.addEventListener('contextmenu', ev =>
          {
            // Prevent recursion through bubbling
            if (!ev.isTrusted)
              return;

            if (ev.target === linkElement)
              return;

            linkElement.dispatchEvent (new PointerEvent (ev.type, ev));
            ev.preventDefault();
            ev.stopPropagation();
          });
        }

        const expanderElement = node.querySelector(':scope > a:first-child');
        if (expanderElement)
        {
          expanderElement.addEventListener('click', ev =>
          {
            ev.stopPropagation();
          });
        }
      }
    }

    const focusableTreeNode = treeView.querySelector ('a[tabindex="0"][role=treeitem]');
    if (focusableTreeNode === null)
    {
      const firstSelectedTreeNode = treeView.querySelector ('a[aria-selected=true]');
      if (firstSelectedTreeNode === null)
      {
        const firstTopLevelTreeNodeList = treeView.querySelector ('ul[role=tree]');
        const firstTopLevelTreeNode = firstTopLevelTreeNodeList?.querySelector (':scope > li > span.treeViewNode a[role=treeitem]');
        if (firstTopLevelTreeNode)
        {
          firstTopLevelTreeNode.setAttribute ('tabindex', '0');
        }
      }
      else
      {
        firstSelectedTreeNode.setAttribute ('tabindex', '0');
      }
    }
  };

  private static OnKeyDown (event: KeyboardEvent, treeView: HTMLElement): void
  {
    const treeNodes = Array.from (treeView.querySelectorAll<HTMLElement> ('a[role=treeitem]'));

    let activeTreeNodeIndex = -1;

    let activeTreeNode = document.activeElement as HTMLElement;
    if (activeTreeNode != null &&
      TypeUtility.IsDefined (activeTreeNode.tagName) &&
      activeTreeNode.tagName.toUpperCase() === 'A')
    {
      activeTreeNodeIndex = treeNodes.indexOf (activeTreeNode);
    }
    else
    {
      for (let i = 0; i < treeNodes.length; i++)
      {
        if ((treeNodes[i] as any).tabindex === 0) // TODO RM-7686: Fix misspellings of tabIndex in the TypeScript codebase
        {
          activeTreeNodeIndex = i;
          break;
        }
      }
      if (activeTreeNodeIndex >= 0)
        activeTreeNode = treeNodes[activeTreeNodeIndex];
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

          WebTreeView.SelectNode (activeTreeNode);

          return;
        }
      case 37: // left arrow
        {
          // When focus is on an open node, closes the node.
          // When focus is on a child node that is also either an end node or a closed node, moves focus to its parent node.
          // When focus is on a root node that is also either an end node or a closed node, does nothing.

          event.preventDefault();
          event.stopPropagation();

          let hasChildren = activeTreeNode.getAttribute ('aria-expanded') === 'true';
          if (!hasChildren)
          {
            // Selects the <li> that contains this activeTreeNode <a>'s parent <li>
            let parent = activeTreeNode.closest<HTMLElement>('li')!.parentElement;
            while (parent && !parent.matches ('ul[role=tree]'))
            {
              if (parent.matches('li'))
              {
                let newlyFocusedTreeNode = parent.querySelector<HTMLElement>('a[role=treeitem]')!;
                WebTreeView.SetFocus (newlyFocusedTreeNode, activeTreeNode);
                break;
              }

              parent = parent.parentElement;
            }
          }
          else
          {
            WebTreeView.ToggleExpander (activeTreeNode);
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

          let hasChildren = activeTreeNode.getAttribute ('aria-expanded') === 'true';
          if (hasChildren)
          {
            const activeTreeNodeListItem = activeTreeNode.closest<HTMLElement>('li')!;
            const newTreeNodeListItem = activeTreeNodeListItem.querySelector<HTMLElement> (':scope > ul > li:first-child')!;

            const newNode =  newTreeNodeListItem.querySelector<HTMLElement>('a[role=treeitem]')!;
            WebTreeView.SetFocus (newNode, activeTreeNode);
          }
          else
          {
            WebTreeView.ToggleExpander (activeTreeNode);
          }

          return;
        }
      case 38: // up arrow
        {
          event.preventDefault();
          event.stopPropagation();

          // Moves focus to the previous node that is focusable without opening or closing a node.
          let newTreeNodeIndex = activeTreeNodeIndex - 1;
          if (newTreeNodeIndex >= 0)
          {
            let newTreeNode = treeNodes[newTreeNodeIndex];
            WebTreeView.SetFocus (newTreeNode, activeTreeNode);
          }

          return;
        }
      case 40: // down arrow
        {
          event.preventDefault();
          event.stopPropagation();

          // Moves focus to the next node that is focusable without opening or closing a node.
          let newTreeNodeIndex = activeTreeNodeIndex + 1;
          if (newTreeNodeIndex < treeNodes.length)
          {
            let newTreeNode = treeNodes[newTreeNodeIndex];
            WebTreeView.SetFocus (newTreeNode, activeTreeNode);
          }

          return;
        }
      case 36: // home
        {
          event.preventDefault();
          event.stopPropagation();

          // Moves focus to the first node in the tree without opening or closing a node.
          let newTreeNodeIndex = 0;
          if (newTreeNodeIndex < treeNodes.length)
          {
            let newTreeNode = treeNodes[newTreeNodeIndex];
            WebTreeView.SetFocus (newTreeNode, activeTreeNode);
          }

          return;
        }
      case 35: // end
        {
          event.preventDefault();
          event.stopPropagation();

          // Moves focus to the last node in the tree that is focusable without opening a node.
          let newTreeNodeIndex = treeNodes.length -1;
          if (newTreeNodeIndex >= 0)
          {
            let newTreeNode = treeNodes[newTreeNodeIndex];
            WebTreeView.SetFocus (newTreeNode, activeTreeNode);
          }

          return;
        }
    }
  };

  private static SetFocus (newTreeNode: HTMLElement, oldTreeNode: HTMLElement): void
  {
    newTreeNode.setAttribute ('tabindex', '0');
    newTreeNode.focus();
    oldTreeNode.setAttribute ('tabindex', '-1');
  };

  private static ToggleExpander (treeNode: HTMLElement): void
  {
    const expanderSpan = treeNode.parentElement!.parentElement!;
    if (expanderSpan.getAttribute('class') !== 'treeViewNode')
    {
      return;
    }

    const expanderAnchor = expanderSpan.querySelector<HTMLElement> ('a[aria-hidden=true]')!;
    expanderAnchor.click();
  };

  private static SelectNode (treeNode: HTMLElement): void
  {
    treeNode.setAttribute ('tabindex', '0');
    treeNode.focus();
    treeNode.click();
  };
}