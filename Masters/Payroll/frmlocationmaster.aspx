﻿<%@ Page Title="Location master" Language="C#" MasterPageFile="~/ERPmaster.master"
    AutoEventWireup="true" Inherits="Masters_Payroll_frmlocationmaster" Codebehind="frmlocationmaster.aspx.cs" %>

<%@ Register Src="~/HRUserControls/Location.ascx" TagPrefix="uc" TagName="Location" %>
<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" runat="Server">
    <div style="margin: 0% 20% 0% 20%">
        <div style="width: 100%">
            <uc:Location ID="Locationid" runat="server" />
        </div>
    </div>
</asp:Content>
