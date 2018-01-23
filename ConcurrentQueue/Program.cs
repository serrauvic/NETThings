using System;
using System.Threading;

namespace ConcurrentQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            BlockingQueue<Message> _queue = new BlockingQueue<Message>();

            // Process message input queue.
            System.Threading.Tasks.Task.Run(() => {

                while (true)
                {
                    try
                    {
                        Message message = _queue.Take();

                        if (message != null)
                        {
                            // do something with message
                        }

                    } catch
                    {
                        //nyam nyam..
                    }
                }
            });

            // Send message to queue..
            System.Threading.Tasks.Task.Run(() => {

                while (true)
                {
                    try
                    {
                        Thread.Sleep(1000);

                        _queue.Add(new Message());

                    } catch
                    {
                        //nyam nyam..
                    }
                }                    

            });

            Console.ReadLine();
        }
    }
    class Message
    {
        //
    }
}
