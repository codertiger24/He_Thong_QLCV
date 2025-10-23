using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace QLCVan
{
    public partial class QLNhom : Page
    {
        string maQuyenYeuCau = "Q007";
        // ===== Model =====
        public class DonVi
        {
            public string MaDonVi { get; set; }
            public string TenDonVi { get; set; }
        }

        private string ConnStr => ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString1"].ConnectionString;

        // Lưu tiêu chí tìm kiếm để phân trang không mất filter
        private string FilterMa
        {
            get => (ViewState["f_ma"] as string) ?? string.Empty;
            set => ViewState["f_ma"] = value;
        }
        private string FilterTen
        {
            get => (ViewState["f_ten"] as string) ?? string.Empty;
            set => ViewState["f_ten"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PermissionHelper.HasPermission(maQuyenYeuCau))
            {
                Response.Write("<script>alert('Bạn không có quyền truy cập trang này!'); window.history.back();</script>");
                Response.End();
            }
            if (!IsPostBack)
            {
                BindGrid(); // load tất cả
            }
        }

        // ====== DATA ACCESS (ADO.NET) ======
        private IEnumerable<DonVi> GetAllDonVi()
        {
            var list = new List<DonVi>();
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT MaDonVi, TenDonVi
                FROM dbo.tblDonVi
                ORDER BY TenDonVi;", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new DonVi
                        {
                            MaDonVi = rd["MaDonVi"] as string,
                            TenDonVi = rd["TenDonVi"] as string
                        });
                    }
                }
            }
            return list;
        }

        private IEnumerable<DonVi> SearchDonVi(string ma, string ten)
        {
            var list = new List<DonVi>();
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT MaDonVi, TenDonVi
                FROM dbo.tblDonVi
                WHERE (@ma = '' OR MaDonVi LIKE N'%' + @ma + N'%')
                  AND (@ten = '' OR TenDonVi LIKE N'%' + @ten + N'%')
                ORDER BY TenDonVi;", cn))
            {
                cmd.Parameters.AddWithValue("@ma", ma ?? string.Empty);
                cmd.Parameters.AddWithValue("@ten", ten ?? string.Empty);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new DonVi
                        {
                            MaDonVi = rd["MaDonVi"] as string,
                            TenDonVi = rd["TenDonVi"] as string
                        });
                    }
                }
            }
            return list;
        }

        private bool MaDonViExists(string ma)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM dbo.tblDonVi
                WHERE MaDonVi = @ma;", cn))
            {
                cmd.Parameters.AddWithValue("@ma", ma ?? "");
                cn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private bool TenDonViExists(string ten, string exceptMa = null)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM dbo.tblDonVi
                WHERE UPPER(LTRIM(RTRIM(TenDonVi))) = UPPER(LTRIM(RTRIM(@ten)))
                  AND (@ma IS NULL OR MaDonVi <> @ma);", cn))
            {
                cmd.Parameters.AddWithValue("@ten", ten ?? "");
                if (string.IsNullOrWhiteSpace(exceptMa))
                    cmd.Parameters.Add("@ma", SqlDbType.NVarChar, 20).Value = DBNull.Value;
                else
                    cmd.Parameters.AddWithValue("@ma", exceptMa);

                cn.Open();
                var count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // KHÔNG tự sinh mã nữa, dùng mã người dùng nhập
        private void InsertDonVi(string ma, string ten)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                INSERT INTO dbo.tblDonVi (MaDonVi, TenDonVi)
                VALUES (@ma, @ten);", cn))
            {
                cmd.Parameters.AddWithValue("@ma", ma);
                cmd.Parameters.AddWithValue("@ten", ten);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private bool UpdateDonVi(string ma, string tenMoi)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                UPDATE dbo.tblDonVi
                SET TenDonVi = @ten
                WHERE MaDonVi = @ma;", cn))
            {
                cmd.Parameters.AddWithValue("@ma", ma);
                cmd.Parameters.AddWithValue("@ten", tenMoi);
                cn.Open();
                int n = cmd.ExecuteNonQuery();
                return n > 0;
            }
        }

        private bool DeleteDonVi(string ma)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                DELETE FROM dbo.tblDonVi
                WHERE MaDonVi = @ma;", cn))
            {
                cmd.Parameters.AddWithValue("@ma", ma);
                cn.Open();
                int n = cmd.ExecuteNonQuery();
                return n > 0;
            }
        }

        // ====== BIND GRID ======
        private void BindGrid(IEnumerable<DonVi> data = null)
        {
            if (data == null)
            {
                if (!string.IsNullOrEmpty(FilterMa) || !string.IsNullOrEmpty(FilterTen))
                    data = SearchDonVi(FilterMa, FilterTen);
                else
                    data = GetAllDonVi();
            }

            gvQLNhom.DataSource = data.ToList();
            gvQLNhom.DataBind();
        }

        // ====== HANDLERS ======
        protected void btnSave_Click(object sender, EventArgs e)
        {
            // LƯU Ý: ASPX đang có 2 ô: txtMaDonVi & txtTenDonVi
            string ma = (txtMaDonVi.Text ?? "").Trim().ToUpperInvariant();
            string ten = (txtTenDonVi.Text ?? "").Trim();

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
            {
                Alert("Vui lòng nhập đầy đủ MÃ và TÊN đơn vị!");
                return;
            }

            // (tuỳ chọn) ràng buộc định dạng mã: bắt đầu bằng chữ + số, ví dụ DV001
            // if (!Regex.IsMatch(ma, @"^[A-Z]{2}\d{3,}$"))
            // {
            //     Alert("Mã đơn vị không hợp lệ (VD: DV001, DV010...).");
            //     return;
            // }

            if (MaDonViExists(ma))
            {
                Alert("Mã đơn vị đã tồn tại!");
                return;
            }
            if (TenDonViExists(ten))
            {
                Alert("Tên đơn vị đã tồn tại!");
                return;
            }

            try
            {
                InsertDonVi(ma, ten);
                txtMaDonVi.Text = "";
                txtTenDonVi.Text = "";
                BindGrid();

                // Đóng modal Thêm
                ScriptManager.RegisterStartupScript(this, GetType(), "hideAddModal",
                    "var el=document.getElementById('addModal');if(el){var m=bootstrap.Modal.getInstance(el)||new bootstrap.Modal(el);m.hide();}", true);
            }
            catch (Exception ex)
            {
                Alert("Lỗi thêm đơn vị: " + ex.Message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            FilterMa = (txtSearchMa.Text ?? "").Trim();
            FilterTen = (txtSearchTen.Text ?? "").Trim();
            BindGrid(SearchDonVi(FilterMa, FilterTen));
        }

        protected void gvQLNhom_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvQLNhom.PageIndex = e.NewPageIndex;
            BindGrid(); // giữ filter
        }

        // Mở modal sửa (không dùng edit inline)
        protected void rowEditing(object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true;
            GridViewRow row = gvQLNhom.Rows[e.NewEditIndex];
            string ma = ((Label)row.FindControl("lblMaDonVi"))?.Text?.Trim();
            string ten = ((Label)row.FindControl("lblTenDonVi"))?.Text?.Trim();

            hdfID.Value = ma;
            txtEditMaDonVi.Text = ma;
            txtEditTenDonVi.Text = ten ?? "";

            ScriptManager.RegisterStartupScript(this, GetType(), "showEditModal", "showEditModal();", true);
        }

        // Lưu sửa (chỉ sửa TÊN)
        protected void btnEditSave_Click(object sender, EventArgs e)
        {
            string ma = hdfID.Value?.Trim();
            string tenMoi = (txtEditTenDonVi.Text ?? "").Trim();

            if (string.IsNullOrEmpty(ma))
            {
                Alert("Thiếu mã đơn vị!");
                return;
            }
            if (string.IsNullOrEmpty(tenMoi))
            {
                Alert("Vui lòng nhập tên đơn vị!");
                return;
            }
            if (TenDonViExists(tenMoi, exceptMa: ma))
            {
                Alert("Tên đơn vị đã tồn tại!");
                return;
            }

            try
            {
                if (!UpdateDonVi(ma, tenMoi))
                {
                    Alert("Không tìm thấy đơn vị để cập nhật!");
                    return;
                }

                BindGrid();

                // Đóng modal Sửa
                ScriptManager.RegisterStartupScript(this, GetType(), "hideEditModal", "hideEditModal();", true);
            }
            catch (Exception ex)
            {
                Alert("Lỗi cập nhật: " + ex.Message);
            }
        }

        // Xoá qua modal xác nhận
        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            var ma = hdfDeleteKey.Value?.Trim();
            if (string.IsNullOrWhiteSpace(ma))
            {
                Alert("Thiếu mã đơn vị cần xoá!");
                return;
            }

            try
            {
                if (DeleteDonVi(ma))
                {
                    BindGrid();
                }
                else
                {
                    Alert("Không tìm thấy đơn vị để xoá!");
                }
            }
            catch (Exception ex)
            {
                Alert("Lỗi xoá: " + ex.Message);
            }
        }

        // Handler cũ (không dùng edit inline)
        protected void rowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvQLNhom.EditIndex = -1;
            BindGrid();
        }

        protected void rowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            e.Cancel = true;
            gvQLNhom.EditIndex = -1;
            BindGrid();
        }

        protected void rowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Không dùng (đã dùng modal)
            string ma = gvQLNhom.DataKeys[e.RowIndex].Value.ToString();
            if (DeleteDonVi(ma)) BindGrid();
        }

        protected void rowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Edit"))
            {
                hdfID.Value = Convert.ToString(e.CommandArgument);
            }
        }

        // Gán JS mở modal xoá cho từng dòng
        protected void gvQLNhom_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lblMa = e.Row.FindControl("lblMaDonVi") as Label;
                var btn = e.Row.FindControl("btnDelete") as LinkButton;
                if (btn != null && lblMa != null)
                {
                    string ma = (lblMa.Text ?? string.Empty).Replace("'", "\\'");
                    btn.Attributes["onclick"] = "return openDeleteModal('" + ma + "');";
                }
            }
        }

        // Helper
        private void Alert(string msg)
        {
            string encoded = System.Web.HttpUtility.JavaScriptStringEncode(msg, true);
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                Guid.NewGuid().ToString(),
                "alert(" + encoded + ");",
                true
            );
        }
    }
}
