<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LaTeXEditor.aspx.cs" Inherits="LaTeXEditor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.2/MathJax.js?config=TeX-MML-AM_CHTML"></script>
    <title>LaTeX Editor</title>
</head>
<body>
    <form id="form1" runat="server">
         <div>
             <h1>LaTeX to PDF Converter</h1>
             <textarea runat="server" id="txtLatex" rows="10" cols="80" placeholder="Enter LaTeX code"></textarea>
                <br />
             <asp:Button ID="btnConvert" runat="server" Text="Convert to PDF" OnClick="btnConvert_Click" />
         </div>
    </form>
</body>
</html>
