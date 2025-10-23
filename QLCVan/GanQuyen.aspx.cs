using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class GanQuyen : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                string maNhom = Request.QueryString["ma"];
                string tenNhom = Request.QueryString["ten"];

                if (string.IsNullOrEmpty(maNhom))
                {
                    Response.Redirect("QLNhomQuyen.aspx");
                    return; 
                }
                lblTenNhom.Text = tenNhom;
                hdfMaNhom.Value = maNhom;
                LoadData(maNhom);
            }

        }
        private void LoadData(string maNhom)
        {
            var list = from q in db.tblQuyens
                       select new
                       {
                           q.MaQuyen,
                           q.TenQuyen,
                           DaGan = db.tblNhomQuyen_tblQuyens.Any(nq => nq.MaNhomQuyen == maNhom && nq.MaQuyen == q.MaQuyen)
                       };

            gvGanQuyen.DataSource = list.ToList();
            gvGanQuyen.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string maNhom = hdfMaNhom.Value;
            string ma = txtMaQuyen.Text.Trim().ToLower();
            string ten = txtTenQuyen.Text.Trim().ToLower();

            var query = from q in db.tblQuyens
                        where (ma == "" || q.MaQuyen.ToLower().Contains(ma))
                           && (ten == "" || q.TenQuyen.ToLower().Contains(ten))
                        select new
                        {
                            q.MaQuyen,
                            q.TenQuyen,
                            DaGan = db.tblNhomQuyen_tblQuyens.Any(nq => nq.MaNhomQuyen == maNhom && nq.MaQuyen == q.MaQuyen)
                        };

            gvGanQuyen.DataSource = query.ToList();
            gvGanQuyen.DataBind();
        }


        protected void gvGanQuyen_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleQuyen")
            {
                string maQuyen = e.CommandArgument.ToString();
                string maNhom = hdfMaNhom.Value;
                var quyen = db.tblNhomQuyen_tblQuyens.FirstOrDefault(x => x.MaNhomQuyen == maNhom && x.MaQuyen == maQuyen);
                if (quyen == null)
                {
                    var newRow = new tblNhomQuyen_tblQuyen
                    {
                        MaNhomQuyen = maNhom,
                        MaQuyen = maQuyen,
                    };
                    db.tblNhomQuyen_tblQuyens.InsertOnSubmit(newRow);
                }
                else
                {
                    db.tblNhomQuyen_tblQuyens.DeleteOnSubmit(quyen);
                }
                db.SubmitChanges();
                LoadData(maNhom);
                PermissionHelper.ReSyncPermission();
            }
        }

        protected void gvGanQuyen_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvGanQuyen.PageIndex = e.NewPageIndex;
            LoadData(hdfMaNhom.Value);
        }
    }
}