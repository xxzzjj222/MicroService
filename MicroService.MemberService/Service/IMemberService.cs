using MicroService.MemberService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.MemberService.Service
{
    public interface IMemberService
    {
        IEnumerable<Member> GetMembers();
        Member GetMemberById(int id);
        void Create(Member member);
        void Update(Member member);
        void Delete(Member member);
        bool MemberExists(int id);
        IEnumerable<Member> GetMembers(int teamId);
    }
}
