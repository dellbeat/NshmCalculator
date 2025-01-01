using NCalc;
using NshmCalculator.Shared.Models.CalculatorModel.CalculatorConfig;
using NshmCalculator.Shared.Models.CalculatorModel.Enums;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Formula;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Params;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.SpecialRule;

namespace NshmCalculator.Shared;

public class PveUtility
{
    /// <summary>
    /// 用于关联公式层级的映射
    /// </summary>
    private static readonly Dictionary<int, List<string>> FormulaLevelDic = new();

    /// <summary>
    /// 中间公式代号与表达式实体的映射
    /// </summary>
    private static readonly Dictionary<string, Expression> InternalFormulaExpressionDic = new();

    /// <summary>
    /// 最终结果的代号与表达式实体映射
    /// </summary>
    private static readonly Dictionary<string, Expression> ResultFormulaExpressionDic = new();

    /// <summary>
    /// 具有特殊规则的表达式集合
    /// </summary>
    private static readonly Dictionary<string, SpecialFormulaRule> SpecialFormulaRuleDic = new();

    /// <summary>
    /// 初始化后包含的参数代号列表
    /// </summary>
    private static readonly List<string> ParamCodeList = new();

    public static void InitUtilityFromConfig(PveConfig config)
    {
        ParamCodeList.Clear();
        FormulaLevelDic.Clear();
        InternalFormulaExpressionDic.Clear();
        SpecialFormulaRuleDic.Clear();
        ParamCodeList.AddRange(config.FrontParamInfoArray.Select(s => s.Code));
        InitInternalFormulaDic(config.InternalFormulas);
        InitResultFormulaDic(config.ResultFormulas);
    }

