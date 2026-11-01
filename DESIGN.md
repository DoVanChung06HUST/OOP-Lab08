# Thiết kế Lab 08 trước cài đặt
Đỗ Văn Chung - 202418855 - Lớp 174373

Publisher: TeachingDevice và ba lớp con. StatusChanged dành cho lịch sử trạng thái;
AlertRaised dùng chung cho pin, tín hiệu, mất phản hồi vì cùng cấu trúc mức độ/thông điệp.
Subscriber: ConsoleNotificationService, DeviceAuditLogger, MaintenanceService,
FilteredAlertSubscriber. Coordinator sở hữu vòng đời đăng ký, không sở hữu thiết bị.

Thiết bị không gọi dịch vụ bảo trì: tránh phụ thuộc cụ thể, cho phép thêm subscriber.
Chỉ publisher phát event; mã bên ngoài chỉ += và -=. On... protected virtual.

Quy tắc vùng: 0 Critical; 1..ngưỡng-1 Warning; từ ngưỡng trở lên an toàn.
Chỉ phát khi chuyển sang vùng nguy hiểm khác, kể cả 0 -> 10 (Critical -> Warning).
Không phát khi phục hồi an toàn, không lặp trong cùng vùng. Pin micro và tín hiệu
được xét riêng. Constructor mặc định 100, chỉ update mới phát cảnh báo.

Kiểm tra dữ liệu trước cập nhật; cập nhật trạng thái/reading trước khi gọi event.
Dùng multicast chuẩn: exception ở handler truyền ra caller và có thể ngăn handler
sau chạy; trạng thái đã cập nhật không rollback. Demo kiểm chứng và giải thích hạn chế.
Coordinator Subscribe idempotent; Unsubscribe đúng handler; Dispose hủy và không
cho Subscribe lại. Callback bổ sung dùng try/finally để hủy đăng ký.
