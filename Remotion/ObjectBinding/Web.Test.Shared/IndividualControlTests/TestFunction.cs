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
using System.Threading;
using Remotion.ObjectBinding.Sample;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.ObjectBinding.Web.Test.Shared.IndividualControlTests
{

[Serializable]
public class TestFunction: WxeFunction
{
  private Person _person;

  public TestFunction ()
    : base(new NoneTransactionMode())
  {

  }

  [WxeParameter(1, false, WxeParameterDirection.In)]
  public string UserControl
  {
    get
    {
      return (string)Variables["UserControl"];
    }
    set
    {
      ArgumentUtility.CheckNotNullOrEmpty("UserControl", value);
      Variables["UserControl"] = value;
    }
  }

  [WxeParameter(2, false, WxeParameterDirection.In)]
  public int? Delay
  {
    get
    {
      return (int?)Variables["Delay"];
    }
    set
    {
      Variables["Delay"] = value;
    }
  }

  public Person Person
  {
    get
    {
      return _person;
    }
  }

  public override void Execute (WxeContext context)
  {
    var delay = Delay;
    if (delay.HasValue)
      Thread.Sleep(delay.Value);

    base.Execute(context);
  }

  // steps
  private void Step1 ()
  {
    if (string.IsNullOrEmpty(UserControl))
      UserControl = "BocBooleanValueUserControl.ascx";

    ExceptionHandler.AppendCatchExceptionTypes(typeof(WxeUserCancelException));
  }

  private void Step2 ()
  {
    XmlReflectionBusinessObjectStorageProvider.Current.Reset();

    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject(personID);
    Person partner;
    if (person == null)
    {
      person = Person.CreateObject(personID);
      person.FirstName = "Hugo";
      person.LastName = "Meier";
      person.DateOfBirth = new DateTime(1959, 4, 15);
      person.Height = 179;
      person.Income = 2000;

      partner = person.Partner = Person.CreateObject();
      partner.FirstName = "Sepp";
      partner.LastName = "Forcher";
    }
    else
    {
      partner = person.Partner;
    }

    Job[] jobs = new Job[2];

    jobs[0] = Job.CreateObject(Guid.NewGuid());
    jobs[0].Title = "Programmer";
    jobs[0].StartDate = new DateTime(2000, 1, 1);
    jobs[0].EndDate = new DateTime(2004, 12, 31);

    jobs[1] = Job.CreateObject(Guid.NewGuid());
    jobs[1].Title = "CEO";
    jobs[1].StartDate = new DateTime(2005, 1, 1);

    if (person.Children.Count == 0)
    {
        var child0 = Person.CreateObject(Guid.NewGuid());
        child0.FirstName = "Jack";
        child0.LastName = "Doe";
        child0.DateOfBirth = new DateTime(1990, 4, 15);
        child0.Height = 160;
        child0.MarriageStatus = MarriageStatus.Single;
        child0.Jobs = jobs;

        var child1 = Person.CreateObject(Guid.NewGuid());
        child1.FirstName = "Max";
        child1.LastName = "Doe";
        child1.DateOfBirth = new DateTime(1991, 4, 15);
        child1.Height = 155;
        child1.MarriageStatus = MarriageStatus.Single;

        person.Children.Add(child0);
        person.Children.Add(child1);
    }

    if (person.Jobs.Length == 0)
    {
      person.Jobs = jobs;
    }

    _person = person;
  }

  private WxeStep Step3 = new WxePageStep("IndividualControlTests/Form.aspx");

  private void Step4 ()
  {
    _person.SaveObject();
    if (_person.Children != null)
    {
      foreach (Person child in _person.Children)
        child.SaveObject();
    }
    if (_person.Jobs != null)
    {
      foreach (Job job in _person.Jobs)
        job.SaveObject();
    }
    XmlReflectionBusinessObjectStorageProvider.Current.Reset();
  }
}

}
