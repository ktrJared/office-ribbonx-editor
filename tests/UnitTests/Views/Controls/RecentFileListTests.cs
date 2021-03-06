﻿using System.Collections.Generic;
using System.IO;
using System.Threading;
using NUnit.Framework;
using OfficeRibbonXEditor.Views.Controls;

namespace OfficeRibbonXEditor.UnitTests.Views.Controls
{
    [Apartment(ApartmentState.STA)]
    public sealed class RecentFileListTests
    {
        private string? filePath;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable. Always defined in SetUp
        private RecentFileList control;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        [SetUp]
        public void SetUp()
        {
            string path;
            do
            {
                path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            } while (File.Exists(path));
            this.filePath = path;

            this.control = new RecentFileList();
            this.control.UseXmlPersister(this.filePath);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(this.filePath);
        }

        [Test]
        public void CanAddFiles()
        {
            using (var stream = TempFile())
            {
                // Arrange
                Assume.That(this.control.RecentFiles, Is.Empty);

                // Act
                this.control.InsertFile(stream.Name);

                // Assert
                Assert.Contains(stream.Name, this.control?.RecentFiles);
            }
        }


        [Test]
        public void CanRemoveFiles()
        {
            using (var stream = TempFile())
            {
                // Arrange
                Assume.That(this.control.RecentFiles, Is.Empty);

                // Act
                this.control.InsertFile(stream.Name);
                this.control.RemoveFile(stream.Name);

                // Assert
                Assert.IsEmpty(this.control.RecentFiles);
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public void CannotPassMaxLimit(int maxFiles)
        {
            // Arrange
            this.control.MaxNumberOfFiles = maxFiles;
            var collection = new List<FileStream>(maxFiles + 5);
            for (var i = 0; i < maxFiles + 5; ++i)
            {
                collection.Add(TempFile());
            }

            // Act
            foreach (var stream in collection)
            {
                this.control.InsertFile(stream.Name);
            }

            // Assert
            try
            {
                Assert.AreEqual(maxFiles, this.control.RecentFiles.Count);
            }
            finally
            {
                foreach (var stream in collection)
                {
                    stream.Close();
                }
            }
        }

        private static FileStream TempFile()
        {
            return new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose);
        }
    }
}
