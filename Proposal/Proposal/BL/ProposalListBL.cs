using Proposal.DAC;
using Proposal.Models;
using System.Collections.Generic;

namespace Proposal.BL
{
    public class ProposalListBL
    {
        private readonly ProposalListDAC _proposallist;

        public ProposalListBL(string connectionString)
        {
            _proposallist = new ProposalListDAC(connectionString);
        }

        public (List<ProposalList> Items, int TotalCount, int TotalPages) GetProposalList(
            string userId, string? year, int? status, DateTime? dateFrom, DateTime? dateTo, int page, int pageSize)
        {
            return _proposallist.GetProposals(userId, year, status, dateFrom, dateTo, page, pageSize);
        }
        public List<ProposalStatus> GetProposalStatuses()
        {
            return _proposallist.GetProposalStatuses();
        }
    }
}
