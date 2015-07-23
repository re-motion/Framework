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
using System.Xml.Serialization;

namespace Remotion.ObjectBinding.Sample
{
  [XmlRoot ("Job")]
  [XmlType]
  [Serializable]
  public class Job : BindableXmlObject
  {
    public static Job GetObject (Guid id)
    {
      return GetObject<Job> (id);
    }

    public static Job CreateObject ()
    {
      return CreateObject<Job>();
    }

    public static Job CreateObject (Guid id)
    {
      return CreateObject<Job> (id);
    }

    private string _title;
    private DateTime _startDate;
    private DateTime _endDate;

    protected Job ()
    {
    }

    [XmlAttribute]
    public string Title
    {
      get { return _title; }
      set { _title = value; }
    }

    [XmlAttribute (DataType="date")]
    [DateProperty]
    public DateTime StartDate
    {
      get { return _startDate; }
      set { _startDate = value; }
    }

    [XmlAttribute (DataType="date")]
    [DateProperty]
    public DateTime EndDate
    {
      get { return _endDate; }
      set { _endDate = value; }
    }

    public override string DisplayName
    {
      get { return Title; }
    }

    public override string ToString ()
    {
      return DisplayName;
    }
  }
}
