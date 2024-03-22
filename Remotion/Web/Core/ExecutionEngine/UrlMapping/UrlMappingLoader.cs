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
using System.Collections.Generic;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.UrlMapping
{
  public class UrlMappingLoader
  {
    private readonly IUrlMappingFileFinder _urlMappingFileFinder;
    private readonly IUrlMappingFileLoader _urlMappingFileLoader;

    public UrlMappingLoader (IUrlMappingFileFinder urlMappingFileFinder, IUrlMappingFileLoader urlMappingFileLoader)
    {
      ArgumentUtility.CheckNotNull(nameof(urlMappingFileFinder), urlMappingFileFinder);
      ArgumentUtility.CheckNotNull(nameof(urlMappingFileLoader), urlMappingFileLoader);

      _urlMappingFileFinder = urlMappingFileFinder;
      _urlMappingFileLoader = urlMappingFileLoader;
    }

    public UrlMappingConfiguration CreateUrlMappingConfiguration ()
    {
      var urlMappingConfiguration = new UrlMappingConfiguration();

      var entries = _urlMappingFileFinder.GetUrlMappingFilePaths()
          .SelectMany(file => _urlMappingFileLoader.LoadUrlMappingEntries(file).Select(entry => (file, entry)));

      var entryIDToResourceLookup = new Dictionary<string, (string file, string resource)>();
      var resourceToEntryLookup = new Dictionary<string, (string file, UrlMappingEntry entry)>();
      foreach (var (filePath, urlMappingEntry) in entries)
      {
        if (urlMappingEntry.ID != null)
        {
          if (entryIDToResourceLookup.TryGetValue(urlMappingEntry.ID, out var existingResource) && urlMappingEntry.Resource != existingResource.resource)
          {
            throw new InvalidOperationException(
                $"Two URL mapping entries from files '{filePath}' and '{existingResource.file}' have the same ID '{urlMappingEntry.ID}', "
                + $"but they point to different resources: '{existingResource.resource}' and '{urlMappingEntry.Resource}'.");
          }

          if (existingResource.resource == null)
            entryIDToResourceLookup[urlMappingEntry.ID] = (filePath, urlMappingEntry.Resource);
        }

        if (resourceToEntryLookup.TryGetValue(urlMappingEntry.Resource, out var existingEntryInfo))
        {
          if (!existingEntryInfo.entry.FunctionType.IsAssignableFrom(urlMappingEntry.FunctionType))
          {
            throw new InvalidOperationException(
                $"URL mapping entry for resource '{urlMappingEntry.Resource}' from the file '{filePath}' "
                + $"cannot override the existing URL mapping entry from the file '{existingEntryInfo.file}' as the function type "
                + $"'{urlMappingEntry.FunctionType}' is not assignable to the existing "
                + $"function type '{existingEntryInfo.entry.FunctionType}'.");
          }

          urlMappingConfiguration.Mappings.Remove(existingEntryInfo.entry);
        }

        resourceToEntryLookup[urlMappingEntry.Resource] = (filePath, urlMappingEntry);
        urlMappingConfiguration.Mappings.Add(urlMappingEntry);
      }

      return urlMappingConfiguration;
    }
  }
}
