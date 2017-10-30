using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FormReceiver
{
    public partial class Form1 : Form
    {
        private Thread _thread;
        private readonly SynchronizationContext _syncContext;

        public Action ReceiveAction => delegate
        {
            SetText(label1, "start");
            var factory = new ConnectionFactory() {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                Port = 6012
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel()) {
                channel.QueueDeclare(queue: "hello",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    SetText(label1, message);
                    AddItem(listBox1, message);
                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: "hello",
                    autoAck: true,
                    consumer: consumer);

                Console.WriteLine(" Thread Sleep");
                Thread.Sleep(Timeout.Infinite);
                Console.WriteLine(" END");
            }
        };

        public Form1()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //这里选择Thread而不使用Task的原因:
            //此任务是个监听任务, 需要保持执行, 并可手动停止.
            //而Task类没有手动停止的功能, 只能等待任务完成.
            _thread = new Thread(() => { ReceiveAction(); });
            _thread.Start();
            button2.Enabled = true;
            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _thread.Abort();
            button2.Enabled = false;
            button1.Enabled = true;
        }

        private void SetText(Control ctrl, string text)
        {
            _syncContext.Post(state => ctrl.Text = state.ToString(), text);
        }

        private void AddItem(ListBox lb, string item)
        {
            _syncContext.Post(state => lb.Items.Add(state), item);
        }
    }
}
