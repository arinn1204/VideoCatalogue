﻿Feature: AClientWantsToViewInformationAboutAFile

@Matroska
Scenario: A Client wants to view information about an MKV
	Given an MKV file named small.mkv
	When the information about the file is requested
	Then the information is returned about small.mkv
