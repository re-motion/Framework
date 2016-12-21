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
using System.Linq;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Serialization
{
  public static class FlattenedSerializationLogger
  {
    public static void LogStatistics (ILog log, object[] objects, int[] ints, bool[] bools)
    {
      ArgumentUtility.CheckNotNull ("log", log);
      ArgumentUtility.CheckNotNull ("objects", objects);
      ArgumentUtility.CheckNotNull ("ints", ints);
      ArgumentUtility.CheckNotNull ("bools", bools);

      if (log.IsDebugEnabled())
      {
        log.DebugFormat (
            "Flattened serialization: {0} objects ({1} unique), {2} integers, and {3} boolean values.",
            objects.Length,
            objects.Distinct ().Count (),
            ints.Length,
            bools.Length);

        var objectsByType = from o in objects
                            group o by o != null ? o.GetType () : typeof (object);
        var groupingsWithCount = from g in objectsByType
                                 let count = g.Count ()
                                 orderby count descending
                                 select new { g.Key, Count = count };

        var statisticsString = string.Join (Environment.NewLine, groupingsWithCount.Select (g => g.Key + ": " + g.Count));
        log.Debug (statisticsString);
      }
      
    }
  }
}
