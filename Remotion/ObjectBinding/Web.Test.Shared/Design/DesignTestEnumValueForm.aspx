<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Page language="c#" Codebehind="DesignTestEnumValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.Design.DesignTestEnumValueForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>DesignTest: EnumValue Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><remotion:htmlheadcontents id=HtmlHeadContents runat="server"></remotion:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server"><remotion:webbutton id=PostBackButton runat="server" Text="PostBack"></remotion:webbutton>
<h1>DesignTest: EnumValue Form</h1>
<table width="100%">
  <tr>
    <td colSpan=2>Edit Mode: Drop Down List</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue1 runat="server" propertyidentifier="MarriageStatus" width="100%" datasourcecontrol="CurrentObject"></remotion:bocenumvalue></td>
    <td width="50%">width 100%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue36 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<commonstyle width="100%">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">common 100%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue37 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<listcontrolstyle width="100%">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">listcontrol 100%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue2 runat="server" propertyidentifier="MarriageStatus" width="50%" datasourcecontrol="CurrentObject"></remotion:bocenumvalue></td>
    <td width="50%">width 50%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue38 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<commonstyle width="50%">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">common 50%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue39 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<listcontrolstyle width="50%">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">listcontrol 50%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue3 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="300px"></remotion:bocenumvalue></td>
    <td width="50%">width 300px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue40 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<commonstyle width="300px">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">common 300px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue41 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<listcontrolstyle width="300px">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">listcontrol 300px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue4 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="150px"></remotion:bocenumvalue></td>
    <td width="50%">width 150px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue42 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<commonstyle width="150px">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">common 150px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue43 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<listcontrolstyle width="150px">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">listcontrol 150px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue17 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="33%">
    </remotion:bocenumvalue><remotion:bocenumvalue id=BocEnumValue18 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="33%"></remotion:bocenumvalue></td>
    <td width="50%">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Edit Mode: Radio Button List</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue53" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="MarriageStatus">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">width 100%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue54" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="MarriageStatus">
<commonstyle width="100%">
</CommonStyle>

<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">common 100%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue55" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="MarriageStatus">
<listcontrolstyle width="100%" controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">listcontrol 100%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue56" runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="MarriageStatus">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">width 50%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue5" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<commonstyle width="50%">
</CommonStyle>

<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">common 50%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue6" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<listcontrolstyle width="50%" controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">listcontrol 50%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue7" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="300px">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">width 300px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue8" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<commonstyle width="300px">
</CommonStyle>

<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">common 300px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue9" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<listcontrolstyle width="300px" controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">listcontrol 300px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue10" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="150px">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">width 150px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue11" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<commonstyle width="150px">
</CommonStyle>

<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">common 150px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue12" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<listcontrolstyle width="150px" controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">listcontrol 150px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue13" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="33%">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle>
    </remotion:bocenumvalue><remotion:bocenumvalue id="Bocenumvalue14" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="33%">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle></remotion:bocenumvalue></td>
    <td width="50%">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue57" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="MarriageStatus" readonly="True"></remotion:bocenumvalue></td>
    <td width="50%">width 100%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue44 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="100%">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td width="50%">common 100%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue45 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" readonly="True">
<listcontrolstyle>
</ListControlStyle>

<labelstyle width="100%">
</LabelStyle>
</remotion:bocenumvalue></td>
    <td width="50%">label 100%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue58" runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="MarriageStatus" readonly="True"></remotion:bocenumvalue></td>
    <td width="50%">width 50%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue46 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="50%">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td width="50%">common 50%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue47 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" readonly="True">
<listcontrolstyle>
</ListControlStyle>

<labelstyle width="50%">
</LabelStyle>
</remotion:bocenumvalue></td>
    <td width="50%">label 50%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue59" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="MarriageStatus" Width="300px" readonly="True">
<listcontrolstyle>
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td width="50%">width 300px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue48 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="300px">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td width="50%">common 300px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue49 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" readonly="True">
<listcontrolstyle>
</ListControlStyle>

<labelstyle width="300px">
</LabelStyle>
</remotion:bocenumvalue></td>
    <td width="50%">label 300px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue51 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="150px" readonly="True">
