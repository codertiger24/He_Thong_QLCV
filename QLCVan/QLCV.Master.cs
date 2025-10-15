using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace QLCVan
{
    public partial class QLCV : System.Web.UI.MasterPage
    {
        // Whitelist cho ROLE=User (chỉ 2 trang được phép)
        // TODO: Nếu trang "Giới thiệu" không phải Trangchu.aspx, sửa lại cho đúng.
        private static readonly HashSet<string> UserAllowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "~/Gioithieu.aspx",
            "~/Trangchu.aspx",
            // "~/GioiThieu.aspx", // nếu trang tên khác, bỏ comment và chỉnh
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            // 1) Chào mừng + hiển thị nút đăng nhập/đăng xuất
            lblWelcome.Visible = false;
            if (Session["TenDN"] != null)
            {
                lblWelcome.Visible = true;
                lbtLogIn.Visible = false;
                lblWelcome.Text = "Xin chào: " + Session["TenDN"].ToString();
            }

            // 2) Xác định vai trò hiện tại
            string role = GetCurrentRole(); // "Admin" | "User" | ""

            // 3) Bỏ qua kiểm tra cho các trang public (login/403/tài nguyên)
            var path = Request.AppRelativeCurrentExecutionFilePath; // dạng "~/Trang.aspx"
            if (!IsPublicPath(path))
            {
                // Chưa đăng nhập -> về trang đăng nhập
                if (string.IsNullOrEmpty(role))
                {
                    Response.Redirect("~/Dangnhap.aspx");
                    return;
                }

                // ROLE=User -> chỉ cho vào 2 trang whitelisted
                if (role.Equals("User", StringComparison.OrdinalIgnoreCase) && !UserAllowed.Contains(path))
                {
                    // Nếu có trang 403 thì điều hướng tới đó, không thì về login
                    if (System.IO.File.Exists(Server.MapPath("~/Errors/403.aspx")))
                        Response.Redirect("~/Errors/403.aspx");
                    else
                        Response.Redirect("~/Dangnhap.aspx");
                    return;
                }
                // ROLE=Admin -> không hạn chế
            }

            // 4) Đổ menu theo vai trò
            BindMenuByRole(role);
            // Loại bỏ mục "Danh bạ email" và "Tìm kiếm công văn" khỏi menu
            rptMenu.DataSource = db.tblMenus
           .Where(c => c.MenuID.ToString() == "1" || c.MenuID.ToString() == "7" ||
                       c.MenuID.ToString() == "6" || c.MenuID.ToString() == "4" ||
                       c.MenuID.ToString() == "5" || c.MenuID.ToString() == "2" ||
                       c.MenuID.ToString() == "3")
           .Where(c => !c.MenuName.ToLower().Contains("giới thiệu")
                    && c.MenuName != "Danh bạ email"
                    && c.MenuName != "Tìm kiếm công văn");
            rptMenu.DataBind();


        }

        private void BindMenuByRole(string role)
        {
            using (var db = new InfoDataContext())
            {
                // Lấy tập menu cơ bản (đã bỏ Danh bạ email + Tìm kiếm công văn)
                var baseQuery = db.tblMenus.Where(c =>
                        c.MenuID.ToString() == "1" || c.MenuID.ToString() == "7" ||
                        c.MenuID.ToString() == "6" || c.MenuID.ToString() == "4" ||
                        c.MenuID.ToString() == "5" || c.MenuID.ToString() == "2" ||
                        c.MenuID.ToString() == "3")
                    .Where(c => c.MenuName != "Danh bạ email" && c.MenuName != "Tìm kiếm công văn");

                // Kéo ra bộ nhớ để lọc theo danh sách cho phép
                var list = baseQuery.ToList();

                if (role.Equals("User", StringComparison.OrdinalIgnoreCase))
                {
                    var allowedNames = new[] { "GIỚI THIỆU", "XEM CÔNG VĂN" };

                    list = list
                        .Where(m => allowedNames.Any(n =>
                            string.Equals(n, (m.MenuName ?? "").Trim(), StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }

                rptMenu.DataSource = list;
                rptMenu.DataBind();
            }
        }


        private bool IsPublicPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return true;

            return path.Equals("~/Dangnhap.aspx", StringComparison.OrdinalIgnoreCase)
                || path.Equals("~/Errors/403.aspx", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Lấy vai trò hiện tại:
        /// 1) Ưu tiên Session["ROLE"] nếu đã set ở trang đăng nhập
        /// 2) Nếu chưa có, tra DB theo Session["TenDN"] để lấy cột QuyenHan và lưu lại vào Session
        /// </summary>
        private string GetCurrentRole()
        {
            var roleInSession = (Session["ROLE"] ?? "").ToString();
            if (!string.IsNullOrEmpty(roleInSession)) return roleInSession;

            var tenDn = (Session["TenDN"] ?? "").ToString();
            if (string.IsNullOrEmpty(tenDn)) return "";

            try
            {
                using (var db = new InfoDataContext())
                {
                    // Giả sử bảng người dùng là tblNguoiDung và có cột TenDN / QuyenHan
                    var u = db.tblNguoiDungs.FirstOrDefault(x => x.TenDN == tenDn);
                    if (u != null && !string.IsNullOrEmpty(u.QuyenHan))
                    {
                        Session["ROLE"] = u.QuyenHan; // "Admin" | "User"
                        return u.QuyenHan;
                    }
                }
            }
            catch { /* tránh văng lỗi tại master */ }

            return "";
        }

        protected void lbtLogOut_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            lbtLogIn.Visible = true;
            lblWelcome.Visible = false;
            lbtLogOut.Visible = false;
            Response.Redirect("Trangchu.aspx");
        }

        protected void lbtLogIn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dangnhap.aspx");
        }
    }
}
