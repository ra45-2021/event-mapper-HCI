using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using WorldEventMapper.Models;
using WorldEventMapper.Services;

namespace WorldEventMapper.ViewModels
{
    public class MainDataViewModel : ObservableObject
    {
        private readonly JsonDataService _dataService;

        private EventModel? _selectedEvent;
        private EventTypeModel? _selectedEventType;
        private EventTagModel? _selectedEventTag;
        private string _eventSearchText = "";

        public ObservableCollection<EventModel> Events { get; }
        public ObservableCollection<EventTypeModel> EventTypes { get; }
        public ObservableCollection<EventTagModel> EventTags { get; }

        public ICollectionView EventsView { get; }

        public EventModel? SelectedEvent
        {
            get => _selectedEvent;
            set => SetProperty(ref _selectedEvent, value);
        }

        public EventTypeModel? SelectedEventType
        {
            get => _selectedEventType;
            set => SetProperty(ref _selectedEventType, value);
        }

        public EventTagModel? SelectedEventTag
        {
            get => _selectedEventTag;
            set => SetProperty(ref _selectedEventTag, value);
        }

        public string EventSearchText
        {
            get => _eventSearchText;
            set
            {
                if (SetProperty(ref _eventSearchText, value))
                {
                    EventsView.Refresh();
                }
            }
        }

        public string[] AttendanceOptions { get; } =
        {
            "<1000",
            "1000-5000",
            "5000-10000",
            ">10000"
        };

        public ICommand AddEventCommand { get; }
        public ICommand UpdateEventCommand { get; }
        public ICommand DeleteEventCommand { get; }

        public ICommand AddEventTypeCommand { get; }
        public ICommand UpdateEventTypeCommand { get; }
        public ICommand DeleteEventTypeCommand { get; }

        public ICommand AddEventTagCommand { get; }
        public ICommand UpdateEventTagCommand { get; }
        public ICommand DeleteEventTagCommand { get; }

        public ICommand SaveAllCommand { get; }

        public MainDataViewModel()
        {
            _dataService = new JsonDataService();

            AppData data = _dataService.Load();

            Events = new ObservableCollection<EventModel>(data.Events);
            EventTypes = new ObservableCollection<EventTypeModel>(data.EventTypes);
            EventTags = new ObservableCollection<EventTagModel>(data.EventTags);

            EventsView = CollectionViewSource.GetDefaultView(Events);
            EventsView.Filter = FilterEvent;

            AddEventCommand = new RelayCommand(_ => AddEvent());
            UpdateEventCommand = new RelayCommand(_ => SaveAll(), _ => SelectedEvent != null);
            DeleteEventCommand = new RelayCommand(_ => DeleteEvent(), _ => SelectedEvent != null);

            AddEventTypeCommand = new RelayCommand(_ => AddEventType());
            UpdateEventTypeCommand = new RelayCommand(_ => SaveAll(), _ => SelectedEventType != null);
            DeleteEventTypeCommand = new RelayCommand(_ => DeleteEventType(), _ => SelectedEventType != null);

            AddEventTagCommand = new RelayCommand(_ => AddEventTag());
            UpdateEventTagCommand = new RelayCommand(_ => SaveAll(), _ => SelectedEventTag != null);
            DeleteEventTagCommand = new RelayCommand(_ => DeleteEventTag(), _ => SelectedEventTag != null);

            SaveAllCommand = new RelayCommand(_ => SaveAll());
        }

        private bool FilterEvent(object obj)
        {
            if (obj is not EventModel ev)
                return false;

            if (string.IsNullOrWhiteSpace(EventSearchText))
                return true;

            string search = EventSearchText.Trim().ToLower();

            EventTypeModel? type = EventTypes.FirstOrDefault(t => t.ID == ev.EventTypeId);

            string tagText = string.Join(" ",
                EventTags
                    .Where(tag => ev.TagIds.Contains(tag.ID))
                    .Select(tag => $"{tag.ID} {tag.Description} {tag.Color}")
            );

            string years = string.Join(",", ev.PastPerformingYears);

            string combined = string.Join(" ",
                ev.ID,
                ev.Name,
                ev.Description,
                ev.EventTypeId,
                type?.Name,
                type?.Description,
                ev.Attendance,
                ev.IsHumanitarian ? "humanitarian yes true" : "not humanitarian no false",
                ev.Cost.ToString(),
                ev.IconPath,
                years,
                ev.UpcomingDate.ToShortDateString(),
                ev.UpcomingDate.Year.ToString(),
                ev.Location,
                tagText
            ).ToLower();

            return combined.Contains(search);
        }

        private void AddEvent()
        {
            EventModel newEvent = new EventModel
            {
                ID = GenerateId("EV", Events.Count + 1),
                Name = "New Event",
                Description = "",
                EventTypeId = EventTypes.FirstOrDefault()?.ID ?? "",
                Attendance = "<1000",
                IsHumanitarian = false,
                Cost = 0,
                IconPath = "",
                PastPerformingYears = new(),
                UpcomingDate = DateTime.Today,
                Location = "",
                TagIds = new()
            };

            Events.Add(newEvent);
            SelectedEvent = newEvent;
            SaveAll();
            EventsView.Refresh();
        }

        private void DeleteEvent()
        {
            if (SelectedEvent == null)
                return;

            Events.Remove(SelectedEvent);
            SelectedEvent = null;
            SaveAll();
            EventsView.Refresh();
        }

        private void AddEventType()
        {
            EventTypeModel newType = new EventTypeModel
            {
                ID = GenerateId("ET", EventTypes.Count + 1),
                Name = "New Event Type",
                Description = "",
                IconPath = ""
            };

            EventTypes.Add(newType);
            SelectedEventType = newType;
            SaveAll();
            EventsView.Refresh();
        }

        private void DeleteEventType()
        {
            if (SelectedEventType == null)
                return;

            bool isUsed = Events.Any(ev => ev.EventTypeId == SelectedEventType.ID);

            if (isUsed)
                return;

            EventTypes.Remove(SelectedEventType);
            SelectedEventType = null;
            SaveAll();
            EventsView.Refresh();
        }

        private void AddEventTag()
        {
            EventTagModel newTag = new EventTagModel
            {
                ID = GenerateId("TAG", EventTags.Count + 1),
                Description = "New Tag",
                Color = "#0E7EEC"
            };

            EventTags.Add(newTag);
            SelectedEventTag = newTag;
            SaveAll();
            EventsView.Refresh();
        }

        private void DeleteEventTag()
        {
            if (SelectedEventTag == null)
                return;

            string tagId = SelectedEventTag.ID;

            foreach (EventModel ev in Events)
            {
                ev.TagIds.Remove(tagId);
            }

            EventTags.Remove(SelectedEventTag);
            SelectedEventTag = null;
            SaveAll();
            EventsView.Refresh();
        }

        private void SaveAll()
        {
            AppData data = new AppData
            {
                Events = Events.ToList(),
                EventTypes = EventTypes.ToList(),
                EventTags = EventTags.ToList()
            };

            _dataService.Save(data);
            EventsView.Refresh();
        }

        public string GetEffectiveEventIcon(EventModel ev)
        {
            if (!string.IsNullOrWhiteSpace(ev.IconPath))
                return ev.IconPath;

            EventTypeModel? type = EventTypes.FirstOrDefault(t => t.ID == ev.EventTypeId);

            return type?.IconPath ?? "";
        }

        private string GenerateId(string prefix, int number)
        {
            return $"{prefix}{number:000}";
        }
    }
}