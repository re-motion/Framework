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
using Remotion.ServiceLocation;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// Defines a method for resolving resource urls for the current <see cref="ResourceTheme"/>.
  /// The <see cref="IInfrastructureResourceUrlFactory"/> is only intended for use with controls located in <b>Remotion.Web</b> that do not have their own renderers.
  /// </summary>
 public interface IInfrastructureResourceUrlFactory
  {
    IResourceUrl CreateThemedResourceUrl (ResourceType resourceType, string relativeUrl);
  }

  #region Obsolete interfaces

  [Obsolete ("Use IInfrastructureResourceUrlFactory instead. (Version 1.13.197)",  true)]
  public interface IThemedResourceUrlResolverFactory
  {
    [Obsolete ("Retrieve an instance of type IInfrastructureResourceUrlFactory instead via the IoC container. (Version 1.13.197)", true)]
    IThemedResourceUrlResolver CreateResourceUrlResolver ();
  }

  [Obsolete ("Use IInfrastructureResourceUrlFactory instead. (Version 1.13.197)",  true)]
  public interface IThemedResourceUrlResolver
  {
    [Obsolete ("Use IInfrastructureResourceUrlFactory.CreateResourceUrl(...) instead. (Version 1.13.197)",  true)]
    string GetResourceUrl (IControl control, ResourceType resourceType, string relativeUrl);
  }

  #endregion
}