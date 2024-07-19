<%@ Page Title="warping rate master" Language="C#" MasterPageFile="~/ERPmaster.master"
    AutoEventWireup="true" Inherits="Masters_WARP_frmwarpingratemaster" Codebehind="frmwarpingratemaster.aspx.cs" %>

<%@ Register Src="~/UserControls/Warpingratemaster.ascx" TagPrefix="uc" TagName="Warpingratemaster" %>
<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" runat="Server">
    <uc:Warpingratemaster ID="Warpingrateid" runat="server" />
</asp:Content>
