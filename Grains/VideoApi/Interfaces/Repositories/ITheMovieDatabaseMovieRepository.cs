﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Grains.VideoApi.Models;

namespace Grains.VideoApi.Interfaces
{
	public interface ITheMovieDatabaseMovieRepository
	{
		Task<MovieCredit> GetMovieCredit(int movieId);
		Task<MovieDetail> GetMovieDetail(int movieId);
		IAsyncEnumerable<SearchResult> SearchMovie(string title, int? year = null);
	}
}