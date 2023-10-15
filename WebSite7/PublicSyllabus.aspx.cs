using System;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

public partial class CS : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            string uniqueId = Request.QueryString["uniqueId"];
            this.BindGrid(uniqueId);
        }
    }

    private void BindGrid(string uniqueId)
    {

        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string query = " SELECT S.* " +
                       " FROM SYLLABUS S " +
                       " WHERE S.UNIQUE_ID = '" + uniqueId + "'";
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
        
    }

    protected void OnRowEditing(object sender, GridViewEditEventArgs e)
    {
        
    }

    protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        
    }

    protected void OnRowCancelingEdit(object sender, EventArgs e)
    {
        
    }

    protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        
    }

    protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        
    }

    protected void OnPaging(object sender, GridViewPageEventArgs e)
    {
        string uniqueId = Request.QueryString["uniqueId"];
        GridView1.PageIndex = e.NewPageIndex;
        this.BindGrid(uniqueId);
    }

}