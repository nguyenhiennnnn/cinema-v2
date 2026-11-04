# MÃ NGUỒN SƠ ĐỒ MERMAID BỔ SUNG — ĐIỂM THÀNH VIÊN 12 THÁNG

Các đoạn dưới đây dán vào `ma-nguon-so-do.md` (file Mermaid gốc, chưa có trong cuộc trò chuyện này). Tên cột khóa chính của `Users`, `Bookings`, `CustomerProfiles` lấy theo quy ước của outline; cần đối chiếu với ERD hiện có.

### Hình 4.58. Lưu đồ ghi sổ điểm, thu hồi và đồng bộ điểm hết hạn

```mermaid
flowchart TD
  A["Thanh toán thành công"] --> B["ApplyLoyaltyAsync: tạo dòng sổ điểm<br/>earned = ⌊FinalAmount / PointsPerAmount⌋"]
  B --> C["Tính điểm hiệu lực P(u, t)<br/>các dòng có EarnedAt trong 12 tháng"]
  C --> D["Chọn hạng có MinPoints lớn nhất ≤ P"]
  D --> E["Cập nhật CustomerProfile.Points và TierId"]
  F["Hoàn tiền thành công"] --> G["ReverseLoyaltyAsync kèm bookingId"]
  G --> H["RevokedPoints = min(Points,<br/>RevokedPoints + ⌊RefundedAmount / PointsPerAmount⌋)"]
  H --> C
  I["Hangfire: ExpireLoyaltyPointsJob<br/>cron 5 * * * * (mỗi giờ, phút 05)"] --> J["LoyaltyExpiryService.RecalculateAllAsync<br/>cho từng khách hàng"]
  J --> C
```

### Hình 4.34. ERD module Thành viên

```mermaid
erDiagram
  MembershipTiers {
    int TierId PK
    varchar TierName UK
    int MinPoints
    decimal DiscountPercent
  }
  LoyaltyPointEntries {
    int EntryId PK
    int UserId FK
    int BookingId FK "nullable, duy nhất khi khác NULL"
    int Points
    int RevokedPoints
    timestamp EarnedAt
  }
  Users {
    int UserId PK
  }
  Bookings {
    int BookingId PK
  }
  CustomerProfiles {
    int UserId PK
  }
  Users ||--o{ LoyaltyPointEntries : "UserId"
  Bookings |o--o| LoyaltyPointEntries : "BookingId"
  MembershipTiers |o--o{ CustomerProfiles : "TierId"
```

### Hình 4.26. Sơ đồ ERD đầy đủ — phần cần thêm

Thêm khối thực thể và hai quan hệ sau vào ERD đầy đủ:

```mermaid
erDiagram
  LoyaltyPointEntries {
    int EntryId PK
    int UserId FK
    int BookingId FK "nullable"
    int Points
    int RevokedPoints
    timestamp EarnedAt
  }
  Users ||--o{ LoyaltyPointEntries : "UserId"
  Bookings |o--o| LoyaltyPointEntries : "BookingId"
```

### Hình 4.51 và Hình 4.54 — bước cần thêm

- Hình 4.51 (tuần tự phát hành vé): sau bước tạo vé và mã QR, thêm thông điệp `BookingService → DB: ghi LoyaltyPointEntry(BookingId, Points, EarnedAt)`, rồi `BookingService → DB: tính lại Points, TierId từ sổ điểm`.
- Hình 4.54 (lưu đồ hủy vé): sau bước hoàn tiền qua cổng, đổi bước "thu hồi điểm" thành "thu hồi điểm vào đúng dòng sổ điểm của booking (`bookingId`), tính lại điểm và hạng".
