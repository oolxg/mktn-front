using System.Net;
using System.Text;
using MakeathonBot.Models;
using Newtonsoft.Json;

namespace MakeathonBot.API;
// ГовнокодMode: ON
public class ApiClient
{
    private static string apiUrl = "https://mktn.vercel.app";
    
    public static async Task<string> SendUserMessageAsync(string message, long chatId)
    {
        using var client = new HttpClient();
    
        var payload = new MKTNRequest
        {
            chatId = chatId,
            message = message
        };
        
        // content is a array of {`role`: `model|user`, `text`: `string`}
        var response = await client.PostAsync($"{apiUrl}/chat", new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception("Response code from Vercel API is not OK"); 
        }
        
        return await response.Content.ReadAsStringAsync();
    }
    
    public static async Task<List<Dictionary<string, string>>> GetChatHistoryAsync(long chatId)
    {
        using var client = new HttpClient();

        var response = await client.GetAsync($"{apiUrl}/history/{chatId}");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception("Response code from Vercel API is not OK"); 
        }
        
        var contentString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(contentString);
    }
    
    public static async Task ResetChatHistoryAsync(long chatId)
    {
        using var client = new HttpClient();

        var response = await client.DeleteAsync($"{apiUrl}/reset/{chatId}");
    }
}