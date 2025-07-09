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
        /// <summary>
        /// Verifies that all properties of KanbanCard can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void Properties_AreSetAndGetCorrectly()
        {
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

            Assert.Equal(42, card.Id);
            Assert.Equal("Test Card", card.Title);
            Assert.Equal("Tester", card.Owner);
            Assert.Equal("Description", card.Description);
            Assert.Equal("High", card.Urgency);
            Assert.Equal("To Do", card.Status);
            Assert.Equal(now, card.DueDate);
            Assert.Equal("Some comment", card.Comment);
        }

        /// <summary>
        /// Ensures that Clone creates a deep copy of the KanbanCard, including its history.
        /// </summary>
        [Fact]
        public void Clone_CreatesDeepCopy()
        {
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
        }

        /// <summary>
        /// Verifies that UpdateFrom copies all properties from another KanbanCard instance.
        /// </summary>
        [Fact]
        public void UpdateFrom_UpdatesAllProperties()
        {
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

            Assert.Equal("New", card.Title);
            Assert.Equal("B", card.Owner);
            Assert.Equal("D", card.Description);
            Assert.Equal("High", card.Urgency);
            Assert.Equal("Done", card.Status);
            Assert.Equal(DateTime.Today.AddDays(1), card.DueDate);
            Assert.Equal("E", card.Comment);
        }

        /// <summary>
        /// Checks that PeekNextId returns the current next ID without incrementing it.
        /// </summary>
        [Fact]
        public void PeekNextId_ReturnsCurrentNextId()
        {
            int nextId = KanbanCard.PeekNextId();
            int nextIdAgain = KanbanCard.PeekNextId();
            Assert.Equal(nextId, nextIdAgain);
        }

        /// <summary>
        /// Checks that GetNextId returns and increments the next available ID.
        /// </summary>
        [Fact]
        public void GetNextId_IncrementsId()
        {
            int before = KanbanCard.PeekNextId();
            int got = KanbanCard.GetNextId();
            int after = KanbanCard.PeekNextId();
            Assert.Equal(before, got);
            Assert.Equal(before + 1, after);
        }

        /// <summary>
        /// Verifies that history entries can be added and enumerated in the History collection.
        /// </summary>
        [Fact]
        public void History_CanBeAddedAndEnumerated()
        {
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

            Assert.Single(card.History);
            Assert.Equal("Title", card.History[0].PropertyChanged);
        }
    }
}
