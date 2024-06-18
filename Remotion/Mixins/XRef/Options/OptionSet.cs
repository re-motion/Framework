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
// OptionSet.cs
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Remotion.Mixins.XRef.Options
{
  public sealed class OptionSet : KeyedCollection<string, Option>
  {
    public OptionSet (Converter<string, string>? localizer = null)
    {
      MessageLocalizer = localizer ?? (f => f);
    }

    public Converter<string, string> MessageLocalizer { get; private set; }

    protected override string GetKeyForItem (Option item)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      if (item.Names is { Length: > 0 })
        return item.Names[0];
      // This should never happen, as it's invalid for Option to be
      // constructed w/o any names.
      throw new InvalidOperationException("Option has no names!");
    }

    protected override void InsertItem (int index, Option item)
    {
      base.InsertItem(index, item);
      AddImpl(item);
    }

    protected override void RemoveItem (int index)
    {
      base.RemoveItem(index);
      Option p = Items[index];
      // KeyedCollection.RemoveItem() handles the 0th item
      for (int i = 1; i < p.Names.Length; ++i)
      {
        Dictionary?.Remove(p.Names[i]);
      }
    }

    protected override void SetItem (int index, Option item)
    {
      base.SetItem(index, item);
      RemoveItem(index);
      AddImpl(item);
    }

    private void AddImpl (Option option)
    {
      if (option == null)
        throw new ArgumentNullException("option");
      var added = new List<string>(option.Names.Length);
      try
      {
        // KeyedCollection.InsertItem/SetItem handle the 0th name.
        for (int i = 1; i < option.Names.Length; ++i)
        {
          Dictionary?.Add(option.Names[i], option);
          added.Add(option.Names[i]);
        }
      }
      catch (Exception)
      {
        foreach (string name in added)
          Dictionary?.Remove(name);
        throw;
      }
    }

    public void Add (string prototype, string description, Action<string> action)
    {
      if (action == null)
        throw new ArgumentNullException("action");
      Option p = new ActionOption(
          prototype,
          description,
          1,
          v => action(v[0]));
      Add(p);
    }

    private OptionContext CreateOptionContext ()
    {
      return new OptionContext(this);
    }

    public void Parse (IEnumerable<string> arguments)
    {
      var c = CreateOptionContext();
      c.OptionIndex = -1;
      bool process = true;
      var unprocessed = new List<string>();
      Option def = Contains("<>") ? this["<>"] : null;
      foreach (string argument in arguments)
      {
        ++c.OptionIndex;
        if (argument == "--")
        {
          process = false;
          continue;
        }

        if (!process)
        {
          Unprocessed(unprocessed, def, c, argument);
          continue;
        }

        if (!Parse(argument, c))
          Unprocessed(unprocessed, def, c, argument);
      }

      if (unprocessed.Any())
        throw new OptionException(string.Format("Error: Found unknown option \"{0}\"", unprocessed.First()), null);

      if (c.Option != null)
        c.Option.Invoke(c);
    }

    private static void Unprocessed (ICollection<string> extra, Option def, OptionContext c, string argument)
    {
      if (def == null)
      {
        extra.Add(argument);
        return;
      }

      c.OptionValues.Add(argument);
      c.Option = def;
      c.Option.Invoke(c);
    }

    private readonly Regex _valueOption = new(
        @"^(?<flag>--|-|/)(?<name>[^:=]+)((?<sep>[:=])(?<value>.*))?$");

    private bool GetOptionParts (string argument, out string flag, out string name, out string sep, out string value)
    {
      if (argument == null)
        throw new ArgumentNullException("argument");

      flag = name = sep = value = null;
      Match m = _valueOption.Match(argument);
      if (!m.Success)
      {
        return false;
      }

      flag = m.Groups["flag"].Value;
      name = m.Groups["name"].Value;
      if (m.Groups["sep"].Success && m.Groups["value"].Success)
      {
        sep = m.Groups["sep"].Value;
        value = m.Groups["value"].Value;
      }

      return true;
    }

    private bool Parse (string argument, OptionContext c)
    {
      if (c.Option != null)
      {
        ParseValue(argument, c);
        return true;
      }

      string f, n, s, v;
      if (!GetOptionParts(argument, out f, out n, out s, out v))
        return false;

      if (Contains(n))
      {
        var p = this[n];
        c.OptionName = f + n;
        c.Option = p;
        switch (p.OptionValueType)
        {
          case OptionValueType.None:
            c.OptionValues.Add(n);
            c.Option.Invoke(c);
            break;
          case OptionValueType.Optional:
          case OptionValueType.Required:
            ParseValue(v, c);
            break;
        }

        return true;
      }

      // no match; is it a bool option?
      if (ParseBool(argument, n, c))
        return true;
      // is it a bundled option?
      if (ParseBundledValue(f, string.Concat(n, s, v), c))
        return true;

      return false;
    }

    private void ParseValue (string option, OptionContext c)
    {
      if (option != null)
        foreach (string o in c.Option.ValueSeparators != null
                     ? option.Split(c.Option.ValueSeparators, StringSplitOptions.None)
                     : new[] { option })
        {
          c.OptionValues.Add(o);
        }

      if (c.OptionValues.Count == c.Option.MaxValueCount ||
          c.Option.OptionValueType == OptionValueType.Optional)
        c.Option.Invoke(c);
      else if (c.OptionValues.Count > c.Option.MaxValueCount)
      {
        throw new OptionException(
            MessageLocalizer(
                string.Format(
                    "Error: Found {0} option values when expecting {1}.",
                    c.OptionValues.Count,
                    c.Option.MaxValueCount)),
            c.OptionName);
      }
    }

    private bool ParseBool (string option, string n, OptionContext c)
    {
      string rn;
      if (n.Length >= 1 && (n[n.Length - 1] == '+' || n[n.Length - 1] == '-') &&
          Contains((rn = n.Substring(0, n.Length - 1))))
      {
        var p = this[rn];
        string v = n[n.Length - 1] == '+' ? option : null;
        c.OptionName = option;
        c.Option = p;
        c.OptionValues.Add(v);
        p.Invoke(c);
        return true;
      }

      return false;
    }

    private bool ParseBundledValue (string f, string n, OptionContext c)
    {
      if (f != "-")
        return false;
      for (int i = 0; i < n.Length; ++i)
      {
        string opt = f + n[i].ToString(CultureInfo.InvariantCulture);
        string rn = n[i].ToString(CultureInfo.InvariantCulture);
        if (!Contains(rn))
        {
          if (i == 0)
            return false;
          throw new OptionException(
              string.Format(
                  MessageLocalizer(
                      "Cannot bundle unregistered option '{0}'."),
                  opt),
              opt);
        }

        var p = this[rn];
        switch (p.OptionValueType)
        {
          case OptionValueType.None:
            Invoke(c, opt, n, p);
            break;
          case OptionValueType.Optional:
          case OptionValueType.Required:
          {
            string v = n.Substring(i + 1);
            c.Option = p;
            c.OptionName = opt;
            ParseValue(v.Length != 0 ? v : null, c);
            return true;
          }
          default:
            throw new InvalidOperationException("Unknown OptionValueType: " + p.OptionValueType);
        }
      }

      return true;
    }

    private static void Invoke (OptionContext c, string name, string value, Option option)
    {
      c.OptionName = name;
      c.Option = option;
      c.OptionValues.Add(value);
      option.Invoke(c);
    }

    private const int c_optionWidth = 29;

    public void WriteOptionDescriptions (TextWriter o)
    {
      foreach (Option p in this)
      {
        int written = 0;
        if (!WriteOptionPrototype(o, p, ref written))
          continue;

        if (written < c_optionWidth)
          o.Write(new string(' ', c_optionWidth - written));
        else
        {
          o.WriteLine();
          o.Write(new string(' ', c_optionWidth));
        }

        var lines = GetLines(MessageLocalizer(GetDescription(p.Description)));
        o.WriteLine(lines[0]);
        var prefix = new string(' ', c_optionWidth + 2);
        for (int i = 1; i < lines.Count; ++i)
        {
          o.Write(prefix);
          o.WriteLine(lines[i]);
        }
      }
    }

    bool WriteOptionPrototype (TextWriter o, Option p, ref int written)
    {
      string[] names = p.Names;

      int i = GetNextOptionIndex(names, 0);
      if (i == names.Length)
        return false;

      if (names[i].Length == 1)
      {
        Write(o, ref written, "  -");
        Write(o, ref written, names[0]);
      }
      else
      {
        Write(o, ref written, "      --");
        Write(o, ref written, names[0]);
      }

      for (i = GetNextOptionIndex(names, i + 1);
           i < names.Length;
           i = GetNextOptionIndex(names, i + 1))
      {
        Write(o, ref written, ", ");
        Write(o, ref written, names[i].Length == 1 ? "-" : "--");
        Write(o, ref written, names[i]);
      }

      if (p.OptionValueType == OptionValueType.Optional ||
          p.OptionValueType == OptionValueType.Required)
      {
        if (p.OptionValueType == OptionValueType.Optional)
        {
          Write(o, ref written, MessageLocalizer("["));
        }

        Write(o, ref written, MessageLocalizer("=" + GetArgumentName(0, p.MaxValueCount, p.Description)));
        string sep = p.ValueSeparators != null && p.ValueSeparators.Length > 0
            ? p.ValueSeparators[0]
            : " ";
        for (int c = 1; c < p.MaxValueCount; ++c)
        {
          Write(o, ref written, MessageLocalizer(sep + GetArgumentName(c, p.MaxValueCount, p.Description)));
        }

        if (p.OptionValueType == OptionValueType.Optional)
        {
          Write(o, ref written, MessageLocalizer("]"));
        }
      }

      return true;
    }

    static int GetNextOptionIndex (string[] names, int i)
    {
      while (i < names.Length && names[i] == "<>")
      {
        ++i;
      }

      return i;
    }

    static void Write (TextWriter o, ref int n, string s)
    {
      n += s.Length;
      o.Write(s);
    }

    private static string GetArgumentName (int index, int maxIndex, string description)
    {
      if (description == null)
        return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1);
      string[] nameStart = maxIndex == 1 ? new[] { "{0:", "{" } : new[] { "{" + index + ":" };
      foreach (string t in nameStart)
      {
        int start, j = 0;
        do
        {
          start = description.IndexOf(t, j, StringComparison.Ordinal);
        } while (start >= 0 && j != 0 && description[j++ - 1] == '{');

        if (start == -1)
          continue;
        var end = description.IndexOf("}", start, StringComparison.Ordinal);
        if (end == -1)
          continue;
        return description.Substring(start + t.Length, end - start - t.Length);
      }

      return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1);
    }

    private static string GetDescription (string description)
    {
      if (description == null)
        return string.Empty;
      var sb = new StringBuilder(description.Length);
      var start = -1;
      for (var i = 0; i < description.Length; ++i)
      {
        switch (description[i])
        {
          case '{':
            if (i == start)
            {
              sb.Append('{');
              start = -1;
            }
            else if (start < 0)
              start = i + 1;

            break;
          case '}':
            if (start < 0)
            {
              if ((i + 1) == description.Length || description[i + 1] != '}')
                throw new InvalidOperationException("Invalid option description: " + description);
              ++i;
              sb.Append("}");
            }
            else
            {
              sb.Append(description.Substring(start, i - start));
              start = -1;
            }

            break;
          case ':':
            if (start < 0)
              goto default;
            start = i + 1;
            break;
          default:
            if (start < 0)
              sb.Append(description[i]);
            break;
        }
      }

      return sb.ToString();
    }

    private static List<string> GetLines (string description)
    {
      var lines = new List<string>();
      if (string.IsNullOrEmpty(description))
      {
        lines.Add(string.Empty);
        return lines;
      }

      const int length = 80 - c_optionWidth - 2;
      int start = 0, end;
      do
      {
        end = GetLineEnd(start, length, description);
        bool cont = false;
        if (end < description.Length)
        {
          char c = description[end];
          if (c == '-' || (char.IsWhiteSpace(c) && c != '\n'))
            ++end;
          else if (c != '\n')
          {
            cont = true;
            --end;
          }
        }

        lines.Add(description.Substring(start, end - start));
        if (cont)
        {
          lines[lines.Count - 1] += "-";
        }

        start = end;
        if (start < description.Length && description[start] == '\n')
          ++start;
      } while (end < description.Length);

      return lines;
    }

    private static int GetLineEnd (int start, int length, string description)
    {
      int end = Math.Min(start + length, description.Length);
      int sep = -1;
      for (int i = start; i < end; ++i)
      {
        switch (description[i])
        {
          case ' ':
          case '\t':
          case '\v':
          case '-':
          case ',':
          case '.':
          case ';':
            sep = i;
            break;
          case '\n':
            return i;
        }
      }

      if (sep == -1 || end == description.Length)
        return end;
      return sep;
    }
  }
}
