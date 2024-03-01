using System.ComponentModel.DataAnnotations;

namespace NshmCalculator.Shared.Models.BaseModel
{
    /// <summary>
    /// 玩家基础面板
    /// </summary>
    public class PlayerBaseInfo
    {
        #region 玩家基础数值

        [Required]
        [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
        public int BaseAttack { get; set; }

        /// <summary>
        /// 玩家基础克制
        /// </summary>
        [Required]
        [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
        public int BaseRestraint { get; set; }

        /// <summary>
        /// 玩家基础属性（元素）攻击
        /// </summary>
        [Required]
        [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
        public int BaseElementAttack { get; set; }

        /// <summary>
        /// 玩家基础破防
        /// </summary>
        [Required]
        [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
        public int BaseBreakDefense { get; set; }

        /// <summary>
        /// 玩家基础命中
        /// </summary>
        [Required]
        [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
        public int BaseHit { get; set; }

        /// <summary>
        /// 玩家基础会心
        /// </summary>
        [Required]
        [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
        public int BaseCriticalHits { get; set; }

        /// <summary>
        /// 玩家基础会伤率
        /// </summary>
        [Required]
        [Range(0, 1000, ErrorMessage = "请保证会伤百分比在0.0-1000.0范围内")]
        public double BaseCriticalRate { get; set; }

        /// <summary>
        /// 玩家周天会心率
        /// </summary>
        [Required]
        [Range(0, 5, ErrorMessage = "请保证周天会心百分比在0.0-5.0范围内")]
        public double BaseZtCriticalHitsRate { get; set; }

        #endregion

        /// <summary>
        /// 结果（百分比增伤）
        /// </summary>
        public double Result { get; set; }

        /// <summary>
        /// 实际命中率
        /// </summary>
        public double HitRate { get; set; }
    }
}