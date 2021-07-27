using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;

namespace POS3K
{
    public class Transition_Ripple_modifiedEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(Transition_Ripple_modifiedEffect), 0);
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(Transition_Ripple_modifiedEffect), new UIPropertyMetadata(((double)(30D)), PixelShaderConstantCallback(0)));


        public Transition_Ripple_modifiedEffect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(@"C:\Registan\POS\Shaders\Transition_Ripple_modifiedEffect2.ps", UriKind.Absolute);
            this.PixelShader = pixelShader;

            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(ProgressProperty);
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
    }
}
