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

                // Đơn vị
                using (var cmd = new SqlCommand("SELECT MaDonVi, TenDonVi FROM tblDonVi ORDER BY TenDonVi", con))
                using (var rd = cmd.ExecuteReader())
                {
                    ddlDonVi.DataSource = rd;
                    ddlDonVi.DataTextField = "TenDonVi";
                    ddlDonVi.DataValueField = "MaDonVi";
                    ddlDonVi.DataBind();
                }
                ddlDonVi.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Đơn vị", ""));

                // Chức vụ
                using (var cmd = new SqlCommand("SELECT MaChucVu, TenChucVu FROM tblChucVu ORDER BY TenChucVu", con))
                using (var rd = cmd.ExecuteReader())
                {
                    ddlChucVu.DataSource = rd;
                    ddlChucVu.DataTextField = "TenChucVu";
                    ddlChucVu.DataValueField = "MaChucVu";
                    ddlChucVu.DataBind();
                }
                ddlChucVu.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Chức vụ", ""));
            }
        }

        // ======= TIỆN ÍCH CHỐNG TRÙNG / TỰ SINH MÃ =======
        private bool MaNguoiDungExists(string ma)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM tblNguoiDung WHERE MaNguoiDung = @v", con))
            {
                cmd.Parameters.Add("@v", SqlDbType.NVarChar, 50).Value = ma ?? "";
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private bool TenDNExists(string tenDN)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM tblNguoiDung WHERE TenDN = @v", con))
            {
                cmd.Parameters.Add("@v", SqlDbType.NVarChar, 100).Value = tenDN ?? "";
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private bool EmailExists(string email)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM tblNguoiDung WHERE Email = @v", con))
            {
                cmd.Parameters.Add("@v", SqlDbType.NVarChar, 200).Value = email ?? "";
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // Sinh mã ND001, ND002,... (KHÔNG dùng TRY_CONVERT)
        private string GenerateNextMaNguoiDung()
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
                ;WITH x AS (
	                SELECT CASE WHEN MaNguoiDung LIKE 'ND[0-9]%' 
			                    THEN CONVERT(int, SUBSTRING(MaNguoiDung, 3, 50))
			                    ELSE 0 END AS n
	                FROM tblNguoiDung
                )
                SELECT ISNULL(MAX(n), 0) + 1 FROM x;", con))
            {
                con.Open();
                int next = Convert.ToInt32(cmd.ExecuteScalar());
                return "ND" + next.ToString("000");
            }
        }

        protected void btnThem_Click(object sender, EventArgs e)
        {
            // --- Lấy dữ liệu từ form ---
            string inputMa = (txtMaNguoiDung.Text ?? "").Trim(); // Mã người dùng nhập tay (có thể để trống)
            string tenDN = (txtTenDN.Text ?? "").Trim();
            string matKhau = (txtMatKhau.Text ?? "").Trim();
            string xacNhan = (txtXacNhanMK.Text ?? "").Trim();
            string hoTen = (txtHoTen.Text ?? "").Trim();
            string email = (txtEmail.Text ?? "").Trim();

            string maDonVi = ddlDonVi.SelectedValue;
            string maChucVu = ddlChucVu.SelectedValue;
            string quyenHan = "User";              // mặc định (UI đã bỏ)
            bool trangThai = rdoKichHoat.Checked;

            // --- Validate cơ bản ---
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
            if (string.IsNullOrEmpty(maDonVi) || string.IsNullOrEmpty(maChucVu))
            {
                ClientScript.RegisterStartupScript(GetType(), "alert",
                    "alert('Vui lòng chọn Đơn vị và Chức vụ!');", true);
                return;
            }

            // --- Check trùng TenDN & Email ---
            if (TenDNExists(tenDN))
            {
                ClientScript.RegisterStartupScript(GetType(), "alert",
                    "alert('Tên đăng nhập đã tồn tại!');", true);
                return;
            }
            if (EmailExists(email))
            {
                ClientScript.RegisterStartupScript(GetType(), "alert",
                    "alert('Email đã tồn tại!');", true);
                return;
            }

            // --- Xác định MaNguoiDung (PK) ---
            string maND;
            if (string.IsNullOrWhiteSpace(inputMa))
            {
                // Không nhập -> tự sinh NDxxx duy nhất
                maND = GenerateNextMaNguoiDung();
            }
            else
            {
                // Có nhập -> kiểm tra trùng
                if (MaNguoiDungExists(inputMa))
                {
                    ClientScript.RegisterStartupScript(GetType(), "alert",
                        "alert('Mã người dùng đã tồn tại. Vui lòng nhập mã khác hoặc để trống để hệ thống tự sinh!');", true);
                    return;
                }
                maND = inputMa;
            }

            // 🔹 Lấy MaNhom mặc định (ưu tiên nhóm có sẵn trong DB)
            string maNhom = null;
            using (var conLookup = new SqlConnection(CS))
            using (var cmdLookup = new SqlCommand("SELECT TOP 1 MaNhom FROM tblNhom ORDER BY MaNhom", conLookup))
            {
                conLookup.Open();
                var res = cmdLookup.ExecuteScalar();
                maNhom = res != null ? res.ToString() : null;
            }
            if (string.IsNullOrEmpty(maNhom))
            {
                maNhom = "NHOM01"; // Đảm bảo tồn tại trong DB
            }

            // --- Insert vào DB ---
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
                INSERT INTO tblNguoiDung
                    (MaNguoiDung, Email, TenDN, MatKhau, QuyenHan, TrangThai, HoTen, MaDonVi, MaChucVu, MaNhom)
                VALUES
                    (@MaNguoiDung, @Email, @TenDN, @MatKhau, @QuyenHan, @TrangThai, @HoTen, @MaDonVi, @MaChucVu, @MaNhom);", con))
            {
                cmd.Parameters.Add("@MaNguoiDung", SqlDbType.NVarChar, 50).Value = maND;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = email;
                cmd.Parameters.Add("@TenDN", SqlDbType.NVarChar, 100).Value = tenDN;
                cmd.Parameters.Add("@MatKhau", SqlDbType.NVarChar, 200).Value = matKhau;
                cmd.Parameters.Add("@QuyenHan", SqlDbType.NVarChar, 50).Value = quyenHan;
                cmd.Parameters.Add("@TrangThai", SqlDbType.Bit).Value = trangThai;
                cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 200).Value = hoTen;
                cmd.Parameters.Add("@MaDonVi", SqlDbType.NVarChar, 20).Value = maDonVi;
                cmd.Parameters.Add("@MaChucVu", SqlDbType.NVarChar, 50).Value = maChucVu;
                cmd.Parameters.Add("@MaNhom", SqlDbType.NVarChar, 20).Value = maNhom;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            Response.Redirect("QLNguoiDung.aspx?added=1");
        }
    }
}
