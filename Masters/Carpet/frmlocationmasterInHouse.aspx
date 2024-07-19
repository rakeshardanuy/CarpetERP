<%@ Page Title="Location master InHouse" Language="C#" MasterPageFile="~/ERPmaster.master"
    AutoEventWireup="true" Inherits="Masters_Carpet_frmlocationmasterInHouse" Codebehind="frmlocationmasterInHouse.aspx.cs" %>

<%@ Register Src="~/UserControls/LocationInhouse.ascx" TagPrefix="uc" TagName="Locationinhouse" %>
<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" runat="Server">
    <div style="margin: 0% 20% 0% 20%">
        <div style="width: 100%">
            <uc:Locationinhouse ID="Locationid" runat="server" />
        </div>
    </div>
</asp:Content>
