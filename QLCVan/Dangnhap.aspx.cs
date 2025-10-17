using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class Dangnhap : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Nếu đã đăng nhập thì về trang chủ
            if (!IsPostBack && Session["ROLE"] != null)
            {
                Response.Redirect("Trangchu.aspx");
            }
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            var username = (Username.Text ?? "").Trim();
            var password = (Password.Text ?? "").Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Session["MaNguoiDung"] = acc.MaNguoiDung;
                Session["TenDN"] = Username.Text;
                Session["Matkhau"] = (Password.Text);
                Session["QuyenHan"] = acc.QuyenHan.ToString();
                /// Lấy danh sách quyền 
                var listQuyen = (
                    from nd in db.tblNguoiDungs
                    join cv in db.tblChucVus on nd.MaChucVu equals cv.MaChucVu
                    join cvnq in db.tblChucVu_tblNhomQuyens on cv.MaChucVu equals cvnq.MaChucVu
                    join nq in db.tblNhomQuyens on cvnq.MaNhomQuyen equals nq.MaNhomQuyen
                    join nqq in db.tblNhomQuyen_tblQuyens on nq.MaNhomQuyen equals nqq.MaNhomQuyen
                    join q in db.tblQuyens on nqq.MaQuyen equals q.MaQuyen
                    where nd.MaNguoiDung == acc.MaNguoiDung
                    select q.MaQuyen

                    ).Distinct().ToList();

                /// nạp danh sách quyền của user vào list 
                Session["ListQuyen"] = listQuyen;
                Response.Redirect("Trangchu.aspx");


            }

            // NOTE: hiện tại so sánh plain-text theo DB demo.
            // Sau này có hash thì thay bằng so sánh hash.
            var acc = db.tblNguoiDungs.SingleOrDefault(a => a.TenDN == username && a.MatKhau == password);

            if (acc == null)
            {
                lbThongbaoLoi.Text = "Tài khoản hoặc mật khẩu không đúng! Yêu cầu nhập lại!";
                return;
            }

            // Nếu có cột TrangThai (1=active, 0=locked) thì bỏ comment đoạn dưới:
            // if (acc.TrangThai != 1)
            // {
            //     lbThongbaoLoi.Text = "Tài khoản đang bị khóa. Vui lòng liên hệ quản trị.";
            //     return;
            // }

            // Chuẩn hoá role: chỉ dùng "Admin" hoặc "User"
            var role = (acc.QuyenHan ?? "").ToString().Trim();
            if (!role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                role = "User";
            }

            // Set session phục vụ MasterPage & toàn hệ thống
            Session["USER_ID"] = acc.MaNguoiDung;        // nếu là string thì vẫn ok
            Session["TenDN"] = acc.TenDN;
            Session["ROLE"] = role;                    // <-- MasterPage sẽ dùng key này
            Session["QuyenHan"] = role;                    // giữ tương thích key cũ
            // Không nên lưu mật khẩu vào session; xoá nếu có:
            Session.Remove("Matkhau");

            // Điều hướng
            Response.Redirect("Trangchu.aspx");
        }
    }
}
