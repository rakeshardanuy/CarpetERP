<%@ Page Title="" Language="C#" MasterPageFile="~/ERPmaster.master" AutoEventWireup="true" Inherits="Masters_ReportForms_frmtestinvoiceexcelprint" Codebehind="frmtestinvoiceexcelprint.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" runat="Server">
    <div>
        <asp:Button ID="btnprint" Text="Print" runat="server" OnClick="btnprint_Click" />
    </div>
</asp:Content>
