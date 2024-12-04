using NCalc;
using NshmCalculator.Shared.Models.CalculatorModel;

namespace NshmCalculator.Shared;

public class PveUtility
{
    private static readonly ExpressionContext ExpContext = new();

    /// <summary>
    /// 用于关联公式层级的映射
    /// </summary>
    private static readonly Dictionary<int, List<string>> _formulaLevelDic = new();

    /// <summary>
    /// 用于关联代号与公式的映射
    /// </summary>
    private static readonly Dictionary<string, Expression> _expressionDic = new();
    
    /// <summary>
    /// 初始化公式的方法，如再次初始化会清空所有私有变量
    /// </summary>
    public static void InitFormulaDic(PveFormula[] array)
    {
        _formulaLevelDic.Clear();
        _expressionDic.Clear();

        int levelCount = array.Select(s => s.Level).Where(s => s > 0).Distinct().Count();
        for (int i = 1; i <= levelCount; i++)
        {
            _formulaLevelDic.Add(i, new List<string>());
            var levelArray = array.Where(s => s.Level == i).ToArray();
            _formulaLevelDic[i].AddRange(levelArray.Select(s => s.Code));
            foreach (var singleFormula in levelArray)
            {
                Expression exp = new Expression(singleFormula.Formula);
                foreach (string singleParam in singleFormula.FormulaParam)
                {
                    if (_expressionDic.TryGetValue(singleParam, out var paramExp))
                    {
                        exp.Parameters[singleParam] = paramExp;
                    }
                }

                _expressionDic.Add(singleFormula.Code, exp);
            }
        }
    }
    
    //TODO:前端参数加载至计算类内
}