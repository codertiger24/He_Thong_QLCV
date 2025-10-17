using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace QLCVan
{
    public partial class ThemNguoiDung : Page
    {
        private static readonly string CS =
            ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDropdowns();
            }
        }

        private void BindDropdowns()
        {
            using (var con = new SqlConnection(CS))
            {
                con.Open();

                // Đơn vị (tblNhom: MaNhom INT, MoTa NVARCHAR)
                using (var cmd = new SqlCommand("SELECT MaNhom, MoTa FROM tblNhom ORDER BY MoTa", con))
                using (var rd = cmd.ExecuteReader())
                {
                    ddlDonVi.DataSource = rd;
                    ddlDonVi.DataTextField = "MoTa";
                    ddlDonVi.DataValueField = "MaNhom";
                    ddlDonVi.DataBind();
                }
                ddlDonVi.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Đơn vị", ""));

                // Chức vụ (tblChucVu: MaChucVu NVARCHAR, TenChucVu NVARCHAR)
                using (var cmd = new SqlCommand("SELECT MaChucVu, TenChucVu FROM tblChucVu ORDER BY TenChucVu", con))
                using (var rd = cmd.ExecuteReader())
                {
                    ddlChucVu.DataSource = rd;
                    ddlChucVu.DataTextField = "TenChucVu";
                    ddlChucVu.DataValueField = "MaChucVu"; // <-- CHUỖI (VD: NV_01)
                    ddlChucVu.DataBind();
                }
                ddlChucVu.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Chức vụ", ""));
            }
        }

        protected void btnThem_Click(object sender, EventArgs e)
        {
            // --- Lấy dữ liệu từ form ---
            string maND = string.IsNullOrWhiteSpace(txtMaNguoiDung.Text)
                            ? Guid.NewGuid().ToString("N")
                            : txtMaNguoiDung.Text.Trim();

            string tenDN = txtTenDN.Text.Trim();
            string matKhau = txtMatKhau.Text.Trim();
            string xacNhan = txtXacNhanMK.Text.Trim();
            string hoTen = txtHoTen.Text.Trim();
            string email = txtEmail.Text.Trim();

            // dropdowns
            string maNhomStr = ddlDonVi.SelectedValue;     // INT trong DB
            string maChucVu = ddlChucVu.SelectedValue;    // NVARCHAR: "NV_01", "TK_01"
            string quyenHan = ddlQuyenHan.SelectedValue;  // "Admin"/"User"/"QuanLy"
            bool trangThai = rdoKichHoat.Checked;        // true/false

            // --- Validate đơn giản ---
            if (string.IsNullOrWhiteSpace(tenDN) || string.IsNullOrWhiteSpace(email))
            {
                ClientScript.RegisterStartupScript(GetType(), "alert",
                    "alert('Vui lòng nhập đầy đủ Tên đăng nhập và Email!');", true);
                return;
            }
            if (string.IsNullOrWhiteSpace(matKhau) || matKhau != xacNhan)
            {
                ClientScript.RegisterStartupScript(GetType(), "alert",
                    "alert('Mật khẩu trống hoặc xác nhận mật khẩu không khớp!');", true);
                return;
            }
            if (string.IsNullOrEmpty(maNhomStr) || string.IsNullOrEmpty(maChucVu))
            {
                ClientScript.RegisterStartupScript(GetType(), "alert",
                    "alert('Vui lòng chọn Đơn vị và Chức vụ!');", true);
                return;
            }

            // Parse MaNhom an toàn
            if (!int.TryParse(maNhomStr, out int maNhom))
            {
                ClientScript.RegisterStartupScript(GetType(), "alert",
                    "alert('Giá trị Đơn vị không hợp lệ!');", true);
                return;
            }

            // --- Insert vào DB ---
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
                INSERT INTO tblNguoiDung
                (MaNguoiDung, Email, TenDN, MatKhau, QuyenHan, TrangThai, HoTen, MaNhom, MaChucVu)
                VALUES
                (@MaNguoiDung, @Email, @TenDN, @MatKhau, @QuyenHan, @TrangThai, @HoTen, @MaNhom, @MaChucVu);
            ", con))
            {
                cmd.Parameters.Add("@MaNguoiDung", SqlDbType.NVarChar, 50).Value = maND;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = email;
                cmd.Parameters.Add("@TenDN", SqlDbType.NVarChar, 100).Value = tenDN;
                cmd.Parameters.Add("@MatKhau", SqlDbType.NVarChar, 200).Value = matKhau;
                cmd.Parameters.Add("@QuyenHan", SqlDbType.NVarChar, 50).Value = quyenHan;
                cmd.Parameters.Add("@TrangThai", SqlDbType.Bit).Value = trangThai;
                cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 200).Value = hoTen;
                cmd.Parameters.Add("@MaNhom", SqlDbType.Int).Value = maNhom;
                cmd.Parameters.Add("@MaChucVu", SqlDbType.NVarChar, 50).Value = maChucVu;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            // Trở về danh sách
            Response.Redirect("QLNguoiDung.aspx?added=1");
        }
    }
}
