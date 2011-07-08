// Various Enumerations
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// The type of the indicator.
    /// It serves for arrangement of the indicators in the
    /// indicator properties dialog window.
    /// This doesn't affect the indicator application.
    /// </summary>
    public enum TypeOfIndicator
    {
        Indicator,
        Additional,
        OscillatorOfIndicators,
        IndicatorsMA,
        DateTime
    }

    /// <summary>
    /// The type of the time execution indicator.
    /// It is used with the indicators, which set opening / closing position price.
    /// </summary>
    public enum ExecutionTime
    {
        DuringTheBar,   // The opening / closing price can be everywhere in the bar.
        AtBarOpening,   // The opening / closing price is at the beginning of the bar.
        AtBarClosing,   // The opening / closing price is at the end of the bar.
        CloseAndReverse // For the close and reverse logic .
    }

    /// <summary>
    /// The type of indicator component
    /// </summary>
    public enum IndComponentType
    {
        NotDefined,
        OpenLongPrice,   // Contains the long positions opening price.
        OpenShortPrice,  // Contains the short positions opening price.
        OpenPrice,       // Contains the long or short positions opening price.
        CloseLongPrice,  // Contains the long positions closing price.
        CloseShortPrice, // Contains the short positions closing price.
        ClosePrice,      // Contains the long or short positions closing price.
        OpenClosePrice,  // SlotTypes.Close or SlotTypes.Close
        IndicatorValue,  // It's not a signal or opening / closing price
        AllowOpenLong,   // Long positions opening signal
        AllowOpenShort,  // Short positions opening signal
        ForceCloseLong,  // Long positions closing signal
        ForceCloseShort, // Short positions closing signal
        ForceClose,      // Long or Short positions closing signal
        Other
    }

    /// <summary>
    /// The type of base price.
    /// </summary>
    public enum BasePrice
    {
        Open,
        High,
        Low,
        Close,
        Median,  // adPrice[bar] = (Low[bar] + High[bar]) / 2;
        Typical, // adPrice[bar] = (Low[bar] + High[bar] + Close[bar]) / 3;
        Weighted // adPrice[bar] = (Low[bar] + High[bar] + 2 * Close[bar]) / 4;
    }

    /// <summary>
    /// The method of Moving Average used for the calculations
    /// </summary>
    public enum MAMethod
    {
        Simple,
        Weighted,
        Exponential,
        Smoothed
    }

    /// <summary>
    /// Indicator Logic
    /// </summary>
    public enum IndicatorLogic
    {
        The_indicator_rises,
        The_indicator_falls,
        The_indicator_is_higher_than_the_level_line,
        The_indicator_is_lower_than_the_level_line,
        The_indicator_crosses_the_level_line_upward,
        The_indicator_crosses_the_level_line_downward,
        The_indicator_changes_its_direction_upward,
        The_indicator_changes_its_direction_downward,
        The_price_buy_is_higher_than_the_indicator_value,
        The_price_buy_is_lower_than_the_indicator_value,
        The_price_open_is_higher_than_the_indicator_value,
        The_price_open_is_lower_than_the_indicator_value,
        It_does_not_act_as_a_filter,
    }

    /// <summary>
    /// Band Indicators Logic
    /// </summary>
    public enum BandIndLogic
    {
        The_bar_opens_below_the_Upper_Band,
        The_bar_opens_above_the_Upper_Band,
        The_bar_opens_below_the_Lower_Band,
        The_bar_opens_above_the_Lower_Band,
        The_position_opens_below_the_Upper_Band,
        The_position_opens_above_the_Upper_Band,
        The_position_opens_below_the_Lower_Band,
        The_position_opens_above_the_Lower_Band,
        The_bar_opens_below_the_Upper_Band_after_opening_above_it,
        The_bar_opens_above_the_Upper_Band_after_opening_below_it,
        The_bar_opens_below_the_Lower_Band_after_opening_above_it,
        The_bar_opens_above_the_Lower_Band_after_opening_below_it,
        The_bar_closes_below_the_Upper_Band,
        The_bar_closes_above_the_Upper_Band,
        The_bar_closes_below_the_Lower_Band,
        The_bar_closes_above_the_Lower_Band,
        It_does_not_act_as_a_filter
    }

    /// <summary>
    /// Show dependence from the position's opening price
    /// </summary>
    public enum PositionPriceDependence
    {
        None,
        PriceBuyHigher,
        PriceBuyLower,
        PriceSellHigher,
        PriceSellLower,
        BuyHigherSellLower,
        BuyLowerSelHigher,
        PriceBuyCrossesUpBandInwards,
        PriceBuyCrossesUpBandOutwards,
        PriceBuyCrossesDownBandInwards,
        PriceBuyCrossesDownBandOutwards,
        PriceSellCrossesUpBandInwards,
        PriceSellCrossesUpBandOutwards,
        PriceSellCrossesDownBandInwards,
        PriceSellCrossesDownBandOutwards
    }

    /// <summary>
    /// The type of indicator chart.
    /// Sets the chart type of an indicator component.
    /// </summary>
    public enum IndChartType
    {
        NoChart,   // This component is not drawn on a chart.
        Line,      // Line for the main or a separated chart: Alligator, MA, RSI, Momentum
        Dot,       // For the main chart: Parabolic SAR
        Histogram, // Histogram on a separated chart: MACD Histogram, Aroon
        Level,     // Fibonacci, Donchian Channel 
        CloudUp,   // Ichimoku Kinko Hyo
        CloudDown  // Ichimoku Kinko Hyo
    }

    /// <summary>
    /// Describes the type of the strategy slots.
    /// </summary>
    [FlagsAttribute]
    public enum SlotTypes : short
    {
        NotDefined  = 0,
        Open        = 1, // Opening Point of the Position
        OpenFilter  = 2, // Opening Logic Condition
        Close       = 4, // Closing Point of the Position
        CloseFilter = 8  // Closing Logic Condition
    }
}
