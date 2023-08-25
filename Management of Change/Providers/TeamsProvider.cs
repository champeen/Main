using Newtonsoft.Json;

namespace External_Website_v2.Provider
{
    public class TeamsProvider
    {
        private readonly HttpClient _httpClient;
        private string _teamsUrl;
        private class TeamsPayload
        {
            public string text { get; set; }
        }

        public TeamsProvider(string teamsUrl, HttpClient httpClient)
        {
            if (teamsUrl == null)
                throw new ArgumentNullException("Error, teamsUrl argument cannot be null.");
            if (httpClient == null)
                throw new ArgumentNullException("Error, httpClient argument cannot be null.");
            _teamsUrl = teamsUrl;
            _httpClient = httpClient;
        }

        public void SendMessage(string htmlMessage)
        {
            try
            {
                TeamsPayload payload = new TeamsPayload() { text = htmlMessage };
                StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(payload));
                _httpClient.PostAsync(_teamsUrl, jsonContent).Wait();
            }
            catch (Exception ex)
            {
                string systemMessage = "Exception calling TeamsClient.SendMessage(): " +
                    "ex.Message=(" + ex.Message + ")";
            }
        }
    }
}
