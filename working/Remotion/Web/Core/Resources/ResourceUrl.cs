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
using Remotion.Utilities;

namespace Remotion.Web.Resources
{
  /// <summary>
  /// Represents the absolute URL for a resource file that does not change with the <see cref="ResourceTheme"/>.
  /// </summary>
  public class ResourceUrl : IResourceUrl
  {
    private readonly IResourcePathBuilder _resourcePathBuilder;
    private readonly Type _definingType;
    private readonly ResourceType _resourceType;
    private readonly string _relativeUrl;

    public ResourceUrl (IResourcePathBuilder resourcePathBuilder, Type definingType, ResourceType resourceType, string relativeUrl)
    {
      ArgumentUtility.CheckNotNull ("resourcePathBuilder", resourcePathBuilder);
      ArgumentUtility.CheckNotNull ("definingType", definingType);
      ArgumentUtility.CheckNotNull ("resourceType", resourceType);
      ArgumentUtility.CheckNotNullOrEmpty ("relativeUrl", relativeUrl);

      _resourcePathBuilder = resourcePathBuilder;
      _definingType = definingType;
      _resourceType = resourceType;
      _relativeUrl = relativeUrl;
    }

    public IResourcePathBuilder ResourcePathBuilder
    {
      get { return _resourcePathBuilder; }
    }

    public Type DefiningType
    {
      get { return _definingType; }
    }

    public ResourceType ResourceType
    {
      get { return _resourceType; }
    }

    public string RelativeUrl
    {
      get { return _relativeUrl; }
    }

    public virtual string GetUrl ()
    {
      return _resourcePathBuilder.BuildAbsolutePath (DefiningType.Assembly, ResourceType.Name, RelativeUrl);
    }
  }
}