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
            int? year, int? status, int page, int pageSize)
        {
            return _proposallist.GetProposals(year, status, page, pageSize);
        }
        public List<ProposalStatus> GetProposalStatuses()
        {
            return _proposallist.GetProposalStatuses();
        }
    }
}
