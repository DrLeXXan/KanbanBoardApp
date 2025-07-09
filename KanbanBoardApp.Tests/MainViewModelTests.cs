using System;
using System.Linq;
using System.Text.Json;
using KanbanBoardApp.Models;
using KanbanBoardApp.ViewModels;
using Xunit;

namespace KanbanBoardApp.Tests
{
    /// <summary>
    /// Unit tests for the MainViewModel class.
    /// Covers card and column management, persistence, and history tracking.
    /// </summary>
    public class MainViewModelTests
    {
        /// <summary>
        /// Verifies that AddCardToColumn adds a card to the correct column and records its creation in history.
        /// </summary>
        [Fact]
        public void AddCardToColumn_AddsCardToCorrectColumn()
        {
            var vm = new MainViewModel();
            var card = new KanbanCard { Title = "Test", Status = "To Do" };

            vm.AddCardToColumn(card);

            var column = vm.Columns.First(c => c.Title == "To Do");
            Assert.Contains(card, column.Cards);
            Assert.Single(card.History);
            Assert.Equal("Created", card.History[0].PropertyChanged);
        }

        /// <summary>
        /// Verifies that RemoveColumn removes the specified column from the collection.
        /// </summary>
        [Fact]
        public void RemoveColumn_RemovesColumnFromCollection()
        {
            var vm = new MainViewModel();
            var column = vm.Columns.First();

            vm.RemoveColumn(column);

            Assert.DoesNotContain(column, vm.Columns);
        }

        /// <summary>
        /// Verifies that MoveCard moves a card to the target column, updates its status, and records the change in history.
        /// </summary>
        [Fact]
        public void MoveCard_MovesCardToTargetColumnAndUpdatesStatus()
        {
            var vm = new MainViewModel();
            var sourceColumn = vm.Columns.First(c => c.Title == "To Do");
            var targetColumn = vm.Columns.First(c => c.Title == "Done");
            var card = new KanbanCard { Title = "Move Me", Status = sourceColumn.Title };
            sourceColumn.Cards.Add(card);

            vm.MoveCard(card, targetColumn);

            Assert.DoesNotContain(card, sourceColumn.Cards);
            Assert.Contains(card, targetColumn.Cards);
            Assert.Equal("Done", card.Status);
            Assert.Contains(card.History, h => h.PropertyChanged == "Status" && h.NewValue == "Done");
        }

        /// <summary>
        /// Verifies that MoveColumn changes the order of columns in the collection.
        /// </summary>
        [Fact]
        public void MoveColumn_ChangesColumnOrder()
        {
            var vm = new MainViewModel();
            var first = vm.Columns[0];
            var last = vm.Columns[vm.Columns.Count - 1];

            vm.MoveColumn(first, vm.Columns.Count - 1);

            Assert.Equal(last, vm.Columns[vm.Columns.Count - 2]);
            Assert.Equal(first, vm.Columns[vm.Columns.Count - 1]);
        }

        /// <summary>
        /// Verifies that saving and loading the board preserves columns and cards.
        /// </summary>
        [Fact]
        public void SaveBoard_And_LoadBoardFromJson_PreservesColumnsAndCards()
        {
            var vm = new MainViewModel();
            var card = new KanbanCard { Title = "Persisted", Status = "To Do" };
            vm.AddCardToColumn(card);

            // Save to JSON string
            string tempFile = System.IO.Path.GetTempFileName();
            vm.SaveBoard(tempFile);
            string json = System.IO.File.ReadAllText(tempFile);

            // Create new ViewModel and load
            var vm2 = new MainViewModel();
            vm2.LoadBoardFromJson(json);

            Assert.Equal(vm.Columns.Count, vm2.Columns.Count);
            Assert.Contains(vm2.Columns.SelectMany(c => c.Cards), c => c.Title == "Persisted");
        }

        /// <summary>
        /// Verifies that RecordCardHistory adds history entries for each changed property between two cards.
        /// </summary>
        [Fact]
        public void RecordCardHistory_AddsHistoryEntriesForChangedProperties()
        {
            var card = new KanbanCard
            {
                Title = "A",
                Owner = "B",
                Description = "C",
                Urgency = "Low",
                Status = "To Do",
                DueDate = DateTime.Today,
                Comment = "D"
            };
            var edited = new KanbanCard
            {
                Title = "A2",
                Owner = "B2",
                Description = "C2",
                Urgency = "High",
                Status = "Done",
                DueDate = DateTime.Today.AddDays(1),
                Comment = "D2"
            };

            var vm = new MainViewModel();
            vm.RecordCardHistory(card, edited);

            Assert.Contains(card.History, h => h.PropertyChanged == "Title" && h.NewValue == "A2");
            Assert.Contains(card.History, h => h.PropertyChanged == "Owner" && h.NewValue == "B2");
            Assert.Contains(card.History, h => h.PropertyChanged == "Description" && h.NewValue == "C2");
            Assert.Contains(card.History, h => h.PropertyChanged == "Urgency" && h.NewValue == "High");
            Assert.Contains(card.History, h => h.PropertyChanged == "Status" && h.NewValue == "Done");
            Assert.Contains(card.History, h => h.PropertyChanged == "DueDate" && h.NewValue.Contains(DateTime.Today.AddDays(1).ToString("yyyy-MM-dd")));
            Assert.Contains(card.History, h => h.PropertyChanged == "Comment" && h.NewValue == "D2");
        }
    }
}