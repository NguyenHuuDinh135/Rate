using Fluxor;
using System.Collections.Generic;
using System.Linq;

namespace WebUI.Shared.Store.Bookings;

[FeatureState]
public class BookingWorkflowState
{
    public bool IsLoading { get; }
    public string? ErrorMessage { get; }

    // Dữ liệu luồng đặt vé
    public int? SelectedMovieId { get; }
    public int? SelectedTheaterId { get; }
    public int? SelectedShowtimeId { get; }
    
    // Dữ liệu ghế và bắp nước
    public List<SeatSelectionItem> SelectedSeats { get; }
    public List<ConcessionSelectionItem> SelectedConcessions { get; }
    
    // Tổng hợp
    public decimal TotalPrice => SelectedSeats.Sum(s => s.Price) + SelectedConcessions.Sum(c => c.Price * c.Quantity);

    public BookingWorkflowState()
    {
        IsLoading = false;
        ErrorMessage = null;
        SelectedMovieId = null;
        SelectedTheaterId = null;
        SelectedShowtimeId = null;
        SelectedSeats = new List<SeatSelectionItem>();
        SelectedConcessions = new List<ConcessionSelectionItem>();
    }

    public BookingWorkflowState(bool isLoading, string? errorMessage, int? selectedMovieId, int? selectedTheaterId, int? selectedShowtimeId, List<SeatSelectionItem> selectedSeats, List<ConcessionSelectionItem> selectedConcessions)
    {
        IsLoading = isLoading;
        ErrorMessage = errorMessage;
        SelectedMovieId = selectedMovieId;
        SelectedTheaterId = selectedTheaterId;
        SelectedShowtimeId = selectedShowtimeId;
        SelectedSeats = selectedSeats;
        SelectedConcessions = selectedConcessions;
    }
}

public class SeatSelectionItem
{
    public string SeatId { get; set; } = string.Empty;
    public string Row { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Type { get; set; } = string.Empty; // Normal, VIP, Sweetbox
    public decimal Price { get; set; }
}

public class ConcessionSelectionItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
