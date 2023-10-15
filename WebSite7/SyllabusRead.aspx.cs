using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

public partial class CS : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.User.Identity.IsAuthenticated == false)
            Response.Redirect("~/Account/Login.aspx");

        if (!this.IsPostBack)
        {
            string syllabusId = Request.QueryString["syllabusId"];
            this.BindGrid(syllabusId);
        }
    }

    private void BindGrid(string syllabusId)
    {
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string query = " SELECT ID, USER_NAME " +
                       " FROM SYLLABUS_READ " +
                       " WHERE SYLLABUS_ID = " + syllabusId;
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
        string syllabusId = Request.QueryString["syllabusId"];

        string userName = txtUserName.Text;
        txtUserName.Text = "";

        string query = "INSERT INTO SYLLABUS_READ VALUES(@SYLLABUS_ID, @USER_NAME)";
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Parameters.AddWithValue("@SYLLABUS_ID", syllabusId);
                cmd.Parameters.AddWithValue("@USER_NAME", userName);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        this.BindGrid(syllabusId);
    }

    protected void OnRowEditing(object sender, GridViewEditEventArgs e)
    {
        string syllabusId = Request.QueryString["syllabusId"];

        GridView1.EditIndex = e.NewEditIndex;
        this.BindGrid(syllabusId);
    }

    protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        string syllabusId = Request.QueryString["syllabusId"];

        GridViewRow row = GridView1.Rows[e.RowIndex];
        int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
        string userName = (row.FindControl("txtUserName") as TextBox).Text;
        if (!string.IsNullOrWhiteSpace(userName))
        {
            string query = "UPDATE SYLLABUS_READ SET USER_NAME=@USER_NAME WHERE ID=@ID";
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@USER_NAME", userName);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
        
        GridView1.EditIndex = -1;
        this.BindGrid(syllabusId);
    }

    protected void OnRowCancelingEdit(object sender, EventArgs e)
    {
        string syllabusId = Request.QueryString["syllabusId"];

        GridView1.EditIndex = -1;
        this.BindGrid(syllabusId);
    }

    protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        string syllabusId = Request.QueryString["syllabusId"];

        int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
        string query = "DELETE FROM SYLLABUS_READ WHERE ID=@ID";
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        this.BindGrid(syllabusId);
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
        string syllabusId = Request.QueryString["syllabusId"];

        GridView1.PageIndex = e.NewPageIndex;
        this.BindGrid(syllabusId);
    }
}