﻿<%@ Page Title="NEW LEAVE TYPE" Language="C#" MasterPageFile="~/ERPmaster.master"
    AutoEventWireup="true" Inherits="Masters_Payroll_frmleavetype" Codebehind="frmleavetype.aspx.cs" %>

<%@ Register Src="~/HRUserControls/Leavetype.ascx" TagPrefix="uc" TagName="frmleavetype" %>
<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" runat="Server">
    <div style="margin: 0% 20% 0% 20%">
        <div style="width: 100%">
            <uc:frmleavetype ID="leavetypeid" runat="server" />
        </div>
    </div>
</asp:Content>
