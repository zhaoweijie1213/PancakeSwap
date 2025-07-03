using System.ComponentModel.DataAnnotations;

namespace PancakeSwap.Application.Input
{
    /// <summary>
    /// 用户下注请求参数。
    /// </summary>
    public class BetInput
    {
        /// <summary>
        /// 回合编号
        /// </summary>
        public long Epoch { get; set; }

        /// <summary>
        /// 下注金额，单位为 Ton。
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// 下注方向，bull 表示看涨，bear 表示看跌。
        /// </summary>
        [Required]
        [RegularExpression("^(bull|bear)$", ErrorMessage = "Direction must be bull or bear")]
        public string Direction { get; set; } = string.Empty;
    }
}
