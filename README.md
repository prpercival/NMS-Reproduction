# NMS-Reproduction
Reproduction of issue migrating VirtualTopics when migrating from ActiveMQ to Artemis. The issue is that when trying to recreate virtual topic logic in Artemis by having one topic produce to multiple queues, only one of the queues will actually be able to consume.

## Instructions to reproduce

1. Have .NET Framework 4.8 SDK installed
2. Clone repository and open the solution
3. Run the command "update-packages -reinstall" in NuGet package manager
4. Change the connection string in "MessageBusTest.cs" to your Artemis server address
5. Run the project!

I ran this on VS 2019, but it should work on a number of different platforms as long as they support .NET Framework.