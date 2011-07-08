// NUD
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// New NumericUpDown
    /// </summary>
    public class NUD : NumericUpDown
    {
        protected override void OnValueChanged(EventArgs e)
        {
            ForeColor = Color.Black;
            base.OnValueChanged(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            decimal decValue;
            if (decimal.TryParse(Text, out decValue))
            {
                if (Minimum <= decValue && decValue <= Maximum)
                {
                    ForeColor = Color.Black;
                    Value = decValue;
                }
                else
                {
                    ForeColor = Color.Red;
                }
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (Value + Increment <= Maximum)
                    Value += Increment;
            }
            else
            {
                if (Value - Increment >= Minimum)
                    Value -= Increment;
            }
        }
    }
}
