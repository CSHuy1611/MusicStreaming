using Microsoft.EntityFrameworkCore;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Interfaces;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Models;
using MusicStreaming.Infrastructure.Persistence;

public class PlaylistQueries : IPlaylistQueries
{
    private readonly ApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public PlaylistQueries(ApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<PlaylistDetailVm> GetPlaylistDetailAsync(int playlistId, string userId, CancellationToken cancellationToken)
    {
        var playlist = await _context.Playlists.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == playlistId, cancellationToken);

        if (playlist == null) return null;

        var songs = await _context.PlaylistSongs.AsNoTracking()
            .Where(ps => ps.PlaylistId == playlistId)
            .Select(ps => new SongDto
            {
                Id = ps.Song.Id,
                Title = ps.Song.Title,
                ArtistName = ps.Song.Singer.Name,
                ImageUrl = ps.Song.ImageUrl,
                AudioUrl = ps.Song.AudioUrl,
                Duration = ps.Song.Duration,
                IsVip = ps.Song.IsVip
            })
            .ToListAsync(cancellationToken);

        var isVip = await _identityService.IsUserVipAsync(userId);

        return new PlaylistDetailVm
        {
            Id = playlist.Id,
            Name = playlist.Name,
            Description = "Playlist được tuyển chọn dành riêng cho bạn.",
            ImageUrl = "https://cdn2.fptshop.com.vn/unsafe/1920x0/filters:format(webp):quality(75)/hinh_nen_am_nhac_cover_735bc482b1.png",
            Songs = songs,
            IsUserVip = isVip
        };
    }
}