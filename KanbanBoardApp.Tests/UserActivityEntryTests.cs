using System;
using KanbanBoardApp.Models;
using Xunit;

namespace KanbanBoardApp.Tests
{
    /// <summary>
    /// Unit tests for the UserActivityEntry model.
    /// Covers property assignment, mutability, edge cases, and value handling.
    /// </summary>
    public class UserActivityEntryTests
    {
        /// <summary>
        /// Verifies that all properties of UserActivityEntry can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void UserActivityEntry_Properties_AreSetAndGetCorrectly()
        {
            var now = DateTime.Now;
            var entry = new UserActivityEntry
            {
                Timestamp = now,
                PropertyChanged = "Title",
                OldValue = "Old",
                NewValue = "New",
                ChangedBy = "Tester"
            };

            Assert.Equal(now, entry.Timestamp);
            Assert.Equal("Title", entry.PropertyChanged);
            Assert.Equal("Old", entry.OldValue);
            Assert.Equal("New", entry.NewValue);
            Assert.Equal("Tester", entry.ChangedBy);
        }

        /// <summary>
        /// Ensures that empty strings and very long strings are accepted for string properties.
        /// </summary>
        [Fact]
        public void UserActivityEntry_Allows_Empty_And_Long_Strings()
        {
            var longString = new string('x', 10000);
            var entry = new UserActivityEntry
            {
                Timestamp = DateTime.MinValue,
                PropertyChanged = "",
                OldValue = longString,
                NewValue = "!@#$%^&*()_+",
                ChangedBy = ""
            };

            Assert.Equal(DateTime.MinValue, entry.Timestamp);
            Assert.Equal("", entry.PropertyChanged);
            Assert.Equal(longString, entry.OldValue);
            Assert.Equal("!@#$%^&*()_+", entry.NewValue);
            Assert.Equal("", entry.ChangedBy);
        }

        /// <summary>
        /// Verifies that properties can be changed after construction and the changes are reflected.
        /// </summary>
        [Fact]
        public void UserActivityEntry_Properties_CanBeChangedAfterConstruction()
        {
            var entry = new UserActivityEntry
            {
                Timestamp = DateTime.Now,
                PropertyChanged = "Title",
                OldValue = "A",
                NewValue = "B",
                ChangedBy = "User"
            };

            entry.PropertyChanged = "Comment";
            entry.OldValue = "Old";
            entry.NewValue = "New";
            entry.ChangedBy = "AnotherUser";

            Assert.Equal("Comment", entry.PropertyChanged);
            Assert.Equal("Old", entry.OldValue);
            Assert.Equal("New", entry.NewValue);
            Assert.Equal("AnotherUser", entry.ChangedBy);
        }

        /// <summary>
        /// Ensures that DateTime edge values (e.g., DateTime.MaxValue) are accepted for the Timestamp property.
        /// </summary>
        [Fact]
        public void UserActivityEntry_Allows_DateTime_EdgeCases()
        {
            var entry = new UserActivityEntry
            {
                Timestamp = DateTime.MaxValue,
                PropertyChanged = "Test",
                OldValue = "Old",
                NewValue = "New",
                ChangedBy = "User"
            };

            Assert.Equal(DateTime.MaxValue, entry.Timestamp);
        }
    }
}
