<%@ Page Title="Weaver Raw SubItem Rate" Language="C#" AutoEventWireup="true"
    MasterPageFile="~/ERPmaster.master" Inherits="Masters_RawMaterial_WeaverRawSubItemRate"
    EnableEventValidation="false" Codebehind="WeaverRawSubItemRate.aspx.cs" %>

<%@ Register Src="~/UserControls/MasterWeaverRawSubItemRate.ascx" TagPrefix="uc"
    TagName="MasterWeaverRawSubItemRate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPH_Form" runat="server">
    <uc:MasterWeaverRawSubItemRate ID="ColorUserControl" runat="server" />
</asp:Content>
