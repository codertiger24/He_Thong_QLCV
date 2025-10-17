using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class SuaNguoiDung : System.Web.UI.Page
    {
        private static string CS => ConfigurationManager.ConnectionStrings["QLCVDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDropdowns();
                LoadUser();
            }
        }

        private void BindDropdowns()
        {
            // Đơn vị (tblNhom)
            using (var con = new SqlConnection(CS))
            using (var da = new SqlDataAdapter("SELECT MaNhom, MoTa FROM dbo.tblNhom ORDER BY MoTa", con))
            {
                var tb = new DataTable(); da.Fill(tb);
                ddlDonVi.DataSource = tb;
                ddlDonVi.DataTextField = "MoTa";
                ddlDonVi.DataValueField = "MaNhom";
                ddlDonVi.DataBind();
            }

            // Chức vụ (tblChucVu)
            using (var con = new SqlConnection(CS))
            using (var da = new SqlDataAdapter("SELECT MaChucVu, TenChucVu FROM dbo.tblChucVu ORDER BY TenChucVu", con))
            {
                var tb = new DataTable(); da.Fill(tb);
                ddlChucVu.DataSource = tb;
                ddlChucVu.DataTextField = "TenChucVu";
                ddlChucVu.DataValueField = "MaChucVu";
                ddlChucVu.DataBind();
            }
        }

        private void LoadUser()
        {
            string id = Request.QueryString["id"];
            if (string.IsNullOrWhiteSpace(id)) return;

            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT TOP 1 * FROM dbo.tblNguoiDung WHERE MaNguoiDung=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var tb = new DataTable(); da.Fill(tb);
                    if (tb.Rows.Count == 0) return;

                    var r = tb.Rows[0];
                    txtMaNguoiDung.Text = r["MaNguoiDung"].ToString();
                    txtTenDN.Text = r["TenDN"].ToString();
                    txtMatKhau.Attributes["value"] = r["MatKhau"].ToString(); // giữ giá trị password
                    txtHoTen.Text = r["HoTen"].ToString();
                    txtEmail.Text = r["Email"].ToString();

                    // Quyền hạn (Admin/User/QuanLy)
                    string qh = r["QuyenHan"] == DBNull.Value ? "" : r["QuyenHan"].ToString();
                    var item = ddlQuyenHan.Items.FindByValue(qh);
                    if (item != null) ddlQuyenHan.SelectedValue = qh;

                    // Trạng thái
                    bool trangThai = r["TrangThai"] != DBNull.Value && Convert.ToBoolean(r["TrangThai"]);
                    rdoKichHoat.Checked = trangThai;
                    rdoKhongKichHoat.Checked = !trangThai;

                    // Nhóm & Chức vụ
                    if (r["MaNhom"] != DBNull.Value)
                        ddlDonVi.SelectedValue = r["MaNhom"].ToString();
                    if (r["MaChucVu"] != DBNull.Value)
                        ddlChucVu.SelectedValue = r["MaChucVu"].ToString();
                }
            }
        }

        protected void btnLuu_Click(object sender, EventArgs e)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
UPDATE dbo.tblNguoiDung
SET TenDN=@TenDN, MatKhau=@MatKhau, HoTen=@HoTen, Email=@Email,
    QuyenHan=@QuyenHan, TrangThai=@TrangThai, MaNhom=@MaNhom, MaChucVu=@MaChucVu
WHERE MaNguoiDung=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", txtMaNguoiDung.Text);
                cmd.Parameters.AddWithValue("@TenDN", txtTenDN.Text.Trim());
                cmd.Parameters.AddWithValue("@MatKhau", txtMatKhau.Text.Trim());
                cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@QuyenHan", ddlQuyenHan.SelectedValue);
                cmd.Parameters.AddWithValue("@TrangThai", rdoKichHoat.Checked);
                cmd.Parameters.AddWithValue("@MaNhom", ddlDonVi.SelectedValue);
                cmd.Parameters.AddWithValue("@MaChucVu", ddlChucVu.SelectedValue);

                con.Open(); cmd.ExecuteNonQuery();
            }

            Response.Redirect("QLNguoiDung.aspx");
        }

        protected void btnQuayLai_Click(object sender, EventArgs e)
        {
            Response.Redirect("QLNguoiDung.aspx");
        }
    }
}
