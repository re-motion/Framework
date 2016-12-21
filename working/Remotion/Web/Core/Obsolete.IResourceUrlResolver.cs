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
using Remotion.Web.UI.Controls;

namespace Remotion.Web
{
  /// <summary>
  ///   Resolve the relative image URL into an absolute image url.
  /// </summary>
  [Obsolete ("Use IResourceUrlFactory instead. (Version 1.13.197)", true)]
  public interface IResourceUrlResolver
  {
    /// <summary>
    ///   Resolves a relative URL into an absolute URL.
    /// </summary>
    /// <param name="control"> 
    ///   The current <see cref="IControl"/>. Used to detect design time.
    /// </param>
    /// <param name="fileName">
    ///   The relative URL to be resolved into an absolute URL.
    /// </param>
    /// <param name="definingType"> 
    ///   The type that defines the resource. If the resource instance is not defined by a type, 
    ///   this is <see langword="null"/>. 
    /// </param>
    /// <param name="resourceType">
    ///   The type of resource to get. 
    /// </param>
    /// <returns>
    ///   The absulute URL.
    /// </returns>
    string GetResourceUrl (IControl control, Type definingType, ResourceType resourceType, string fileName);
  }
}
