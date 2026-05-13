using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WorldEventMapper.Help;
using WorldEventMapper.Home;
using WorldEventMapper.Models;
using WorldEventMapper.Services;
using WorldEventMapper.Events;
using WorldEventMapper.EventTags;
using WorldEventMapper.EventTypes;

namespace WorldEventMapper.Events
{
    public partial class Events_Add : Window
    {
        private readonly JsonDataService _dataService = new JsonDataService();
        private AppData _data = new AppData();

        private string _selectedImageSourcePath = "";
        private string _storedImageRelativePath = "";
        private bool _isFilteringLocation = false;
        private bool _isFormattingPastYears = false;
        private readonly DispatcherTimer _toastTimer = new DispatcherTimer();
        private bool _isClearingForm = false;


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

        public Events_Add()
        {
            InitializeComponent();
            LoadData();
            SetupForm();
            ConfigureUpcomingDatePicker();

            _toastTimer.Interval = TimeSpan.FromSeconds(3);
            _toastTimer.Tick += ToastTimer_Tick;
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
                IsHumanitarian = HumanitarianYesToggle.IsChecked == true,
                Cost = double.Parse(CostTextBox.Text.Trim(), CultureInfo.InvariantCulture),
                IconPath = iconPath,
                PastPerformingYears = ParseYears(PastYearsTextBox.Text),
                UpcomingDate = UpcomingDatePicker.SelectedDate ?? DateTime.Today,
                Location = LocationComboBox.Text.Trim(),
                TagIds = GetSelectedTagIds()
            };

            _data.Events.Add(newEvent);
            _dataService.Save(_data);

            ShowSuccessToast("Event created successfully.");

            ClearAllFields();
            EventIdTextBox.Text = GenerateEventId();
            AttendanceComboBox.SelectedIndex = 0;
            UpcomingDatePicker.SelectedDate = DateTime.Today;
        }

        private void ShowSuccessToast(string message)
        {
            ShowToast("SUCCESS", message, true);
        }

        private void ShowErrorToast(string message)
        {
            ShowToast("ERROR", message, false);
        }

        private void ShowToast(string title, string message, bool isSuccess)
        {
            _toastTimer.Stop();

            ToastTitleTextBlock.Text = title;
            ToastMessageTextBlock.Text = message;

            if (isSuccess)
            {
                ToastBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(220, 252, 231));
                ToastBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(134, 239, 172));

