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
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  ///   This interface contains all public members of <see cref="TemplateControl"/>. It is used to 
  ///   derive interfaces that will be implemented by deriving from <see cref="TemplateControl"/>.
  /// </summary>
  /// <remarks>
  ///   The reason for providing this interface is that derived interfaces do not need to be casted 
  ///   to <see cref="TemplateControl"/>.
  /// </remarks>
  public interface ITemplateControl : IControl, INamingContainer, IFilterResolutionService
  {
    event EventHandler AbortTransaction;
    event EventHandler CommitTransaction;
    event EventHandler Error;

    /// <summary>
    /// Loads a <see cref="T:System.Web.UI.Control"/> object from a file based on a specified virtual path.
    /// </summary>
    /// <returns>
    /// Returns the specified <see cref="T:System.Web.UI.Control"/>.
    /// </returns>
    /// <param name="virtualPath">The virtual path to a control file. 
    /// </param><exception cref="T:System.ArgumentNullException">The virtual path is null or empty.
    /// </exception>
    Control LoadControl (string virtualPath);

    /// <summary>
    /// Obtains an instance of the <see cref="T:System.Web.UI.ITemplate"/> interface from an external file.
    /// </summary>
    /// <returns>
    /// An instance of the specified template.
    /// </returns>
    /// <param name="virtualPath">The virtual path to a user control file. 
    /// </param>
    ITemplate LoadTemplate (string virtualPath);

    /// <summary>
    /// Parses an input string into a <see cref="T:System.Web.UI.Control"/> object on the Web Forms page or user control.
    /// </summary>
    /// <returns>
    /// The parsed <see cref="T:System.Web.UI.Control"/>.
    /// </returns>
    /// <param name="content">A string that contains a user control. 
    /// </param>
    Control ParseControl (string content);
  }
}
