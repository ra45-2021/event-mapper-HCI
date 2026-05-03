using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WorldEventMapper.Help;
using WorldEventMapper.Home;
using WorldEventMapper.Models;
using WorldEventMapper.Services;

namespace WorldEventMapper.Event_Management
{
    public partial class EventManagement_Main : Window
    {
        private readonly JsonDataService _dataService = new JsonDataService();
        private AppData _data = new AppData();

        private string _selectedImageSourcePath = "";
        private string _storedImageRelativePath = "";

        private readonly List<string> _attendanceOptions = new()
        {
            "< 1000",
            "1000-5000",
            "5000-10000",
            "> 10000"
        };

        private readonly List<string> _locationOptions = new()
        {
            "Belgrade, Serbia",
            "Novi Sad, Serbia",
            "Niš, Serbia",
            "Mokra Gora, Serbia",
            "Sarajevo, Bosnia and Herzegovina",
            "Vienna, Austria",
            "Paris, France",
            "Rome, Italy",
            "Lisbon, Portugal",
            "Basel, Switzerland",
            "Tokyo, Japan",
            "Berlin, Germany",
            "Prague, Czech Republic",
            "Budapest, Hungary",
            "Madrid, Spain"
        };

        public EventManagement_Main()
        {
            InitializeComponent();
            LoadData();
            SetupForm();
        }

        private void LoadData()
        {
            _data = _dataService.Load();

            EventTypeComboBox.ItemsSource = _data.EventTypes;
            TagsListBox.ItemsSource = _data.EventTags;

            AttendanceComboBox.ItemsSource = _attendanceOptions;
            LocationComboBox.ItemsSource = _locationOptions;
        }

        private void SetupForm()
        {
            EventIdTextBox.Text = GenerateEventId();
            AttendanceComboBox.SelectedIndex = 0;
            UpcomingDatePicker.SelectedDate = DateTime.Today;
        }

        private string GenerateEventId()
        {
            int nextNumber = _data.Events.Count + 1;
            string id;

            do
            {
                id = $"EV{nextNumber:000}";
                nextNumber++;
            }
            while (_data.Events.Any(e => e.ID.Equals(id, StringComparison.OrdinalIgnoreCase)));

            return id;
        }

        private void SaveEvent_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            EventTypeModel selectedType = (EventTypeModel)EventTypeComboBox.SelectedItem;

            string iconPath = SaveUploadedImageIfNeeded();

            if (string.IsNullOrWhiteSpace(iconPath))
                iconPath = selectedType.IconPath;

            EventModel newEvent = new EventModel
            {
                ID = EventIdTextBox.Text.Trim(),
                Name = EventNameTextBox.Text.Trim(),
                Description = DescriptionTextBox.Text.Trim(),
                EventTypeId = selectedType.ID,
                Attendance = AttendanceComboBox.SelectedItem?.ToString() ?? "< 1000",
                IsHumanitarian = HumanitarianCheckBox.IsChecked == true,
                Cost = double.Parse(CostTextBox.Text.Trim(), CultureInfo.InvariantCulture),
                IconPath = iconPath,
                PastPerformingYears = ParseYears(PastYearsTextBox.Text),
                UpcomingDate = UpcomingDatePicker.SelectedDate ?? DateTime.Today,
                Location = LocationComboBox.Text.Trim(),
                TagIds = GetSelectedTagIds()
            };

            _data.Events.Add(newEvent);
            _dataService.Save(_data);

            MessageBox.Show("Event created successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);

            ClearAllFields();
            EventIdTextBox.Text = GenerateEventId();
            AttendanceComboBox.SelectedIndex = 0;
            UpcomingDatePicker.SelectedDate = DateTime.Today;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(EventIdTextBox.Text))
            {
                ShowValidationMessage("Event ID is required.");
                return false;
            }

