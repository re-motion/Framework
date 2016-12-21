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
using System.Xml;
using Remotion.Utilities;

namespace Remotion.Security.UnitTests.XmlAsserter
{
  public class XmlnsAttributeEventArgs : EventArgs
  {
    private string _namespaceUri;
    private string _prefix;
    private bool _isDefaultNamespace;

    public XmlnsAttributeEventArgs (string namespaceUri, string prefix)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("namespaceUri", namespaceUri);

      _namespaceUri = namespaceUri;
      _prefix = prefix;
      _isDefaultNamespace = string.IsNullOrEmpty (prefix);
    }

    public string NamespaceUri
    {
      get { return _namespaceUri; }
    }

    public string Prefix
    {
      get { return _prefix; }
    }

    public bool IsDefaultNamespace
    {
      get { return _isDefaultNamespace; }
    }
  }

  public delegate void XmlnsAttributeEventHandler (object sender, XmlnsAttributeEventArgs args);

  public class XmlnsAttributeHandler
  {
    public event XmlnsAttributeEventHandler XmlnsAttributeFound;

    private bool _filter;

    public XmlnsAttributeHandler ()
    {
      _filter = true;
    }

    public bool Filter
    {
      get { return _filter; }
      set { _filter = value; }
    }

    public bool IsXmlnsAttribute (XmlAttribute attribute)
    {
      if (attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/")
        return false;

      if (attribute.LocalName == "xmlns")
        return true;

      return attribute.Prefix == "xmlns";
    }

    public void Handle (XmlAttributeCollection attributes)
    {
      for (int i = attributes.Count - 1; i >= 0; i--)
      {
        if (IsXmlnsAttribute (attributes[i]))
        {
          ExtractNamespaceInformation (attributes[i]);

          if (_filter)
            attributes.Remove (attributes[i]);
        }
      }
    }

    private void ExtractNamespaceInformation (XmlAttribute attribute)
    {
      if (attribute.LocalName == "xmlns")
        OnXmlnsAttributeFound (new XmlnsAttributeEventArgs (attribute.Value, null));

      if (attribute.Prefix == "xmlns")
        OnXmlnsAttributeFound (new XmlnsAttributeEventArgs (attribute.Value, attribute.LocalName));
    }

    protected virtual void OnXmlnsAttributeFound (XmlnsAttributeEventArgs args)
    {
      if (XmlnsAttributeFound != null)
        XmlnsAttributeFound (this, args);
    }
  }
}
