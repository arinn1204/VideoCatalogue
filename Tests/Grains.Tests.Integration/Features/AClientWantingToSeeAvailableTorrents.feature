@BitTorrent
Feature: AClientWantingToSeeAvailableTorrents

  @Transmission
  Scenario: A client wants to see what torrents are currently seeding
    Given an active seeding torrent
    When the client goes to view the active torrents
    Then the client sees 1 seeding torrent