                ToastIconCircle.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 197, 94));
                ToastIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Check;

                ToastTitleTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(22, 101, 52));
                ToastMessageTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(20, 83, 45));
            }
            else
            {
                ToastBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 226, 226));
                ToastBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(252, 165, 165));

                ToastIconCircle.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(239, 68, 68));
                ToastIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Exclamation;

                ToastTitleTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(153, 27, 27));
                ToastMessageTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(127, 29, 29));
            }

            ToastBorder.Visibility = Visibility.Visible;
            _toastTimer.Start();
        }

        private void ToastTimer_Tick(object? sender, EventArgs e)
        {
            _toastTimer.Stop();
            ToastBorder.Visibility = Visibility.Collapsed;
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
                ShowValidationMessage("Event Cost must be a valid positive number. Use format like: 15000 or 15000.05");
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

            return true;
        }

        private void ShowValidationMessage(string message)
        {
            ShowErrorToast(message);
        }

        private void LocationComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (LocationComboBox.Template.FindName("PART_EditableTextBox", LocationComboBox) is TextBox textBox)
            {
                textBox.TextChanged += LocationSearchTextBox_TextChanged;
            }
        }

        private void ConfigureUpcomingDatePicker()
        {
            DateTime today = DateTime.Today;
            DateTime endOfThisYear = new DateTime(today.Year, 12, 31);

            UpcomingDatePicker.DisplayDateStart = today;
            UpcomingDatePicker.DisplayDateEnd = endOfThisYear;
            UpcomingDatePicker.DisplayDate = today;

            if (UpcomingDatePicker.SelectedDate < today || UpcomingDatePicker.SelectedDate > endOfThisYear)
            {
                UpcomingDatePicker.SelectedDate = null;
            }
        }

        private void LocationSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isFilteringLocation || _isClearingForm)
                return;

            string searchText = LocationComboBox.Text;

            _isFilteringLocation = true;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                LocationComboBox.ItemsSource = _locationOptions;
                LocationComboBox.IsDropDownOpen = false;
            }
            else
            {
                LocationComboBox.ItemsSource = _locationOptions
                    .Where(location => location.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                LocationComboBox.IsDropDownOpen = true;
            }

            LocationComboBox.Text = searchText;

            if (LocationComboBox.Template.FindName("PART_EditableTextBox", LocationComboBox) is TextBox textBox)
            {
                textBox.CaretIndex = textBox.Text.Length;
            }

            _isFilteringLocation = false;
        }

        private void PastYearsTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void PastYearsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isFormattingPastYears)
                return;

            _isFormattingPastYears = true;

            string digitsOnly = new string(PastYearsTextBox.Text.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length > 20)
                digitsOnly = digitsOnly.Substring(0, 20);

            List<string> groups = new();

            for (int i = 0; i < digitsOnly.Length; i += 4)
            {
                int length = Math.Min(4, digitsOnly.Length - i);
                groups.Add(digitsOnly.Substring(i, length));
            }

            string formatted = string.Join(",", groups);

            PastYearsTextBox.Text = formatted;
            PastYearsTextBox.CaretIndex = PastYearsTextBox.Text.Length;

            _isFormattingPastYears = false;
        }

        private void PastYearsTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            string pastedText = e.DataObject.GetData(typeof(string)) as string ?? "";

            if (!pastedText.All(c => char.IsDigit(c) || c == ','))
            {
                e.CancelCommand();
            }
        }

        private bool ValidateYears(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;

            string trimmed = input.Trim();

            if (!System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"^\d{4}(,\d{4})*$"))
                return false;

            string[] years = trimmed.Split(',');

            foreach (string yearText in years)
            {
                int year = int.Parse(yearText);

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

            _storedImageRelativePath = destinationPath;

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
                ImagePathTextBlock.Text = Path.GetFileName(_selectedImageSourcePath);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_selectedImageSourcePath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                SelectedImagePreview.Source = bitmap;
                ImagePlaceholderIcon.Visibility = Visibility.Collapsed;
            }
        }

        private void EventTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_selectedImageSourcePath))
                return;

            if (EventTypeComboBox.SelectedItem is not EventTypeModel selectedType)
                return;

            ImagePathTextBlock.Text = $"No image selected. Default type icon will be used.";
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
            _isClearingForm = true;

            EventIdTextBox.Clear();
            EventNameTextBox.Clear();
            DescriptionTextBox.Clear();

            EventTypeComboBox.SelectedItem = null;
            AttendanceComboBox.SelectedItem = null;

            HumanitarianYesToggle.IsChecked = false;
            HumanitarianNoToggle.IsChecked = false;
            CostTextBox.Clear();
            PastYearsTextBox.Clear();

            UpcomingDatePicker.SelectedDate = null;

            LocationComboBox.ItemsSource = _locationOptions;
            LocationComboBox.SelectedItem = null;
            LocationComboBox.Text = "";
            LocationComboBox.IsDropDownOpen = false;

            TagsListBox.SelectedItems.Clear();

            _selectedImageSourcePath = "";
            _storedImageRelativePath = "";

            SelectedImagePreview.Source = null;
            ImagePlaceholderIcon.Visibility = Visibility.Visible;
            ImagePathTextBlock.Text = "Upload event image";

            _isClearingForm = false;
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
            ImagePathTextBlock.Text = "Upload event image";
        }

        private void ClearAttendance_Click(object sender, RoutedEventArgs e)
        {
            AttendanceComboBox.SelectedItem = null;
        }

        private void ClearHumanitarian_Click(object sender, RoutedEventArgs e)
        {
            HumanitarianYesToggle.IsChecked = false;
            HumanitarianNoToggle.IsChecked = false;
        }

        private void HumanitarianYesToggle_Checked(object sender, RoutedEventArgs e)
        {
            HumanitarianNoToggle.IsChecked = false;
        }

        private void HumanitarianNoToggle_Checked(object sender, RoutedEventArgs e)
        {
            HumanitarianYesToggle.IsChecked = false;
        }

        private void ClearCost_Click(object sender, RoutedEventArgs e)
        {
            CostTextBox.Clear();
        }

        private void CostTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            string newText = textBox.Text
                .Remove(textBox.SelectionStart, textBox.SelectionLength)
                .Insert(textBox.SelectionStart, e.Text);

            e.Handled = !IsValidFullCostValue(newText);
        }

        private void CostTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CostTextBox.Text))
                return;

            string fixedText = FixCostInput(CostTextBox.Text);

            if (CostTextBox.Text != fixedText)
            {
                int caretIndex = CostTextBox.CaretIndex;

                CostTextBox.Text = fixedText;
                CostTextBox.CaretIndex = Math.Min(caretIndex, CostTextBox.Text.Length);
            }
        }

        private void CostTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            TextBox textBox = (TextBox)sender;
            string pastedText = e.DataObject.GetData(typeof(string)) as string ?? "";

            string newText = textBox.Text
                .Remove(textBox.SelectionStart, textBox.SelectionLength)
                .Insert(textBox.SelectionStart, pastedText);

            if (!IsValidFullCostValue(newText))
                e.CancelCommand();
        }

        private bool IsValidFullCostValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            value = value.Replace(",", ".");

            return Regex.IsMatch(value, @"^\d+(\.\d{0,2})?$|^\d*\.$");
        }

        private string FixCostInput(string value)
        {
            value = value.Replace(",", ".");

            string result = "";
            bool hasDecimalPoint = false;
            int decimalCount = 0;

            foreach (char c in value)
            {
                if (char.IsDigit(c))
                {
                    if (hasDecimalPoint)
                    {
                        if (decimalCount < 2)
                        {
                            result += c;
                            decimalCount++;
                        }
                    }
                    else
                    {
                        result += c;
                    }
                }
                else if (c == '.' && !hasDecimalPoint)
                {
                    result += c;
                    hasDecimalPoint = true;
                }
            }

            return result;
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
            LocationComboBox.ItemsSource = _locationOptions;
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
            ImagePlaceholderIcon.Visibility = Visibility.Visible;
            ImagePathTextBlock.Text = "Upload event image";
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

            Close();
        }

        private void Events_Click(object sender, RoutedEventArgs e)
        {
            Events_Main eventsWindow = new Events_Main();
            eventsWindow.Show();

            Close();
        }

        private void EventTypes_Click(object sender, RoutedEventArgs e)
        {
            EventTypes_Main eventTypesWindow = new EventTypes_Main();
            eventTypesWindow.Show();

            Close();
        }

        private void EventTags_Click(object sender, RoutedEventArgs e)
        {
            EventTags_Main eventTagsWindow = new EventTags_Main();
            eventTagsWindow.Show();

            Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help_Main helpWindow = new Help_Main();
            helpWindow.Show();

            Close();
        }

    }
}