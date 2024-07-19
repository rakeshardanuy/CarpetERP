<%@ Page Title="GST Master Rate" Language="C#" AutoEventWireup="true"
    MasterPageFile="~/ERPmaster.master" Inherits="Masters_Carpet_FrmGST" EnableEventValidation="false" Codebehind="FrmGST.aspx.cs" %>

<%@ Register Src="~/UserControls/MasterGST.ascx" TagPrefix="uc" TagName="MasterGST" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPH_Form" runat="server">
    <uc:MasterGST ID="GSTUserControl" runat="server" />
</asp:Content>
