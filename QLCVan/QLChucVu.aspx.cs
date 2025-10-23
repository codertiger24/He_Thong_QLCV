using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class QLChucVu : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();

        private void LoadData()
        {
            var list = from cv in db.tblChucVus select cv;
            gvChucVu.DataSource = list.ToList();
            gvChucVu.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string ma = txtMaChucVuSR.Text.Trim();
            string ten = txtTenChucVuSR.Text.Trim();

            var query = db.tblChucVus.AsQueryable();

            if (!string.IsNullOrEmpty(ma))
                query = query.Where(x => x.MaChucVu.Contains(ma));
            if (!string.IsNullOrEmpty(ten))
                query = query.Where(x => x.TenChucVu.Contains(ten));

            gvChucVu.DataSource = query.ToList();
            gvChucVu.DataBind();
        }

        protected void gvChucVu_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvChucVu.PageIndex = e.NewPageIndex;
            LoadData();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string ma = txtMdMaChucVu.Text.Trim();
            string ten = txtMdTenChucVu.Text.Trim();

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Mã và Tên chức vụ không được để trống!');", true);
                return;
            }

            if (db.tblChucVus.Any(x => x.MaChucVu == ma))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Mã chức vụ đã tồn tại!');", true);
                return;
            }

            tblChucVu cv = new tblChucVu
            {
                MaChucVu = ma,
                TenChucVu = ten
            };

            db.tblChucVus.InsertOnSubmit(cv);
            db.SubmitChanges();
            LoadData();

            txtMdMaChucVu.Text = txtMdTenChucVu.Text = "";
            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal",
                "bootstrap.Modal.getInstance(document.getElementById('addModal')).hide();", true);
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            string ma = hdDeleteId.Value;
            bool hasChild = db.tblChucVu_tblNhomQuyens.Any(x => x.MaChucVu == ma);

            if (hasChild)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Không thể xóa chức vụ vì vẫn còn nhóm quyền được gán.');", true);
                return;
            }
            if (!string.IsNullOrEmpty(ma))
            {
                var item = db.tblChucVus.FirstOrDefault(x => x.MaChucVu == ma);
                if (item != null)
                {
                    try
                    {
                        db.tblChucVus.DeleteOnSubmit(item);
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert",
     "alert('Không thể xóa chức vụ vì vẫn còn người dùng được gán.');", true);
                    }

                }
                LoadData();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string ma = hdfMaChucVu.Value.Trim();
            string ten = txtEditTen.Text.Trim();

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Tên chức vụ không được để trống!');", true);
                return;
            }

            var cv = db.tblChucVus.FirstOrDefault(x => x.MaChucVu == ma);
            if (cv != null)
            {
                cv.TenChucVu = ten;
                db.SubmitChanges();
                LoadData();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Không tìm thấy chức vụ.');", true);
            }
        }
    }
}
