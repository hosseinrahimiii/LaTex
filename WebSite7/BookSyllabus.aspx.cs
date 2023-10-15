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
            string bookId = Request.QueryString["bookid"];
            this.BindGrid(bookId);
        }
    }

    private void BindGrid(string bookId)
    {
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string query = " SELECT BS.ID, S.NAME AS syllabus_name, BS.POSITION AS position " +
                       " FROM BOOK_SYLLABUS BS " +
                       " LEFT JOIN BOOK B ON BS.BOOK_ID = B.ID " +
                       " LEFT JOIN SYLLABUS S ON BS.SYLLABUS_id = S.ID " +
                       " WHERE BS.BOOK_ID = " + bookId +
                       " ORDER BY BS.POSITION ";
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
        string bookId = Request.QueryString["bookId"];

        string syllabusId = txtSyllabusId.Text;
        txtSyllabusId.Text = "";

        string position = txtPosition.Text;
        txtPosition.Text = "";

        string query = "INSERT INTO BOOK_SYLLABUS VALUES(@BOOK_ID, @SYLLABUS_ID, @POSITION)";
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Parameters.AddWithValue("@BOOK_ID", bookId);
                cmd.Parameters.AddWithValue("@SYLLABUS_ID", syllabusId);
                cmd.Parameters.AddWithValue("@POSITION", position);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        this.BindGrid(bookId);
    }

    protected void OnRowEditing(object sender, GridViewEditEventArgs e)
    {
        string bookId = Request.QueryString["bookId"];

        GridView1.EditIndex = e.NewEditIndex;
        this.BindGrid(bookId);
    }

    protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        string bookId = Request.QueryString["bookId"];

        GridViewRow row = GridView1.Rows[e.RowIndex];
        int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
        string position = (row.FindControl("txtPosition") as TextBox).Text;
        if (!string.IsNullOrWhiteSpace(position))
        {
            string query = "UPDATE BOOK_SYLLABUS SET POSITION=@POSITION WHERE ID=@ID";
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@POSITION", position);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
        
        GridView1.EditIndex = -1;
        this.BindGrid(bookId);
    }

    protected void OnRowCancelingEdit(object sender, EventArgs e)
    {
        string bookId = Request.QueryString["bookId"];

        GridView1.EditIndex = -1;
        this.BindGrid(bookId);
    }

    protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        string bookId = Request.QueryString["bookId"];

        int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
        string query = "DELETE FROM BOOK_SYLLABUS WHERE ID=@ID";
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

        this.BindGrid(bookId);
    }

    protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowIndex != GridView1.EditIndex)
        {
            (e.Row.Cells[2].Controls[2] as LinkButton).Attributes["onclick"] = "return confirm('Do you want to delete this row?');";
        }
    }

    protected void OnPaging(object sender, GridViewPageEventArgs e)
    {
        string bookId = Request.QueryString["bookId"];

        GridView1.PageIndex = e.NewPageIndex;
        this.BindGrid(bookId);
    }
}