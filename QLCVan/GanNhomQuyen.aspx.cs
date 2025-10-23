using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class GanNhomQuyen : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string maChucVu = Request.QueryString["ma"];
                string tenChucVu = Request.QueryString["ten"];
                if (string.IsNullOrEmpty(maChucVu))
                {
                    Response.Redirect("QLChucVu.aspx");
                    return; // Dừng luôn, tránh chạy tiếp
                }
                lblTenNhom.Text = tenChucVu;
                hdfMaNhom.Value = maChucVu;

                LoadData(maChucVu);
            }
        }

        private void LoadData(string maChucVu)
        {
            var list = from nq in db.tblNhomQuyens
                       select new
                       {
                           nq.MaNhomQuyen,
                           nq.TenNhomQuyen,
                           DaGan = db.tblChucVu_tblNhomQuyens
                               .Any(cnq => cnq.MaChucVu == maChucVu && cnq.MaNhomQuyen == nq.MaNhomQuyen)
                       };

            gvGanQuyen.DataSource = list.ToList();
            gvGanQuyen.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string maChucVu = hdfMaNhom.Value;
            string ma = txtMaQuyen.Text.Trim().ToLower();
            string ten = txtTenQuyen.Text.Trim().ToLower();

            var query = from nq in db.tblNhomQuyens
                        where (ma == "" || nq.MaNhomQuyen.ToLower().Contains(ma))
                           && (ten == "" || nq.TenNhomQuyen.ToLower().Contains(ten))
                        select new
                        {
                            nq.MaNhomQuyen,
                            nq.TenNhomQuyen,
                            DaGan = db.tblChucVu_tblNhomQuyens
                                .Any(cnq => cnq.MaChucVu == maChucVu && cnq.MaNhomQuyen == nq.MaNhomQuyen)
                        };

            gvGanQuyen.DataSource = query.ToList();
            gvGanQuyen.DataBind();
        }

        protected void gvGanQuyen_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleQuyen")
            {
                string maNhomQuyen = e.CommandArgument.ToString();
                string maChucVu = hdfMaNhom.Value;

                var existing = db.tblChucVu_tblNhomQuyens
                                 .FirstOrDefault(x => x.MaChucVu == maChucVu && x.MaNhomQuyen == maNhomQuyen);

                if (existing == null)
                {
                    var newRow = new tblChucVu_tblNhomQuyen
                    {
                        MaChucVu = maChucVu,
                        MaNhomQuyen = maNhomQuyen
                    };
                    db.tblChucVu_tblNhomQuyens.InsertOnSubmit(newRow);
                }
                else
                {
                    db.tblChucVu_tblNhomQuyens.DeleteOnSubmit(existing);
                }

                db.SubmitChanges();
                LoadData(maChucVu);
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
