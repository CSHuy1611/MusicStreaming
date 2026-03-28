using MediatR;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Singers.Queries.GetArtistProfile
{
    public class GetArtistProfileQuery : IRequest<ArtistProfileVm>
    {
        public int SingerId { get; set; }
    }
}
