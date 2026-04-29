using System;
using System.IO;
using System.Text.Json;
using WorldEventMapper.Models;

namespace WorldEventMapper.Services
{
    public class JsonDataService
    {
        private readonly string _filePath;

        public JsonDataService()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WorldEventMapper"
            );

            Directory.CreateDirectory(folder);

            _filePath = Path.Combine(folder, "data.json");
        }

        public AppData Load()
        {
            if (!File.Exists(_filePath))
            {
                AppData defaultData = CreateDefaultData();
                Save(defaultData);
                return defaultData;
            }

            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                AppData defaultData = CreateDefaultData();
                Save(defaultData);
                return defaultData;
            }

            AppData? data = JsonSerializer.Deserialize<AppData>(json, GetOptions());

            return data ?? new AppData();
        }

        public void Save(AppData data)
        {
            string json = JsonSerializer.Serialize(data, GetOptions());
            File.WriteAllText(_filePath, json);
        }

        private JsonSerializerOptions GetOptions()
        {
            return new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        private AppData CreateDefaultData()
        {
            return new AppData
            {
                EventTypes =
        {
            new EventTypeModel
            {
                ID = "ET001",
                Name = "Film Festival",
                Description = "Festival focused on film screenings and cinema culture.",
                IconPath = "/Assets/film.png"
            },
            new EventTypeModel
            {
                ID = "ET002",
                Name = "Music Event",
                Description = "Concerts, performances, and music festivals.",
                IconPath = "/Assets/music.png"
            },
            new EventTypeModel
            {
                ID = "ET003",
                Name = "Conference",
                Description = "Educational or professional gathering.",
                IconPath = "/Assets/conference.png"
            },
            new EventTypeModel
            {
                ID = "ET004",
                Name = "Art Exhibition",
                Description = "Gallery, museum, and contemporary art events.",
                IconPath = "/Assets/art.png"
            },
            new EventTypeModel
            {
                ID = "ET005",
                Name = "Sport Event",
                Description = "Sport competitions, tournaments, and public sport events.",
                IconPath = "/Assets/sport.png"
            },
            new EventTypeModel
            {
                ID = "ET006",
                Name = "Food Festival",
                Description = "Food, wine, gastronomy, and local cuisine events.",
                IconPath = "/Assets/food.png"
            },
            new EventTypeModel
            {
                ID = "ET007",
                Name = "Technology Expo",
                Description = "Technology fairs, startup expos, and innovation events.",
                IconPath = "/Assets/technology.png"
            },
            new EventTypeModel
            {
                ID = "ET008",
                Name = "Humanitarian Event",
                Description = "Charity, donation, and social impact events.",
                IconPath = "/Assets/humanitarian.png"
            }
        },

                EventTags =
        {
            new EventTagModel
            {
                ID = "TAG001",
                Description = "Culture",
                Color = "#8B5CF6"
            },
            new EventTagModel
            {
                ID = "TAG002",
                Description = "Humanitarian",
                Color = "#EF4444"
            },
            new EventTagModel
            {
                ID = "TAG003",
                Description = "Tourism",
                Color = "#0E7EEC"
            },
            new EventTagModel
            {
                ID = "TAG004",
                Description = "Outdoor",
                Color = "#22C55E"
            },
            new EventTagModel
            {
                ID = "TAG005",
                Description = "Education",
                Color = "#F59E0B"
            },
            new EventTagModel
            {
                ID = "TAG006",
                Description = "Technology",
                Color = "#06B6D4"
            },
            new EventTagModel
            {
                ID = "TAG007",
                Description = "Family Friendly",
                Color = "#EC4899"
            },
            new EventTagModel
            {
                ID = "TAG008",
                Description = "International",
                Color = "#6366F1"
            }
        },

                Events =
        {
            new EventModel
            {
                ID = "FI001",
                Name = "Kustendorf",
                Description = "International film and music festival.",
                EventTypeId = "ET001",
                Attendance = "1000-5000",
                IsHumanitarian = false,
                Cost = 25000,
                IconPath = "",
                PastPerformingYears = new() { 2020, 2021, 2022, 2023 },
                UpcomingDate = DateTime.Today.AddMonths(2),
                Location = "Mokra Gora, Serbia",
                TagIds = new() { "TAG001", "TAG003" }
            },
            new EventModel
            {
                ID = "MU002",
                Name = "Vienna Philharmonic",
                Description = "Classical music performance event.",
                EventTypeId = "ET002",
                Attendance = "5000-10000",
                IsHumanitarian = false,
                Cost = 50000,
                IconPath = "",
                PastPerformingYears = new() { 2021, 2022, 2023 },
                UpcomingDate = DateTime.Today.AddMonths(1),
                Location = "Vienna, Austria",
                TagIds = new() { "TAG001", "TAG008" }
            },
            new EventModel
            {
                ID = "AR001",
                Name = "Art Basel",
                Description = "Contemporary art fair with international galleries and artists.",
                EventTypeId = "ET004",
                Attendance = "5000-10000",
                IsHumanitarian = false,
                Cost = 75000,
                IconPath = "",
                PastPerformingYears = new() { 2019, 2021, 2022, 2023, 2024 },
                UpcomingDate = DateTime.Today.AddMonths(3),
                Location = "Basel, Switzerland",
                TagIds = new() { "TAG001", "TAG008" }
            },
            new EventModel
            {
                ID = "SP001",
                Name = "Belgrade Marathon",
                Description = "Large public running event through central Belgrade.",
                EventTypeId = "ET005",
                Attendance = ">10000",
                IsHumanitarian = true,
                Cost = 60000,
                IconPath = "",
                PastPerformingYears = new() { 2018, 2019, 2021, 2022, 2023, 2024 },
                UpcomingDate = DateTime.Today.AddMonths(4),
                Location = "Belgrade, Serbia",
                TagIds = new() { "TAG002", "TAG004", "TAG007" }
            },
            new EventModel
            {
                ID = "FO001",
                Name = "Taste of Italy",
                Description = "Food festival focused on Italian cuisine and local producers.",
                EventTypeId = "ET006",
                Attendance = "1000-5000",
                IsHumanitarian = false,
                Cost = 18000,
                IconPath = "",
                PastPerformingYears = new() { 2021, 2022, 2023 },
                UpcomingDate = DateTime.Today.AddMonths(5),
                Location = "Rome, Italy",
                TagIds = new() { "TAG003", "TAG007", "TAG008" }
            },
            new EventModel
            {
                ID = "TE001",
                Name = "Web Summit",
                Description = "Technology conference focused on startups, AI, and digital products.",
                EventTypeId = "ET007",
                Attendance = ">10000",
                IsHumanitarian = false,
                Cost = 120000,
                IconPath = "",
                PastPerformingYears = new() { 2020, 2021, 2022, 2023, 2024 },
                UpcomingDate = DateTime.Today.AddMonths(6),
                Location = "Lisbon, Portugal",
                TagIds = new() { "TAG005", "TAG006", "TAG008" }
            },
            new EventModel
            {
                ID = "CO001",
                Name = "TEDx Vienna",
                Description = "Talks by innovators, researchers, and creative professionals.",
                EventTypeId = "ET003",
                Attendance = "1000-5000",
                IsHumanitarian = false,
                Cost = 30000,
                IconPath = "",
                PastPerformingYears = new() { 2020, 2022, 2023 },
                UpcomingDate = DateTime.Today.AddMonths(2).AddDays(10),
                Location = "Vienna, Austria",
                TagIds = new() { "TAG005", "TAG008" }
            },
            new EventModel
            {
                ID = "HU001",
                Name = "Global Charity Run",
                Description = "Humanitarian running event for fundraising and community support.",
                EventTypeId = "ET008",
                Attendance = "5000-10000",
                IsHumanitarian = true,
                Cost = 22000,
                IconPath = "",
                PastPerformingYears = new() { 2021, 2022, 2023, 2024 },
                UpcomingDate = DateTime.Today.AddMonths(1).AddDays(15),
                Location = "Paris, France",
                TagIds = new() { "TAG002", "TAG004", "TAG007", "TAG008" }
            },
            new EventModel
            {
                ID = "MU003",
                Name = "Exit Festival",
                Description = "Large music festival held at Petrovaradin Fortress.",
                EventTypeId = "ET002",
                Attendance = ">10000",
                IsHumanitarian = false,
                Cost = 95000,
                IconPath = "",
                PastPerformingYears = new() { 2018, 2019, 2021, 2022, 2023, 2024 },
                UpcomingDate = DateTime.Today.AddMonths(7),
                Location = "Novi Sad, Serbia",
                TagIds = new() { "TAG001", "TAG003", "TAG008" }
            },
            new EventModel
            {
                ID = "FI002",
                Name = "Sarajevo Film Festival",
                Description = "Regional film festival with screenings, awards, and industry events.",
                EventTypeId = "ET001",
                Attendance = "5000-10000",
                IsHumanitarian = false,
                Cost = 45000,
                IconPath = "",
                PastPerformingYears = new() { 2019, 2020, 2021, 2022, 2023, 2024 },
                UpcomingDate = DateTime.Today.AddMonths(3).AddDays(20),
                Location = "Sarajevo, Bosnia and Herzegovina",
                TagIds = new() { "TAG001", "TAG003", "TAG008" }
            },
            new EventModel
            {
                ID = "AR002",
                Name = "Louvre Night Exhibition",
                Description = "Evening art exhibition with guided tours and special collections.",
                EventTypeId = "ET004",
                Attendance = "<1000",
                IsHumanitarian = false,
                Cost = 15000,
                IconPath = "",
                PastPerformingYears = new() { 2022, 2023 },
                UpcomingDate = DateTime.Today.AddMonths(2).AddDays(5),
                Location = "Paris, France",
                TagIds = new() { "TAG001", "TAG003" }
            },
            new EventModel
            {
                ID = "TE002",
                Name = "Tokyo Game Show",
                Description = "Technology and gaming expo presenting new games and devices.",
                EventTypeId = "ET007",
                Attendance = ">10000",
                IsHumanitarian = false,
                Cost = 110000,
                IconPath = "",
                PastPerformingYears = new() { 2019, 2020, 2021, 2022, 2023 },
                UpcomingDate = DateTime.Today.AddMonths(8),
                Location = "Tokyo, Japan",
                TagIds = new() { "TAG006", "TAG008" }
            }
        }
            };
        }
    }
}