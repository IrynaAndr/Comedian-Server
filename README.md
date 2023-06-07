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
