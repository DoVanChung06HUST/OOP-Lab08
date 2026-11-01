/*
 * Mã sinh viên: 202418855
 * Họ tên: Đỗ Văn Chung
 * Mã lớp: 174373
 */
namespace Lab08;
public class ConsoleNotificationService
{
    public void HandleAlert(object? sender, DeviceAlertEventArgs e)
        => Console.WriteLine($"[{e.Severity}] {e.DeviceId}: {e.Message}");
}
public class DeviceAuditLogger
{
    private readonly List<string> entries = new();
    public IReadOnlyList<string> Entries => entries.AsReadOnly();
    public void HandleStatusChanged(object? sender, DeviceStatusChangedEventArgs e)
        => entries.Add($"{e.ChangedAt:yyyy-MM-dd HH:mm:ss}: {e.DeviceId}, {e.OldStatus} -> {e.NewStatus}");
}
public class MaintenanceService
{
    private readonly List<string> requests = new();
    public IReadOnlyList<string> Requests => requests.AsReadOnly();
    public void HandleCriticalAlert(object? sender, DeviceAlertEventArgs e)
    {
        if (e.Severity != AlertSeverity.Critical) return;
        requests.Add($"Yêu cầu bảo trì {e.DeviceId}: {e.Message}");
    }
}
public delegate bool AlertFilter(DeviceAlertEventArgs alert);
public class FilteredAlertSubscriber
{
    private readonly AlertFilter filter;
    private readonly Action<DeviceAlertEventArgs> action;
    public FilteredAlertSubscriber(AlertFilter filter, Action<DeviceAlertEventArgs> action)
    {
        this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
        this.action = action ?? throw new ArgumentNullException(nameof(action));
    }
    public void HandleAlert(object? sender, DeviceAlertEventArgs e)
    {
        if (filter(e)) action(e);
    }
}
