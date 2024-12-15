using NCalc;
using NCalc.Handlers;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.Enums;

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
    /// 前端值
    /// </summary>
    private static readonly Dictionary<string, ParamValue> _paramValueDic = new();

    /// <summary>
    /// 具有特殊规则的参数集合
    /// </summary>
    private static readonly Dictionary<string, SpecialParamRule> _specialParamRuleDic = new();

    /// <summary>
    /// 初始化公式的方法，如再次初始化会清空所有私有变量
    /// </summary>
    public static void InitFormulaDic(PveFormula[] array)
    {
        _formulaLevelDic.Clear();
        _expressionDic.Clear();

        var lambdaFormulas = array.Where(s => s.Rule is { Mode: FormulaMode.Lambda }).ToArray();
        foreach (var formula in lambdaFormulas)
        {
            Expression exp = new Expression(formula.Formula, ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
            _expressionDic.Add(formula.Formula, exp);
        }

        int levelCount = array.Select(s => s.Level).Where(s => s > 0).Distinct().Count();
        for (int i = 1; i <= levelCount; i++)
        {
            _formulaLevelDic.Add(i, new List<string>());
            var levelArray = array.Where(s => s.Level == i).ToArray();
            _formulaLevelDic[i].AddRange(levelArray.Select(s => s.Code));
            foreach (var singleFormula in levelArray)
            {
                Expression exp = _expressionDic.TryGetValue(singleFormula.Code, out var relateExp)
                    ? relateExp
                    : new Expression(singleFormula.Formula, ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
                foreach (string singleParam in singleFormula.FormulaParam)
                {
                    if (_expressionDic.TryGetValue(singleParam, out var paramExp))
                    {
                        exp.Parameters[singleParam] = paramExp;
                    }
                }

                if (singleFormula.Rule is SpecialFormulaRule rule)
                {
                    switch (rule.Mode)
                    {
                        case FormulaMode.Ki:
                        {
                            exp.Functions[nameof(VLookup).ToLower()] = args => VLookup(args[0].Evaluate().ToString());
                        }
                            break;
                        case FormulaMode.LinkLambda:
                        {
                            //Todo:在Json中尝试关联expression（暂时想不好怎么套用自定义函数），且做好公式配置的测试类方法编写
                        }
                            break;
                    }
                }

                _expressionDic.Add(singleFormula.Code, exp);
            }
        }
    }

    /// <summary>
    /// 针对内功公式提供的查找方法,需要挂载在<see cref="Expression">公式实例</see>>上
    /// </summary>
    /// <param name="itemValue">传入的内功名称</param>
    /// <returns>如果能找到该内功名称，则返回内功级别，否则返回空值</returns>
    private static string VLookup(string itemValue)
    {
        string result = string.Empty;

        foreach ((string? code, var rule) in _specialParamRuleDic)
        {
            if (rule.Mode == ParamRuleMode.ShareOptions && _paramValueDic[code].StringValue == itemValue)
            {
                result = _paramValueDic[rule.FrontParamCode].StringValue;
                break;
            }
        }

        return result;
    }
    //TODO:前端参数加载至计算类内
}