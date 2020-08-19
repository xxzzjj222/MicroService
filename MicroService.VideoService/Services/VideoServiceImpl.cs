using MicroService.VideoService.Models;
using MicroService.VideoService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.VideoService.Services
{
    public class VideoServiceImpl : IVideoService
    {
        private readonly IVideoRepository videoRepository;

        public VideoServiceImpl(IVideoRepository videoRepository)
        {
            this.videoRepository = videoRepository;
        }
        public void Create(Videos video)
        {
            videoRepository.Create(video);
        }

        public void Delete(Videos video)
        {
            videoRepository.Delete(video);
        }

        public Videos GetVideoById(int id)
        {
            return videoRepository.GetVideoById(id);
        }

        public IEnumerable<Videos> GetVideos()
        {
            return videoRepository.GetVideos();
        }

        public void Update(Videos video)
        {
            videoRepository.Update(video);
        }

        public bool VideoExists(int id)
        {
            return videoRepository.VideoExists(id);
        }
    }
}
