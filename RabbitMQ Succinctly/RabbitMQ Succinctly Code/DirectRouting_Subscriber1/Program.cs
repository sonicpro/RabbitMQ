﻿using System;
using RabbitMQ.Client;

namespace RabbitMQ.Examples
{
    class Program
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;

        private const string ExchangeName = "DirectRouting_Exchange";

        static void Main()
        {
            _factory = new ConnectionFactory { HostName = "localhost", UserName = "Vovan", Password = "peacemaker" };
            using (_connection = _factory.CreateConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.ExchangeDeclare(ExchangeName, "direct");
                    var queueName = channel.QueueDeclare().QueueName;
                                     
                    channel.QueueBind(queueName, ExchangeName, "CardPayment");
                   
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(queueName, true, consumer);

                    while (true)
                    {                        
                        var ea = consumer.Queue.Dequeue();
                        var message = (Payment)ea.Body.DeSerialize();                        
                        var routingKey = ea.RoutingKey;
                        Console.WriteLine("--- Payment - Key <{0}> : {1} : {2}", routingKey, message.CardNumber, message.AmountToPay);
                    }
                }
            }
        }
    }
}
