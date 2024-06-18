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
// OptionValueCollection.cs
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
using System.Collections;
using System.Collections.Generic;

namespace Remotion.Mixins.XRef.Options
{
  public class OptionValueCollection : IList, IList<string>
  {
    private readonly List<string> _values = new();
    private readonly OptionContext _context;

    internal OptionValueCollection (OptionContext context)
    {
      _context = context;
    }

    #region ICollection

    void ICollection.CopyTo (Array array, int index)
    {
      (_values as ICollection).CopyTo(array, index);
    }

    bool ICollection.IsSynchronized => (_values as ICollection).IsSynchronized;

    object ICollection.SyncRoot => (_values as ICollection).SyncRoot;

    #endregion

    #region ICollection<T>

    public void Add (string item)
    {
      _values.Add(item);
    }

    public void Clear ()
    {
      _values.Clear();
    }

    public bool Contains (string item)
    {
      return _values.Contains(item);
    }

    public void CopyTo (string[] array, int arrayIndex)
    {
      _values.CopyTo(array, arrayIndex);
    }

    public bool Remove (string item)
    {
      return _values.Remove(item);
    }

    public int Count => _values.Count;

    public bool IsReadOnly => false;

    #endregion

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return _values.GetEnumerator();
    }

    #endregion

    #region IEnumerable<T>

    public IEnumerator<string> GetEnumerator ()
    {
      return _values.GetEnumerator();
    }

    #endregion

    #region IList

    int IList.Add (object? value)
    {
      return (_values as IList).Add(value);
    }

    bool IList.Contains (object? value)
    {
      return (_values as IList).Contains(value);
    }

    int IList.IndexOf (object? value)
    {
      return (_values as IList).IndexOf(value);
    }

    void IList.Insert (int index, object? value)
    {
      (_values as IList).Insert(index, value);
    }

    void IList.Remove (object? value)
    {
      (_values as IList).Remove(value);
    }

    void IList.RemoveAt (int index)
    {
      (_values as IList).RemoveAt(index);
    }

    bool IList.IsFixedSize => false;

    object? IList.this [int index]
    {
      get => this[index];
      set => (_values as IList)[index] = value;
    }

    #endregion

    #region IList<T>

    public int IndexOf (string item)
    {
      return _values.IndexOf(item);
    }

    public void Insert (int index, string item)
    {
      _values.Insert(index, item);
    }

    public void RemoveAt (int index)
    {
      _values.RemoveAt(index);
    }

    private void AssertValid (int index)
    {
      if (_context.Option == null)
        throw new InvalidOperationException("OptionContext.Option is null.");
      if (index >= _context.Option.MaxValueCount)
        throw new ArgumentOutOfRangeException("index");
      if (_context.Option.OptionValueType == OptionValueType.Required &&
          index >= _values.Count)
        throw new OptionException(
            string.Format(
                _context.OptionSet.MessageLocalizer("Missing required value for option '{0}'."),
                _context.OptionName),
            _context.OptionName);
    }

    public string this [int index]
    {
      get
      {
        AssertValid(index);
        return index >= _values.Count ? null! : _values[index];
      }
      set => _values[index] = value;
    }

    #endregion

    public override string ToString ()
    {
      return string.Join(", ", _values.ToArray());
    }
  }
}
