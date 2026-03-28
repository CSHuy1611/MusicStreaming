using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Exceptions;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Services
{
    public class LibraryAdminService : ILibraryAdminService
    {
        private readonly IGenericRepository<Genre> _genreRepository;
        private readonly IGenericRepository<Playlist> _playlistRepository;
        private readonly IGenericRepository<PlaylistSong> _playlistSongRepository;
        private readonly IGenericRepository<Song> _songRepository;
        private readonly IGenericRepository<Singer> _singerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDashboardNotifier _dashboardNotifier; 

        public LibraryAdminService(
            IGenericRepository<Genre> genreRepository,
            IGenericRepository<Playlist> playlistRepository,
            IGenericRepository<PlaylistSong> playlistSongRepository,
            IGenericRepository<Song> songRepository,
            IGenericRepository<Singer> singerRepository,
            IUnitOfWork unitOfWork,
            IDashboardNotifier dashboardNotifier) 
        {
            _genreRepository = genreRepository;
            _playlistRepository = playlistRepository;
            _playlistSongRepository = playlistSongRepository;
            _songRepository = songRepository;
            _singerRepository = singerRepository;
            _unitOfWork = unitOfWork;
            _dashboardNotifier = dashboardNotifier;
        }

        // =======================================================
        // 1. NGHIỆP VỤ THỂ LOẠI (GENRE)
        // =======================================================
        public async Task<IEnumerable<GenreAdminDto>> GetAllGenresAsync(CancellationToken cancellationToken = default)
        {
            var genres = await _genreRepository.GetAllAsync();
            return genres.Where(g => !g.IsDeleted).Select(g => new GenreAdminDto
            {
                Id = g.Id,
                Name = g.Name
            }).ToList();
        }

        public async Task<GenreAdminDto> CreateGenreAsync(CreateGenreDto dto, CancellationToken cancellationToken = default)
        {
            
            if (await _genreRepository.ExistsAsync(g => g.Name.ToLower() == dto.Name.ToLower() && !g.IsDeleted))
                throw new BadRequestException($"Thể loại '{dto.Name}' đã tồn tại.");

            var genre = new Genre(dto.Name); 
            await _genreRepository.AddAsync(genre);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

           
            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return new GenreAdminDto { Id = genre.Id, Name = genre.Name };
        }

        public async Task<GenreAdminDto> UpdateGenreAsync(UpdateGenreDto dto, CancellationToken cancellationToken = default)
        {
            var genre = await _genreRepository.GetByIdAsync(dto.Id);
            if (genre == null || genre.IsDeleted) throw new BadRequestException("Không tìm thấy thể loại.");

            if (await _genreRepository.ExistsAsync(g => g.Id != dto.Id && g.Name.ToLower() == dto.Name.ToLower() && !g.IsDeleted))
                throw new BadRequestException($"Thể loại '{dto.Name}' đã tồn tại.");

            genre.UpdateName(dto.Name);
            _genreRepository.Update(genre);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            
            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return new GenreAdminDto { Id = genre.Id, Name = genre.Name };
        }

        public async Task<bool> DeleteGenreAsync(int id, CancellationToken cancellationToken = default)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null || genre.IsDeleted) return false;

            bool hasSongs = await _songRepository.ExistsAsync(s => s.GenreId == id && !s.IsDeleted);
            if (hasSongs) throw new BadRequestException("Không thể xóa thể loại đang có chứa bài hát.");
            
            genre.Delete();
            _genreRepository.Update(genre);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            
            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return true;
        }

        // =======================================================
        // 2. NGHIỆP VỤ SYSTEM PLAYLIST
        // =======================================================
        public async Task<IEnumerable<PlaylistAdminDto>> GetSystemPlaylistsAsync(CancellationToken cancellationToken = default)
        {
            var playlists = await _playlistRepository.GetAllAsync();
            var playlistSongs = await _playlistSongRepository.GetAllAsync();

            var activeSystemPlaylists = playlists.Where(p => p.IsSystemPlaylist && !p.IsDeleted).ToList();

            return activeSystemPlaylists.Select(p => new PlaylistAdminDto
            {
                Id = p.Id,
                Name = p.Name,
                SongCount = playlistSongs.Count(ps => ps.PlaylistId == p.Id)
            }).ToList();
        }

        public async Task<PlaylistAdminDto> CreateSystemPlaylistAsync(CreateSystemPlaylistDto dto, CancellationToken cancellationToken = default)
        {
            if (await _playlistRepository.ExistsAsync(p => p.Name.ToLower() == dto.Name.ToLower() && p.IsSystemPlaylist && !p.IsDeleted))
                throw new BadRequestException($"Playlist '{dto.Name}' đã tồn tại.");


            var playlist = new Playlist(dto.Name, isSystemPlaylist: true);

            await _playlistRepository.AddAsync(playlist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return new PlaylistAdminDto { Id = playlist.Id, Name = playlist.Name, SongCount = 0 };
        }

        public async Task<PlaylistAdminDto> UpdateSystemPlaylistAsync(UpdateSystemPlaylistDto dto, CancellationToken cancellationToken = default)
        {
            var playlist = await _playlistRepository.GetByIdAsync(dto.Id);
            if (playlist == null || !playlist.IsSystemPlaylist || playlist.IsDeleted)
                throw new BadRequestException("Không tìm thấy Playlist.");

            if (await _playlistRepository.ExistsAsync(p => p.Id != dto.Id && p.Name.ToLower() == dto.Name.ToLower() && p.IsSystemPlaylist && !p.IsDeleted))
                throw new BadRequestException($"Playlist '{dto.Name}' đã tồn tại.");

            playlist.UpdateName(dto.Name);
            _playlistRepository.Update(playlist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return new PlaylistAdminDto { Id = playlist.Id, Name = playlist.Name };
        }

        public async Task<bool> DeleteSystemPlaylistAsync(int id, CancellationToken cancellationToken = default)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);
            if (playlist == null || !playlist.IsSystemPlaylist || playlist.IsDeleted) return false;

            playlist.Delete();
            _playlistRepository.Update(playlist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return true;
        }

        // =======================================================
        // 3. NGHIỆP VỤ BÀI HÁT TRONG PLAYLIST
        // =======================================================
        public async Task<IEnumerable<PlaylistSongDto>> GetSongsInPlaylistAsync(int playlistId, CancellationToken cancellationToken = default)
        {
            var allPlaylistSongs = await _playlistSongRepository.GetAllAsync();
            var links = allPlaylistSongs.Where(ps => ps.PlaylistId == playlistId).ToList();

            var allSongs = await _songRepository.GetAllAsync();
            var allSingers = await _singerRepository.GetAllAsync();

            var result = new List<PlaylistSongDto>();

            foreach (var link in links)
            {
                var song = allSongs.FirstOrDefault(s => s.Id == link.SongId && !s.IsDeleted);
                if (song != null)
                {
                    var singer = allSingers.FirstOrDefault(s => s.Id == song.SingerId);
                    result.Add(new PlaylistSongDto
                    {
                        SongId = song.Id,
                        Title = song.Title,
                        Duration = song.Duration,
                        AddedDate = link.AddedDate,
                        SingerName = singer?.Name ?? "Unknown"
                    });
                }
            }

            return result;
        }

        public async Task<bool> AddSongToPlaylistAsync(AddSongToPlaylistDto dto, CancellationToken cancellationToken = default)
        {

            bool isExist = await _playlistSongRepository.ExistsAsync(ps => ps.PlaylistId == dto.PlaylistId && ps.SongId == dto.SongId);
            if (isExist) throw new BadRequestException("Bài hát này đã có sẵn trong Playlist.");


            var playlistSong = new PlaylistSong(dto.PlaylistId, dto.SongId);

            await _playlistSongRepository.AddAsync(playlistSong);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return true;
        }

        public async Task<bool> RemoveSongFromPlaylistAsync(RemoveSongFromPlaylistDto dto, CancellationToken cancellationToken = default)
        {
            var all = await _playlistSongRepository.GetAllAsync();
            var target = all.FirstOrDefault(ps => ps.PlaylistId == dto.PlaylistId && ps.SongId == dto.SongId);

            if (target == null) throw new BadRequestException("Không tìm thấy bài hát trong Playlist này.");

            _playlistSongRepository.Delete(target); 
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return true;
        }
    }
}