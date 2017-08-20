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
<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ScrollTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.ScrollTest" %>
<table>
  <tr>
    <td>
      <%-- Test positioning with a fitting block --%>
      <div class="parent">
        <div id="outerA" class="outer">
          <div class="inner">
            <div id="blockA" class="block">
            </div>
          </div>
        </div>
        <div class="guide">
        </div>
      </div>
    </td>
    <td>
      <%-- Test positioning with a block that does not fit the container--%>
      <div class="parent">
        <div id="outerB" class="outer">
          <div class="inner">
            <div id="blockB" class="block nocolor">
              <div class="centerblock">
              </div>
            </div>
          </div>
        </div>
        <div class="guide">
        </div>
      </div>
    </td>
    <td>
      <%-- Test padding with a fitting block --%>
      <div class="parent">
        <div id="outerC" class="outer">
          <div class="inner">
            <div id="blockC" class="block">
            </div>
          </div>
        </div>
        <div class="guide">
        </div>
      </div>
    </td>
  </tr>

</table>

<style>
  .parent
  {
    position: relative;
    width: 98px;
    height: 98px;
  }

  .outer
  {
    position: absolute;
    top: 0;
    left: 0;

    width: 94px;
    height: 94px;
    overflow: hidden;

    border: solid black 1px;
  }

  .inner
  {
    position: relative;
    width: 368px;
    height: 368px;
  }

  .block
  {
    position: absolute;
    top: 94px;
    left: 94px;

    background-color: black;
  }

  .centerblock
  {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);

    width: 30px;
    height: 30px;
    
    background-color: black;
  }

  .guide 
  {
    position: absolute;
    top: 32px;
    left: 32px;

    width: 30px;
    height: 30px;

    border: solid red 1px;
  }

  .nocolor
  {
    background-color: transparent !important;
  }

  #blockA
  {
    width: 30px;
    height: 30px;
  }

  #blockB
  {
    width: 88px;
    height: 88px;
  }

  #blockC
  {
    width: 30px;
    height: 30px;

    background-color: black !important;
  }
</style>