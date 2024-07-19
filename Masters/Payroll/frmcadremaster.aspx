<%@ Page Title="Cadre master" Language="C#" MasterPageFile="~/ERPmaster.master"
    AutoEventWireup="true" Inherits="Masters_Payroll_frmcadremaster" Codebehind="frmcadremaster.aspx.cs" %>

<%@ Register Src="~/HRUserControls/Cadre.ascx" TagPrefix="uc" TagName="Cadre" %>

<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" runat="Server">
    <div style="margin: 0% 20% 0% 20%">
        <div style="width: 100%">
            <uc:Cadre ID="Cadreid" runat="server" />
        </div>
    </div>
</asp:Content>
