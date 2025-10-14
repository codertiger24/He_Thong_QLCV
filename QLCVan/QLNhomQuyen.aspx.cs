using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class QLNhomQuyen : System.Web.UI.Page
    {

        InfoDataContext db = new InfoDataContext();
        private void LoadData()
        {
            var list = from nq in db.tblNhomQuyens select nq;
            gvNhomQuyen.DataSource = list.ToList<tblNhomQuyen>();
            gvNhomQuyen.DataBind();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string ma = txtMaQuyenSR.Text.Trim();
            string ten = txtTenQuyenSR.Text.Trim();

            var query = db.tblNhomQuyens.AsQueryable();

            if (!string.IsNullOrEmpty(ma))
                query = query.Where(x => x.MaNhomQuyen.Contains(ma));
            if (!string.IsNullOrEmpty(ten))
                query = query.Where(x => x.TenNhomQuyen.Contains(ten));

            gvNhomQuyen.DataSource = query.ToList();
            gvNhomQuyen.DataBind();
        }

        protected void gvNhomQuyen_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvNhomQuyen.PageIndex = e.NewPageIndex;
            LoadData();
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            string ma = txtMdMaNhomQuyen.Text.Trim();
            string ten = txtMdTenNhomQuyen.Text.Trim();

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Mã và Tên nhóm quyền không được để trống!');", true);
                // Xoá text trong modal
                txtMdMaNhomQuyen.Text = "";
                txtMdTenNhomQuyen.Text = "";
                return;
            }

            // Kiểm tra trùng mã
            var exist = db.tblNhomQuyens.FirstOrDefault(x => x.MaNhomQuyen == ma);
            if (exist != null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Mã nhóm quyền đã tồn tại!');", true);
                // Xoá text trong modal
                txtMdMaNhomQuyen.Text = "";
                txtMdTenNhomQuyen.Text = "";
                return;
            }

            // Thêm mới
            tblNhomQuyen nq = new tblNhomQuyen
            {
                MaNhomQuyen = ma,
                TenNhomQuyen = ten
            };
            db.tblNhomQuyens.InsertOnSubmit(nq);
            db.SubmitChanges();

            // Refresh lại GridView
            LoadData();

            // Xoá text trong modal
            txtMdMaNhomQuyen.Text = "";
            txtMdTenNhomQuyen.Text = "";

            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal",
                "var modal = bootstrap.Modal.getInstance(document.getElementById('addModal')); if(modal) modal.hide();", true);
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            string ma = hdDeleteId.Value;
            bool hasChild = db.tblNhomQuyen_tblQuyens.Any(x => x.MaNhomQuyen == ma);

            if (hasChild)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Không thể xóa nhóm quyền vì vẫn còn quyền được gán.');", true);
                return;
            }
            if (!string.IsNullOrEmpty(ma))
            {
                var item = db.tblNhomQuyens.FirstOrDefault(x => x.MaNhomQuyen == ma);
                if (item != null)
                {
                    db.tblNhomQuyens.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }

                LoadData();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string ma = hdfMaNhomQuyen.Value.Trim(); 
            string ten = txtEditTen.Text.Trim();

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Tên nhóm quyền không được để trống!');", true);
                return;
            }


                var nhom = db.tblNhomQuyens.FirstOrDefault(x => x.MaNhomQuyen == ma);
                if (nhom != null)
                {
                    nhom.TenNhomQuyen = ten; // ✅ chỉ update tên
                    db.SubmitChanges();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Không tìm thấy nhóm quyền.');", true);
                }

            LoadData(); 
        }
    }
}