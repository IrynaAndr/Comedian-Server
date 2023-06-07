using System;
using System.Collections.Generic;
using System.Threading;

class JokeServer
{
    private static int bufferSize;
    private static int numComedians;
    private static int sophistication;
    private static string newestJoke;
    private static Queue<string> jokeBuffer;
    private static Semaphore comedianSemaphore;
    private static Semaphore audienceSemaphore;

    static void Comedian(object comedianId)
    {
        Random rand = new Random();
        int id = (int)comedianId;
        int counter  = 1;
        while (true)
        {
            if (jokeBuffer.Count <= bufferSize)
            {
                string joke = $"Joke #{counter} by comedian {id}";
                comedianSemaphore.WaitOne();
                lock (jokeBuffer)
                {
                    jokeBuffer.Enqueue(joke);
                    counter++;
                    newestJoke = joke;
                    Console.WriteLine($"Comedian {id} created new joke: {joke}. (Buffer: {jokeBuffer.Count})");
                }
                audienceSemaphore.Release(); //Notify the audience that a joke is available
                Thread.Sleep(rand.Next(sophistication * 1000));
            }
        }
    }

    static void GreedyAudience()
    {
        string joke;
        while (true)
        {
            audienceSemaphore.WaitOne(); //Wait for a joke
            lock (jokeBuffer)
            {
                joke = jokeBuffer.Dequeue();
            }
            comedianSemaphore.Release(); //Notify comedians that a buffer slot is available
            Console.WriteLine($"-Greedy receive the joke: {joke}. (Buffer: {jokeBuffer.Count})");
            //Thread.Sleep(1000);
        }
    }

    static void FreshJokeAudience()
    {
        string joke;
        while (true)
        {
            audienceSemaphore.WaitOne(); 
            bool taken = false;
            lock (jokeBuffer)
            {
                joke = jokeBuffer.Dequeue();
                if (joke == newestJoke)
                {
                    Console.WriteLine($"--Fresh joke audience received joke: {joke}. (Buffer: {jokeBuffer.Count}) ");
                    taken = true;
                }
                else
                {
                    jokeBuffer.Enqueue(joke);
                }
            }
            if (taken)
            {
                comedianSemaphore.Release(); 
            }
            else
            {
                audienceSemaphore.Release();
            }
            //Thread.Sleep(1000);
        }
    }

    
    static void OtherAudience()
    {
        string joke;
        while (true)
        {
            bool taken = false;
            
            if (audienceSemaphore.WaitOne()) {
                //audienceSemaphore.WaitOne();
                //freshJokeMutex.WaitOne();
                lock (jokeBuffer)
                {
                    joke = jokeBuffer.Dequeue();
                    if (joke != newestJoke)
                    {
                        Console.WriteLine($"---Other audience received joke: {joke}.(Buffer: {jokeBuffer.Count})");
                        taken = true;
                    }
                    else
                    {
                        jokeBuffer.Enqueue(joke);
                    }
                   
                }
                if (taken)
                {
                    comedianSemaphore.Release();
                }
                else
                {
                    audienceSemaphore.Release();
                }
                //Thread.Sleep(1000);
            }
            
        }
    }
    

    static void Main()
    {
        Console.WriteLine("Enter BUFFER_SIZE:");
        bufferSize = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter Number of Comedians:");
        numComedians = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter Sophistication (max number of seconds to produce a joke):");
        sophistication = int.Parse(Console.ReadLine());

        jokeBuffer = new Queue<string>(bufferSize);
        comedianSemaphore = new Semaphore(bufferSize, bufferSize);
        audienceSemaphore = new Semaphore(0, bufferSize);

        // Create thread for each comedian
        for (int i = 1; i <= numComedians; i++)
        {
            Thread comedianThread = new Thread(Comedian);
            comedianThread.Start(i);
        }
        // Threads for audience members
        Thread greedyThread = new Thread(GreedyAudience);
        Thread freshJokeThread = new Thread(FreshJokeAudience);
        Thread unreceivedThread = new Thread(OtherAudience);
        greedyThread.Start();
        freshJokeThread.Start();
        unreceivedThread.Start();
    }
}
