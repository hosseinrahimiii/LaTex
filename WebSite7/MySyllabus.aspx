<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MySyllabus.aspx.cs" Inherits="CS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body
        {
            font-family: Arial;
            font-size: 10pt;
        }
        table
        {
            border: 1px solid #ccc;
            border-collapse: collapse;
            background-color: #fff;
        }
        table th
        {
            background-color: #B8DBFD;
            color: #333;
            font-weight: bold;
        }
        table th, table td
        {
            padding: 5px;
            border: 1px solid #ccc;
        }
        table, table table td
        {
            border: 0px solid #ccc;
        }
        .text-danger {
            color: #ff0e0e;
        }
        .chkboxDisable {
            pointer-events:none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:PlaceHolder runat="server" ID="fillInsert" Visible="true">
            <div id="dvGrid" style="padding: 0px; width: 1200px">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" OnRowDataBound="OnRowDataBound"
                            DataKeyNames="ID" OnRowEditing="OnRowEditing" OnRowCancelingEdit="OnRowCancelingEdit"
                            PageSize="3" AllowPaging="true" OnPageIndexChanging="OnPaging" OnRowUpdating="OnRowUpdating"
                            OnRowDeleting="OnRowDeleting" EmptyDataText="No records has been added." Width="1350" Height="150">
                            <Columns>
                                <asp:TemplateField HeaderText="Id" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="3%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblId" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Syllabus Name" ItemStyle-Width="20%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSyllabus" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtSyllabus" runat="server" Text='<%# Eval("Name") %>' Width="90%"></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Latex" ItemStyle-Width="40%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLatex" runat="server" Text='<%# Eval("Latex").ToString().Substring(0,Math.Min(200,Eval("Latex").ToString().Length)) %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtLatex" runat="server" TextMode="MultiLine" Text='<%# Eval("Latex") %>' Width="650" Height="200"></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="All Users Access" ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" Text="Read" Checked='<%# Convert.ToBoolean(Eval("read_access_all"))%>' CssClass="chkboxDisable" />
                                        <asp:CheckBox runat="server" Text="Write" Checked='<%# Convert.ToBoolean(Eval("write_access_all"))%>' CssClass="chkboxDisable" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="checkBoxReadAll" runat="server" Text="Read" Checked='<%# Convert.ToBoolean(Eval("read_access_all"))%>' />
                                        <asp:CheckBox ID="checkBoxWriteAll" runat="server" Text="Write" Checked='<%# Convert.ToBoolean(Eval("write_access_all"))%>' />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField ButtonType="Link" ShowEditButton="true" ShowDeleteButton="true" ItemStyle-Width="8%"
                                    HeaderText="Actions" ItemStyle-HorizontalAlign="Center"/>
                                <asp:TemplateField ItemStyle-Width="30%" HeaderText="Details" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>  
                                        <asp:HyperLink ID="HyperLink1" Width="30%" NavigateUrl='<%# Eval("ID","~/SyllabusRead.aspx?syllabusId={0}") %>' 
                                            runat="server">Read Access</asp:HyperLink>
                                        <asp:HyperLink ID="HyperLink2" Width="30%" NavigateUrl='<%# Eval("ID","~/SyllabusWrite.aspx?syllabusId={0}") %>' 
                                            runat="server">Write Access</asp:HyperLink>
                                        <asp:HyperLink ID="HyperLink3" Width="30%" NavigateUrl='<%# Eval("UNIQUE_ID","~/PublicSyllabus.aspx?uniqueId={0}") %>' 
                                            runat="server">Public Url</asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <table border="1" cellpadding="0" cellspacing="0" style="border-collapse: collapse" width="1350">
                            <tr>
                                <td style="width: 23%">
                                    Syllabus Name:<br />
                                    <asp:TextBox ID="txtSyllabus" runat="server" Width="90%" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSyllabus"
                                        CssClass="text-danger" ErrorMessage="The syllabus field is required."
                                        Display="Dynamic" ValidationGroup="fillInsert" />
                                </td>
                                <td style="width: 52%">
                                    Latex:<br />
                                    <asp:TextBox ID="txtLatex" runat="server" TextMode="MultiLine" Width="95%" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLatex"
                                        CssClass="text-danger" ErrorMessage="The latex field is required."
                                        Display="Dynamic" ValidationGroup="fillInsert" />
                                </td>
                                <td style="width: 13%">
                                    <asp:CheckBox ID="checkBoxReadAll" runat="server" Text="All Users Read Access" />
                                    <br />
                                    <asp:CheckBox ID="checkBoxWriteAll" runat="server" Text="All Users Write Access" />
                                </td>
                                <td style="width: 13%" align="center">
                                    <asp:Button ID="btnAdd" runat="server" Text="Add" Width="80%" OnClick="Insert" ValidationGroup="fillInsert" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:PlaceHolder>
    </form>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.4/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.js"></script>
    <script type="text/javascript">
        $(function () {
            BlockUI("dvGrid");
            $.blockUI.defaults.css = {};
        });
        function BlockUI(elementID) {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                $("#" + elementID).block({ message: '<div align = "center">' + '<img src="images/loadingAnim.gif"/></div>',
                    css: {},
                    overlayCSS: { backgroundColor: '#000000', opacity: 0.6, border: '3px solid #63B2EB' }
                });
            });
            prm.add_endRequest(function () {
                $("#" + elementID).unblock();
            });
        };
    </script>
</body>
</html>
