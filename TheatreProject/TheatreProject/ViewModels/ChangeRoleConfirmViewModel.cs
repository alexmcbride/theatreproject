namespace TheatreProject.ViewModels
{
    public class ChangeRoleConfirmViewModel
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string OldRole { get; set; }
        public string NewRole { get; set; }

        public bool WasChanged
        {
            get { return OldRole != NewRole; }
        }
    }
}