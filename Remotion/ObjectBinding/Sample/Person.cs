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
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Sample
{
  [XmlRoot("Person")]
  [XmlType]
  [Serializable]
  public class Person : BindableXmlObject
  {
    public static Person GetObject (Guid id)
    {
      return GetObject<Person>(id);
    }

    public static Person CreateObject ()
    {
      return CreateObject<Person>();
    }

    public static Person CreateObject (Guid id)
    {
      return CreateObject<Person>(id);
    }

    private string _firstName;
    private string _lastName;
    private DateTime _dateOfBirth;
    private byte _height;
    private decimal? _income = 1000.50m;
    private Gender _gender;
    private MarriageStatus _marriageStatus;
    private DateTime _dateOfDeath;
#if NET6_0_OR_GREATER
    private DateOnly _dateOfCitizenship;
#else
    private DateTime _dateOfCitizenship;
#endif
    private bool _deceased = false;
    private string[] _cv;
    private Guid _partnerID;
    private ObservableCollection<Person> _children;
    private Guid[] _childIDs;
    private Guid[] _jobIDs;
    private Guid _fatherID;

    protected Person ()
    {
    }

    [XmlAttribute]
    public string FirstName
    {
      get { return _firstName; }
      set { _firstName = value; }
    }

    [XmlAttribute]
    public string LastName
    {
      get { return _lastName; }
      set { _lastName = value; }
    }

    [XmlAttribute]
    public DateTime DateOfBirth
    {
      get { return _dateOfBirth; }
      set { _dateOfBirth = value; }
    }

    [XmlAttribute]
    public byte Height
    {
      get { return _height; }
      set { _height = value; }
    }

    [XmlElement]
    public decimal? Income
    {
      get { return _income; }
      set { _income = value; }
    }

    [XmlAttribute]
    [DisableEnumValues(Gender.UnknownGender)]
    public Gender Gender
    {
      get { return _gender; }
      set { _gender = value; }
    }

    [XmlAttribute]
    [DisableEnumValues(MarriageStatus.Bigamist, MarriageStatus.Polygamist)]
    public MarriageStatus MarriageStatus
    {
      get { return _marriageStatus; }
      set { _marriageStatus = value; }
    }

    [XmlElement]
    [ObjectBinding(Visible = false)]
    public Guid PartnerID
    {
      get { return _partnerID; }
      set { _partnerID = value; }
    }

    [XmlIgnore]
    public Person Partner
    {
      get { return (_partnerID != Guid.Empty) ? Person.GetObject(_partnerID) : null; }
      set { _partnerID = (value != null) ? value.ID : Guid.Empty; }
    }

    [XmlElement]
    [ObjectBinding(Visible = false)]
    public Guid FatherID
    {
      get { return _fatherID; }
      set { _fatherID = value; }
    }

    [XmlIgnore]
    public Person Father
    {
      get { return (_fatherID != Guid.Empty) ? Person.GetObject(_fatherID) : null; }
      set { _fatherID = (value != null) ? value.ID : Guid.Empty; }
    }

    [XmlElement]
    [ObjectBinding(Visible = false)]
    public Guid[] ChildIDs
    {
      get { return _childIDs; }
      set { _childIDs = value; }
    }

    [XmlIgnore]
    public Collection<Person> Children
    {
      get
      {
        if (_children == null)
        {
          _children = new ObservableCollection<Person>((_childIDs??Enumerable.Empty<Guid>()).Select(Person.GetObject));
          _children.CollectionChanged +=
              (sender, args) =>
              {
                _childIDs =
                    (_childIDs ?? Enumerable.Empty<Guid>())
                        .Except((args.OldItems ?? (IList)Enumerable.Empty<Person>()).Cast<Person>().Select(i => i.ID))
                        .Concat((args.NewItems ?? (IList)Enumerable.Empty<Person>()).Cast<Person>().Select(i => i.ID))
                        .ToArray();
              };
        }
        return _children;
      }
    }

    [XmlIgnore]
    public BindableXmlObject[] ChildrenAsObjects
    {
      get
      {
        return Children.ToArray();
      }
      set
      {
        Children.Clear();
        foreach (var item in value)
          Children.Add((Person)item);
      }
    }

    [XmlElement]
    [ObjectBinding(Visible = false)]
    public Guid[] JobIDs
    {
      get { return _jobIDs; }
      set { _jobIDs = value; }
    }

    [XmlIgnore]
    public Job[] Jobs
    {
      get
      {
        if (_jobIDs == null)
          return new Job[0];

        Job[] jobs = new Job[_jobIDs.Length];
        for (int i = 0; i < _jobIDs.Length; i++)
          jobs[i] = Job.GetObject(_jobIDs[i]);

        return jobs;
      }
      set
      {
        if (value != null)
        {
          ArgumentUtility.CheckNotNullOrItemsNull("value", value);
          _jobIDs = new Guid[value.Length];
          for (int i = 0; i < value.Length; i++)
            _jobIDs[i] = value[i].ID;
        }
        else
        {
          _jobIDs = new Guid[0];
        }
      }
    }

    [XmlAttribute(DataType="date")]
    [DateProperty]
    public DateTime DateOfDeath
    {
      get { return _dateOfDeath; }
      set { _dateOfDeath = value; }
    }

#if NET6_0_OR_GREATER
    [XmlIgnore]
    public DateOnly DateOfCitizenship
    {
      get { return _dateOfCitizenship; }
      set { _dateOfCitizenship = value; }
    }

    // DateOnly is not supported by the XmlSerializer so we have to add a conversion property ourselves
    [XmlAttribute(AttributeName = nameof(DateOfCitizenship))]
    [ObjectBinding(Visible = false)]
    public string DateOfCitizenshipValue
    {
      get => _dateOfCitizenship.ToString("yyyy-MM-dd");
      set => DateOfCitizenship = DateOnly.Parse(value);
    }
#else
    [XmlAttribute(DataType="date")]
    [DateProperty]
    public DateTime DateOfCitizenship
    {
      get { return _dateOfCitizenship; }
      set { _dateOfCitizenship = value; }
    }
#endif

    [XmlElement]
    public bool Deceased
    {
      get { return _deceased; }
      set { _deceased = value; }
    }

    [XmlElement]
    public string[] CV
    {
      get { return _cv; }
      set { _cv = value; }
    }

    [XmlIgnore]
    [MultiLingualName("IstVolljährig", "")]
    public bool IsOfLegalAge => (DateTime.Now - DateOfBirth).Days > 365 * 18;

    public string CVStringLiteral
    {
      get
      {
        if (_cv == null)
          return null;
        return string.Join("<br/>", _cv);
      }
    }

    public string CVString
    {
      get
      {
        if (_cv == null)
          return null;
        return string.Join("\r\n", _cv);
      }
      set
      {
        if (value == null)
          _cv = null;
        else
          _cv = value.Replace("\r", "").Split('\n');
      }
    }

    public override string DisplayName
    {
      get { return LastName + ", " + FirstName; }
    }

    public override string ToString ()
    {
      return DisplayName;
    }
  }

  public enum Gender
  {
    Male,
    Female,
    UnknownGender
  }

  public enum MarriageStatus
  {
    [MultiLingualName("Vermählt", "")]
    Married,
    Single,
    Divorced,
    Bigamist,
    Polygamist,
  }
}
