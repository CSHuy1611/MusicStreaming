using MediatR;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Application.Common.Models;
using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Search.Queries.Explore
{
    public class ExploreQueryHandler : IRequestHandler<ExploreQuery, ExploreVm>
    {
        private readonly IGenericRepository<Song> _songRepository;
        private readonly IGenericRepository<Singer> _singerRepository;
        private readonly IGenericRepository<Genre> _genreRepository;
        private readonly IGenericRepository<FavoriteSong> _favoriteRepository;

        // Inject các Repository cần thiết
        public ExploreQueryHandler(
            IGenericRepository<Song> songRepository,
            IGenericRepository<Singer> singerRepository,
            IGenericRepository<Genre> genreRepository,
            IGenericRepository<FavoriteSong> favoriteRepository)
        {
            _songRepository = songRepository;
            _singerRepository = singerRepository;
            _genreRepository = genreRepository;
            _favoriteRepository = favoriteRepository;
        }

        public async Task<ExploreVm> Handle(ExploreQuery request, CancellationToken cancellationToken)
        {
            var viewModel = new ExploreVm();

            // 1. Lấy dữ liệu danh mục để bind ra các ô ComboBox (Thể loại, Ca sĩ)
            var genres = await _genreRepository.GetAllAsync();
            var singers = await _singerRepository.GetAllAsync();

            viewModel.Genres = genres.Where(g => !g.IsDeleted)
                                     .Select(g => new FilterOptionDto { Id = g.Id, Name = g.Name })
                                     .ToList();

            viewModel.Singers = singers.Where(s => !s.IsDeleted)
                                       .Select(s => new FilterOptionDto { Id = s.Id, Name = s.Name })
                                       .ToList();

            // 2. Xác định cờ IsSearching
            viewModel.IsSearching = !string.IsNullOrWhiteSpace(request.Keyword) ||
                                    request.GenreId.HasValue ||
                                    request.SingerId.HasValue ||
                                    !string.IsNullOrWhiteSpace(request.SortBy);

            // 3. Truy vấn kho bài hát
            var allSongs = await _songRepository.GetAllAsync();
            var activeSongs = allSongs.Where(s => !s.IsDeleted).AsQueryable();

            if (!viewModel.IsSearching)
            {

                var topNewestSongs = activeSongs
                    .OrderByDescending(s => s.CreatedDate)
                    .Take(12)
                    .ToList(); 
                viewModel.RecommendedSongs = topNewestSongs
                    .Select(s => new SongDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        ArtistName = singers.FirstOrDefault(x => x.Id == s.SingerId)?.Name ?? "Unknown",
                        ImageUrl = s.ImageUrl,
                        AudioUrl = s.AudioUrl,
                        Duration = s.Duration,
                        IsVip = s.IsVip
                    }).ToList();
            }
            else
            {
                // ========================================================
                // TRẠNG THÁI 2: CÓ TÌM KIẾM -> ÁP DỤNG LỌC & PHÂN TRANG
                // ========================================================
                var query = activeSongs;

                // Lọc theo từ khóa
                if (!string.IsNullOrWhiteSpace(request.Keyword))
                {
                    var keywordLower = request.Keyword.ToLower();
                    var matchingSingerIds = singers
                        .Where(x => x.Name.ToLower().Contains(keywordLower))
                        .Select(x => x.Id)
                        .ToList();
                    query = query.Where(s => s.Title.ToLower().Contains(keywordLower) ||
                                             matchingSingerIds.Contains(s.SingerId));
                }

                // Lọc Thể loại
                if (request.GenreId.HasValue)
                {
                    query = query.Where(s => s.GenreId == request.GenreId.Value);
                }

                // Lọc Ca sĩ
                if (request.SingerId.HasValue)
                {
                    query = query.Where(s => s.SingerId == request.SingerId.Value);
                }

                // Sắp xếp
                if (request.SortBy == "listens")
                {
                    query = query.OrderByDescending(s => s.ListenCount);
                }
                else if (request.SortBy == "likes")
                {
                    var allFavorites = await _favoriteRepository.GetAllAsync();
                    var likeCounts = allFavorites.GroupBy(f => f.SongId).ToDictionary(g => g.Key, g => g.Count());
                    query = query.OrderByDescending(s => likeCounts.ContainsKey(s.Id) ? likeCounts[s.Id] : 0);
                }
                else // Mặc định newest
                {
                    query = query.OrderByDescending(s => s.CreatedDate);
                }

                // ÁP DỤNG PHÂN TRANG (PagedResult)
                int totalItems = query.Count();
                var paginatedSongs = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();

                var songDtos = paginatedSongs.Select(s => new SongDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    ArtistName = singers.FirstOrDefault(x => x.Id == s.SingerId)?.Name ?? "Unknown",
                    ImageUrl = s.ImageUrl,
                    AudioUrl = s.AudioUrl,
                    Duration = s.Duration,
                    IsVip = s.IsVip
                }).ToList();

                // Nạp dữ liệu vào PagedResult
                viewModel.SearchResults = new PagedResult<SongDto>
                {
                    Items = songDtos,
                    TotalCount = totalItems,
                    PageNumber = request.Page,
                    PageSize = request.PageSize
                };
            }

            return viewModel;
        }
    }
}
