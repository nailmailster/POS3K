using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;

namespace POS3K
{
	public class Transition_Ripple_modified_with_MonochromeEffect : ShaderEffect
    {
		public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(Transition_Ripple_modified_with_MonochromeEffect), 0);
		public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(Transition_Ripple_modified_with_MonochromeEffect), new UIPropertyMetadata(((double)(30D)), PixelShaderConstantCallback(0)));
		public static readonly DependencyProperty FilterColorProperty = DependencyProperty.Register("FilterColor", typeof(Color), typeof(Transition_Ripple_modified_with_MonochromeEffect), new UIPropertyMetadata(Color.FromArgb(255, 0, 0, 0), PixelShaderConstantCallback(1)));

        public Transition_Ripple_modified_with_MonochromeEffect()
        {
			PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(@"C:\Registan\POS\Shaders\Transition_Ripple_modified_with_MonochromeEffect.ps", UriKind.Absolute);
            this.PixelShader = pixelShader;

			this.UpdateShaderValue(InputProperty);
			this.UpdateShaderValue(ProgressProperty);
			this.UpdateShaderValue(FilterColorProperty);
		}

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

        public double Progress
        {
			get
            {
				return ((double)(this.GetValue(ProgressProperty)));
			}
			set
            {
				this.SetValue(ProgressProperty, value);
			}
		}

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
