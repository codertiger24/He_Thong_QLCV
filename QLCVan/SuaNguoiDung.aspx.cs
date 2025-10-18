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
            // Đơn vị
            using (var con = new SqlConnection(CS))
            using (var da = new SqlDataAdapter("SELECT MaDonVi, TenDonVi FROM dbo.tblDonVi ORDER BY TenDonVi", con))
            {
                var tb = new DataTable(); da.Fill(tb);
                ddlDonVi.DataSource = tb;
                ddlDonVi.DataTextField = "TenDonVi";
                ddlDonVi.DataValueField = "MaDonVi";
                ddlDonVi.DataBind();
            }
            ddlDonVi.Items.Insert(0, new ListItem("Đơn vị", ""));

            // Chức vụ
            using (var con = new SqlConnection(CS))
            using (var da = new SqlDataAdapter("SELECT MaChucVu, TenChucVu FROM dbo.tblChucVu ORDER BY TenChucVu", con))
            {
                var tb = new DataTable(); da.Fill(tb);
                ddlChucVu.DataSource = tb;
                ddlChucVu.DataTextField = "TenChucVu";
                ddlChucVu.DataValueField = "MaChucVu";
                ddlChucVu.DataBind();
            }
            ddlChucVu.Items.Insert(0, new ListItem("Chức vụ", ""));
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
                    txtMatKhau.Attributes["value"] = r["MatKhau"].ToString(); // hiển thị như placeholder
                    txtHoTen.Text = r["HoTen"].ToString();
                    txtEmail.Text = r["Email"].ToString();

                    bool trangThai = r["TrangThai"] != DBNull.Value && Convert.ToBoolean(r["TrangThai"]);
                    rdoKichHoat.Checked = trangThai;
                    rdoKhongKichHoat.Checked = !trangThai;

                    if (r["MaDonVi"] != DBNull.Value) ddlDonVi.SelectedValue = r["MaDonVi"].ToString();
                    if (r["MaChucVu"] != DBNull.Value) ddlChucVu.SelectedValue = r["MaChucVu"].ToString();
                }
            }
        }

        // Lấy mật khẩu hiện tại (khi ô mật khẩu để trống)
        private string GetCurrentPassword(string maNguoiDung)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT MatKhau FROM dbo.tblNguoiDung WHERE MaNguoiDung=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", maNguoiDung);
                con.Open();
                var obj = cmd.ExecuteScalar();
                return obj == null ? "" : obj.ToString();
            }
        }

        // Lấy MaNhom hiện tại để giữ nguyên
        private string GetCurrentMaNhom(string maNguoiDung)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT MaNhom FROM dbo.tblNguoiDung WHERE MaNguoiDung=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", maNguoiDung);
                con.Open();
                var obj = cmd.ExecuteScalar();
                return obj == null ? null : obj.ToString();
            }
        }

        protected void btnLuu_Click(object sender, EventArgs e)
        {
            var id = txtMaNguoiDung.Text.Trim();
            var tenDN = txtTenDN.Text.Trim();
            var hoTen = txtHoTen.Text.Trim();
            var email = txtEmail.Text.Trim();
            var maDonVi = ddlDonVi.SelectedValue;
            var maChucVu = ddlChucVu.SelectedValue;
            var trangThai = rdoKichHoat.Checked;

            // Nếu không nhập mật khẩu mới thì dùng mật khẩu hiện tại
            var matKhau = string.IsNullOrWhiteSpace(txtMatKhau.Text)
                          ? GetCurrentPassword(id)
                          : txtMatKhau.Text;

            var maNhom = GetCurrentMaNhom(id); // giữ nguyên nhóm hiện có

            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
UPDATE dbo.tblNguoiDung SET
    Email     = @Email,
    TenDN     = @TenDN,
    MatKhau   = @MatKhau,
    TrangThai = @TrangThai,
    HoTen     = @HoTen,
    MaDonVi   = @MaDonVi,
    MaChucVu  = @MaChucVu,
    QuyenHan  = @QuyenHan,   -- nếu DB không dùng, bỏ dòng này + tham số
    MaNhom    = @MaNhom      -- hoặc bỏ nếu không muốn đổi nhóm
WHERE MaNguoiDung = @Id;", con))
            {
                cmd.Parameters.Add("@Id", SqlDbType.NVarChar, 50).Value = id;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = email;
                cmd.Parameters.Add("@TenDN", SqlDbType.NVarChar, 100).Value = tenDN;
                cmd.Parameters.Add("@MatKhau", SqlDbType.NVarChar, 200).Value = matKhau;
                cmd.Parameters.Add("@TrangThai", SqlDbType.Bit).Value = trangThai;
                cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 200).Value = hoTen;
                cmd.Parameters.Add("@MaDonVi", SqlDbType.NVarChar, 20).Value = string.IsNullOrWhiteSpace(maDonVi) ? (object)DBNull.Value : maDonVi;
                cmd.Parameters.Add("@MaChucVu", SqlDbType.NVarChar, 50).Value = string.IsNullOrWhiteSpace(maChucVu) ? (object)DBNull.Value : maChucVu;

                // Quyền hạn mặc định (hoặc bỏ hẳn khỏi SQL nếu không dùng)
                cmd.Parameters.Add("@QuyenHan", SqlDbType.NVarChar, 50).Value = "User";

                // Giữ nguyên MaNhom hiện có (nếu null thì cho DBNull)
                cmd.Parameters.Add("@MaNhom", SqlDbType.NVarChar, 20).Value = string.IsNullOrEmpty(maNhom) ? (object)DBNull.Value : maNhom;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            Response.Redirect("QLNguoiDung.aspx");
        }

        protected void btnQuayLai_Click(object sender, EventArgs e)
        {
            Response.Redirect("QLNguoiDung.aspx");
        }
    }
}
