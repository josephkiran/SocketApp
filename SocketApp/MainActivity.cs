using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketApp
{
    [Activity(Label = "SocketApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        #region Global Values
        // Global Values {
        int i;
        JKSocket client; // Creates a TCP Client
        NetworkStream stream; //Creats a NetworkStream (used for sending and receiving data)
        byte[] datalength = new byte[4]; // creates a new byte with length 4 ( used for receivng data's lenght)
        Button buttonConnect;// = FindViewById(Resource.Id._buttonConnect);
        Button buttonSend;// = FindViewById(Resource.Id._buttonSend);
        EditText etextSend;// = FindViewById(Resource.Id._etextSend);
        TextView textReceive;
        // Global Values }
        #endregion

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            // Get our button from the layout resource,
            // and attach an event to it
            // UI init {
            buttonConnect = FindViewById<Button>(Resource.Id._buttonConnect);
            buttonSend = FindViewById<Button>(Resource.Id._buttonSend);
            etextSend = FindViewById<EditText>(Resource.Id._etextSend);
            textReceive = FindViewById<TextView>(Resource.Id._textviewReceive);
            buttonSend.Enabled = false;
            // UI init }
            buttonConnect.Click += buttonConnect_Click;
            buttonSend.Click += buttonSend_Click;
            //ThreadPool.QueueUserWorkItem(o => receiveData());
        }

        void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (buttonConnect.Text == "Connect")
                {
                    client = new JKSocket("192.168.2.8", 1800); //Trys to Connect
                    Task t = new Task(new Action(client.ConnectToServer));
                    client.RecdDataEvent = recvText;
                    client.StatusEvent = ClientStatusUpdate;
                    t.Start();
                    
                    Toast.MakeText(this, "Connecting", ToastLength.Short).Show();
                }
                else
                {
                    buttonConnect.Text = "Connect";
                    Toast.MakeText(this, "Dis-Connected", ToastLength.Short).Show();
                    client.Stop();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void recvText(string msg)
        {
            this.RunOnUiThread(() => this.textReceive.Text = textReceive.Text + (msg) +
                                   System.Environment.NewLine);
        }

        public void ClientStatusUpdate(ConnectionStatus cs, string msg)
        {
            if (cs == ConnectionStatus.Connected)
            {
                this.RunOnUiThread(() => Toast.MakeText(this, "Connected", ToastLength.Short).Show());

            }
            else
            {
                this.RunOnUiThread(() => this.textReceive.Text = textReceive.Text + (msg) +
                                   System.Environment.NewLine);
            }

        }


        void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                client.SendData(this.etextSend.Text);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }


    }
}

