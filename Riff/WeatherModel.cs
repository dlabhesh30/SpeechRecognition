using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff
{
    public class WeatherModel
    {
        public string Time
        {
            get;
            set;
        }
        
        public string FeelsLike
        {
            get;
            set;
        }
        
        public string Humidity
        {
            get;
            set;
        }
        
        public string WindSpeed
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Sunrise
        {
            get;
            set;
        }
        
        public string Sunset
        {
            get;
            set;
        }

    }
}
