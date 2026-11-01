/*
 * Mã sinh viên: 202418855
 * Họ tên: Đỗ Văn Chung
 * Mã lớp: 174373
 */
using System.Text;
namespace Lab08;
internal static class Program
{
    private static int checks, failures;
    private static void Check(bool ok, string message)
    {
        checks++; if (!ok) failures++;
        Console.WriteLine($"Đối chiếu: {message} => {(ok ? "ĐÚNG" : "SAI")}");
    }
    private static void Section(int n, string title) => Console.WriteLine($"\n========== {n}. {title} ==========");
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("LAB 08 - DELEGATE & EVENT\nĐỗ Văn Chung - 202418855 - Lớp 174373");
        var mc = new ProjectorRemote("MC01", "Điều khiển máy chiếu");
        var dh = new AirConditionerRemote("DH01", "Điều khiển điều hòa", "A203");
        var mi = new WirelessMicrophone("MI01", "Micro không dây");
        var logger = new DeviceAuditLogger(); var maintenance = new MaintenanceService();
        using var coordinator = new DeviceMonitoringCoordinator(new TeachingDevice[] { mc, dh, mi, mc },
            new ConsoleNotificationService(), logger, maintenance);
        coordinator.Subscribe(); coordinator.Subscribe();
        int alertCount = 0, filteredCount = 0;
        EventHandler<DeviceAlertEventArgs> counter = (_, _) => alertCount++;
        var filtered = new FilteredAlertSubscriber(e => e.Severity == AlertSeverity.Critical,
            e => { filteredCount++; Console.WriteLine($"CALLBACK CRITICAL: {e.DeviceId} - {e.Message}"); });
        TeachingDevice[] devices = { mc, dh, mi };
        foreach (var d in devices) { d.AlertRaised += counter; d.AlertRaised += filtered.HandleAlert; }
        try
        {
            Section(1, "THAY ĐỔI TRẠNG THÁI, KHÔNG GHI LOG TRÙNG");
            bool seesNew = false;
            EventHandler<DeviceStatusChangedEventArgs> observer = (sender, e) =>
                seesNew = ReferenceEquals(sender, mc) && mc.Status == e.NewStatus;
            mc.StatusChanged += observer;
            mc.ChangeStatus(DeviceStatus.Borrowed); mc.ChangeStatus(DeviceStatus.Borrowed);
            mc.ChangeStatus(DeviceStatus.Available); mc.ChangeStatus(DeviceStatus.UnderMaintenance);
            mc.StatusChanged -= observer;
            foreach (var row in logger.Entries) Console.WriteLine(row);
            Check(logger.Entries.Count == 3 && seesNew, "3 log; cập nhật trước event; Subscribe hai lần không trùng");

            Section(2, "PIN MÁY CHIẾU: 100 -> 20 -> 19 -> 10 -> 0 -> 0");
            foreach (int value in new[] { 20, 19, 10, 0, 0 }) mc.UpdateBatteryLevel(value);
            Check(alertCount == 2 && maintenance.Requests.Count == 1, "1 Warning, 1 Critical; cùng vùng không lặp");
            mc.UpdateBatteryLevel(10); mc.UpdateBatteryLevel(100); mc.UpdateBatteryLevel(19);
            Check(alertCount == 4, "0 -> 10 cảnh báo Warning; hồi phục rồi xuống thấp cảnh báo lại");

            Section(3, "ĐIỀU HÒA MẤT PHẢN HỒI");
            dh.UpdateResponseState(false); dh.UpdateResponseState(false);
            dh.UpdateResponseState(true); dh.UpdateResponseState(false);
            Check(alertCount == 6 && maintenance.Requests.Count == 3, "Hai lần mất phản hồi, không báo lặp khi vẫn mất");

            Section(4, "PIN VÀ TÍN HIỆU MICRO");
            foreach (int value in new[] { 30, 29, 20, 0, 0 }) mi.UpdateBatteryLevel(value);
            foreach (int value in new[] { 40, 39, 30, 0, 0 }) mi.UpdateSignalQuality(value);
            Check(alertCount == 10 && maintenance.Requests.Count == 5, "Micro có 2 Warning và 2 Critical");
            Check(filteredCount == 5, "AlertFilter + Action chỉ xử lý 5 Critical");

            Section(5, "DỮ LIỆU SAI KHÔNG ĐỔI TRẠNG THÁI, KHÔNG PHÁT EVENT");
            int rejected = 0;
            Action[] badUpdates = { () => mc.UpdateBatteryLevel(-1), () => mc.UpdateBatteryLevel(101),
                () => mi.UpdateBatteryLevel(-1), () => mi.UpdateBatteryLevel(101),
                () => mi.UpdateSignalQuality(-1), () => mi.UpdateSignalQuality(101) };
            foreach (var update in badUpdates)
            {
                try { update(); }
                catch (InvalidDeviceReadingException ex)
                { rejected++; Console.WriteLine($"Đã bắt {ex.GetType().Name}: {ex.Message}"); }
            }
            Check(rejected == 6 && mc.BatteryLevel == 19 && mi.BatteryLevel == 0 &&
                mi.SignalQuality == 0 && alertCount == 10, "6 giá trị ngoài miền đều bị chặn, dữ liệu giữ nguyên");

            Section(6, "DANH SÁCH YÊU CẦU BẢO TRÌ");
            foreach (var row in maintenance.Requests) Console.WriteLine(row);
            Check(maintenance.Requests.Count == 5, "Chỉ Critical tạo yêu cầu; dịch vụ không tự đổi trạng thái");

            Section(7, "HỦY ĐĂNG KÝ VÀ ĐĂNG KÝ LẠI");
            coordinator.Unsubscribe(); coordinator.Unsubscribe();
            mc.ChangeStatus(DeviceStatus.Available); mc.UpdateBatteryLevel(0);
            Check(logger.Entries.Count == 3 && maintenance.Requests.Count == 5 && filteredCount == 6,
                "Coordinator ngừng nhận; callback đăng ký độc lập vẫn chạy");
            coordinator.Subscribe(); coordinator.Subscribe();
            mc.ChangeStatus(DeviceStatus.Borrowed);
            mc.UpdateBatteryLevel(100); mc.UpdateBatteryLevel(0);
            Check(logger.Entries.Count == 4 && maintenance.Requests.Count == 6,
                "Đăng ký lại đúng một lần");

            Section(8, "DISPOSE VÀ PUBLISHER KHÔNG CÓ SUBSCRIBER");
            coordinator.Dispose(); coordinator.Dispose();
            mc.ChangeStatus(DeviceStatus.Available);
            Check(logger.Entries.Count == 4, "Dispose đã hủy handler");
            bool blocked = false;
            try { coordinator.Subscribe(); } catch (ObjectDisposedException) { blocked = true; }
            Check(blocked, "Không Subscribe coordinator đã Dispose");
            var alone = new ProjectorRemote("MC02", "Thiết bị độc lập");
            alone.ChangeStatus(DeviceStatus.Borrowed); alone.UpdateBatteryLevel(0);
            Check(alone.BatteryLevel == 0, "Không có subscriber vẫn hoạt động");

            Section(9, "GIỚI HẠN MULTICAST KHI HANDLER NÉM NGOẠI LỆ");
            bool laterRan = false, propagated = false;
            var faultDevice = new ProjectorRemote("MC03", "Kiểm tra handler lỗi");
            EventHandler<DeviceStatusChangedEventArgs> bad = (_, _) => throw new InvalidOperationException("Lỗi giả lập từ subscriber");
            EventHandler<DeviceStatusChangedEventArgs> later = (_, _) => laterRan = true;
            faultDevice.StatusChanged += bad; faultDevice.StatusChanged += later;
            try { faultDevice.ChangeStatus(DeviceStatus.Borrowed); }
            catch (InvalidOperationException ex) { propagated = true; Console.WriteLine(ex.Message); }
            finally { faultDevice.StatusChanged -= bad; faultDevice.StatusChanged -= later; }
            Check(propagated && !laterRan && faultDevice.Status == DeviceStatus.Borrowed,
                "Ngoại lệ truyền ra; handler sau không chạy; trạng thái đã đổi không rollback");
        }
        finally
        {
            foreach (var d in devices) { d.AlertRaised -= counter; d.AlertRaised -= filtered.HandleAlert; }
        }
        Console.WriteLine($"\nHOÀN THÀNH 9 NHÓM. {checks} đối chiếu, sai: {failures}.");
        Environment.ExitCode = failures == 0 ? 0 : 1;
    }
}
