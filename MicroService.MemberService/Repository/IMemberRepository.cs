using MicroService.MemberService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.MemberService.Repository
{
    public interface IMemberRepository
    {
        IEnumerable<Member> GetMembers();
        Member GetMemberById(int id);
        IEnumerable<Member> GetMembers(int teamId);
        void Create(Member member);
        void Update(Member member);
        void Delete(Member member);
        bool MemberExists(int id);
    }
}
