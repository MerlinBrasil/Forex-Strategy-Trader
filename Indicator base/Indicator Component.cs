// IndicatorComp Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Indicator's component.
    /// </summary>
    public class IndicatorComp
    {
        private string   compName;
        private IndComponentType dataType;
        private IndChartType     chartType;
        private Color    chartColor;
        private int      firstBar;
        private int      previous;
        private bool     isDynInfo;
        private double[] values;
        private PositionPriceDependence posPriceDependence;

        /// <summary>
        /// The component's name
        /// </summary>
        public string CompName { get { return compName; } set { compName = value; } }

        /// <summary>
        /// The component's data type
        /// </summary>
        public IndComponentType DataType { get { return dataType; } set { dataType = value; } }

        /// <summary>
        /// The component's chart type
        /// </summary>
        public IndChartType ChartType { get { return chartType; } set { chartType = value; } }

        /// <summary>
        /// The component's chart color
        /// </summary>
        public Color ChartColor { get { return chartColor; } set { chartColor = value; } }

        /// <summary>
        /// The component's first bar
        /// </summary>
        public int FirstBar { get { return firstBar; } set { firstBar = value; } }

        /// <summary>
        /// The indicator uses the previous bar value
        /// </summary>
        public int UsePreviousBar { get { return previous; } set { previous = value; } }

        /// <summary>
        /// Whether the component has to be shown on dynamic info or not?
        /// </summary>
        public bool ShowInDynInfo { get { return isDynInfo; } set { isDynInfo = value; } }

        /// <summary>
        /// Whether the component depends on the position entry price or not?
        /// </summary>
        public PositionPriceDependence PosPriceDependence { get { return posPriceDependence; } set { posPriceDependence = value; } }

        /// <summary>
        /// The component's data value
        /// </summary>
        public double[] Value { get { return values; } set { values = value; } }

        /// <summary>
        /// Public constructor
        /// </summary>
        public IndicatorComp()
        {
            compName   = "Not defined";
            dataType   = IndComponentType.NotDefined;
            chartType  = IndChartType.NoChart;
            chartColor = Color.Red;
            firstBar   = 0;
            previous   = 0;
            isDynInfo  = true;
            values     = new double[] { };
            posPriceDependence = PositionPriceDependence.None;
        }

        /// <summary>
        /// Returns a copy
        /// </summary>
        public IndicatorComp Clone()
        {
            IndicatorComp component = new IndicatorComp();

            component.compName   = compName;
            component.dataType   = dataType;
            component.chartType  = chartType;
            component.chartColor = chartColor;
            component.firstBar   = firstBar;
            component.previous   = previous;
            component.isDynInfo  = isDynInfo;
            component.posPriceDependence = posPriceDependence;

            if (values != null)
            {
                component.values = new double[values.Length];
                values.CopyTo(component.values, 0);
            }

            return component;
        }
    }
}
