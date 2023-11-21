namespace UserAPI.Services
{
    public class MockEmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            Console.WriteLine($"Sending email to {to}: Subject - {subject}, Body - {body}");
        }
    }

    public class MockSmsService : ISmsService
    {
        public async Task SendSmsAsync(string to, string message)
        {
            Console.WriteLine($"Sending SMS to {to}: Message - {message}");
        }
    }
}
