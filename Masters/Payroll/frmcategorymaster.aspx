<%@ Page Title="Category master" Language="C#" MasterPageFile="~/ERPmaster.master"
    AutoEventWireup="true" Inherits="Masters_Payroll_frmcategorymaster" Codebehind="frmcategorymaster.aspx.cs" %>

<%@ Register Src="~/HRUserControls/Category.ascx" TagPrefix="uc" TagName="Category" %>
<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" runat="Server">
    <div style="margin: 0% 20% 0% 20%">
        <div style="width: 100%">
            <uc:Category ID="Categoryid" runat="server" />
        </div>
    </div>
</asp:Content>
