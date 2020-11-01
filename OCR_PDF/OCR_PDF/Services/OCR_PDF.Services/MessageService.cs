using OCR_PDF.Services.Interfaces;

namespace OCR_PDF.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
