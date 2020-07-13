using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using System;

namespace Apache.NMS.Repro
{
    public static class MessageBusTest
    {
        public delegate void TextMessageHandler(string text, IMessage message);

        public static void SendReceiveMessage()
        {
            Console.WriteLine("Starting...");

            string clientId = string.Empty;
            string connectionString = "failover:(tcp://pr-rhjboss-v01:61616,tcp://pr-rhjboss-v02:61616)?randomize=false&priorityBackup=true";

            var connectionFactory = new ConnectionFactory(connectionString)
            {
                ClientId = clientId,
                AsyncSend = true,
                DispatchAsync = true,
                UseCompression = false
            };

            var connection = connectionFactory.CreateConnection();

            var session = (Session)connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            session.Start();

            session.Retroactive = false;

            //Consumer A Consumer.A.VirtualTopic.Artemis.Test queue://Consumer.A.VirtualTopic.Artemis.Test
            IDestination destinationA = BuildDestination("Consumer.A.VirtualTopic.Artemis.Test", Apache.NMS.DestinationType.Queue);
            var consumerA = (MessageConsumer)session.CreateConsumer(destinationA);

            ConfigureListener(ref consumerA, TestMethodA);

            //Consumer B
            var destinationB = BuildDestination("Consumer.B.VirtualTopic.Artemis.Test", Apache.NMS.DestinationType.Queue);
            var consumerB = (MessageConsumer)session.CreateConsumer(destinationB);

            ConfigureListener(ref consumerB, TestMethodB);

            //Consumer C
            var destinationC = BuildDestination("Consumer.C.VirtualTopic.Artemis.Test", Apache.NMS.DestinationType.Queue);
            var consumerC = (MessageConsumer)session.CreateConsumer(destinationC);

            ConfigureListener(ref consumerC, TestMethodC);

            //Consumer D
            var destinationD = BuildDestination("Consumer.D.VirtualTopic.Artemis.Test", Apache.NMS.DestinationType.Queue);
            var consumerD = (MessageConsumer)session.CreateConsumer(destinationD);

            ConfigureListener(ref consumerD, TestMethodD);

            //Producer which should produce to consumer A,B,C,D. I also tested with "VirtualTopic.Artemis.Test, multicast://VirtualTopic.Artemis.Test" as well
            var destination = BuildDestination("VirtualTopic.Artemis.Test, topic://Artemis.Test", Apache.NMS.DestinationType.Topic);
            var producer = (MessageProducer)session.CreateProducer(destination);

            Console.WriteLine("Producing message \"Test\" that should be delivered to consumers A,B,C,D");

            IMessage msg = producer.CreateTextMessage("Test");
            producer.Send(msg);

            while (true)
            {

            }
        }

        public static void ConfigureListener(ref MessageConsumer consumer, TextMessageHandler handler)
        {
            consumer.Stop();

            consumer.ConsumerInfo.Retroactive = false;
            consumer.Listener += (message) => Consumer_Listener(message, handler);

            consumer.Start();
        }

        public static IDestination BuildDestination(string destination, Apache.NMS.DestinationType destType)
        {
            switch (destType)
            {
                case Apache.NMS.DestinationType.Queue:

                    return new ActiveMQQueue(destination);

                default:
                case Apache.NMS.DestinationType.Topic:

                    return new ActiveMQTopic(destination);
            }
        }

        private static void Consumer_Listener(IMessage message, TextMessageHandler messageHandler)
        {
            if (message is ActiveMQTextMessage && messageHandler != null)
            {
                ActiveMQTextMessage actualMessage = (ActiveMQTextMessage)message;

                messageHandler(actualMessage.Text, message);
            }
        }

        private static void TestMethodA(string message, IMessage info)
        {
            Console.WriteLine($"A: {message}, Destination: {info.NMSDestination}");
        }

        private static void TestMethodB(string message, IMessage info)
        {
            Console.WriteLine($"B: {message}, Destination: {info.NMSDestination}");
        }

        private static void TestMethodC(string message, IMessage info)
        {
            Console.WriteLine($"C: {message}, Destination: {info.NMSDestination}");
        }

        private static void TestMethodD(string message, IMessage info)
        {
            Console.WriteLine($"D: {message}, Destination: {info.NMSDestination}");
        }
    }
}
