using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;
using Microsoft.AspNet.Identity;

public partial class CS : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.User.Identity.IsAuthenticated == false)
            Response.Redirect("~/Account/Login.aspx");

        if (!this.IsPostBack)
        {
            this.BindGrid();
        }
    }

    private void BindGrid()
    {
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string query = "SELECT * FROM Book WHERE OWNER_USER_NAME = '" + Context.User.Identity.GetUserName() + "'";
        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlDataAdapter sda = new SqlDataAdapter(query, con))
            {
                using (DataTable dt = new DataTable())
                {
                    sda.Fill(dt);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }
        }
    }

    protected void Insert(object sender, EventArgs e)
    {
        string bookName = txtBook.Text;
        txtBook.Text = "";
        string query = "INSERT INTO BOOK VALUES(@NAME, @OWNER_USER_NAME)";
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Parameters.AddWithValue("@NAME", bookName);
                cmd.Parameters.AddWithValue("@OWNER_USER_NAME", Context.User.Identity.GetUserName());
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        this.BindGrid();
    }

    protected void OnRowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView1.EditIndex = e.NewEditIndex;
        this.BindGrid();
    }

    protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridViewRow row = GridView1.Rows[e.RowIndex];
        int bookId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
        string bookName = (row.FindControl("txtBook") as TextBox).Text;
        if (!string.IsNullOrWhiteSpace(bookName))
        {
            string query = "UPDATE BOOK SET NAME=@NAME WHERE ID=@ID";
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@ID", bookId);
                    cmd.Parameters.AddWithValue("@NAME", bookName);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
        
        GridView1.EditIndex = -1;
        this.BindGrid();
    }

    protected void OnRowCancelingEdit(object sender, EventArgs e)
    {
        GridView1.EditIndex = -1;
        this.BindGrid();
    }

    protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int bookId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
        string query = "DELETE FROM BOOK WHERE ID=@ID";
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Parameters.AddWithValue("@ID", bookId);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        this.BindGrid();
    }

    protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowIndex != GridView1.EditIndex)
        {
            (e.Row.Cells[1].Controls[2] as LinkButton).Attributes["onclick"] = "return confirm('Do you want to delete this row?');";
        }
    }

    protected void OnPaging(object sender, GridViewPageEventArgs e)
    {
        GridView1.PageIndex = e.NewPageIndex;
        this.BindGrid();
    }

    protected void PrintBook(object sender, EventArgs e)
    {
        Button btn = sender as Button;
        int bookId = Convert.ToInt32(btn.CommandArgument);
        string query = " SELECT S.LATEX " +
                       " FROM BOOK_SYLLABUS BS " +
                       " LEFT JOIN SYLLABUS S ON BS.SYLLABUS_ID = S.ID " +
                       " WHERE BS.BOOK_ID = " + bookId +
                       " ORDER BY BS.POSITION ";
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        List<String> columnData = new List<String>();

        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Connection = con;
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        columnData.Add(reader.GetString(0));
                }
                con.Close();
            }
        }

        string latexCode = "\\documentclass{article}\r\n\\usepackage{pdfpages}\r\n\\begin{document}\n";

        for (int i = 0; i < columnData.Count; i++) {
            // Save LaTeX code to .tex file
            string texFilePath = Server.MapPath("~/PDFs/chapter" + i + ".tex");
            File.WriteAllText(texFilePath, columnData[i]);
            // Compile .tex file to PDF
            string outputFilePath = Server.MapPath("~/PDFs/chapter" + i + ".pdf");
            CompileLatexToPdf(texFilePath, outputFilePath);

            latexCode += "\\includepdf[pages=-]{chapter" + i + "}\n";
        }

        latexCode += "\\end{document}";

        // Save LaTeX code to .tex file
        string texBookFilePath = Server.MapPath("~/PDFs/book.tex");
        File.WriteAllText(texBookFilePath, latexCode);
        // Compile .tex file to PDF
        string outputBookFilePath = Server.MapPath("~/PDFs/book.pdf");
        CompileLatexToPdf(texBookFilePath, outputBookFilePath);

        // Provide download link to the user
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("Content-Disposition", "attachment;filename=output.pdf");
        Response.TransmitFile(outputBookFilePath);
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