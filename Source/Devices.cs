/*
 * Mã sinh viên: 202418855
 * Họ tên: Đỗ Văn Chung
 * Mã lớp: 174373
 */
namespace Lab08;
public abstract class TeachingDevice
{
    public string Id { get; }
    public string Name { get; }
    public DeviceStatus Status { get; private set; } = DeviceStatus.Available;
    public event EventHandler<DeviceStatusChangedEventArgs>? StatusChanged;
    public event EventHandler<DeviceAlertEventArgs>? AlertRaised;
    protected TeachingDevice(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Mã và tên không được rỗng.");
        Id = id.Trim(); Name = name.Trim();
    }
    public void ChangeStatus(DeviceStatus newStatus)
    {
        if (!Enum.IsDefined(typeof(DeviceStatus), newStatus))
            throw new ArgumentOutOfRangeException(nameof(newStatus));
        if (Status == newStatus) return;
        var oldStatus = Status;
        Status = newStatus; // Subscriber luôn quan sát trạng thái mới.
        OnStatusChanged(new(Id, oldStatus, newStatus, DateTime.Now));
    }
    protected void RaiseAlert(string message, AlertSeverity severity)
        => OnAlertRaised(new(Id, message, severity, DateTime.Now));
    protected virtual void OnStatusChanged(DeviceStatusChangedEventArgs e)
        => StatusChanged?.Invoke(this, e);
    protected virtual void OnAlertRaised(DeviceAlertEventArgs e)
        => AlertRaised?.Invoke(this, e);
    protected void ValidateReading(int value)
    {
        if (value < 0 || value > 100) throw new InvalidDeviceReadingException(Id, value);
    }
    private static int Zone(int value, int threshold) => value == 0 ? 2 : value < threshold ? 1 : 0;
    protected void CheckTransition(int oldValue, int newValue, int threshold, string label)
    {
        int zone = Zone(newValue, threshold);
        if (zone == 0 || zone == Zone(oldValue, threshold)) return;
        RaiseAlert($"{label}: {oldValue}% -> {newValue}%.",
            zone == 2 ? AlertSeverity.Critical : AlertSeverity.Warning);
    }
}
public class ProjectorRemote : TeachingDevice
{
    public int BatteryLevel { get; private set; } = 100;
    public ProjectorRemote(string id, string name) : base(id, name) { }
    public void UpdateBatteryLevel(int newLevel)
    {
        ValidateReading(newLevel);
        int old = BatteryLevel; BatteryLevel = newLevel;
        CheckTransition(old, newLevel, 20, "Pin máy chiếu");
    }
}
public class AirConditionerRemote : TeachingDevice
{
    public string AssignedRoom { get; }
    public bool IsResponsive { get; private set; } = true;
    public AirConditionerRemote(string id, string name, string assignedRoom) : base(id, name)
    {
        if (string.IsNullOrWhiteSpace(assignedRoom)) throw new ArgumentException("Phòng không được rỗng.");
        AssignedRoom = assignedRoom.Trim();
    }
    public void UpdateResponseState(bool isResponsive)
    {
        bool old = IsResponsive; IsResponsive = isResponsive;
        if (old && !isResponsive)
            RaiseAlert($"Điều hòa phòng {AssignedRoom} không phản hồi.", AlertSeverity.Critical);
    }
}
public class WirelessMicrophone : TeachingDevice
{
    public int BatteryLevel { get; private set; } = 100;
    public int SignalQuality { get; private set; } = 100;
    public WirelessMicrophone(string id, string name) : base(id, name) { }
    public void UpdateBatteryLevel(int newLevel)
    {
        ValidateReading(newLevel);
        int old = BatteryLevel; BatteryLevel = newLevel;
        CheckTransition(old, newLevel, 30, "Pin micro");
    }
    public void UpdateSignalQuality(int newQuality)
    {
        ValidateReading(newQuality);
        int old = SignalQuality; SignalQuality = newQuality;
        CheckTransition(old, newQuality, 40, "Tín hiệu micro");
    }
}
