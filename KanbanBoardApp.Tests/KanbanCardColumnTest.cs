using System.Collections.Specialized;
using KanbanBoardApp.Models;
using Xunit;

namespace KanbanBoardApp.Tests
{
    /// <summary>
    /// Unit tests for the KanbanColumn model.
    /// Covers card count logic, property changes, and collection events.
    /// </summary>
    public class KanbanColumnTests
    {
        /// <summary>
        /// Verifies that CardCount reflects the number of cards in the Cards collection.
        /// </summary>
        [Fact]
        public void CardCount_ReflectsCardsCollection()
        {
            var column = new KanbanColumn { Title = "Test" };
            Assert.Equal(0, column.CardCount);

            column.Cards.Add(new KanbanCard { Title = "A" });
            Assert.Equal(1, column.CardCount);

            column.Cards.Add(new KanbanCard { Title = "B" });
            Assert.Equal(2, column.CardCount);

            column.Cards.RemoveAt(0);
            Assert.Equal(1, column.CardCount);
        }

        /// <summary>
        /// Verifies that the IsEditing property can be set and retrieved.
        /// </summary>
        [Fact]
        public void IsEditing_CanBeSetAndGet()
        {
            var column = new KanbanColumn();
            column.IsEditing = true;
            Assert.True(column.IsEditing);
            column.IsEditing = false;
            Assert.False(column.IsEditing);
        }

        /// <summary>
        /// Ensures that changing the Title property raises the PropertyChanged event.
        /// </summary>
        [Fact]
        public void Title_RaisesPropertyChangedEvent()
        {
            var column = new KanbanColumn();
            string? changedProperty = null;
            column.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            column.Title = "New Title";

            Assert.Equal("Title", changedProperty);
        }

        /// <summary>
        /// Ensures that changing the IsEditing property raises the PropertyChanged event.
        /// </summary>
        [Fact]
        public void IsEditing_RaisesPropertyChangedEvent()
        {
            var column = new KanbanColumn();
            string? changedProperty = null;
            column.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            column.IsEditing = true;

            Assert.Equal("IsEditing", changedProperty);
        }

        /// <summary>
        /// Ensures that adding a card to the Cards collection raises the PropertyChanged event for CardCount.
        /// </summary>
        [Fact]
        public void CardCount_RaisesPropertyChangedEventOnCardsChanged()
        {
            var column = new KanbanColumn();
            string? changedProperty = null;
            column.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            column.Cards.Add(new KanbanCard { Title = "A" });

            Assert.Equal("CardCount", changedProperty);
        }

        /// <summary>
        /// Verifies that the CollectionChanged event is raised when a card is added to the Cards collection.
        /// </summary>
        [Fact]
        public void Cards_CollectionChanged_EventIsRaised()
        {
            var column = new KanbanColumn();
            bool eventRaised = false;
            column.Cards.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    eventRaised = true;
            };

            column.Cards.Add(new KanbanCard { Title = "A" });

            Assert.True(eventRaised);
        }
    }
}
