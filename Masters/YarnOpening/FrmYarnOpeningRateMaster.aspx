<%@ Page Title="RATE MASTER" Language="C#" MasterPageFile="~/ERPmaster.master" AutoEventWireup="true" Inherits="Masters_YarnOpening_FrmYarnOpeningRateMaster" Codebehind="FrmYarnOpeningRateMaster.aspx.cs" %>

<%@ Register Src="~/UserControls/YarnOpeningRateMaster.ascx" TagPrefix="uc" TagName="YarnOpeningRateMaster" %>
<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" runat="Server">
    <uc:YarnOpeningRateMaster ID="YarnOpeningRateid" runat="server" />
</asp:Content>
