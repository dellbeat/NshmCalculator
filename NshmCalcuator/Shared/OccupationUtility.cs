using NCalc;
using NshmCalculator.Shared.Models.CalculatorModel;
using NshmCalculator.Shared.Models.CalculatorModel.FormulaParam.Formula;

namespace NshmCalculator.Shared;

public static class OccupationUtility
{
    /// <summary>
    /// 计算公式结果
    /// </summary>
    /// <param name="occupationFormulas"></param>
    /// <param name="valueDic"></param>
    /// <returns></returns>
    public static double?[] CalculateResults(OccupationFormulas[] occupationFormulas, Dictionary<string, int> valueDic)
    {
        double?[] valueArray = new double?[occupationFormulas.Length];

        for (var i = 0; i < occupationFormulas.Length; i++)
        {
            string formula = occupationFormulas[i].Formula.Replace("IF", "if");
            foreach (string para in occupationFormulas[i].FormulaParam)
            {
                formula = formula.Replace(para, valueDic[para].ToString());
            }

            try
            {
                valueArray[i] = Convert.ToDouble(new Expression(formula).Evaluate());
            }
            catch (Exception)
            {
                valueArray[i] = null;
            }
        }

        return valueArray;
    }
}