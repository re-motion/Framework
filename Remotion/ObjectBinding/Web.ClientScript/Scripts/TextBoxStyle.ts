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

class TextBoxStyle
{
  // Cannot detect paste operations.
  public static OnKeyDown (textBox: HTMLInputElement, length: number): boolean
  {
    if (textBox.disabled)
      return true;
    
    // TODO RM-7677: Pass event objects into the handler methods instead of using the ambient event variable
    function typeOverride <T>(value: unknown): asserts value is T {};
    typeOverride<KeyboardEvent>(event);
    
    const isInsertDelete = event.keyCode == 45 || event.keyCode == 46;
    const isCursor = event.keyCode >= 37 && event.keyCode <= 40; // Left, Top, Right, Buttom
    const isPagePosition = event.keyCode >= 33 && event.keyCode <= 36; // Home, End, PageUp, PageDown
    const isControlCharacter = 
          event.keyCode < 32 
        || isInsertDelete 
        || isCursor 
        || isPagePosition
        || event.ctrlKey 
        || event.altKey;
    const isLineFeed = event.keyCode == 10;
    const isCarriageReturn = event.keyCode == 13;
    
    
    if (isControlCharacter && ! (isLineFeed || isCarriageReturn))
      return true;
    
    if (textBox.value.length >= length) 
      return false;
      
    return true;
  }
}