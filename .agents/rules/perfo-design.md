# Perfo: quy tắc luôn bật

Mọi thay đổi UI trong frontend/ phải theo skill perfo-design-system
(.agents/skills/perfo-design-system/SKILL.md). Đọc skill trước khi sửa giao diện,
kể cả khi chỉ đổi một nút hay một màu.

Cấm trong code mới:
- Hardcode màu (HEX, RGB, OKLCH) trong component. Chỉ dùng class semantic.
- rounded-* (trừ avatar rounded-full), shadow-*, backdrop-blur, bg-gradient-*.
- Nhãn viết hoa cả chuỗi, eyebrow, đánh số 01/02/03, mũi tên cuối nút, emoji.
- Dùng lại các motif của design system cũ (ticket-notch, sprocket-rule,
  clip-corner, text-eyebrow). Dùng cut-*, notch-*, perf-*, t-* thay thế.

Không được đổi: namespace và class C# (Cinema.*), tên DB, route API và route
frontend, tên field DTO, key i18n, key localStorage/cookie, logic nghiệp vụ,
phân quyền. Đây là thay đổi giao diện và nhận diện thuần túy.

Gặp chỗ không chắc là thương hiệu hay thuật ngữ nghiệp vụ, hoặc design system
không đủ chỉ dẫn: hỏi lại, không tự đoán.
