// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
// Cannot detect paste operations.
function TextBoxStyle_OnKeyDown (textBox, length)
{
  if (textBox.disabled)
    return true;
    
  var isInsertDelete = event.keyCode == 45 || event.keyCode == 46;
  var isCursor = event.keyCode >= 37 && event.keyCode <= 40; // Left, Top, Right, Buttom
  var isPagePosition = event.keyCode >= 33 && event.keyCode <= 36; // Home, End, PageUp, PageDown
  var isControlCharacter = 
         event.keyCode < 32 
      || isInsertDelete 
      || isCursor 
      || isPagePosition
      || event.ctrlKey 
      || event.altKey;
  var isLineFeed = event.keyCode == 10;
  var isCarriageReturn = event.keyCode == 13;
  
  
  if (isControlCharacter && ! (isLineFeed || isCarriageReturn))
    return true;
  
  if (textBox.value.length >= length) 
    return false;
    
  return true;
}
