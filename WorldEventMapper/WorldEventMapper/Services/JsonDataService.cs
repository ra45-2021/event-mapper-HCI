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
                        IconPath = "/Images/film.png"
                    },
                    new EventTypeModel
                    {
                        ID = "ET002",
                        Name = "Music Event",
                        Description = "Concerts, performances, and music festivals.",
                        IconPath = "/Images/music.png"
                    },
                    new EventTypeModel
                    {
                        ID = "ET003",
                        Name = "Conference",
                        Description = "Educational or professional gathering.",
                        IconPath = "/Images/conference.png"
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
                        TagIds = new() { "TAG001" }
                    }
                }
            };
        }
    }
}