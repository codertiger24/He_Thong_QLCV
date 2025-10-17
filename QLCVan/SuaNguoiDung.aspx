<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="SuaNguoiDung.aspx.cs"
    Inherits="QLCVan.SuaNguoiDung" MasterPageFile="~/QLCV.Master" %>

<asp:Content ID="HeadC" ContentPlaceHolderID="head" runat="server">
  <style type="text/css">
    .form-wrap{max-width:900px;margin:20px auto;background:#fff;border-radius:12px;
      box-shadow:0 4px 12px rgba(0,0,0,.08);padding:24px}
    .grid{display:grid;grid-template-columns:1fr 1fr;gap:16px}
    .row-full{grid-column:1 / -1}
    .field{display:flex;flex-direction:column}
    .field label{font-weight:600;margin-bottom:6px;color:#333}
    .txt,.sel{height:36px;border:1px solid #dcdcdc;border-radius:8px;padding:8px 12px}
    .btn{display:inline-block;padding:10px 18px;border:none;border-radius:8px;font-weight:600;cursor:pointer}
    .primary{background:#0b57d0;color:#fff}
    .secondary{background:#e5e7eb}
    .actions{display:flex;gap:10px;justify-content:flex-end;margin-top:18px}
  </style>
</asp:Content>

<asp:Content ID="BodyC" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="form-wrap">
    <h3 style="text-align:center;margin:0 0 16px">CHỈNH SỬA TÀI KHOẢN NGƯỜI DÙNG</h3>

    <div class="grid">
      <div class="field">
        <label>Mã người dùng:</label>
        <asp:TextBox ID="txtMaNguoiDung" runat="server" CssClass="txt" ReadOnly="true" />
      </div>

      <div class="field">
        <label>Tên đăng nhập:</label>
        <asp:TextBox ID="txtTenDN" runat="server" CssClass="txt" />
      </div>

      <div class="field">
        <label>Mật khẩu:</label>
        <asp:TextBox ID="txtMatKhau" runat="server" CssClass="txt" TextMode="Password" />
      </div>

      <div class="field">
        <label>Họ và tên:</label>
        <asp:TextBox ID="txtHoTen" runat="server" CssClass="txt" />
      </div>

      <div class="field">
        <label>Email:</label>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="txt" />
      </div>

      <!-- Quyền hạn (ID đúng theo code-behind) -->
      <div class="field">
        <label>Quyền hạn:</label>
        <asp:DropDownList ID="ddlQuyenHan" runat="server" CssClass="sel">
          <asp:ListItem Text="Admin"  Value="Admin" />
          <asp:ListItem Text="User"   Value="User" />
          <asp:ListItem Text="QuanLy" Value="QuanLy" />
        </asp:DropDownList>
      </div>

      <div class="field">
        <label>Đơn vị:</label>
        <asp:DropDownList ID="ddlDonVi" runat="server" CssClass="sel" />
      </div>

      <div class="field">
        <label>Chức vụ:</label>
        <asp:DropDownList ID="ddlChucVu" runat="server" CssClass="sel" />
      </div>

      <!-- Trạng thái (ID rdoKichHoat đúng theo code-behind) -->
      <div class="field row-full">
        <label>Trạng thái:</label>
        <div>
          <asp:RadioButton ID="rdoKichHoat" runat="server" GroupName="TrangThai" Text="Kích hoạt" Checked="true" />
          &nbsp;&nbsp;
          <asp:RadioButton ID="rdoKhongKichHoat" runat="server" GroupName="TrangThai" Text="Chưa kích hoạt" />
        </div>
      </div>
    </div>

    <div class="actions">
      <asp:Button ID="btnQuayLai" runat="server" CssClass="btn secondary" Text="Quay lại" OnClick="btnQuayLai_Click" />
      <asp:Button ID="btnLuu" runat="server" CssClass="btn primary" Text="Hoàn tất" OnClick="btnLuu_Click" />
    </div>
  </div>
</asp:Content>
