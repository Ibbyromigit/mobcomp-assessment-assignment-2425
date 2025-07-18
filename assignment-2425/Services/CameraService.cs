

namespace assignment_2425.Services
{
    /// <summary>
    /// Service for handling camera operations and photo storage
    /// </summary>
    public class CameraService
    {
        private static CameraService _instance;
        private static readonly object _lock = new object();

        private CameraService()
        {
        }

        /// <summary>
        /// Singleton instance of CameraService
        /// </summary>
        public static CameraService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new CameraService();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Take a photo using the device camera
        /// </summary>
        /// <returns>Path to the captured photo or null if failed</returns>
        public async Task<string?> TakePhotoAsync()
        {
            try
            {
                // Check if camera is available
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    throw new FeatureNotSupportedException("Camera not supported on this device");
                }

                // Request camera photo
                var photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // Save the photo to local app data
                    var localFilePath = await SavePhotoAsync(photo);
                    return localFilePath;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error taking photo: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Pick a photo from the device gallery
        /// </summary>
        /// <returns>Path to the selected photo or null if cancelled</returns>
        public async Task<string?> PickPhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();

                if (photo != null)
                {
                    // Save the photo to local app data
                    var localFilePath = await SavePhotoAsync(photo);
                    return localFilePath;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error picking photo: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Save a photo to local app storage
        /// </summary>
        /// <param name="photo">The photo file result</param>
        /// <returns>Local file path</returns>
        private async Task<string> SavePhotoAsync(FileResult photo)
        {
            try
            {
                // Create a unique filename
                var fileName = $"meal_photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                var localFilePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                // Copy the photo to local storage
                using var sourceStream = await photo.OpenReadAsync();
                using var localFileStream = File.Create(localFilePath);
                await sourceStream.CopyToAsync(localFileStream);

                return localFilePath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving photo: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get photo file info
        /// </summary>
        /// <param name="filePath">Path to the photo file</param>
        /// <returns>File info or null if file doesn't exist</returns>
        public PhotoInfo? GetPhotoInfo(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    return null;

                var fileInfo = new FileInfo(filePath);
                return new PhotoInfo
                {
                    FilePath = filePath,
                    FileName = fileInfo.Name,
                    FileSize = fileInfo.Length,
                    DateTaken = fileInfo.CreationTime
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting photo info: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Delete a photo file
        /// </summary>
        /// <param name="filePath">Path to the photo file</param>
        /// <returns>True if deleted successfully</returns>
        public bool DeletePhoto(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    return false;

                File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting photo: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if camera permission is granted
        /// </summary>
        /// <returns>True if permission is granted</returns>
        public async Task<bool> CheckCameraPermissionAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                }
                return status == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking camera permission: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if storage permission is granted
        /// </summary>
        /// <returns>True if permission is granted</returns>
        public async Task<bool> CheckStoragePermissionAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                }
                return status == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking storage permission: {ex.Message}");
                return false;
            }
        }
    }

    /// <summary>
    /// Information about a photo file
    /// </summary>
    public class PhotoInfo
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime DateTaken { get; set; }

        public string FileSizeFormatted
        {
            get
            {
                if (FileSize < 1024)
                    return $"{FileSize} B";
                else if (FileSize < 1024 * 1024)
                    return $"{FileSize / 1024:F1} KB";
                else
                    return $"{FileSize / (1024 * 1024):F1} MB";
            }
        }

        public string DateTakenFormatted => DateTaken.ToString("MMM dd, yyyy HH:mm");
    }
}
