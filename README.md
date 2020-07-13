# NMS-Reproduction
Reproduction of issue migrating VirtualTopics when migrating from ActiveMQ to Artemis

# H1
Instructions to reproduce

1. Have .NET Framework 4.8 SDK installed
2. Clone repository
3. Run the command "update-packages -reinstall" in NuGet package manager
4. Change the connection string in "MessageBusTest.cs" to your Artemis server address
5. Run!

I ran this on VS 2019, but it should work on a number of different platforms as long as they support .NET Framework.