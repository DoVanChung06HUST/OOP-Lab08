/*
 * Mã sinh viên: 202418855
 * Họ tên: Đỗ Văn Chung
 * Mã lớp: 174373
 */
namespace Lab08;
public enum DeviceStatus { Available, Borrowed, UnderMaintenance }
public enum AlertSeverity { Information, Warning, Critical }

public class DeviceStatusChangedEventArgs : EventArgs
{
    public string DeviceId { get; }
    public DeviceStatus OldStatus { get; }
    public DeviceStatus NewStatus { get; }
    public DateTime ChangedAt { get; }
    public DeviceStatusChangedEventArgs(string deviceId, DeviceStatus oldStatus,
        DeviceStatus newStatus, DateTime changedAt)
        => (DeviceId, OldStatus, NewStatus, ChangedAt) = (deviceId, oldStatus, newStatus, changedAt);
}
public class DeviceAlertEventArgs : EventArgs
{
    public string DeviceId { get; }
    public string Message { get; }
    public AlertSeverity Severity { get; }
    public DateTime OccurredAt { get; }
    public DeviceAlertEventArgs(string deviceId, string message, AlertSeverity severity, DateTime occurredAt)
        => (DeviceId, Message, Severity, OccurredAt) = (deviceId, message, severity, occurredAt);
}
public class InvalidDeviceReadingException : Exception
{
    public string DeviceId { get; }
    public int InvalidValue { get; }
    public InvalidDeviceReadingException(string deviceId, int invalidValue)
        : base($"Thiết bị {deviceId}: giá trị {invalidValue} ngoài khoảng 0..100.")
        => (DeviceId, InvalidValue) = (deviceId, invalidValue);
}
