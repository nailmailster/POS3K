using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace POS3K
{
    class Sine1Effect : ShaderEffect
    {
        public Sine1Effect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(@"C:\Registan\POS\Shaders\Sine1Effect.ps", UriKind.Absolute);
            this.PixelShader = pixelShader;

            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(ParameterProperty);
        }

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(Sine1Effect), 0);
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

        //public static readonly DependencyProperty ParameterProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("SampleInputParam", typeof(Double), 0);
        public static readonly DependencyProperty ParameterProperty = DependencyProperty.Register("SampleInputParam", typeof(double), typeof(Sine1Effect), new UIPropertyMetadata(((double)(0D)), PixelShaderConstantCallback(0)));
        public double Parameter
        {
            get
            {
                return ((double)(this.GetValue(ParameterProperty)));
            }
            set
            {
                this.SetValue(ParameterProperty, value);
            }
        }
    }
}
