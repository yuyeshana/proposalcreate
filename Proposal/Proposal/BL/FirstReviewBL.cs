using Microsoft.AspNetCore.Http.HttpResults;
using Proposal.DAC;
using Proposal.Models;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Proposal.BL
{
    public class FirstReviewBL
    {
        private readonly FirstReviewDAC _firstReviewDAC = new FirstReviewDAC();

        public FirstReviewBL()
        {

        }

        public FirstReviewBL(string connectionString)
        {
            _firstReviewDAC = new FirstReviewDAC(connectionString);
        }

        /// <summary>
        /// 提案書の第一次審査情報を取得
        /// </summary>
        public void GetFirstReviewContentById(FirstReviewContentModel firstReviewContent)
        {
            //提案書詳細更新
            var dataTable = _firstReviewDAC.GetFirstReviewContentById(firstReviewContent.ProposalId);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0]; // 指定IDのデータを取得
                firstReviewContent.ReviewGuidance = row["review_guidance"].ToString();
            }
        }

        /// <summary>
        /// 提案書の第一次審査情報を更新
        /// </summary>
        public void UpdateFirstReviewContent(FirstReviewContentModel firstReviewContent)
        {
            //提案書詳細更新
            _firstReviewDAC.SqlUpdateFirstReviewContent(firstReviewContent);
        }

    }
}
