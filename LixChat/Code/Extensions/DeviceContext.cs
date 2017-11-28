//using System;

//using System.ComponentModel;//needed to overide OnClosing
////I removed useless usings
//using System.Windows.Forms;

//using SharpDX.Direct3D11;
//using SharpDX.DXGI;
//using SharpDX;

//namespace WindowsFormsApplication2
//{
//    using Device = SharpDX.Direct3D11.Device;
//    using Buffer = SharpDX.Direct3D11.Buffer;

//    public partial class Form1 : Form
//    {
//        Device d;
//        SwapChain sc;

//        Texture2D target;
//        RenderTargetView targetveiw;

//        public Form1()
//        {
//            InitializeComponent();

//            SwapChainDescription scd = new SwapChainDescription()
//            {
//                BufferCount = 1,                                 //how many buffers are used for writing. it's recommended to have at least 2 buffers but this is an example
//                Flags = SwapChainFlags.None,
//                IsWindowed = true,                               //it's windowed
//                ModeDescription = new ModeDescription(
//                    this.ClientSize.Width,                       //windows veiwable width
//                    this.ClientSize.Height,                      //windows veiwable height
//                    new Rational(60, 1),                          //refresh rate
//                    Format.R8G8B8A8_UNorm),                      //pixel format, you should resreach this for your specific implementation

//                OutputHandle = this.Handle,                      //the magic

//                SampleDescription = new SampleDescription(1, 0), //the first number is how many samples to take, anything above one is multisampling.
//                SwapEffect = SwapEffect.Discard,
//                Usage = Usage.RenderTargetOutput
//            };

//            Device.CreateWithSwapChain(
//                SharpDX.Direct3D.DriverType.Hardware,//hardware if you have a graphics card otherwise you can use software
//                DeviceCreationFlags.Debug,           //helps debuging don't use this for release verion
//                scd,                                 //the swapchain description made above
//                out d, out sc                        //our directx objects
//                );

//            target = Texture2D.FromSwapChain<Texture2D>(sc, 0);
//            targetveiw = new RenderTargetView(d, target);

//            d.ImmediateContext.OutputMerger.SetRenderTargets(targetveiw);

//        }

//        protected override void OnClosing(CancelEventArgs e)
//        {
//            //dipose of all objects
//            d.Dispose();
//            sc.Dispose();
//            target.Dispose();
//            targetveiw.Dispose();

//            base.OnClosing(e);
//        }

//        protected override void OnPaint(PaintEventArgs e)
//        {
//            //I am rendering here for this example
//            //normally I use a seperate thread to call Draw() and Present() in a loop
//            d.ImmediateContext.ClearRenderTargetView(targetveiw, Color.CornflowerBlue);//Color to make it look like default XNA project output.
//            sc.Present(0, PresentFlags.None);

//            base.OnPaint(e);
//        }

//    }
//}