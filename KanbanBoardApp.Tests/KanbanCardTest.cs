using System;
using System.Collections.ObjectModel;
using KanbanBoardApp.Models;
using Xunit;

namespace KanbanBoardApp.Tests
{
    /// <summary>
    /// Unit tests for the KanbanCard model.
    /// Covers property assignment, cloning, updating, ID management, and history functionality.
    /// </summary>
    public class KanbanCardTests
    {
        [Fact(DisplayName = "UT17: All properties can be set and retrieved correctly")]
        public void Properties_AreSetAndGetCorrectly()
        {
            // Preconditions: None

            // Input/Action
            var now = DateTime.Today;
            var card = new KanbanCard
            {
                Id = 42,
                Title = "Test Card",
                Owner = "Tester",
                Description = "Description",
                Urgency = "High",
                Status = "To Do",
                DueDate = now,
                Comment = "Some comment"
            };

            // Expected Result
            Assert.Equal(42, card.Id);
            Assert.Equal("Test Card", card.Title);
            Assert.Equal("Tester", card.Owner);
            Assert.Equal("Description", card.Description);
            Assert.Equal("High", card.Urgency);
            Assert.Equal("To Do", card.Status);
            Assert.Equal(now, card.DueDate);
            Assert.Equal("Some comment", card.Comment);

            // Actual result is verified by assertions above
        }

        [Fact(DisplayName = "UT18: Clone creates a deep copy including history")]
        public void Clone_CreatesDeepCopy()
        {
            // Preconditions: KanbanCard with populated History

            // Input/Action
            var card = new KanbanCard
            {
                Id = 1,
                Title = "Original",
                Owner = "Owner",
                Description = "Desc",
                Urgency = "Medium",
                Status = "To Do",
                DueDate = DateTime.Today,
                Comment = "Comment",
                History = new ObservableCollection<UserActivityEntry>
                {
                    new UserActivityEntry
                    {
                        Timestamp = DateTime.Now,
                        PropertyChanged = "Title",
                        OldValue = "Old",
                        NewValue = "Original",
                        ChangedBy = "User"
                    }
                }
            };

            var clone = card.Clone();

            // Expected Result
            Assert.NotSame(card, clone);
            Assert.Equal(card.Id, clone.Id);
            Assert.Equal(card.Title, clone.Title);
            Assert.Equal(card.Owner, clone.Owner);
            Assert.Equal(card.Description, clone.Description);
            Assert.Equal(card.Urgency, clone.Urgency);
            Assert.Equal(card.Status, clone.Status);
            Assert.Equal(card.DueDate, clone.DueDate);
            Assert.Equal(card.Comment, clone.Comment);
            Assert.NotSame(card.History, clone.History);
            Assert.Equal(card.History.Count, clone.History.Count);
            Assert.Equal(card.History[0].PropertyChanged, clone.History[0].PropertyChanged);

            // Actual result is verified by assertions above
        }

        [Fact(DisplayName = "UT19: UpdateFrom copies all properties from another KanbanCard")]
        public void UpdateFrom_UpdatesAllProperties()
        {
            // Preconditions: Two KanbanCard instances with different property values

            // Input/Action
            var card = new KanbanCard
            {
                Title = "Old",
                Owner = "A",
                Description = "B",
                Urgency = "Low",
                Status = "To Do",
                DueDate = DateTime.Today,
                Comment = "C"
            };
            var other = new KanbanCard
            {
                Title = "New",
                Owner = "B",
                Description = "D",
                Urgency = "High",
                Status = "Done",
                DueDate = DateTime.Today.AddDays(1),
                Comment = "E"
            };

            card.UpdateFrom(other);

            // Expected Result
            Assert.Equal("New", card.Title);
            Assert.Equal("B", card.Owner);
            Assert.Equal("D", card.Description);
            Assert.Equal("High", card.Urgency);
            Assert.Equal("Done", card.Status);
            Assert.Equal(DateTime.Today.AddDays(1), card.DueDate);
            Assert.Equal("E", card.Comment);

            // Actual result is verified by assertions above
        }

        [Fact(DisplayName = "UT20: PeekNextId returns the current next ID without incrementing")]
        public void PeekNextId_ReturnsCurrentNextId()
        {
            // Preconditions: None

            // Input/Action
            int nextId = KanbanCard.PeekNextId();
            int nextIdAgain = KanbanCard.PeekNextId();

            // Expected Result
            Assert.Equal(nextId, nextIdAgain);

            // Actual result is verified by assertion above
        }

        [Fact(DisplayName = "UT21: GetNextId returns and increments the next available ID")]
        public void GetNextId_IncrementsId()
        {
            // Preconditions: None

            // Input/Action
            int before = KanbanCard.PeekNextId();
            int got = KanbanCard.GetNextId();
            int after = KanbanCard.PeekNextId();

            // Expected Result
            Assert.Equal(before, got);
            Assert.Equal(before + 1, after);

            // Actual result is verified by assertions above
        }

        [Fact(DisplayName = "UT22: History entries can be added and enumerated")]
        public void History_CanBeAddedAndEnumerated()
        {
            // Preconditions: New KanbanCard instance

            // Input/Action
            var card = new KanbanCard();
            var entry = new UserActivityEntry
            {
                Timestamp = DateTime.Now,
                PropertyChanged = "Title",
                OldValue = "A",
                NewValue = "B",
                ChangedBy = "User"
            };
            card.History.Add(entry);

            // Expected Result
            Assert.Single(card.History);
            Assert.Equal("Title", card.History[0].PropertyChanged);

            // Actual result is verified by assertions above
        }
    }
}
