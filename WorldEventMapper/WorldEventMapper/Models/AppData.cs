using System.Collections.Generic;

namespace WorldEventMapper.Models
{
    public class AppData
    {
        public List<EventModel> Events { get; set; } = new();
        public List<EventTypeModel> EventTypes { get; set; } = new();
        public List<EventTagModel> EventTags { get; set; } = new();
    }
}