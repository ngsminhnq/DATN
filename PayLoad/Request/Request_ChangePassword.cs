namespace HRemployee.PayLoad.Request
{
    public class Request_ChangePassword
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}