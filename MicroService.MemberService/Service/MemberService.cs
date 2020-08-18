using MicroService.MemberService.Model;
using MicroService.MemberService.Repository;
using Servicecomb.Saga.Omega.Abstractions.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.MemberService.Service
{
    public class MemberService : IMemberService
    {
        public readonly IMemberRepository memberRepository;
        public MemberService(IMemberRepository memberRepository)
        {
            this.memberRepository = memberRepository;
        }
        [Compensable(nameof(Delete))]
        public void Create(Member member)
        {
            memberRepository.Create(member);
        }

        public void Delete(Member member)
        {
            memberRepository.Delete(member);
        }

        public Member GetMemberById(int id)
        {
            return memberRepository.GetMemberById(id);
        }

        public IEnumerable<Member> GetMembers()
        {
            return memberRepository.GetMembers();
        }

        public IEnumerable<Member> GetMembers(int teamId)
        {
            return memberRepository.GetMembers(teamId);
        }

        public bool MemberExists(int id)
        {
            return memberRepository.MemberExists(id);
        }

        public void Update(Member member)
        {
            memberRepository.Update(member);
        }
    }
}
