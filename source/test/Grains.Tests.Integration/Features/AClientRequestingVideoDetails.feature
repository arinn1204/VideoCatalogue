﻿@VideoApi
Feature: AClientRequestingVideoDetails

	@TheMovieDatabase
	Scenario: A client that is inquiring about the avengers
		Given a client that is inquiring about The Avengers
		And the video was release in 2012
		When the client requests movie details
		Then the client is given the information about The Avengers

