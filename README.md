# Comedian-Server
Joke server with multiple comedians, multiple audience members and bounded joke buffer.

	The server takes the following parameter
		BUFFER_SIZE  (INTEGER)  number of buffer slots, each capable of storing exactly one joke.
		NUM_COMEDIANS (INTEGER) number of processes/threads coming up with jokes
		SOPHISTICATION (INTEGER) a maximum of a random number of seconds that it takes for a comedian to come up with a new joke

	Clients connect to the server in one of the modes
		* greedy mode = take all jokes as they are produced
		* fresh joke mode = always take the most recently produced one
		* take whatever joke that has not been received yet 
-------------------------------------------------------------------------------------------------------------
Short description of solution:
	 Queue is used as JokeBuffer. When comedian or audience try to get or put something in buffer we lock this operation, so other threats can't access it and need to wait. 2 semaphores are used one for audience and one for Comedians to notify each member if resources are available. Or if new Joke was added/space for Joke available. Fresh Joke is updated when comedian put new and is stored additionally in separate variable. In the main program generate threats for comedians and 3 threat for each type of audience.
