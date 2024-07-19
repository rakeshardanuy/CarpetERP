<%@ Page Title="Color" Language="C#" AutoEventWireup="true"
    MasterPageFile="~/ERPmaster.master" Inherits="Masters_Carpet_Color" EnableEventValidation="false" Codebehind="Color.aspx.cs" %>

<%@ Register Src="~/UserControls/MasterColor.ascx" TagPrefix="uc" TagName="MasterColor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPH_Form" runat="server">
    <uc:MasterColor ID="ColorUserControl" runat="server" />
</asp:Content>
