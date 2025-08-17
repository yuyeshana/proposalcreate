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

        /// <summary>
        /// 提案書一覧を取得する
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="year">提案年度</param>
        /// <param name="status">提案状態</param>
        /// <param name="dateFrom">開始日</param>
        /// <param name="dateTo">終了日</param>
        /// <param name="page">ページ番号</param>
        /// <param name="pageSize">ページサイズ</param>
        /// <returns>提案書一覧</returns>
        public (List<ProposalList> Items, int TotalCount, int TotalPages) GetProposalList(
            string userId, string? year, int? status, DateTime? dateFrom, DateTime? dateTo, int page, int pageSize)
        {
            return _proposallist.GetProposals(userId, year, status, dateFrom, dateTo, page, pageSize);
        }
        
        /// <summary>
        /// 提案状態一覧を取得する
        /// </summary>
        /// <returns>提案状態一覧</returns>
        public List<ProposalStatus> GetProposalStatuses()
        {
            return _proposallist.GetProposalStatuses();
        }
    }
}
