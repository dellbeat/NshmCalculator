using NCalc;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;
using NshmCalculator.Shared.Models.CalculatorModel.Enums;

namespace NshmCalculator.Shared;

public class PveUtility
{
    private static readonly ExpressionContext ExpContext = new() { Options = ExpressionOptions.IgnoreCaseAtBuiltInFunctions };

    /// <summary>
    /// 用于关联公式层级的映射
    /// </summary>
    private static readonly Dictionary<int, List<string>> FormulaLevelDic = new();

    /// <summary>
    /// 用于关联公式代号与表达式实体的映射
    /// </summary>
    private static readonly Dictionary<string, Expression> FormulaExpressionDic = new();

    /// <summary>
    /// 用于关联参数代号与表达式实体的映射
    /// </summary>
    private static readonly Dictionary<string, Expression> ParamExpressionDic = new();

    /// <summary>
    /// 具有特殊规则的参数集合
    /// </summary>
    private static readonly Dictionary<string, SpecialParamRule> SpecialParamRuleDic = new();

    // /// <summary>
    // /// 结果公式与表达式实体的映射
    // /// </summary>
    // private static readonly Dictionary<string, Expression> ResultExpressionDic = new();

    public static void InitUtilityFromConfig(PveConfig config)
    {
        ParamExpressionDic.Clear();
        FormulaLevelDic.Clear();
        FormulaExpressionDic.Clear();
        InitParamDic(config.FrontParamInfoArray);
        InitFormulaDic(config.PveFormulas);
    }

    /// <summary>
    /// 将参数初始化为表达式实体
    /// </summary>
    /// <param name="array"></param>
    private static void InitParamDic(FrontParamInfo[] array)
    {
        ParamExpressionDic.Clear();

        foreach (var paramInfo in array)
        {
            Expression exp = new Expression("[value]");
            ParamExpressionDic.Add(paramInfo.Code, exp);
        }
    }

    /// <summary>
    /// 初始化公式的方法，如再次初始化会清空所有私有变量
    /// </summary>
    private static void InitFormulaDic(PveFormula[] array)
    {
        var lambdaFormulas = array.Where(s => s.Rule != null && s.Rule.Any(y => y.Mode == FormulaMode.Lambda)).ToArray();
        foreach (var formula in lambdaFormulas)
        {
            Expression exp = new Expression(formula.Formula, ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
            FormulaExpressionDic.Add(formula.Code, exp);
        }

        int levelCount = array.Select(s => s.Level).Where(s => s > 0).Distinct().Count();
        for (int i = 1; i <= levelCount; i++)
        {
            FormulaLevelDic.Add(i, new List<string>());
            var levelArray = array.Where(s => s.Level == i).ToArray();
            FormulaLevelDic[i].AddRange(levelArray.Select(s => s.Code));
            foreach (var singleFormula in levelArray)
            {
                Expression exp = FormulaExpressionDic.TryGetValue(singleFormula.Code, out var relateExp)
                    ? relateExp
                    : new Expression(singleFormula.Formula, ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
                foreach (string singleParam in singleFormula.FormulaParam)
                {
                    if (FormulaExpressionDic.TryGetValue(singleParam, out var formulaExp))
                    {
                        exp.Parameters[singleParam] = formulaExp;
                    }
                    else if (ParamExpressionDic.TryGetValue(singleParam, out var paramExp))
                    {
                        exp.Parameters[singleParam] = paramExp;
                    }
                }

                if (singleFormula.Rule != null)
                {
                    foreach (var rule in singleFormula.Rule)
                    {
                        switch (rule.Mode)
                        {
                            case FormulaMode.Ki:
                            {
                                exp.Functions[nameof(VLookup).ToLower()] = args => VLookup(args[0].Evaluate().ToString());
                            }
                                break;
                        }
                    }
                }

                FormulaExpressionDic.TryAdd(singleFormula.Code, exp);
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

        foreach ((string? code, var rule) in SpecialParamRuleDic)
        {
            if (rule.Mode == ParamRuleMode.ShareOptions && ParamExpressionDic[code].Evaluate().ToString() == itemValue)
            {
                result = ParamExpressionDic[rule.FrontParamCode].Evaluate().ToString();
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// 传入界面上所有选项的当前值，供后续计算用
    /// </summary>
    /// <param name="valueDic"></param>
    public static void InsertParam(Dictionary<string, ParamValue> valueDic)
    {
        foreach ((string? code, var value) in valueDic)
        {
            if (ParamExpressionDic.TryGetValue(code, out var exp))
            {
                exp.Parameters["value"] = value.NumberMode ? value.NumberValue : value.StringValue;
            }
        }
    }

    /// <summary>
    /// 针对结果公式调用计算方法（实际最后面是都要调用的）
    /// </summary>
    /// <param name="codeList"></param>
    /// <returns></returns>
    public static List<double?> Calculate(List<string> codeList)
    {
        List<double?> result = new List<double?>();
        
        foreach ((string? code, var exp) in ParamExpressionDic)
        {
            exp.Evaluate();
        }
        
        foreach ((int level, var list) in FormulaLevelDic)
        {
            foreach (string code in list)
            {
                try
                {
                    if (FormulaExpressionDic.TryGetValue(code, out var codeExp))
                    {
                        codeExp.Evaluate();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(code);
                    throw;
                }
            }
        }

        foreach (string code in codeList)
        {
            if (FormulaExpressionDic.TryGetValue(code, out var formulaExp))
            {
                foreach ((string? key, object? _) in formulaExp.Parameters)
                {
                    if (ParamExpressionDic.TryGetValue(key, out var paramExp))
                    {
                        formulaExp.Parameters[key] = paramExp.Evaluate();
                    }
                    else if (FormulaExpressionDic.TryGetValue(key, out var subFormulaExp))
                    {
                        formulaExp.Parameters[key] = subFormulaExp;
                    }
                }
        
                if (double.TryParse(formulaExp.Evaluate().ToString(), out double value))
                {
                    result.Add(value);
                }
                else
                {
                    result.Add(null);
                }
            }
        }

        return result;
    }
}