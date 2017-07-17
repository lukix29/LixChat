using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace System.Threading
{
    public static class CallBack
    {
        private static Control F = null;

        private delegate void AddItemCallback(object objectcallback, ListBox C);

        private delegate void SetColorCallback(Color Backcolor, Control C);

        private delegate void SetEnabledCallback(Control C, bool enabled);

        private delegate void SetLinesCallback(string[] stringcallback, RichTextBox C);

        private delegate void SetMaximumCallback(int maximumcallback, ProgressBar progressBar);

        private delegate void SetTextCallback(Control C, string stringcallback);

        private delegate void SetValueCallback(int valuecallback, ProgressBar PRGB);

        private delegate void SetValueCallbackNUD(decimal valuecallback, NumericUpDown NUD);

        private delegate void SetValueCallbackTSS(int valuecallback, ToolStripProgressBar PRGB);

        private delegate void SetVisibleCallback(Control FormControl, bool Visible);

        public static Control Main
        {
            get { return F; }
        }

        public static void AddItem(object Output, ListBox ListBox)
        {
            try
            {
                if (ListBox.InvokeRequired)
                {
                    AddItemCallback d = new AddItemCallback(AddItem);
                    F.Invoke(d, new object[] { ListBox.Items.Add(Output) });
                }
                else
                {
                    ListBox.Items.Add(Output);
                }
            }
            catch { }
        }

        public static void SetAll(bool set)
        {
            foreach (Control c in F.Controls)
            {
                SetEnabled(c, set);
            }
        }

        public static void SetAll(bool set, Control except)
        {
            foreach (Control c in F.Controls)
            {
                if (c.Location != except.Location) SetEnabled(c, set);
            }
        }

        public static void SetCallBack(Control F_Main)
        {
            F = F_Main;
        }

        public static void SetColor(Color backcolor, Control FormControl)
        {
            try
            {
                if (FormControl.InvokeRequired)
                {
                    SetColorCallback d = new SetColorCallback(SetColor);
                    F.Invoke(d, new object[] { backcolor, FormControl });
                }
                else
                {
                    FormControl.BackColor = backcolor;
                }
            }
            catch { }
        }

        public static void SetEnabled(Control FormControl, bool Enabled)
        {
            try
            {
                if (FormControl.InvokeRequired)
                {
                    SetEnabledCallback d = new SetEnabledCallback(SetEnabled);
                    F.Invoke(d, new object[] { FormControl, Enabled });
                }
                else
                {
                    FormControl.Enabled = Enabled;
                }
            }
            catch { }
        }

        public static void SetLines(string[] Output, RichTextBox FormControl)
        {
            try
            {
                if (FormControl.InvokeRequired)
                {
                    SetLinesCallback d = new SetLinesCallback(SetLines);
                    F.Invoke(d, new object[] { Output, FormControl });
                }
                else
                {
                    FormControl.Lines = Output;
                }
            }
            catch { }
        }

        public static void SetMaximum(int Maximum, ProgressBar prgB)
        {
            try
            {
                if (prgB.InvokeRequired)
                {
                    SetMaximumCallback d = new SetMaximumCallback(SetMaximum);
                    F.Invoke(d, new object[] { Maximum, prgB });
                }
                else
                {
                    prgB.Maximum = Maximum;
                }
            }
            catch { }
        }

        public static void SetText(Control FormControl, string Output)
        {
            try
            {
                if (FormControl.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    F.Invoke(d, new object[] { FormControl, Output });
                }
                else
                {
                    FormControl.Text = Output;
                }
            }
            catch { }
        }

        public static void SetValue(int Value, ProgressBar PRGB)
        {
            try
            {
                if (PRGB.InvokeRequired)
                {
                    SetValueCallback d = new SetValueCallback(SetValue);
                    F.Invoke(d, new object[] { Value, PRGB });
                }
                else
                {
                    PRGB.Value = Value;
                }
            }
            catch { }
        }

        public static void SetValueNUD(decimal Value, NumericUpDown NUD)
        {
            try
            {
                if (NUD.InvokeRequired)
                {
                    SetValueCallbackNUD d = new SetValueCallbackNUD(SetValueNUD);
                    F.Invoke(d, new object[] { Value, NUD });
                }
                else
                {
                    NUD.Value = Value;
                }
            }
            catch { }
        }

        public static void SetValueTSS(int Value, ToolStripProgressBar PRGB)
        {
            try
            {
                if (F.InvokeRequired)
                {
                    SetValueCallbackTSS d = new SetValueCallbackTSS(SetValueTSS);
                    F.Invoke(d, new object[] { Value, PRGB });
                }
                else
                {
                    PRGB.Value = Value;
                }
            }
            catch { }
        }

        public static void SetVisible(Control FormControl, bool Visible)
        {
            try
            {
                if (FormControl.InvokeRequired)
                {
                    SetVisibleCallback d = new SetVisibleCallback(SetVisible);
                    F.Invoke(d, new object[] { FormControl, Visible });
                }
                else
                {
                    FormControl.Visible = Visible;
                }
            }
            catch { }
        }
    }

    public class TDS
    {
        public System.Threading.Timer Timer;
    }

    public class ThreadManager
    {
        private int curIndex = 0;

        private int curWorkCount = 0;

        private bool isRunning = false;

        private List<Act> list = new List<Act>();

        private int maxWorkCount = 4;

        private int workCount = 0;

        public ThreadManager(int MaxWorkCnt)
        {
            maxWorkCount = MaxWorkCnt;
        }

        public delegate void FinishedHandler();

        public delegate void RefreshedHandler(int index, object o);

        public event FinishedHandler Finished;

        public event RefreshedHandler Refreshed;

        public void Add(Func<object, object, object> a, object S0, object S1)
        {
            if (!isRunning)
            {
                list.Add(new Act(a, S0, S1));
            }
        }

        public void Add(Func<object, object, object> a)
        {
            if (!isRunning)
            {
                list.Add(new Act(a, null, null));
            }
        }

        public void Add(Action<object> a)
        {
            if (!isRunning)
            {
                Add(new Func<object, object, object>(delegate(object o, object o1)
                    {
                        a.Invoke(o);
                        return null;
                    }));
            }
        }

        public void Add(Action a)
        {
            Add(new Func<object, object, object>(delegate(object o, object o1)
                {
                    a.Invoke();
                    return null;
                }), null, null);
        }

        public void Start()
        {
            if (!isRunning)
            {
                workCount = 0;
                curIndex = 0;
                curWorkCount = 0;
                isRunning = true;
                workThread();
            }
        }

        public void StartAsync()
        {
            if (!isRunning)
            {
                workCount = 0;
                curIndex = 0;
                curWorkCount = 0;
                isRunning = true;
                isRunning = true;
                new Thread(workThread).Start();
            }
        }

        private void actionThread(Act a)
        {
            object o = a.DoWork();

            curWorkCount = Math.Max(0, curWorkCount - 1);

            if (Refreshed != null) Refreshed(workCount, o);

            workCount++;
        }

        private void workThread()
        {
            int count = list.Count;
            while (workCount < count)
            {
                if (curWorkCount < maxWorkCount)
                {
                    if (curIndex < count)
                    {
                        long idx = curIndex;
                        curWorkCount++;
                        if (idx >= 0)
                        {
                            new Thread(() => actionThread(list[(int)idx])).Start();
                        }
                        curIndex++;
                    }
                }
                Thread.CurrentThread.Join(100);
            }
            //if (target != null)
            //{
            //    var Timer = ((TDS)target).Timer;
            //    Timer.Change(Timeout.Infinite, Timeout.Infinite);
            //    Timer.Dispose();
            //}

            if (Finished != null) Finished();
            isRunning = false;
        }

        private class Act
        {
            public Func<object, object, object> Action;
            public object S0;
            public object S1;

            public Act(Func<object, object, object> a, object s0, object s1)
            {
                Action = a;
                S0 = s0;
                S1 = s1;
            }

            public object DoWork()
            {
                try
                {
                    return Action.Invoke(S0, S1);
                }
                catch (Exception x)
                {
                    x.Handle();
                }
                return null;
            }
        }
    }
}