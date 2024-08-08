using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pet_Shop2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookWebhookController : Controller
    {
        private const string VERIFY_TOKEN = "TokenFBabcd123@";
        private const string PAGE_ACCESS_TOKEN = "EAALb2sXVWqwBO7LPWLblq4BrR8vJmjaZBPgPQeuNAR4o8FpwGCZCn0ynay8GrZCd1w9nZBhqTz926MTVSQczEHuAgMlkJ5ZAZAZBC0fZBTOZALZBshnZCmUj5MlYCOcsyTm26fkoLElQJLCTEdhdiNLVletZBecKi98l5GPPbHihvszSRjel8K5YAxGw4KzxWehLjFONoezqXuy1DP2l8HHZBfmBnXxry";

        private readonly ILogger<FacebookWebhookController> _logger;

        public FacebookWebhookController(ILogger<FacebookWebhookController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery(Name = "hub.mode")] string mode,
                                 [FromQuery(Name = "hub.verify_token")] string token,
                                 [FromQuery(Name = "hub.challenge")] string challenge)
        {
            if (mode == "subscribe" && token == VERIFY_TOKEN)
            {
                _logger.LogInformation("Webhook verified");
                return Ok(challenge);
            }
            else
            {
                _logger.LogWarning("Failed to verify webhook");
                return Forbid();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JsonElement body)
        {
            if (body.TryGetProperty("object", out var obj) && obj.GetString() == "page")
            {
                if (body.TryGetProperty("entry", out var entries))
                {
                    foreach (var entry in entries.EnumerateArray())
                    {
                        if (entry.TryGetProperty("messaging", out var messaging))
                        {
                            foreach (var event_ in messaging.EnumerateArray())
                            {
                                await ProcessMessagingEvent(event_);
                            }
                        }
                    }
                }
                return Ok("EVENT_RECEIVED");
            }

            return BadRequest();
        }

        private async Task ProcessMessagingEvent(JsonElement event_)
        {
            if (event_.TryGetProperty("sender", out var sender) &&
                event_.TryGetProperty("message", out var message))
            {
                var senderId = sender.GetProperty("id").GetString();
                var messageText = message.GetProperty("text").GetString();

                _logger.LogInformation($"Received message from {senderId}: {messageText}");

                // Gửi tin nhắn phản hồi
                await SendTextMessage(senderId, $"Bạn đã gửi: {messageText}");
            }
        }

        private async Task SendTextMessage(string recipientId, string text)
        {
            var apiUrl = $"https://graph.facebook.com/v13.0/me/messages?access_token={PAGE_ACCESS_TOKEN}";

            var messageData = new
            {
                recipient = new { id = recipientId },
                message = new { text = text }
            };

            using var client = new HttpClient();
            var response = await client.PostAsync(apiUrl, new StringContent(JsonSerializer.Serialize(messageData), Encoding.UTF8, "application/json"));
            var result = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Message sent. Response: {result}");
        }
    }
}
