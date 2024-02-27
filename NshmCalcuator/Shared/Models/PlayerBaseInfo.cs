using System.ComponentModel.DataAnnotations;

namespace NshmCalculator.Shared.Models
{
    /// <summary>
    /// 玩家基础面板
    /// </summary>
    public class PlayerBaseInfo
    {
        #region 玩家基础数值

        [Required]
        [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
        public int PlayerBaseAttack { get; set; }

        /// <summary>
        /// 玩家基础克制
        /// </summary>
        [Required]
        [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
        public int PlayerBaseRestraint { get; set; }

        /// <summary>
        /// 玩家基础属性（元素）攻击
        /// </summary>
        [Required]
        [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
        public int PlayerBaseElementAttack { get; set; }

        /// <summary>
        /// 玩家基础破防
        /// </summary>
        [Required]
        [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
        public int PlayerBaseBreakDefense { get; set; }

        /// <summary>
        /// 玩家基础命中
        /// </summary>
        [Required]
        [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
        public int PlayerBaseHit { get; set; }

        /// <summary>
        /// 玩家基础会心
        /// </summary>
        [Required]
        [Range(1, 100000, ErrorMessage = "请输入1至100000内的整数")]
        public int PlayerBaseCriticalHits { get; set; }

        /// <summary>
        /// 玩家基础会伤率
        /// </summary>
        [Required]
        [Range(0, 1000, ErrorMessage = "请保证会伤百分比在0.0-1000.0范围内")]
        public double PlayerBaseCriticalRate { get; set; }

        /// <summary>
        /// 玩家周天会心率
        /// </summary>
        [Required]
        [Range(0, 5, ErrorMessage = "请保证周天会心百分比在0.0-5.0范围内")]
        public double PlayerBaseZtCriticalHitsRate { get; set; }

        #endregion

        #region 敌方数值

        /// <summary>
        /// 敌方防御
        /// </summary>
        [Required]
        [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
        public int EnemyDefense { get; set; }

        /// <summary>
        /// 敌方格挡
        /// </summary>
        [Required]
        [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
        public int EnemyBlock { get; set; }

        /// <summary>
        /// 敌方抵御
        /// </summary>
        [Required]
        [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
        public int EnemyAntiRestraint { get; set; }

        /// <summary>
        /// 敌方会心抗
        /// </summary>
        [Required]
        [Range(0, 100000, ErrorMessage = "请输入0至100000内的整数")]
        public int EnemyAntiCriticalHits { get; set; }

        /// <summary>
        /// 敌方元素抗，暂不作为必填属性
        /// </summary>
        public int EnemyAntiElementAttack { get; set; }

        /// <summary>
        /// 满命中
        /// </summary>
        public int FullHit => Convert.ToInt32(Math.Floor((105524 * EnemyBlock + 2541845) * 1.0 / (98394 - 5 * EnemyBlock)));

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