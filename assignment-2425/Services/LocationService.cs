namespace assignment_2425.Services
{
    /// <summary>
    /// Service for handling location operations and GPS functionality
    /// </summary>
    public class LocationService
    {
        private static LocationService _instance;
        private static readonly object _lock = new object();

        private LocationService()
        {
        }

        /// <summary>
        /// Singleton instance of LocationService
        /// </summary>
        public static LocationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new LocationService();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Get the current GPS location of the device
        /// </summary>
        /// <returns>Current location or null if failed</returns>
        public async Task<Location?> GetCurrentLocationAsync()
        {
            try
            {
                // Check if location services are available
                if (!await IsLocationAvailableAsync())
                {
                    throw new FeatureNotSupportedException("Location services not available on this device");
                }

                // Request location with high accuracy
                var request = new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(10)
                };

                var location = await Geolocation.Default.GetLocationAsync(request);
                return location;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting location: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get the last known location of the device
        /// </summary>
        /// <returns>Last known location or null if not available</returns>
        public async Task<Location?> GetLastKnownLocationAsync()
        {
            try
            {
                var location = await Geolocation.Default.GetLastKnownLocationAsync();
                return location;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting last known location: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get address information from coordinates (reverse geocoding)
        /// </summary>
        /// <param name="location">GPS coordinates</param>
        /// <returns>List of possible addresses</returns>
        public async Task<IEnumerable<Placemark>?> GetAddressFromLocationAsync(Location location)
        {
            try
            {
                if (location == null)
                    return null;

                var placemarks = await Geocoding.Default.GetPlacemarksAsync(location.Latitude, location.Longitude);
                return placemarks;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting address from location: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get coordinates from an address (forward geocoding)
        /// </summary>
        /// <param name="address">Address string</param>
        /// <returns>List of possible locations</returns>
        public async Task<IEnumerable<Location>?> GetLocationFromAddressAsync(string address)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(address))
                    return null;

                var locations = await Geocoding.Default.GetLocationsAsync(address);
                return locations;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting location from address: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Format location information for display
        /// </summary>
        /// <param name="location">GPS location</param>
        /// <param name="placemark">Address information (optional)</param>
        /// <returns>Formatted location string</returns>
        public string FormatLocationForDisplay(Location location, Placemark? placemark = null)
        {
            try
            {
                if (location == null)
                    return "Unknown location";

                var locationText = $"📍 {location.Latitude:F6}, {location.Longitude:F6}";

                if (placemark != null)
                {
                    var addressParts = new List<string>();

                    if (!string.IsNullOrEmpty(placemark.FeatureName))
                        addressParts.Add(placemark.FeatureName);
                    
                    if (!string.IsNullOrEmpty(placemark.Thoroughfare))
                        addressParts.Add(placemark.Thoroughfare);
                    
                    if (!string.IsNullOrEmpty(placemark.Locality))
                        addressParts.Add(placemark.Locality);
                    
                    if (!string.IsNullOrEmpty(placemark.AdminArea))
                        addressParts.Add(placemark.AdminArea);

                    if (addressParts.Count > 0)
                    {
                        var address = string.Join(", ", addressParts.Take(3)); // Limit to first 3 parts
                        locationText = $"📍 {address}";
                    }
                }

                return locationText;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error formatting location: {ex.Message}");
                return $"📍 {location?.Latitude:F6}, {location?.Longitude:F6}";
            }
        }

        /// <summary>
        /// Check if location permission is granted
        /// </summary>
        /// <returns>True if permission is granted</returns>
        public async Task<bool> CheckLocationPermissionAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
                return status == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking location permission: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if location services are available on the device
        /// </summary>
        /// <returns>True if location services are available</returns>
        public async Task<bool> IsLocationAvailableAsync()
        {
            try
            {
                // Check if location permission is granted
                bool hasPermission = await CheckLocationPermissionAsync();
                if (!hasPermission)
                    return false;

                // Try to get last known location to test if location services work
                try
                {
                    var lastLocation = await Geolocation.Default.GetLastKnownLocationAsync();
                    return true; // If we can call the method, location services are available
                }
                catch (FeatureNotSupportedException)
                {
                    return false; // Location services not supported
                }
                catch
                {
                    return true; // Other errors don't mean location services are unavailable
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking location availability: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get current location with address information
        /// </summary>
        /// <returns>Location info with address details</returns>
        public async Task<LocationInfo?> GetCurrentLocationWithAddressAsync()
        {
            try
            {
                var location = await GetCurrentLocationAsync();
                if (location == null)
                    return null;

                var placemarks = await GetAddressFromLocationAsync(location);
                var placemark = placemarks?.FirstOrDefault();

                return new LocationInfo
                {
                    Location = location,
                    Placemark = placemark,
                    FormattedAddress = FormatLocationForDisplay(location, placemark),
                    Timestamp = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting location with address: {ex.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// Information about a location including GPS coordinates and address
    /// </summary>
    public class LocationInfo
    {
        public Location? Location { get; set; }
        public Placemark? Placemark { get; set; }
        public string FormattedAddress { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }

        public double? Latitude => Location?.Latitude;
        public double? Longitude => Location?.Longitude;
        public double? Accuracy => Location?.Accuracy;
        public double? Altitude => Location?.Altitude;

        public string CoordinatesText => Location != null 
            ? $"{Location.Latitude:F6}, {Location.Longitude:F6}" 
            : "Unknown";

        public string TimestampFormatted => Timestamp.ToString("MMM dd, yyyy HH:mm");
    }
}
