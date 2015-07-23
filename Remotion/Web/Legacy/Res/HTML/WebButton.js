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

function WebButton_MouseDown(element, cssClass)
{
  element.className += " " + cssClass;
  return false;
}

function WebButton_MouseUp (element, cssClass)
{
  element.className = element.className.replace (cssClass, '');
  return false;
}

function WebButton_MouseOut (element, cssClass)
{
  element.className = element.className.replace (cssClass, '');
  return false;
}

