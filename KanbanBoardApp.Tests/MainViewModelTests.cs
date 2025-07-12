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
        [Fact(DisplayName = "UT05: AddCardToColumn adds card to correct column and records creation in history")]
        public void AddCardToColumn_AddsCardToCorrectColumn()
        {
            // Preconditions: New MainViewModel instance, card with Status "To Do"

            // Input/Action
            var vm = new MainViewModel();
            var card = new KanbanCard { Title = "Test", Status = "To Do" };
            vm.AddCardToColumn(card);

            // Expected Result
            var column = vm.Columns.First(c => c.Title == "To Do");
            Assert.Contains(card, column.Cards);
            Assert.Single(card.History);
            Assert.Equal("Created", card.History[0].PropertyChanged);

            // Actual result is verified by assertions above
        }

        [Fact(DisplayName = "UT06: RemoveColumn removes the specified column from the collection")]
        public void RemoveColumn_RemovesColumnFromCollection()
        {
            // Preconditions: MainViewModel with at least one column

            // Input/Action
            var vm = new MainViewModel();
            var column = vm.Columns.First();
            vm.RemoveColumn(column);

            // Expected Result
            Assert.DoesNotContain(column, vm.Columns);

            // Actual result is verified by assertion above
        }

        [Fact(DisplayName = "UT07: MoveCard moves card to target column, updates status, and records history")]
        public void MoveCard_MovesCardToTargetColumnAndUpdatesStatus()
        {
            // Preconditions: Card in source column, target column exists

            // Input/Action
            var vm = new MainViewModel();
            var sourceColumn = vm.Columns.First(c => c.Title == "To Do");
            var targetColumn = vm.Columns.First(c => c.Title == "Done");
            var card = new KanbanCard { Title = "Move Me", Status = sourceColumn.Title };
            sourceColumn.Cards.Add(card);
            vm.MoveCard(card, targetColumn);

            // Expected Result
            Assert.DoesNotContain(card, sourceColumn.Cards);
            Assert.Contains(card, targetColumn.Cards);
            Assert.Equal("Done", card.Status);
            Assert.Contains(card.History, h => h.PropertyChanged == "Status" && h.NewValue == "Done");

            // Actual result is verified by assertions above
        }

        [Fact(DisplayName = "UT08: MoveColumn changes the order of columns in the collection")]
        public void MoveColumn_ChangesColumnOrder()
        {
            // Preconditions: MainViewModel with multiple columns

            // Input/Action
            var vm = new MainViewModel();
            var first = vm.Columns[0];
            var last = vm.Columns[vm.Columns.Count - 1];
            vm.MoveColumn(first, vm.Columns.Count - 1);

            // Expected Result
            Assert.Equal(last, vm.Columns[vm.Columns.Count - 2]);
            Assert.Equal(first, vm.Columns[vm.Columns.Count - 1]);

            // Actual result is verified by assertions above
        }

        [Fact(DisplayName = "UT09: SaveBoard and LoadBoardFromJson preserve columns and cards")]
        public void SaveBoard_And_LoadBoardFromJson_PreservesColumnsAndCards()
        {
            // Preconditions: MainViewModel with at least one card in a column

            // Input/Action
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

            // Expected Result
            Assert.Equal(vm.Columns.Count, vm2.Columns.Count);
            Assert.Contains(vm2.Columns.SelectMany(c => c.Cards), c => c.Title == "Persisted");

            // Actual result is verified by assertions above
        }

        [Fact(DisplayName = "UT10: RecordCardHistory adds history entries for each changed property")]
        public void RecordCardHistory_AddsHistoryEntriesForChangedProperties()
        {
            // Preconditions: Two KanbanCard instances with different property values

            // Input/Action
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

            // Expected Result
            Assert.Contains(card.History, h => h.PropertyChanged == "Title" && h.NewValue == "A2");
            Assert.Contains(card.History, h => h.PropertyChanged == "Owner" && h.NewValue == "B2");
            Assert.Contains(card.History, h => h.PropertyChanged == "Description" && h.NewValue == "C2");
            Assert.Contains(card.History, h => h.PropertyChanged == "Urgency" && h.NewValue == "High");
            Assert.Contains(card.History, h => h.PropertyChanged == "Status" && h.NewValue == "Done");
            Assert.Contains(card.History, h => h.PropertyChanged == "DueDate" && h.NewValue.Contains(DateTime.Today.AddDays(1).ToString("yyyy-MM-dd")));
            Assert.Contains(card.History, h => h.PropertyChanged == "Comment" && h.NewValue == "D2");

            // Actual result is verified by assertions above
        }
    }
}