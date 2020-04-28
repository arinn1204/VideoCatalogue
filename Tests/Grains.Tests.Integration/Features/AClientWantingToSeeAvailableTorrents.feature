@BitTorrent
Feature: AClientWantingToSeeAvailableTorrents

  @Transmission
  Scenario: A client wants to see what torrents are currently seeding
    Given 1 active seeding torrent
    When the client goes to view the active torrents in transmission
    Then the client sees 1 active seeding torrent