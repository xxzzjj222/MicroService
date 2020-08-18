using MicroService.MemberService.Context;
using MicroService.MemberService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.MemberService.Repository
{
    public class MemberRepository : IMemberRepository
    {
        public readonly MemberContext memberContext;
        public MemberRepository(MemberContext memberContext)
        {
            this.memberContext = memberContext;
        }
        public void Create(Member member)
        {
            memberContext.Members.Add(member);
            memberContext.SaveChanges();
        }

        public void Delete(Member member)
        {
            memberContext.Members.Remove(member);
            memberContext.SaveChanges();
        }

        public Member GetMemberById(int id)
        {
            return memberContext.Members.Find(id);
        }

        public IEnumerable<Member> GetMembers()
        {
            return memberContext.Members.ToList();
        }

        public IEnumerable<Member> GetMembers(int teamId)
        {
            return memberContext.Members.Where(e => e.TeamId == teamId);
        }

        public bool MemberExists(int id)
        {
            return memberContext.Members.Any(e => e.Id == id);
        }

        public void Update(Member member)
        {
            memberContext.Members.Update(member);
            memberContext.SaveChanges();
        }
    }
}
