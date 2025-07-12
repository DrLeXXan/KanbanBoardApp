using System;
using KanbanBoardApp.Models;
using Xunit;

namespace KanbanBoardApp.Tests
{
    /// <summary>
    /// Unit tests for the UserActivityEntry model.
    /// Tests cover property assignment, mutability, edge cases, and value handling.
    /// </summary>
    public class UserActivityEntryTests
    {
        [Fact(DisplayName = "UT01: All properties can be set and retrieved correctly")]
        public void UserActivityEntry_Properties_AreSetAndGetCorrectly()
        {
            // Preconditions: None

            // Input/Action
            var now = DateTime.Now;
            var entry = new UserActivityEntry
            {
                Timestamp = now,
                PropertyChanged = "Title",
                OldValue = "Old",
                NewValue = "New",
                ChangedBy = "Tester"
            };

            // Expected Result
            Assert.Equal(now, entry.Timestamp);
            Assert.Equal("Title", entry.PropertyChanged);
            Assert.Equal("Old", entry.OldValue);
            Assert.Equal("New", entry.NewValue);
            Assert.Equal("Tester", entry.ChangedBy);

            // Actual result is verified by assertions above
        }

        [Fact(DisplayName = "UT02: Allows empty and long strings for string properties")]
        public void UserActivityEntry_Allows_Empty_And_Long_Strings()
        {
            // Preconditions: None

            // Input/Action
            var longString = new string('x', 10000);
            var entry = new UserActivityEntry
            {
                Timestamp = DateTime.MinValue,
                PropertyChanged = "",
                OldValue = longString,
                NewValue = "!@#$%^&*()_+",
                ChangedBy = ""
            };

            // Expected Result
            Assert.Equal(DateTime.MinValue, entry.Timestamp);
            Assert.Equal("", entry.PropertyChanged);
            Assert.Equal(longString, entry.OldValue);
            Assert.Equal("!@#$%^&*()_+", entry.NewValue);
            Assert.Equal("", entry.ChangedBy);

            // Actual result is verified by assertions above
        }

        [Fact(DisplayName = "UT03: Properties can be changed after construction")]
        public void UserActivityEntry_Properties_CanBeChangedAfterConstruction()
        {
            // Preconditions: Entry is constructed with initial values

            // Input/Action
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

            // Expected Result
            Assert.Equal("Comment", entry.PropertyChanged);
            Assert.Equal("Old", entry.OldValue);
            Assert.Equal("New", entry.NewValue);
            Assert.Equal("AnotherUser", entry.ChangedBy);

            // Actual result is verified by assertions above
        }

        [Fact(DisplayName = "UT04: Accepts DateTime edge values for Timestamp")]
        public void UserActivityEntry_Allows_DateTime_EdgeCases()
        {
            // Preconditions: None

            // Input/Action
            var entry = new UserActivityEntry
            {
                Timestamp = DateTime.MaxValue,
                PropertyChanged = "Test",
                OldValue = "Old",
                NewValue = "New",
                ChangedBy = "User"
            };

            // Expected Result
            Assert.Equal(DateTime.MaxValue, entry.Timestamp);

            // Actual result is verified by assertion above
        }
    }
}
