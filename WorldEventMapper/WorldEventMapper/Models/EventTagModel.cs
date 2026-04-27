using WorldEventMapper.ViewModels;

namespace WorldEventMapper.Models
{
    public class EventTagModel : ObservableObject
    {
        private string _id = "";
        private string _description = "";
        private string _color = "#0E7EEC";

        public string ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
    }
}