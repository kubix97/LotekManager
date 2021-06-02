using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LotekManager.Models
{
	class Ball
	{
		public Frame Frame { get; set; }
		public Label Label { get; set; }
		public Ball (int number = 0, int size = 40, int radius = 20)
		{
			Frame = new Frame
			{
				HeightRequest = size,
				WidthRequest = size,
				CornerRadius = radius,
				Margin = 0,
				Padding = 0,
				BackgroundColor = Color.Gold,
				BorderColor = Color.LightSteelBlue,
				HasShadow = true
			};

			Label = new Label
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				Text = number.ToString(),
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.DarkSlateGray
			};

			Frame.Content = Label;
		}
	}
}
