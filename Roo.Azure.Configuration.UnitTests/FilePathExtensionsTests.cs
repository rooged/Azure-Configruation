using Roo.Azure.Configuration.Common.Utilities.Extensions;
using System.Runtime.InteropServices;

namespace Roo.Azure.Configuration.UnitTests
{
    public class FilePathExtensionsTests
    {
        [Test]
        [TestCase("fileName")]
        [TestCase("file:Name")]
        public void CheckValidFileName_Verify(string fileName)
        {
            //Arrange
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            //Act
            var result = FilePathExtensions.CheckValidFileName(fileName);

            //Assert
            if (fileName.Equals("fileName"))
            {
                Assert.That(result, Is.True);
            }
            else
            {
                Assert.That(result, Is.False);
            }
        }

        [Test]
        [TestCase("test/file-path/test")]
        [TestCase("test/file|path/test")]
        public void CheckValidFilePath_Verify(string path)
        {
            //Arrange
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            //Act
            var result = FilePathExtensions.CheckValidFilePath(path);

            //Assert
            if (path.Equals("test/file-path/test"))
            {
                Assert.That(result, Is.True);
            }
            else
            {
                Assert.That(result, Is.False);
            }
        }

        [Test]
        [TestCase("path")]
        [TestCase("")]
        public void GetBasePath_Verify(string path)
        {
            //Act
            var result = FilePathExtensions.GetBasePath("test/", path);

            //Assert
            if (path.Equals("path"))
            {
                Assert.That(result, Is.EqualTo("test/path"));
            }
            else
            {
                Assert.That(result, Is.EqualTo(string.Empty));
            }
        }

        [Test]
        public void BuildValidPathFromAllowedPath_Verify()
        {
            //Act
            var result = FilePathExtensions.BuildValidPathFromAllowedPath("test", "path");

            //Assert
            if (OperatingSystem.IsWindows())
            {
                Assert.That(result, Contains.Substring("path\\test"));
            }
            else
            {
                Assert.That(result, Contains.Substring("path/test"));
            }
        }

        [Test]
        public void BuildValidPathFromAllowedPathPathTraversal_Verify()
        {
            //Arrange
            var path = Path.Combine("..", "Windows", "system32");

            //Act & Assert
            Assert.Throws<ArgumentException>(() => FilePathExtensions.BuildValidPathFromAllowedPath(path, "allowedPath"));
        }

        [Test]
        public void BuildValidPathFromAllowedPathReservedNames_Verify()
        {
            //Arrange
            var path = "CON.txt";

            //Act & Assert
            if (OperatingSystem.IsWindows())
            {
                Assert.Throws<ArgumentException>(() => FilePathExtensions.BuildValidPathFromAllowedPath(path, "allowedPath"));
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                var result = FilePathExtensions.BuildValidPathFromAllowedPath(path, "allowedPath");
                Assert.That(result, Does.EndWith("CON.txt"));
            }
        }

        [Test]
        public void BuildValidPathFromAllowedPathSymLink_Verify()
        {
            //Arrange
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            var targetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(targetDirectory);
            var symlink = Path.Combine(tempDirectory, "symlink");

            //Act
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    try
                    {
                        Directory.CreateSymbolicLink(symlink, targetDirectory);
                    }
                    catch
                    {
                        //Doesn't have privilege to create symbolic links, skipping test
                        return;
                    }
                }
                var path = Path.Combine("symlink", "file.txt");
                Assert.Throws<ArgumentException>(() => FilePathExtensions.BuildValidPathFromAllowedPath(path, tempDirectory));
            }
            finally
            {
                if (Directory.Exists(symlink))
                {
                    Directory.Delete(symlink);
                }
                if (Directory.Exists(targetDirectory))
                {
                    Directory.Delete(targetDirectory);
                }
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory);
                }
            }
        }

        [Test]
        public void BuildValidPathFromAllowedPathAbsolutePath_Verify()
        {
            //Arrange
            var basePath = Path.GetTempPath();
            var path = Path.GetFullPath(Path.Combine(basePath, "file.txt"));

            //Act & Assert
            Assert.Throws<ArgumentException>(() => FilePathExtensions.BuildValidPathFromAllowedPath(path, basePath));
        }

        [Test]
        [TestCase("https://unittest.com")]
        [TestCase("unittest.path")]
        public void ValidateUrl_Verify(string url)
        {
            //Act
            var result = FilePathExtensions.ValidateUrl(url);

            //Assert
            if (url.Equals("https://unittest.com"))
            {
                Assert.That(result, Is.True);
            }
            else
            {
                Assert.That(result, Is.False);
            }
        }
    }
}