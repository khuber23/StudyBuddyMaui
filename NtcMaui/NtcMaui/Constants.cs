using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace NtcMaui
{
    public static class Constants
    {
        public static HttpClient _client = new HttpClient();
        public static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        // URL of REST service (Android does not use localhost)
        // Use http cleartext for local deployment. Change to https for production
        public static string LocalhostUrl = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
        //public static string Scheme = "https"; // or http
        //public static string Port = "5001";
        //Url of our test api
        public static string TestUrl = "https://instruct.ntc.edu/teststudybuddyapi";

        public static string LocalApiUrl = "https://localhost:7025";

    }
}
