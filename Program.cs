using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApplication1.Properties;
using Autofac;
using Castle.DynamicProxy;

namespace WindowsFormsApplication1
{
     public class Program
    {
        static readonly ProxyGenerator Generator = new ProxyGenerator(new PersistentProxyBuilder());
        static readonly ProxyGenerationOptions XOptions = new ProxyGenerationOptions() { Hook = new AllMethodsHook() };
        static readonly ContainerBuilder Builder = new Autofac.ContainerBuilder();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Builder.RegisterAssemblyTypes(typeof(Button).Assembly);
            Builder.RegisterAssemblyTypes(typeof(Program).Assembly);

            Builder.Register(context => Makeclass<Mybutton>(context.Resolve<Buttonx>()));
            Builder.Register(context => Makeclass<Showmemessage>(context.Resolve<Cutsomeinfomation>()));

            IContainer service = Builder.Build();
           // var makeclass = Makeclass<Mybutton>(new Buttonx());

            Application.Run(new Myfroms(service.Resolve<Mybutton>(),service.Resolve<Showmemessage>()));
        }

         public static T Makeclass<T>( params IInterceptor[] many) where T : class,new()
         {
             Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add(
                typeof(System.Security.Permissions.UIPermissionAttribute));
             return  Generator.CreateClassProxy<T>(XOptions,many);
         }
    }

    public class Buttonx : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {

            if (invocation.Method.Name.EndsWith("OnClick"))
            {
                var mouseEventArgs = invocation.Arguments[0] as MouseEventArgs;

                if (mouseEventArgs != null) MessageBox.Show("helloworld ->" + mouseEventArgs.Location);
            }
            invocation.Proceed();

        }
    }

    public class Cutsomeinfomation : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.EndsWith("OnKeyPress"))
            {
                var arguments = invocation.Arguments[0] as KeyPressEventArgs;

                if (arguments != null)
                {
                    var keyChar = arguments.KeyChar;
                    MessageBox.Show("helloworld ->" +keyChar);
                }
            }
            invocation.Proceed();
        }
    }


    public class Showmemessage : TextBox
    {
        public Showmemessage()
        {
            Width = 50;
            Height = 50;
          
            Name = "passworld";
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }
    }



    public class Mybutton : Button
    {
        public Mybutton()
        {
            Name = "B1";
            Text = "fuckme";
        }

        protected override void OnClick(EventArgs e)
        {
            var mouseEventArgs = e as MouseEventArgs;

            var name = Parent.Name;

            MessageBox.Show(name);
        }

    }

    public class Myfroms : Form
    {
        public IEnumerable<Control> Xx;
        private readonly Point _orgion = new Point(0, 0);

        public Myfroms(params  Control[] inputs)
        {
            Name = "main";
            Width = 400;
            Height = 400;
            Xx = inputs;
            Icon = Resources.cat;
            foreach (var control in inputs)
            {
                control.Location = _orgion;
                _orgion.X += 100;
                _orgion.Y += 100;
                control.Size = new Size(100, 100);
                Controls.Add(control);
            }
          ResumeLayout(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var control in Xx)
                {
                    control.Dispose();

                }
                base.Dispose(true);
            }
        }
    }
   

}
