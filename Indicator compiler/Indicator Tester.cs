// Indicator_Tester Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Text;

namespace Forex_Strategy_Trader
{
    public static class Indicator_Tester
    {
        /// <summary>
        /// Tests general parameters of a custom indicator.
        /// </summary>
        /// <returns>True, if the test succeed.</returns>
        public static bool CustomIndicatorFastTest(Indicator indicator, out string errorList)
        {
            bool isOk = true;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("ERROR: Indicator test failed for the '" + indicator.IndicatorName + "' indicator.");

            // Tests the IndicatorName property.
            if (string.IsNullOrEmpty(indicator.IndicatorName))
            {
                sb.AppendLine("\tThe property 'IndicatorName' is not set.");
                isOk = false;
            }

            // Tests the PossibleSlots property.
            if (!indicator.TestPossibleSlot(SlotTypes.Open)       && 
                !indicator.TestPossibleSlot(SlotTypes.OpenFilter) && 
                !indicator.TestPossibleSlot(SlotTypes.Close)      && 
                !indicator.TestPossibleSlot(SlotTypes.CloseFilter))
            {
                sb.AppendLine("\tThe property 'PossibleSlots' is not set.");
                isOk = false;
            }

            // Tests the CustomIndicator property.
            if (!indicator.CustomIndicator)
            {
                sb.AppendLine("\tThe indicator '" + indicator.IndicatorName + "' is not marked as custom. Set CustomIndicator = true;");
                isOk = false;
            }

            // Tests the SeparatedChartMaxValue properties.
            if (!indicator.SeparatedChart && indicator.SeparatedChartMaxValue != double.MinValue)
            {
                sb.AppendLine("\tSet SeparatedChart = true; or remove the property: SeparatedChartMaxValue = " + indicator.SeparatedChartMaxValue.ToString());
                isOk = false;
            }

            // Tests the SeparatedChartMinValue properties.
            if (!indicator.SeparatedChart && indicator.SeparatedChartMinValue != double.MaxValue)
            {
                sb.AppendLine("\tSet SeparatedChart = true; or remove the property: SeparatedChartMinValue = " + indicator.SeparatedChartMinValue.ToString());
                isOk = false;
            }

            // Tests the SpecialValues properties.
            if (!indicator.SeparatedChart && indicator.SpecialValues.Length > 0)
            {
                sb.AppendLine("\tSet SeparatedChart = true; or remove the property SpecialValues");
                isOk = false;
            }

            // Tests the IndParam properties.
            if (indicator.IndParam == null)
            {
                sb.AppendLine("\tThe property IndParam is not set. Set IndParam = new IndicatorParam();");
                isOk = false;
            }

            // Tests the IndParam.IndicatorName properties.
            if (indicator.IndParam.IndicatorName != indicator.IndicatorName)
            {
                sb.AppendLine("\tThe property IndParam.IndicatorName is not set. Set IndParam.IndicatorName = IndicatorName;");
                isOk = false;
            }

            // Tests the IndParam.SlotType properties.
            if (indicator.IndParam.SlotType != SlotTypes.NotDefined)
            {
                sb.AppendLine("\tThe property IndParam.SlotType is not set. Set IndParam.SlotType = slotType;");
                isOk = false;
            }

            // Tests the IndParam.ListParam properties.
            for (int iParam = 0; iParam < indicator.IndParam.ListParam.Length; iParam++)
            {
                ListParam listParam = indicator.IndParam.ListParam[iParam];
                if (!listParam.Enabled)
                    continue;

                if (string.IsNullOrEmpty(listParam.Caption))
                {
                    sb.AppendLine("\tThe property IndParam.ListParam[" + iParam + "].Caption is not set.");
                    isOk = false;
                }

                if (listParam.ItemList.Length == 0)
                {
                    sb.AppendLine("\tThe property IndParam.ListParam[" + iParam + "].ItemList is not set.");
                    isOk = false;
                }

                if (listParam.ItemList[listParam.Index] != listParam.Text)
                {
                    sb.AppendLine("\tThe property IndParam.ListParam[" + iParam + "].Text is wrong." +
                        " Set " + "IndParam.ListParam[" + iParam + "].Text = IndParam.ListParam[" + iParam + 
                        "].ItemList[IndParam.ListParam[" + iParam + "].Index];");
                    isOk = false;
                }

                if (string.IsNullOrEmpty(listParam.ToolTip))
                {
                    sb.AppendLine("\tThe property IndParam.ListParam[" + iParam + "].ToolTip is not set.");
                    isOk = false;
                }
            }

            // Tests the IndParam.NumParam properties.
            for (int param = 0; param < indicator.IndParam.NumParam.Length; param++)
            {
                NumericParam numParam = indicator.IndParam.NumParam[param];
                if (!numParam.Enabled)
                    continue;

                if (string.IsNullOrEmpty(numParam.Caption))
                {
                    sb.AppendLine("\tThe property IndParam.NumParam[" + param + "].Caption is not set.");
                    isOk = false;
                }

                double value = numParam.Value;
                double min   = numParam.Min;
                double max   = numParam.Max;
                double point = numParam.Point;

                if (min > max)
                {
                    sb.AppendLine("\tIndParam.NumParam[" + param + "].Min > IndParam.NumParam[" + param + "].Max.");
                    isOk = false;
                }

                if (value > max)
                {
                    sb.AppendLine("\tIndParam.NumParam[" + param + "].Value > IndParam.NumParam[" + param + "].Max.");
                    isOk = false;
                }

                if (value < min)
                {
                    sb.AppendLine("\tIndParam.NumParam[" + param + "].Value < IndParam.NumParam[" + param + "].Min.");
                    isOk = false;
                }

                if (point < 0)
                {
                    sb.AppendLine("\tIndParam.NumParam[" + param + "].Point cannot be < 0");
                    isOk = false;
                }

                if (point > 6)
                {
                    sb.AppendLine("\tIndParam.NumParam[" + param + "].Point cannot be > 6");
                    isOk = false;
                }

                if (string.IsNullOrEmpty(numParam.ToolTip))
                {
                    sb.AppendLine("\tThe property IndParam.NumParam[" + param + "].ToolTip is not set.");
                    isOk = false;
                }
            }

            // Tests the IndParam.CheckParam properties.
            for (int param = 0; param < indicator.IndParam.CheckParam.Length; param++)
            {
                CheckParam checkParam = indicator.IndParam.CheckParam[param];
                if (!checkParam.Enabled)
                    continue;

                if (string.IsNullOrEmpty(checkParam.Caption))
                {
                    sb.AppendLine("\tThe property IndParam.CheckParam[" + param + "].Caption is not set.");
                    isOk = false;
                }

                if (string.IsNullOrEmpty(checkParam.ToolTip))
                {
                    sb.AppendLine("\tThe property IndParam.CheckParam[" + param + "].ToolTip is not set.");
                    isOk = false;
                }
            }

            try
            {
                indicator.Calculate(SlotTypes.NotDefined);
            }
            catch (System.Exception exc)
            {
                sb.AppendLine("\tError when executing Calculate(SlotTypes.NotDefined). " + exc.Message);
                isOk = false;
            }

            try
            {
                indicator.SetDescription(SlotTypes.NotDefined);
            }
            catch (System.Exception exc)
            {
                sb.AppendLine("\tError when executing SetDescription(SlotTypes.NotDefined). " + exc.Message);
                isOk = false;
            }

            try
            {
                indicator.ToString();
            }
            catch (System.Exception exc)
            {
                sb.AppendLine("\tError when executing ToString(). " + exc.Message);
                isOk = false;
            }

            if (isOk)
                errorList = string.Empty;
            else
                errorList = sb.ToString();

            return isOk;
        }