            if (_data.Events.Any(e => e.ID.Equals(EventIdTextBox.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                ShowValidationMessage("Event ID already exists.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(EventNameTextBox.Text))
            {
                ShowValidationMessage("Event Name is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
            {
                ShowValidationMessage("Event Description is required.");
                return false;
            }

            if (EventTypeComboBox.SelectedItem == null)
            {
                ShowValidationMessage("Event Type is required.");
                return false;
            }

            if (AttendanceComboBox.SelectedItem == null)
            {
                ShowValidationMessage("Event Attendance is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(CostTextBox.Text))
            {
                ShowValidationMessage("Event Cost is required.");
                return false;
            }

            if (!double.TryParse(CostTextBox.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double cost) || cost < 0)
            {
                ShowValidationMessage("Event Cost must be a valid positive number. Use format like: 15000");
                return false;
            }

            if (!ValidateYears(PastYearsTextBox.Text))
            {
                ShowValidationMessage("Past Performing Years must be written like this: 2020,2021,2022");
                return false;
            }

            if (UpcomingDatePicker.SelectedDate == null)
            {
                ShowValidationMessage("Upcoming Date is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(LocationComboBox.Text))
            {
                ShowValidationMessage("Event Location is required.");
                return false;
            }

            if (GetSelectedTagIds().Count == 0)
            {
                ShowValidationMessage("Please select at least one Event Tag.");
                return false;
            }

            return true;
        }

        private void ShowValidationMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private bool ValidateYears(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;

            string[] parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                if (!int.TryParse(part.Trim(), out int year))
                    return false;

                if (year < 1900 || year > 2100)
                    return false;
            }

            return true;
        }

        private List<int> ParseYears(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<int>();

            return input
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x.Trim()))
                .ToList();
        }

        private List<string> GetSelectedTagIds()
        {
            List<string> selectedTagIds = new();

            foreach (EventTagModel tag in TagsListBox.SelectedItems)
                selectedTagIds.Add(tag.ID);

            return selectedTagIds;
        }

        private string SaveUploadedImageIfNeeded()
        {
            if (string.IsNullOrWhiteSpace(_selectedImageSourcePath))
                return "";

            string projectRoot = GetProjectRoot();

            string imagesFolder = Path.Combine(projectRoot, "Images");
            Directory.CreateDirectory(imagesFolder);

            string extension = Path.GetExtension(_selectedImageSourcePath);
            string safeEventId = EventIdTextBox.Text.Trim().Replace(" ", "_");
            string fileName = $"{safeEventId}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
            string destinationPath = Path.Combine(imagesFolder, fileName);

            File.Copy(_selectedImageSourcePath, destinationPath, true);

            _storedImageRelativePath = $"/Images/{fileName}";

            return _storedImageRelativePath;
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

        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select Event Image",
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (dialog.ShowDialog() == true)
            {
                _selectedImageSourcePath = dialog.FileName;
                ImagePathTextBlock.Text = _selectedImageSourcePath;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_selectedImageSourcePath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                SelectedImagePreview.Source = bitmap;
            }
        }

        private void EventTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_selectedImageSourcePath))
                return;

            if (EventTypeComboBox.SelectedItem is not EventTypeModel selectedType)
                return;

            ImagePathTextBlock.Text = $"No image selected. Default type icon will be used: {selectedType.IconPath}";
        }

        private void NewEvent_Click(object sender, RoutedEventArgs e)
        {
            ClearAllFields();
            EventIdTextBox.Text = GenerateEventId();
            AttendanceComboBox.SelectedIndex = 0;
            UpcomingDatePicker.SelectedDate = DateTime.Today;
        }

        private void ManageEvents_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Manage Events view will be added later.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ManageTags_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Manage Tags view will be added later.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ManageTypes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Manage Event Types view will be added later.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            ClearAllFields();
        }

        private void ClearAllFields()
        {
            EventIdTextBox.Clear();
            EventNameTextBox.Clear();
            DescriptionTextBox.Clear();

            EventTypeComboBox.SelectedItem = null;
            AttendanceComboBox.SelectedItem = null;

            HumanitarianCheckBox.IsChecked = false;
            CostTextBox.Clear();
            PastYearsTextBox.Clear();

            UpcomingDatePicker.SelectedDate = null;

            LocationComboBox.SelectedItem = null;
            LocationComboBox.Text = "";

            TagsListBox.SelectedItems.Clear();

            _selectedImageSourcePath = "";
            _storedImageRelativePath = "";

            SelectedImagePreview.Source = null;
            ImagePathTextBlock.Text = "No image selected. Type icon will be used.";
        }

        private void ClearEventId_Click(object sender, RoutedEventArgs e)
        {
            EventIdTextBox.Clear();
        }

        private void ClearEventName_Click(object sender, RoutedEventArgs e)
        {
            EventNameTextBox.Clear();
        }

        private void ClearDescription_Click(object sender, RoutedEventArgs e)
        {
            DescriptionTextBox.Clear();
        }

        private void ClearEventType_Click(object sender, RoutedEventArgs e)
        {
            EventTypeComboBox.SelectedItem = null;
            ImagePathTextBlock.Text = "No image selected. Type icon will be used.";
        }

        private void ClearAttendance_Click(object sender, RoutedEventArgs e)
        {
            AttendanceComboBox.SelectedItem = null;
        }

        private void ClearHumanitarian_Click(object sender, RoutedEventArgs e)
        {
            HumanitarianCheckBox.IsChecked = false;
        }

        private void ClearCost_Click(object sender, RoutedEventArgs e)
        {
            CostTextBox.Clear();
        }

        private void ClearYears_Click(object sender, RoutedEventArgs e)
        {
            PastYearsTextBox.Clear();
        }

        private void ClearDate_Click(object sender, RoutedEventArgs e)
        {
            UpcomingDatePicker.SelectedDate = null;
        }

        private void ClearLocation_Click(object sender, RoutedEventArgs e)
        {
            LocationComboBox.SelectedItem = null;
            LocationComboBox.Text = "";
        }

        private void ClearTags_Click(object sender, RoutedEventArgs e)
        {
            TagsListBox.SelectedItems.Clear();
        }

        private void ClearImage_Click(object sender, RoutedEventArgs e)
        {
            _selectedImageSourcePath = "";
            _storedImageRelativePath = "";

            SelectedImagePreview.Source = null;
            ImagePathTextBlock.Text = "No image selected. Type icon will be used.";
        }

        private void CloseToMain_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            this.Close();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.Content = new HomePage();

            this.Close();
        }

        private void EventList_Click(object sender, RoutedEventArgs e)
        {
            Event_List.EventList_Main eventListWindow = new Event_List.EventList_Main();
            eventListWindow.Show();

            this.Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help_Main helpWindow = new Help_Main();
            helpWindow.Show();

            this.Close();
        }
    }
}