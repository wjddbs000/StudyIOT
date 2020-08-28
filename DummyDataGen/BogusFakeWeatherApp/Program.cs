using Bogus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BogusFakeWeatherApp
{
    class Program
    {
        static void Main()
        {
            var Rooms = new[] { "DiningRoom", "LivingRoom", "BathRoom", "BedRoom", "GuestRoom" };

            var sensorFaker = new Faker<SensorInfo>()
            .RuleFor(o => o.Dev_Id, f => f.PickRandom(Rooms))
            .RuleFor(o => o.Curr_Time, f => f.Date.Past(0).ToString("yyyy-MM-dd HH:mm:ss.ff"))
            .RuleFor(o => o.Temp, f => float.Parse(f.Random.Float(19.0f, 35.9f).ToString("0.00")))
            .RuleFor(o => o.Humid, f => float.Parse(f.Random.Float(40.0f, 63.9f).ToString("0.0")))
            .RuleFor(o => o.Press, f => float.Parse(f.Random.Float(800.0f, 999.9f).ToString("0.0")));

            var thisValue = sensorFaker.Generate(10);
            Console.WriteLine(JsonConvert.SerializeObject(thisValue, Formatting.Indented));
        }
    }
}
