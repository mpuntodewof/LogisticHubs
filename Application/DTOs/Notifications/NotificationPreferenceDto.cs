namespace Application.DTOs.Notifications
{
    public class NotificationPreferenceDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool EnableEmail { get; set; }
        public bool EnableSMS { get; set; }
        public bool EnablePush { get; set; }
    }

    public class UpdateNotificationPreferenceRequest
    {
        public bool? EnableEmail { get; set; }
        public bool? EnableSMS { get; set; }
        public bool? EnablePush { get; set; }
    }
}
