using MicroService.VideoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.VideoService.Services
{
    public interface IVideoService
    {
        IEnumerable<Videos> GetVideos();
        Videos GetVideoById(int id);
        void Create(Videos video);
        void Update(Videos video);
        void Delete(Videos video);
        bool VideoExists(int id);
    }
}
