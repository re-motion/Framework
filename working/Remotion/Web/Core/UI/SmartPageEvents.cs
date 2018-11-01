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

namespace Remotion.Web.UI
{
  /// <summary> Specifies the client side events supported for registration by the <see cref="ISmartPage"/>. </summary>
  public enum SmartPageEvents
  {
    /// <summary> Rasied when the document has finished loading. Signature: <c>void Function (hasSubmitted, isCached, isAsynchronous)</c> </summary>
    OnLoad,
    /// <summary> Raised when the user posts back to the server. Signature: <c>void Function (eventTargetID, eventArgs)</c> </summary>
    OnPostBack,
    /// <summary> Raised when the user leaves the page. Signature: <c>void Function (hasSubmitted, isCached)</c> </summary>
    OnAbort,
    /// <summary> Raised when the user scrolls the page. Signature: <c>void Function ()</c> </summary>
    OnScroll,
    /// <summary> Raised when the user resizes the page. Signature: <c>void Function ()</c> </summary>
    OnResize,
    /// <summary> 
    ///   Raised before the request to load a new page (or reload the current page) is executed. Not supported in Opera.
    ///   Signature: <c>void Function ()</c>
    /// </summary>
    OnBeforeUnload,
    /// <summary> Raised before the page is removed from the window. Signature: <c>void Function ()</c> </summary>
    OnUnload
  }
}
