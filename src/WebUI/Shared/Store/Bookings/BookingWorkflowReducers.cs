using Fluxor;
using System.Collections.Generic;
using System.Linq;

namespace WebUI.Shared.Store.Bookings;

public static class BookingWorkflowReducers
{
    [ReducerMethod]
    public static BookingWorkflowState ReduceStartBookingAction(BookingWorkflowState state, StartBookingAction action)
    {
        return new BookingWorkflowState(
            isLoading: false,
            errorMessage: null,
            selectedMovieId: action.MovieId,
            selectedTheaterId: null,
            selectedShowtimeId: null,
            selectedSeats: new List<SeatSelectionItem>(),
            selectedConcessions: new List<ConcessionSelectionItem>()
        );
    }

    [ReducerMethod]
    public static BookingWorkflowState ReduceSelectShowtimeAction(BookingWorkflowState state, SelectShowtimeAction action)
    {
        return new BookingWorkflowState(
            isLoading: state.IsLoading,
            errorMessage: state.ErrorMessage,
            selectedMovieId: state.SelectedMovieId,
            selectedTheaterId: action.TheaterId,
            selectedShowtimeId: action.ShowtimeId,
            selectedSeats: new List<SeatSelectionItem>(), // Reset ghế khi đổi suất chiếu
            selectedConcessions: state.SelectedConcessions
        );
    }

    [ReducerMethod]
    public static BookingWorkflowState ReduceToggleSeatAction(BookingWorkflowState state, ToggleSeatAction action)
    {
        var newSeats = new List<SeatSelectionItem>(state.SelectedSeats);
        var existingSeat = newSeats.FirstOrDefault(s => s.SeatId == action.Seat.SeatId);

        if (existingSeat != null)
        {
            newSeats.Remove(existingSeat);
        }
        else
        {
            newSeats.Add(action.Seat);
        }

        return new BookingWorkflowState(
            isLoading: state.IsLoading,
            errorMessage: state.ErrorMessage,
            selectedMovieId: state.SelectedMovieId,
            selectedTheaterId: state.SelectedTheaterId,
            selectedShowtimeId: state.SelectedShowtimeId,
            selectedSeats: newSeats,
            selectedConcessions: state.SelectedConcessions
        );
    }

    [ReducerMethod]
    public static BookingWorkflowState ReduceUpdateConcessionQuantityAction(BookingWorkflowState state, UpdateConcessionQuantityAction action)
    {
        var newConcessions = new List<ConcessionSelectionItem>(state.SelectedConcessions);
        var existingItem = newConcessions.FirstOrDefault(c => c.Id == action.Concession.Id);

        if (existingItem != null)
        {
            if (action.Quantity <= 0)
            {
                newConcessions.Remove(existingItem);
            }
            else
            {
                existingItem.Quantity = action.Quantity;
            }
        }
        else if (action.Quantity > 0)
        {
            var newItem = new ConcessionSelectionItem
            {
                Id = action.Concession.Id,
                Name = action.Concession.Name,
                Price = action.Concession.Price,
                Quantity = action.Quantity,
                ImageUrl = action.Concession.ImageUrl
            };
            newConcessions.Add(newItem);
        }

        return new BookingWorkflowState(
            isLoading: state.IsLoading,
            errorMessage: state.ErrorMessage,
            selectedMovieId: state.SelectedMovieId,
            selectedTheaterId: state.SelectedTheaterId,
            selectedShowtimeId: state.SelectedShowtimeId,
            selectedSeats: state.SelectedSeats,
            selectedConcessions: newConcessions
        );
    }

    [ReducerMethod(typeof(ResetBookingWorkflowAction))]
    public static BookingWorkflowState ReduceResetBookingWorkflowAction(BookingWorkflowState state)
    {
        return new BookingWorkflowState();
    }
}
