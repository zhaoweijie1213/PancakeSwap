using Microsoft.AspNetCore.Mvc;
using PancakeSwap.Application.Output;
using PancakeSwap.Application.Services;
using QYQ.Base.Common.ApiResult;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PancakeSwap.Api.Controllers
{
    /// <summary>
    /// 提供回合相关查询接口。
    /// </summary>
    /// <remarks>
    /// 构造函数。
    /// </remarks>
    [ApiController]
    [Route("/rounds")]
    public class RoundsController(IRoundService roundService) : ControllerBase
    {
        private readonly IRoundService _roundService = roundService;

        /// <summary>
        /// 获取历史回合信息。
        /// </summary>
        [HttpGet("history")]
        public async Task<ApiResult<IList<HistoryRoundOutput>>> HistoryAsync()
        {
            var data = await _roundService.GetHistoryAsync(3);
            var result = new ApiResult<IList<HistoryRoundOutput>>();
            result.SetRsult(ApiResultCode.Success, data);
            return result;
        }

        /// <summary>
        /// 获取即将开始的回合列表。
        /// </summary>
        [HttpGet("upcoming")]
        public async Task<ApiResult<IList<UpcomingRoundOutput>>> UpcomingAsync()
        {
            var data = await _roundService.GetUpcomingAsync(2);
            var result = new ApiResult<IList<UpcomingRoundOutput>>();
            result.SetRsult(ApiResultCode.Success, data);
            return result;
        }
    }
}
