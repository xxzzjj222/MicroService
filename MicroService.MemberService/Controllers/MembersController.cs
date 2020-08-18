using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroService.MemberService.Context;
using MicroService.MemberService.Model;
using MicroService.MemberService.Service;

namespace MicroService.MemberService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService memberService;

        public MembersController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        // GET: api/Members/5
        [HttpGet("{id}")]
        public ActionResult<Member> GetMember(int id)
        {
            var member = memberService.GetMemberById(id);

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        /// <summary>
        /// 查询所有成员信息
        /// </summary>
        /// <param name="teamId">?teamId参数结尾方式</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Member>> GetMembers(int teamId)
        {
            if (teamId == 0)
            {
                return memberService.GetMembers().ToList();
            }
            else
            {
                return memberService.GetMembers(teamId).ToList();
            }
        }

        // PUT: api/Members/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public IActionResult PutMember(int id, Member member)
        {
            if (id != member.Id)
            {
                return BadRequest();
            }
            try
            {
                memberService.Create(member);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // POST: api/Members
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public ActionResult<Member> PostMember(Member member)
        {
            memberService.Create(member);
            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

        // DELETE: api/Members/5
        [HttpDelete("{id}")]
        public ActionResult<Member> DeleteMember(int id)
        {
            var member = memberService.GetMemberById(id);
            if (member == null)
            {
                return NotFound();
            }

            memberService.Delete(member);
            return member;
        }

        private bool MemberExists(int id)
        {
            return memberService.MemberExists(id);
        }
    }
}
