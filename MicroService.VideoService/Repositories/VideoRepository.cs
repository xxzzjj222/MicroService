using MicroService.VideoService.Context;
using MicroService.VideoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.VideoService.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly VideoContext videoContext;

        public VideoRepository(VideoContext videoContext)
        {
            this.videoContext = videoContext;
        }
        public void Create(Videos video)
        {
            videoContext.Add(video);
            videoContext.SaveChanges();
        }

        public void Delete(Videos video)
        {
            videoContext.Videos.Remove(video);
            videoContext.SaveChanges();
        }

        public Videos GetVideoById(int id)
        {
            return videoContext.Videos.Find(id);
        }

        public IEnumerable<Videos> GetVideos()
        {
            return videoContext.Videos.ToList();
        }

        public void Update(Videos video)
        {
            videoContext.Videos.Update(video);
            videoContext.SaveChanges();
        }

        public bool VideoExists(int id)
        {
            return videoContext.Videos.Any(e => e.Id == id);
        }
    }
}
