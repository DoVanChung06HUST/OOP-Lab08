/*
 * Mã sinh viên: 202418855
 * Họ tên: Đỗ Văn Chung
 * Mã lớp: 174373
 */
namespace Lab08;
public class DeviceMonitoringCoordinator : IDisposable
{
    private readonly List<TeachingDevice> devices;
    private readonly ConsoleNotificationService notificationService;
    private readonly DeviceAuditLogger auditLogger;
    private readonly MaintenanceService maintenanceService;
    private bool subscribed;
    private bool disposed;
    public DeviceMonitoringCoordinator(IEnumerable<TeachingDevice> devices,
        ConsoleNotificationService notificationService, DeviceAuditLogger auditLogger,
        MaintenanceService maintenanceService)
    {
        ArgumentNullException.ThrowIfNull(devices);
        this.devices = devices.Distinct().ToList();
        if (this.devices.Any(d => d is null)) throw new ArgumentException("Thiết bị null.");
        this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        this.auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
        this.maintenanceService = maintenanceService ?? throw new ArgumentNullException(nameof(maintenanceService));
    }
    public void Subscribe()
    {
        if (disposed) throw new ObjectDisposedException(nameof(DeviceMonitoringCoordinator));
        if (subscribed) return;
        foreach (var d in devices)
        {
            d.AlertRaised += notificationService.HandleAlert;
            d.AlertRaised += maintenanceService.HandleCriticalAlert;
            d.StatusChanged += auditLogger.HandleStatusChanged;
        }
        subscribed = true;
    }
    public void Unsubscribe()
    {
        if (!subscribed) return;
        foreach (var d in devices)
        {
            d.AlertRaised -= notificationService.HandleAlert;
            d.AlertRaised -= maintenanceService.HandleCriticalAlert;
            d.StatusChanged -= auditLogger.HandleStatusChanged;
        }
        subscribed = false;
    }
    public void Dispose()
    {
        if (disposed) return;
        Unsubscribe(); disposed = true;
    }
}
