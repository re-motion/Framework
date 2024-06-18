// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 

//
// Option.cs
//
// Authors:
//  Jonathan Pryor <jpryor@novell.com>
//
// Copyright (C) 2008 Novell (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Remotion.Mixins.XRef.Options
{
  public abstract class Option
  {
    protected Option (string prototype, string description, int maxValueCount = 1)
    {
      if (prototype == null)
        throw new ArgumentNullException("prototype");
      if (prototype.Length == 0)
        throw new ArgumentException("Cannot be the empty string.", "prototype");
      if (maxValueCount < 0)
        throw new ArgumentOutOfRangeException("maxValueCount");

      Prototype = prototype;
      Names = prototype.Split('|');
      Description = description;
      MaxValueCount = maxValueCount;
      OptionValueType = ParsePrototype();

      if (MaxValueCount == 0 && OptionValueType != OptionValueType.None)
        throw new ArgumentException(
            "Cannot provide maxValueCount of 0 for OptionValueType.Required or " +
            "OptionValueType.Optional.",
            "maxValueCount");
      if (OptionValueType == OptionValueType.None && maxValueCount > 1)
        throw new ArgumentException(
            string.Format("Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount),
            "maxValueCount");
      if (Array.IndexOf(Names, "<>") >= 0 &&
          ((Names.Length == 1 && OptionValueType != OptionValueType.None) ||
           (Names.Length > 1 && MaxValueCount > 1)))
        throw new ArgumentException(
            "The default option handler '<>' cannot require values.",
            "prototype");
    }

    static readonly char[] s_nameTerminator = new[] { '=', ':' };

    public string Prototype { get; private set; }
    public string Description { get; private set; }
    public OptionValueType OptionValueType { get; private set; }
    public int MaxValueCount { get; private set; }
    internal string[] Names { get; private set; }
    internal string[] ValueSeparators { get; private set; }

    private OptionValueType ParsePrototype ()
    {
      var type = '\0';
      var seps = new List<string>();
      for (int i = 0; i < Names.Length; ++i)
      {
        var name = Names[i];
        if (name.Length == 0)
          throw new NotSupportedException("Empty option names are not supported.");

        var end = name.IndexOfAny(s_nameTerminator);
        if (end == -1)
          continue;
        Names[i] = name.Substring(0, end);
        if (type == '\0' || type == name[end])
          type = name[end];
        else
          throw new NotSupportedException(
              string.Format("Conflicting option types: '{0}' vs. '{1}'.", type, name[end]));
        AddSeparators(name, end, seps);
      }

      if (type == '\0')
        return OptionValueType.None;

      if (MaxValueCount <= 1 && seps.Count != 0)
        throw new NotSupportedException(
            string.Format("Cannot provide key/value separators for Options taking {0} value(s).", MaxValueCount));
      if (MaxValueCount > 1)
      {
        if (seps.Count == 0)
          ValueSeparators = new[] { ":", "=" };
        else if (seps.Count == 1 && seps[0].Length == 0)
          ValueSeparators = null;
        else
          ValueSeparators = seps.ToArray();
      }

      return type == '=' ? OptionValueType.Required : OptionValueType.Optional;
    }

    private static void AddSeparators (string name, int end, ICollection<string> seps)
    {
      int start = -1;
      for (int i = end + 1; i < name.Length; ++i)
      {
        switch (name[i])
        {
          case '{':
            if (start != -1)
              throw new NotSupportedException(
                  string.Format("Ill-formed name/value separator found in \"{0}\".", name));
            start = i + 1;
            break;
          case '}':
            if (start == -1)
              throw new NotSupportedException(
                  string.Format("Ill-formed name/value separator found in \"{0}\".", name));
            seps.Add(name.Substring(start, i - start));
            start = -1;
            break;
          default:
            if (start == -1)
              seps.Add(name[i].ToString(CultureInfo.InvariantCulture));
            break;
        }
      }

      if (start != -1)
        throw new NotSupportedException(
            string.Format("Ill-formed name/value separator found in \"{0}\".", name));
    }

    public void Invoke (OptionContext c)
    {
      OnParseComplete(c);
      c.OptionName = null;
      c.Option = null;
      c.OptionValues.Clear();
    }

    protected abstract void OnParseComplete (OptionContext c);

    public override string ToString ()
    {
      return Prototype;
    }
  }
}
