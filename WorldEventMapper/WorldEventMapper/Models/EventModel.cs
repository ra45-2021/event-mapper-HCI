using System;
using System.Collections.Generic;
using WorldEventMapper.ViewModels;

namespace WorldEventMapper.Models
{
    public class EventModel : ObservableObject
    {
        private string _id = "";
        private string _name = "";
        private string _description = "";
        private string _eventTypeId = "";
        private string _eventTypeName = "";
        private string _attendance = "< 1000";
        private bool _isHumanitarian;
        private double _cost;
        private string _iconPath = "";
        private List<int> _pastPerformingYears = new();
        private DateTime _upcomingDate = DateTime.Today;
        private string _location = "";
        private List<string> _tagIds = new();

        public string ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string EventTypeId
        {
            get => _eventTypeId;
            set => SetProperty(ref _eventTypeId, value);
        }

        public string EventTypeName
        {
            get => _eventTypeName;
            set => SetProperty(ref _eventTypeName, value);
        }

        public string Attendance
        {
            get => _attendance;
            set => SetProperty(ref _attendance, value);
        }

        public bool IsHumanitarian
        {
            get => _isHumanitarian;
            set => SetProperty(ref _isHumanitarian, value);
        }

        public double Cost
        {
            get => _cost;
            set => SetProperty(ref _cost, value);
        }

        public string IconPath
        {
            get => _iconPath;
            set => SetProperty(ref _iconPath, value);
        }

        public List<int> PastPerformingYears
        {
            get => _pastPerformingYears;
            set => SetProperty(ref _pastPerformingYears, value);
        }

        public DateTime UpcomingDate
        {
            get => _upcomingDate;
            set => SetProperty(ref _upcomingDate, value);
        }

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public List<string> TagIds
        {
            get => _tagIds;
            set => SetProperty(ref _tagIds, value);
        }
    }
}