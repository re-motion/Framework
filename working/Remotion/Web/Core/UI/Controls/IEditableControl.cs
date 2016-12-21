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
using System;

namespace Remotion.Web.UI.Controls
{

public interface IEditableControl: IControl
{
  /// <summary>
  ///   Specifies whether the value of the control has been changed on the Client since the last load/save operation.
  /// </summary>
  /// <remarks>
  ///   Initially, the value of <c>IsDirty</c> is <see langword="true" />. The value is set to <see langword="false" /> during loading
  ///   and saving values. Resetting <c>IsDirty</c> during saving is not implemented by all controls.
  /// </remarks>
  // TODO: redesign IsDirty semantics!
  bool IsDirty { get; }
  string[] GetTrackedClientIDs();
}

}
