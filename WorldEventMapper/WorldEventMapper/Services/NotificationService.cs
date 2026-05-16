using System;

namespace WorldEventMapper.Services
{
    public enum NotificationType
    {
        Success,
        Error
    }

    public class NotificationMessage
    {
        public string Message { get; set; } = "";
        public NotificationType Type { get; set; }
    }

    public static class NotificationService
    {
        public static event Action<NotificationMessage>? NotificationRequested;

        public static void ShowSuccess(string message)
        {
            NotificationRequested?.Invoke(new NotificationMessage
            {
                Message = message,
                Type = NotificationType.Success
            });
        }

        public static void ShowError(string message)
        {
            NotificationRequested?.Invoke(new NotificationMessage
            {
                Message = message,
                Type = NotificationType.Error
            });
        }
    }
}