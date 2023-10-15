using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using System.Security.Cryptography;
using System.Xml;

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
        string query = "SELECT * FROM Syllabus WHERE OWNER_USER_NAME = '" + Context.User.Identity.GetUserName() + "'";
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
        string syllabusName = txtSyllabus.Text;
        string latex = txtLatex.Text;
        bool readAccessAll = checkBoxReadAll.Checked;
        bool writeAccessAll = checkBoxWriteAll.Checked;
        txtSyllabus.Text = "";
        txtLatex.Text = "";
        checkBoxReadAll.Checked = false;
        checkBoxWriteAll.Checked = false;
        string unique_id = Utill.GenerateUniqueRandomToken(new Random().Next(0, int.MaxValue));
        string query = "INSERT INTO SYLLABUS VALUES(@NAME, @LATEX, @OWNER_USER_NAME, @READ_ACCESS_ALL, @WRITE_ACCESS_ALL, @UNIQUE_ID)";
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Parameters.AddWithValue("@NAME", syllabusName);
                cmd.Parameters.AddWithValue("@LATEX", latex);
                cmd.Parameters.AddWithValue("@OWNER_USER_NAME", Context.User.Identity.GetUserName());
                cmd.Parameters.AddWithValue("@READ_ACCESS_ALL", writeAccessAll);
                cmd.Parameters.AddWithValue("@WRITE_ACCESS_ALL", readAccessAll);
                cmd.Parameters.AddWithValue("@UNIQUE_ID", unique_id);
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
        int syllabusId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
        string syllabusName = (row.FindControl("txtSyllabus") as TextBox).Text;
        string latex = (row.FindControl("txtLatex") as TextBox).Text;
        bool readAccessAll = (row.FindControl("checkBoxReadAll") as CheckBox).Checked;
        bool writeAccessAll = (row.FindControl("checkBoxWriteAll") as CheckBox).Checked;
        if (!string.IsNullOrWhiteSpace(syllabusName) && !string.IsNullOrWhiteSpace(latex))
        {
            string query = " UPDATE SYLLABUS " +
                           " SET NAME=@NAME, LATEX=@LATEX, WRITE_ACCESS_ALL=@WRITE_ACCESS_ALL, READ_ACCESS_ALL=@READ_ACCESS_ALL " +
                           " WHERE ID=@ID";
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@ID", syllabusId);
                    cmd.Parameters.AddWithValue("@NAME", syllabusName);
                    cmd.Parameters.AddWithValue("@LATEX", latex);
                    cmd.Parameters.AddWithValue("@READ_ACCESS_ALL", readAccessAll);
                    cmd.Parameters.AddWithValue("@WRITE_ACCESS_ALL", writeAccessAll);
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
        int syllabusId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
        string query01 = "DELETE FROM SYLLABUS_WRITE WHERE SYLLABUS_ID=@SYLLABUS_ID";
        string query02 = "DELETE FROM SYLLABUS_READ WHERE SYLLABUS_ID=@SYLLABUS_ID";
        string query03 = "DELETE FROM SYLLABUS WHERE ID=@ID";
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand(query01))
            {
                cmd.Parameters.AddWithValue("@SYLLABUS_ID", syllabusId);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            using (SqlCommand cmd = new SqlCommand(query02))
            {
                cmd.Parameters.AddWithValue("@SYLLABUS_ID", syllabusId);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            using (SqlCommand cmd = new SqlCommand(query03))
            {
                cmd.Parameters.AddWithValue("@ID", syllabusId);
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
            (e.Row.Cells[4].Controls[2] as LinkButton).Attributes["onclick"] = "return confirm('Do you want to delete this row?');";
        }
    }

    protected void OnPaging(object sender, GridViewPageEventArgs e)
    {
        GridView1.PageIndex = e.NewPageIndex;
        this.BindGrid();
    }

}