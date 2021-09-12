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
<%@ Page language="c#" Codebehind="DesignTestTextValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.Design.DesignTestTextValueForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>DesignTest: TextValue Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><remotion:htmlheadcontents id=HtmlHeadContents runat="server"></remotion:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server"><remotion:webbutton id=PostBackButton runat="server" Text="PostBack"></remotion:webbutton>
<h1>DesignTest: TextValue Form</h1>
<table width="100%">
  <tr>
    <td colSpan=2>Edit Mode</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue1 runat="server" propertyidentifier="FirstName" width="100%" datasourcecontrol="CurrentObject"></remotion:boctextvalue></td>
    <td width="50%">width 100%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue36 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<commonstyle width="100%">
</CommonStyle>

<textboxstyle>
</TextBoxStyle></remotion:boctextvalue></td>
    <td width="50%">common 100%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue37 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<textboxstyle width="100%">
</TextBoxStyle></remotion:boctextvalue></td>
    <td width="50%">textbox 100%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue2 runat="server" propertyidentifier="FirstName" width="50%" datasourcecontrol="CurrentObject"></remotion:boctextvalue></td>
    <td width="50%">width 50%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue38 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<commonstyle width="50%">
</CommonStyle>

<textboxstyle>
</TextBoxStyle></remotion:boctextvalue></td>
    <td width="50%">common 50%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue39 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<textboxstyle width="50%">
</TextBoxStyle></remotion:boctextvalue></td>
    <td width="50%">textbox 50%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue3 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="300px"></remotion:boctextvalue></td>
    <td width="50%">width 300px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue40 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<commonstyle width="300px">
</CommonStyle>

<textboxstyle>
</TextBoxStyle></remotion:boctextvalue></td>
    <td width="50%">common 300px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue41 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<textboxstyle width="300px">
</TextBoxStyle></remotion:boctextvalue></td>
    <td width="50%">textbox 300px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue4 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="150px"></remotion:boctextvalue></td>
    <td width="50%">width 150px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue42 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<commonstyle width="150px">
</CommonStyle>

<textboxstyle>
</TextBoxStyle></remotion:boctextvalue></td>
    <td width="50%">common 150px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue43 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<textboxstyle width="150px">
</TextBoxStyle></remotion:boctextvalue></td>
    <td width="50%">textbox 150px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue17 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%">
    </remotion:boctextvalue><remotion:boctextvalue id=BocTextValue18 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%"></remotion:boctextvalue></td>
    <td width="50%">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue5 runat="server" propertyidentifier="FirstName" width="100%" datasourcecontrol="CurrentObject" readonly="True"></remotion:boctextvalue></td>
    <td width="50%">width 100%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue44 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="100%">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:boctextvalue></td>
    <td width="50%">common 100%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue45 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<textboxstyle>
</TextBoxStyle>

<labelstyle width="100%">
</LabelStyle>
</remotion:boctextvalue></td>
    <td width="50%">label 100%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue6 runat="server" propertyidentifier="FirstName" width="50%" datasourcecontrol="CurrentObject" readonly="True"></remotion:boctextvalue></td>
    <td width="50%">width 50%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue46 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="50%">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:boctextvalue></td>
    <td width="50%">common 50%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue47 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<textboxstyle>
</TextBoxStyle>

<labelstyle width="50%">
</LabelStyle>
</remotion:boctextvalue></td>
    <td width="50%">label 50%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue7 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="300px" readonly="True">
<textboxstyle>
</TextBoxStyle>
</remotion:boctextvalue></td>
    <td width="50%">width 300px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue48 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="300px">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:boctextvalue></td>
    <td width="50%">common 300px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue49 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<textboxstyle>
</TextBoxStyle>

<labelstyle width="300px">
</LabelStyle>
</remotion:boctextvalue></td>
    <td width="50%">label 300px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue51 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="150px" readonly="True">
<textboxstyle>
</TextBoxStyle>
</remotion:boctextvalue></td>
    <td width="50%">width 150px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue50 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="150px">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:boctextvalue></td>
    <td width="50%">common 150px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=Boctextvalue52 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<textboxstyle>
</TextBoxStyle>

<labelstyle width="150px">
</LabelStyle>
</remotion:boctextvalue></td>
    <td width="50%">label 150px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue8 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" readonly="True">
    </remotion:boctextvalue><remotion:boctextvalue id=BocTextValue19 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" readonly="True"></remotion:boctextvalue></td>
    <td width="50%">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Edit Mode right</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue9 runat="server" propertyidentifier="FirstName" width="100%" datasourcecontrol="CurrentObject" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:boctextvalue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue10 runat="server" propertyidentifier="FirstName" width="50%" datasourcecontrol="CurrentObject" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:boctextvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue11 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="300px" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:boctextvalue></td>
    <td width="50%">300px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue12 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="150px" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:boctextvalue></td>
    <td width="50%">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue22 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:boctextvalue><remotion:boctextvalue id=BocTextValue23 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:boctextvalue></td>
    <td width="50%">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode right</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue13 runat="server" propertyidentifier="FirstName" width="100%" datasourcecontrol="CurrentObject" readonly="True" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:boctextvalue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue14 runat="server" propertyidentifier="FirstName" width="50%" datasourcecontrol="CurrentObject" readonly="True" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:boctextvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue15 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="300px" readonly="True" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:boctextvalue></td>
    <td width="50%">300px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue16 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="150px" readonly="True" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:boctextvalue></td>
    <td width="50%">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue20 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" readonly="True" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:boctextvalue><remotion:boctextvalue id=BocTextValue21 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" readonly="True" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:boctextvalue></td>
    <td width="50%">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Edit Mode block</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue24 runat="server" propertyidentifier="FirstName" width="100%" datasourcecontrol="CurrentObject" cssclass="bocTextValue block"></remotion:boctextvalue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue25 runat="server" propertyidentifier="FirstName" width="50%" datasourcecontrol="CurrentObject" cssclass="bocTextValue block"></remotion:boctextvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue26 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="300px" cssclass="bocTextValue block"></remotion:boctextvalue></td>
    <td width="50%">300px</td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue27 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="150px" cssclass="bocTextValue block"></remotion:boctextvalue></td>
    <td width="50%">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%"><remotion:boctextvalue id=BocTextValue28 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocTextValue block"></remotion:boctextvalue><remotion:boctextvalue id=BocTextValue29 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocTextValue block"></remotion:boctextvalue></td>
    <td width="50%">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode block</td></tr>
  <tr>
    <td width="50%"><remotion:bocTextvalue id="BocTextValue30" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="FirstName" readonly="True" cssclass="bocTextValue block">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocTextvalue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><remotion:bocTextvalue id="BocTextValue31" cssclass="bocTextValue block" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="FirstName" width="50%" readonly="True">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocTextvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><remotion:bocTextvalue id="BocTextValue32" cssclass="bocTextValue block" runat="server" Width="300px" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="FirstName">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocTextvalue></td>
    <td width="50%">300px</td>
  </tr>
  <tr>
    <td width="50%"><remotion:bocTextvalue id="BocTextValue33" cssclass="bocTextValue block" runat="server" Width="150px" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="FirstName">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocTextvalue></td>
    <td width="50%">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%"><remotion:bocTextvalue id="BocTextValue34" cssclass="bocTextValue block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="FirstName">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle></remotion:bocTextvalue>
<remotion:bocTextvalue id="BocTextValue35" cssclass="bocTextValue block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="FirstName">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle></remotion:bocTextvalue></td>
    <td width="50%">
      <p>2x 33%</p></td>
  </tr></table><remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" /></form>
  </body>
</html>
