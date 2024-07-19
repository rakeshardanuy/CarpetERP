<%@ Page Title="RATE MASTER" Language="C#" MasterPageFile="~/ERPmaster.master" AutoEventWireup="true" Inherits="Masters_Motteling_frmMottelingratemaster" Codebehind="frmMottelingratemaster.aspx.cs" %>

<%@ Register Src="~/UserControls/Motellingratemaster.ascx" TagPrefix="uc" TagName="Motellingratemaster" %>
<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" runat="Server">
    <uc:Motellingratemaster ID="MotellingRateid" runat="server" />
</asp:Content>
