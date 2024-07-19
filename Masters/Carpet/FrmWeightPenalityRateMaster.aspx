<%@ Page Title="Weight Penality Rate" Language="C#" AutoEventWireup="true"
    MasterPageFile="~/ERPmaster.master" Inherits="Masters_Carpet_FrmWeightPenalityRateMaster"
    EnableEventValidation="false" Codebehind="FrmWeightPenalityRateMaster.aspx.cs" %>

<%@ Register Src="~/UserControls/MasterWeightPenalityRate.ascx" TagPrefix="uc" TagName="MasterWeightPenalityRate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPH_Form" runat="server">
    <uc:MasterWeightPenalityRate ID="ColorUserControl" runat="server" />
</asp:Content>