        /// <summary>
        /// Performs thorough indicator test.
        /// </summary>
        /// <returns>True, if the test succeed.</returns>
        public static bool CustomIndicatorThoroughTest(string indicatorName, out string errorList)
        {
            bool isOk = true;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("ERROR: Indicator test failed for the '" + indicatorName + "' indicator.");

            foreach (SlotTypes slotType in  Enum.GetValues(typeof(SlotTypes)))
            {
                if (slotType == SlotTypes.NotDefined)
                    continue;

                Indicator indicator = Indicator_Store.ConstructIndicator(indicatorName, slotType);

                if (!indicator.TestPossibleSlot(slotType))
                    continue;

                foreach (NumericParam numParam in indicator.IndParam.NumParam)
                    if (numParam.Enabled)
                        numParam.Value = numParam.Min;

                try
                {
                    indicator.Calculate(slotType);
                }
                catch (System.Exception exc)
                {
                    sb.AppendLine("\tError when calculating with NumParams set to their minimal values. " + exc.Message);
                    isOk = false;
                    break;
                }

                foreach (NumericParam numParam in indicator.IndParam.NumParam)
                    if (numParam.Enabled)
                        numParam.Value = numParam.Max;

                try
                {
                    indicator.Calculate(slotType);
                }
                catch (System.Exception exc)
                {
                    sb.AppendLine("\tError when calculating with NumParams set to their maximal values. " + exc.Message);
                    isOk = false;
                    break;
                }

                try
                {
                    foreach (IndicatorComp component in indicator.Component)
                    {
                        switch (slotType)
                        {
                            case SlotTypes.Open:
                                if (component.DataType == IndComponentType.AllowOpenLong   ||
                                    component.DataType == IndComponentType.AllowOpenShort  ||
                                    component.DataType == IndComponentType.CloseLongPrice  ||
                                    component.DataType == IndComponentType.ClosePrice      ||
                                    component.DataType == IndComponentType.CloseShortPrice ||
                                    component.DataType == IndComponentType.ForceClose      ||
                                    component.DataType == IndComponentType.ForceCloseLong  ||
                                    component.DataType == IndComponentType.ForceCloseShort ||
                                    component.DataType == IndComponentType.NotDefined)
                                {
                                    sb.AppendLine("\tProbably wrong component type when SlotType is 'SlotTypes.Open' - '" + component.CompName + "' of type '" + component.DataType + "'.");
                                    isOk = false;
                                }
                                break;
                            case SlotTypes.OpenFilter:
                                if (component.DataType == IndComponentType.OpenClosePrice  ||
                                    component.DataType == IndComponentType.OpenLongPrice   ||
                                    component.DataType == IndComponentType.OpenPrice       ||
                                    component.DataType == IndComponentType.OpenShortPrice  ||
                                    component.DataType == IndComponentType.CloseLongPrice  ||
                                    component.DataType == IndComponentType.ClosePrice      ||
                                    component.DataType == IndComponentType.CloseShortPrice ||
                                    component.DataType == IndComponentType.ForceClose      ||
                                    component.DataType == IndComponentType.ForceCloseLong  ||
                                    component.DataType == IndComponentType.ForceCloseShort ||
                                    component.DataType == IndComponentType.NotDefined)
                                {
                                    sb.AppendLine("\tProbably wrong component type when SlotType is 'SlotTypes.OpenFilter' - '" + component.CompName + "' of type '" + component.DataType + "'.");
                                    isOk = false;
                                }
                                break;
                            case SlotTypes.Close:
                                if (component.DataType == IndComponentType.AllowOpenLong   ||
                                    component.DataType == IndComponentType.AllowOpenShort  ||
                                    component.DataType == IndComponentType.OpenLongPrice   ||
                                    component.DataType == IndComponentType.OpenPrice       ||
                                    component.DataType == IndComponentType.OpenShortPrice  ||
                                    component.DataType == IndComponentType.ForceClose      ||
                                    component.DataType == IndComponentType.ForceCloseLong  ||
                                    component.DataType == IndComponentType.ForceCloseShort ||
                                    component.DataType == IndComponentType.NotDefined)
                                {
                                    sb.AppendLine("\tProbably wrong component type when SlotType is 'SlotTypes.Close' - '" + component.CompName + "' of type '" + component.DataType + "'.");
                                    isOk = false;
                                }
                                break;
                            case SlotTypes.CloseFilter:
                                if (component.DataType == IndComponentType.AllowOpenLong   ||
                                    component.DataType == IndComponentType.AllowOpenShort  ||
                                    component.DataType == IndComponentType.OpenLongPrice   ||
                                    component.DataType == IndComponentType.OpenPrice       ||
                                    component.DataType == IndComponentType.OpenShortPrice  ||
                                    component.DataType == IndComponentType.CloseLongPrice  ||
                                    component.DataType == IndComponentType.ClosePrice      ||
                                    component.DataType == IndComponentType.CloseShortPrice ||
                                    component.DataType == IndComponentType.NotDefined)
                                {
                                    sb.AppendLine("\tProbably wrong component type when SlotType is 'SlotTypes.CloseFilter' - '" + component.CompName + "' of type '" + component.DataType + "'.");
                                    isOk = false;
                                }
                                break;
                            default:
                                break;
                        }
                        if (component.DataType == IndComponentType.AllowOpenLong   ||
                            component.DataType == IndComponentType.AllowOpenShort  ||
                            component.DataType == IndComponentType.ForceClose      ||
                            component.DataType == IndComponentType.ForceCloseLong  ||
                            component.DataType == IndComponentType.ForceCloseShort)
                        {
                            foreach(double value in component.Value)
                                if (value != 0 && value != 1)
                                {
                                    sb.AppendLine("\tWrong component values. The values of '" + component.CompName + "' must be 0 or 1.");
                                    isOk = false;
                                    break;
                                }
                        }
                    }
                }
                catch (System.Exception exc)
                {
                    sb.AppendLine("\tError when checking the indicator's components. " + exc.Message);
                    isOk = false;
                    break;
                }

            }

            if (isOk)
                errorList = string.Empty;
            else
                errorList = sb.ToString();

            return isOk;
        }
    }
}
