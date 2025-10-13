<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/QLCV.Master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <!-- Bootstrap + Font Awesome -->
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" />

  <style>
    :root {
      --ink:#0f172a;
      --red:#c00000;
      --blue:#0d6efd;
      --line:#e5e7eb;
    }

    body { background:#fff; font-family:"Segoe UI", Arial, sans-serif; }

 .content-header {
  background: transparent;
  padding: 0;
  border-bottom: none;
  margin: 0 auto 6px auto;
}

.content-header-title {
  text-transform: uppercase;
  font-weight: 700;
  font-size: 20px;
  color: #444;
  margin: 0 0 6px 0;
  letter-spacing: 0;
}


/* ===== Thanh chạy chữ giống hình mẫu ===== */
.welcome-bar {
  background: #c00;                  /* nền đỏ đậm */
  color: #fff;
  border-radius: 4px;                /* bo góc mềm */
  padding: 8px 0;                    /* cao vừa để chữ nằm giữa */
  margin: 0 auto 26px auto;
  font-weight: bold;                 /* in đậm */
  text-align: center;
  display: flex;
  align-items: center;               /* căn giữa theo chiều cao */
  justify-content: center;
  height: 30px;                      /* chiều cao cố định để đều */
  overflow: hidden;                  /* ẩn phần chữ thừa */
}

.welcome-bar marquee {
  font-size: 16px;                   /* chữ lớn hơn chút */
  font-weight: bold;
  color: #fff;
                
}

    /* ===== Phần bố cục chi tiết công văn (giữ nguyên) ===== */
    .cv-wrap{max-width:980px;margin:24px auto 48px auto;}
    .cv-card{background:#fff;border:1px solid #e5e7eb;border-radius:10px;}
    .cv-head{position:relative;padding:18px 22px 0 22px}
    .cv-title{font-weight:700;text-align:center;margin:0 0 14px}
    .cv-badge{position:absolute;right:22px;top:14px;background:#ffe08a;color:#333;
      border-radius:20px;padding:6px 12px;font-weight:600}
    .cv-body{padding:18px 22px 22px}
    .grid2{display:grid;grid-template-columns:1fr 1fr;gap:14px 24px}
    @media(max-width:768px){.grid2{grid-template-columns:1fr}}
    .field{display:flex;align-items:center;gap:12px}
    .field .lbl{width:160px;min-width:160px;color:#0f172a;font-weight:600;text-align:left}
    .field .ctl{flex:1}
    .row-full{grid-column:1/-1}
    .ipt,.sel,.ta{
      width:100%;box-sizing:border-box;height:38px;padding:6px 10px;border:1px solid #d1d5db;
      border-radius:6px;background:#f8fafc;outline:none
    }
    .ta{min-height:92px;height:auto;resize:none;padding-top:8px;line-height:1.4}
    .ipt[readonly],.sel:disabled,.ta[readonly]{background:#f1f5f9;color:#111827}
    .sel{appearance:none;background-image:
      linear-gradient(45deg, transparent 50%, #6b7280 50%),
      linear-gradient(135deg, #6b7280 50%, transparent 50%),
      linear-gradient(to right, #e5e7eb, #e5e7eb);
      background-position:
      calc(100% - 18px) 16px, calc(100% - 12px) 16px, calc(100% - 36px) 0;
      background-size:6px 6px,6px 6px,1px 100%; background-repeat:no-repeat}
    .sel:disabled{opacity:.8}
    .radio-wrap{display:flex;align-items:center;gap:14px}
    .file-line i{font-style:italic}
    .muted{color:#6b7280}
    .actions{display:flex;justify-content:flex-end;gap:10px;margin-top:18px}
    .btn{border:none;border-radius:6px;padding:.5rem 1rem;cursor:pointer}
    .btn-ghost{border:1px solid #cbd5e1;background:#f8fafc;color:#475569}
    .btn-info{background:#0d6efd;color:#fff}
    .btn-primary{background:#0d6efd;color:#fff}
  </style>

  <!-- ===== Tiêu đề + Thanh chạy chữ ===== -->
  <div class="content-header">
    <h2 class="content-header-title">NHẬP NỘI DUNG CÔNG VĂN</h2>
  </div>

  <div class="welcome-bar">
    <marquee behavior="scroll" direction="left" scrollamount="6">
      Chào mừng bạn đến với hệ thống Quản lý Công Văn điện tử.
    </marquee>
  </div>

  <!-- ===== Nội dung công văn ===== -->
  <div class="cv-wrap">
    <div class="cv-card">
      <div class="cv-head">
        <h5 class="cv-title">CHI TIẾT CÔNG VĂN</h5>
        <span class="cv-badge">Đang trình văn bản</span>
      </div>

      <div class="cv-body">
        <div class="grid2">
          <div class="row-full field">
            <div class="lbl">Tiêu đề:</div>
            <div class="ctl"><input class="ipt" type="text" value="Công văn chiến lược" readonly></div>
          </div>

          <div class="field">
            <div class="lbl">Số CV:</div>
            <div class="ctl"><input class="ipt" type="text" value="334-TB/QSQK2" readonly></div>
          </div>

          <div class="field">
            <div class="lbl">Loại công văn:</div>
            <div class="ctl">
              <select class="sel" disabled>
                <option selected>Chiến lược</option>
                <option>Hành chính</option>
              </select>
            </div>
          </div>

          <div class="field">
            <div class="lbl">Ngày ban hành:</div>
            <div class="ctl"><input class="ipt" type="text" value="10/09/2025" readonly></div>
          </div>

          <div class="field">
            <div class="lbl">Ngày gửi:</div>
            <div class="ctl"><input class="ipt" type="text" value="10/09/2025" readonly></div>
          </div>

          <div class="field">
            <div class="lbl">Cơ quan ban hành:</div>
            <div class="ctl">
              <select class="sel" disabled><option selected>Tiểu đoàn 2</option></select>
            </div>
          </div>

          <div class="field">
            <div class="lbl">Đơn vị nhận:</div>
            <div class="ctl">
              <select class="sel" disabled><option selected>Ban giám hiệu</option></select>
            </div>
          </div>

          <div class="field">
            <div class="lbl">Người ký:</div>
            <div class="ctl">
              <select class="sel" disabled><option selected>Tiểu đoàn trưởng</option></select>
            </div>
          </div>

          <div class="field">
            <div class="lbl">Người nhận:</div>
            <div class="ctl">
              <select class="sel" disabled><option selected>Dương Anh Tuấn</option></select>
            </div>
          </div>

          <div class="field">
            <div class="lbl">Bảo mật:</div>
            <div class="ctl radio-wrap">
              <label><input type="radio" checked disabled> Có</label>
              <label><input type="radio" disabled> Không</label>
            </div>
          </div>

          <div class="row-full field">
            <div class="lbl">Trích yếu:</div>
            <div class="ctl">
              <textarea class="ta" readonly>1. Khoa thông báo kế hoạch đồ án đến các Bộ môn...</textarea>
            </div>
          </div>

          <div class="row-full field">
            <div class="lbl">Ghi chú:</div>
            <div class="ctl">
              <textarea class="ta" readonly>Không có ghi chú</textarea>
            </div>
          </div>

          <div class="row-full field">
            <div class="lbl">Tệp đính kèm:</div>
            <div class="ctl file-line">
              <i><a href="#" target="_blank">ISO-IT13.01Lap ke hoach do an tot nghiep (2016).doc</a></i>
              <span class="muted">38400 MB</span>
            </div>
          </div>
        </div>

        <div class="actions">
          <button type="button" class="btn btn-ghost">Quay lại</button>
          <button type="button" class="btn btn-info">Không duyệt</button>
          <button type="button" class="btn btn-primary">Duyệt</button>
        </div>
      </div>
    </div>
  </div>
</asp:Content>
