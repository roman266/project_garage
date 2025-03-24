namespace project_garage.Interfaces.IService
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task<bool> TrySendVerificationEmailAsync(string email);
        Task VerifyPasswordChangeAsync(string email, string code);
    }
}
