@VideoApi
Feature: AClientRequestingMovieDetails

	@TheMovieDatabase
	Scenario: A client that is inquiring about movies
		Given a client that is inquiring about The Avengers
		When the client requests movie details
		Then the client is given the information about The Avengers
