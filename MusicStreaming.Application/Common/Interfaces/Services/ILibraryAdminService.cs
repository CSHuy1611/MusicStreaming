using MusicStreaming.Application.Common.Dtos.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Services
{
    public interface ILibraryAdminService
    {
        // --- QUẢN LÝ THỂ LOẠI (GENRE) ---
        Task<IEnumerable<GenreAdminDto>> GetAllGenresAsync(CancellationToken cancellationToken = default);
        Task<GenreAdminDto> CreateGenreAsync(CreateGenreDto dto, CancellationToken cancellationToken = default);
        Task<GenreAdminDto> UpdateGenreAsync(UpdateGenreDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteGenreAsync(int id, CancellationToken cancellationToken = default);

        // --- QUẢN LÝ SYSTEM PLAYLIST ---
        Task<IEnumerable<PlaylistAdminDto>> GetSystemPlaylistsAsync(CancellationToken cancellationToken = default);
        Task<PlaylistAdminDto> CreateSystemPlaylistAsync(CreateSystemPlaylistDto dto, CancellationToken cancellationToken = default);
        Task<PlaylistAdminDto> UpdateSystemPlaylistAsync(UpdateSystemPlaylistDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteSystemPlaylistAsync(int id, CancellationToken cancellationToken = default);

        // --- CHI TIẾT BÀI HÁT TRONG PLAYLIST ---
        Task<IEnumerable<PlaylistSongDto>> GetSongsInPlaylistAsync(int playlistId, CancellationToken cancellationToken = default);
        Task<bool> AddSongToPlaylistAsync(AddSongToPlaylistDto dto, CancellationToken cancellationToken = default);
        Task<bool> RemoveSongFromPlaylistAsync(RemoveSongFromPlaylistDto dto, CancellationToken cancellationToken = default);
    }
}
