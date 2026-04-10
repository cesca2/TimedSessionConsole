// make this fully api independent handle API requests and common statuses
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SessionLogger.Controllers;

public class APIHandler : BaseController
{
    private readonly HttpClient _httpClient;

    public APIHandler(string url)
    {
        _httpClient = new();
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        // move these to some config and define in controller
        _httpClient.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
        // _httpClient.DefaultRequestHeaders.Add("x-api-key", key);
        _httpClient.BaseAddress = new Uri(url);

    }

    public async Task<byte[]> RetrieveImageBytes(string url)
    {
        byte[] imageBytes = await _httpClient.GetByteArrayAsync(url);
        return imageBytes;
    }

    public async Task<bool> PostAPIInfo<T>(T model, string parameters)
    {

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(parameters, model);
        var jsonString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return true;

        }
        else
        {
            DisplayMessage("Encountered Error", "red");
            Console.WriteLine(jsonString);
            return false;

        }
    }

    public async Task<bool> PutAPIInfo<T>(T model, string parameters)
    {

        HttpResponseMessage response = await _httpClient.PutAsJsonAsync(parameters, model);

        var jsonString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return true;

        }
        else
        {
            //Console.Write((int)response.StatusCode);

            DisplayMessage("Encountered Error", "red");
            Console.WriteLine(jsonString);
            return false;

        }
    }

    public async Task<bool> DeleteAPIInfo(string parameters)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync(parameters);

        var jsonString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return true;

        }
        else
        {
            Console.Write((int)response.StatusCode);

            DisplayMessage("Encountered Error", "red");
            Console.WriteLine(jsonString);
            return false;

        }
    }

    public async Task<List<T>> RetrieveAPIInfo<T>(string parameters)
    {
        List<T> result;

        HttpResponseMessage response = await _httpClient.GetAsync(parameters).ConfigureAwait(false);
        var jsonString = await response.Content.ReadAsStringAsync();

        // Console.WriteLine(jsonString);
        if (response.IsSuccessStatusCode)
        {
            if ((int)response.StatusCode == 204)
            {
                return new List<T>();
            }

            List<T>? result_temp;
            // try List<T> and <T> could be either dependent on context
            try
            {
                List<T>? callresult = await _httpClient.GetFromJsonAsync<List<T>>(parameters);
                result_temp = callresult;
            }
            catch
            {

                T? callresult = await _httpClient.GetFromJsonAsync<T>(parameters);

                if (callresult is T value)
                {
                    result_temp = new List<T> { value };
                }
                else
                {
                    result_temp = new List<T>();
                }

            }
            if (result_temp is List<T> list)
            {
                result = list;
            }
            else
            {
                result = new List<T>();  // capture null case
            }

            return result;
        }
        else
        {
            DisplayMessage("Could not access API");
            return new List<T>();
        }

    }
}