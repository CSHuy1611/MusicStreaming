using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Songs.Commands.IncrementListen
{
    public class IncrementListenCommand : IRequest<bool>
    {
        public int SongId { get; set; }

        public IncrementListenCommand(int songId)
        {
            SongId = songId;
        }
    }
}
