namespace Roo.Azure.Configuration.Common.Utilities.Extensions
{
    /// <summary>
    /// File path extension methods. Not really an extension since it's not a string extension.
    /// </summary>
    public static class FilePathExtensions
    {
        /// <summary>
        /// Checks if the string includes any invalid file name characters. If fileName is null, empty, or whitespaces then it returns false.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool CheckValidFileName(string fileName)
        {
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1 || string.IsNullOrWhiteSpace(fileName))
            {
                return false; 
            }
            return true;
        }

        /// <summary>
        /// Checks if the string includes any invalid file path characters. If path is null, empty, or whitespaces then it returns false.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckValidFilePath(string path)
        {
            if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1 || string.IsNullOrWhiteSpace(path))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the absolute path based on the base directory. If path is null, empty, or whitespaces then it returns string.Empty.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));
        }

        /// <summary>
        /// Generates the base path with base path and a non-absolute path. Do not use with user inputs. If basePath is null, empty, or whitespaces then it returns string.Empty.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetBasePath(string basePath, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }
            return Path.Combine(basePath, path);
        }

        /// <summary>
        /// Builds a safe, valid path with allowPath as the base.
        /// </summary>
        /// <param name="path">Pathway to build for.</param>
        /// <param name="allowedPath">Valid base path to build from.</param>
        /// <param name="additionalReservedNames">Additional reserved device or data stream names to account for. CON, PRN, AUX, NUL, COM1, and LPT1 already are checked.</param>
        /// <returns></returns>
        public static string BuildValidPathFromAllowedPath(string path, string allowedPath, List<string>? additionalReservedNames = null)
        {
            if (string.IsNullOrWhiteSpace(path) || Path.IsPathRooted(path))
            {
                throw new ArgumentException($"path must be an absolute, trusted path.");
            }
            if (string.IsNullOrWhiteSpace(allowedPath) || Path.IsPathRooted(allowedPath))
            {
                throw new ArgumentException($"allowedPath must be an absolute, trusted path.");
            }

            var isWindows = OperatingSystem.IsWindows();
            var pathComparison = isWindows ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            if (IsLocalPath(allowedPath))
            {
                var basePath = GetFullPath(allowedPath);

                //Reserved device name and data streams check, only needed for Windows
                if (isWindows)
                {
                    var fileName = Path.GetFileName(path);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                    var reservedNames = new string[] { "CON", "PRN", "AUX", "NUL", "COM1", "LPT1" };
                    if (fileName.Contains(':') || reservedNames.Any(x => string.Equals(fileNameWithoutExtension, x, StringComparison.OrdinalIgnoreCase)) || (additionalReservedNames != null && additionalReservedNames.Any(x => string.Equals(fileNameWithoutExtension, x, StringComparison.OrdinalIgnoreCase))))
                    {
                        throw new ArgumentException($"Path contains reserved device names or data streams.");
                    }
                }
                if (!CheckValidFilePath(allowedPath))
                {
                    throw new ArgumentException($"Allowed path contains invalid characters.");
                }

                var combinedPath = Path.Combine(basePath, path);
                var absolutePath = Path.GetFullPath(combinedPath);

                //Handle Symlink
                var resolvedBasePath = new DirectoryInfo(basePath).FullName;
                var resolvedAbsolutePath = Directory.Exists(absolutePath) ? new DirectoryInfo(absolutePath).FullName : new FileInfo(absolutePath).FullName;

                if (!resolvedAbsolutePath.StartsWith(resolvedBasePath, pathComparison))
                {
                    throw new ArgumentException($"Invalid path.");
                }
                return absolutePath;
            }
            var baseUri = new Uri(allowedPath, UriKind.Absolute);
            var uri = new Uri(baseUri, path);

            //Compare scheme and host server & ensure target path is within base path
            if (!Uri.Compare(baseUri, uri, UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.SafeUnescaped, pathComparison).Equals(0) || !uri.AbsoluteUri.StartsWith(baseUri.AbsoluteUri, pathComparison) || !ValidateUrl(uri.AbsoluteUri))
            {
                throw new ArgumentException($"Invalid url.");
            }
            return uri.AbsoluteUri;
        }

        /// <summary>
        /// Checks if a URL is valid and matches the http or https scheme.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool ValidateUrl(string url)
        {
            if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out var result))
            {
                return result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps;
            }
            return false;
        }

        private static bool IsLocalPath(string path)
        {
            if (!string.IsNullOrEmpty(path) && Uri.TryCreate(path, UriKind.Absolute, out var result))
            {
                return result.IsFile;
            }
            return true;
        }
    }
}
