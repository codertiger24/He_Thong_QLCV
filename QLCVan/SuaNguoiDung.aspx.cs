using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class SuaNguoiDung : System.Web.UI.Page
    {
        private List<QLNguoiDung.NguoiDung> GetDanhSachNguoiDung()
        {
            return Session["DanhSachNguoiDung"] as List<QLNguoiDung.NguoiDung>;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1) Lấy param
                var tenDN = Request.QueryString["TenDangNhap"]; // đúng với link bạn đang dùng
                if (string.IsNullOrWhiteSpace(tenDN))
                {
                    Response.Redirect("QLNguoiDung.aspx");
                    return;
                }

                // 2) Lấy DS từ Session (có thể null)
                var ds = GetDanhSachNguoiDung();
                if (ds == null || ds.Count == 0)
                {
                    // Không có dữ liệu trong Session -> quay lại list (hoặc bạn có thể nạp lại từ DB tại đây)
                    Response.Redirect("QLNguoiDung.aspx");
                    return;
                }

                // 3) Tìm user
                var user = ds.FirstOrDefault(u => string.Equals(u.TenDangNhap, tenDN, StringComparison.OrdinalIgnoreCase));
                if (user == null)
                {
                    Response.Redirect("QLNguoiDung.aspx");
                    return;
                }

                // 4) Đổ dữ liệu
                txtTenDN.Text = user.TenDangNhap ?? "";
                txtEmail.Text = user.Email ?? "";
                txtMaNguoiDung.Text = user.MaNguoiDung ?? "";
                txtHoTen.Text = user.HoTen ?? "";

                // Chọn DonVi/ChucVu an toàn: nếu không có trong danh sách -> thêm tạm rồi chọn
                SelectOrInsert(ddlDonVi, user.DonVi);
                SelectOrInsert(ddlChucVu, user.ChucVu);

                rbKichHoat.Checked = user.DangKichHoat;
                rbChuaKichHoat.Checked = !user.DangKichHoat;
            }
        }

        // Helper: chọn giá trị nếu có; nếu chưa có thì thêm ListItem(value,value) rồi chọn
        private static void SelectOrInsert(DropDownList ddl, string value)
        {
            var val = value ?? "";
            var item = ddl.Items.FindByValue(val);
            if (item == null)
            {
                // Nếu bạn không muốn tự thêm, có thể bỏ nhánh này và chỉ cần: if(item!=null) ddl.SelectedValue=...
                ddl.Items.Add(new ListItem(val, val));
            }
            ddl.SelectedValue = val;
        }

        protected void btnSua_Click(object sender, EventArgs e)
        {
            var ds = GetDanhSachNguoiDung();
            if (ds == null)
            {
                // Không có session -> quay về danh sách
                Response.Redirect("QLNguoiDung.aspx");
                return;
            }

            var tenDN = (txtTenDN.Text ?? "").Trim();
            var user = ds.FirstOrDefault(u => string.Equals(u.TenDangNhap, tenDN, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                Response.Redirect("QLNguoiDung.aspx");
                return;
            }

            // Validate mật khẩu (nếu bạn muốn bắt buộc khớp khi có nhập)
            if (!string.IsNullOrEmpty(txtMatKhau.Text) || !string.IsNullOrEmpty(txtXacNhanMK.Text))
            {
                if (!string.Equals(txtMatKhau.Text, txtXacNhanMK.Text, StringComparison.Ordinal))
                {
                    // Hiển thị thông báo nhẹ nhàng
                    ClientScript.RegisterStartupScript(GetType(), "pw", "alert('Mật khẩu xác nhận không khớp');", true);
                    return;
                }
                // TODO: Hash mật khẩu trước khi lưu (khuyến nghị). Ở đây demo Session nên bỏ qua.
                // user.MatKhau = txtMatKhau.Text;
            }

            // Cập nhật các trường có trên form
            user.Email = (txtEmail.Text ?? "").Trim();
            user.HoTen = (txtHoTen.Text ?? "").Trim();
            user.DonVi = ddlDonVi.SelectedValue;
            user.ChucVu = ddlChucVu.SelectedValue;
            user.DangKichHoat = rbKichHoat.Checked;

            // Lưu lại vào Session (thực ra user là reference trong list nên không cần, nhưng giữ cho rõ ràng)
            Session["DanhSachNguoiDung"] = ds;

            // Quay lại danh sách
            Response.Redirect("QLNguoiDung.aspx?updated=1", endResponse: false);
        }
    }
}