<listcontrolstyle>
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td width="50%">width 150px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue50 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="150px">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td width="50%">common 150px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue52 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" readonly="True">
<listcontrolstyle>
</ListControlStyle>

<labelstyle width="150px">
</LabelStyle>
</remotion:bocenumvalue></td>
    <td width="50%">label 150px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue60" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="MarriageStatus" Width="33%" readonly="True">
    </remotion:bocenumvalue><remotion:bocenumvalue id=BocEnumValue19 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="33%" readonly="True"></remotion:bocenumvalue></td>
    <td width="50%">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Edit Mode right</td></tr>
  <tr>
    <td width="50%" style="HEIGHT: 17px"><remotion:bocenumvalue id="Bocenumvalue61" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="MarriageStatus" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocenumvalue></td>
    <td width="50%" style="HEIGHT: 17px">100%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue62" runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="MarriageStatus" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocenumvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue63" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="MarriageStatus" Width="300px" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocenumvalue></td>
    <td width="50%">300px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue64" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="MarriageStatus" Width="150px" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocenumvalue></td>
    <td width="50%">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue22 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="33%" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocenumvalue><remotion:bocenumvalue id=BocEnumValue23 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="33%" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocenumvalue></td>
    <td width="50%">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode right</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue65" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="MarriageStatus" readonly="True" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id="Bocenumvalue66" runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="MarriageStatus" readonly="True" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue15 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="300px" readonly="True" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td width="50%">300px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue16 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="150px" readonly="True" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td width="50%">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue20 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="33%" readonly="True" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</remotion:bocenumvalue><remotion:bocenumvalue id=BocEnumValue21 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="33%" readonly="True" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td width="50%">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Edit Mode block</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue24 runat="server" propertyidentifier="MarriageStatus" width="100%" datasourcecontrol="CurrentObject" cssclass="BocEnumValue block"></remotion:bocenumvalue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue25 runat="server" propertyidentifier="MarriageStatus" width="50%" datasourcecontrol="CurrentObject" cssclass="BocEnumValue block"></remotion:bocenumvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue26 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="300px" cssclass="BocEnumValue block"></remotion:bocenumvalue></td>
    <td width="50%">300px</td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue27 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="150px" cssclass="BocEnumValue block"></remotion:bocenumvalue></td>
    <td width="50%">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%"><remotion:bocenumvalue id=BocEnumValue28 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="33%" cssclass="BocEnumValue block"></remotion:bocenumvalue><remotion:bocenumvalue id=BocEnumValue29 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" Width="33%" cssclass="BocEnumValue block"></remotion:bocenumvalue></td>
    <td width="50%">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode block</td></tr>
  <tr>
    <td width="50%"><remotion:BocEnumValue id="BocEnumValue30" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="MarriageStatus" readonly="True" cssclass="BocEnumValue block">
<commonstyle cssclass="label">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:BocEnumValue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><remotion:BocEnumValue id="BocEnumValue31" cssclass="BocEnumValue block" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="MarriageStatus" width="50%" readonly="True">
<commonstyle cssclass="label">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:BocEnumValue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><remotion:BocEnumValue id="BocEnumValue32" cssclass="BocEnumValue block" runat="server" Width="300px" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="MarriageStatus">
<commonstyle cssclass="label">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:BocEnumValue></td>
    <td width="50%">300px</td>
  </tr>
  <tr>
    <td width="50%"><remotion:BocEnumValue id="BocEnumValue33" cssclass="BocEnumValue block" runat="server" Width="150px" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="MarriageStatus">
<commonstyle cssclass="label">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:BocEnumValue></td>
    <td width="50%">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%"><remotion:BocEnumValue id="BocEnumValue34" cssclass="BocEnumValue block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="MarriageStatus">
<commonstyle cssclass="label">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle></remotion:BocEnumValue>
<remotion:BocEnumValue id="BocEnumValue35" cssclass="BocEnumValue block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="MarriageStatus">
<commonstyle cssclass="label">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle></remotion:BocEnumValue></td>
    <td width="50%">
      <p>2x 33%</p></td>
  </tr></table><remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person"></remotion:BindableObjectDataSourceControl></form>
  </body>
</html>
