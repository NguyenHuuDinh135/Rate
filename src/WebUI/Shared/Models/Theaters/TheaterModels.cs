using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Models.Theaters;

public record TheaterSeatDto(string SeatRow, int SeatNumber);

public record TheaterDto(
    int Id,
    string Name,
    int NumOfRows,
    int SeatsPerRow,
    string Type,
    List<TheaterSeatDto> Missing,
    List<TheaterSeatDto> Blocked);

public record ShowDto(
    int Id,
    string StartTime,
    string EndTime,
    string Date,
    int MovieId,
    int TheaterId,
    string Status,
    ShowType Type);
