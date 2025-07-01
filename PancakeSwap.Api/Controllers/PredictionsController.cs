using Microsoft.AspNetCore.Mvc;
using PancakeSwap.Application.Input;
using PancakeSwap.Application.Output;
using PancakeSwap.Application.Services;
using QYQ.Base.Common.ApiResult;
using System.Threading;
using System.Threading.Tasks;

namespace PancakeSwap.Api.Controllers
{
    /// <summary>
    /// 提供预测相关接口。
    /// </summary>
    [ApiController]
    [Route("/predictions")]
    public class PredictionsController : ControllerBase
    {
        private readonly IRoundService _roundService;

        /// <summary>
        /// 初始化控制器实例。
        /// </summary>
        /// <param name="roundService">回合业务服务。</param>
        public PredictionsController(IRoundService roundService)
        {
            _roundService = roundService;
        }

        /// <summary>
        /// 获取当前回合信息。
        /// </summary>
        /// <param name="ct">取消标记。</param>
        [HttpGet("current")]
        public async Task<ApiResult<CurrentRoundOutput?>> GetCurrentAsync(CancellationToken ct)
        {
            var data = await _roundService.GetCurrentRoundAsync(ct);
            var result = new ApiResult<CurrentRoundOutput?>();
            result.SetRsult(ApiResultCode.Success, data);
            return result;
        }

        /// <summary>
        /// 提交下注请求。
        /// </summary>
        /// <param name="epoch">回合编号。</param>
        /// <param name="input">下注参数。</param>
        [HttpPost("{epoch}/bet")]
        public ApiResult<object> Bet(long epoch, [FromBody] BetInput input)
        {
            // TODO: 实现下注逻辑
            var result = new ApiResult<object>();
            result.SetRsult(ApiResultCode.Success, new { });
            return result;
        }
    }
}
