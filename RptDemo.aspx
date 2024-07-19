<%@ Page Title="" Language="C#" MasterPageFile="~/ERPmaster.master" AutoEventWireup="true" Inherits="RptDemo" Codebehind="RptDemo.aspx.cs" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" Runat="Server">
    <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
        AutoDataBind="true" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="CPH_SideBar" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="CPH_Footer" Runat="Server">
</asp:Content>

