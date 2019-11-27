// 
// ProgressBarBackend.cs
//  
// Author:
//       Andres G. Aragoneses <knocte@gmail.com>
// 
// Copyright (c) 2012 Andres G. Aragoneses
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Linq;
using System.Windows;
using System.Windows.Media;
using Xwt.Backends;

namespace Xwt.WPFBackend
{
	public class ProgressBarBackend: WidgetBackend, IProgressBarBackend
	{
		protected override void Initialize()
		{
			base.Initialize();

			var progressBar = new System.Windows.Controls.ProgressBar();
			Widget = progressBar;
			progressBar.Maximum = 1.0;
			progressBar.Minimum = 0.0;
			progressBar.Value = 0.0;

			progressBar.IsIndeterminate = false;
		}

		Style style;
		Xwt.Drawing.Color foregroundColor = Xwt.Drawing.Colors.Green;
		void UpdateStyle ()
		{
			var wpfColorBrush = ResPool.GetSolidBrush (foregroundColor.ToWpfColor ());
			if (style == null) {
				style = new Style (typeof (System.Windows.Controls.ProgressBar));
				style.Setters.Add (new Setter (System.Windows.Controls.ProgressBar.ForegroundProperty, wpfColorBrush));
				Widget.Style = style;
				return;
			}

			var setter = style.Setters
				.FirstOrDefault (s => (s as Setter).Property == System.Windows.Controls.ProgressBar.ForegroundProperty)
					as Setter;
			if (setter != null)
				setter.Value = wpfColorBrush;
		}

		public Xwt.Drawing.Color Color {
			get {
				return foregroundColor;
			}
			set {
				if (foregroundColor == value)
					return;

				foregroundColor = value;
				UpdateStyle ();
			}
		}

		public void SetFraction (double fraction)
		{
			var widget = (System.Windows.Controls.ProgressBar) Widget;

			if (widget.Value == fraction)
				return;

			widget.Value = fraction;
		}

		public void SetIndeterminate (bool value) {
			var widget = (System.Windows.Controls.ProgressBar) Widget;

			if (widget.IsIndeterminate == value)
				return;

			widget.IsIndeterminate = value;
		}
	}
}
