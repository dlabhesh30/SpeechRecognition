using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Riff
{
    public class Weather : SpeechHandler
    {
        #region Private Data
        private string m_apiBasePath = "https://api.openweathermap.org/data/2.5/onecall?";
        private WebRequest m_webRequest = null;
        private JObject m_weatherResponseObject = null;
        private string m_latitude = "37.5934";
        private string m_longitude = "-122.0439";
        private string m_apiKey = "4a6e22581624fec26914f50cd6342cee";
        private bool m_weatherDataCached = false;
        private string RequestUrl
        {
            get
            {
                return m_apiBasePath + "lat=" + m_latitude + "&lon=" + m_longitude + "&exclude=minutely,daily&units=metric&appid=" + m_apiKey;
            }
        }
        #endregion

        #region Constructor(s)
        public Weather()
        {
            m_webRequest = Bootstrapper.ResolveType<WebRequest>();
        }
        #endregion
        
        #region Public method(s)
        public override void HandleSpeechRequest(string speech)
        {
            if (!speech.Contains("TOMORROW") && speech.Contains("WEATHER") || speech.Contains("TEMPRATURE") || speech.Contains("HOT") || speech.Contains("NOW"))
                GetWeather();
            else
            if ((speech.Contains("TOMORROW") && speech.Contains("WEATHER") || speech.Contains("TEMPRATURE") || speech.Contains("HOT")) || speech.Contains("FORECAST"))
                GetForecast();
            else
                this.PassRequestHandling(speech);

        }
        #endregion

        #region Private method(s)
        private void GetWeather()
        {
            if (!m_weatherDataCached)
            {
                MakeWeatherRequest();
            }

            var currentWeather = new WeatherModel()
            {
                Time = (string)m_weatherResponseObject["current"]["dt"],
                FeelsLike = (string)m_weatherResponseObject["current"]["feels_like"],
                Humidity = (string)m_weatherResponseObject["current"]["humidity"],
                WindSpeed = (string)m_weatherResponseObject["current"]["wind_speed"],
                Description = (string)m_weatherResponseObject["current"]["weather"][0]["description"],
                Sunset = (string)m_weatherResponseObject["current"]["sunset"],
                Sunrise = (string)m_weatherResponseObject["current"]["sunrise"]
            };

            SpeakWeather(currentWeather);
        }

        private void GetForecast()
        {
            if (!m_weatherDataCached)
            {
                MakeWeatherRequest();
            }

            var hourlyWeatherData = (JArray)m_weatherResponseObject["hourly"];
            var hourlyWeatherModel = new List<WeatherModel>();
            foreach (var hourlyData in hourlyWeatherData)
            {
                var hourlyWeather = new WeatherModel()
                {
                    Time = (string)hourlyData["dt"],
                    FeelsLike = (string)hourlyData["feels_like"],
                    Humidity = (string)hourlyData["humidity"],
                    WindSpeed = (string)hourlyData["wind_speed"],
                    Description = (string)hourlyData["weather"][0]["description"],
                    Sunset = null,
                    Sunrise = null
                };
                hourlyWeatherModel.Add(hourlyWeather);
            }

            if (hourlyWeatherModel.Count > 0)
            {
                var forecastModel = hourlyWeatherModel[hourlyWeatherModel.Count / 2];
                if (null != forecastModel)
                {
                    SpeakWeather(forecastModel);
                }
            }
            else
            {
                m_speechContext.Speak("Weather data not found");
            }

        }

        private void MakeWeatherRequest()
        {
            var responseString = m_webRequest.GetRequest(RequestUrl).Result;
            m_weatherResponseObject = JObject.Parse(responseString);
            m_weatherDataCached = true;
        }

        private void SpeakWeather(WeatherModel weatherModel)
        {
            var weather = new StringBuilder();
            weather.AppendLine("I see " + weatherModel.Description);
            weather.AppendLine("It feels like " + weatherModel.FeelsLike + " degree celsius.");
            weather.AppendLine("and Humidity is " + weatherModel.Humidity);

            var weatherSpeechThread = new Thread(new ThreadStart(() => m_speechContext.Speak(weather.ToString())));
            weatherSpeechThread.IsBackground = true;
            weatherSpeechThread.Start();
        }
        #endregion
    }
}