    /// <summary>
    /// 初始化结果表达式的方法
    /// </summary>
    /// <param name="array"></param>
    private static void InitResultFormulaDic(PveFormula[] array)
    {
        foreach (var formula in array)
        {
            Expression exp = new Expression(formula.Formula, ExpressionOptions.StrictTypeMatching | ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
            foreach (string param in formula.FormulaParam)
            {
                if (InternalFormulaExpressionDic.ContainsKey(param))
                {
                    exp.Parameters[param] = string.Empty;
                }
                else if (ParamCodeList.Contains(param))
                {
                    exp.Parameters[param] = string.Empty;
                }
            }

            if (formula.Rule != null && formula.Rule.FirstOrDefault(s => s.Mode == FormulaMode.LinkLambda) is { } linkRule)
            {
                foreach (string functionName in linkRule.LambdaParam)
                {
                    if (!exp.Functions.ContainsKey(functionName))
                    {
                        exp.Functions.Add(functionName, args =>
                        {
                            var fun = InternalFormulaExpressionDic[functionName];
                            var rule = SpecialFormulaRuleDic[functionName];
                            int index = 0;
                            foreach (var expression in args)
                            {
                                fun.Parameters[rule.LambdaParam[index++]] = expression;
                            }

                            return fun.Evaluate();
                        });
                    }
                }
            }

            ResultFormulaExpressionDic.Add(formula.Code, exp);
        }
    }

    /// <summary>
    /// 初始化公式的方法，如再次初始化会清空所有私有变量
    /// </summary>
    private static void InitInternalFormulaDic(PveFormula[] array)
    {
        var lambdaFormulas = array.Where(s => s.Rule != null && s.Rule.Any(y => y.Mode == FormulaMode.Lambda)).ToArray();
        foreach (var formula in lambdaFormulas)
        {
            Expression exp = new Expression(formula.Formula, ExpressionOptions.StrictTypeMatching | ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
            InternalFormulaExpressionDic.Add(formula.Code, exp);
            SpecialFormulaRuleDic.Add(formula.Code, formula.Rule.First(y => y.Mode == FormulaMode.Lambda));
        }

        int levelCount = array.Select(s => s.Level).Where(s => s > 0).Distinct().Count();
        for (int i = 1; i <= levelCount; i++)
        {
            FormulaLevelDic.Add(i, new List<string>());
            var levelArray = array.Where(s => s.Level == i).ToArray();
            FormulaLevelDic[i].AddRange(levelArray.Select(s => s.Code));
            foreach (var singleFormula in levelArray)
            {
                Expression exp = InternalFormulaExpressionDic.TryGetValue(singleFormula.Code, out var relateExp)
                    ? relateExp
                    : new Expression(singleFormula.Formula, ExpressionOptions.StrictTypeMatching | ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
                foreach (string singleParam in singleFormula.FormulaParam)
                {
                    if (InternalFormulaExpressionDic.TryGetValue(singleParam, out var formulaExp))
                    {
                        exp.Parameters[singleParam] = string.Empty;
                    }
                    else if (ParamCodeList.Contains(singleParam))
                    {
                        exp.Parameters[singleParam] = string.Empty;
                    }
                }

                if (singleFormula.Rule != null)
                {
                    foreach (var rule in singleFormula.Rule.Where(s => s.Mode == FormulaMode.LinkLambda))
                    {
                        foreach (string functionName in rule.LambdaParam)
                        {
                            if (!exp.Functions.ContainsKey(functionName))
                            {
                                exp.Functions.Add(functionName, args =>
                                {
                                    var fun = InternalFormulaExpressionDic[functionName];
                                    var rule = SpecialFormulaRuleDic[functionName];
                                    int index = 0;
                                    foreach (var expression in args)
                                    {
                                        fun.Parameters[rule.LambdaParam[index++]] = expression;
                                    }

                                    return fun.Evaluate();
                                });
                            }
                        }
                    }
                }

                InternalFormulaExpressionDic.TryAdd(singleFormula.Code, exp);
            }
        }
    }

    /// <summary>
    /// 针对结果公式调用计算方法
    /// </summary>
    /// <param name="codeList"></param>
    /// <returns></returns>
    public static Dictionary<string, double?> Calculate(List<string> codeList, Dictionary<string, ParamValue> valueDic)
    {
        Dictionary<string, double?> result = new Dictionary<string, double?>();

        Dictionary<string, object> internalValueDic = new Dictionary<string, object>();
        foreach ((string? key, var value) in valueDic)
        {
            internalValueDic.Add(key, value.NumberMode ? value.NumberValue : value.StringValue);
        }

        foreach ((int _, var list) in FormulaLevelDic)
        {
            foreach (string code in list)
            {
                try
                {
                    if (InternalFormulaExpressionDic.TryGetValue(code, out var codeExp))
                    {
                        if (!SpecialFormulaRuleDic.ContainsKey(code))
                        {
                            foreach (string? paramCode in codeExp.Parameters.Keys)
                            {
                                codeExp.Parameters[paramCode] = internalValueDic[paramCode];
                            }

                            internalValueDic.Add(code, codeExp.Evaluate());
                        }
                        else
                        {
                            foreach ((string? paramCode, object? _) in codeExp.Parameters)
                            {
                                if (internalValueDic.TryGetValue(paramCode, out object? paramValue))
                                {
                                    codeExp.Parameters[paramCode] = paramValue;
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(code);
                    throw;
                }
            }
        }

        foreach (string code in codeList)
        {
            if (ResultFormulaExpressionDic.TryGetValue(code, out var formulaExp))
            {
                foreach ((string? key, object? _) in formulaExp.Parameters)
                {
                    if (internalValueDic.TryGetValue(key, out object? internalValue))
                    {
                        formulaExp.Parameters[key] = internalValue;
                    }
                }

                try
                {
                    if (double.TryParse(formulaExp.Evaluate().ToString(), out double value))
                    {
                        result.Add(code, value);
                        //paramList.Add($"finalValue:{value}");
                    }
                    else
                    {
                        result.Add(code, null);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(code);
                    throw;
                }
            }
        }

        return result;
    }
}