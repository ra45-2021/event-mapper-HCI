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
            string projectRoot = GetProjectRoot();

            string folder = Path.Combine(projectRoot, "Services");

            Directory.CreateDirectory(folder);

            _filePath = Path.Combine(folder, "data.json");
        }

        private string GetProjectRoot()
        {
            DirectoryInfo? directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "WorldEventMapper.csproj")))
            {
                directory = directory.Parent;
            }

            if (directory != null)
                return directory.FullName;

            return AppDomain.CurrentDomain.BaseDirectory;
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
            new EventTypeModel { ID = "ET001", Name = "Film Festival", Description = "Film screenings and cinema culture.", IconPath = "Images/Types/film.png" },
            new EventTypeModel { ID = "ET002", Name = "Music Event", Description = "Concerts, performances, and music festivals.", IconPath = "Images/Types/music.png" },
            new EventTypeModel { ID = "ET003", Name = "Conference", Description = "Educational or professional gathering.", IconPath = "Images/Types/conference.png" },
            new EventTypeModel { ID = "ET004", Name = "Art Exhibition", Description = "Gallery, museum, and contemporary art events.", IconPath = "Images/Types/art.png" },
            new EventTypeModel { ID = "ET005", Name = "Sport Event", Description = "Sport competitions and public sport events.", IconPath = "Images/Types/sport.png" },
            new EventTypeModel { ID = "ET006", Name = "Food Festival", Description = "Food, gastronomy, and local cuisine events.", IconPath = "Images/Types/food.png" }
        },

                EventTags =
        {
            new EventTagModel { ID = "TAG001", Description = "Culture", Color = "#8B5CF6" },
            new EventTagModel { ID = "TAG002", Description = "Humanitarian", Color = "#EF4444" },
            new EventTagModel { ID = "TAG003", Description = "Tourism", Color = "#0E7EEC" },
            new EventTagModel { ID = "TAG004", Description = "Outdoor", Color = "#22C55E" },
            new EventTagModel { ID = "TAG005", Description = "Education", Color = "#F59E0B" },
            new EventTagModel { ID = "TAG006", Description = "International", Color = "#6366F1" }
        },

                Events =
        {
            new EventModel { ID = "FI001", Name = "Kustendorf", Description = "International film and music festival.", EventTypeId = "ET001", Attendance = "1000-5000", IsHumanitarian = false, Cost = 25000, IconPath = "Images/Events/kustendorf.png", PastPerformingYears = new() { 2020, 2021, 2022, 2023 }, UpcomingDate = DateTime.Today.AddMonths(2), Location = "Mokra Gora, SRB", TagIds = new() { "TAG001", "TAG003" } },
            new EventModel { ID = "MU001", Name = "Vienna Philharmonic", Description = "Classical music performance event.", EventTypeId = "ET002", Attendance = "5000-10000", IsHumanitarian = false, Cost = 50000, IconPath = "Images/Events/vienna-philharmonic.png", PastPerformingYears = new() { 2021, 2022, 2023 }, UpcomingDate = DateTime.Today.AddMonths(1), Location = "Vienna, AUT", TagIds = new() { "TAG001", "TAG006" } },
            new EventModel { ID = "AR001", Name = "Art London", Description = "Contemporary art fair with international galleries and artists.", EventTypeId = "ET004", Attendance = "5000-10000", IsHumanitarian = false, Cost = 75000, IconPath = "Images/Events/art-london.png", PastPerformingYears = new() { 2019, 2021, 2022, 2023, 2024 }, UpcomingDate = DateTime.Today.AddMonths(3), Location = "London, UK", TagIds = new() { "TAG001", "TAG006" } },
            new EventModel { ID = "SP001", Name = "Belgrade Marathon", Description = "Large public running event through central Belgrade.", EventTypeId = "ET005", Attendance = "> 10000", IsHumanitarian = true, Cost = 60000, IconPath = "Images/Types/sport.png", PastPerformingYears = new() { 2018, 2019, 2021, 2022, 2023, 2024 }, UpcomingDate = DateTime.Today.AddMonths(4), Location = "Belgrade, SRB", TagIds = new() { "TAG002", "TAG004" } },
            new EventModel { ID = "FO001", Name = "Taste of Italy", Description = "Food festival focused on Italian cuisine and local producers.", EventTypeId = "ET006", Attendance = "1000-5000", IsHumanitarian = false, Cost = 18000, IconPath = "Images/Events/taste-of-italy.png", PastPerformingYears = new() { 2021, 2022, 2023 }, UpcomingDate = DateTime.Today.AddMonths(5), Location = "Rome, ITA", TagIds = new() { "TAG003", "TAG006" } },
            new EventModel { ID = "CO001", Name = "TEDx Vienna", Description = "Talks by innovators, researchers, and creative professionals.", EventTypeId = "ET003", Attendance = "1000-5000", IsHumanitarian = false, Cost = 30000, IconPath = "Images/Types/conference.png", PastPerformingYears = new() { 2020, 2022, 2023 }, UpcomingDate = DateTime.Today.AddMonths(2).AddDays(10), Location = "Vienna, AUT", TagIds = new() { "TAG005", "TAG006" } },
            new EventModel { ID = "MU002", Name = "Exit Festival", Description = "Large music festival held at Petrovaradin Fortress.", EventTypeId = "ET002", Attendance = "> 10000", IsHumanitarian = false, Cost = 95000, IconPath = "Images/Events/exit-festival.png", PastPerformingYears = new() { 2018, 2019, 2021, 2022, 2023, 2024 }, UpcomingDate = DateTime.Today.AddMonths(7), Location = "Novi Sad, SRB", TagIds = new() { "TAG001", "TAG003", "TAG006" } },
            new EventModel { ID = "FI002", Name = "Sarajevo Film Festival", Description = "Regional film festival with screenings, awards, and industry events.", EventTypeId = "ET001", Attendance = "5000-10000", IsHumanitarian = false, Cost = 45000, IconPath = "Images/Types/film.png", PastPerformingYears = new() { 2019, 2020, 2021, 2022, 2023, 2024 }, UpcomingDate = DateTime.Today.AddMonths(3).AddDays(20), Location = "Sarajevo, BIH", TagIds = new() { "TAG001", "TAG003", "TAG006" } },
            new EventModel { ID = "AR002", Name = "Louvre Night Exhibition", Description = "Evening art exhibition with guided tours and special collections.", EventTypeId = "ET004", Attendance = "< 1000", IsHumanitarian = false, Cost = 15000, IconPath = "Images/Events/louvre-night.png", PastPerformingYears = new() { 2022, 2023 }, UpcomingDate = DateTime.Today.AddMonths(2).AddDays(5), Location = "Paris, FRA", TagIds = new() { "TAG001", "TAG003" } },
            new EventModel { ID = "SP002", Name = "Global Charity Run", Description = "Humanitarian running event for fundraising and community support.", EventTypeId = "ET005", Attendance = "5000-10000", IsHumanitarian = true, Cost = 22000, IconPath = "Images/Events/global-charity-run.png", PastPerformingYears = new() { 2021, 2022, 2023, 2024 }, UpcomingDate = DateTime.Today.AddMonths(1).AddDays(15), Location = "Paris, FRA", TagIds = new() { "TAG002", "TAG004", "TAG006" } }
        }
            };
        }
    
    }
}