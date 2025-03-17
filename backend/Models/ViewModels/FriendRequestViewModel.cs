namespace project_garage.Models.ViewModels
{
    public class FriendRequestViewModel
    {
        public string RequestId { get; set; } // ID заявки
        public string SenderId { get; set; } // ID відправника
        public string SenderName { get; set; } // Ім'я відправника
        public string SenderDescription { get; set; } // Опис профілю відправника
    }

}
