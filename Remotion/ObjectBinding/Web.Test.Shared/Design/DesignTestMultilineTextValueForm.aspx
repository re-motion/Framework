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
<%@ Page language="c#" Codebehind="DesignTestMultilineTextValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.Design.DesignTestMultilineTextValueForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>DesignTest: MultilineTextValue Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><remotion:htmlheadcontents id=HtmlHeadContents runat="server"></remotion:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server"><remotion:webbutton id=PostBackButton runat="server" Text="PostBack"></remotion:webbutton>
<h1>DesignTest: MultilineTextValue Form</h1>
<table width="100%">
  <tr>
    <td colSpan=2>Edit Mode</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue1 runat="server" propertyidentifier="CV" width="100%" datasourcecontrol="CurrentObject">
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">width 100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue36 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject">
<commonstyle width="100%">
</CommonStyle>

<textboxstyle>
</TextBoxStyle></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">common 100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue37 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject">
<textboxstyle width="100%">
</TextBoxStyle></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">textbox 100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue2 runat="server" propertyidentifier="CV" width="50%" datasourcecontrol="CurrentObject"></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">width 50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue38 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject">
<commonstyle width="50%">
</CommonStyle>

<textboxstyle>
</TextBoxStyle></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">common 50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue39 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject">
<textboxstyle width="50%">
</TextBoxStyle></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">textbox 50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue3 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="300px"></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">width 300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue40 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject">
<commonstyle width="300px">
</CommonStyle>

<textboxstyle>
</TextBoxStyle></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">common 300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue41 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject">
<textboxstyle width="300px">
</TextBoxStyle></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">textbox 300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue4 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="150px"></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">width 150px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue42 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject">
<commonstyle width="150px">
</CommonStyle>

<textboxstyle>
</TextBoxStyle></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">common 150px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue43 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject">
<textboxstyle width="150px">
</TextBoxStyle></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">textbox 150px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue17 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="33%">
    </remotion:bocmultilinetextvalue><remotion:bocmultilinetextvalue id=BocMultilineTextValue18 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="33%"></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue5 runat="server" propertyidentifier="CV" width="100%" datasourcecontrol="CurrentObject" readonly="True"></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">width 100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue44 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="100%">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">common 100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue45 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" readonly="True">
<textboxstyle>
</TextBoxStyle>

<labelstyle width="100%">
</LabelStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">label 100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue6 runat="server" propertyidentifier="CV" width="50%" datasourcecontrol="CurrentObject" readonly="True"></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">width 50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue46 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="50%">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">common 50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue47 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" readonly="True">
<textboxstyle>
</TextBoxStyle>

<labelstyle width="50%">
</LabelStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">label 50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue7 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="300px" readonly="True">
<textboxstyle>
</TextBoxStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">width 300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue48 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="300px">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">common 300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue49 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" readonly="True">
<textboxstyle>
</TextBoxStyle>

<labelstyle width="300px">
</LabelStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">label 300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue51 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="150px" readonly="True">
<textboxstyle>
</TextBoxStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">width 150px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue50 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="150px">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">common 150px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue52 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" readonly="True">
<textboxstyle>
</TextBoxStyle>

<labelstyle width="150px">
</LabelStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">label 150px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue8 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="33%" readonly="True">
    </remotion:bocmultilinetextvalue><remotion:bocmultilinetextvalue id=BocMultilineTextValue19 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="33%" readonly="True"></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Edit Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue9 runat="server" propertyidentifier="CV" width="100%" datasourcecontrol="CurrentObject" cssclass="BocMultilineTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue10 runat="server" propertyidentifier="CV" width="50%" datasourcecontrol="CurrentObject" cssclass="BocMultilineTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue11 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="300px" cssclass="BocMultilineTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue12 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="150px" cssclass="BocMultilineTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue22 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="33%" cssclass="BocMultilineTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocmultilinetextvalue><remotion:bocmultilinetextvalue id=BocMultilineTextValue23 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="33%" cssclass="BocMultilineTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue13 runat="server" propertyidentifier="CV" width="100%" datasourcecontrol="CurrentObject" readonly="True" cssclass="BocMultilineTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue14 runat="server" propertyidentifier="CV" width="50%" datasourcecontrol="CurrentObject" readonly="True" cssclass="BocMultilineTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue15 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="300px" readonly="True" cssclass="BocMultilineTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue16 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="150px" readonly="True" cssclass="BocMultilineTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue20 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="33%" readonly="True" cssclass="BocMultilineTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:bocmultilinetextvalue><remotion:bocmultilinetextvalue id=BocMultilineTextValue21 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="33%" readonly="True" cssclass="BocMultilineTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Edit Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue24 runat="server" propertyidentifier="CV" width="100%" datasourcecontrol="CurrentObject" cssclass="BocMultilineTextValue block"></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue25 runat="server" propertyidentifier="CV" width="50%" datasourcecontrol="CurrentObject" cssclass="BocMultilineTextValue block"></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue26 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="300px" cssclass="BocMultilineTextValue block"></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue27 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="150px" cssclass="BocMultilineTextValue block"></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocmultilinetextvalue id=BocMultilineTextValue28 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="33%" cssclass="BocMultilineTextValue block"></remotion:bocmultilinetextvalue><remotion:bocmultilinetextvalue id=BocMultilineTextValue29 runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" Width="33%" cssclass="BocMultilineTextValue block"></remotion:bocmultilinetextvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:BocMultilineTextValue id="BocMultilineTextValue30" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="CV" readonly="True" cssclass="BocMultilineTextValue block">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:BocMultilineTextValue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:BocMultilineTextValue id="BocMultilineTextValue31" cssclass="BocMultilineTextValue block" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="CV" width="50%" readonly="True">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:BocMultilineTextValue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:BocMultilineTextValue id="BocMultilineTextValue32" cssclass="BocMultilineTextValue block" runat="server" Width="300px" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="CV">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:BocMultilineTextValue></td>
    <td width="50%" class="cell">300px</td>
  </tr>
  <tr>
    <td width="50%" class="cell"><remotion:BocMultilineTextValue id="BocMultilineTextValue33" cssclass="BocMultilineTextValue block" runat="server" Width="150px" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="CV">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</remotion:BocMultilineTextValue></td>
    <td width="50%" class="cell">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%" class="cell"><remotion:BocMultilineTextValue id="BocMultilineTextValue34" cssclass="BocMultilineTextValue block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="CV">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle></remotion:BocMultilineTextValue>
<remotion:BocMultilineTextValue id="BocMultilineTextValue35" cssclass="BocMultilineTextValue block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="CV">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle></remotion:BocMultilineTextValue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td>
  </tr></table><remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person"></remotion:BindableObjectDataSourceControl></form>
  </body>
</html>
