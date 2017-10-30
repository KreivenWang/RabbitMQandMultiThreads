# 消息队列和多线程的学习
## 关于线程
- 非ui线程更新ui: 使用`SynchronizationContext`类的`Post`方法来更新ui
- 线程阻塞: `Thread.Sleep(Timeout.Infinite)`;

## 关于RabbitMQ
- 接收者需要一个阻塞的线程来维持其**监听**的功能
  - 控制台程序可使用`Console.ReadLine()`方法

## 接下来
1. 尝试多种类型的消息传输
