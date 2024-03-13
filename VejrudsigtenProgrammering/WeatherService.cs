using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VejrudsigtenProgrammering
{
    internal class WeatherService
    {
        private const int MAX_FUTURE_DAYS = 3;

        private string key;
        private string location;

        public string TodayDescription { get; set; }
        public double TodayTemperature { get; set; }

        public WeatherInfo[] FutureWeather { get; set; }

        public WeatherService(string location)
        {
            key = ConfigurationManager.AppSettings["servicekey"] ?? "";
            this.location = location;
            TodayDescription = "Ingen beskrivelse";
        }
        public async Task UpdateWeatherAsync()
        {
            var info = await GetTodaysWeather();
            TodayDescription = info.Conditions ?? "Ingen beskrivelse";
            TodayTemperature = info.Temperature;

            var future = await GetFutureWeatherAsync();
            FutureWeather = future;
        }

        public string FutureDescription(int day)
        {
            if (day > MAX_FUTURE_DAYS)
            {
                throw new ArgumentException($"Parameter day is to high. Max value {MAX_FUTURE_DAYS}");
            }
            return FutureWeather[day].Conditions ?? "Ingen beskrivelse";
        }

        public double FutureTemperature(int day)
        {
            if (day > MAX_FUTURE_DAYS)
            {
                throw new ArgumentException($"Parameter day is too high. Max value {MAX_FUTURE_DAYS}");
            }
            return FutureWeather[day].Temperature;
        }

        public string[] GetAllFutureDescriptions()
        {
            // Det første element i future weather er i dag
            // Vi returnerer kun fremtidige dage i result - derfor i+1
            string[] result = new string[FutureWeather.Length - 1];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = FutureWeather[i + 1].Conditions ?? "Ingen beskrivelse";
            }
            return result;
        }

        // Det første element i future weather er i dag
        // Vi returnerer kun fremtidige dage i result - derfor i+1
        public double[] GetAllFutureTemperatures()
        {
            double[] result = new double[FutureWeather.Length - 1];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = FutureWeather[i + 1].Temperature;
            }
            return result;
        }

        private async Task<WeatherInfo> GetTodaysWeather()
        {
            HttpClient client = new HttpClient();
            string url = $"https://smartweatherdk.azurewebsites.net/api/GetTodaysWeather?key={key}&location={location}";
            var streamTask = client.GetStreamAsync(url);
            var weatherInfo = await JsonSerializer.DeserializeAsync<WeatherInfo>(await streamTask);
            return weatherInfo;
        }

        private async Task<WeatherInfo[]> GetFutureWeatherAsync()
        {
            HttpClient client = new HttpClient();
            string url = $"https://smartweatherdk.azurewebsites.net/api/GetForecast?key={key}&location={location}&days={MAX_FUTURE_DAYS}";
            var streamTask = client.GetStreamAsync(url);
            var weatherInfo = await JsonSerializer.DeserializeAsync<WeatherInfo[]>(await streamTask) ?? new WeatherInfo[0];
            return weatherInfo;

        }
    }
}
