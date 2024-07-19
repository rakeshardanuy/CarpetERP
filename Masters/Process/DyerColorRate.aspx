<%@ Page Title="Dyer Color Rate" Language="C#" AutoEventWireup="true"
    MasterPageFile="~/ERPmaster.master" Inherits="Masters_Process_DyerColorRate"
    EnableEventValidation="false" Codebehind="DyerColorRate.aspx.cs" %>

<%@ Register Src="~/UserControls/MasterDyerColorRate.ascx" TagPrefix="uc" TagName="MasterDyerColorRate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPH_Form" runat="server">
    <uc:MasterDyerColorRate ID="ColorUserControl" runat="server" />
</asp:Content>
