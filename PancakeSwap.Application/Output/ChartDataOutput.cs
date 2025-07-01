using System.Collections.Generic;

namespace PancakeSwap.Application.Output
{
    /// <summary>
    /// Chainlink 图表数据。
    /// </summary>
    public class ChartDataOutput
    {
        /// <summary>
        /// X 轴时间字符串数组。
        /// </summary>
        public IList<string> OriginalXData { get; set; } = new List<string>();

        /// <summary>
        /// Y 轴价格数组。
        /// </summary>
        public IList<decimal> SeriesData { get; set; } = new List<decimal>();
    }
}
