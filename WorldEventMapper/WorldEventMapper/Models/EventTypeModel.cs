using WorldEventMapper.ViewModels;

namespace WorldEventMapper.Models
{
    public class EventTypeModel : ObservableObject
    {
        private string _id = "";
        private string _name = "";
        private string _description = "";
        private string _iconPath = "";

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

        public string IconPath
        {
            get => _iconPath;
            set => SetProperty(ref _iconPath, value);
        }
    }
}