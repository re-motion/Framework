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
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Diagnostics
{
  public struct MemoryUsageInfo
  {
    public static MemoryUsageInfo GetCurrent (string description)
    {
      using (var process = Process.GetCurrentProcess ())
      {
        return new MemoryUsageInfo (
            description,
            new ByteValue (process.WorkingSet64),
            new ByteValue (GC.GetTotalMemory (false)),
            new ByteValue (GC.GetTotalMemory (true)));
      }
    }

    private readonly string _description;
    private readonly ByteValue _workingSet;
    private readonly ByteValue _managedMemoryBeforeCollect;
    private readonly ByteValue _managedMemoryAfterCollect;

    public MemoryUsageInfo (string description, ByteValue workingSet, ByteValue managedMemoryBeforeCollect, ByteValue managedMemoryAfterCollect)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("description", description);

      _description = description;
      _managedMemoryAfterCollect = managedMemoryAfterCollect;
      _managedMemoryBeforeCollect = managedMemoryBeforeCollect;
      _workingSet = workingSet;
    }

    public string Description
    {
      get { return _description; }
    }

    public ByteValue WorkingSet
    {
      get { return _workingSet; }
    }

    public ByteValue ManagedMemoryBeforeCollect
    {
      get { return _managedMemoryBeforeCollect; }
    }

    public ByteValue ManagedMemoryAfterCollect
    {
      get { return _managedMemoryAfterCollect; }
    }

    public void DumpToConsole ()
    {
      Console.WriteLine (ToString());
    }

    public override string ToString ()
    {
      return string.Format ("{0}:{1}\tWorking set: {2}{1}\tManaged memory before collect: {3}{1}\tAfter collect: {4}",
                            Description,
                            Environment.NewLine,
                            WorkingSet,
                            ManagedMemoryBeforeCollect,
                            ManagedMemoryAfterCollect);
    }

    public void DumpComparisonToConsole (MemoryUsageInfo comparison)
    {
      Console.WriteLine (ToDifferenceString (comparison));
    }

    public string ToDifferenceString (MemoryUsageInfo comparison)
    {
      return string.Format (
          "Compared to {0}:{1}\tWorking set: {2}{1}\tManaged memory before collect: {3}{1}\tAfter collect: {4}",
          comparison.Description,
          Environment.NewLine,
          (WorkingSet - comparison.WorkingSet).ToDifferenceString (),
          (ManagedMemoryBeforeCollect - comparison.ManagedMemoryBeforeCollect).ToDifferenceString (),
          (ManagedMemoryAfterCollect - comparison.ManagedMemoryAfterCollect).ToDifferenceString ());
    }

    public string ToCSVString ()
    {
      return string.Format ("{0};{1};{2}", WorkingSet.Bytes, ManagedMemoryBeforeCollect.Bytes, ManagedMemoryAfterCollect.Bytes);
    }
  }
}
