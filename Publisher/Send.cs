using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Publisher
{
    internal class Send
    {
        public static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++) {
                GoSend(i);
                Thread.Sleep(3000);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        public static void GoSend(int s)
        {
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

                string message = "Hello World!" + s;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "hello",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }
        }
    }
}
