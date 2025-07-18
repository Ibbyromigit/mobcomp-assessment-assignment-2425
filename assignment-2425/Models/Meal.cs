using System.ComponentModel;
using SQLite;

namespace assignment_2425.Models
{
    /// <summary>
    /// Represents a meal entry in the meal tracker app
    /// </summary>
    [Table("Meals")]
    public class Meal : INotifyPropertyChanged
    {
        private string _id;
        private string _name;
        private DateTime _mealTime;
        private string _notes;
        private string _imagePath;
        private string _location;
        private string _category;

        public Meal()
        {
            Id = Guid.NewGuid().ToString();
            MealTime = DateTime.Now;
            Name = string.Empty;
            Notes = string.Empty;
            ImagePath = string.Empty;
            Location = string.Empty;
            Category = "Meal";
        }

        /// <summary>
        /// Unique identifier for the meal
        /// </summary>
        [PrimaryKey]
        public string Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        /// <summary>
        /// Name of the meal (e.g., "Breakfast", "Lunch", "Dinner", or custom name)
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Date and time when the meal was consumed
        /// </summary>
        public DateTime MealTime
        {
            get => _mealTime;
            set
            {
                if (_mealTime != value)
                {
                    _mealTime = value;
                    OnPropertyChanged(nameof(MealTime));
                    OnPropertyChanged(nameof(MealTimeFormatted));
                }
            }
        }

        /// <summary>
        /// Additional notes about the meal
        /// </summary>
        public string Notes
        {
            get => _notes;
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    OnPropertyChanged(nameof(Notes));
                }
            }
        }

        /// <summary>
        /// Path to the meal image (if captured)
        /// </summary>
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                    OnPropertyChanged(nameof(HasImage));
                }
            }
        }

        /// <summary>
        /// Location where the meal was consumed
        /// </summary>
        public string Location
        {
            get => _location;
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }

        /// <summary>
        /// Category of the meal (Breakfast, Lunch, Dinner, Snack)
        /// </summary>
        public string Category
        {
            get => _category;
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged(nameof(Category));
                }
            }
        }

        /// <summary>
        /// Formatted meal time for display
        /// </summary>
        public string MealTimeFormatted => MealTime.ToString("MMM dd, yyyy - HH:mm");

        /// <summary>
        /// Indicates whether the meal has an associated image
        /// </summary>
        public bool HasImage => !string.IsNullOrEmpty(ImagePath);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
