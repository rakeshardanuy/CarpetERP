﻿<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="Masters_Carpet_AddShape" EnableEventValidation="false" Codebehind="AddPhotoRefImage.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="~/App_Themes/Default/Style.css" />
    <script src="../../Scripts/JScript.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script type="text/javascript"></script>
    <style type="text/css">
        #newPreview
        {
            filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=scale);
        }
        #DivReferenceImage
        {
            filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=scale);
        }
    </style>
    <script language="javascript" type="text/javascript">
        function PreviewImg(imgFile) {
            var newPreview = document.getElementById("newPreview");
            document.getElementById("newPreview").value = "";
            newPreview.filters.item("DXImageTransform.Microsoft.AlphaImageLoader").src = imgFile.value;
            newPreview.style.width = "111px";
            newPreview.style.height = "66px";
            var control = document.getElementById("newPreview1");
            control.style.visibility = "hidden";
        }
        function PreviewReferenceImage(imgFile) {
            var newPreviewRef = document.getElementById("DivReferenceImage");
            document.getElementById("DivReferenceImage").value = "";
            newPreviewRef.filters.item("DXImageTransform.Microsoft.AlphaImageLoader").src = imgFile.value;
            newPreviewRef.style.width = "111px";
            newPreviewRef.style.height = "66px";
            var controlRef = document.getElementById("ImageReferenceImage");
            controlRef.style.visibility = "hidden";
        }
        function CloseForm() {

            var objParent = window.opener;
            if (objParent != null) {
                if (window.opener.document.getElementById('CPH_Form_refreshPhotoRefImage')) {
                    window.opener.document.getElementById('CPH_Form_refreshPhotoRefImage').click();
                    self.close();
                }
                else if (window.opener.document.getElementById('refreshPhotoRefImage')) {
                    window.opener.document.getElementById('refreshPhotoRefImage').click();
                    self.close();
                }

            }
            else {
                window.location.href = "../../main.aspx";
            }
        }
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <table>
            <tr>
                <td>
                    Photo
                </td>
                <td colspan="2">
                    <asp:Label ID="lblMessage" ForeColor="White" runat="server" Text=""></asp:Label>
                    <div id="newPreview" runat="server">
                        <asp:Image ID="newPreview1" runat="server" Height="66px" Width="111px" />
                    </div>
                </td>
                <td colspan="2">
                    <asp:FileUpload ID="PhotoImage" onchange="PreviewImg(this)" ViewStateMode="Enabled"
                        runat="server" />
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Only .gif,.jpeg or.jpg files are allowed!"
                        ValidationExpression="^.*\.(jpg|JPG|gif|GIF|jpeg|JPEG|BMP|bmp)$" ControlToValidate="PhotoImage"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr id="trReferenceImage" runat="server">
                <td>
                    Ref Image
                </td>
                <td colspan="2">
                    <div id="DivReferenceImage" runat="server">
                        <asp:Image ID="ImageReferenceImage" runat="server" Height="66px" Width="111px" />
                    </div>
                </td>
                <td colspan="2">
                    <asp:FileUpload ID="FileReferenceImage" onchange="PreviewReferenceImage(this)" runat="server"
                        ViewStateMode="Enabled" />
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="Only .gif,.jpeg or.jpg files are allowed!"
                        ValidationExpression="^.*\.(jpg|JPG|gif|GIF|jpeg|JPEG|BMP|bmp)$" ControlToValidate="FileReferenceImage"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td align="right">
                    <asp:Button ID="BTNSAVE" runat="server" Text="SAVE" OnClick="BTNSAVE_Click" CssClass="buttonnorm" />
                </td>
                <td align="left">
                    <asp:Button ID="BtnClose" runat="server" Text="Close" OnClientClick="return CloseForm()"
                        CssClass="buttonnorm" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
