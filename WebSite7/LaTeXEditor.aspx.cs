using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class LaTeXEditor : System.Web.UI.Page
{
    protected void btnConvert_Click(object sender, EventArgs e)
    {
        string latexCode = txtLatex.Value;

        // Save LaTeX code to .tex file
        string texFilePath = Server.MapPath("~/PDFs/input.tex");
        File.WriteAllText(texFilePath, latexCode);

        // Compile .tex file to PDF
        string outputFilePath = Server.MapPath("~/PDFs/input.pdf");
        CompileLatexToPdf(texFilePath, outputFilePath);

        // Provide download link to the user
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("Content-Disposition", "attachment;filename=output.pdf");
        Response.TransmitFile(outputFilePath);
        Response.End();
    }
    private void CompileLatexToPdf(string inputFilePath, string outputFilePath)
    {
        // Create a new process for running pdflatex command
        Process process = new Process();
        process.StartInfo.FileName = "\"C:\\Users\\Hossein\\AppData\\Local\\Programs\\MiKTeX\\miktex\\bin\\x64\\pdflatex.exe\"";
        //process.StartInfo.FileName = @"PATH";
        process.StartInfo.Arguments = $"\"{inputFilePath}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;

        // Set working directory to the folder containing the .tex file
        process.StartInfo.WorkingDirectory = Path.GetDirectoryName(inputFilePath);

        // Start the process
        process.Start();

        // Wait for the process to exit
        process.WaitForExit();

        // Check if the PDF file was generated successfully
        if (File.Exists(outputFilePath))
        {
            // Move the generated PDF file to the desired output path
            File.Move(Path.ChangeExtension(inputFilePath, ".pdf"), outputFilePath);
        }
        else
        {
            // There was an error generating the PDF file, read the error output
            string errorMessage = process.StandardError.ReadToEnd();
            throw new Exception("Error generating PDF: " + errorMessage);
        }
    }

}