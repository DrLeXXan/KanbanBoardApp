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
        [Fact(DisplayName = "UT11: CardCount reflects the number of cards in the Cards collection")]
        public void CardCount_ReflectsCardsCollection()
        {
            // Preconditions: New KanbanColumn instance, Cards collection is empty

            // Input/Action
            var column = new KanbanColumn { Title = "Test" };

            // Expected Result
            Assert.Equal(0, column.CardCount);

            column.Cards.Add(new KanbanCard { Title = "A" });
            Assert.Equal(1, column.CardCount);

            column.Cards.Add(new KanbanCard { Title = "B" });
            Assert.Equal(2, column.CardCount);

            column.Cards.RemoveAt(0);
            Assert.Equal(1, column.CardCount);

            // Actual result is verified by assertions above
        }

        [Fact(DisplayName = "UT12: IsEditing property can be set and retrieved")]
        public void IsEditing_CanBeSetAndGet()
        {
            // Preconditions: New KanbanColumn instance

            // Input/Action
            var column = new KanbanColumn();
            column.IsEditing = true;

            // Expected Result
            Assert.True(column.IsEditing);

            column.IsEditing = false;
            Assert.False(column.IsEditing);

            // Actual result is verified by assertions above
        }

        [Fact(DisplayName = "UT13: Changing Title raises PropertyChanged event")]
        public void Title_RaisesPropertyChangedEvent()
        {
            // Preconditions: New KanbanColumn instance

            // Input/Action
            var column = new KanbanColumn();
            string? changedProperty = null;
            column.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            column.Title = "New Title";

            // Expected Result
            Assert.Equal("Title", changedProperty);

            // Actual result is verified by assertion above
        }

        [Fact(DisplayName = "UT14: Changing IsEditing raises PropertyChanged event")]
        public void IsEditing_RaisesPropertyChangedEvent()
        {
            // Preconditions: New KanbanColumn instance, event handler attached

            // Input/Action
            var column = new KanbanColumn();
            string? changedProperty = null;
            column.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            column.IsEditing = true;

            // Expected Result
            Assert.Equal("IsEditing", changedProperty);

            // Actual result is verified by assertion above
        }

        [Fact(DisplayName = "UT15: Adding card raises PropertyChanged event for CardCount")]
        public void CardCount_RaisesPropertyChangedEventOnCardsChanged()
        {
            // Preconditions: New KanbanColumn instance, event handler attached

            // Input/Action
            var column = new KanbanColumn();
            string? changedProperty = null;
            column.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            column.Cards.Add(new KanbanCard { Title = "A" });

            // Expected Result
            Assert.Equal("CardCount", changedProperty);

            // Actual result is verified by assertion above
        }

        [Fact(DisplayName = "UT16: Cards collection raises CollectionChanged event when a card is added")]
        public void Cards_CollectionChanged_EventIsRaised()
        {
            // Preconditions: New KanbanColumn instance, event handler attached

            // Input/Action
            var column = new KanbanColumn();
            bool eventRaised = false;
            column.Cards.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    eventRaised = true;
            };

            column.Cards.Add(new KanbanCard { Title = "A" });

            // Expected Result
            Assert.True(eventRaised);

            // Actual result is verified by assertion above
        }
    }
}
