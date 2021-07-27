using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace POS3K
{
	public class MonochromeEffect : ShaderEffect
    {
		public MonochromeEffect()
        {
			PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(@"C:\Registan\POS\Shaders\MonochromeEffect.ps", UriKind.Absolute);
            this.PixelShader = pixelShader;

			this.UpdateShaderValue(InputProperty);
			this.UpdateShaderValue(FilterColorProperty);
		}

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(MonochromeEffect), 0);
        public Brush Input
        {
			get
            {
				return ((Brush)(this.GetValue(InputProperty)));
			}
			set
            {
				this.SetValue(InputProperty, value);
			}
		}

        public static readonly DependencyProperty FilterColorProperty = DependencyProperty.Register("FilterColor", typeof(Color), typeof(MonochromeEffect), new UIPropertyMetadata(Color.FromArgb(255, 255, 255, 0), PixelShaderConstantCallback(0)));
        public Color FilterColor
        {
			get
            {
				return ((Color)(this.GetValue(FilterColorProperty)));
			}
			set
            {
				this.SetValue(FilterColorProperty, value);
			}
		}
	}
}
