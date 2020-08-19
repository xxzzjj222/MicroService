using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using MicroService.VideoService.Models;
using MicroService.VideoService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MicroService.VideoService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService videoService;
        public VideoController(IVideoService videoService)
        {
            this.videoService = videoService;
        }

        // GET: api/<controller>
        [HttpGet]
        public ActionResult<IEnumerable<Videos>> GetVideos()
        {
            return videoService.GetVideos().ToList();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public ActionResult<Videos> GetVideo(int id)
        {
            Videos video = videoService.GetVideoById(id);
            if (video==null)
            {
                return NotFound();
            }
            return video;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public IActionResult PutVideo(int id, Videos video)
        {
            if (id!=video.Id)
            {
                return BadRequest();
            }

            try
            {
                videoService.Update(video);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!videoService.VideoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// 视频添加
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        /// video.event video.event  video.event.1
        /// video.event video.1 video.1
        /// *  一对多匹配
        /// # 一对一匹配
        [NonAction]
        [CapSubscribe("videoevent")]
        public ActionResult<Videos> PostVideo(Videos video)
        {
            //throw new Exception("aaa");
            Console.WriteLine($"接受到视频事件消息");
            videoService.Create(video);
            return CreatedAtAction("GetVideo", new { id = video.Id }, video);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public ActionResult<Videos> Delete(int id)
        {
            var Video = videoService.GetVideoById(id);
            if (Video == null)
            {
                return NotFound();
            }

            videoService.Delete(Video);
            return Video;
        }
    }
}
