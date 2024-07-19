<%@ Page Language="C#" AutoEventWireup="true" Inherits="Masters_Campany_Design"
    MasterPageFile="~/ERPmaster.master" EnableEventValidation="false" Codebehind="Design.aspx.cs" %>

<%@ Register Src="~/UserControls/MasterDesign.ascx" TagPrefix="uc" TagName="MasterDesign" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPH_Form" runat="server">
    <uc:MasterDesign ID="usercontrol" runat="server" />
</asp:Content>
